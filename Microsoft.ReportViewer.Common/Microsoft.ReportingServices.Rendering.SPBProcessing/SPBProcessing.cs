using Microsoft.ReportingServices.Diagnostics;
using Microsoft.ReportingServices.Diagnostics.Internal;
using Microsoft.ReportingServices.Diagnostics.Utilities;
using Microsoft.ReportingServices.Interfaces;
using Microsoft.ReportingServices.OnDemandReportRendering;
using Microsoft.ReportingServices.Rendering.RichText;
using Microsoft.ReportingServices.Rendering.RPLProcessing;
using Microsoft.ReportingServices.ReportProcessing;
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

namespace Microsoft.ReportingServices.Rendering.SPBProcessing
{
	internal class SPBProcessing : ISPBProcessing, IDisposable
	{
		internal enum RPLState : byte
		{
			RPLObjectModel,
			RPLStream,
			Unknown
		}

		private CreateAndRegisterStream m_createAndRegisterStream;

		private Report m_report;

		private PageContext m_pageContext;

		private RPLState m_rplState = RPLState.Unknown;

		private bool m_useInteractiveHeight = true;

		private int m_startPage = 1;

		private int m_endPage = 1;

		private int m_currentPage;

		private int m_totalPages;

		private ReportPaginationInfo m_reportInfo;

		private ReportSectionHelper m_lastPageInfo;

		private ReportSectionHelper m_lastPageInfoForCancel;

		private Version m_serverRPLVersion = new Version(10, 6, 0);

		public bool Done
		{
			get
			{
				if (m_report == null)
				{
					return false;
				}
				return m_report.Done;
			}
		}

		public Dictionary<string, string> PageBookmarks
		{
			get
			{
				if (m_pageContext == null)
				{
					return null;
				}
				return m_pageContext.PageBookmarks;
			}
		}

		internal FontCache SharedFontCache => m_pageContext.Common.FontCache;

		internal bool UseEmSquare
		{
			get
			{
				return m_pageContext.Common.EmSquare;
			}
			set
			{
				m_pageContext.Common.EmSquare = value;
			}
		}

		internal bool CanTracePagination
		{
			get
			{
				return m_pageContext.CanTracePagination;
			}
			set
			{
				m_pageContext.CanTracePagination = value;
			}
		}

		public SPBProcessing(Microsoft.ReportingServices.OnDemandReportRendering.Report report, CreateAndRegisterStream createAndRegisterStream, double pageHeight)
		{
			m_pageContext = new PageContext(report.Instance.InitialPageName, pageHeight, registerEvents: false, report.ConsumeContainerWhitespace, createAndRegisterStream);
			m_report = new Report(report, m_pageContext, null, defaultVersion: false);
			m_createAndRegisterStream = createAndRegisterStream;
			if (report.SnapshotPageSizeInfo == Microsoft.ReportingServices.OnDemandReportRendering.Report.SnapshotPageSize.Large)
			{
				m_rplState = RPLState.RPLStream;
			}
			else if (report.SnapshotPageSizeInfo == Microsoft.ReportingServices.OnDemandReportRendering.Report.SnapshotPageSize.Small)
			{
				m_rplState = RPLState.RPLObjectModel;
			}
			m_reportInfo = new ReportPaginationInfo();
			m_useInteractiveHeight = false;
			if (m_totalPages <= 0)
			{
				if (m_reportInfo.IsDone)
				{
					m_totalPages = m_reportInfo.PaginatedPages;
				}
				else
				{
					m_totalPages = 0;
					if (report.NeedsTotalPages)
					{
						SetContext(new SPBContext());
					}
				}
			}
			m_pageContext.CanTracePagination = true;
		}

		public SPBProcessing(Microsoft.ReportingServices.OnDemandReportRendering.Report report, CreateAndRegisterStream createAndRegisterStream, bool registerEvents, string rplVersion, ref Hashtable renderProperties)
		{
			double pageHeight = report.ReportSections[0].Page.InteractiveHeight.ToMillimeters();
			m_pageContext = new PageContext(report.Instance.InitialPageName, pageHeight, registerEvents, report.ConsumeContainerWhitespace, createAndRegisterStream);
			m_report = new Report(report, m_pageContext, rplVersion, defaultVersion: true);
			m_reportInfo = new ReportPaginationInfo(report, m_serverRPLVersion, pageHeight);
			m_reportInfo.ReadRegionPageTotalInfo(m_pageContext.PageTotalInfo);
			InitializeForInteractiveRenderer(report, createAndRegisterStream, registerEvents, ref renderProperties);
			m_pageContext.CanTracePagination = true;
		}

		public SPBProcessing(Microsoft.ReportingServices.OnDemandReportRendering.Report report, CreateAndRegisterStream createAndRegisterStream, bool registerEvents, ref Hashtable renderProperties)
		{
			double pageHeight = report.ReportSections[0].Page.InteractiveHeight.ToMillimeters();
			m_pageContext = new PageContext(report.Instance.InitialPageName, pageHeight, registerEvents, report.ConsumeContainerWhitespace, createAndRegisterStream);
			m_report = new Report(report, m_pageContext, null, defaultVersion: false);
			m_reportInfo = new ReportPaginationInfo(report, m_serverRPLVersion, pageHeight);
			m_reportInfo.ReadRegionPageTotalInfo(m_pageContext.PageTotalInfo);
			InitializeForInteractiveRenderer(report, createAndRegisterStream, registerEvents, ref renderProperties);
			m_pageContext.CanTracePagination = true;
		}

