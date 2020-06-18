using System.Drawing;

namespace Microsoft.Reporting.Map.WebForms
{
	internal class FrameTitle5Border : FrameThin5Border
	{
		public override string Name => "FrameTitle5";

		public FrameTitle5Border()
		{
			sizeLeftTop = new SizeF(sizeLeftTop.Width, defaultRadiusSize * 2f);
			drawScrews = true;
		}

		public override RectangleF GetTitlePositionInBorder()
		{
			return new RectangleF(defaultRadiusSize * 0.25f, defaultRadiusSize * 0.25f, defaultRadiusSize * 0.25f, defaultRadiusSize * 1.6f);
		}
	}
}
