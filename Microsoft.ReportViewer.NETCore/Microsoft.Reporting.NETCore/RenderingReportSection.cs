using Microsoft.ReportingServices.Rendering.RPLProcessing;
using System.Drawing;
using System.Globalization;

namespace Microsoft.Reporting.NETCore
{
	internal sealed class RenderingReportSection : RenderingElementBase
	{
		private RenderingBody m_body;

		private RenderingHeader m_header;

		private RenderingFooter m_footer;

		internal RenderingHeader Header => m_header;

		internal RenderingBody Body => m_body;

		internal RenderingFooter Footer => m_footer;

		internal RenderingReportSection(GdiContext context, RPLReportSection reportSection, RPLSizes sectionSize, int index, float width, float offsetTop)
		{
			m_position.Y = offsetTop;
			m_position.X = sectionSize.Left;
			m_position.Width = width;
			m_position.Height = sectionSize.Height;
			string str = index.ToString(CultureInfo.InvariantCulture);
			m_accessibleName = "ReportSection" + str;
			RectangleF empty = RectangleF.Empty;
			if (reportSection.Header != null)
			{
				empty = new RectangleF(reportSection.Header.Left, reportSection.Header.Top + offsetTop, width, reportSection.Header.Height);
				reportSection.Header.Width = width;
				reportSection.Header.Element.ElementProps.UniqueName = "ReportSection" + str + "Header";
				m_header = new RenderingHeader();
				m_header.Initialize(context, reportSection.Header, empty, ReportPreviewStrings.ReportItemAccessibleNameHeader);
				m_header.SetWidth(width);
				offsetTop += reportSection.Header.Height;
			}
			RPLItemMeasurement rPLItemMeasurement = reportSection.Columns[0];
			empty = new RectangleF(rPLItemMeasurement.Left, rPLItemMeasurement.Top + offsetTop, width, rPLItemMeasurement.Height);
			rPLItemMeasurement.Width = width;
			rPLItemMeasurement.Element.ElementProps.UniqueName = "ReportSection" + str + "Body";
			m_body = new RenderingBody();
			m_body.Initialize(context, rPLItemMeasurement, empty, ReportPreviewStrings.ReportItemAccessibleNameBody);
			m_body.SetWidth(width);
			offsetTop += rPLItemMeasurement.Height;
			if (reportSection.Footer != null)
			{
				empty = new RectangleF(reportSection.Footer.Left, reportSection.Footer.Top + offsetTop, width, reportSection.Footer.Height);
				reportSection.Footer.Width = width;
				reportSection.Footer.Element.ElementProps.UniqueName = "Section" + str + "Footer";
				m_footer = new RenderingFooter();
				m_footer.Initialize(context, reportSection.Footer, empty, ReportPreviewStrings.ReportItemAccessibleNameFooter);
				m_footer.SetWidth(width);
				offsetTop += reportSection.Footer.Height;
			}
		}

		internal void DrawToPage(GdiContext context)
		{
			m_body.DrawToPage(context);
			if (m_header != null)
			{
				m_header.DrawToPage(context);
			}
			if (m_footer != null)
			{
				m_footer.DrawToPage(context);
			}
		}

		internal void Search(GdiContext context)
		{
			if (m_header != null)
			{
				m_header.Search(context);
			}
			m_body.Search(context);
			if (m_footer != null)
			{
				m_footer.Search(context);
			}
		}
	}
}
