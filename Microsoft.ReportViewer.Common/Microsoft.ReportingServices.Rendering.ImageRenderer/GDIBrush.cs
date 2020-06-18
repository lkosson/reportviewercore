using System.Collections.Generic;
using System.Drawing;

namespace Microsoft.ReportingServices.Rendering.ImageRenderer
{
	internal sealed class GDIBrush
	{
		private GDIBrush()
		{
		}

		internal static Brush GetBrush(Dictionary<string, Brush> brushes, Color color)
		{
			string key = color.ToString();
			if (brushes.TryGetValue(key, out Brush value))
			{
				return value;
			}
			value = new SolidBrush(color);
			brushes.Add(key, value);
			return value;
		}
	}
}
