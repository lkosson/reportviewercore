using System.Drawing;

namespace Microsoft.Reporting.Chart.WebForms.Borders3D
{
	internal class FrameTitle4Border : FrameThin4Border
	{
		public override string Name => "FrameTitle4";

		public override float Resolution
		{
			set
			{
				base.Resolution = value;
				sizeLeftTop = new SizeF(sizeLeftTop.Width, defaultRadiusSize * 2f);
			}
		}

		public FrameTitle4Border()
		{
			sizeLeftTop = new SizeF(sizeLeftTop.Width, defaultRadiusSize * 2f);
		}

		public override RectangleF GetTitlePositionInBorder()
		{
			return new RectangleF(defaultRadiusSize * 0.25f, defaultRadiusSize * 0.25f, defaultRadiusSize * 0.25f, defaultRadiusSize * 1.6f);
		}
	}
}
