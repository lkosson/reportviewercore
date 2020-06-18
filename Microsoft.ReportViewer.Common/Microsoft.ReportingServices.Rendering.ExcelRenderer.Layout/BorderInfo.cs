using Microsoft.ReportingServices.Rendering.ExcelRenderer.Excel;
using Microsoft.ReportingServices.Rendering.RPLProcessing;

namespace Microsoft.ReportingServices.Rendering.ExcelRenderer.Layout
{
	internal class BorderInfo
	{
		private IColor m_backgroundColor;

		private BorderProperties m_leftBorder;

		private BorderProperties m_topBorder;

		private BorderProperties m_rightBorder;

		private BorderProperties m_bottomBorder;

		private BorderProperties m_diagonal;

		private bool m_omitBorderTop;

		private bool m_omitBorderBottom;

		internal BorderProperties RightBorder => m_rightBorder;

		internal BorderProperties BottomBorder => m_bottomBorder;

		internal IColor BackgroundColor => m_backgroundColor;

		internal BorderProperties LeftBorder => m_leftBorder;

		internal BorderProperties TopBorder => m_topBorder;

		internal BorderProperties Diagonal => m_diagonal;

		internal bool OmitBorderTop
		{
			get
			{
				return m_omitBorderTop;
			}
			set
			{
				m_omitBorderTop = value;
			}
		}

		internal bool OmitBorderBottom
		{
			get
			{
				return m_omitBorderBottom;
			}
			set
			{
				m_omitBorderBottom = value;
			}
		}

		internal BorderInfo()
		{
		}

		internal BorderInfo(RPLStyleProps style, bool omitBorderTop, bool omitBorderBottom, IExcelGenerator excel)
		{
			m_omitBorderTop = omitBorderTop;
			m_omitBorderBottom = omitBorderBottom;
			FillAllBorders(style, ref m_leftBorder, ref m_rightBorder, ref m_topBorder, ref m_bottomBorder, ref m_backgroundColor, excel);
		}

		internal BorderInfo(RPLElementStyle style, int width, int height, bool slant, bool omitBorderTop, bool omitBorderBottom, bool defaultLine, IExcelGenerator excel)
		{
			m_omitBorderTop = omitBorderTop;
			m_omitBorderBottom = omitBorderBottom;
			if (height == 0)
			{
				if (defaultLine)
				{
					m_topBorder = new BorderProperties(ExcelBorderPart.Top);
					FillBorderProperties(excel, m_topBorder, style[5], style[10], style[0]);
				}
				else
				{
					m_bottomBorder = new BorderProperties(ExcelBorderPart.Bottom);
					FillBorderProperties(excel, m_bottomBorder, style[5], style[10], style[0]);
				}
			}
			else if (width == 0)
			{
				m_leftBorder = new BorderProperties(ExcelBorderPart.Left);
				FillBorderProperties(excel, m_leftBorder, style[5], style[10], style[0]);
			}
			else if (slant)
			{
				m_diagonal = new BorderProperties(ExcelBorderPart.DiagonalUp);
				FillBorderProperties(excel, m_diagonal, style[5], style[10], style[0]);
			}
			else
			{
				m_diagonal = new BorderProperties(ExcelBorderPart.DiagonalDown);
				FillBorderProperties(excel, m_diagonal, style[5], style[10], style[0]);
			}
		}

		internal static void FillAllBorders(RPLStyleProps style, ref BorderProperties leftBorder, ref BorderProperties rightBorder, ref BorderProperties topBorder, ref BorderProperties bottomBorder, ref IColor backgroundColor, IExcelGenerator excel)
		{
			if (style[34] != null && !style[34].Equals("Transparent"))
			{
				backgroundColor = excel.AddColor((string)style[34]);
			}
			FillLeftBorderProperties(style, excel, ref leftBorder);
			FillRightBorderProperties(style, excel, ref rightBorder);
			FillTopBorderProperties(style, excel, ref topBorder);
			FillBottomBorderProperties(style, excel, ref bottomBorder);
		}

