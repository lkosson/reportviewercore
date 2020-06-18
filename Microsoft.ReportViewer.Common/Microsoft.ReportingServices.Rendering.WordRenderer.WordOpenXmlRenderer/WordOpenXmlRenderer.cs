using Microsoft.ReportingServices.Diagnostics.Utilities;
using Microsoft.ReportingServices.Interfaces;
using Microsoft.ReportingServices.Rendering.HtmlRenderer;
using Microsoft.ReportingServices.Rendering.RPLProcessing;
using Microsoft.ReportingServices.Rendering.SPBProcessing;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.Rendering.WordRenderer.WordOpenXmlRenderer
{
	internal sealed class WordOpenXmlRenderer : WordRenderer
	{
		internal WordOpenXmlRenderer(CreateAndRegisterStream createAndRegisterStream, Microsoft.ReportingServices.Rendering.SPBProcessing.SPBProcessing spbProcessing, IWordWriter writer, DeviceInfo deviceInfo, string reportName)
			: base(createAndRegisterStream, spbProcessing, writer, deviceInfo, reportName)
		{
		}

		internal override bool Render()
		{
			RPLItemMeasurement rPLItemMeasurement = null;
			bool flag = true;
			string author = "";
			string title = "";
			string description = "";
			AutoFit autoFit = m_writer.AutoFit;
			float width = 0f;
			float leftMargin = 0f;
			float rightMargin = 0f;
			RPLPageLayout rplPageLayout = null;
			List<RPLReport> rplReportCache = new List<RPLReport>();
			bool hasHeaderSoFar = false;
			bool hasFooterSoFar = false;
			SectionEntry sectionEntry = null;
			while (!m_spbProcessing.Done)
			{
				if (!flag)
				{
					m_writer.WritePageBreak();
				}
				m_spbProcessing.GetNextPage(out m_rplReport);
				RPLPageContent rPLPageContent = m_rplReport.RPLPaginatedPages[0];
				RPLReportSection rPLReportSection = rPLPageContent.GetNextReportSection();
				bool pageCached = false;
				bool firstSection = true;
				while (rPLReportSection != null)
				{
					rPLItemMeasurement = rPLReportSection.Columns[0];
					float width2 = rPLReportSection.BodyArea.Width;
					RPLHeaderFooter footer = null;
					SectionEntry se = null;
					if (!firstSection || sectionEntry == null || string.CompareOrdinal(sectionEntry.SectionId, rPLReportSection.ID) != 0)
					{
						if (RSTrace.RenderingTracer.TraceVerbose)
						{
							RSTrace.RenderingTracer.Trace("The left or right margin is either <0 or the sum exceeds the page width.");
						}
						sectionEntry = (se = RenderHeaderFooters(rPLReportSection, firstSection, ref pageCached, rplReportCache, ref footer, ref hasHeaderSoFar, ref hasFooterSoFar));
					}
					flag = SetFirstPageDimensions(flag, rPLPageContent, ref rplPageLayout, ref leftMargin, ref rightMargin, ref width, ref title, ref author, ref description);
					width = RevisePageDimensions(leftMargin, rightMargin, width, width2, autoFit);
					RenderHeaderBetweenSections(rPLReportSection, firstSection);
					RenderBodyContent(width2, rPLItemMeasurement);
					rPLReportSection = AdvanceToNextSection(rPLPageContent, rPLReportSection, ref firstSection, sectionEntry, footer, se);
				}
				if (!m_spbProcessing.Done && !pageCached)
				{
					m_rplReport.Release();
				}
			}
			m_writer.WriteParagraphEnd();
			m_writer.SetPageDimensions(m_pageHeight, width, leftMargin, rightMargin, rplPageLayout.MarginTop, rplPageLayout.MarginBottom);
			FinishRendering(rplReportCache, title, author, description);
			return true;
		}

		protected override void RenderTablixCell(RPLTablix tablix, float left, float[] widths, TablixGhostCell[] ghostCells, BorderContext borderContext, int nextCell, RPLTablixCell cell, List<RPLTablixMemberCell>.Enumerator omittedCells, bool lastCell)
		{
			RPLItemMeasurement tablixCellMeasurement = GetTablixCellMeasurement(cell, nextCell, widths, ghostCells, omittedCells, lastCell, tablix);
			ClearTablixCellBorders(cell);
			m_writer.ApplyCellBorderContext(borderContext);
			RenderTablixCellItem(cell, widths, tablixCellMeasurement, left, borderContext);
			FinishRenderingTablixCell(cell, widths, ghostCells, borderContext);
		}

		protected override void RenderTextBox(RPLTextBox textBox, RPLItemMeasurement measurement, int cellIndex, float left, BorderContext borderContext, bool inTablix, bool hasBorder)
		{
			RPLTextBoxPropsDef textBoxPropsDef;
			bool isSimple;
			string textBoxValue;
			bool notCanGrow;
			bool needsTable;
			RPLElementStyle style;
			int oldCellIndex;
			RPLTextBoxProps textBoxProperties = GetTextBoxProperties(textBox, out textBoxPropsDef, out isSimple, out textBoxValue, inTablix, out notCanGrow, hasBorder, cellIndex, out needsTable, out style, out oldCellIndex);
			RenderTextBoxProperties(inTablix, cellIndex, needsTable, style);
			RenderTextBox(textBox, inTablix, cellIndex, needsTable, style, measurement, notCanGrow, textBoxPropsDef, textBoxProperties, isSimple, textBoxValue, borderContext, oldCellIndex);
		}

		protected override bool RenderRectangleItemAndLines(RPLContainer rectangle, BorderContext borderContext, int y, PageTableCell cell, string linkToChildId, float runningLeft, bool rowUsed)
		{
			RenderLines(y, cell, borderContext);
			rowUsed = RenderRectangleItem(y, cell, borderContext, linkToChildId, rectangle, runningLeft, rowUsed);
			return rowUsed;
		}

		private SectionEntry RenderHeaderFooters(RPLReportSection section, bool firstSection, ref bool pageCached, List<RPLReport> rplReportCache, ref RPLHeaderFooter footer, ref bool hasHeaderSoFar, ref bool hasFooterSoFar)
		{
			SectionEntry result = new SectionEntry(section);
			if (section.Footer != null)
			{
				footer = (section.Footer.Element as RPLHeaderFooter);
				if (footer.Children != null && footer.Children.Length != 0)
				{
					hasFooterSoFar = true;
				}
			}
			if (section.Header != null)
			{
				RPLHeaderFooter rPLHeaderFooter = section.Header.Element as RPLHeaderFooter;
				if (rPLHeaderFooter.Children != null && rPLHeaderFooter.Children.Length != 0)
				{
					hasHeaderSoFar = true;
				}
			}
			CachePage(ref pageCached, rplReportCache);
			m_inHeaderFooter = true;
			if (firstSection)
			{
				m_needsToResetTextboxes = true;
			}
			RPLItemMeasurement header = section.Header;
			if (hasHeaderSoFar)
			{
				m_writer.StartHeader();
				if (header != null)
				{
					RenderRectangle((RPLContainer)header.Element, 0f, canGrow: true, header, new BorderContext(), inTablix: false, ignoreStyles: true);
				}
				m_writer.FinishHeader();
			}
			RPLItemMeasurement footer2 = section.Footer;
			if (hasFooterSoFar)
			{
				m_writer.StartFooter();
				if (footer2 != null)
				{
					RenderRectangle((RPLContainer)footer2.Element, 0f, canGrow: true, footer2, new BorderContext(), inTablix: false, ignoreStyles: true);
				}
				m_writer.FinishFooter();
			}
			if (firstSection)
			{
				bool flag = header != null;
				bool flag2 = flag && !(header.Element.ElementPropsDef as RPLHeaderFooterPropsDef).PrintOnFirstPage;
				bool flag3 = footer2 != null;
				bool flag4 = flag3 && !(footer2.Element.ElementPropsDef as RPLHeaderFooterPropsDef).PrintOnFirstPage;
				if ((flag2 || flag4) && (flag || flag3))
				{
					if (hasHeaderSoFar)
					{
						m_writer.StartHeader(firstPage: true);
						if (flag && !flag2)
						{
							RenderRectangle((RPLContainer)header.Element, 0f, canGrow: true, header, new BorderContext(), inTablix: false, ignoreStyles: true);
						}
						m_writer.FinishHeader();
					}
					if (hasFooterSoFar)
					{
						m_writer.StartFooter(firstPage: true);
						if (flag3 && !flag4)
						{
							RenderRectangle((RPLContainer)footer2.Element, 0f, canGrow: true, footer2, new BorderContext(), inTablix: false, ignoreStyles: true);
						}
						m_writer.FinishFooter();
					}
					m_writer.HasTitlePage = true;
				}
				m_needsToResetTextboxes = false;
			}
			m_inHeaderFooter = false;
			return result;
		}

		protected override void RenderRPLContainer(RPLElement element, bool inTablix, RPLItemMeasurement measurement, int cellIndex, BorderContext borderContext, bool hasBorder)
		{
			RenderRPLContainerProperties(element, inTablix, cellIndex);
			RenderRPLContainerContents(element, measurement, borderContext, inTablix, hasBorder);
		}
	}
}