		internal SPBProcessing(Microsoft.ReportingServices.OnDemandReportRendering.Report report, int totalPages, bool needTotalPages)
		{
			double pageHeight = report.ReportSections[0].Page.InteractiveHeight.ToMillimeters();
			m_pageContext = new PageContext(report.Instance.InitialPageName, pageHeight, registerEvents: true, report.ConsumeContainerWhitespace, null);
			m_report = new Report(report, m_pageContext, null, defaultVersion: false);
			m_reportInfo = new ReportPaginationInfo(report, m_serverRPLVersion, pageHeight);
			m_reportInfo.ReadRegionPageTotalInfo(m_pageContext.PageTotalInfo);
			m_totalPages = totalPages;
			if (report.SnapshotPageSizeInfo == Microsoft.ReportingServices.OnDemandReportRendering.Report.SnapshotPageSize.Large)
			{
				m_rplState = RPLState.RPLStream;
			}
			else if (report.SnapshotPageSizeInfo == Microsoft.ReportingServices.OnDemandReportRendering.Report.SnapshotPageSize.Small)
			{
				m_rplState = RPLState.RPLObjectModel;
			}
			if (m_reportInfo.IsDone)
			{
				m_totalPages = m_reportInfo.PaginatedPages;
			}
			else if (needTotalPages && ((report.NeedsOverallTotalPages && m_totalPages <= 0) || report.NeedsPageBreakTotalPages))
			{
				SetContext(new SPBContext());
			}
		}

		public static int TotalNrOfPages(Microsoft.ReportingServices.OnDemandReportRendering.Report report)
		{
			int num = 0;
			using (SPBProcessing sPBProcessing = new SPBProcessing(report, 0, needTotalPages: false))
			{
				if (sPBProcessing.m_totalPages <= 0)
				{
					sPBProcessing.m_pageContext.CanTracePagination = true;
					sPBProcessing.SetContext(new SPBContext());
				}
				return sPBProcessing.m_totalPages;
			}
		}

		public static bool RenderSecondaryStream(Microsoft.ReportingServices.OnDemandReportRendering.Report report, CreateAndRegisterStream createAndRegisterStream, string streamName)
		{
			if (string.IsNullOrEmpty(streamName))
			{
				return false;
			}
			char[] separator = new char[1]
			{
				'_'
			};
			string[] array = streamName.Split(separator);
			if (array.Length < 3)
			{
				return false;
			}
			if (array[0].Equals("C"))
			{
				int pageNumber = ParseInt(array[2], 0);
				FindChart(report, array[1], pageNumber, streamName, createAndRegisterStream);
				return true;
			}
			if (array[0].Equals("G"))
			{
				int pageNumber2 = ParseInt(array[2], 0);
				FindGaugePanel(report, array[1], pageNumber2, streamName, createAndRegisterStream);
				return true;
			}
			if (array[0].Equals("M"))
			{
				int pageNumber3 = ParseInt(array[2], 0);
				FindMap(report, array[1], pageNumber3, streamName, createAndRegisterStream);
				return true;
			}
			if (array[0].Equals("I"))
			{
				int pageNumber4 = ParseInt(array[2], 0);
				FindImage(report, array[1], pageNumber4, streamName, createAndRegisterStream);
				return true;
			}
			string sTREAMPREFIX = ImageConsolidation.STREAMPREFIX;
			if (streamName.StartsWith(sTREAMPREFIX, StringComparison.OrdinalIgnoreCase))
			{
				string[] array2 = streamName.Substring(sTREAMPREFIX.Length).Split(separator);
				if (array2.Length == 2)
				{
					int num = ParseInt(array2[0], 0);
					int num2 = ParseInt(array2[1], 0);
					if (num > -1 && num2 > -1)
					{
						FindImageConsolidation(report, num, num2, streamName, createAndRegisterStream);
						return true;
					}
				}
			}
			return false;
		}

		public static void GetDocumentMap(Microsoft.ReportingServices.OnDemandReportRendering.Report report)
		{
			if (report.HasDocumentMap)
			{
				using (SPBProcessing sPBProcessing = new SPBProcessing(report, 0, needTotalPages: false))
				{
					sPBProcessing.m_pageContext.CanTracePagination = true;
					sPBProcessing.SetContext(new SPBContext(0, 0, addToggledItems: true));
					sPBProcessing.GetDocumentMap();
				}
			}
		}

		internal static int ParseInt(string intValue, int defaultValue)
		{
			if (int.TryParse(intValue, out int result))
			{
				return result;
			}
			return defaultValue;
		}

		internal static int CompareWithOrdinalComparison(string x, string y, bool ignoreCase)
		{
			if (ignoreCase)
			{
				return string.Compare(x, y, StringComparison.OrdinalIgnoreCase);
			}
			return string.Compare(x, y, StringComparison.Ordinal);
		}

		private static void FindChart(Microsoft.ReportingServices.OnDemandReportRendering.Report report, string uniqueName, int pageNumber, string streamName, CreateAndRegisterStream createAndRegisterStream)
		{
			if (uniqueName != null)
			{
				using (SPBProcessing sPBProcessing = new SPBProcessing(report, 0, needTotalPages: false))
				{
					sPBProcessing.SetContext(new SPBContext(0, 0, addToggledItems: false));
					sPBProcessing.FindChart(uniqueName, pageNumber, streamName, createAndRegisterStream);
				}
			}
		}

		private static void FindGaugePanel(Microsoft.ReportingServices.OnDemandReportRendering.Report report, string uniqueName, int pageNumber, string streamName, CreateAndRegisterStream createAndRegisterStream)
		{
			if (uniqueName != null)
			{
				using (SPBProcessing sPBProcessing = new SPBProcessing(report, 0, needTotalPages: false))
				{
					sPBProcessing.SetContext(new SPBContext(0, 0, addToggledItems: false));
					sPBProcessing.FindGaugePanel(uniqueName, pageNumber, streamName, createAndRegisterStream);
				}
			}
		}

