using System;

namespace SVGSharpie
{
    public sealed class SvgPreserveAspectRatio
    {
        private static readonly char[] WhitespaceChars = { ' ', '\t', '\r', '\n' };

        /// <summary>
        /// Gets or sets a value indicating whether the 'preserveAspectRatio' flag of any referenced content should be used
        /// </summary>
        public bool Defer { get; set; }

        /// <summary>
        /// Gets or sets the alignment type
        /// </summary>
        public SvgPreserveAspectRatioAlign Align { get; set; } = SvgPreserveAspectRatioAlign.XMidYMid;

        /// <summary>
        /// Gets or sets the meet or slice type
        /// </summary>
        public SvgPreserveAspectRatioMeetOrSlice MeetOrSlice { get; set; } = SvgPreserveAspectRatioMeetOrSlice.Meet;

        public override string ToString()
            => $"{(Defer ? "defer " : string.Empty)}{Align.ToString()} {MeetOrSlice.ToString().ToLowerInvariant()}";

        public static SvgPreserveAspectRatio Parse(string value)
        {
            if (value == null) throw new ArgumentNullException(nameof(value));
            value = value.Trim();

            var result = new SvgPreserveAspectRatio();
            if (value.StartsWith("defer"))
            {
                result.Defer = true;
                value = value.Substring("defer".Length).TrimStart();
            }

            var wsIndex = value.IndexOfAny(WhitespaceChars);
            var alignAsStr = wsIndex < 0 ? value : value.Substring(0, wsIndex);

            if (!Enum.TryParse(alignAsStr, true, out SvgPreserveAspectRatioAlign align))
            {
                throw new Exception($"Invalid preserveAspectRatio alignment value '{align}'");
            }

            result.Align = align;

            if (wsIndex > 0)
            {
                var meetOrSliceAsStr = value.Substring(wsIndex + 1).Trim();
                if (!Enum.TryParse(meetOrSliceAsStr, true, out SvgPreserveAspectRatioMeetOrSlice meetOrSlice))
                {
                    throw new Exception($"Invalid preserveAspectRatio meet or slice value '{align}'");
                }
                result.MeetOrSlice = meetOrSlice;
            }
            
            return result;
        }
    }
}