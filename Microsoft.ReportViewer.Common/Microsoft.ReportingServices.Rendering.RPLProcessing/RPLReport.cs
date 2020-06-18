using System;
using System.IO;

namespace Microsoft.ReportingServices.Rendering.RPLProcessing
{
	internal sealed class RPLReport
	{
		private RPLPageContent[] m_rplPaginatedPages;

		private string m_location;

		private string m_description;

		private string m_language;

		private string m_author;

		private int m_autoRefresh;

		private string m_reportName;

		private DateTime m_executionTime;

		private Version m_rplVersion;

		private RPLContext m_rplContext;

		private bool m_consumeContainerWhitespace;

		public Version RPLVersion
		{
			get
			{
				return m_rplVersion;
			}
			set
			{
				m_rplVersion = value;
			}
		}

		public string ReportName
		{
			get
			{
				return m_reportName;
			}
			set
			{
				m_reportName = value;
			}
		}

		public string Description
		{
			get
			{
				return m_description;
			}
			set
			{
				m_description = value;
			}
		}

		public string Author
		{
			get
			{
				return m_author;
			}
			set
			{
				m_author = value;
			}
		}

		public int AutoRefresh
		{
			get
			{
				return m_autoRefresh;
			}
			set
			{
				m_autoRefresh = value;
			}
		}

		public DateTime ExecutionTime
		{
			get
			{
				return m_executionTime;
			}
			set
			{
				m_executionTime = value;
			}
		}

		public string Location
		{
			get
			{
				return m_location;
			}
			set
			{
				m_location = value;
			}
		}

		public string Language
		{
			get
			{
				return m_language;
			}
			set
			{
				m_language = value;
			}
		}

		public RPLPageContent[] RPLPaginatedPages
		{
			get
			{
				return m_rplPaginatedPages;
			}
			set
			{
				m_rplPaginatedPages = value;
			}
		}

		public bool ConsumeContainerWhitespace
		{
			get
			{
				return m_consumeContainerWhitespace;
			}
			set
			{
				m_consumeContainerWhitespace = value;
			}
		}

		internal RPLReport()
		{
		}

		public RPLReport(BinaryReader reader)
		{
			m_rplContext = new RPLContext(reader);
			RPLReader.ReadReport(this, m_rplContext);
		}

		public void GetImage(long offset, Stream imageStream)
		{
			if (offset >= 0 && imageStream != null && m_rplContext.BinaryReader != null)
			{
				BinaryReader binaryReader = m_rplContext.BinaryReader;
				Stream baseStream = binaryReader.BaseStream;
				baseStream.Seek(offset, SeekOrigin.Begin);
				int num = binaryReader.ReadInt32();
				byte[] array = new byte[4096];
				while (num > 0)
				{
					int num2 = baseStream.Read(array, 0, Math.Min(array.Length, num));
					imageStream.Write(array, 0, num2);
					num -= num2;
				}
			}
		}

		public byte[] GetImage(long offset)
		{
			if (offset < 0 || m_rplContext.BinaryReader == null)
			{
				return null;
			}
			BinaryReader binaryReader = m_rplContext.BinaryReader;
			Stream baseStream = binaryReader.BaseStream;
			baseStream.Seek(offset, SeekOrigin.Begin);
			int num = binaryReader.ReadInt32();
			byte[] array = new byte[num];
			baseStream.Read(array, 0, num);
			return array;
		}

		public RPLItemProps GetItemProps(long startOffset, out byte elementType)
		{
			elementType = 0;
			if (startOffset < 0 || m_rplContext.BinaryReader == null)
			{
				return null;
			}
			return RPLReader.ReadElementProps(startOffset, m_rplContext, out elementType);
		}

		public RPLItemProps GetItemProps(object rplSource, out byte elementType)
		{
			elementType = 0;
			RPLItemProps rPLItemProps = rplSource as RPLItemProps;
			if (rPLItemProps != null)
			{
				if (rPLItemProps is RPLTextBoxProps)
				{
					elementType = 7;
				}
				else if (rPLItemProps is RPLChartProps)
				{
					elementType = 11;
				}
				else if (rPLItemProps is RPLGaugePanelProps)
				{
					elementType = 14;
				}
				else if (rPLItemProps is RPLMapProps)
				{
					elementType = 21;
				}
				else if (rPLItemProps is RPLImageProps)
				{
					elementType = 9;
				}
				else if (rPLItemProps is RPLLineProps)
				{
					elementType = 8;
				}
				return rPLItemProps;
			}
			long num = (long)rplSource;
			if (num < 0 || m_rplContext.BinaryReader == null)
			{
				return null;
			}
			return RPLReader.ReadElementProps(num, m_rplContext, out elementType);
		}

		public void Release()
		{
			if (m_rplContext != null)
			{
				m_rplContext.Release();
				m_rplContext = null;
			}
		}
	}
}
