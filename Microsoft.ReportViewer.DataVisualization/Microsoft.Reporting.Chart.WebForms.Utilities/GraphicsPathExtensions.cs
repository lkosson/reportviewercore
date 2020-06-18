using System.Drawing;
using System.Drawing.Drawing2D;

namespace Microsoft.Reporting.Chart.WebForms.Utilities
{
	internal static class GraphicsPathExtensions
	{
		public static bool IsSuperSetOf(this GraphicsPath path1, GraphicsPath path2, Graphics graphics)
		{
			if (path2 == null)
			{
				return true;
			}
			if (path1 == null)
			{
				return false;
			}
			using (Region region = new Region(path1))
			{
				using (Region region2 = new Region(path2))
				{
					return region.IsSuperSetOf(region2, graphics);
				}
			}
		}
	}
}
