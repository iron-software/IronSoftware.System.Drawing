using System;
using System.Collections.Generic;

namespace SVGSharpie.Css
{
    public class CssSelectorVisitor
    {
        public virtual void DefaultVisit(ICssSelector selector)
        {
        }

        public virtual void DefaultVisitList<T>(IEnumerable<T> selectors)
            where T: ICssSelector
        {
            if (selectors == null) return;
            foreach (var selector in selectors)
            {
                selector.Accept(this);
            }
        }

        public virtual void VisitClassSelector(CssSimpleSelector selector)
            => DefaultVisit(selector);

        public virtual void VisitIdSelector(CssSimpleSelector selector)
            => DefaultVisit(selector);

        public virtual void VisitTypeSelector(CssSimpleSelector selector)
            => DefaultVisit(selector);

        public virtual void VisitUniversalSelector(CssSimpleSelector selector)
            => DefaultVisit(selector);

        public virtual void VisitAttributeSelector(CssAttributeSelector selector)
            => DefaultVisit(selector);

        public virtual void VisitSimpleSelector(CssSimpleSelector selector)
        {
            switch (selector?.SimpleSelectorType)
            {
                case CssSimpleSelectorType.TypeSelector:
                    VisitTypeSelector(selector);
                    break;
                case CssSimpleSelectorType.UniversalSelector:
                    VisitUniversalSelector(selector);
                    break;
                case CssSimpleSelectorType.ClassSelector:
                    VisitClassSelector(selector);
                    break;
                case CssSimpleSelectorType.IdSelector:
                    VisitIdSelector(selector);
                    break;
                case null:
                    DefaultVisit(null);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(selector), $"Unknown simple selector type '{selector.SimpleSelectorType}'");
            }
            DefaultVisitList(selector?.AttributeSelectors);
        }

        public virtual void VisitCompoundSelector(CssCompoundSelector selector)
            => DefaultVisit(selector);

        public virtual void VisitComplexSelector(CssComplexSelector selector)
            => DefaultVisit(selector);
    }

    public abstract class CssSelectorVisitor<TResult>
    {
        protected abstract TResult AggregateResult(TResult a, TResult b);

        public virtual TResult DefaultVisit(ICssSelector selector)
            => default(TResult);

        public virtual TResult DefaultVisitList<T>(IEnumerable<T> selectors)
            where T : ICssSelector
        {
            if (selectors == null) return default(TResult);
            var result = default(TResult);
            var first = true;
            foreach (var selector in selectors)
            {
                var r = selector.Accept(this);
                result = first ? r : AggregateResult(result, r);
                first = false;
            }
            return result;
        }

        public virtual TResult VisitClassSelector(CssSimpleSelector selector)
            => DefaultVisit(selector);

        public virtual TResult VisitIdSelector(CssSimpleSelector selector)
            => DefaultVisit(selector);

        public virtual TResult VisitTypeSelector(CssSimpleSelector selector)
            => DefaultVisit(selector);

        public virtual TResult VisitUniversalSelector(CssSimpleSelector selector)
            => DefaultVisit(selector);

        public virtual TResult VisitAttributeSelector(CssAttributeSelector selector)
            => DefaultVisit(selector);

        public virtual TResult VisitSimpleSelector(CssSimpleSelector selector)
        {
            var simpleResult = VisitSpecificSimpleSelector(selector);
            var attributeSelectors = selector?.AttributeSelectors;
            return attributeSelectors == null || attributeSelectors.Count == 0
                ? simpleResult
                : AggregateResult(simpleResult, DefaultVisitList(attributeSelectors));
        }

        private TResult VisitSpecificSimpleSelector(CssSimpleSelector selector)
        {
            switch (selector?.SimpleSelectorType)
            {
                case CssSimpleSelectorType.TypeSelector:
                    return VisitTypeSelector(selector);
                case CssSimpleSelectorType.UniversalSelector:
                    return VisitUniversalSelector(selector);
                case CssSimpleSelectorType.ClassSelector:
                    return VisitClassSelector(selector);
                case CssSimpleSelectorType.IdSelector:
                    return VisitIdSelector(selector);
                case null:
                    return DefaultVisit(null);
                default:
                    throw new ArgumentOutOfRangeException(nameof(selector),
                        $"Unknown simple selector type '{selector.SimpleSelectorType}'");
            }
        }

        public virtual TResult VisitCompoundSelector(CssCompoundSelector selector)
            => DefaultVisit(selector);

        public virtual TResult VisitComplexSelector(CssComplexSelector selector)
            => DefaultVisit(selector);
    }
}