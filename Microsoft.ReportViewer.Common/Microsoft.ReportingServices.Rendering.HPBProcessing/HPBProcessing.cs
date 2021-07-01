using Microsoft.ReportingServices.Diagnostics;
using Microsoft.ReportingServices.Diagnostics.Internal;
using Microsoft.ReportingServices.Diagnostics.Utilities;
using Microsoft.ReportingServices.Interfaces;
using Microsoft.ReportingServices.OnDemandReportRendering;
using Microsoft.ReportingServices.Rendering.RichText;
using Microsoft.ReportingServices.Rendering.RPLProcessing;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Resources;
using System.Text;

namespace Microsoft.ReportingServices.Rendering.HPBProcessing
{
	internal class HPBProcessing : IDisposable
	{
		private PageContext m_pageContext;

		private PaginationSettings m_paginationSettings;

		private List<SectionItemizedData> m_glyphCache;

		private int m_startPage = 1;

		private int m_endPage = 1;

		private int m_totalPages;

		private bool m_createStream = true;

		private CreateAndRegisterStream m_createAndRegisterStream;

		private Report m_report;

		private SelectiveRendering m_selectiveRendering;

		private static Version m_rplVersion;

		public PaginationSettings PaginationSettings => m_paginationSettings;

		internal FontCache SharedFontCache => m_pageContext.Common.FontCache;

		internal List<SectionItemizedData> GlyphCache => m_glyphCache;

		static HPBProcessing()
		{
			m_rplVersion = new Version(10, 6, 0);
		}

		public HPBProcessing(Microsoft.ReportingServices.OnDemandReportRendering.Report report, NameValueCollection deviceInfo, CreateAndRegisterStream createAndRegisterStream, ref Hashtable renderProperties)
		{
			Init(report, new PaginationSettings(report, deviceInfo), createAndRegisterStream, ref renderProperties);
		}

		public HPBProcessing(Microsoft.ReportingServices.OnDemandReportRendering.Report report, PaginationSettings pagination, CreateAndRegisterStream createAndRegisterStream, ref Hashtable renderProperties)
		{
			Init(report, pagination, createAndRegisterStream, ref renderProperties);
		}

		private void Init(Microsoft.ReportingServices.OnDemandReportRendering.Report report, PaginationSettings pagination, CreateAndRegisterStream createAndRegisterStream, ref Hashtable renderProperties)
		{
			m_pageContext = new PageContext(pagination, report.AddToCurrentPage, report.ConsumeContainerWhitespace, createAndRegisterStream);
			m_paginationSettings = pagination;
			m_report = new Report(report, m_pageContext, pagination);
			m_createAndRegisterStream = createAndRegisterStream;
			if (report.SnapshotPageSizeInfo != Microsoft.ReportingServices.OnDemandReportRendering.Report.SnapshotPageSize.Large)
			{
				m_createStream = false;
			}
			if (!string.IsNullOrEmpty(pagination.ReportItemPath))
			{
				m_pageContext.Common.IsInSelectiveRendering = true;
				m_selectiveRendering = new SelectiveRendering(report, m_pageContext, pagination);
			}
			else
			{
				if (m_totalPages > 0)
				{
					return;
				}
				m_totalPages = 0;
				if (report.NeedsOverallTotalPages | report.NeedsPageBreakTotalPages)
				{
					m_pageContext.Common.PauseDiagnostics();
					SetContext(0, 0);
					m_pageContext.PropertyCacheState = PageContext.CacheState.CountPages;
					while (NextPage())
					{
					}
					m_totalPages = m_pageContext.PageNumber;
					m_pageContext.Common.UpdateTotalPagesRegionMapping();
					m_pageContext.Common.ResumeDiagnostics();
					m_pageContext.TextBoxDuplicates = null;
				}
			}
		}

		public void Dispose()
		{
			m_glyphCache = null;
			m_pageContext.DisposeGraphics();
			if (m_report.JobContext != null)
			{
				IJobContext jobContext = m_report.JobContext;
				lock (jobContext.SyncRoot)
				{
					if (jobContext.AdditionalInfo.ScalabilityTime == null)
					{
						jobContext.AdditionalInfo.ScalabilityTime = new ScaleTimeCategory();
					}
					jobContext.AdditionalInfo.ScalabilityTime.Pagination = m_pageContext.TotalScaleTimeMs;
					if (jobContext.AdditionalInfo.EstimatedMemoryUsageKB == null)
					{
						jobContext.AdditionalInfo.EstimatedMemoryUsageKB = new EstimatedMemoryUsageKBCategory();
					}
					jobContext.AdditionalInfo.EstimatedMemoryUsageKB.Pagination = m_pageContext.PeakMemoryUsageKB;
				}
			}
			GC.SuppressFinalize(this);
		}

		private bool NextPage()
		{
			if (m_report.Done)
			{
				return false;
			}
			IncrementPageNumber();
			m_report.NextPage(null, m_totalPages);
			return true;
		}

