using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;
using SVGSharpie.Css;

namespace SVGSharpie
{
    /// <summary>
    /// All of the SVG DOM interfaces that correspond directly to elements in the SVG language derive from the SVGElement interface.
    /// </summary>
    public abstract class SvgElement
    {
        [XmlIgnore]
        public virtual SvgElement Parent
        {
            get => _parent;
            internal set
            {
                if (_parent != null && value != null)
                {
                    throw new Exception("The element must first be detached from its current parent");
                }
                _parent = value;
                if (_parent != null)
                {
                    OnAttachedToParent();
                }
                else
                {
                    OnDetachedFromParent();
                }
            }
        }

        /// <summary>
        /// Gets the parent <see cref="SvgSvgElement"/> of the currentElement
        /// </summary>
        internal SvgSvgElement ParentSvg => Parent is SvgSvgElement svg ? svg : Parent?.ParentSvg;

        /// <summary>
        /// Gets the CSS style declarations of the current element
        /// </summary>
        [XmlIgnore]
        public virtual ISvgElementComputedStyle Style => _computedStyle;

        /// <summary>
        /// Gets or sets the value of the id attribute on the given element, or the empty string if id is not present.
        /// </summary>
        [XmlAttribute("id")]
        public string Id { get; set; } = string.Empty;

        /// <summary>
        /// Gets a value indicating whether the element partakes in the rendering tree
        /// </summary>
        [XmlIgnore]
        internal virtual bool PartakesInRenderingTree => _explicitStyle.Display != CssDisplayType.None && !HasParentOfType<SvgDefsElement>();

        /// <summary>
        /// Gets a value indicating whether the current element is a part of the generated document tree (not the 
        /// formal declared document tree).  For example in a <see cref="SvgUseElement">use</see> element.
        /// </summary>
        [XmlIgnore]
        public bool IsGenerated => Parent is SvgUseElement || (Parent?.IsGenerated ?? false);

        /// <summary>
        /// Gets the CSS style declarations of the current element
        /// </summary>
        [XmlAttribute("style")]
        public string StyleAsString
        {
            get => _explicitStyle.ToString();
            set => _explicitStyle.Populate(value);
        }

        /// <summary>
        /// Gets the collection of CSS class names of the current element
        /// </summary>
        public List<string> ClassNames { get; } = new List<string>();

        /// <summary>
        /// Gets or sets the the 'class' attribute on the current element
        /// </summary>
        [XmlAttribute("class")]
        public string ClassNamesAsString
        {
            get => string.Join(" ", ClassNames);
            set
            {
                ClassNames.Clear();
                ClassNames.AddRange((value ?? string.Empty).Split(ClassNameSplitters, StringSplitOptions.RemoveEmptyEntries));
            }
        }

        [XmlAnyAttribute]
        public XmlAttribute[] XAttributes
        {
            get => _xAttributes;
            set
            {
                if ((_xAttributes = value) != null)
                {
                    Array.ForEach(value, attr => OnAnyAttribute(attr.LocalName, attr.Value));
                }
            }
        }

        /// <summary>
        /// Gets the bounding box of the current element, which is the tightest fitting rectangle aligned with the axes 
        /// of that element's user coordinate system that entirely encloses it and its descendants.
        /// </summary>
        /// <returns>the current bounding box of the element</returns>
        public abstract SvgRect? GetBBox();

        /// <summary>
        /// Calls the appropriate Visit method on the <see cref="SvgElementVisitor">visitor</see> specified.
        /// </summary>
        /// <param name="visitor">visitor to visit</param>
        public abstract void Accept(SvgElementVisitor visitor);

        /// <summary>
        /// Calls the appropriate Visit method on the <see cref="SvgElementVisitor">visitor</see> specified
        /// and returns the result.
        /// </summary>
        /// <param name="visitor">visitor to visit</param>
        /// <returns>result of the visitor visit method call</returns>
        public abstract TResult Accept<TResult>(SvgElementVisitor<TResult> visitor);