		private static void FindMap(Microsoft.ReportingServices.OnDemandReportRendering.Report report, string uniqueName, int pageNumber, string streamName, CreateAndRegisterStream createAndRegisterStream)
		{
			if (uniqueName != null)
			{
				using (SPBProcessing sPBProcessing = new SPBProcessing(report, 0, needTotalPages: false))
				{
					sPBProcessing.SetContext(new SPBContext(0, 0, addToggledItems: false));
					sPBProcessing.FindMap(uniqueName, pageNumber, streamName, createAndRegisterStream);
				}
			}
		}

		private static void FindImage(Microsoft.ReportingServices.OnDemandReportRendering.Report report, string uniqueName, int pageNumber, string streamName, CreateAndRegisterStream createAndRegisterStream)
		{
			if (uniqueName != null)
			{
				using (SPBProcessing sPBProcessing = new SPBProcessing(report, 0, needTotalPages: false))
				{
					sPBProcessing.SetContext(new SPBContext(0, 0, addToggledItems: false));
					sPBProcessing.FindImage(uniqueName, pageNumber, streamName, createAndRegisterStream);
				}
			}
		}

		private static void FindImageConsolidation(Microsoft.ReportingServices.OnDemandReportRendering.Report report, int pageNumber, int offset, string streamName, CreateAndRegisterStream createAndRegisterStream)
		{
			using (SPBProcessing sPBProcessing = new SPBProcessing(report, 0, needTotalPages: false))
			{
				sPBProcessing.SetContext(new SPBContext(0, 0, addToggledItems: false));
				sPBProcessing.FindImageConsolidation(report.Name, pageNumber, offset, streamName, createAndRegisterStream);
			}
		}

		public static Dictionary<string, string> CollectBookmarks(Microsoft.ReportingServices.OnDemandReportRendering.Report report, int totalPages)
		{
			if (!report.HasBookmarks)
			{
				return null;
			}
			Dictionary<string, string> dictionary = null;
			using (SPBProcessing sPBProcessing = new SPBProcessing(report, totalPages, needTotalPages: true))
			{
				sPBProcessing.m_pageContext.CanTracePagination = true;
				sPBProcessing.SetContext(new SPBContext(0, 0, addToggledItems: false));
				return sPBProcessing.CollectBookmarks();
			}
		}

