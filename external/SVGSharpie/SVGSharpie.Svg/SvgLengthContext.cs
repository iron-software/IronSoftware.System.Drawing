using System;

namespace SVGSharpie
{
    internal abstract class SvgLengthContext
    {
        public static SvgLengthContext Null { get; } = new NullContext();

        /// <summary>
        /// Calculates the total length in the direction of an <see cref="SvgLength"/>
        /// </summary>
        public abstract float ComputeTotalLength();
        
        private sealed class NullContext : SvgLengthContext
        {
            public override float ComputeTotalLength() => throw new Exception("Unable to compute total length");
        }
    }
}