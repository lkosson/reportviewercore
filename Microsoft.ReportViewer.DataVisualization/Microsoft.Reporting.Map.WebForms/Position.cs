using System.Drawing;

namespace Microsoft.Reporting.Map.WebForms
{
	internal class Position
	{
		private MapLocation location;

		private MapSize size;

		private ContentAlignment locationAlignment;

		public float X => location.X;

		public float Y => location.Y;

		public float Width => size.Width;

		public float Height => size.Height;

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

		public Position(MapLocation location, MapSize size, ContentAlignment locationAlignment)
		{
			this.location = location;
			this.size = size;
			this.locationAlignment = locationAlignment;
		}
	}
}
