using Microsoft.ReportingServices.OnDemandReportRendering;
using Microsoft.ReportingServices.Rendering.RPLProcessing;
using System;
using System.IO;

namespace Microsoft.ReportingServices.Rendering.SPBProcessing
{
	internal sealed class ReportSection
	{
		private Microsoft.ReportingServices.OnDemandReportRendering.ReportSection m_reportSectionDef;

		private PageContext m_pageContext;

		private PageItem m_body;

		private PageHeadFoot m_header;

		private PageHeadFoot m_footer;

		private ItemSizes m_itemRenderSizes;

		private int m_sectionIndex = -1;

		private PageItemHelper m_bodyHelper;

		private long m_offset;

		private long m_bodyOffset;

		private RPLReportSection m_rplReportSection;

		private int m_itemsOnPage;

		internal bool Done
		{
			get
			{
				if (m_body == null)
				{
					return true;
				}
				if (m_body.ItemState != PageItem.State.OnPage)
				{
					return m_body.ItemState == PageItem.State.OnPagePBEnd;
				}
				return true;
			}
		}

		internal bool SpanPages
		{
			get
			{
				if (m_body == null)
				{
					return true;
				}
				return m_body.ItemState == PageItem.State.SpanPages;
			}
		}

		internal bool OnPagePBEnd
		{
			get
			{
				if (m_body == null)
				{
					return true;
				}
				return m_body.ItemState == PageItem.State.OnPagePBEnd;
			}
		}

		internal PageItem Body => m_body;

		internal PageHeadFoot Header => m_header;

		internal PageHeadFoot Footer => m_footer;

		internal ItemSizes ItemRenderSizes => m_itemRenderSizes;

		internal int SectionIndex
		{
			get
			{
				return m_sectionIndex;
			}
			set
			{
				m_sectionIndex = value;
			}
		}

		internal long Offset => m_offset;

		internal RPLReportSection RPLReportSection => m_rplReportSection;

		internal ReportSection(Microsoft.ReportingServices.OnDemandReportRendering.ReportSection section, PageContext pageContext)
		{
			m_reportSectionDef = section;
			m_pageContext = pageContext;
		}

		internal void SetContext()
		{
			m_body = new ReportBody(m_reportSectionDef.Body, m_reportSectionDef.Width, m_pageContext);
		}

