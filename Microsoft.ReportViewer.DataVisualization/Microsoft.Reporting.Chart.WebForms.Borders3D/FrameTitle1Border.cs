using System.Drawing;

namespace Microsoft.Reporting.Chart.WebForms.Borders3D
{
	internal class FrameTitle1Border : FrameThin1Border
	{
		public override string Name => "FrameTitle1";

		public override float Resolution
		{
			set
			{
				base.Resolution = value;
				sizeLeftTop = new SizeF(sizeLeftTop.Width, defaultRadiusSize * 2f);
			}
		}

		public FrameTitle1Border()
		{
			sizeLeftTop = new SizeF(sizeLeftTop.Width, defaultRadiusSize * 2f);
		}

		public override RectangleF GetTitlePositionInBorder()
		{
			return new RectangleF(defaultRadiusSize * 0.25f, defaultRadiusSize * 0.25f, defaultRadiusSize * 0.25f, defaultRadiusSize * 1.6f);
		}
	}
}