		private void CreateCacheStream()
		{
			Stream stream = m_pageContext.PropertyCache;
			if (stream == null)
			{
				stream = m_createAndRegisterStream("NonSharedCache", "rpl", null, null, willSeek: true, StreamOper.CreateOnly);
				m_pageContext.PropertyCache = stream;
			}
			stream.Position = 0L;
		}

		public void SetContext()
		{
			SetContext(m_paginationSettings.StartPage, m_paginationSettings.EndPage);
		}

		public void SetContext(int startPage, int endPage)
		{
			if (startPage <= endPage && endPage >= 0)
			{
				m_startPage = startPage;
				m_endPage = endPage;
				if (startPage == 0)
				{
					m_endPage = m_startPage;
				}
				CreateCacheStream();
				if (m_createStream)
				{
					m_pageContext.PropertyCacheState = PageContext.CacheState.RPLStream;
					RSTrace.RenderingTracer.Trace(TraceLevel.Verbose, "RPL stream in use");
				}
				else
				{
					m_pageContext.PropertyCacheState = PageContext.CacheState.RPLObjectModel;
					RSTrace.RenderingTracer.Trace(TraceLevel.Verbose, "RPL object model in use");
				}
				m_report.SetContext();
				ResetPageNumber();
				while (m_pageContext.PageNumber < m_startPage - 1 && !m_report.Done)
				{
					IncrementPageNumber();
					m_report.NextPage(null, m_totalPages);
				}
			}
		}

		public void SetContext(int startPage, int endPage, bool createStream)
		{
			m_createStream = createStream;
			SetContext(startPage, endPage);
		}

		public Stream GetNextPage()
		{
			RPLReport rplReport = null;
			m_createStream = true;
			return GetNextPage(out rplReport);
		}

		public Stream GetNextPage(out RPLReport rplReport)
		{
			rplReport = null;
			m_glyphCache = null;
			if (m_report.Done)
			{
				return null;
			}
			if (m_selectiveRendering != null && m_selectiveRendering.Done)
			{
				return null;
			}
			Stream stream = null;
			RPLWriter rPLWriter = null;
			IncrementPageNumber();
			if (m_startPage >= 0 && m_endPage >= 0 && (m_endPage == 0 || (m_pageContext.PageNumber >= m_startPage && m_pageContext.PageNumber <= m_endPage)))
			{
				rPLWriter = new RPLWriter();
				if (m_createStream)
				{
					string name = m_pageContext.PageNumber.ToString(CultureInfo.InvariantCulture);
					stream = m_createAndRegisterStream(name, "rpl", null, null, willSeek: true, StreamOper.CreateOnly);
					BinaryWriter binaryWriter2 = rPLWriter.BinaryWriter = new BinaryWriter(new BufferedStream(stream), Encoding.Unicode);
					binaryWriter2.Write("RPLIF");
					WriteVersionStamp(binaryWriter2);
				}
			}
			if (m_selectiveRendering != null)
			{
				m_selectiveRendering.RenderReportItem(rPLWriter, m_paginationSettings.ReportItemPath);
			}
			else
			{
				m_report.NextPage(rPLWriter, m_totalPages);
			}
			if (rPLWriter != null)
			{
				m_glyphCache = rPLWriter.GlyphCache;
				if (m_createStream)
				{
					WriteVersionStamp(rPLWriter.BinaryWriter);
					rPLWriter.BinaryWriter.Flush();
					rPLWriter = null;
					BinaryReader reader = new BinaryReader(new BufferedStream(stream), Encoding.Unicode);
					rplReport = new RPLReport(reader);
				}
				else
				{
					rplReport = rPLWriter.Report;
				}
			}
			return stream;
		}

		private void ResetPageNumber()
		{
			m_pageContext.PageNumber = 0;
			m_pageContext.PageNumberRegion = 0;
			m_pageContext.Common.ResetPageBreakProcessing();
			m_pageContext.Common.ResetPageNameProcessing();
			m_pageContext.Common.ResetPageNameTracing();
		}

		private void IncrementPageNumber()
		{
			m_pageContext.Common.ProcessPageBreakProperties();
			if (!m_pageContext.Common.PaginatingHorizontally)
			{
				m_pageContext.Common.ResetPageNameProcessing();
			}
			if (m_pageContext.PageNumber == 0)
			{
				m_pageContext.Common.OverwritePageName(m_report.InitialPageName);
			}
			m_pageContext.PageNumber++;
			m_pageContext.PageNumberRegion++;
		}

		private void WriteVersionStamp(BinaryWriter spbifWriter)
		{
			spbifWriter.Write((byte)m_rplVersion.Major);
			spbifWriter.Write((byte)m_rplVersion.Minor);
			spbifWriter.Write(m_rplVersion.Build);
		}
	}
}
