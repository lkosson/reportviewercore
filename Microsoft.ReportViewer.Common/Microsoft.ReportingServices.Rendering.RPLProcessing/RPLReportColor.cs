using System;
using System.Drawing;
using System.Text.RegularExpressions;

namespace Microsoft.ReportingServices.Rendering.RPLProcessing
{
	internal sealed class RPLReportColor
	{
		private const RegexOptions RegExOptions = RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture | RegexOptions.Compiled | RegexOptions.Singleline;

		private static Regex m_colorRegex = new Regex("^#(\\d|a|b|c|d|e|f){6}$", RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture | RegexOptions.Compiled | RegexOptions.Singleline);

		private string m_color;

		private Color m_GDIColor = Color.Empty;

		public RPLReportColor(string color)
		{
			m_color = color;
			ParseColor();
		}

		public override string ToString()
		{
			return m_color;
		}

		public Color ToColor()
		{
			return m_GDIColor;
		}

		private void ParseColor()
		{
			if (string.IsNullOrEmpty(m_color))
			{
				m_GDIColor = Color.Empty;
			}
			else if (m_color.Length == 7 && m_color[0] == '#' && m_colorRegex.Match(m_color).Success)
			{
				ColorFromArgb();
			}
			else
			{
				m_GDIColor = Color.FromName(m_color);
			}
		}

		private void ColorFromArgb()
		{
			try
			{
				int red = Convert.ToInt32(m_color.Substring(1, 2), 16);
				int green = Convert.ToInt32(m_color.Substring(3, 2), 16);
				int blue = Convert.ToInt32(m_color.Substring(5, 2), 16);
				m_GDIColor = Color.FromArgb(red, green, blue);
			}
			catch
			{
				m_GDIColor = Color.FromArgb(0, 0, 0);
			}
		}
	}
}
