using System;

namespace Microsoft.ReportingServices.Library
{
	[Serializable]
	internal sealed class ChunkHeader
	{
		internal static readonly short MissingVersion = 1;

		internal static readonly short CurrentVersion = 2;

		private ChunkFlags m_chunkFlags;

		private int m_chunkType;

		private string m_chunkName;

		private string m_mimeType;

		private long m_chunkSize;

		private short m_version;

		internal string MimeType => m_mimeType;

		internal string ChunkName
		{
			get
			{
				return m_chunkName;
			}
			set
			{
				m_chunkName = value;
			}
		}

		internal int ChunkType => m_chunkType;

		internal ChunkFlags ChunkFlag => m_chunkFlags;

		internal long ChunkSize => m_chunkSize;

		internal short Version => m_version;

		internal ChunkHeader(string chunkName, int chunkType, ChunkFlags chunkFlag, string mimeType, short version, long chunkSize)
		{
			m_chunkName = chunkName;
			m_chunkType = chunkType;
			m_chunkFlags = chunkFlag;
			m_mimeType = mimeType;
			m_version = version;
			m_chunkSize = chunkSize;
		}

		internal ChunkHeader(ChunkHeader baseHeader)
			: this(baseHeader.ChunkName, baseHeader.ChunkType, baseHeader.ChunkFlag, baseHeader.MimeType, baseHeader.Version, baseHeader.ChunkSize)
		{
		}
	}
}
