using Microsoft.ReportingServices.Diagnostics.Utilities;
using Microsoft.ReportingServices.OnDemandReportRendering;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;

namespace Microsoft.ReportingServices.Rendering.SPBProcessing
{
	internal class ReportPaginationInfo
	{
		internal const string PaginationInfoChunk = "PaginationInfo";

		private Version m_version = new Version(0, 0, 0);

		private int m_paginatedPages = -1;

		private Stream m_stream;

		private BinaryReader m_reader;

		private BinaryWriter m_writer;

		private List<long> m_metadataPages;

		private long m_offsetLastPage = -1L;

		private long m_regionPageTotalInfoOffset = -1L;

		private long m_offsetHeader = -1L;

		private bool m_reportDone;

		private double m_pageHeight;

		private bool m_newPagesMetadata;

		internal Version Version => m_version;

		internal int PaginatedPages => m_paginatedPages;

		internal long OffsetLastPage => m_offsetLastPage;

		internal BinaryWriter BinaryWriter
		{
			get
			{
				if (m_stream != null)
				{
					if (m_writer == null)
					{
						m_writer = new BinaryWriter(m_stream, Encoding.Unicode);
					}
					return m_writer;
				}
				return null;
			}
		}

		internal BinaryReader BinaryReader
		{
			get
			{
				if (m_stream != null)
				{
					if (m_reader == null)
					{
						m_reader = new BinaryReader(m_stream, Encoding.Unicode);
					}
					return m_reader;
				}
				return null;
			}
		}

		internal bool IsDone => m_reportDone;

		internal ReportPaginationInfo()
		{
			m_stream = null;
			m_version = new Version(0, 0, 0);
			m_paginatedPages = -1;
			m_offsetLastPage = -1L;
			m_offsetHeader = -1L;
			m_pageHeight = 0.0;
			m_metadataPages = null;
			m_reportDone = false;
		}

		internal ReportPaginationInfo(Microsoft.ReportingServices.OnDemandReportRendering.Report report, Version serverRPLVersion, double pageHeight)
		{
			bool isNewChunk = false;
			m_stream = report.GetOrCreateChunk(Microsoft.ReportingServices.OnDemandReportRendering.Report.ChunkTypes.Pagination, "PaginationInfo", out isNewChunk);
			if (m_stream != null)
			{
				if (isNewChunk)
				{
					m_version = serverRPLVersion;
					m_pageHeight = pageHeight;
					SavePaginationInfo();
					m_paginatedPages = 0;
					m_metadataPages = new List<long>();
					m_reportDone = false;
				}
				else if (ExtractPaginationInfo(serverRPLVersion))
				{
					m_stream.Close();
					m_stream = report.CreateChunk(Microsoft.ReportingServices.OnDemandReportRendering.Report.ChunkTypes.Pagination, "PaginationInfo");
					m_version = serverRPLVersion;
					m_pageHeight = pageHeight;
					SavePaginationInfo();
					m_paginatedPages = 0;
					m_metadataPages = new List<long>();
					m_reportDone = false;
				}
				else if (pageHeight != m_pageHeight)
				{
					m_stream = null;
					m_version = new Version(0, 0, 0);
					m_paginatedPages = -1;
					m_offsetLastPage = -1L;
					m_offsetHeader = -1L;
					m_pageHeight = 0.0;
					m_metadataPages = null;
					m_reportDone = false;
				}
			}
		}

		internal bool HasPaginationInfoStream()
		{
			Version value = new Version(0, 0, 0);
			if (m_version.CompareTo(value) == 0)
			{
				return false;
			}
			return true;
		}

