using System;

namespace SVGSharpie.Css
{
    /// <summary>
    /// Represents a name explicitly located within (associated with) a namespace.  To form a qualified name in CSS 
    /// syntax, a namespace prefix that has been declared within scope is prepended to a local name (such as an element 
    /// or attribute name), separated by a "vertical bar"(|, U+007C). The prefix, representing the namespace for which 
    /// it has been declared, indicates the namespace of the local name.
    /// </summary>
    public sealed class CssQualifiedName
    {
        public const string UniversalNamespace = "*";

        /// <summary>
        /// Gets the namespace of the current qualified name or '*' for universal
        /// </summary>
        public string Namespace { get; }

        /// <summary>
        /// Gets the local name of the current qualified name
        /// </summary>
        public string LocalName { get; }

        public CssQualifiedName(string localName)
            : this(UniversalNamespace, localName)
        {
        }

        public CssQualifiedName(string ns, string localName)
        {
            Namespace = ns ?? throw new ArgumentNullException(nameof(ns));
            LocalName = localName ?? throw new ArgumentNullException(nameof(localName));
        }

        public override string ToString() => $"{Namespace}|{LocalName}";
    }
}