        internal SvgElement()
        {
            _explicitStyle = new SvgElementStyleData();
            _styleSheetComputedStyle = new SvgElementComputedStyle(EmptyStyleDatas, SvgElementDefaultStyle.Instance);
            _computedStyle = new SvgElementComputedStyle(_explicitStyle, _styleSheetComputedStyle);
        }

        internal SvgElement DeepClone()
        {
            var clone = CreateClone();
            PopulateClone(clone);
            return clone;
        }

        internal virtual IEnumerable<SvgElement> GetChildren() => EmptyElements;

        internal SvgElement FindElementById(string id)
        {
            return Id == id ? this : GetChildren().Select(child => child.FindElementById(id)).FirstOrDefault(found => found != null);
        }

        internal virtual IEnumerable<SvgElement> Descendants()
        {
            foreach (var child in GetChildren())
            {
                yield return child;
                foreach (var descendant in child.Descendants())
                {
                    yield return descendant;
                }
            }
        }

        protected abstract SvgElement CreateClone();

        protected virtual void PopulateClone(SvgElement element)
        {
            element.Id = Id;
            _explicitStyle.CopyTo(element._explicitStyle);
            element._xAttributes = _xAttributes;
            element.ClassNames.AddRange(ClassNames);
        }

        /// <summary>
        /// Called when the <see cref="SvgDocument"/> tree the element resides in has been completely loaded
        /// </summary>
        internal virtual void OnLoaded()
        {
            ResolveAppliedStyles();
        }

        private void ResolveAppliedStyles()
        {
            var matcher = new SvgElementCssSelectorMatcher(this);
            var matching = new List<Tuple<CssSelector, int, SvgElementStyleData>>();
            var styleElements = ParentSvg?.StyleElements ?? Enumerable.Empty<SvgStyleElement>();
            var allStyleRules = styleElements.SelectMany(i => i.StyleRules);
            var index = 0;
            foreach (var styleRule in allStyleRules)
            {
                var matchingSelectors = styleRule.Selectors.Where(i => i.Accept(matcher));
                var matchingRules = matchingSelectors.Select(i => Tuple.Create(i, index++, styleRule.Rules));
                matching.AddRange(matchingRules);
            }
            if (matching.Count > 0)
            {
                var matchingStylesBySpecificity = matching
                    .OrderByDescending(i => i.Item1.Specificity.Total)
                    .ThenByDescending(i => i.Item2)
                    .Select(i => i.Item3).ToArray();
                _styleSheetComputedStyle.StyleDatas = matchingStylesBySpecificity;
            }
            else
            {
                _styleSheetComputedStyle.StyleDatas = EmptyStyleDatas;
            }
        }

        protected virtual void OnAttachedToParent()
        {
            _styleSheetComputedStyle.Parent = Parent.Style;
        }

        protected virtual void OnDetachedFromParent()
        {
            _styleSheetComputedStyle.Parent = SvgElementDefaultStyle.Instance;
        }

        protected virtual void OnAnyAttribute(string name, string value)
        {
            // nop...
        }

        private bool HasParentOfType<T>()
            where T : SvgElement
        {
            var p = Parent;
            while (p != null)
            {
                if (p is T)
                {
                    return true;
                }
                p = p.Parent;
            }
            return false;
        }

        private XmlAttribute[] _xAttributes;

        private SvgElement _parent;

        private readonly SvgElementComputedStyle _computedStyle;

        private readonly SvgElementComputedStyle _styleSheetComputedStyle;

        private readonly SvgElementStyleData _explicitStyle;

        protected static readonly SvgElement[] EmptyElements = new SvgElement[0];

        private static readonly char[] ClassNameSplitters = { ' ' };

        private static readonly SvgElementStyleData[] EmptyStyleDatas = new SvgElementStyleData[0];
    }
}
