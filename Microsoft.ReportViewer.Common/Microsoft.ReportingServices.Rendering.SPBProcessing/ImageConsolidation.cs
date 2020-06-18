using Microsoft.ReportingServices.Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;

namespace Microsoft.ReportingServices.Rendering.SPBProcessing
{
	internal class ImageConsolidation
	{
		protected const float SUPPORTED_DPI = 96f;

		protected const float DPI_TOLERANCE = 0.02f;

		private const float OUTPUT_DPI = 96f;

		public static string STREAMPREFIX = "IMGCON_";

		public List<ImageInfo> ImageInfos = new List<ImageInfo>();

		public int MaxHeight;

		public int MaxWidth;

		public int CurrentOffset;

		private static int MAXIMAGECONSOLIDATION_TOTALSIZE = 200000;

		private static int MAXIMAGECONSOLIDATION_PERIMAGESIZE = MAXIMAGECONSOLIDATION_TOTALSIZE / 10;

		private CreateAndRegisterStream m_createAndRegisterStream;

		private int m_currentByteCount;

		private int m_ignoreOffsetTill = -1;

		private string m_imagePrefix;

		public int IgnoreOffsetTill => m_ignoreOffsetTill;

		public ImageConsolidation(CreateAndRegisterStream createAndRegisterStream)
			: this(createAndRegisterStream, -1)
		{
		}

		public ImageConsolidation(CreateAndRegisterStream createAndRegisterStream, int ignoreOffsetTill)
		{
			m_createAndRegisterStream = createAndRegisterStream;
			m_ignoreOffsetTill = ignoreOffsetTill;
		}

		public System.Drawing.Rectangle AppendImage(Stream imageStream)
		{
			if (imageStream == null)
			{
				return System.Drawing.Rectangle.Empty;
			}
			long length = imageStream.Length;
			if (length > MAXIMAGECONSOLIDATION_PERIMAGESIZE)
			{
				return System.Drawing.Rectangle.Empty;
			}
			if (m_currentByteCount + length > MAXIMAGECONSOLIDATION_TOTALSIZE)
			{
				RenderToStream();
				if (m_ignoreOffsetTill > -1 && m_ignoreOffsetTill + 1 == CurrentOffset)
				{
					return System.Drawing.Rectangle.Empty;
				}
			}
			ImageInfo imageInfo = new ImageInfo();
			imageInfo.ImageData = imageStream;
			long position = imageStream.Position;
			float dpiX = 0f;
			float dpiY = 0f;
			int num = 1;
			ImageFormat imageFormat = ImageFormat.Png;
			try
			{
				using (System.Drawing.Image image = System.Drawing.Image.FromStream(imageStream))
				{
					imageInfo.Width = image.Width;
					imageInfo.Height = image.Height;
					dpiX = image.HorizontalResolution;
					dpiY = image.VerticalResolution;
					imageFormat = image.RawFormat;
					num = image.FrameDimensionsList.Length;
					if (num == 1)
					{
						num = image.GetFrameCount(new FrameDimension(image.FrameDimensionsList[0]));
					}
				}
			}
			catch (Exception)
			{
				return System.Drawing.Rectangle.Empty;
			}
			if (!IsDPISupported(dpiX, dpiY) || num != 1 || imageFormat.Guid != ImageFormat.Png.Guid)
			{
				return System.Drawing.Rectangle.Empty;
			}
			System.Drawing.Rectangle result = System.Drawing.Rectangle.Empty;
			if (CurrentOffset >= m_ignoreOffsetTill)
			{
				ImageInfos.Add(imageInfo);
				imageStream.Position = position;
				result = new System.Drawing.Rectangle(0, MaxHeight, imageInfo.Width, imageInfo.Height);
				MaxHeight += imageInfo.Height;
				MaxWidth = Math.Max(MaxWidth, imageInfo.Width);
			}
			m_currentByteCount += (int)length;
			return result;
		}

		public System.Drawing.Image Render()
		{
			if (ImageInfos.Count == 0 || MaxWidth == 0 || MaxHeight == 0)
			{
				return null;
			}
			Bitmap bitmap = new Bitmap(MaxWidth, MaxHeight);
			if (bitmap.HorizontalResolution != 96f || bitmap.VerticalResolution != 96f)
			{
				bitmap.SetResolution(96f, 96f);
			}
			using (Graphics g = Graphics.FromImage(bitmap))
			{
				int num = 0;
				foreach (ImageInfo imageInfo in ImageInfos)
				{
					imageInfo.RenderAndDispose(g, 0, num);
					num += imageInfo.Height;
				}
			}
			ImageInfos.Clear();
			return bitmap;
		}

		public static string GetStreamName(string reportName, int page)
		{
			if (page > 0)
			{
				return STREAMPREFIX + page.ToString(CultureInfo.InvariantCulture);
			}
			return STREAMPREFIX;
		}

		public string GetStreamName()
		{
			return m_imagePrefix + CurrentOffset;
		}

		public void SetName(string reportName, int pageNumber)
		{
			m_imagePrefix = STREAMPREFIX + pageNumber.ToString(CultureInfo.InvariantCulture) + "_";
		}

		public void RenderToStream()
		{
			if (m_currentByteCount > 0 && ImageInfos.Count > 0)
			{
				string streamName = GetStreamName();
				Stream stream = m_createAndRegisterStream(streamName, "png", null, PageContext.PNG_MIME_TYPE, willSeek: false, StreamOper.CreateAndRegister);
				using (System.Drawing.Image image = Render())
				{
					image?.Save(stream, ImageFormat.Png);
				}
			}
			CurrentOffset++;
			m_currentByteCount = 0;
			MaxHeight = 0;
			MaxWidth = 0;
		}

		public void ResetCancelPage()
		{
			if (CurrentOffset > 0 && m_ignoreOffsetTill < CurrentOffset)
			{
				m_ignoreOffsetTill = CurrentOffset;
			}
			CurrentOffset = 0;
			foreach (ImageInfo imageInfo in ImageInfos)
			{
				imageInfo.Dispose();
			}
			ImageInfos.Clear();
			m_currentByteCount = 0;
			MaxHeight = 0;
			MaxWidth = 0;
		}

		public void Reset()
		{
			CurrentOffset = 0;
		}

		private bool IsDPISupported(float dpiX, float dpiY)
		{
			if (95.98f < dpiX && 96.02f > dpiX && 95.98f < dpiY)
			{
				return 96.02f > dpiY;
			}
			return false;
		}
	}
}
