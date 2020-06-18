using System;

namespace Microsoft.Reporting.Map.WebForms
{
	internal class MapBounds
	{
		public MapPoint MinimumPoint;

		public MapPoint MaximumPoint;

		public MapBounds(MapPoint minimumPoint, MapPoint maximumPoint)
		{
			MinimumPoint = minimumPoint;
			MaximumPoint = maximumPoint;
		}

		public static bool Intersect(MapBounds a, MapBounds b)
		{
			double num = a.MaximumPoint.X - a.MinimumPoint.X;
			double num2 = a.MaximumPoint.Y - a.MinimumPoint.Y;
			double num3 = b.MaximumPoint.X - b.MinimumPoint.X;
			double num4 = b.MaximumPoint.Y - b.MinimumPoint.Y;
			double num5 = Math.Max(a.MinimumPoint.X, b.MinimumPoint.X);
			double num6 = Math.Min(a.MinimumPoint.X + num, b.MinimumPoint.X + num3);
			double num7 = Math.Max(a.MinimumPoint.Y, b.MinimumPoint.Y);
			double num8 = Math.Min(a.MinimumPoint.Y + num2, b.MinimumPoint.Y + num4);
			if (num6 >= num5 && num8 >= num7)
			{
				return true;
			}
			return false;
		}
	}
}