		internal bool CalculatePage(RPLWriter rplWriter, int page, int totalPages, int regionPageNumber, int regionTotalPages, bool firstSectionOnPage, bool lastSection, Interactivity interactivity, double heightToBeUsed, ref PageItemHelper lastBodyInfo, ref bool delayedHeader, ref bool delayedFooter, ref bool lastSectionOnPage)
		{
			m_pageContext.EvaluatePageHeaderFooter = false;
			PageSection pageHeader = m_reportSectionDef.Page.PageHeader;
			PageSection pageFooter = m_reportSectionDef.Page.PageFooter;
			bool renderHeader = false;
			bool renderFooter = false;
			if (pageHeader != null || pageFooter != null)
			{
				m_reportSectionDef.SetPage(regionPageNumber, regionTotalPages, page, totalPages);
			}
			if ((rplWriter != null || (interactivity != null && !interactivity.Done && interactivity.NeedPageHeaderFooter)) && m_reportSectionDef.NeedsReportItemsOnPage)
			{
				InitialCheckForHeader(pageHeader, page, totalPages, firstSectionOnPage, ref renderHeader);
				InitialCheckForFooter(pageFooter, page, totalPages, lastSection, ref renderFooter);
				if (renderHeader || renderFooter)
				{
					m_pageContext.EvaluatePageHeaderFooter = true;
				}
			}
			WriteStartItemToStream(rplWriter);
			double parentHeight = 0.0;
			m_body.UpdateItem(lastBodyInfo);
			m_body.CalculatePage(rplWriter, lastBodyInfo, m_pageContext, null, null, 0.0, ref parentHeight, interactivity);
			m_pageContext.ApplyPageName(page);
			m_itemsOnPage++;
			if (m_pageContext.CancelPage)
			{
				m_body = null;
				return false;
			}
			WriteBodyColumnsToStream(rplWriter);
			CreateReportSectionSizes(rplWriter);
			CheckForLastSectionOnPage(heightToBeUsed, lastSection, ref lastSectionOnPage);
			if (rplWriter != null || (interactivity != null && !interactivity.Done && interactivity.NeedPageHeaderFooter))
			{
				if (!m_reportSectionDef.NeedsReportItemsOnPage)
				{
					InitialCheckForHeader(pageHeader, page, totalPages, firstSectionOnPage, ref renderHeader);
					InitialCheckForFooter(pageFooter, page, totalPages, lastSection, ref renderFooter);
				}
				FinalCheckForHeader(pageHeader, page, lastSection && Done, firstSectionOnPage, ref renderHeader);
				FinalCheckForFooter(pageFooter, page, lastSection && Done, lastSectionOnPage, ref renderFooter);
				if (pageHeader != null || pageFooter != null)
				{
					string pageName = m_pageContext.PageTotalInfo.GetPageName(page);
					m_reportSectionDef.SetPageName(pageName);
					m_reportSectionDef.GetPageSections();
				}
				PageContext pageContext = new PageContext(m_pageContext, PageContext.PageContextFlags.FullOnPage, PageContext.IgnorePBReasonFlag.HeaderFooter);
				if (renderFooter)
				{
					if ((m_pageContext.VersionPicker == RPLVersionEnum.RPL2008 || pageContext.VersionPicker == RPLVersionEnum.RPL2008WithImageConsolidation) & lastSectionOnPage)
					{
						delayedFooter = true;
					}
					if (!delayedFooter)
					{
						pageContext.RPLSectionArea = PageContext.RPLReportSectionArea.Footer;
						m_footer = new PageHeadFoot(pageFooter, m_reportSectionDef.Width, pageContext);
						if (m_pageContext.VersionPicker == RPLVersionEnum.RPL2008 || m_pageContext.VersionPicker == RPLVersionEnum.RPL2008WithImageConsolidation)
						{
							m_footer.CalculateItem(rplWriter, pageContext, isHeader: false, interactivity, native: false);
						}
						else
						{
							m_footer.CalculateItem(rplWriter, pageContext, isHeader: false, interactivity, native: true);
						}
						m_itemsOnPage++;
					}
					if (m_pageContext.CancelPage)
					{
						m_body = null;
						m_footer = null;
						return false;
					}
				}
				if (renderHeader)
				{
					if (m_pageContext.VersionPicker == RPLVersionEnum.RPL2008 || pageContext.VersionPicker == RPLVersionEnum.RPL2008WithImageConsolidation)
					{
						if (firstSectionOnPage)
						{
							delayedHeader = true;
						}
					}
					else if (page > 1 && firstSectionOnPage && !pageHeader.PrintOnLastPage && !m_pageContext.AddFirstPageHeaderFooter)
					{
						delayedHeader = true;
					}
					if (!delayedHeader)
					{
						pageContext.RPLSectionArea = PageContext.RPLReportSectionArea.Header;
						m_header = new PageHeadFoot(pageHeader, m_reportSectionDef.Width, pageContext);
						if (m_pageContext.VersionPicker == RPLVersionEnum.RPL2008 || m_pageContext.VersionPicker == RPLVersionEnum.RPL2008WithImageConsolidation)
						{
							m_header.CalculateItem(rplWriter, pageContext, isHeader: true, interactivity, native: false);
						}
						else
						{
							m_header.CalculateItem(rplWriter, pageContext, isHeader: true, interactivity, native: true);
						}
						m_itemsOnPage++;
					}
					if (m_pageContext.CancelPage)
					{
						m_body = null;
						m_footer = null;
						m_header = null;
						return false;
					}
				}
			}
			if (!delayedHeader || m_pageContext.VersionPicker == RPLVersionEnum.RPL2008 || m_pageContext.VersionPicker == RPLVersionEnum.RPL2008WithImageConsolidation)
			{
				UpdateReportSectionSizes(rplWriter);
				WriteEndItemToStream(rplWriter);
			}
			lastBodyInfo = null;
			return true;
		}

