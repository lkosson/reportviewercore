using Microsoft.ReportingServices.Rendering.ExcelRenderer.Excel;

namespace Microsoft.ReportingServices.Rendering.ExcelRenderer.Layout
{
	internal sealed class BorderProperties
	{
		private IColor m_color;

		private ExcelBorderStyle m_style;

		private ExcelBorderPart m_part;

		private double m_width = -1.0;

		internal ExcelBorderPart ExcelBorderPart
		{
			set
			{
				m_part = value;
			}
		}

		internal IColor Color
		{
			get
			{
				return m_color;
			}
			set
			{
				m_color = value;
			}
		}

		internal ExcelBorderStyle Style
		{
			get
			{
				return m_style;
			}
			set
			{
				if (m_width != -1.0)
				{
					m_style = LayoutConvert.ToBorderLineStyle(value, m_width);
				}
				else
				{
					m_style = value;
				}
			}
		}

		internal double Width
		{
			get
			{
				return m_width;
			}
			set
			{
				if (m_style != 0)
				{
					m_style = LayoutConvert.ToBorderLineStyle(m_style, value);
				}
				m_width = value;
			}
		}

		internal BorderProperties(IColor color, ExcelBorderStyle style, ExcelBorderPart part)
		{
			m_color = color;
			m_style = style;
			m_part = part;
		}

		internal BorderProperties(ExcelBorderPart part)
		{
			m_part = part;
		}

		internal BorderProperties(BorderProperties borderProps, ExcelBorderPart part)
		{
			if (borderProps != null)
			{
				m_color = borderProps.Color;
				m_style = borderProps.Style;
				m_width = borderProps.Width;
			}
			m_part = part;
		}

		internal void Render(IStyle style)
		{
			if (m_style != 0)
			{
				switch (m_part)
				{
				case ExcelBorderPart.Left:
					style.BorderLeftStyle = m_style;
					style.BorderLeftColor = m_color;
					break;
				case ExcelBorderPart.Right:
					style.BorderRightStyle = m_style;
					style.BorderRightColor = m_color;
					break;
				case ExcelBorderPart.Top:
					style.BorderTopStyle = m_style;
					style.BorderTopColor = m_color;
					break;
				case ExcelBorderPart.Bottom:
					style.BorderBottomStyle = m_style;
					style.BorderBottomColor = m_color;
					break;
				case ExcelBorderPart.Outline:
					style.BorderLeftStyle = m_style;
					style.BorderLeftColor = m_color;
					style.BorderRightStyle = m_style;
					style.BorderRightColor = m_color;
					style.BorderTopStyle = m_style;
					style.BorderTopColor = m_color;
					style.BorderBottomStyle = m_style;
					style.BorderBottomColor = m_color;
					break;
				case ExcelBorderPart.DiagonalDown:
				case ExcelBorderPart.DiagonalUp:
				case ExcelBorderPart.DiagonalBoth:
					style.BorderDiagStyle = m_style;
					style.BorderDiagColor = m_color;
					style.BorderDiagPart = m_part;
					break;
				}
			}
		}

		public override string ToString()
		{
			return m_style.ToString();
		}
	}
}
