using Microsoft.ReportingServices.OnDemandReportRendering;
using System.IO;

namespace Microsoft.ReportingServices.Rendering.SPBProcessing
{
	internal class InteractivityChunks
	{
		internal enum Token : byte
		{
			BookmarkInformation,
			LabelInformation,
			PageInformation,
			PageInformationOffset,
			EndObject
		}

		internal const string LabelsChunk = "Labels";

		internal const string BookmarksChunk = "Bookmarks";

		protected BinaryWriter m_writer;

		protected int m_page;

		private Stream m_stream;

		internal int Page
		{
			get
			{
				return m_page;
			}
			set
			{
				m_page = value;
				if (m_writer == null)
				{
					m_writer = new BinaryWriter(m_stream);
				}
			}
		}

		internal static int FindBoomark(Microsoft.ReportingServices.OnDemandReportRendering.Report report, string bookmarkId, ref string uniqueName, ref int lastPageCollected, ref bool reportDone)
		{
			int result = 0;
			string text = null;
			Stream chunk = report.GetChunk(Microsoft.ReportingServices.OnDemandReportRendering.Report.ChunkTypes.Interactivity, "Bookmarks");
			if (chunk != null && chunk.Length > 0)
			{
				chunk.Position = 0L;
				BinaryReader binaryReader = new BinaryReader(chunk);
				byte b = binaryReader.ReadByte();
				while (true)
				{
					switch (b)
					{
					case 0:
						text = binaryReader.ReadString();
						if (SPBProcessing.CompareWithOrdinalComparison(bookmarkId, text, ignoreCase: false) != 0)
						{
							goto IL_0059;
						}
						uniqueName = binaryReader.ReadString();
						return binaryReader.ReadInt32();
					case 2:
						lastPageCollected = binaryReader.ReadInt32();
						reportDone = binaryReader.ReadBoolean();
						break;
					}
					break;
					IL_0059:
					binaryReader.ReadString();
					binaryReader.ReadInt32();
					binaryReader.ReadByte();
					b = binaryReader.ReadByte();
				}
			}
			return result;
		}

		internal static int FindDocumentMapLabel(Microsoft.ReportingServices.OnDemandReportRendering.Report report, string documentMapId, ref int lastPageCollected, ref bool reportDone)
		{
			int result = 0;
			string text = null;
			Stream chunk = report.GetChunk(Microsoft.ReportingServices.OnDemandReportRendering.Report.ChunkTypes.Interactivity, "Labels");
			if (chunk != null && chunk.Length > 0)
			{
				chunk.Position = 0L;
				BinaryReader binaryReader = new BinaryReader(chunk);
				byte b = binaryReader.ReadByte();
				while (true)
				{
					switch (b)
					{
					case 1:
						text = binaryReader.ReadString();
						if (SPBProcessing.CompareWithOrdinalComparison(documentMapId, text, ignoreCase: true) != 0)
						{
							goto IL_0051;
						}
						return binaryReader.ReadInt32();
					case 2:
						lastPageCollected = binaryReader.ReadInt32();
						reportDone = binaryReader.ReadBoolean();
						break;
					}
					break;
					IL_0051:
					binaryReader.ReadInt32();
					binaryReader.ReadByte();
					b = binaryReader.ReadByte();
				}
			}
			return result;
		}

		private static Stream GetInteractivityChunck(Microsoft.ReportingServices.OnDemandReportRendering.Report report, string chunkName, int page, out int lastPage)
		{
			lastPage = 0;
			Stream stream = null;
			bool isNewChunk = false;
			stream = ((page != 1) ? report.GetChunk(Microsoft.ReportingServices.OnDemandReportRendering.Report.ChunkTypes.Interactivity, chunkName) : report.GetOrCreateChunk(Microsoft.ReportingServices.OnDemandReportRendering.Report.ChunkTypes.Interactivity, chunkName, out isNewChunk));
			if (stream == null)
			{
				return null;
			}
			if (!isNewChunk)
			{
				long num = stream.Length - 9;
				if (num > 0)
				{
					stream.Seek(num, SeekOrigin.Begin);
					BinaryReader binaryReader = new BinaryReader(stream);
					binaryReader.ReadByte();
					num = 9 + binaryReader.ReadInt64();
					stream.Seek(-num, SeekOrigin.End);
					binaryReader.ReadByte();
					lastPage = binaryReader.ReadInt32();
					if (binaryReader.ReadBoolean())
					{
						return null;
					}
					stream.Seek(-num, SeekOrigin.End);
				}
				else
				{
					stream.Seek(0L, SeekOrigin.Begin);
				}
			}
			return stream;
		}

		internal static Bookmarks GetBookmarksStream(Microsoft.ReportingServices.OnDemandReportRendering.Report report, int page)
		{
			int lastPage = 0;
			Stream interactivityChunck = GetInteractivityChunck(report, "Bookmarks", page, out lastPage);
			if (interactivityChunck != null)
			{
				return new Bookmarks(interactivityChunck, lastPage);
			}
			return null;
		}

		internal static DocumentMapLabels GetLabelsStream(Microsoft.ReportingServices.OnDemandReportRendering.Report report, int page)
		{
			int lastPage = 0;
			Stream interactivityChunck = GetInteractivityChunck(report, "Labels", page, out lastPage);
			if (interactivityChunck != null)
			{
				return new DocumentMapLabels(interactivityChunck, lastPage);
			}
			return null;
		}

		internal InteractivityChunks(Stream stream, int page)
		{
			m_stream = stream;
			m_page = page;
		}

		internal void Flush(bool reportDone)
		{
			if (m_writer != null)
			{
				long position = m_stream.Position;
				m_writer.Write((byte)2);
				m_writer.Write(m_page);
				m_writer.Write(reportDone);
				m_writer.Write((byte)4);
				position = m_stream.Position - position;
				m_writer.Write((byte)3);
				m_writer.Write(position);
				m_writer = null;
			}
		}
	}
}