		internal void WriteStartItemToStream(RPLWriter rplWriter)
		{
			if (rplWriter == null)
			{
				return;
			}
			BinaryWriter binaryWriter = rplWriter.BinaryWriter;
			if (binaryWriter != null)
			{
				if (m_pageContext.VersionPicker == RPLVersionEnum.RPL2008 || m_pageContext.VersionPicker == RPLVersionEnum.RPL2008WithImageConsolidation)
				{
					WriteStartRectangleItemToRPLStream2008(binaryWriter);
				}
				else
				{
					WriteStartSectionItemToRPLStream(binaryWriter);
				}
			}
			else
			{
				WriteStartSectionItemToRPLOM(rplWriter);
			}
		}

		internal void WriteEndItemToStream(RPLWriter rplWriter)
		{
			if (rplWriter == null)
			{
				return;
			}
			BinaryWriter binaryWriter = rplWriter.BinaryWriter;
			if (binaryWriter != null)
			{
				if (m_pageContext.VersionPicker == RPLVersionEnum.RPL2008 || m_pageContext.VersionPicker == RPLVersionEnum.RPL2008WithImageConsolidation)
				{
					WriteEndRectangleItemToRPLStream2008(binaryWriter);
				}
				else
				{
					WriteEndSectionItemToRPLStream(binaryWriter);
				}
			}
			else
			{
				WriteEndSectionItemToRPLOM(rplWriter);
			}
		}

		internal void WriteBodyColumnsToStream(RPLWriter rplWriter)
		{
			if (m_pageContext.VersionPicker != 0 && m_pageContext.VersionPicker != RPLVersionEnum.RPL2008WithImageConsolidation && rplWriter != null)
			{
				BinaryWriter binaryWriter = rplWriter.BinaryWriter;
				if (binaryWriter != null)
				{
					Stream baseStream = binaryWriter.BaseStream;
					long position = baseStream.Position;
					binaryWriter.Write((byte)16);
					binaryWriter.Write(m_bodyOffset);
					binaryWriter.Write(1);
					m_body.WritePageItemRenderSizes(binaryWriter);
					m_bodyOffset = baseStream.Position;
					binaryWriter.Write((byte)254);
					binaryWriter.Write(position);
					binaryWriter.Write(byte.MaxValue);
				}
				else
				{
					m_rplReportSection.Columns[0] = m_body.WritePageItemRenderSizes();
				}
			}
		}

		internal void CalculateDelayedHeader(RPLWriter rplWriter, Interactivity interactivity)
		{
			PageSection pageHeader = m_reportSectionDef.Page.PageHeader;
			PageContext pageContext = new PageContext(m_pageContext, PageContext.PageContextFlags.FullOnPage, PageContext.IgnorePBReasonFlag.HeaderFooter);
			pageContext.RPLSectionArea = PageContext.RPLReportSectionArea.Header;
			m_header = new PageHeadFoot(pageHeader, m_reportSectionDef.Width, pageContext);
			m_header.CalculateItem(rplWriter, pageContext, isHeader: true, interactivity, native: true);
			m_itemsOnPage++;
		}

		internal void CalculateDelayedFooter(RPLWriter rplWriter, Interactivity interactivity)
		{
			PageSection pageFooter = m_reportSectionDef.Page.PageFooter;
			PageContext pageContext = new PageContext(m_pageContext, PageContext.PageContextFlags.FullOnPage, PageContext.IgnorePBReasonFlag.HeaderFooter);
			pageContext.RPLSectionArea = PageContext.RPLReportSectionArea.Footer;
			m_footer = new PageHeadFoot(pageFooter, m_reportSectionDef.Width, pageContext);
			m_footer.CalculateItem(rplWriter, pageContext, isHeader: false, interactivity, native: true);
			m_itemsOnPage++;
		}

		internal void UpdateItem(ReportSectionHelper sectionHelper)
		{
			if (sectionHelper != null)
			{
				m_sectionIndex = sectionHelper.SectionIndex;
				m_bodyHelper = sectionHelper.BodyHelper;
			}
		}

