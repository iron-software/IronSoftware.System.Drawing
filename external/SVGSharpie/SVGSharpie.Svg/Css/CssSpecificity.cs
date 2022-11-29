using System;

namespace SVGSharpie.Css
{
    public struct CssSpecificity
    {
        private const int ClassBMultiplier = 0x100;
        private const int ClassAMultiplier = 0x10000;

        /// <summary>
        /// Gets the class A specificity, representing the number of ID selectors in the selector
        /// </summary>
        public int ClassA { get; }

        /// <summary>
        /// Gets the class B specificity, representing the number of class selectors, attributes selectors, and 
        /// pseudo-classes in the selector
        /// </summary>
        public int ClassB { get; }

        /// <summary>
        /// Gets the class C specificity, representing the number of type selectors and pseudo-elements in the selector
        /// </summary>
        public int ClassC { get; }

        /// <summary>
        /// Gets the total specificity
        /// </summary>
        public int Total =>
            ClassA * ClassAMultiplier +
            ClassB * ClassBMultiplier +
            ClassC;

        public CssSpecificity(int classA, int classB, int classC) : this()
        {
            ClassA = classA;
            ClassB = classB < ClassAMultiplier ? classB : throw new ArgumentOutOfRangeException(nameof(classB));
            ClassC = classC < ClassBMultiplier ? classC : throw new ArgumentOutOfRangeException(nameof(classC));
        }

        public static CssSpecificity operator +(CssSpecificity a, CssSpecificity b)
            => new CssSpecificity(a.ClassA + b.ClassA, a.ClassB + b.ClassB, a.ClassC + b.ClassC);

        public override string ToString() => $"A:{ClassA},B:{ClassB},C:{ClassC}={Total}";
    }
}