using System.Collections.Generic;
using System.Text;

namespace SVGSharpie.Css
{
    internal static class CssStyleRulesParser
    {
        public static bool TryParseRules(CssStringStreamReader reader, out CssStyleRulesList rules)
        {
            rules = new CssStyleRulesList();
            reader.SkipWhitespaceAndComments();
            while (!reader.IsEndOfStream)
            {
                if (!TryParseSelectorList(reader, out var selectors) ||
                    !reader.SkipWhitespaceAndComments() ||
                    !reader.Read('{') ||
                    !TryParseStyleRuleBodyProperties(reader, out var properties) ||
                    !reader.SkipWhitespaceAndComments() ||
                    !reader.Read('}'))
                {
                    return false;
                }

                rules.Add(new CssStyleRule(selectors, properties));
                reader.SkipWhitespaceAndComments();
            }
            return true;
        }

        /// <summary>
        /// Tries to parse a collection of properties (name value pairs, e.g. 'fill:#000') of a style body
        /// </summary>
        public static bool TryParseStyleRuleBodyProperties(CssStringStreamReader reader, out Dictionary<string, CssStylePropertyValue> properties)
        {
            properties = new Dictionary<string, CssStylePropertyValue>();

            reader.SkipWhitespaceAndComments();

            var builder = new StringBuilder();

            while (!reader.IsEndOfStream && reader.CurrentChar != '}')
            {
                if (!reader.TryReadIdentifier(out var name))
                {
                    return false;
                }

                reader.SkipWhitespaceAndComments();
                if (!reader.Read(':'))
                {
                    return false;
                }
                reader.SkipWhitespaceAndComments();

                while (reader.CurrentChar != ';' && reader.CurrentChar != '}' && !reader.IsEndOfStream)
                {
                    builder.Append(reader.CurrentChar);
                    reader.Advance();
                }

                properties[name] = new CssStylePropertyValue(builder.ToString());
                builder.Clear();

                if (reader.CurrentChar == ';')
                {
                    reader.Read(';');
                }

                reader.SkipWhitespaceAndComments();
            }

            return true;
        }

        public static bool TryParseSelectorList(CssStringStreamReader reader, out CssSelectorList selectors)
        {
            selectors = new CssSelectorList();
            while (!reader.IsEndOfStream)
            {
                if (!TryParseComplexSelector(reader, out var complex))
                {
                    break;
                }
                selectors.Add(Flatten(complex));
                reader.SkipWhitespaceAndComments();
                if (reader.CurrentChar != ',')
                {
                    break;
                }
                reader.Advance();
            }
            return selectors.Count > 0;
        }

        private static bool TryParseComplexSelector(CssStringStreamReader reader, out CssComplexSelector selector)
        {
            var items = new List<CssComplexSelectorItem>();
            while (!reader.IsEndOfStream)
            {
                if (!TryParseCompoundSelector(reader, out var compound))
                {
                    if (items.Count > 0)
                    {
                        var last = items[items.Count - 1];
                        if (last.Combinator == CssCombinatorType.Descendant)
                        {
                            // remove the last items combinator
                            items[items.Count - 1] = new CssComplexSelectorItem(last.Selector, null);
                        }
                        else
                        {
                            items.Clear();
                        }
                    }
                    break;
                }

                var hasCombinator = reader.TryReadCombinatorType(out var combinator);

                items.Add(hasCombinator
                    ? new CssComplexSelectorItem(compound, combinator)
                    : new CssComplexSelectorItem(compound, null));

                if (!hasCombinator)
                {
                    break;
                }
            }
            selector = items.Count > 0 ? new CssComplexSelector(items.ToArray()) : null;
            return selector != null;
        }

        private static bool TryParseCompoundSelector(CssStringStreamReader reader, out CssCompoundSelector selector)
        {
            var simpleSelectors = new List<CssSimpleSelector>();
            reader.SkipWhitespaceAndComments();
            while (!reader.IsEndOfStream)
            {
                if (!TryParseSimpleSelector(reader, out var simpleSelector))
                {
                    break;
                }
                simpleSelectors.Add(simpleSelector);
                if (IsCombinatorChar(reader.CurrentChar))
                {
                    break;
                }
            }
            if (simpleSelectors.Count == 0)
            {
                selector = null;
                return false;
            }
            selector = new CssCompoundSelector(simpleSelectors.ToArray());
            return true;
        }