		internal void WritePaginationInfo(BinaryWriter reportPageInfo)
		{
			if (reportPageInfo != null)
			{
				reportPageInfo.Write((byte)16);
				WritePaginationInfoProperties(reportPageInfo);
				reportPageInfo.Write(byte.MaxValue);
			}
		}

		internal ReportSectionHelper WritePaginationInfo()
		{
			ReportSectionHelper reportSectionHelper = new ReportSectionHelper();
			WritePaginationInfoProperties(reportSectionHelper);
			return reportSectionHelper;
		}

		internal void WritePaginationInfoProperties(BinaryWriter reportPageInfo)
		{
			if (reportPageInfo != null)
			{
				reportPageInfo.Write((byte)23);
				reportPageInfo.Write(m_sectionIndex);
				if (m_body != null && !Done)
				{
					m_body.WritePaginationInfo(reportPageInfo);
				}
			}
		}

		internal void WritePaginationInfoProperties(ReportSectionHelper sectionHelper)
		{
			if (sectionHelper != null)
			{
				sectionHelper.SectionIndex = m_sectionIndex;
				if (m_body != null && !Done)
				{
					sectionHelper.BodyHelper = m_body.WritePaginationInfo();
				}
			}
		}

		internal void UpdateReportSectionSizes(RPLWriter rplWriter)
		{
			if (m_itemRenderSizes != null && rplWriter != null)
			{
				double num = m_itemRenderSizes.Width;
				if (m_header != null)
				{
					num = Math.Max(num, m_header.ItemRenderSizes.Width);
					m_itemRenderSizes.Height += m_header.ItemRenderSizes.Height;
				}
				if (m_footer != null)
				{
					num = Math.Max(num, m_footer.ItemRenderSizes.Width);
					m_itemRenderSizes.Height += m_footer.ItemRenderSizes.Height;
				}
				m_itemRenderSizes.Width = num;
				NormalizeSectionAreasWidths(rplWriter, num);
			}
		}

		internal void Reset()
		{
			m_header = null;
			m_footer = null;
			m_rplReportSection = null;
			m_bodyOffset = 0L;
			m_offset = 0L;
			m_itemRenderSizes = null;
			m_sectionIndex = -1;
			m_bodyHelper = null;
			m_itemsOnPage = 0;
			if (Done)
			{
				m_body = null;
			}
		}

		internal bool IsHeaderPrintOnLastPage()
		{
			PageSection pageHeader = m_reportSectionDef.Page.PageHeader;
			if (pageHeader == null)
			{
				return false;
			}
			if (pageHeader.PrintOnLastPage)
			{
				return true;
			}
			return false;
		}

		private void NormalizeSectionAreasWidths(RPLWriter rplWriter, double width)
		{
			if (m_itemRenderSizes != null && rplWriter != null)
			{
				m_body.ItemRenderSizes.Width = width;
				if (m_header != null)
				{
					m_header.ItemRenderSizes.Width = width;
				}
				if (m_footer != null)
				{
					m_footer.ItemRenderSizes.Width = width;
				}
			}
		}

		private void CreateReportSectionSizes(RPLWriter rplWriter)
		{
			if (rplWriter != null)
			{
				m_itemRenderSizes = new ItemSizes(m_body.ItemRenderSizes);
			}
			else
			{
				m_itemRenderSizes = new ItemSizes(m_body.ItemPageSizes);
			}
		}

		private void InitialCheckForHeader(PageSection header, int page, int totalPages, bool firstSectionOnPage, ref bool renderHeader)
		{
			if (header == null)
			{
				return;
			}
			if (m_pageContext.AddFirstPageHeaderFooter)
			{
				renderHeader = true;
			}
			else if (totalPages > 0)
			{
				if (page == 1)
				{
					if (firstSectionOnPage)
					{
						if (header.PrintOnFirstPage)
						{
							renderHeader = true;
						}
					}
					else if (header.PrintBetweenSections)
					{
						renderHeader = true;
					}
				}
				else if (page > 1 && page < totalPages)
				{
					if (firstSectionOnPage)
					{
						renderHeader = true;
					}
					else if (header.PrintBetweenSections)
					{
						renderHeader = true;
					}
				}
				else if (firstSectionOnPage)
				{
					if (header.PrintOnLastPage)
					{
						renderHeader = true;
					}
				}
				else if (header.PrintBetweenSections)
				{
					renderHeader = true;
				}
			}
			else if (page == 1)
			{
				if (firstSectionOnPage)
				{
					if (header.PrintOnFirstPage)
					{
						renderHeader = true;
					}
				}
				else if (header.PrintBetweenSections)
				{
					renderHeader = true;
				}
			}
			else if (firstSectionOnPage)
			{
				renderHeader = true;
			}
			else if (header.PrintBetweenSections)
			{
				renderHeader = true;
			}
		}

