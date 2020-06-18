using Microsoft.ReportingServices.OnDemandReportRendering;
using Microsoft.ReportingServices.Rendering.RichText;
using Microsoft.ReportingServices.Rendering.RPLProcessing;
using System.Collections.Generic;
using System.IO;

namespace Microsoft.ReportingServices.Rendering.HPBProcessing
{
	internal sealed class ReportSection
	{
		internal class ColumnDetail
		{
			private double m_left;

			private double m_top;

			private double m_width;

			private double m_height;

			private long m_offset;

			private RPLBody m_element;

			internal double Left => m_left;

			internal double Top => m_top;

			internal double Width => m_width;

			internal RPLBody Element => m_element;

			internal double Height => m_height;

			internal ColumnDetail(double leftEdge, double topEdge, ReportBody reportBody)
			{
				m_left = leftEdge;
				m_top = topEdge;
				m_width = reportBody.ItemPageSizes.Width;
				m_height = reportBody.ItemPageSizes.Height;
				m_offset = reportBody.Offset;
				m_element = (RPLBody)reportBody.RPLElement;
			}

			internal void WriteMeasurement(BinaryWriter spbifWriter)
			{
				spbifWriter.Write((float)Left);
				spbifWriter.Write((float)Top);
				spbifWriter.Write((float)Width);
				spbifWriter.Write((float)Height);
				spbifWriter.Write(0);
				spbifWriter.Write((byte)0);
				spbifWriter.Write(m_offset);
			}

			internal RPLItemMeasurement WriteMeasurement()
			{
				return new RPLItemMeasurement
				{
					Left = (float)Left,
					Top = (float)Top,
					Width = (float)Width,
					Height = (float)Height,
					ZIndex = 0,
					State = 0,
					Element = Element
				};
			}
		}

		private Microsoft.ReportingServices.OnDemandReportRendering.ReportSection m_reportSection;

		private ReportBody m_reportBody;

		private PageContext m_pageContext;

		private double m_leftEdge;

		private double m_topEdge;

		private long m_offset;

		private long m_pageOffset;

		private long m_columnsOffset;

		private List<ColumnDetail> m_columns;

		private PaginationSettings m_pageSettings;

		private SectionPaginationSettings m_sectionPageSettings;

		private RPLReportSection m_rplElement;

		private double m_columnHeight;

		private ItemSizes m_itemPageSizes = new ItemSizes();

		private PageHeadFoot m_header;

		private PageHeadFoot m_footer;

		private bool m_needsHeaderHeight;

		private int m_verticalPageNumber;

		private bool m_wroteEndToStream;

		internal double LeftEdge => m_leftEdge;

		internal bool Done
		{
			get
			{
				if (m_reportBody == null)
				{
					return true;
				}
				return false;
			}
		}

		public SectionPaginationSettings SectionPaginationSettings => m_sectionPageSettings;

		public Microsoft.ReportingServices.OnDemandReportRendering.ReportSection ROMReportSection => m_reportSection;

		internal ReportSection(Microsoft.ReportingServices.OnDemandReportRendering.ReportSection reportSection, PageContext pageContext, PaginationSettings paginationSettings, SectionPaginationSettings sectionPaginationSettings)
		{
			m_reportSection = reportSection;
			m_pageContext = pageContext;
			m_pageSettings = paginationSettings;
			m_sectionPageSettings = sectionPaginationSettings;
		}

		internal void SetContext()
		{
			m_reportBody = new ReportBody(m_reportSection.Body, m_reportSection.Width);
		}

