using System.Drawing;

namespace Microsoft.Reporting.Map.WebForms
{
	internal class FrameTitle6Border : FrameThin6Border
	{
		public override string Name => "FrameTitle6";

		public FrameTitle6Border()
		{
			sizeLeftTop = new SizeF(sizeLeftTop.Width, defaultRadiusSize * 2f);
		}

		public override RectangleF GetTitlePositionInBorder()
		{
			return new RectangleF(defaultRadiusSize * 0.25f, defaultRadiusSize * 0.25f, defaultRadiusSize * 0.25f, defaultRadiusSize * 1.6f);
		}
	}
}