		internal void ReadPageInfo(int page, out ReportSectionHelper pageInfo)
		{
			RSTrace.RenderingTracer.Assert(m_stream != null, "The pagination stream is null.");
			RSTrace.RenderingTracer.Assert(m_paginatedPages >= 0, "The number of paginated pages is negative.");
			RSTrace.RenderingTracer.Assert(page >= 0 && page <= m_paginatedPages, "The number of the solicited page is outside of the interval: 0 - " + m_paginatedPages.ToString(CultureInfo.InvariantCulture) + ".");
			pageInfo = null;
			switch (page)
			{
			case 0:
				return;
			case 1:
			{
				BinaryReader.BaseStream.Seek(m_offsetHeader, SeekOrigin.Begin);
				long offsetEndPage = m_metadataPages[0];
				if (RSTrace.RenderingTracer.TraceVerbose)
				{
					RSTrace.RenderingTracer.Trace(TraceLevel.Verbose, "We extract from stream the page: " + page.ToString(CultureInfo.InvariantCulture));
				}
				pageInfo = ReportSectionHelper.ReadReportSection(BinaryReader, offsetEndPage);
				return;
			}
			}
			long offset = m_metadataPages[page - 2];
			long offsetEndPage2 = m_metadataPages[page - 1];
			BinaryReader.BaseStream.Seek(offset, SeekOrigin.Begin);
			if (RSTrace.RenderingTracer.TraceVerbose)
			{
				RSTrace.RenderingTracer.Trace(TraceLevel.Verbose, "We extract from stream the page: " + page.ToString(CultureInfo.InvariantCulture));
			}
			pageInfo = ReportSectionHelper.ReadReportSection(BinaryReader, offsetEndPage2);
		}

		internal void ReadRegionPageTotalInfo(PageTotalInfo pageTotalInfo)
		{
			if (m_regionPageTotalInfoOffset <= 0)
			{
				return;
			}
			BinaryReader.BaseStream.Seek(m_regionPageTotalInfoOffset, SeekOrigin.Begin);
			if (BinaryReader.ReadBoolean())
			{
				bool isCalculationDone = BinaryReader.ReadBoolean();
				bool isCounting = BinaryReader.ReadBoolean();
				int num = BinaryReader.ReadInt32();
				List<KeyValuePair<int, string>> list = new List<KeyValuePair<int, string>>(num);
				for (int i = 0; i < num; i++)
				{
					int key = BinaryReader.ReadInt32();
					string value = BinaryReader.ReadString();
					list.Add(new KeyValuePair<int, string>(key, value));
				}
				num = BinaryReader.ReadInt32();
				List<KeyValuePair<int, int>> list2 = new List<KeyValuePair<int, int>>(num);
				for (int j = 0; j < num; j++)
				{
					int key2 = BinaryReader.ReadInt32();
					int value2 = BinaryReader.ReadInt32();
					list2.Add(new KeyValuePair<int, int>(key2, value2));
				}
				pageTotalInfo.SetupPageTotalInfo(isCalculationDone, isCounting, list2, list);
			}
		}

		internal void SavePaginationMetadata(bool reportDone, PageTotalInfo pageTotalInfo)
		{
			if (m_stream == null || m_reportDone || !m_newPagesMetadata)
			{
				return;
			}
			m_reportDone = reportDone;
			BinaryWriter.BaseStream.Seek(m_offsetLastPage, SeekOrigin.Begin);
			BinaryWriter.Write(m_reportDone);
			BinaryWriter.Write(m_metadataPages.Count);
			for (int i = 0; i < m_metadataPages.Count; i++)
			{
				BinaryWriter.Write(m_metadataPages[i]);
			}
			m_regionPageTotalInfoOffset = BinaryWriter.BaseStream.Position;
			if (pageTotalInfo == null)
			{
				BinaryWriter.Write(value: false);
			}
			else
			{
				BinaryWriter.Write(value: true);
				BinaryWriter.Write(pageTotalInfo.CalculationDone);
				BinaryWriter.Write(pageTotalInfo.IsCounting);
				List<KeyValuePair<int, string>> pageNameList = pageTotalInfo.GetPageNameList();
				BinaryWriter.Write(pageNameList.Count);
				foreach (KeyValuePair<int, string> item in pageNameList)
				{
					BinaryWriter.Write(item.Key);
					BinaryWriter.Write(item.Value);
				}
				List<KeyValuePair<int, int>> pageNumberList = pageTotalInfo.GetPageNumberList();
				BinaryWriter.Write(pageNumberList.Count);
				foreach (KeyValuePair<int, int> item2 in pageNumberList)
				{
					BinaryWriter.Write(item2.Key);
					BinaryWriter.Write(item2.Value);
				}
			}
			BinaryWriter.Write(m_regionPageTotalInfoOffset);
			BinaryWriter.Flush();
			m_newPagesMetadata = false;
		}