		internal double NextPage(RPLWriter rplWriter, int pageNumber, int totalPages, double top, double availableHeight, ReportSection nextSection, bool isFirstSectionOnPage)
		{
			double num = 0.0;
			bool flag = nextSection == null;
			if (!flag)
			{
				SectionPaginationSettings sectionPaginationSettings = nextSection.SectionPaginationSettings;
				num = 2.5399999618530273 + sectionPaginationSettings.FooterHeight;
				PageSection pageHeader = nextSection.ROMReportSection.Page.PageHeader;
				if (pageHeader != null && pageHeader.PrintBetweenSections)
				{
					num += sectionPaginationSettings.HeaderHeight;
				}
			}
			double num2 = 0.0;
			PageSection pageHeader2 = m_reportSection.Page.PageHeader;
			PageSection pageFooter = m_reportSection.Page.PageFooter;
			bool flag2 = pageHeader2?.PrintBetweenSections ?? false;
			bool flag3 = pageFooter?.PrintBetweenSections ?? false;
			m_needsHeaderHeight = (isFirstSectionOnPage ? (pageHeader2 != null) : flag2);
			if (m_needsHeaderHeight)
			{
				num2 += m_sectionPageSettings.HeaderHeight;
				availableHeight -= num2;
			}
			if (pageFooter != null)
			{
				availableHeight -= m_sectionPageSettings.FooterHeight;
			}
			bool flag4 = false;
			bool flag5 = false;
			m_reportSection.SetPage(m_pageContext.PageNumberRegion, m_pageContext.Common.GetTotalPagesRegion(pageNumber), pageNumber, totalPages);
			bool needsReportItemsOnPage = m_reportSection.NeedsReportItemsOnPage;
			bool flag6 = m_pageContext.PropertyCacheState != PageContext.CacheState.CountPages;
			m_pageContext.EvaluatePageHeaderFooter = false;
			if (needsReportItemsOnPage && flag6)
			{
				if ((flag2 && !isFirstSectionOnPage) || HasHeaderOnPage(pageNumber, totalPages))
				{
					flag4 = true;
				}
				if (HasFooterOnPage(pageNumber, totalPages) || (flag3 && !flag))
				{
					flag5 = true;
				}
				if (flag4 || flag5)
				{
					m_pageContext.EvaluatePageHeaderFooter = true;
				}
			}
			if (rplWriter != null)
			{
				m_columns = new List<ColumnDetail>(m_sectionPageSettings.Columns);
				WriteStartItemToStream(rplWriter);
			}
			long num3 = 0L;
			if (!Done)
			{
				double columnWidth = m_sectionPageSettings.ColumnWidth;
				m_pageContext.Common.Pagination.CurrentColumnWidth = columnWidth;
				m_columnHeight = availableHeight;
				m_pageContext.Common.Pagination.CurrentColumnHeight = m_columnHeight;
				int num4 = 0;
				int columns = m_sectionPageSettings.Columns;
				RoundedDouble roundedDouble = new RoundedDouble(m_topEdge);
				RoundedDouble roundedDouble2 = new RoundedDouble(m_leftEdge);
				m_pageContext.VerticalPageNumber = m_verticalPageNumber;
				while (num4 < columns)
				{
					bool anyAncestorHasKT = false;
					if (m_leftEdge == 0.0)
					{
						if (m_pageContext.TextBoxDuplicates != null)
						{
							m_pageContext.TextBoxDuplicates = null;
							m_reportBody.ResolveDuplicates(m_pageContext, m_topEdge, null, recalculate: false);
						}
						m_reportBody.CalculateVertical(m_pageContext, m_topEdge, m_topEdge + availableHeight, null, new List<PageItem>(), ref anyAncestorHasKT, hasUnpinnedAncestors: false);
						m_verticalPageNumber++;
						m_pageContext.VerticalPageNumber = m_verticalPageNumber;
					}
					anyAncestorHasKT = false;
					m_reportBody.CalculateHorizontal(m_pageContext, m_leftEdge, m_leftEdge + columnWidth, null, new List<PageItem>(), ref anyAncestorHasKT, hasUnpinnedAncestors: false);
					num3 = m_pageContext.PropertyCacheWriter.BaseStream.Position;
					m_reportBody.AddToPage(rplWriter, m_pageContext, m_leftEdge, m_topEdge, m_leftEdge + columnWidth, m_topEdge + availableHeight, PageItem.RepeatState.None);
					rplWriter?.RegisterSectionItemizedData();
					m_pageContext.PropertyCacheWriter.BaseStream.Seek(num3, SeekOrigin.Begin);
					if (rplWriter != null)
					{
						m_columns.Add(new ColumnDetail(0.0 - m_leftEdge, 0.0 - m_topEdge, m_reportBody));
					}
					m_leftEdge += columnWidth;
					roundedDouble2.Value = m_leftEdge;
					if (num4 == 0 && m_reportBody.ItemPageSizes.Bottom - m_topEdge < availableHeight)
					{
						m_columnHeight = m_reportBody.ItemPageSizes.Bottom - m_topEdge;
					}
					if (roundedDouble2 >= m_reportBody.ItemPageSizes.Width)
					{
						m_leftEdge = 0.0;
						m_topEdge += availableHeight;
						roundedDouble.Value = m_topEdge;
						m_reportBody.ResetHorizontal(spanPages: true, null);
						m_pageContext.Common.PaginatingHorizontally = false;
					}
					else
					{
						m_pageContext.Common.PaginatingHorizontally = true;
					}
					num4++;
					if (roundedDouble >= m_reportBody.ItemPageSizes.Bottom)
					{
						m_reportBody = null;
						m_topEdge = 0.0;
						m_leftEdge = 0.0;
						break;
					}
				}
			}
			double num5 = availableHeight - m_columnHeight;
			if (Done && !flag && pageFooter != null && !flag3)
			{
				num5 += m_sectionPageSettings.FooterHeight;
			}
			bool flag7 = false;
			if (num5 < num || flag)
			{
				m_columnHeight = availableHeight;
				flag7 = true;
				num2 += m_sectionPageSettings.FooterHeight;
			}
			else if (flag3)
			{
				num2 += m_sectionPageSettings.FooterHeight;
			}
			m_itemPageSizes.Height = num2 + m_columnHeight;
			m_itemPageSizes.Top = top;
			WriteColumns(rplWriter);
			m_columns = null;
			if (Done && flag && totalPages == 0)
			{
				totalPages = pageNumber;
			}
			if (flag6)
			{
				flag4 = ((!isFirstSectionOnPage) ? flag2 : HasHeaderOnPage(pageNumber, totalPages));
				flag5 = ((!flag7) ? flag3 : HasFooterOnPage(pageNumber, totalPages));
			}
			if (flag4 || flag5)
			{
				m_pageContext.Common.CheckPageNameChanged();
				m_reportSection.SetPageName(m_pageContext.PageName);
				m_reportSection.GetPageSections();
				if (flag4 && !IsHeaderUnknown(isFirstSectionOnPage, pageNumber, totalPages))
				{
					RenderHeader(rplWriter);
				}
				else
				{
					m_header = null;
					flag4 = false;
				}
				if (flag5)
				{
					RenderFooter(rplWriter);
				}
				else
				{
					m_footer = null;
				}
				if (rplWriter != null && (flag4 || flag5))
				{
					rplWriter.RegisterSectionHeaderFooter();
				}
			}
			if (!IsHeaderUnknown(isFirstSectionOnPage, pageNumber, totalPages))
			{
				WriteEndItemToStream(rplWriter);
				m_wroteEndToStream = true;
			}
			else
			{
				m_wroteEndToStream = false;
			}
			rplWriter?.RegisterPageItemizedData();
			return m_itemPageSizes.Height;
		}

