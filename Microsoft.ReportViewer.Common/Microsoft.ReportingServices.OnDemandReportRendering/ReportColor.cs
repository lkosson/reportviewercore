using Microsoft.ReportingServices.Diagnostics.Utilities;
using Microsoft.ReportingServices.ReportProcessing;
using Microsoft.ReportingServices.ReportRendering;
using System.Drawing;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class ReportColor
	{
		private string m_color;

		private Color m_GDIColor = Color.Empty;

		private bool m_parsed;

		public ReportColor(string color)
			: this(color, allowTransparency: false)
		{
		}

		public ReportColor(string color, bool allowTransparency)
		{
			m_color = color;
			Validate(allowTransparency);
			m_parsed = true;
		}

		internal ReportColor(string color, Color gdiColor, bool parsed)
		{
			m_color = color;
			m_parsed = parsed;
			m_GDIColor = gdiColor;
		}

		internal ReportColor(Microsoft.ReportingServices.ReportRendering.ReportColor oldColor)
		{
			m_color = oldColor.ToString();
			m_parsed = oldColor.Parsed;
			if (m_parsed)
			{
				m_GDIColor = oldColor.ToColor();
			}
		}

		public override string ToString()
		{
			return m_color;
		}

		public Color ToColor()
		{
			if (!m_parsed)
			{
				Validator.ParseColor(m_color, out m_GDIColor, allowTransparency: false);
				m_parsed = true;
			}
			return m_GDIColor;
		}

		internal void Validate(bool allowTransparency)
		{
			if (!Validator.ValidateColor(m_color, out m_GDIColor, allowTransparency))
			{
				throw new RenderingObjectModelException(ErrorCode.rrInvalidColor, m_color);
			}
		}

		public static bool TryParse(string value, out ReportColor reportColor)
		{
			return TryParse(value, allowTransparency: false, out reportColor);
		}

		public static bool TryParse(string value, bool allowTransparency, out ReportColor reportColor)
		{
			if (Validator.ValidateColor(value, out Color c, allowTransparency))
			{
				reportColor = new ReportColor(value, c, parsed: true);
				return true;
			}
			reportColor = null;
			return false;
		}
	}
}
