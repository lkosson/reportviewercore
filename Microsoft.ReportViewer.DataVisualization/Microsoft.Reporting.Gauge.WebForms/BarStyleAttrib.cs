using System.Drawing;
using System.Drawing.Drawing2D;

namespace Microsoft.Reporting.Gauge.WebForms
{
	internal class BarStyleAttrib
	{
		public GraphicsPath primaryPath;

		public Brush primaryBrush;

		public GraphicsPath[] secondaryPaths;

		public Brush[] secondaryBrushes;

		public GraphicsPath totalPath;

		public Brush totalBrush;

		public BarStyleAttrib()
		{
			primaryPath = null;
			primaryBrush = null;
			secondaryPaths = null;
			secondaryBrushes = null;
			totalPath = null;
			totalBrush = null;
		}

		public void Dispose()
		{
			if (primaryPath != null)
			{
				primaryPath.Dispose();
				primaryPath = null;
			}
			if (primaryBrush != null)
			{
				primaryBrush.Dispose();
				primaryBrush = null;
			}
			if (secondaryPaths != null)
			{
				GraphicsPath[] array = secondaryPaths;
				for (int i = 0; i < array.Length; i++)
				{
					array[i]?.Dispose();
				}
				secondaryPaths = null;
			}
			if (secondaryBrushes != null)
			{
				Brush[] array2 = secondaryBrushes;
				for (int i = 0; i < array2.Length; i++)
				{
					array2[i]?.Dispose();
				}
				secondaryBrushes = null;
			}
			if (totalPath != null)
			{
				totalPath.Dispose();
				totalPath = null;
			}
			if (totalBrush != null)
			{
				totalBrush.Dispose();
				totalBrush = null;
			}
		}
	}
}