		private bool IsHeaderUnknown(bool isFirstSection, int page, int totalPages)
		{
			PageSection pageHeader = m_reportSection.Page.PageHeader;
			if (isFirstSection && page > 1 && totalPages == 0 && pageHeader != null)
			{
				return !pageHeader.PrintOnLastPage;
			}
			return false;
		}

		internal void WriteDelayedSectionHeader(RPLWriter rplWriter, bool isLastPage)
		{
			if (m_wroteEndToStream)
			{
				return;
			}
			if (!isLastPage)
			{
				List<SectionItemizedData> glyphCache = rplWriter.GlyphCache;
				if (m_footer != null && glyphCache != null)
				{
					SectionItemizedData sectionItemizedData = glyphCache[0];
					if (sectionItemizedData != null)
					{
						rplWriter.PageParagraphsItemizedData = sectionItemizedData.HeaderFooter;
					}
				}
				RenderHeader(rplWriter);
				if (m_footer == null && glyphCache != null)
				{
					Dictionary<string, List<TextRunItemizedData>> dictionary = rplWriter.PageParagraphsItemizedData;
					if (dictionary != null && dictionary.Count == 0)
					{
						dictionary = null;
					}
					glyphCache[0].HeaderFooter = dictionary;
				}
			}
			WriteEndItemToStream(rplWriter);
		}

