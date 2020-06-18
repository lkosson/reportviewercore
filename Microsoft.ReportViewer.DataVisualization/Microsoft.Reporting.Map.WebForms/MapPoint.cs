using System.ComponentModel;
using System.Drawing;
using System.Globalization;

namespace Microsoft.Reporting.Map.WebForms
{
	[TypeConverter(typeof(MapPointConverter))]
	internal struct MapPoint
	{
		private double x;

		private double y;

		[SRDescription("DescriptionAttributeMapPoint_X")]
		public double X
		{
			get
			{
				return x;
			}
			set
			{
				x = value;
			}
		}

		[SRDescription("DescriptionAttributeMapPoint_Y")]
		public double Y
		{
			get
			{
				return y;
			}
			set
			{
				y = value;
			}
		}

		public MapPoint(double x, double y)
		{
			this.x = x;
			this.y = y;
		}

		public override string ToString()
		{
			return X.ToString(CultureInfo.CurrentCulture) + ", " + Y.ToString(CultureInfo.CurrentCulture);
		}

		public override bool Equals(object obj)
		{
			if (obj is MapPoint)
			{
				MapPoint mapPoint = (MapPoint)obj;
				if (mapPoint.X == X)
				{
					return mapPoint.Y == Y;
				}
			}
			return false;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public PointF ToPointF()
		{
			return new PointF((float)X, (float)Y);
		}
	}
}
