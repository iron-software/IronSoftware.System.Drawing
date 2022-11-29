using System;
using System.Text;

namespace SVGSharpie.Css
{
    internal sealed class CssStringStreamReader
    {
        private readonly StringBuilder _builder = new StringBuilder();

        public string Stream { get; }

        public int Position { get; private set; }

        public bool IsEndOfStream => Position >= Stream.Length;

        public char CurrentChar => !IsEndOfStream ? Stream[Position] : '\0';

        public char NextChar => Position + 1 < Stream.Length ? Stream[Position + 1] : '\0';

        public CssStringStreamReader(string stream, int index = 0)
        {
            Stream = stream ?? throw new ArgumentNullException(nameof(stream));
            Position = index;
        }

        public bool Read(char c)
        {
            if (CurrentChar != c)
            {
                return false;
            }
            Advance();
            return true;
        }

        public bool Advance()
        {
            Position++;
            return IsEndOfStream;
        }

        public bool TryReadCombinatorType(out CssCombinatorType combinatorType)
        {
            var isDescendant = CurrentChar == ' ';
            if (isDescendant)
            {
                SkipWhitespaceAndComments();
            }

            switch (CurrentChar)
            {
                case '>' when NextChar == '>':
                    Position += 2;
                    combinatorType = CssCombinatorType.Descendant;
                    return true;
                case '>':
                    Position++;
                    combinatorType = CssCombinatorType.Child;
                    return true;
                case '+':
                    Position++;
                    combinatorType = CssCombinatorType.NextSibling;
                    return true;
                case '~':
                    Position++;
                    combinatorType = CssCombinatorType.SubsequentSibling;
                    return true;
                default:
                    combinatorType = CssCombinatorType.Descendant;
                    return isDescendant;
            }
        }

        public bool TryReadCssAttributeSelectorOperation(out CssAttributeSelectorOperation operation)
        {
            switch (CurrentChar)
            {
                case ']':
                    operation = CssAttributeSelectorOperation.Has;
                    return true;
                case '=':
                    Position++;
                    operation = CssAttributeSelectorOperation.Exact;
                    return true;
                case '~' when NextChar == '=':
                    Position += 2;
                    operation = CssAttributeSelectorOperation.WordExact;
                    return true;
                case '|' when NextChar == '=':
                    Position += 2;
                    operation = CssAttributeSelectorOperation.ExactOrHyphenatedPrefix;
                    return true;
                case '^' when NextChar == '=':
                    Position += 2;
                    operation = CssAttributeSelectorOperation.Prefix;
                    return true;
                case '$' when NextChar == '=':
                    Position += 2;
                    operation = CssAttributeSelectorOperation.Suffix;
                    return true;
                case '*' when NextChar == '=':
                    Position += 2;
                    operation = CssAttributeSelectorOperation.Contains;
                    return true;
                default:
                    operation = CssAttributeSelectorOperation.Exact;
                    return false;
            }
        }

        public bool TryReadQualifiedName(out CssQualifiedName qualifiedName)
        {
            qualifiedName = null;
            if (!TryReadIdentifier(out var name1))
            {
                return false;
            }
            if (CurrentChar == '|')
            {
                if (TryReadIdentifier(out var name2))
                {
                    qualifiedName = new CssQualifiedName(name1, name2);
                }
            }
            else
            {
                qualifiedName = new CssQualifiedName("", name1);
            }
            return qualifiedName != null;
        }

        public bool TryReadIdentifierOrString(out string id, out bool wasString)
        {
            if (CurrentChar == '"')
            {
                Position++;
                wasString = true;
                if (!TryReadIdentifier(out id) || CurrentChar != '"')
                {
                    return false;
                }
                Position++;
                return true;
            }
            wasString = false;
            return TryReadIdentifier(out id);
        }

        public bool TryReadIdentifier(out string identifier)
        {
            _builder.Clear();

            while (!IsEndOfStream)
            {
                var c = CurrentChar;
                if (char.IsLetterOrDigit(c) || c == '-' || c == '_')
                {
                    Position++;
                    _builder.Append(c);
                    continue;
                }
                break;
            }

            identifier = _builder.ToString();
            return identifier.Length > 0;
        }

        public bool SkipWhitespaceAndComments()
        {
            var inComment = false;
            while (!IsEndOfStream)
            {
                if (!inComment)
                {
                    if (CurrentChar == '/' && NextChar == '*')
                    {
                        inComment = true;
                        Position++;
                    }
                    else if (!char.IsWhiteSpace(CurrentChar))
                    {
                        break;
                    }
                }
                else if (CurrentChar == '*' && NextChar == '/')
                {
                    Position++;
                    inComment = false;
                }
                Position++;
            }
            return !IsEndOfStream;
        }
    }
}