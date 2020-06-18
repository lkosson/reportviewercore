using System.Drawing;

namespace Microsoft.Reporting.Gauge.WebForms
{
	internal class Position
	{
		private GaugeLocation location;

		private GaugeSize size;

		private ContentAlignment locationAlignment;

		public bool DefaultValues
		{
			get
			{
				if (location.DefaultValues)
				{
					return size.DefaultValues;
				}
				return false;
			}
		}

		internal RectangleF Rectangle
		{
			get
			{
				RectangleF result = new RectangleF(location, size);
				switch (locationAlignment)
				{
				case ContentAlignment.TopCenter:
					result.X -= size.Width / 2f;
					break;
				case ContentAlignment.TopRight:
					result.X -= size.Width;
					break;
				case ContentAlignment.MiddleLeft:
					result.Y -= size.Height / 2f;
					break;
				case ContentAlignment.MiddleCenter:
					result.X -= size.Width / 2f;
					result.Y -= size.Height / 2f;
					break;
				case ContentAlignment.MiddleRight:
					result.X -= size.Width;
					result.Y -= size.Height / 2f;
					break;
				case ContentAlignment.BottomLeft:
					result.Y -= size.Height;
					break;
				case ContentAlignment.BottomCenter:
					result.X -= size.Width / 2f;
					result.Y -= size.Height;
					break;
				case ContentAlignment.BottomRight:
					result.X -= size.Width;
					result.Y -= size.Height;
					break;
				}
				return result;
			}
		}

		public Position(GaugeLocation location, GaugeSize size, ContentAlignment locationAlignment)
		{
			this.location = location;
			this.size = size;
			this.locationAlignment = locationAlignment;
		}
	}
}