		private void Dispose(bool disposing)
		{
			if (!disposing)
			{
				return;
			}
			m_pageContext.DisposeResources();
			if (m_report.JobContext == null)
			{
				return;
			}
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

		public void Dispose()
		{
			Dispose(disposing: true);
			GC.SuppressFinalize(this);
		}

		public void SetContext(SPBContext context)
		{
			int startPage = context.StartPage;
			int endPage = context.EndPage;
			bool measureItems = context.MeasureItems;
			bool emfDynamicImage = context.EmfDynamicImage;
			SecondaryStreams secondaryStreams = context.SecondaryStreams;
			bool addSecondaryStreamNames = context.AddSecondaryStreamNames;
			bool addToggledItems = context.AddToggledItems;
			bool addOriginalValue = context.AddOriginalValue;
			bool addFirstPageHeaderFooter = context.AddFirstPageHeaderFooter;
			bool flag = context.UseImageConsolidation;
			if (flag && (m_pageContext.VersionPicker == RPLVersionEnum.RPL2008 || m_pageContext.VersionPicker == RPLVersionEnum.RPLAccess))
			{
				flag = false;
			}
			if (startPage > endPage)
			{
				throw new ArgumentException(SPBRes.InvalidStartPageNumber);
			}
			if (endPage < -1)
			{
				throw new ArgumentException(SPBRes.InvalidEndPageNumber);
			}
			m_startPage = startPage;
			m_endPage = endPage;
			if (startPage == 0 || startPage == -1)
			{
				m_endPage = m_startPage;
			}
			if (m_rplState == RPLState.RPLStream)
			{
				if (RSTrace.RenderingTracer.TraceVerbose)
				{
					RSTrace.RenderingTracer.Trace(TraceLevel.Verbose, "RPL stream in use");
				}
			}
			else if (RSTrace.RenderingTracer.TraceVerbose)
			{
				RSTrace.RenderingTracer.Trace(TraceLevel.Verbose, "RPL object model in use");
			}
			if (secondaryStreams != 0)
			{
				addSecondaryStreamNames = true;
			}
			if (flag)
			{
				m_pageContext.ImageConsolidation = new ImageConsolidation(m_createAndRegisterStream);
			}
			m_pageContext.SetContext(measureItems, emfDynamicImage, secondaryStreams, addSecondaryStreamNames, addToggledItems, addOriginalValue, addFirstPageHeaderFooter, context.ConvertImages);
			m_report.SetContext(m_reportInfo);
			m_currentPage = 0;
			PaginateReport(startPage, endPage);
		}

		internal void SetContext(SPBContext context, bool createStream)
		{
			if (createStream)
			{
				m_rplState = RPLState.RPLStream;
			}
			SetContext(context);
		}

		public void UpdateRenderProperties(ref Hashtable renderProperties)
		{
			if (!m_useInteractiveHeight)
			{
				return;
			}
			if (renderProperties == null)
			{
				renderProperties = new Hashtable();
			}
			if (m_report.Done)
			{
				renderProperties["UpdatedPaginationMode"] = PaginationMode.TotalPages;
				renderProperties["TotalPages"] = m_currentPage;
				return;
			}
			if (m_totalPages > 0)
			{
				renderProperties["UpdatedPaginationMode"] = PaginationMode.TotalPages;
				renderProperties["TotalPages"] = m_totalPages;
				return;
			}
			PaginationMode paginationMode = PaginationMode.Estimate;
			object obj = renderProperties["ClientPaginationMode"];
			if (obj != null)
			{
				paginationMode = (PaginationMode)obj;
			}
			object obj2 = renderProperties["PreviousTotalPages"];
			if (obj2 != null && obj2 is int)
			{
				m_totalPages = (int)obj2;
			}
			if (paginationMode == PaginationMode.TotalPages)
			{
				if (m_reportInfo.IsDone)
				{
					m_totalPages = m_reportInfo.PaginatedPages;
					renderProperties["UpdatedPaginationMode"] = PaginationMode.TotalPages;
					renderProperties["TotalPages"] = m_totalPages;
				}
				else if (m_totalPages <= 0)
				{
					PaginateReport(-1, -1);
					renderProperties["UpdatedPaginationMode"] = PaginationMode.TotalPages;
					renderProperties["TotalPages"] = m_totalPages;
				}
			}
			else if (m_totalPages <= 0 && m_currentPage + 1 + m_totalPages > 0)
			{
				renderProperties["UpdatedPaginationMode"] = PaginationMode.Estimate;
				renderProperties["TotalPages"] = m_currentPage + 1;
			}
		}

		public Stream GetNextPage()
		{
			return GetNextPage(collectPageBookmarks: false);
		}

		public Stream GetNextPage(bool collectPageBookmarks)
		{
			RPLReport rplReport = null;
			return GetNextPage(out rplReport, collectPageBookmarks);
		}

		public void GetNextPage(Stream outputStream)
		{
			GetNextPage(outputStream, collectPageBookmarks: false);
		}

		public void GetNextPage(Stream outputStream, bool collectPageBookmarks)
		{
			if (!m_report.Done)
			{
				RPLWriter rplWriter = GetReportNextPage(ref outputStream, collectPageBookmarks);
				FlushRPLWriter(ref rplWriter);
			}
		}

		public Stream GetNextPage(out RPLReport rplReport)
		{
			return GetNextPage(out rplReport, collectPageBookmarks: false);
		}

		public Stream GetNextPage(out RPLReport rplReport, bool collectPageBookmarks)
		{
			rplReport = null;
			if (m_report.Done)
			{
				return null;
			}
			Stream stream = null;
			RPLWriter rplWriter = GetReportNextPage(ref stream, collectPageBookmarks);
			if (rplWriter != null)
			{
				if (rplWriter.BinaryWriter != null)
				{
					FlushRPLWriter(ref rplWriter);
					BinaryReader reader = new BinaryReader(new BufferedStream(stream), Encoding.Unicode);
					rplReport = new RPLReport(reader);
				}
				else
				{
					rplReport = rplWriter.Report;
				}
			}
			return stream;
		}

		private RPLWriter GetReportNextPage(ref Stream stream, bool collectPageBookmarks)
		{
			RPLWriter rPLWriter = null;
			Interactivity interactivity = null;
			bool flag = false;
			m_currentPage++;
			m_pageContext.PageBookmarks = null;
			if (m_reportInfo.IsDone && m_currentPage > m_reportInfo.PaginatedPages)
			{
				return null;
			}
			if (m_startPage >= 0 && m_endPage >= 0 && (m_endPage == 0 || (m_currentPage >= m_startPage && m_currentPage <= m_endPage)))
			{
				rPLWriter = new RPLWriter();
				if (m_rplState == RPLState.RPLStream && stream == null)
				{
					string name = m_currentPage.ToString(CultureInfo.InvariantCulture);
					stream = m_createAndRegisterStream(name, "spbif", null, null, willSeek: true, StreamOper.CreateOnly);
				}
				if (stream != null)
				{
					CreateWriter(rPLWriter, stream);
				}
				else if (m_rplState == RPLState.Unknown)
				{
					if (m_lastPageInfoForCancel == null)
					{
						m_lastPageInfoForCancel = m_lastPageInfo;
					}
					m_pageContext.InitCancelPage(m_report.InteractiveHeight);
				}
				if (m_useInteractiveHeight)
				{
					m_report.LoadInteractiveChunks(m_currentPage);
					if (m_report.RegisterPageForCollect(m_currentPage, collectPageBookmarks))
					{
						interactivity = new Interactivity();
					}
					if (m_endPage > 0 && m_currentPage == m_endPage)
					{
						flag = true;
					}
				}
			}
			bool flag2 = m_useInteractiveHeight && m_reportInfo.HasPaginationInfoStream();
			m_report.NextPage(rPLWriter, ref m_lastPageInfo, m_currentPage, m_totalPages, interactivity, hasPaginationChunk: true);
			if (m_rplState == RPLState.Unknown)
			{
				if (stream == null)
				{
					bool cancelPage = m_pageContext.CancelPage;
					m_pageContext.ResetCancelPage();
					if (cancelPage)
					{
						if (RSTrace.RenderingTracer.TraceVerbose)
						{
							RSTrace.RenderingTracer.Trace(TraceLevel.Verbose, "Switch to RPL stream ");
						}
						rPLWriter = new RPLWriter();
						string name2 = m_currentPage.ToString(CultureInfo.InvariantCulture);
						stream = m_createAndRegisterStream(name2, "spbif", null, null, willSeek: true, StreamOper.CreateOnly);
						CreateWriter(rPLWriter, stream);
						m_report.SetContext(m_reportInfo);
						m_report.ResetSectionsOnPage();
						m_report.NextPage(rPLWriter, ref m_lastPageInfoForCancel, m_currentPage, m_totalPages, interactivity, hasPaginationChunk: true);
					}
				}
				m_lastPageInfoForCancel = m_report.GetPaginationInfo();
			}
			if (m_currentPage == m_reportInfo.PaginatedPages + 1)
			{
				m_report.UpdatePagination();
			}
			else if (!flag2)
			{
				m_report.ResetLastSection();
			}
			if (interactivity != null)
			{
				m_report.UnregisterPageForCollect();
			}
			if (m_report.Done || flag)
			{
				if (m_report.Done)
				{
					m_pageContext.PageTotalInfo.CalculationDone = true;
				}
				m_report.UnloadInteractiveChunks();
				m_reportInfo.SavePaginationMetadata(m_report.Done, m_pageContext.PageTotalInfo);
			}
			m_report.DisposeDelayTextBox();
			return rPLWriter;
		}

		private void CreateWriter(RPLWriter rplWriter, Stream stream)
		{
			RSTrace.RenderingTracer.Assert(rplWriter != null, "Writer is null");
			RSTrace.RenderingTracer.Assert(stream != null, "Stream is null");
			BinaryWriter binaryWriter2 = rplWriter.BinaryWriter = new BinaryWriter(new BufferedStream(stream), Encoding.Unicode);
			binaryWriter2.Write("RPLIF");
			WriteVersionStamp(binaryWriter2, m_report.RPLVersion);
		}

		private RPLWriter FlushRPLWriter(ref RPLWriter rplWriter)
		{
			if (rplWriter == null)
			{
				return null;
			}
			if (rplWriter.BinaryWriter != null)
			{
				WriteVersionStamp(rplWriter.BinaryWriter, m_report.RPLVersion);
				rplWriter.BinaryWriter.Flush();
			}
			rplWriter = null;
			return rplWriter;
		}

		private void WriteVersionStamp(BinaryWriter sbpWriter, Version rplVersion)
		{
			sbpWriter.Write((byte)rplVersion.Major);
			sbpWriter.Write((byte)rplVersion.Minor);
			sbpWriter.Write(rplVersion.Build);
		}

		private void PaginateReport(int startPage, int endPage)
		{
			if (startPage > endPage || endPage < -1)
			{
				m_lastPageInfo = null;
				return;
			}
			m_startPage = startPage;
			m_endPage = endPage;
			bool hasPaginationChunk = m_useInteractiveHeight && m_reportInfo.HasPaginationInfoStream();
			if (startPage == 0 || startPage == -1)
			{
				m_endPage = m_startPage;
			}
			if (m_startPage > m_reportInfo.PaginatedPages)
			{
				if (m_reportInfo.PaginatedPages >= 0)
				{
					m_currentPage = m_reportInfo.PaginatedPages;
					if (m_reportInfo.IsDone)
					{
						return;
					}
					m_reportInfo.ReadPageInfo(m_currentPage, out m_lastPageInfo);
					while (!m_report.Done && m_currentPage < m_startPage - 1)
					{
						m_currentPage++;
						if (RSTrace.RenderingTracer.TraceVerbose)
						{
							RSTrace.RenderingTracer.Trace(TraceLevel.Verbose, "We paginate the page: " + m_currentPage.ToString(CultureInfo.InvariantCulture));
						}
						m_report.NextPage(null, ref m_lastPageInfo, m_currentPage, m_totalPages, null, hasPaginationChunk);
						if (m_currentPage == m_reportInfo.PaginatedPages + 1)
						{
							m_report.UpdatePagination();
						}
					}
					if (m_report.Done)
					{
						m_totalPages = m_currentPage;
						m_pageContext.PageTotalInfo.CalculationDone = true;
						m_reportInfo.SavePaginationMetadata(m_report.Done, m_pageContext.PageTotalInfo);
					}
					else
					{
						m_reportInfo.ReadPageInfo(m_currentPage, out m_lastPageInfo);
					}
				}
				else
				{
					if (-1 != m_reportInfo.PaginatedPages || m_startPage <= 0)
					{
						return;
					}
					m_currentPage = 0;
					while (!m_report.Done && m_currentPage < m_startPage - 1)
					{
						m_currentPage++;
						if (RSTrace.RenderingTracer.TraceVerbose)
						{
							RSTrace.RenderingTracer.Trace(TraceLevel.Verbose, "We paginate the page: " + m_currentPage.ToString(CultureInfo.InvariantCulture));
						}
						m_report.NextPage(null, ref m_lastPageInfo, m_currentPage, m_totalPages, null, hasPaginationChunk);
					}
					if (m_report.Done)
					{
						m_pageContext.PageTotalInfo.CalculationDone = true;
						m_totalPages = m_currentPage;
					}
				}
			}
			else if (m_endPage > m_reportInfo.PaginatedPages)
			{
				m_currentPage = m_startPage - 1;
				m_reportInfo.ReadPageInfo(m_currentPage, out m_lastPageInfo);
			}
			else if (m_startPage == -1)
			{
				if (m_reportInfo.PaginatedPages >= 0)
				{
					if (m_reportInfo.IsDone)
					{
						return;
					}
					m_currentPage = m_reportInfo.PaginatedPages;
					m_reportInfo.ReadPageInfo(m_currentPage, out m_lastPageInfo);
					while (!m_report.Done)
					{
						m_currentPage++;
						if (RSTrace.RenderingTracer.TraceVerbose)
						{
							RSTrace.RenderingTracer.Trace(TraceLevel.Verbose, "We paginate the page: " + m_currentPage.ToString(CultureInfo.InvariantCulture));
						}
						m_report.NextPage(null, ref m_lastPageInfo, m_currentPage, m_totalPages, null, hasPaginationChunk);
						if (m_currentPage == m_reportInfo.PaginatedPages + 1)
						{
							m_report.UpdatePagination();
						}
					}
					m_pageContext.PageTotalInfo.CalculationDone = true;
					m_reportInfo.SavePaginationMetadata(m_report.Done, m_pageContext.PageTotalInfo);
					m_totalPages = m_currentPage;
				}
				else
				{
					if (m_reportInfo.PaginatedPages != -1)
					{
						return;
					}
					m_currentPage = 0;
					while (!m_report.Done)
					{
						m_currentPage++;
						if (RSTrace.RenderingTracer.TraceVerbose)
						{
							RSTrace.RenderingTracer.Trace(TraceLevel.Verbose, "We paginate the page: " + m_currentPage.ToString(CultureInfo.InvariantCulture));
						}
						m_report.NextPage(null, ref m_lastPageInfo, m_currentPage, m_totalPages, null, hasPaginationChunk);
					}
					m_pageContext.PageTotalInfo.CalculationDone = true;
					m_totalPages = m_currentPage;
				}
			}
			else if (m_startPage > 0)
			{
				m_currentPage = m_startPage - 1;
				m_reportInfo.ReadPageInfo(m_currentPage, out m_lastPageInfo);
			}
		}

		private void InitializeForInteractiveRenderer(Microsoft.ReportingServices.OnDemandReportRendering.Report report, CreateAndRegisterStream createAndRegisterStream, bool registerEvents, ref Hashtable renderProperties)
		{
			m_createAndRegisterStream = createAndRegisterStream;
			m_useInteractiveHeight = true;
			if (report.SnapshotPageSizeInfo == Microsoft.ReportingServices.OnDemandReportRendering.Report.SnapshotPageSize.Large)
			{
				m_rplState = RPLState.RPLStream;
			}
			else if (report.SnapshotPageSizeInfo == Microsoft.ReportingServices.OnDemandReportRendering.Report.SnapshotPageSize.Small)
			{
				m_rplState = RPLState.RPLObjectModel;
			}
			if (renderProperties != null)
			{
				object obj = renderProperties["ClientPaginationMode"];
				if (obj != null && (PaginationMode)obj == PaginationMode.TotalPages)
				{
					object obj2 = renderProperties["PreviousTotalPages"];
					if (obj2 != null && obj2 is int)
					{
						m_totalPages = (int)obj2;
					}
				}
			}
			if (m_totalPages > 0)
			{
				return;
			}
			if (m_reportInfo.IsDone)
			{
				m_totalPages = m_reportInfo.PaginatedPages;
				if (renderProperties == null)
				{
					renderProperties = new Hashtable();
				}
				renderProperties["UpdatedPaginationMode"] = PaginationMode.TotalPages;
				renderProperties["TotalPages"] = m_totalPages;
				return;
			}
			m_totalPages = 0;
			if (report.NeedsTotalPages)
			{
				SetContext(new SPBContext());
				if (renderProperties == null)
				{
					renderProperties = new Hashtable();
				}
				renderProperties["UpdatedPaginationMode"] = PaginationMode.TotalPages;
				renderProperties["TotalPages"] = m_totalPages;
			}
		}

		internal int FindString(int startPage, int endPage, string findValue)
		{
			int result = 0;
			Interactivity interactivity = null;
			bool hasPaginationChunk = m_useInteractiveHeight && m_reportInfo.HasPaginationInfoStream();
			if (startPage <= endPage)
			{
				while (!m_report.Done)
				{
					m_currentPage++;
					interactivity = null;
					if (m_currentPage >= startPage && m_currentPage <= endPage)
					{
						interactivity = new Interactivity(findValue, Interactivity.EventType.FindStringEvent);
					}
					m_report.NextPage(null, ref m_lastPageInfo, m_currentPage, m_totalPages, interactivity, hasPaginationChunk);
					if (m_currentPage == m_reportInfo.PaginatedPages + 1)
					{
						m_report.UpdatePagination();
					}
					if (interactivity != null && interactivity.Done)
					{
						if (m_report.Done)
						{
							m_pageContext.PageTotalInfo.CalculationDone = true;
						}
						m_reportInfo.SavePaginationMetadata(m_report.Done, m_pageContext.PageTotalInfo);
						return m_currentPage;
					}
				}
			}
			else
			{
				int num = 0;
				bool flag = false;
				while (!m_report.Done)
				{
					m_currentPage++;
					interactivity = null;
					if (m_currentPage <= endPage || m_currentPage >= startPage)
					{
						interactivity = new Interactivity(findValue, Interactivity.EventType.FindStringEvent);
					}
					m_report.NextPage(null, ref m_lastPageInfo, m_currentPage, m_totalPages, interactivity, hasPaginationChunk);
					if (m_currentPage == m_reportInfo.PaginatedPages + 1)
					{
						m_report.UpdatePagination();
					}
					if (interactivity == null || !interactivity.Done)
					{
						continue;
					}
					if (m_currentPage <= endPage)
					{
						if (!flag)
						{
							num = m_currentPage;
							flag = true;
						}
						continue;
					}
					if (m_report.Done)
					{
						m_pageContext.PageTotalInfo.CalculationDone = true;
					}
					m_reportInfo.SavePaginationMetadata(m_report.Done, m_pageContext.PageTotalInfo);
					return m_currentPage;
				}
				result = num;
			}
			m_pageContext.PageTotalInfo.CalculationDone = true;
			m_reportInfo.SavePaginationMetadata(m_report.Done, m_pageContext.PageTotalInfo);
			return result;
		}

		internal int FindUserSort(string textbox, ref int numberOfPages, ref PaginationMode paginationMode)
		{
			int result = 0;
			Interactivity interactivity = new Interactivity(textbox, Interactivity.EventType.UserSortEvent);
			bool hasPaginationChunk = m_useInteractiveHeight && m_reportInfo.HasPaginationInfoStream();
			while (!m_report.Done)
			{
				m_currentPage++;
				m_report.NextPage(null, ref m_lastPageInfo, m_currentPage, m_totalPages, interactivity, hasPaginationChunk);
				if (m_currentPage == m_reportInfo.PaginatedPages + 1)
				{
					m_report.UpdatePagination();
				}
				if (interactivity.Done)
				{
					result = m_currentPage;
					break;
				}
			}
			if (paginationMode == PaginationMode.TotalPages)
			{
				while (!m_report.Done)
				{
					m_currentPage++;
					m_report.NextPage(null, ref m_lastPageInfo, m_currentPage, m_totalPages, null, hasPaginationChunk);
					if (m_currentPage == m_reportInfo.PaginatedPages + 1)
					{
						m_report.UpdatePagination();
					}
				}
			}
			if (m_report.Done)
			{
				m_pageContext.PageTotalInfo.CalculationDone = true;
				paginationMode = PaginationMode.TotalPages;
				numberOfPages = m_reportInfo.PaginatedPages;
			}
			else
			{
				numberOfPages = m_currentPage;
			}
			m_reportInfo.SavePaginationMetadata(m_report.Done, m_pageContext.PageTotalInfo);
			return result;
		}

		private void FindItem(Interactivity interactivityContext, int pageNumber)
		{
			RSTrace.RenderingTracer.Assert(interactivityContext != null, "The interactivity context is null.");
			bool hasPaginationChunk = m_useInteractiveHeight && m_reportInfo.HasPaginationInfoStream();
			while (!m_report.Done)
			{
				m_currentPage++;
				if (m_currentPage < pageNumber)
				{
					m_report.NextPage(null, ref m_lastPageInfo, m_currentPage, m_totalPages, null, hasPaginationChunk);
				}
				else
				{
					m_report.NextPage(null, ref m_lastPageInfo, m_currentPage, m_totalPages, interactivityContext, hasPaginationChunk);
				}
				if (m_currentPage == m_reportInfo.PaginatedPages + 1)
				{
					m_report.UpdatePagination();
				}
				if (interactivityContext.Done || (m_currentPage == pageNumber && interactivityContext.InteractivityEventType == Interactivity.EventType.ImageConsolidation))
				{
					if (m_report.Done)
					{
						m_pageContext.PageTotalInfo.CalculationDone = true;
					}
					m_reportInfo.SavePaginationMetadata(m_report.Done, m_pageContext.PageTotalInfo);
					return;
				}
			}
			m_pageContext.PageTotalInfo.CalculationDone = true;
			m_reportInfo.SavePaginationMetadata(m_report.Done, m_pageContext.PageTotalInfo);
		}

		internal void FindImageConsolidation(string reportName, int pageNumber, int offset, string streamName, CreateAndRegisterStream createAndRegisterStream)
		{
			m_pageContext.ImageConsolidation = new ImageConsolidation(createAndRegisterStream, offset);
			m_pageContext.ImageConsolidation.SetName(reportName, pageNumber);
			Interactivity interactivityContext = new Interactivity(null, Interactivity.EventType.ImageConsolidation, streamName, createAndRegisterStream);
			FindItem(interactivityContext, pageNumber);
			m_pageContext.ImageConsolidation.RenderToStream();
		}

		internal void FindChart(string uniqueName, int pageNumber, string streamName, CreateAndRegisterStream createAndRegisterStream)
		{
			Interactivity interactivityContext = new Interactivity(uniqueName, Interactivity.EventType.FindChart, streamName, createAndRegisterStream);
			FindItem(interactivityContext, pageNumber);
		}

		internal void FindGaugePanel(string uniqueName, int pageNumber, string streamName, CreateAndRegisterStream createAndRegisterStream)
		{
			Interactivity interactivityContext = new Interactivity(uniqueName, Interactivity.EventType.FindGaugePanel, streamName, createAndRegisterStream);
			FindItem(interactivityContext, pageNumber);
		}

		internal void FindMap(string uniqueName, int pageNumber, string streamName, CreateAndRegisterStream createAndRegisterStream)
		{
			Interactivity interactivityContext = new Interactivity(uniqueName, Interactivity.EventType.FindMap, streamName, createAndRegisterStream);
			FindItem(interactivityContext, pageNumber);
		}

		internal void FindImage(string uniqueName, int pageNumber, string streamName, CreateAndRegisterStream createAndRegisterStream)
		{
			Interactivity interactivityContext = new Interactivity(uniqueName, Interactivity.EventType.FindImage, streamName, createAndRegisterStream);
			FindItem(interactivityContext, pageNumber);
		}

		internal string FindDrillthrough(string drillthroughId, int lastPageCollected, out NameValueCollection parameters)
		{
			string result = null;
			parameters = null;
			Interactivity interactivity = new Interactivity(drillthroughId, Interactivity.EventType.DrillthroughEvent);
			bool hasPaginationChunk = m_useInteractiveHeight && m_reportInfo.HasPaginationInfoStream();
			while (!m_report.Done)
			{
				m_currentPage++;
				if (m_currentPage <= lastPageCollected)
				{
					m_report.NextPage(null, ref m_lastPageInfo, m_currentPage, m_totalPages, null, hasPaginationChunk);
					if (m_currentPage == m_reportInfo.PaginatedPages + 1)
					{
						m_report.UpdatePagination();
					}
					continue;
				}
				m_report.NextPage(null, ref m_lastPageInfo, m_currentPage, m_totalPages, interactivity, hasPaginationChunk);
				if (m_currentPage == m_reportInfo.PaginatedPages + 1)
				{
					m_report.UpdatePagination();
				}
				if (!interactivity.Done)
				{
					continue;
				}
				if (interactivity.DrillthroughResult != null)
				{
					result = interactivity.DrillthroughResult.ReportName;
					parameters = interactivity.DrillthroughResult.Parameters;
				}
				if (m_report.Done)
				{
					m_pageContext.PageTotalInfo.CalculationDone = true;
				}
				m_reportInfo.SavePaginationMetadata(m_report.Done, m_pageContext.PageTotalInfo);
				return result;
			}
			m_pageContext.PageTotalInfo.CalculationDone = true;
			m_reportInfo.SavePaginationMetadata(m_report.Done, m_pageContext.PageTotalInfo);
			return result;
		}

		internal int FindBookmark(string bookmarkId, int lastPageCollected, ref string uniqueName)
		{
			int result = 0;
			Interactivity interactivity = new Interactivity(bookmarkId);
			bool hasPaginationChunk = m_useInteractiveHeight && m_reportInfo.HasPaginationInfoStream();
			while (!m_report.Done)
			{
				m_currentPage++;
				if (m_currentPage <= lastPageCollected)
				{
					m_report.NextPage(null, ref m_lastPageInfo, m_currentPage, m_totalPages, null, hasPaginationChunk);
					if (m_currentPage == m_reportInfo.PaginatedPages + 1)
					{
						m_report.UpdatePagination();
					}
					continue;
				}
				m_report.NextPage(null, ref m_lastPageInfo, m_currentPage, m_totalPages, interactivity, hasPaginationChunk);
				if (m_currentPage == m_reportInfo.PaginatedPages + 1)
				{
					m_report.UpdatePagination();
				}
				if (!interactivity.Done)
				{
					continue;
				}
				uniqueName = interactivity.ItemInfo;
				if (m_report.Done)
				{
					m_pageContext.PageTotalInfo.CalculationDone = true;
				}
				m_reportInfo.SavePaginationMetadata(m_report.Done, m_pageContext.PageTotalInfo);
				return m_currentPage;
			}
			m_pageContext.PageTotalInfo.CalculationDone = true;
			m_reportInfo.SavePaginationMetadata(m_report.Done, m_pageContext.PageTotalInfo);
			return result;
		}

		internal Dictionary<string, string> CollectBookmarks()
		{
			Interactivity interactivity = new Interactivity();
			m_pageContext.PageBookmarks = new Dictionary<string, string>();
			bool hasPaginationChunk = m_useInteractiveHeight && m_reportInfo.HasPaginationInfoStream();
			while (!m_report.Done)
			{
				m_currentPage++;
				m_report.NextPage(null, ref m_lastPageInfo, m_currentPage, m_totalPages, interactivity, hasPaginationChunk);
				if (m_currentPage == m_reportInfo.PaginatedPages + 1)
				{
					m_report.UpdatePagination();
				}
			}
			m_pageContext.PageTotalInfo.CalculationDone = true;
			m_reportInfo.SavePaginationMetadata(m_report.Done, m_pageContext.PageTotalInfo);
			return m_pageContext.PageBookmarks;
		}

		internal int FindDocumentMap(string documentMapId, int lastPageCollected)
		{
			int result = 0;
			Interactivity interactivity = new Interactivity(documentMapId, Interactivity.EventType.DocumentMapNavigationEvent);
			bool hasPaginationChunk = m_useInteractiveHeight && m_reportInfo.HasPaginationInfoStream();
			while (!m_report.Done)
			{
				m_currentPage++;
				if (m_currentPage <= lastPageCollected)
				{
					m_report.NextPage(null, ref m_lastPageInfo, m_currentPage, m_totalPages, null, hasPaginationChunk);
					if (m_currentPage == m_reportInfo.PaginatedPages + 1)
					{
						m_report.UpdatePagination();
					}
					continue;
				}
				m_report.NextPage(null, ref m_lastPageInfo, m_currentPage, m_totalPages, interactivity, hasPaginationChunk);
				if (m_currentPage == m_reportInfo.PaginatedPages + 1)
				{
					m_report.UpdatePagination();
				}
				if (!interactivity.Done)
				{
					continue;
				}
				if (m_report.Done)
				{
					m_pageContext.PageTotalInfo.CalculationDone = true;
				}
				m_reportInfo.SavePaginationMetadata(m_report.Done, m_pageContext.PageTotalInfo);
				return m_currentPage;
			}
			m_pageContext.PageTotalInfo.CalculationDone = true;
			m_reportInfo.SavePaginationMetadata(m_report.Done, m_pageContext.PageTotalInfo);
			return result;
		}

		internal void GetDocumentMap()
		{
			bool hasPaginationChunk = m_useInteractiveHeight && m_reportInfo.HasPaginationInfoStream();
			Interactivity interactivity = new Interactivity(Interactivity.EventType.GetDocumentMap);
			m_report.LoadLabelsChunk();
			while (!m_report.Done)
			{
				m_currentPage++;
				m_report.RegisterPageLabelsForCollect(m_currentPage);
				m_report.NextPage(null, ref m_lastPageInfo, m_currentPage, m_totalPages, interactivity, hasPaginationChunk);
				if (m_currentPage == m_reportInfo.PaginatedPages + 1)
				{
					m_report.UpdatePagination();
				}
				m_report.UnregisterPageForCollect();
			}
			m_report.UnloadInteractiveChunks();
			m_pageContext.PageTotalInfo.CalculationDone = true;
			m_reportInfo.SavePaginationMetadata(m_report.Done, m_pageContext.PageTotalInfo);
		}
	}
}
