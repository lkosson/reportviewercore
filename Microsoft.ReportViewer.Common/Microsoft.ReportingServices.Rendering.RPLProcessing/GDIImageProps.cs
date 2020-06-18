using System.Drawing;
using System.Drawing.Imaging;

namespace Microsoft.ReportingServices.Rendering.RPLProcessing
{
	internal sealed class GDIImageProps
	{
		private int m_width;

		private int m_height;

		private float m_horizontalResolution;

		private float m_verticalResolution;

		private ImageFormat m_rawFormat;

		public int Width
		{
			get
			{
				return m_width;
			}
			set
			{
				m_width = value;
			}
		}

		public int Height
		{
			get
			{
				return m_height;
			}
			set
			{
				m_height = value;
			}
		}

		public float VerticalResolution
		{
			get
			{
				return m_verticalResolution;
			}
			set
			{
				m_verticalResolution = value;
			}
		}

		public float HorizontalResolution
		{
			get
			{
				return m_horizontalResolution;
			}
			set
			{
				m_horizontalResolution = value;
			}
		}

		public ImageFormat RawFormat
		{
			get
			{
				return m_rawFormat;
			}
			set
			{
				m_rawFormat = value;
			}
		}

		internal GDIImageProps()
		{
		}

		public GDIImageProps(Image image)
		{
			m_width = image.Width;
			m_height = image.Height;
			m_horizontalResolution = image.HorizontalResolution;
			m_verticalResolution = image.VerticalResolution;
			m_rawFormat = image.RawFormat;
		}
	}
}
