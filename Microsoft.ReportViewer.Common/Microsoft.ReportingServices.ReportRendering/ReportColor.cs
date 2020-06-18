using Microsoft.ReportingServices.Diagnostics.Utilities;
using Microsoft.ReportingServices.OnDemandReportRendering;
using Microsoft.ReportingServices.ReportProcessing;
using System.Drawing;

namespace Microsoft.ReportingServices.ReportRendering
{
	internal sealed class ReportColor
	{
		private string m_color;

		private Color m_GDIColor = Color.Empty;

		private bool m_parsed;

		internal bool Parsed => m_parsed;

		public ReportColor(Color color)
		{
			m_GDIColor = color;
			m_color = color.ToString();
			m_parsed = true;
		}

		public ReportColor(string color)
		{
			m_color = color;
			Validate();
			m_parsed = true;
		}

		internal ReportColor(string color, bool parsed)
		{
			m_color = color;
			m_parsed = parsed;
		}

		public override string ToString()
		{
			return m_color;
		}

		public Color ToColor()
		{
			if (!m_parsed)
			{
				Validator.ParseColor(m_color, out m_GDIColor);
				m_parsed = true;
			}
			return m_GDIColor;
		}

		internal void Validate()
		{
			if (!Validator.ValidateColor(m_color, out m_GDIColor))
			{
				throw new ReportRenderingException(ErrorCode.rrInvalidColor, m_color);
			}
		}
	}
}
