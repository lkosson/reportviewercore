using Microsoft.ReportingServices.Diagnostics.Utilities;
using Microsoft.ReportingServices.Interfaces;
using Microsoft.ReportingServices.Rendering.HtmlRenderer;
using Microsoft.ReportingServices.Rendering.RPLProcessing;
using Microsoft.ReportingServices.Rendering.SPBProcessing;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.Rendering.WordRenderer
{
	internal class Word97Renderer : WordRenderer
	{
		protected List<SectionEntry> m_sections = new List<SectionEntry>();

		internal Word97Renderer(CreateAndRegisterStream createAndRegisterStream, Microsoft.ReportingServices.Rendering.SPBProcessing.SPBProcessing spbProcessing, IWordWriter writer, DeviceInfo deviceInfo, string reportName)
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
			bool flag2 = false;
			List<RPLReport> rplReportCache = new List<RPLReport>();
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
					SectionEntry sectionEntry = null;
					if (!firstSection || m_sections.Count == 0 || string.CompareOrdinal(m_sections[m_sections.Count - 1].SectionId, rPLReportSection.ID) != 0)
					{
						if (RSTrace.RenderingTracer.TraceVerbose)
						{
							RSTrace.RenderingTracer.Trace("The left or right margin is either <0 or the sum exceeds the page width.");
						}
						sectionEntry = new SectionEntry(rPLReportSection);
						if (rPLReportSection.Footer != null)
						{
							footer = (rPLReportSection.Footer.Element as RPLHeaderFooter);
						}
						if (sectionEntry.HeaderMeasurement != null || sectionEntry.FooterMeasurement != null)
						{
							flag2 = true;
						}
						m_sections.Add(sectionEntry);
						CachePage(ref pageCached, rplReportCache);
					}
					flag = SetFirstPageDimensions(flag, rPLPageContent, ref rplPageLayout, ref leftMargin, ref rightMargin, ref width, ref title, ref author, ref description);
					width = RevisePageDimensions(leftMargin, rightMargin, width, width2, autoFit);
					RenderHeaderBetweenSections(rPLReportSection, firstSection);
					RenderBodyContent(width2, rPLItemMeasurement);
					rPLReportSection = AdvanceToNextSection(rPLPageContent, rPLReportSection, ref firstSection, m_sections[m_sections.Count - 1], footer, sectionEntry);
				}
				if (!m_spbProcessing.Done && !pageCached)
				{
					m_rplReport.Release();
				}
			}
			m_writer.WriteParagraphEnd();
			m_writer.SetPageDimensions(m_pageHeight, width, leftMargin, rightMargin, rplPageLayout.MarginTop, rplPageLayout.MarginBottom);
			if (flag2)
			{
				m_inHeaderFooter = true;
				m_writer.InitHeaderFooter();
				bool flag3 = false;
				m_needsToResetTextboxes = true;
				for (int i = 0; i < m_sections.Count; i++)
				{
					RPLItemMeasurement headerMeasurement = m_sections[i].HeaderMeasurement;
					if (headerMeasurement != null)
					{
						RenderRectangle((RPLContainer)headerMeasurement.Element, 0f, canGrow: true, headerMeasurement, new BorderContext(), inTablix: false, ignoreStyles: true);
					}
					m_writer.FinishHeader(i);
					RPLItemMeasurement footerMeasurement = m_sections[i].FooterMeasurement;
					if (footerMeasurement != null)
					{
						RenderRectangle((RPLContainer)footerMeasurement.Element, 0f, canGrow: true, footerMeasurement, new BorderContext(), inTablix: false, ignoreStyles: true);
					}
					m_writer.FinishFooter(i);
					if (i != 0)
					{
						continue;
					}
					bool flag4 = headerMeasurement != null;
					bool flag5 = flag4 && !(headerMeasurement.Element.ElementPropsDef as RPLHeaderFooterPropsDef).PrintOnFirstPage;
					bool flag6 = footerMeasurement != null;
					bool flag7 = flag6 && !(footerMeasurement.Element.ElementPropsDef as RPLHeaderFooterPropsDef).PrintOnFirstPage;
					flag3 = ((flag5 || flag7) && (flag4 || flag6));
					if (flag3)
					{
						if (flag4 && !flag5)
						{
							RenderRectangle((RPLContainer)headerMeasurement.Element, 0f, canGrow: true, headerMeasurement, new BorderContext(), inTablix: false, ignoreStyles: true);
						}
						m_writer.FinishHeader(i, Word97Writer.HeaderFooterLocation.First);
						if (flag6 && !flag7)
						{
							RenderRectangle((RPLContainer)footerMeasurement.Element, 0f, canGrow: true, footerMeasurement, new BorderContext(), inTablix: false, ignoreStyles: true);
						}
						m_writer.FinishFooter(i, Word97Writer.HeaderFooterLocation.First);
					}
					m_needsToResetTextboxes = false;
				}
				m_writer.FinishHeadersFooters(flag3);
				m_inHeaderFooter = false;
			}
			FinishRendering(rplReportCache, title, author, description);
			return true;
		}

		protected override bool RenderRectangleItemAndLines(RPLContainer rectangle, BorderContext borderContext, int y, PageTableCell cell, string linkToChildId, float runningLeft, bool rowUsed)
		{
			rowUsed = RenderRectangleItem(y, cell, borderContext, linkToChildId, rectangle, runningLeft, rowUsed);
			RenderLines(y, cell, borderContext);
			return rowUsed;
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
			RenderTextBox(textBox, inTablix, cellIndex, needsTable, style, measurement, notCanGrow, textBoxPropsDef, textBoxProperties, isSimple, textBoxValue, borderContext, oldCellIndex);
			RenderTextBoxProperties(inTablix, cellIndex, needsTable, style);
		}

		protected override void RenderTablixCell(RPLTablix tablix, float left, float[] widths, TablixGhostCell[] ghostCells, BorderContext borderContext, int nextCell, RPLTablixCell cell, List<RPLTablixMemberCell>.Enumerator omittedCells, bool lastCell)
		{
			RPLItemMeasurement tablixCellMeasurement = GetTablixCellMeasurement(cell, nextCell, widths, ghostCells, omittedCells, lastCell, tablix);
			RenderTablixCellItem(cell, widths, tablixCellMeasurement, left, borderContext);
			ClearTablixCellBorders(cell);
			FinishRenderingTablixCell(cell, widths, ghostCells, borderContext);
		}

		protected override void RenderRPLContainer(RPLElement element, bool inTablix, RPLItemMeasurement measurement, int cellIndex, BorderContext borderContext, bool hasBorder)
		{
			RenderRPLContainerContents(element, measurement, borderContext, inTablix, hasBorder);
			RenderRPLContainerProperties(element, inTablix, cellIndex);
		}
	}
}
