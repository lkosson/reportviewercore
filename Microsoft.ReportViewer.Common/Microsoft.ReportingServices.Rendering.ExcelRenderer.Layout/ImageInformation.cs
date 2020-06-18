using Microsoft.ReportingServices.OnDemandReportRendering;
using Microsoft.ReportingServices.Rendering.ExcelRenderer.Excel;
using Microsoft.ReportingServices.Rendering.RPLProcessing;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace Microsoft.ReportingServices.Rendering.ExcelRenderer.Layout
{
	internal class ImageInformation
	{
		internal const string GIFMIMETYPE = "image/gif";

		internal const string JPGMIMETYPE = "image/jpg";

		internal const string JPEGMIMETYPE = "image/jpeg";

		internal const string PNGMIMETYPE = "image/png";

		internal const string BMPMIMETYPE = "image/bmp";

		internal const string XPNGMIMETYPE = "image/x-png";

		private Stream m_imageData;

		private RPLFormat.Sizings m_imageSizings = RPLFormat.Sizings.Fit;

		private ImageFormat m_imageFormat;

		private string m_imageName;

		private int m_width;

		private int m_height;

		private float m_horizontalResolution;

		private float m_verticalResolution;

		private string m_hyperlinkURL;

		private bool m_hyperlinkIsBookmark;

		private PaddingInformation m_paddings;

		internal Stream ImageData
		{
			get
			{
				return m_imageData;
			}
			set
			{
				m_imageData = value;
			}
		}

		internal string ImageName
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

		internal RPLFormat.Sizings ImageSizings
		{
			get
			{
				return m_imageSizings;
			}
			set
			{
				m_imageSizings = value;
			}
		}

		internal ImageFormat ImageFormat
		{
			get
			{
				if (m_imageFormat == null)
				{
					CalculateMetrics();
				}
				return m_imageFormat;
			}
			set
			{
				m_imageFormat = value;
			}
		}

		internal int Width
		{
			get
			{
				if (m_width == 0)
				{
					CalculateMetrics();
				}
				return m_width;
			}
			set
			{
				m_width = value;
			}
		}

		internal int Height
		{
			get
			{
				if (m_height == 0)
				{
					CalculateMetrics();
				}
				return m_height;
			}
			set
			{
				m_height = value;
			}
		}

		internal float HorizontalResolution
		{
			get
			{
				if (m_horizontalResolution == 0f)
				{
					CalculateMetrics();
				}
				return m_horizontalResolution;
			}
			set
			{
				m_horizontalResolution = value;
			}
		}

		internal float VerticalResolution
		{
			get
			{
				if (m_verticalResolution == 0f)
				{
					CalculateMetrics();
				}
				return m_verticalResolution;
			}
			set
			{
				m_verticalResolution = value;
			}
		}

		internal string HyperlinkURL
		{
			get
			{
				return m_hyperlinkURL;
			}
			set
			{
				m_hyperlinkURL = value;
			}
		}

		internal bool HyperlinkIsBookmark
		{
			get
			{
				return m_hyperlinkIsBookmark;
			}
			set
			{
				m_hyperlinkIsBookmark = value;
			}
		}

		internal PaddingInformation Paddings
		{
			get
			{
				return m_paddings;
			}
			set
			{
				m_paddings = value;
			}
		}

		internal RPLFormat.Sizings Sizings
		{
			set
			{
				m_imageSizings = value;
			}
		}

		internal ImageInformation()
		{
		}

		internal void ReadImage(IExcelGenerator excel, RPLImageData image, string imageName, RPLReport report)
		{
			if (excel == null || image == null || report == null || imageName == null)
			{
				return;
			}
			SetMimeType(image.ImageMimeType);
			m_imageName = imageName;
			if (image.ImageData != null)
			{
				m_imageData = excel.CreateStream(imageName);
				m_imageData.Write(image.ImageData, 0, image.ImageData.Length);
			}
			else
			{
				if (image.ImageDataOffset <= 0)
				{
					m_imageData = null;
					image.ImageData = null;
					return;
				}
				m_imageData = excel.CreateStream(imageName);
				report.GetImage(image.ImageDataOffset, m_imageData);
			}
			image.ImageData = null;
			if (image.GDIImageProps != null)
			{
				m_width = image.GDIImageProps.Width;
				m_height = image.GDIImageProps.Height;
				m_verticalResolution = image.GDIImageProps.VerticalResolution;
				m_horizontalResolution = image.GDIImageProps.HorizontalResolution;
				m_imageFormat = image.GDIImageProps.RawFormat;
			}
		}

		internal void SetMimeType(string mimeType)
		{
			if (mimeType == null)
			{
				return;
			}
			if (mimeType.Equals("image/gif"))
			{
				m_imageFormat = ImageFormat.Gif;
				return;
			}
			if (mimeType.Equals("image/png"))
			{
				m_imageFormat = ImageFormat.Png;
				return;
			}
			if (mimeType.Equals("image/jpg") || mimeType.Equals("image/jpeg"))
			{
				m_imageFormat = ImageFormat.Jpeg;
				return;
			}
			if (mimeType.Equals("image/bmp"))
			{
				m_imageFormat = ImageFormat.Png;
				return;
			}
			throw new ReportRenderingException(ExcelRenderRes.UnknownImageFormat(mimeType));
		}

		private void CalculateMetrics()
		{
			if (m_imageData != null && m_imageData.Length != 0L)
			{
				m_imageData.Position = 0L;
				System.Drawing.Image image = System.Drawing.Image.FromStream(m_imageData);
				m_imageFormat = image.RawFormat;
				Width = image.Width;
				Height = image.Height;
				HorizontalResolution = image.HorizontalResolution;
				VerticalResolution = image.VerticalResolution;
				image.Dispose();
			}
		}
	}
}
