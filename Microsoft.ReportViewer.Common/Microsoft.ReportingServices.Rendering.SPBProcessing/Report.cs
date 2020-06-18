using Microsoft.ReportingServices.Diagnostics;
using Microsoft.ReportingServices.Diagnostics.Utilities;
using Microsoft.ReportingServices.OnDemandReportRendering;
using Microsoft.ReportingServices.Rendering.RPLProcessing;
using Microsoft.ReportingServices.Rendering.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace Microsoft.ReportingServices.Rendering.SPBProcessing
{
	internal sealed class Report
	{
		private Microsoft.ReportingServices.OnDemandReportRendering.Report m_report;

		private ReportPaginationInfo m_reportInfo;

		private int m_lastSectionIndex = -1;

		private PageContext m_pageContext;

		private long m_offset;

		private long m_pageContentOffset;

		private long m_pageLayoutOffset;

		private long m_bodyOffset;

		private long m_columnsOffset;

		private List<ReportSection> m_sections;

		private List<ItemSizes> m_sectionSizes;

		private List<long> m_sectionOffsets;

		private DocumentMapLabels m_labels;

		private Bookmarks m_bookmarks;

		private bool m_chunksLoaded;

		private bool m_labelsChunkLoaded;

		private Version m_rplVersion;

		internal bool Done
		{
			get
			{
				if (m_sections.Count == 0)
				{
					return true;
				}
				if (m_lastSectionIndex == m_sections.Count - 1)
				{
					return m_sections[m_lastSectionIndex].Done;
				}
				return false;
			}
		}

		internal Version RPLVersion => m_rplVersion;

		internal double InteractiveHeight => m_report.ReportSections[0].Page.InteractiveHeight.ToMillimeters();

		internal IJobContext JobContext => m_report.JobContext;

		internal Report(Microsoft.ReportingServices.OnDemandReportRendering.Report report, PageContext pageContext, string rplVersion, bool defaultVersion)
		{
			m_report = report;
			m_pageContext = pageContext;
			if (!string.IsNullOrEmpty(rplVersion))
			{
				char[] separator = new char[1]
				{
					'.'
				};
				string[] array = rplVersion.Split(separator);
				if (array.Length < 2)
				{
					m_rplVersion = new Version(10, 3, 0);
				}
				else
				{
					int major = SPBProcessing.ParseInt(array[0], 10);
					int minor = SPBProcessing.ParseInt(array[1], 3);
					int build = 0;
					if (array.Length > 2)
					{
						build = SPBProcessing.ParseInt(array[2], 0);
					}
					m_rplVersion = new Version(major, minor, build);
				}
			}
			else if (defaultVersion)
			{
				m_rplVersion = new Version(10, 3, 0);
			}
			else
			{
				m_rplVersion = new Version(10, 6, 0);
			}
			m_pageContext.VersionPicker = RPLReader.CompareRPLVersions(m_rplVersion);
		}

		internal void LoadInteractiveChunks(int page)
		{
			if (!m_chunksLoaded)
			{
				m_chunksLoaded = true;
				if (m_report.HasBookmarks)
				{
					m_bookmarks = InteractivityChunks.GetBookmarksStream(m_report, page);
				}
				if (!m_labelsChunkLoaded && m_report.HasDocumentMap)
				{
					m_labels = InteractivityChunks.GetLabelsStream(m_report, page);
				}
			}
		}

		internal void LoadLabelsChunk()
		{
			if (!m_chunksLoaded && !m_labelsChunkLoaded)
			{
				m_labelsChunkLoaded = true;
				if (m_report.HasDocumentMap)
				{
					m_labels = InteractivityChunks.GetLabelsStream(m_report, 1);
				}
			}
		}

		internal void UnloadInteractiveChunks()
		{
			if (m_bookmarks != null)
			{
				m_bookmarks.Flush(Done);
				m_bookmarks = null;
			}
			if (m_labels != null)
			{
				m_labels.Flush(Done);
				m_labels = null;
			}
			m_labelsChunkLoaded = false;
			m_chunksLoaded = false;
		}

		internal bool RegisterPageForCollect(int page, bool collectPageBookmarks)
		{
			bool flag = false;
			if (m_report.HasBookmarks)
			{
				if (m_bookmarks != null && page == m_bookmarks.Page + 1)
				{
					flag = true;
					m_bookmarks.Page = page;
					m_pageContext.Bookmarks = m_bookmarks;
				}
				if (collectPageBookmarks)
				{
					flag = true;
					m_pageContext.PageBookmarks = new Dictionary<string, string>();
				}
			}
			return flag | RegisterPageLabelsForCollect(page);
		}

		internal bool RegisterPageLabelsForCollect(int page)
		{
			bool result = false;
			if (m_report.HasDocumentMap && m_labels != null && page == m_labels.Page + 1)
			{
				result = true;
				m_labels.Page = page;
				m_pageContext.Labels = m_labels;
			}
			return result;
		}

		internal void UnregisterPageForCollect()
		{
			m_pageContext.Labels = null;
			m_pageContext.Bookmarks = null;
		}

		internal void DisposeDelayTextBox()
		{
			m_pageContext.Common.DisposeDelayTextBox();
		}

		internal void ResetSectionsOnPage()
		{
			if (m_sections != null)
			{
				ResetSectionsOnPage(0, m_sections.Count - 1);
			}
		}

		internal void SetContext(ReportPaginationInfo reportInfo)
		{
			if (m_sections == null)
			{
				int count = m_report.ReportSections.Count;
				m_sections = new List<ReportSection>(count);
				m_sectionOffsets = new List<long>(count);
				m_sectionSizes = new List<ItemSizes>(count);
				foreach (Microsoft.ReportingServices.OnDemandReportRendering.ReportSection reportSection in m_report.ReportSections)
				{
					ReportSection item = new ReportSection(reportSection, m_pageContext);
					m_sections.Add(item);
				}
			}
			m_reportInfo = reportInfo;
			m_lastSectionIndex = -1;
		}

		internal void NextPage(RPLWriter rplWriter, ref ReportSectionHelper lastPageInfo, int page, int totalPages, Interactivity interactivity, bool hasPaginationChunk)
		{
			ReportSection reportSection = null;
			bool flag = true;
			bool flag2 = false;
			bool flag3 = false;
			bool flag4 = (m_sections.Count <= 1) ? true : false;
			PageItemHelper lastBodyInfo = null;
			int pageNumber = page;
			int totalPages2 = totalPages;
			int sectionStartIndex = -1;
			CreateFirstSectionBodyFromPaginationState(page, lastPageInfo, ref lastBodyInfo, ref sectionStartIndex);
			if (m_pageContext.ImageConsolidation != null)
			{
				m_pageContext.ImageConsolidation.Reset();
				m_pageContext.ImageConsolidation.SetName(m_report.Name, page);
			}
			m_pageContext.PageNumber = page;
			m_pageContext.PageTotalInfo.RegisterPageNumberForStart(page);
			m_pageContext.PageTotalInfo.RetrievePageBreakData(page, out pageNumber, out totalPages2);
			WriteStartItemToStream(rplWriter);
			WriteReportPageLayoutAtStart(rplWriter);
			for (int i = sectionStartIndex; i < m_sections.Count; i++)
			{
				ReportSection reportSection2 = m_sections[i];
				bool delayedHeader = false;
				bool delayedFooter = false;
				bool lastSectionOnPage = false;
				if (flag)
				{
					reportSection = reportSection2;
				}
				if (i == m_sections.Count - 1)
				{
					flag4 = true;
				}
				if (reportSection2.Body == null)
				{
					reportSection2.SetContext();
				}
				reportSection2.CalculatePage(rplWriter, page, totalPages, pageNumber, totalPages2, flag, flag4, interactivity, m_pageContext.PageHeight, ref lastBodyInfo, ref delayedHeader, ref delayedFooter, ref lastSectionOnPage);
				if (m_pageContext.CancelPage)
				{
					ResetSectionsOnPage(sectionStartIndex, i);
					return;
				}
				if (flag)
				{
					flag = false;
					flag2 = delayedHeader;
					lastPageInfo = null;
				}
				if (delayedFooter)
				{
					flag3 = delayedFooter;
				}
				m_sectionSizes.Add(reportSection2.ItemRenderSizes);
				m_sectionOffsets.Add(reportSection2.Offset);
				if (m_pageContext.PageHeight != double.MaxValue)
				{
					m_pageContext.PageHeight -= reportSection2.Body.ItemPageSizes.Height;
				}
				if (lastSectionOnPage)
				{
					if (i > m_lastSectionIndex)
					{
						m_lastSectionIndex = i;
					}
					reportSection2.SectionIndex = i;
					break;
				}
			}
			if (m_pageContext.TracingEnabled)
			{
				if (Done)
				{
					TracePageBreakIgnoredAtBottom(page, m_pageContext.Common.PageBreakInfo);
				}
				else if (m_pageContext.Common.PageBreakInfo != null)
				{
					TraceLogicalPageBreak(page + 1, m_pageContext.Common.PageBreakInfo);
				}
				else
				{
					TraceVerticalPageBreak(page + 1);
				}
			}
			m_pageContext.ApplyPageBreak(page);
			int num = 1;
			double bodyWidth = 0.0;
			double bodyHeight = 0.0;
			bool flag5 = flag4 && m_sections[m_lastSectionIndex].Done;
			if (m_pageContext.VersionPicker == RPLVersionEnum.RPL2008 || m_pageContext.VersionPicker == RPLVersionEnum.RPL2008WithImageConsolidation)
			{
				WriteEndReportBodyToRPLStream2008(rplWriter, sectionStartIndex, ref bodyWidth, ref bodyHeight);
				if (rplWriter != null || (interactivity != null && !interactivity.Done && interactivity.NeedPageHeaderFooter))
				{
					WriteReportPageLayout2008(rplWriter, bodyWidth, bodyHeight);
					if (flag3)
					{
						m_sections[m_lastSectionIndex].CalculateDelayedFooter(rplWriter, interactivity);
						num++;
					}
					if (flag2 && (page == 1 || !flag5 || reportSection.IsHeaderPrintOnLastPage()))
					{
						reportSection.CalculateDelayedHeader(rplWriter, interactivity);
						num++;
					}
					if (rplWriter != null && rplWriter.BinaryWriter != null)
					{
						rplWriter.BinaryWriter.Write(byte.MaxValue);
					}
				}
			}
			else if (flag2)
			{
				if (!flag5)
				{
					CalculateDelayedHeader(rplWriter, reportSection, interactivity);
				}
				else
				{
					reportSection.WriteEndItemToStream(rplWriter);
					if (rplWriter != null)
					{
						m_sectionSizes[0].Height = reportSection.ItemRenderSizes.Height;
						m_sectionSizes[0].Width = reportSection.ItemRenderSizes.Width;
						m_sectionOffsets[0] = reportSection.Offset;
					}
				}
			}
			interactivity?.RegisterDocMapRootLabel(m_report.Instance.UniqueName, m_pageContext);
			if (m_pageContext.ImageConsolidation != null)
			{
				m_pageContext.ImageConsolidation.RenderToStream();
			}
			string pageName = m_pageContext.PageTotalInfo.GetPageName(page);
			WriteEndItemToStream(rplWriter, sectionStartIndex, num, reportSection.Header, m_sections[m_lastSectionIndex].Footer, bodyWidth, bodyHeight, pageName);
			m_pageContext.PageHeight = m_pageContext.OriginalPageHeight;
			if (hasPaginationChunk)
			{
				ReleaseResourcesOnPage(rplWriter, sectionStartIndex, includeLastSection: false);
			}
			else
			{
				ReleaseResourcesOnPage(rplWriter, sectionStartIndex, includeLastSection: true);
			}
		}

		internal void WriteStartItemToStream(RPLWriter rplWriter)
		{
			if (rplWriter == null)
			{
				return;
			}
			BinaryWriter binaryWriter = rplWriter.BinaryWriter;
			string language = null;
			ReportStringProperty language2 = m_report.Language;
			if (language2 != null)
			{
				language = ((!language2.IsExpression) ? language2.Value : m_report.Instance.Language);
			}
			if (binaryWriter != null)
			{
				if (m_pageContext.VersionPicker == RPLVersionEnum.RPL2008 || m_pageContext.VersionPicker == RPLVersionEnum.RPL2008WithImageConsolidation)
				{
					WriteStartItemToRPLStream2008(binaryWriter, language);
				}
				else
				{
					WriteStartItemToRPLStream(binaryWriter, language);
				}
			}
			else
			{
				WriteStartItemToRPLOM(rplWriter, language);
			}
		}

		internal void WriteEndItemToStream(RPLWriter rplWriter, int sectionStartIndex, int itemsOnPage, PageHeadFoot header, PageHeadFoot footer, double bodyWidth, double bodyHeight, string pageName)
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
					WriteEndItemToRPLStream2008(binaryWriter, itemsOnPage, header, footer, bodyWidth, bodyHeight);
				}
				else
				{
					WriteEndItemToRPLStream(binaryWriter, pageName);
				}
			}
			else
			{
				WriteEndItemToRPLOM(rplWriter, sectionStartIndex, pageName);
			}
		}

		internal void UpdatePagination()
		{
			if (m_sections != null && m_sections.Count > 0 && m_reportInfo.BinaryWriter != null)
			{
				m_reportInfo.BinaryWriter.BaseStream.Seek(m_reportInfo.OffsetLastPage, SeekOrigin.Begin);
				m_sections[m_lastSectionIndex].WritePaginationInfo(m_reportInfo.BinaryWriter);
				m_reportInfo.UpdateReportInfo();
				m_sections[m_lastSectionIndex].Reset();
			}
		}

		internal void ResetLastSection()
		{
			if (m_sections != null && m_lastSectionIndex >= 0 && m_lastSectionIndex < m_sections.Count)
			{
				m_sections[m_lastSectionIndex].Reset();
			}
		}

		internal ReportSectionHelper GetPaginationInfo()
		{
			if (m_sections != null && m_sections.Count > 0 && m_lastSectionIndex >= 0 && m_lastSectionIndex < m_sections.Count)
			{
				return m_sections[m_lastSectionIndex].WritePaginationInfo();
			}
			return null;
		}

		private void CreateFirstSectionBodyFromPaginationState(int page, ReportSectionHelper lastPageInfo, ref PageItemHelper lastBodyInfo, ref int sectionStartIndex)
		{
			if (lastPageInfo != null)
			{
				sectionStartIndex = lastPageInfo.SectionIndex;
				m_sections[sectionStartIndex].UpdateItem(lastPageInfo);
				if (lastPageInfo.BodyHelper != null)
				{
					lastBodyInfo = lastPageInfo.BodyHelper;
				}
				else
				{
					sectionStartIndex++;
				}
				return;
			}
			if (page == 1)
			{
				sectionStartIndex = 0;
				m_sections[sectionStartIndex].Reset();
				return;
			}
			RSTrace.RenderingTracer.Assert(m_lastSectionIndex >= 0 && m_lastSectionIndex < m_sections.Count, "The index of the last section on the previous paginated page should be a valid index");
			if (m_sections[m_lastSectionIndex].Done && m_lastSectionIndex < m_sections.Count - 1)
			{
				m_sections[m_lastSectionIndex].Reset();
				sectionStartIndex = m_lastSectionIndex + 1;
			}
			else
			{
				sectionStartIndex = m_lastSectionIndex;
				m_sections[sectionStartIndex].Reset();
			}
		}

		private void CalculateDelayedHeader(RPLWriter rplWriter, ReportSection firstSection, Interactivity interactivity)
		{
			firstSection.CalculateDelayedHeader(rplWriter, interactivity);
			firstSection.UpdateReportSectionSizes(rplWriter);
			firstSection.WriteEndItemToStream(rplWriter);
			if (rplWriter != null)
			{
				m_sectionSizes[0].Height = firstSection.ItemRenderSizes.Height;
				m_sectionSizes[0].Width = firstSection.ItemRenderSizes.Width;
				m_sectionOffsets[0] = firstSection.Offset;
			}
		}

		private void ReleaseResourcesOnPage(RPLWriter rplWriter, int sectionStartIndex, bool includeLastSection)
		{
			if (includeLastSection)
			{
				ResetSectionsOnPage(sectionStartIndex, m_lastSectionIndex);
			}
			else
			{
				ResetSectionsOnPage(sectionStartIndex, m_lastSectionIndex - 1);
			}
			if (rplWriter != null && rplWriter.BinaryWriter != null)
			{
				m_pageContext.SharedImages = null;
				m_pageContext.ItemPropsStart = null;
			}
		}

		private void ResetSectionsOnPage(int sectionStartIndex, int sectionEndIndex)
		{
			for (int i = sectionStartIndex; i <= sectionEndIndex; i++)
			{
				m_sections[i].Reset();
			}
			m_sectionOffsets.Clear();
			m_sectionSizes.Clear();
		}

		private void WriteStartItemToRPLStream(BinaryWriter spbifWriter, string language)
		{
			Stream baseStream = spbifWriter.BaseStream;
			m_offset = baseStream.Position;
			spbifWriter.Write((byte)0);
			spbifWriter.Write((byte)2);
			if (m_report.Name != null)
			{
				spbifWriter.Write((byte)15);
				spbifWriter.Write(m_report.Name);
			}
			if (m_report.Description != null)
			{
				spbifWriter.Write((byte)9);
				spbifWriter.Write(m_report.Description);
			}
			if (m_report.Author != null)
			{
				spbifWriter.Write((byte)13);
				spbifWriter.Write(m_report.Author);
			}
			if (m_report.AutoRefresh > 0)
			{
				spbifWriter.Write((byte)14);
				spbifWriter.Write(m_report.AutoRefresh);
			}
			DateTime executionTime = m_report.ExecutionTime;
			spbifWriter.Write((byte)12);
			spbifWriter.Write(executionTime.ToBinary());
			ReportUrl location = m_report.Location;
			if (location != null)
			{
				spbifWriter.Write((byte)10);
				spbifWriter.Write(location.ToString());
			}
			if (language != null)
			{
				spbifWriter.Write((byte)11);
				spbifWriter.Write(language);
			}
			if (m_report.ConsumeContainerWhitespace)
			{
				spbifWriter.Write((byte)50);
				spbifWriter.Write(m_report.ConsumeContainerWhitespace);
			}
			spbifWriter.Write(byte.MaxValue);
			m_pageContentOffset = baseStream.Position;
			spbifWriter.Write((byte)19);
		}

		private void WriteStartItemToRPLStream2008(BinaryWriter spbifWriter, string language)
		{
			Stream baseStream = spbifWriter.BaseStream;
			m_offset = baseStream.Position;
			spbifWriter.Write((byte)0);
			spbifWriter.Write((byte)2);
			if (m_report.Name != null)
			{
				spbifWriter.Write((byte)15);
				spbifWriter.Write(m_report.Name);
			}
			if (m_report.Description != null)
			{
				spbifWriter.Write((byte)9);
				spbifWriter.Write(m_report.Description);
			}
			if (m_report.Author != null)
			{
				spbifWriter.Write((byte)13);
				spbifWriter.Write(m_report.Author);
			}
			if (m_report.AutoRefresh > 0)
			{
				spbifWriter.Write((byte)14);
				spbifWriter.Write(m_report.AutoRefresh);
			}
			DateTime executionTime = m_report.ExecutionTime;
			spbifWriter.Write((byte)12);
			spbifWriter.Write(executionTime.ToBinary());
			ReportUrl location = m_report.Location;
			if (location != null)
			{
				spbifWriter.Write((byte)10);
				spbifWriter.Write(location.ToString());
			}
			if (language != null)
			{
				spbifWriter.Write((byte)11);
				spbifWriter.Write(language);
			}
			spbifWriter.Write(byte.MaxValue);
			m_pageContentOffset = baseStream.Position;
			spbifWriter.Write((byte)19);
			m_columnsOffset = baseStream.Position;
			spbifWriter.Write((byte)20);
			WriteStartReportBodyToRPLStream2008(spbifWriter);
		}

		private void WriteStartReportBodyToRPLStream2008(BinaryWriter spbifWriter)
		{
			Stream baseStream = spbifWriter.BaseStream;
			m_bodyOffset = baseStream.Position;
			spbifWriter.Write((byte)6);
			spbifWriter.Write((byte)15);
			spbifWriter.Write((byte)0);
			spbifWriter.Write(byte.MaxValue);
			spbifWriter.Write((byte)1);
			spbifWriter.Write(byte.MaxValue);
			spbifWriter.Write(byte.MaxValue);
		}

		private void WriteStartItemToRPLOM(RPLWriter rplWriter, string language)
		{
			RPLReport rPLReport = new RPLReport();
			rPLReport.ReportName = m_report.Name;
			rPLReport.Description = m_report.Description;
			rPLReport.Author = m_report.Author;
			rPLReport.AutoRefresh = m_report.AutoRefresh;
			rPLReport.ExecutionTime = m_report.ExecutionTime;
			rPLReport.Location = m_report.Location.ToString();
			rPLReport.Language = language;
			rPLReport.RPLVersion = m_rplVersion;
			rPLReport.ConsumeContainerWhitespace = m_report.ConsumeContainerWhitespace;
			rPLReport.RPLPaginatedPages = new RPLPageContent[1];
			rPLReport.RPLPaginatedPages[0] = new RPLPageContent(1);
			rplWriter.Report = rPLReport;
		}

		private void WriteEndItemToRPLStream(BinaryWriter spbifWriter, string pageName)
		{
			spbifWriter.Write(byte.MaxValue);
			Stream baseStream = spbifWriter.BaseStream;
			long position = baseStream.Position;
			spbifWriter.Write((byte)16);
			spbifWriter.Write(m_pageContentOffset);
			spbifWriter.Write(m_sectionSizes.Count);
			for (int i = 0; i < m_sectionSizes.Count; i++)
			{
				spbifWriter.Write((float)m_sectionSizes[i].Left);
				spbifWriter.Write((float)m_sectionSizes[i].Top);
				spbifWriter.Write((float)m_sectionSizes[i].Width);
				spbifWriter.Write((float)m_sectionSizes[i].Height);
				spbifWriter.Write(0);
				spbifWriter.Write((byte)0);
				spbifWriter.Write(m_sectionOffsets[i]);
			}
			WriteReportPageLayoutAtEnd(spbifWriter, pageName);
			m_pageContentOffset = baseStream.Position;
			spbifWriter.Write((byte)254);
			spbifWriter.Write(position);
			spbifWriter.Write(byte.MaxValue);
			long position2 = baseStream.Position;
			spbifWriter.Write((byte)18);
			spbifWriter.Write(m_offset);
			spbifWriter.Write(1);
			spbifWriter.Write(m_pageContentOffset);
			spbifWriter.Write((byte)254);
			spbifWriter.Write(position2);
			spbifWriter.Write(byte.MaxValue);
		}

		private void WriteEndItemToRPLStream2008(BinaryWriter spbifWriter, int itemsOnPage, PageHeadFoot header, PageHeadFoot footer, double reportWidth, double reportHeight)
		{
			Stream baseStream = spbifWriter.BaseStream;
			if (header != null)
			{
				reportWidth = Math.Max(reportWidth, header.ItemRenderSizes.Width);
			}
			if (footer != null)
			{
				reportWidth = Math.Max(reportWidth, footer.ItemRenderSizes.Width);
			}
			long position = baseStream.Position;
			spbifWriter.Write((byte)16);
			spbifWriter.Write(m_pageContentOffset);
			spbifWriter.Write(itemsOnPage);
			spbifWriter.Write(0f);
			spbifWriter.Write(0f);
			spbifWriter.Write((float)reportWidth);
			spbifWriter.Write((float)reportHeight);
			spbifWriter.Write(0);
			spbifWriter.Write((byte)0);
			spbifWriter.Write(m_columnsOffset);
			if (header != null)
			{
				header.ItemRenderSizes.Width = reportWidth;
				header.WritePageItemRenderSizes(spbifWriter);
			}
			if (footer != null)
			{
				footer.ItemRenderSizes.Width = reportWidth;
				footer.WritePageItemRenderSizes(spbifWriter);
			}
			m_pageContentOffset = baseStream.Position;
			spbifWriter.Write((byte)254);
			spbifWriter.Write(position);
			spbifWriter.Write(byte.MaxValue);
			long position2 = baseStream.Position;
			spbifWriter.Write((byte)18);
			spbifWriter.Write(m_offset);
			spbifWriter.Write(1);
			spbifWriter.Write(m_pageContentOffset);
			spbifWriter.Write((byte)254);
			spbifWriter.Write(position2);
			spbifWriter.Write(byte.MaxValue);
		}

		private void WriteEndItemToRPLOM(RPLWriter rplWriter, int sectionStartIndex, string pageName)
		{
			RPLPageContent rPLPageContent = rplWriter.Report.RPLPaginatedPages[0];
			rPLPageContent.PageLayout.PageName = pageName;
			RPLSizes[] array = new RPLSizes[m_sectionSizes.Count];
			for (int i = 0; i < m_sectionSizes.Count; i++)
			{
				array[i] = new RPLSizes((float)m_sectionSizes[i].Top, (float)m_sectionSizes[i].Left, (float)m_sectionSizes[i].Height, (float)m_sectionSizes[i].Width);
			}
			rPLPageContent.ReportSectionSizes = array;
			for (int j = sectionStartIndex; j <= m_lastSectionIndex; j++)
			{
				rPLPageContent.AddReportSection(m_sections[j].RPLReportSection);
			}
		}

		private void WriteReportPageLayoutAtEnd(BinaryWriter spbifWriter, string pageName)
		{
			if ((int)m_pageContext.VersionPicker > 3 && pageName != null)
			{
				spbifWriter.Write((byte)3);
				spbifWriter.Write((byte)48);
				spbifWriter.Write(pageName);
				spbifWriter.Write(byte.MaxValue);
			}
		}

		private void WriteReportPageLayoutAtStart(RPLWriter rplWriter)
		{
			if (m_pageContext.VersionPicker != 0 && m_pageContext.VersionPicker != RPLVersionEnum.RPL2008WithImageConsolidation && rplWriter != null)
			{
				BinaryWriter binaryWriter = rplWriter.BinaryWriter;
				Page page = m_report.ReportSections[0].Page;
				if (binaryWriter != null)
				{
					Stream baseStream = binaryWriter.BaseStream;
					m_pageLayoutOffset = baseStream.Position;
					binaryWriter.Write((byte)3);
					binaryWriter.Write((byte)16);
					binaryWriter.Write((float)page.PageHeight.ToMillimeters());
					binaryWriter.Write((byte)17);
					binaryWriter.Write((float)page.PageWidth.ToMillimeters());
					binaryWriter.Write((byte)20);
					binaryWriter.Write((float)page.BottomMargin.ToMillimeters());
					binaryWriter.Write((byte)19);
					binaryWriter.Write((float)page.LeftMargin.ToMillimeters());
					binaryWriter.Write((byte)21);
					binaryWriter.Write((float)page.RightMargin.ToMillimeters());
					binaryWriter.Write((byte)18);
					binaryWriter.Write((float)page.TopMargin.ToMillimeters());
					new ReportPageLayout(page).WriteElementStyle(rplWriter, m_pageContext);
					binaryWriter.Write(byte.MaxValue);
				}
				else
				{
					RPLPageLayout rPLPageLayout = new RPLPageLayout();
					rplWriter.Report.RPLPaginatedPages[0].PageLayout = rPLPageLayout;
					rPLPageLayout.PageHeight = (float)page.PageHeight.ToMillimeters();
					rPLPageLayout.PageWidth = (float)page.PageWidth.ToMillimeters();
					rPLPageLayout.MarginBottom = (float)page.BottomMargin.ToMillimeters();
					rPLPageLayout.MarginLeft = (float)page.LeftMargin.ToMillimeters();
					rPLPageLayout.MarginRight = (float)page.RightMargin.ToMillimeters();
					rPLPageLayout.MarginTop = (float)page.TopMargin.ToMillimeters();
					new ReportPageLayout(page).WriteElementStyle(rplWriter, m_pageContext);
				}
			}
		}

		private void WriteReportPageLayout2008(RPLWriter rplWriter, double bodyWidth, double bodyHeight)
		{
			if (rplWriter != null)
			{
				BinaryWriter binaryWriter = rplWriter.BinaryWriter;
				Microsoft.ReportingServices.OnDemandReportRendering.ReportSection reportSection = m_report.ReportSections[0];
				Page page = reportSection.Page;
				if (binaryWriter != null)
				{
					Stream baseStream = binaryWriter.BaseStream;
					long position = baseStream.Position;
					binaryWriter.Write((byte)16);
					binaryWriter.Write(m_columnsOffset);
					binaryWriter.Write(1);
					binaryWriter.Write(0f);
					binaryWriter.Write(0f);
					binaryWriter.Write((float)bodyWidth);
					binaryWriter.Write((float)bodyHeight);
					binaryWriter.Write(0);
					binaryWriter.Write((byte)0);
					binaryWriter.Write(m_bodyOffset);
					m_columnsOffset = baseStream.Position;
					binaryWriter.Write((byte)254);
					binaryWriter.Write(position);
					binaryWriter.Write(byte.MaxValue);
					binaryWriter.Write((byte)1);
					binaryWriter.Write((byte)3);
					binaryWriter.Write((byte)1);
					binaryWriter.Write(reportSection.ID);
					binaryWriter.Write((byte)0);
					binaryWriter.Write(page.Instance.UniqueName);
					binaryWriter.Write((byte)16);
					binaryWriter.Write((float)page.PageHeight.ToMillimeters());
					binaryWriter.Write((byte)17);
					binaryWriter.Write((float)page.PageWidth.ToMillimeters());
					binaryWriter.Write((byte)20);
					binaryWriter.Write((float)page.BottomMargin.ToMillimeters());
					binaryWriter.Write((byte)19);
					binaryWriter.Write((float)page.LeftMargin.ToMillimeters());
					binaryWriter.Write((byte)21);
					binaryWriter.Write((float)page.RightMargin.ToMillimeters());
					binaryWriter.Write((byte)18);
					binaryWriter.Write((float)page.TopMargin.ToMillimeters());
					binaryWriter.Write((byte)23);
					binaryWriter.Write(page.Columns);
					binaryWriter.Write((byte)22);
					binaryWriter.Write((float)page.ColumnSpacing.ToMillimeters());
					new ReportPageLayout(page).WriteElementStyle(rplWriter, m_pageContext);
					binaryWriter.Write(byte.MaxValue);
				}
				else
				{
					RPLPageLayout rPLPageLayout = new RPLPageLayout();
					rplWriter.Report.RPLPaginatedPages[0].PageLayout = rPLPageLayout;
					rPLPageLayout.PageHeight = (float)page.PageHeight.ToMillimeters();
					rPLPageLayout.PageWidth = (float)page.PageWidth.ToMillimeters();
					rPLPageLayout.MarginBottom = (float)page.BottomMargin.ToMillimeters();
					rPLPageLayout.MarginLeft = (float)page.LeftMargin.ToMillimeters();
					rPLPageLayout.MarginRight = (float)page.RightMargin.ToMillimeters();
					rPLPageLayout.MarginTop = (float)page.TopMargin.ToMillimeters();
					new ReportPageLayout(page).WriteElementStyle(rplWriter, m_pageContext);
				}
			}
		}

		private void WriteEndReportBodyToRPLStream2008(RPLWriter rplWriter, int sectionStartIndex, ref double bodyWidth, ref double bodyHeight)
		{
			if (rplWriter == null)
			{
				return;
			}
			bodyWidth = 0.0;
			bodyHeight = 0.0;
			BinaryWriter binaryWriter = rplWriter.BinaryWriter;
			if (binaryWriter != null)
			{
				Stream baseStream = binaryWriter.BaseStream;
				long position = baseStream.Position;
				binaryWriter.Write((byte)16);
				binaryWriter.Write(m_bodyOffset);
				binaryWriter.Write(m_sectionSizes.Count);
				for (int i = 0; i < m_sectionSizes.Count; i++)
				{
					binaryWriter.Write((float)m_sectionSizes[i].Left);
					binaryWriter.Write((float)(m_sectionSizes[i].Top + bodyHeight));
					binaryWriter.Write((float)m_sectionSizes[i].Width);
					binaryWriter.Write((float)m_sectionSizes[i].Height);
					binaryWriter.Write(0);
					binaryWriter.Write((byte)0);
					binaryWriter.Write(m_sectionOffsets[i]);
					bodyWidth = Math.Max(bodyWidth, m_sectionSizes[i].Width);
					bodyHeight += m_sectionSizes[i].Height;
				}
				m_bodyOffset = baseStream.Position;
				binaryWriter.Write((byte)254);
				binaryWriter.Write(position);
				binaryWriter.Write(byte.MaxValue);
			}
		}

		private static void TraceLogicalPageBreak(int pageNumber, PageBreakInfo pageBreak)
		{
			if (pageBreak != null && !pageBreak.Disabled)
			{
				string text = "PR-DIAG [Page {0}] Page created by {1} page break";
				if (pageBreak.ResetPageNumber)
				{
					text += ". Page number reset";
				}
				RenderingDiagnostics.Trace(RenderingArea.PageCreation, TraceLevel.Verbose, text, pageNumber, PageCreationType.Logical.ToString());
			}
		}

		private static void TraceVerticalPageBreak(int pageNumber)
		{
			RenderingDiagnostics.Trace(RenderingArea.PageCreation, TraceLevel.Verbose, "PR-DIAG [Page {0}] Page created by {1} page break", pageNumber, PageCreationType.Vertical.ToString());
		}

		private static void TracePageBreakIgnoredAtBottom(int pageNumber, PageBreakInfo pageBreak)
		{
			if (pageBreak != null && !pageBreak.Disabled)
			{
				RenderingDiagnostics.Trace(RenderingArea.PageCreation, TraceLevel.Verbose, "PR-DIAG [Page {0}] Page break on '{1}' ignored – bottom of page", pageNumber, pageBreak.ReportItemName);
			}
		}
	}
}