		private void RenderHeader(RPLWriter rplWriter)
		{
			double columnWidth = m_pageContext.ColumnWidth;
			double columnHeight = m_pageContext.ColumnHeight;
			PageContext pageContext = new PageContext(m_pageContext);
			pageContext.IgnorePageBreaks = true;
			pageContext.IgnorePageBreaksReason = PageContext.IgnorePageBreakReason.InsideHeaderFooter;
			pageContext.EvaluatePageHeaderFooter = false;
			pageContext.Common.Pagination.CurrentColumnWidth = m_pageContext.Common.Pagination.UsablePageWidth;
			pageContext.Common.InHeaderFooter = true;
			double headerHeight = m_sectionPageSettings.HeaderHeight;
			pageContext.Common.Pagination.CurrentColumnHeight = headerHeight;
			PageHeadFoot pageHeadFoot = new PageHeadFoot(m_reportSection.Page.PageHeader, m_pageSettings.UsablePageWidth, aIsHeader: true);
			bool anyAncestorHasKT = false;
			pageHeadFoot.CalculateVertical(pageContext, 0.0, headerHeight, null, new List<PageItem>(), ref anyAncestorHasKT, hasUnpinnedAncestors: true);
			pageHeadFoot.CalculateHorizontal(pageContext, 0.0, m_pageSettings.UsablePageWidth, null, new List<PageItem>(), ref anyAncestorHasKT, hasUnpinnedAncestors: true);
			pageHeadFoot.AddToPage(rplWriter, pageContext, 0.0, 0.0, m_pageSettings.UsablePageWidth, headerHeight, PageItem.RepeatState.None);
			m_header = pageHeadFoot;
			pageContext.Common.Pagination.CurrentColumnWidth = columnWidth;
			pageContext.Common.Pagination.CurrentColumnHeight = columnHeight;
			pageContext.Common.InHeaderFooter = false;
		}

		private void RenderFooter(RPLWriter rplWriter)
		{
			double columnWidth = m_pageContext.ColumnWidth;
			double columnHeight = m_pageContext.ColumnHeight;
			PageContext pageContext = new PageContext(m_pageContext);
			pageContext.IgnorePageBreaks = true;
			pageContext.IgnorePageBreaksReason = PageContext.IgnorePageBreakReason.InsideHeaderFooter;
			pageContext.EvaluatePageHeaderFooter = false;
			pageContext.Common.Pagination.CurrentColumnWidth = m_pageContext.Common.Pagination.UsablePageWidth;
			pageContext.Common.InHeaderFooter = true;
			double footerHeight = m_sectionPageSettings.FooterHeight;
			pageContext.Common.Pagination.CurrentColumnHeight = footerHeight;
			PageSection pageFooter = m_reportSection.Page.PageFooter;
			m_footer = new PageHeadFoot(pageFooter, m_pageSettings.UsablePageWidth, aIsHeader: false);
			bool anyAncestorHasKT = false;
			m_footer.CalculateVertical(pageContext, 0.0, footerHeight, null, new List<PageItem>(), ref anyAncestorHasKT, hasUnpinnedAncestors: true);
			m_footer.CalculateHorizontal(pageContext, 0.0, m_pageSettings.UsablePageWidth, null, new List<PageItem>(), ref anyAncestorHasKT, hasUnpinnedAncestors: true);
			m_footer.AddToPage(rplWriter, pageContext, 0.0, 0.0, m_pageSettings.UsablePageWidth, footerHeight, PageItem.RepeatState.None);
			pageContext.Common.Pagination.CurrentColumnWidth = columnWidth;
			pageContext.Common.Pagination.CurrentColumnHeight = columnHeight;
			pageContext.Common.InHeaderFooter = false;
		}

