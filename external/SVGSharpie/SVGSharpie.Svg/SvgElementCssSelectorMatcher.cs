using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml.Serialization;
using SVGSharpie.Css;

namespace SVGSharpie
{
    internal sealed class SvgElementCssSelectorMatcher : CssSelectorVisitor<bool>
    {
        private readonly SvgElement _element;

        protected override bool AggregateResult(bool a, bool b) => a & b;

        public SvgElementCssSelectorMatcher(SvgElement element) 
            => _element = element ?? throw new ArgumentNullException(nameof(element));

        public override bool VisitUniversalSelector(CssSimpleSelector selector) 
            => true;

        public override bool VisitTypeSelector(CssSimpleSelector selector)
            => selector.Name.LocalName == GetTypeName(_element.GetType());

        public override bool VisitIdSelector(CssSimpleSelector selector)
            => _element.Id == selector.Name.LocalName;

        public override bool VisitClassSelector(CssSimpleSelector selector)
            => _element.ClassNames.Any(i => i == selector.Name.LocalName);

        public override bool VisitAttributeSelector(CssAttributeSelector selector)
        {
            if (selector.Operation != CssAttributeSelectorOperation.Exact)
            {
                throw new NotImplementedException($"Attribute selector operation {selector.Operation}");
            }

            // todo: attributes can automatically be built here based on the XML serialization attributes
            //       only checking for a single attribute for now to make a unit test pass

            if (selector.AttributeName == "id")
            {
                return _element.Id == selector.AttributeValue;
            }

            throw new NotImplementedException();
        }

        public override bool VisitCompoundSelector(CssCompoundSelector selector) 
            => selector.Selectors.All(item => item.Accept(this));

        public override bool VisitComplexSelector(CssComplexSelector selector)
        {
            if (_element.IsGenerated)
            {
                // https://www.w3.org/TR/SVG/struct.html
                // Rules 7 and 8: CSS selectors only apply to the formal document tree, not on the generated tree;
                return false;
            }

            var items = selector.Items;
            var lastItem = items[items.Length - 1];
            if (!lastItem.Selector.Accept(this))
            {
                return false;
            }
            
            return MatchesCombinatorSelectorSequenceStartingAt(_element, items, items.Length - 2);
        }

        /// <summary>
        /// Gets a value indicating whether the specified element matches the combinator sequence at the starting index 
        /// and all the preceeding nodes of it
        /// </summary>
        private static bool MatchesCombinatorSelectorSequenceStartingAt(SvgElement element, CssComplexSelectorItem[] selectors, int index)
        {
            if (index < 0)
            {
                return true;
            }
            var selector = selectors[index];
            foreach (var possible in GetPossibleCombinatorialMatches(selector, element))
            {
                if (MatchesCombinatorSelectorSequenceStartingAt(possible, selectors, index - 1))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Gets all the possible matches for the combinator complex selector, for example the next sibling 
        /// selector 'h1 ~ pre' must match all 'h1' elements preceeding the current 'pre' element.  We want to 
        /// then test all previous combinator selectors using the matching element.
        /// </summary>
        private static IEnumerable<SvgElement> GetPossibleCombinatorialMatches(CssComplexSelectorItem item, SvgElement current)
        {
            var selector = item.Selector;
            switch (item.Combinator)
            {
                case CssCombinatorType.Descendant:
                    var p = current.Parent;
                    while (p != null)
                    {
                        if (selector.Accept(new SvgElementCssSelectorMatcher(p)))
                        {
                            yield return p;
                        }
                        p = p.Parent;
                    }
                    break;
                case CssCombinatorType.Child:
                    if (current.Parent == null)
                    {
                        yield break;
                    }
                    if (selector.Accept(new SvgElementCssSelectorMatcher(current.Parent)))
                    {
                        yield return current.Parent;
                    }
                    break;
                case CssCombinatorType.NextSibling:
                    SvgElement prevElement = null;
                    foreach (var child in current.Parent.GetChildren())
                    {
                        if (child == current)
                        {
                            if (prevElement == null)
                            {
                                yield break;
                            }
                            if (selector.Accept(new SvgElementCssSelectorMatcher(prevElement)))
                            {
                                yield return prevElement;
                            }
                            break;
                        }
                        prevElement = child;
                    }
                    break;
                case CssCombinatorType.SubsequentSibling:
                    foreach (var child in current.Parent.GetChildren())
                    {
                        if (child == current)
                        {
                            yield break;
                        }
                        if (selector.Accept(new SvgElementCssSelectorMatcher(child)))
                        {
                            yield return child;
                        }
                    }
                    break;
                case null:
                    throw new InvalidOperationException("Unexpected combinator is null");
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private static string GetTypeName(Type type)
            => type.GetCustomAttribute<XmlTypeAttribute>().TypeName;
    }
}
