using System.Drawing;
using System.Drawing.Drawing2D;

namespace Microsoft.Reporting.Map.WebForms
{
	internal class MarkerStyleAttrib
	{
		public GraphicsPath path;

		public Brush brush;

		public MarkerStyleAttrib()
		{
			path = null;
			brush = null;
		}

		public void Dispose()
		{
			if (path != null)
			{
				path.Dispose();
			}
			if (brush != null)
			{
				brush.Dispose();
			}
		}
	}
}
