using Microsoft.ReportingServices.Rendering.RPLProcessing;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Microsoft.ReportingServices.Rendering.ImageRenderer
{
	internal sealed class GDIPen
	{
		private GDIPen()
		{
		}

		private static string GetKey(Color color, float size, RPLFormat.BorderStyles style)
		{
			string text = color.ToString() + size;
			if ((int)(style & RPLFormat.BorderStyles.Dashed) > 0)
			{
				text += "s";
			}
			if ((int)(style & RPLFormat.BorderStyles.Dotted) > 0)
			{
				text += "t";
			}
			if ((int)(style & RPLFormat.BorderStyles.Solid) > 0)
			{
				text += "d";
			}
			return text;
		}

		internal static Pen GetPen(Dictionary<string, Pen> pens, Color color, float size, RPLFormat.BorderStyles style)
		{
			string key = GetKey(color, size, style);
			if (pens.TryGetValue(key, out Pen value))
			{
				return value;
			}
			value = new Pen(color, size);
			switch (style)
			{
			case RPLFormat.BorderStyles.Dashed:
				value.DashStyle = DashStyle.Dash;
				break;
			case RPLFormat.BorderStyles.Dotted:
				value.DashStyle = DashStyle.Dot;
				break;
			}
			pens.Add(key, value);
			return value;
		}
	}
}