		internal void WriteStartItemToStream(RPLWriter rplWriter)
		{
			if (rplWriter != null)
			{
				BinaryWriter binaryWriter = rplWriter.BinaryWriter;
				if (binaryWriter != null)
				{
					Stream baseStream = binaryWriter.BaseStream;
					m_offset = baseStream.Position;
					m_pageOffset = baseStream.Position;
					binaryWriter.Write((byte)21);
					binaryWriter.Write((byte)22);
					binaryWriter.Write((byte)0);
					binaryWriter.Write(m_reportSection.ID);
					binaryWriter.Write((byte)1);
					binaryWriter.Write(m_sectionPageSettings.Columns);
					binaryWriter.Write((byte)2);
					binaryWriter.Write((float)m_sectionPageSettings.ColumnSpacing);
					binaryWriter.Write(byte.MaxValue);
					m_columnsOffset = baseStream.Position;
					binaryWriter.Write((byte)20);
				}
			}
		}

		internal void WriteColumns(RPLWriter rplWriter)
		{
			if (rplWriter == null)
			{
				return;
			}
			BinaryWriter binaryWriter = rplWriter.BinaryWriter;
			if (binaryWriter != null)
			{
				Stream baseStream = binaryWriter.BaseStream;
				long position = baseStream.Position;
				binaryWriter.Write((byte)16);
				binaryWriter.Write(m_columnsOffset);
				binaryWriter.Write(m_columns.Count);
				foreach (ColumnDetail column in m_columns)
				{
					column.WriteMeasurement(binaryWriter);
				}
				m_columnsOffset = baseStream.Position;
				binaryWriter.Write((byte)254);
				binaryWriter.Write(position);
				binaryWriter.Write(byte.MaxValue);
			}
			else
			{
				RPLReportSection rPLReportSection = new RPLReportSection(m_columns.Count);
				rPLReportSection.ID = m_reportSection.ID;
				rPLReportSection.ColumnCount = m_sectionPageSettings.Columns;
				rPLReportSection.ColumnSpacing = (float)m_sectionPageSettings.ColumnSpacing;
				for (int i = 0; i < m_columns.Count; i++)
				{
					rPLReportSection.Columns[i] = m_columns[i].WriteMeasurement();
				}
				m_rplElement = rPLReportSection;
			}
		}

		internal void WriteEndItemToStream(RPLWriter rplWriter)
		{
			if (rplWriter == null)
			{
				return;
			}
			PageHeadFoot header = m_header;
			m_header = null;
			PageHeadFoot footer = m_footer;
			m_footer = null;
			bool needsHeaderHeight = m_needsHeaderHeight;
			m_needsHeaderHeight = false;
			BinaryWriter binaryWriter = rplWriter.BinaryWriter;
			if (binaryWriter != null)
			{
				rplWriter.BinaryWriter.Write(byte.MaxValue);
				Stream baseStream = binaryWriter.BaseStream;
				long position = baseStream.Position;
				binaryWriter.Write((byte)16);
				binaryWriter.Write(m_pageOffset);
				binaryWriter.Write(1 + ((header != null) ? 1 : 0) + ((footer != null) ? 1 : 0));
				binaryWriter.Write((float)m_pageSettings.MarginLeft);
				if (needsHeaderHeight)
				{
					binaryWriter.Write((float)m_sectionPageSettings.HeaderHeight);
				}
				else
				{
					binaryWriter.Write(0f);
				}
				binaryWriter.Write((float)(m_pageSettings.PhysicalPageWidth - m_pageSettings.MarginLeft - m_pageSettings.MarginRight));
				binaryWriter.Write((float)m_columnHeight);
				binaryWriter.Write(0);
				binaryWriter.Write((byte)0);
				binaryWriter.Write(m_columnsOffset);
				header?.WritePageItemSizes(binaryWriter);
				footer?.WritePageItemSizes(binaryWriter);
				m_offset = baseStream.Position;
				binaryWriter.Write((byte)254);
				binaryWriter.Write(position);
				binaryWriter.Write(byte.MaxValue);
			}
			else
			{
				m_rplElement.BodyArea = new RPLMeasurement();
				float top = 0f;
				if (needsHeaderHeight)
				{
					top = (float)m_sectionPageSettings.HeaderHeight;
				}
				m_rplElement.BodyArea.Top = top;
				m_rplElement.BodyArea.Height = (float)m_columnHeight;
				if (header != null)
				{
					m_rplElement.Header = header.WritePageItemSizes();
				}
				if (footer != null)
				{
					m_rplElement.Footer = footer.WritePageItemSizes();
				}
			}
		}