		internal void UpdateReportInfo()
		{
			if (m_stream != null)
			{
				m_metadataPages.Add(BinaryWriter.BaseStream.Position);
				m_paginatedPages++;
				m_offsetLastPage = BinaryWriter.BaseStream.Position;
				m_newPagesMetadata = true;
			}
		}

		private void SavePaginationInfo()
		{
			RSTrace.RenderingTracer.Assert(m_stream != null, "The pagination stream is null.");
			BinaryWriter.BaseStream.Seek(0L, SeekOrigin.Begin);
			BinaryWriter.Write("RPLIF");
			BinaryWriter.Write((byte)m_version.Major);
			BinaryWriter.Write((byte)m_version.Minor);
			BinaryWriter.Write(m_version.Build);
			BinaryWriter.Write(m_pageHeight);
			m_offsetHeader = BinaryWriter.BaseStream.Position;
			m_offsetLastPage = BinaryWriter.BaseStream.Position;
		}

		private bool ExtractPaginationInfo(Version serverRPLVersion)
		{
			RSTrace.RenderingTracer.Assert(m_stream != null, "The pagination stream is null.");
			RSTrace.RenderingTracer.Assert(serverRPLVersion != null, "The version of the server shouldn't be null");
			BinaryReader.BaseStream.Seek(0L, SeekOrigin.Begin);
			BinaryReader.ReadString();
			m_version = new Version(BinaryReader.BaseStream.ReadByte(), BinaryReader.BaseStream.ReadByte(), BinaryReader.ReadInt32());
			if (NeedsNewPaginationInfoStream(serverRPLVersion))
			{
				return true;
			}
			m_pageHeight = BinaryReader.ReadDouble();
			m_offsetHeader = BinaryReader.BaseStream.Position;
			BinaryReader.BaseStream.Seek(-8L, SeekOrigin.End);
			m_regionPageTotalInfoOffset = BinaryReader.ReadInt64();
			BinaryReader.BaseStream.Seek(m_regionPageTotalInfoOffset - 8, SeekOrigin.Begin);
			m_offsetLastPage = BinaryReader.ReadInt64();
			if (m_offsetLastPage > 0)
			{
				BinaryReader.BaseStream.Seek(m_offsetLastPage, SeekOrigin.Begin);
				m_reportDone = BinaryReader.ReadBoolean();
				m_paginatedPages = BinaryReader.ReadInt32();
				m_metadataPages = new List<long>(m_paginatedPages);
				for (int i = 0; i < m_paginatedPages; i++)
				{
					long item = BinaryReader.ReadInt64();
					m_metadataPages.Add(item);
					if (i == 0 && m_metadataPages[i] <= m_offsetHeader)
					{
						throw new InvalidDataException(SPBRes.InvalidPaginationStream);
					}
				}
				BinaryReader.BaseStream.Seek(m_offsetLastPage, SeekOrigin.Begin);
				return false;
			}
			throw new InvalidDataException(SPBRes.InvalidPaginationStream);
		}

		private bool NeedsNewPaginationInfoStream(Version serverRPLVersion)
		{
			RSTrace.RenderingTracer.Assert(serverRPLVersion != null, "The version of the server shouldn't be null");
			int major = serverRPLVersion.Major;
			int minor = serverRPLVersion.Minor;
			int major2 = m_version.Major;
			int minor2 = m_version.Minor;
			if (major2 == major && minor2 == minor)
			{
				return false;
			}
			return true;
		}
	}
}
