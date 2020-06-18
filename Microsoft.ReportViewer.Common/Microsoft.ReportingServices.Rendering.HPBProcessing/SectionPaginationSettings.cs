using Microsoft.ReportingServices.OnDemandReportRendering;

namespace Microsoft.ReportingServices.Rendering.HPBProcessing
{
	internal class SectionPaginationSettings
	{
		private double m_columnSpacing = 12.699999809265137;

		private int m_columns = 1;

		private double m_headerHeight;

		private double m_footerHeight;

		private double m_columnWidth = 2.5399999618530273;

		public double ColumnSpacing => m_columnSpacing;

		public int Columns => m_columns;

		public double ColumnSpacingWidth => (double)(m_columns - 1) * m_columnSpacing;

		public double ColumnWidth => m_columnWidth;

		public double HeaderHeight => m_headerHeight;

		public double FooterHeight => m_footerHeight;

		public SectionPaginationSettings(Microsoft.ReportingServices.OnDemandReportRendering.ReportSection reportSection)
		{
			m_columns = reportSection.Page.Columns;
			m_columnSpacing = reportSection.Page.ColumnSpacing.ToMillimeters();
			if (reportSection.Page.PageHeader != null)
			{
				m_headerHeight = reportSection.Page.PageHeader.Height.ToMillimeters();
			}
			if (reportSection.Page.PageFooter != null)
			{
				m_footerHeight = reportSection.Page.PageFooter.Height.ToMillimeters();
			}
		}

		public void Validate(PaginationSettings deviceInfoSettings, int columns, double columnSpacing, ref double pageHeight, ref double pageWidth)
		{
			if (columns > 0)
			{
				m_columns = columns;
			}
			if (columnSpacing >= 0.0)
			{
				m_columnSpacing = columnSpacing;
			}
			double num = (double)m_columns * 2.5399999618530273;
			if (num + (double)(m_columns - 1) * m_columnSpacing + deviceInfoSettings.MarginLeft + deviceInfoSettings.MarginRight > pageWidth)
			{
				m_columnSpacing = 0.0;
				deviceInfoSettings.MarginLeft = 0.0;
				deviceInfoSettings.MarginRight = 0.0;
				if (num > pageWidth)
				{
					pageWidth = num;
				}
			}
			double num2 = 2.5399999618530273 + m_headerHeight + m_footerHeight;
			double num3 = pageHeight - (deviceInfoSettings.MarginTop + deviceInfoSettings.MarginBottom);
			if (num2 > num3)
			{
				deviceInfoSettings.MarginTop = 0.0;
				deviceInfoSettings.MarginBottom = 0.0;
				if (num2 > pageHeight)
				{
					pageHeight = num2;
				}
			}
		}

		public void SetColumnArea(PaginationSettings deviceInfoSettings)
		{
			double num = (double)(m_columns - 1) * m_columnSpacing;
			double num2 = deviceInfoSettings.UsablePageWidth - num;
			m_columnWidth = num2 / (double)m_columns;
		}
	}
}
