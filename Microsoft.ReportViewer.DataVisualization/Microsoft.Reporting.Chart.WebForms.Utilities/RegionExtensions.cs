using System.Drawing;

namespace Microsoft.Reporting.Chart.WebForms.Utilities
{
	internal static class RegionExtensions
	{
		public static bool IsSuperSetOf(this Region region1, Region region2, Graphics graphics)
		{
			if (region2 == null)
			{
				return true;
			}
			if (region1 == null)
			{
				return false;
			}
			Region region3 = region1.Clone();
			region3.Union(region2);
			return region3.Equals(region1, graphics);
		}
	}
}
