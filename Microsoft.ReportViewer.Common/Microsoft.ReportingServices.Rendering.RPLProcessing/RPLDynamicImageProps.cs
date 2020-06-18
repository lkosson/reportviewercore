using System.Drawing;
using System.IO;

namespace Microsoft.ReportingServices.Rendering.RPLProcessing
{
	internal class RPLDynamicImageProps : RPLItemProps
	{
		private string m_streamName;

		private long m_dynamicImageContentOffset = -1L;

		private Stream m_dynamicImageContent;

		private RPLActionInfoWithImageMap[] m_actionImageMaps;

		private Rectangle m_offsets = Rectangle.Empty;

		public string StreamName
		{
			get
			{
				return m_streamName;
			}
			set
			{
				m_streamName = value;
			}
		}

		public long DynamicImageContentOffset
		{
			get
			{
				return m_dynamicImageContentOffset;
			}
			set
			{
				m_dynamicImageContentOffset = value;
			}
		}

		public Stream DynamicImageContent
		{
			get
			{
				return m_dynamicImageContent;
			}
			set
			{
				m_dynamicImageContent = value;
			}
		}

		public RPLActionInfoWithImageMap[] ActionImageMapAreas
		{
			get
			{
				return m_actionImageMaps;
			}
			set
			{
				m_actionImageMaps = value;
			}
		}

		public Rectangle ImageConsolidationOffsets
		{
			get
			{
				return m_offsets;
			}
			set
			{
				m_offsets = value;
			}
		}

		internal RPLDynamicImageProps()
		{
		}
	}
}
