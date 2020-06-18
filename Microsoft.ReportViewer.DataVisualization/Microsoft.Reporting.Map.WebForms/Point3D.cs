using System.Drawing;

namespace Microsoft.Reporting.Map.WebForms
{
	internal struct Point3D
	{
		public double X;

		public double Y;

		public double Z;

		public Point3D(double x, double y, double z)
		{
			X = x;
			Y = y;
			Z = z;
		}

		public PointF ToPointF()
		{
			return new PointF((float)X, (float)Y);
		}
	}
}