        private static bool TryParseSimpleSelector(CssStringStreamReader reader, out CssSimpleSelector selector)
        {
            selector = null;
            reader.SkipWhitespaceAndComments();

            switch (reader.CurrentChar)
            {
                case '#':
                    reader.Advance();
                    return
                        reader.TryReadIdentifier(out var id) &&
                        TryParseSimpleSelector(reader, CssSimpleSelectorType.IdSelector, new CssQualifiedName(id), out selector);
                case '.':
                    reader.Advance();
                    return
                        reader.TryReadIdentifier(out var className) &&
                        TryParseSimpleSelector(reader, CssSimpleSelectorType.ClassSelector, new CssQualifiedName(className), out selector);
                case '*':
                    reader.Advance();
                    return TryParseUniversalSelector(reader, out selector);
                case '[':
                case ':':
                    return TryParseUniversalSelector(reader, out selector);
                default:
                    return
                        reader.TryReadQualifiedName(out var typeName) &&
                        TryParseSimpleSelector(reader, CssSimpleSelectorType.TypeSelector, typeName, out selector);
            }
        }

        private static bool TryParseUniversalSelector(CssStringStreamReader reader, out CssSimpleSelector selector)
            => TryParseSimpleSelector(reader, CssSimpleSelectorType.UniversalSelector, new CssQualifiedName("*"), out selector);

        private static bool TryParseSimpleSelector(CssStringStreamReader reader, CssSimpleSelectorType simpleSelectorType, CssQualifiedName name, out CssSimpleSelector selector)
        {
            var attributeSelectors = ParseAttributeSelectors(reader);
            var pseudoClasses = ParsePseudoClasses(reader);
            if (attributeSelectors == null || pseudoClasses == null)
            {
                selector = null;
                return false;
            }
            selector = new CssSimpleSelector(simpleSelectorType, name, attributeSelectors, pseudoClasses);
            return true;
        }

        private static CssPseudoClass[] ParsePseudoClasses(CssStringStreamReader reader)
        {
            var classes = new List<CssPseudoClass>();
            while (reader.CurrentChar == ':')
            {
                reader.Advance();
                if (!reader.TryReadIdentifier(out var className))
                {
                    return null;
                }
                if (CssFunctionalPseudoClass.TryConvertToFunctionalPseudoClassType(className, out var functionalClassType))
                {
                    if (!reader.Read('(') ||
                        !TryParseSelectorList(reader, out var functionalSelectors) ||
                        !reader.Read(')'))
                    {
                        return null;
                    }
                    classes.Add(new CssFunctionalPseudoClass(functionalSelectors, functionalClassType));
                    continue;
                }
                if (CssUserActionPseudoClass.TryConvertToUserActionPseudoClassType(className, out var userActionClassType))
                {
                    classes.Add(new CssUserActionPseudoClass(userActionClassType));
                    continue;
                }

                if (CssStructuralPseudoClass.TryConvertToStructuralPseudoClassType(className, out var structuralPseudoClassType))
                {
                    if (!reader.Read('(') ||
                        !TryParseSelectorList(reader, out var structuralSelectors) ||
                        !reader.Read(')'))
                    {
                        structuralSelectors = new CssSelectorList();
                    }

                    classes.Add(new CssStructuralPseudoClass(structuralSelectors, structuralPseudoClassType));
                    continue;
                }
                return null;    // unknown / unhandled pseudo class
            }
            return classes.ToArray();
        }

        private static CssAttributeSelector[] ParseAttributeSelectors(CssStringStreamReader reader)
        {
            var selectors = new List<CssAttributeSelector>();
            while (reader.CurrentChar == '[')
            {
                reader.Advance();
                if (!reader.TryReadIdentifier(out var name) || !reader.TryReadCssAttributeSelectorOperation(out var op))
                {
                    return null;
                }

                var value = string.Empty;
                var wasString = false;
                if (op != CssAttributeSelectorOperation.Has)
                {
                    if (!reader.TryReadIdentifierOrString(out value, out wasString))
                    {
                        return null;
                    }
                }

                reader.SkipWhitespaceAndComments();

                var explicitIgnoreCase = reader.CurrentChar == 'i';
                if (explicitIgnoreCase)
                {
                    reader.Advance();
                    reader.SkipWhitespaceAndComments();
                }

                if (reader.CurrentChar != ']')
                {
                    return null;
                }

                reader.Advance();
                var valueType = wasString ? CssAttributeSelectorValueType.String : CssAttributeSelectorValueType.Identifier;
                selectors.Add(new CssAttributeSelector(name, value, op, valueType, explicitIgnoreCase));
            }
            return selectors.ToArray();
        }

        private static CssSelector Flatten(CssComplexSelector complex)
        {
            if (complex.Items.Length != 1)
            {
                return complex;
            }
            var compound = complex.Items[0].Selector;
            if (compound.Selectors.Length == 1)
            {
                return compound.Selectors[0];
            }
            return compound;
        }

        private static bool IsCombinatorChar(char c)
            => c == ' ' || c == '>' || c == '+' || c == '~';
    }
}