using System.Drawing;

namespace Microsoft.ReportingServices.Rendering.RPLProcessing
{
	internal sealed class RPLImageData
	{
		private string m_imageName;

		private string m_imageMimeType;

		private bool m_isShared;

		private long m_imageDataOffset = -1L;

		private byte[] m_stream;

		private GDIImageProps m_gdiImageProps;

		private Rectangle m_offsets = Rectangle.Empty;

		public string ImageName
		{
			get
			{
				return m_imageName;
			}
			set
			{
				m_imageName = value;
			}
		}

		public string ImageMimeType
		{
			get
			{
				return m_imageMimeType;
			}
			set
			{
				m_imageMimeType = value;
			}
		}

		public long ImageDataOffset
		{
			get
			{
				return m_imageDataOffset;
			}
			set
			{
				m_imageDataOffset = value;
			}
		}

		public byte[] ImageData
		{
			get
			{
				return m_stream;
			}
			set
			{
				m_stream = value;
			}
		}

		public GDIImageProps GDIImageProps
		{
			get
			{
				return m_gdiImageProps;
			}
			set
			{
				m_gdiImageProps = value;
			}
		}

		public bool IsShared
		{
			get
			{
				return m_isShared;
			}
			set
			{
				m_isShared = value;
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
	}
}