		internal void WriteMeasurement(BinaryWriter spbifWriter)
		{
			spbifWriter.Write((float)m_itemPageSizes.Left);
			spbifWriter.Write((float)m_itemPageSizes.Top);
			spbifWriter.Write((float)m_itemPageSizes.Width);
			spbifWriter.Write((float)m_itemPageSizes.Height);
			spbifWriter.Write(0);
			spbifWriter.Write((byte)0);
			spbifWriter.Write(m_offset);
		}

		internal void AddToPage(RPLPageContent rplPageContent, out RPLMeasurement measurement)
		{
			measurement = new RPLMeasurement();
			measurement.Left = 0f;
			measurement.Top = (float)m_itemPageSizes.Top;
			measurement.Width = (float)m_pageSettings.UsablePageWidth;
			measurement.Height = (float)m_itemPageSizes.Height;
			rplPageContent.AddReportSection(m_rplElement);
		}

		internal RPLPageLayout WritePageLayout(RPLWriter rplWriter, PageContext pageContext)
		{
			RPLPageLayout rPLPageLayout = new RPLPageLayout();
			ReportSectionPage obj = new ReportSectionPage(m_reportSection.Page)
			{
				PageLayout = rPLPageLayout
			};
			rPLPageLayout.PageHeight = (float)pageContext.Common.Pagination.PhysicalPageHeight;
			rPLPageLayout.PageWidth = (float)pageContext.Common.Pagination.PhysicalPageWidth;
			rPLPageLayout.MarginBottom = (float)pageContext.Common.Pagination.MarginBottom;
			rPLPageLayout.MarginLeft = (float)pageContext.Common.Pagination.MarginLeft;
			rPLPageLayout.MarginRight = (float)pageContext.Common.Pagination.MarginRight;
			rPLPageLayout.MarginTop = (float)pageContext.Common.Pagination.MarginTop;
			obj.WriteItemStyle(rplWriter, pageContext);
			return rPLPageLayout;
		}

		public bool HasHeaderOnPage(int page, int totalPages)
		{
			PageSection pageHeader = m_reportSection.Page.PageHeader;
			if (pageHeader != null && ((page > 1 && (totalPages == 0 || page < totalPages)) || (pageHeader.PrintOnFirstPage && page == 1) || (pageHeader.PrintOnLastPage && page == totalPages && page != 1)))
			{
				return true;
			}
			return false;
		}

		public bool HasFooterOnPage(int page, int totalPages)
		{
			PageSection pageFooter = m_reportSection.Page.PageFooter;
			if (pageFooter != null && ((page > 1 && (totalPages == 0 || page < totalPages)) || (pageFooter.PrintOnFirstPage && page == 1 && page != totalPages) || (pageFooter.PrintOnLastPage && page == totalPages)))
			{
				return true;
			}
			return false;
		}
	}
}