		private void InitialCheckForFooter(PageSection footer, int page, int totalPages, bool lastReportSection, ref bool renderFooter)
		{
			if (footer == null)
			{
				return;
			}
			if (m_pageContext.AddFirstPageHeaderFooter)
			{
				renderFooter = true;
			}
			else if (totalPages > 0)
			{
				if (page >= 1 && page < totalPages)
				{
					renderFooter = true;
				}
				else if (lastReportSection)
				{
					if (footer.PrintOnLastPage)
					{
						renderFooter = true;
					}
				}
				else if (footer.PrintBetweenSections)
				{
					renderFooter = true;
				}
			}
			else
			{
				renderFooter = true;
			}
		}

		private void FinalCheckForHeader(PageSection header, int page, bool lastPage, bool firstSectionOnPage, ref bool renderHeader)
		{
			if (((header != null) & renderHeader) && !m_pageContext.AddFirstPageHeaderFooter && page > 1 && firstSectionOnPage && lastPage && !header.PrintOnLastPage)
			{
				renderHeader = false;
			}
		}

		private void FinalCheckForFooter(PageSection footer, int page, bool lastPage, bool lastSectionOnPage, ref bool renderFooter)
		{
			if (!((footer != null) & renderFooter) || m_pageContext.AddFirstPageHeaderFooter)
			{
				return;
			}
			bool flag = false;
			if (!lastPage)
			{
				if (page == 1)
				{
					if (lastSectionOnPage)
					{
						if (footer.PrintOnFirstPage)
						{
							flag = true;
						}
					}
					else if (footer.PrintBetweenSections)
					{
						flag = true;
					}
				}
				else if (lastSectionOnPage)
				{
					flag = true;
				}
				else if (footer.PrintBetweenSections)
				{
					flag = true;
				}
			}
			else if (lastSectionOnPage)
			{
				if (footer.PrintOnLastPage)
				{
					flag = true;
				}
			}
			else if (footer.PrintBetweenSections)
			{
				renderFooter = true;
			}
			renderFooter = flag;
		}

		private void CheckForLastSectionOnPage(double heightToBeUsed, bool lastSection, ref bool lastSectionOnPage)
		{
			if (!lastSectionOnPage)
			{
				lastSectionOnPage = (SpanPages || OnPagePBEnd || (Done && lastSection));
				if (!lastSectionOnPage && heightToBeUsed != double.MaxValue && heightToBeUsed - Body.ItemPageSizes.Height < 0.01)
				{
					lastSectionOnPage = true;
				}
			}
		}

		private void WriteStartSectionItemToRPLStream(BinaryWriter spbifWriter)
		{
			Stream baseStream = spbifWriter.BaseStream;
			m_offset = baseStream.Position;
			spbifWriter.Write((byte)21);
			spbifWriter.Write((byte)22);
			spbifWriter.Write((byte)0);
			spbifWriter.Write(m_reportSectionDef.ID);
			spbifWriter.Write((byte)2);
			spbifWriter.Write((float)m_reportSectionDef.Page.ColumnSpacing.ToMillimeters());
			spbifWriter.Write((byte)1);
			spbifWriter.Write(m_reportSectionDef.Page.Columns);
			spbifWriter.Write(byte.MaxValue);
			m_bodyOffset = baseStream.Position;
			spbifWriter.Write((byte)20);
		}

