using System.Drawing;
using System.Drawing.Drawing2D;

namespace Microsoft.Reporting.Gauge.WebForms
{
	internal class KnobStyleAttrib
	{
		public GraphicsPath[] paths;

		public Brush[] brushes;

		public KnobStyleAttrib()
		{
			paths = null;
			brushes = null;
		}

		public void Dispose()
		{
			if (paths != null)
			{
				GraphicsPath[] array = paths;
				for (int i = 0; i < array.Length; i++)
				{
					array[i]?.Dispose();
				}
				paths = null;
			}
			if (brushes != null)
			{
				Brush[] array2 = brushes;
				for (int i = 0; i < array2.Length; i++)
				{
					array2[i]?.Dispose();
				}
				brushes = null;
			}
		}
	}
}