		private static void FillLeftBorderProperties(RPLStyleProps style, IExcelGenerator excel, ref BorderProperties leftBorder)
		{
			BorderProperties currBorder = FillBorderProperties(excel, null, leftBorder, ExcelBorderPart.Left, style[5], style[10], style[0]);
			currBorder = FillBorderProperties(excel, currBorder, leftBorder, ExcelBorderPart.Left, style[6], style[11], style[1]);
			if (currBorder != null)
			{
				leftBorder = currBorder;
			}
		}

		private static void FillRightBorderProperties(RPLStyleProps style, IExcelGenerator excel, ref BorderProperties rightBorder)
		{
			BorderProperties currBorder = FillBorderProperties(excel, null, rightBorder, ExcelBorderPart.Right, style[5], style[10], style[0]);
			currBorder = FillBorderProperties(excel, currBorder, rightBorder, ExcelBorderPart.Right, style[7], style[12], style[2]);
			if (currBorder != null)
			{
				rightBorder = currBorder;
			}
		}

		private static void FillTopBorderProperties(RPLStyleProps style, IExcelGenerator excel, ref BorderProperties topBorder)
		{
			BorderProperties currBorder = FillBorderProperties(excel, null, topBorder, ExcelBorderPart.Top, style[5], style[10], style[0]);
			currBorder = FillBorderProperties(excel, currBorder, topBorder, ExcelBorderPart.Top, style[8], style[13], style[3]);
			if (currBorder != null)
			{
				topBorder = currBorder;
			}
		}

		private static void FillBottomBorderProperties(RPLStyleProps style, IExcelGenerator excel, ref BorderProperties bottomBorder)
		{
			BorderProperties currBorder = FillBorderProperties(excel, null, bottomBorder, ExcelBorderPart.Bottom, style[5], style[10], style[0]);
			currBorder = FillBorderProperties(excel, currBorder, bottomBorder, ExcelBorderPart.Bottom, style[9], style[14], style[4]);
			if (currBorder != null)
			{
				bottomBorder = currBorder;
			}
		}

		private static BorderProperties FillBorderProperties(IExcelGenerator excel, BorderProperties currBorder, BorderProperties border, ExcelBorderPart part, object style, object width, object color)
		{
			BorderProperties borderProperties = currBorder;
			if (style != null)
			{
				if (borderProperties == null)
				{
					borderProperties = new BorderProperties(border, part);
				}
				borderProperties.Style = LayoutConvert.ToBorderLineStyle((RPLFormat.BorderStyles)style);
			}
			if (width != null)
			{
				if (borderProperties == null)
				{
					borderProperties = new BorderProperties(border, part);
				}
				borderProperties.Width = LayoutConvert.ToPoints((string)width);
			}
			if (color != null && !color.Equals("Transparent"))
			{
				if (borderProperties == null)
				{
					borderProperties = new BorderProperties(border, part);
				}
				borderProperties.Color = excel.AddColor((string)color);
			}
			return borderProperties;
		}

		private void FillBorderProperties(IExcelGenerator excel, BorderProperties border, object style, object width, object color)
		{
			if (style != null)
			{
				border.Style = LayoutConvert.ToBorderLineStyle((RPLFormat.BorderStyles)style);
			}
			if (width != null)
			{
				border.Width = LayoutConvert.ToPoints((string)width);
			}
			if (color != null && !color.Equals("Transparent"))
			{
				border.Color = excel.AddColor((string)color);
			}
		}

		internal void RenderBorders(IExcelGenerator excel)
		{
			IStyle cellStyle = excel.GetCellStyle();
			if (m_backgroundColor != null)
			{
				cellStyle.BackgroundColor = m_backgroundColor;
			}
			if (m_diagonal != null)
			{
				m_diagonal.Render(cellStyle);
			}
			if (m_topBorder != null && !m_omitBorderTop)
			{
				m_topBorder.Render(cellStyle);
			}
			if (m_bottomBorder != null && !m_omitBorderBottom)
			{
				m_bottomBorder.Render(cellStyle);
			}
			if (m_leftBorder != null)
			{
				m_leftBorder.Render(cellStyle);
			}
			if (m_rightBorder != null)
			{
				m_rightBorder.Render(cellStyle);
			}
		}
	}
}