		private void WriteStartRectangleItemToRPLStream2008(BinaryWriter spbifWriter)
		{
			Stream baseStream = spbifWriter.BaseStream;
			m_offset = baseStream.Position;
			spbifWriter.Write((byte)10);
			spbifWriter.Write((byte)15);
			spbifWriter.Write((byte)0);
			spbifWriter.Write(byte.MaxValue);
			spbifWriter.Write((byte)1);
			spbifWriter.Write(byte.MaxValue);
			spbifWriter.Write(byte.MaxValue);
		}

		private void WriteStartSectionItemToRPLOM(RPLWriter rplWriter)
		{
			m_rplReportSection = new RPLReportSection(1);
			m_rplReportSection.ID = m_reportSectionDef.ID;
			m_rplReportSection.ColumnSpacing = (float)m_reportSectionDef.Page.ColumnSpacing.ToMillimeters();
			m_rplReportSection.ColumnCount = m_reportSectionDef.Page.Columns;
		}

		private void WriteEndSectionItemToRPLStream(BinaryWriter spbifWriter)
		{
			spbifWriter.Write(byte.MaxValue);
			Stream baseStream = spbifWriter.BaseStream;
			long position = baseStream.Position;
			spbifWriter.Write((byte)16);
			spbifWriter.Write(m_offset);
			spbifWriter.Write(m_itemsOnPage);
			m_body.WritePageItemRenderSizes(spbifWriter, m_bodyOffset);
			m_body.ItemRenderSizes = null;
			if (m_header != null)
			{
				m_header.WritePageItemRenderSizes(spbifWriter);
				m_header.ItemRenderSizes = null;
			}
			if (m_footer != null)
			{
				m_footer.WritePageItemRenderSizes(spbifWriter);
				m_footer.ItemRenderSizes = null;
			}
			m_offset = baseStream.Position;
			spbifWriter.Write((byte)254);
			spbifWriter.Write(position);
			spbifWriter.Write(byte.MaxValue);
		}

		private void WriteEndRectangleItemToRPLStream2008(BinaryWriter spbifWriter)
		{
			Stream baseStream = spbifWriter.BaseStream;
			long position = baseStream.Position;
			spbifWriter.Write((byte)16);
			spbifWriter.Write(m_offset);
			spbifWriter.Write(m_itemsOnPage);
			double num = 0.0;
			if (m_header != null)
			{
				m_header.WritePageItemRenderSizes(spbifWriter);
				num += m_header.ItemRenderSizes.Height;
				m_header.ItemRenderSizes = null;
			}
			m_body.ItemRenderSizes.Top += num;
			m_body.WritePageItemRenderSizes(spbifWriter);
			num += m_body.ItemRenderSizes.Height;
			m_body.ItemRenderSizes = null;
			if (m_footer != null)
			{
				m_footer.ItemRenderSizes.Top += num;
				m_footer.WritePageItemRenderSizes(spbifWriter);
				m_footer.ItemRenderSizes = null;
			}
			m_offset = baseStream.Position;
			spbifWriter.Write((byte)254);
			spbifWriter.Write(position);
			spbifWriter.Write(byte.MaxValue);
		}

		private void WriteEndSectionItemToRPLOM(RPLWriter rplWriter)
		{
			RPLItemMeasurement rPLItemMeasurement = m_body.WritePageItemRenderSizes();
			m_body.ItemRenderSizes = null;
			m_rplReportSection.BodyArea = new RPLMeasurement();
			m_rplReportSection.BodyArea.Height = rPLItemMeasurement.Height;
			m_rplReportSection.BodyArea.Width = rPLItemMeasurement.Width;
			m_rplReportSection.BodyArea.Top = rPLItemMeasurement.Top;
			m_rplReportSection.BodyArea.Left = rPLItemMeasurement.Left;
			if (m_header != null)
			{
				m_rplReportSection.Header = m_header.WritePageItemRenderSizes();
				m_header.ItemRenderSizes = null;
			}
			if (m_footer != null)
			{
				m_rplReportSection.Footer = m_footer.WritePageItemRenderSizes();
				m_footer.ItemRenderSizes = null;
			}
		}
	}
}
