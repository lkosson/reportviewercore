using Microsoft.ReportingServices.ReportProcessing;
using System;
using System.IO;

namespace Microsoft.ReportingServices.ReportRendering
{
	internal sealed class InternalImage : ImageBase
	{
		private Image.SourceType m_imageType;

		private object m_valueObject;

		private RenderingContext m_renderingContext;

		private bool m_transparent;

		private static byte[] m_transparentImage;

		private byte[] m_imageData;

		private string m_MIMEType;

		private WeakReference m_imageDataRef;

		private string m_streamName;

		private ImageMapAreaInstanceList m_imageMapAreas;

		internal byte[] ImageData
		{
			get
			{
				if (m_imageData != null)
				{
					return m_imageData;
				}
				byte[] imageData = (m_imageDataRef != null) ? ((byte[])m_imageDataRef.Target) : null;
				if (imageData == null)
				{
					ImageInfo imageInfo = null;
					switch (m_imageType)
					{
					case Image.SourceType.External:
					{
						string imageValue2 = ImageValue;
						if (imageValue2 != null)
						{
							imageInfo = m_renderingContext.ImageStreamNames[imageValue2];
						}
						break;
					}
					case Image.SourceType.Embedded:
					{
						string imageValue = ImageValue;
						if (imageValue != null && m_renderingContext.EmbeddedImages != null)
						{
							imageInfo = m_renderingContext.EmbeddedImages[imageValue];
						}
						break;
					}
					}
					if (imageInfo != null && imageInfo.ImageDataRef != null)
					{
						m_imageDataRef = imageInfo.ImageDataRef;
						imageData = (byte[])m_imageDataRef.Target;
					}
					if (imageData == null)
					{
						GetImageData(out imageData, out string mimeType);
						if (m_renderingContext.CacheState)
						{
							m_imageData = imageData;
							m_MIMEType = mimeType;
						}
						else
						{
							m_imageDataRef = new WeakReference(imageData);
							if (imageInfo != null)
							{
								imageInfo.ImageDataRef = m_imageDataRef;
							}
						}
					}
				}
				return imageData;
			}
		}

		internal string MIMEType
		{
			get
			{
				string mimeType = m_MIMEType;
				if (mimeType == null)
				{
					GetImageMimeType(out mimeType);
					if (m_renderingContext.CacheState)
					{
						m_MIMEType = mimeType;
					}
				}
				return mimeType;
			}
		}

		internal string StreamName
		{
			get
			{
				string streamName = m_streamName;
				if (streamName == null)
				{
					string mimeType = null;
					GetImageInfo(out streamName, out mimeType);
					if (m_renderingContext.CacheState)
					{
						m_streamName = streamName;
					}
				}
				return streamName;
			}
		}

		internal ImageMapAreaInstanceList ImageMapAreaInstances => m_imageMapAreas;

		private string ImageValue => m_valueObject as string;

		private ImageData Data => m_valueObject as ImageData;

		internal static byte[] TransparentImage
		{
			get
			{
				if (m_transparentImage == null)
				{
					MemoryStream memoryStream = new MemoryStream(45);
					Microsoft.ReportingServices.ReportProcessing.ReportProcessing.RuntimeRICollection.FetchTransparentImage(memoryStream);
					m_transparentImage = memoryStream.ToArray();
				}
				return m_transparentImage;
			}
		}

		internal InternalImage(Image.SourceType imgType, string mimeType, object valueObject, RenderingContext rc)
		{
			m_imageType = imgType;
			m_MIMEType = mimeType;
			m_valueObject = valueObject;
			m_renderingContext = rc;
			m_transparent = false;
		}

		internal InternalImage(Image.SourceType imgType, string mimeType, object valueObject, RenderingContext rc, bool brokenImage, ImageMapAreaInstanceList imageMapAreas)
		{
			m_imageType = imgType;
			m_MIMEType = mimeType;
			m_valueObject = valueObject;
			m_renderingContext = rc;
			m_transparent = (!brokenImage && valueObject == null);
			if (!brokenImage)
			{
				m_imageMapAreas = imageMapAreas;
			}
		}

		private static void ReadStream(Stream input, out byte[] streamContents)
		{
			if (input == null)
			{
				streamContents = null;
				return;
			}
			int num = 1024;
			using (MemoryStream memoryStream = new MemoryStream(num))
			{
				byte[] buffer = new byte[num];
				int num2 = 0;
				while ((num2 = input.Read(buffer, 0, num)) > 0)
				{
					memoryStream.Write(buffer, 0, num2);
				}
				streamContents = memoryStream.ToArray();
			}
		}

		private void GetImageData(out byte[] imageData, out string mimeType)
		{
			if (m_transparent)
			{
				mimeType = "image/gif";
				imageData = TransparentImage;
				return;
			}
			string streamName = StreamName;
			if (streamName != null)
			{
				using (Stream input = m_renderingContext.GetChunkCallback(streamName, Microsoft.ReportingServices.ReportProcessing.ReportProcessing.ReportChunkTypes.Image, out mimeType))
				{
					ReadStream(input, out imageData);
				}
				return;
			}
			imageData = null;
			mimeType = null;
			if (GetUrlString() == null)
			{
				ImageData data = Data;
				if (data != null)
				{
					imageData = data.Data;
					mimeType = data.MIMEType;
				}
			}
		}

		private void GetImageInfo(out string streamName, out string mimeType)
		{
			streamName = null;
			mimeType = null;
			switch (m_imageType)
			{
			case Image.SourceType.External:
			{
				string imageValue2 = ImageValue;
				if (imageValue2 != null)
				{
					ImageInfo imageInfo2 = m_renderingContext.ImageStreamNames[imageValue2];
					if (imageInfo2 != null)
					{
						streamName = imageInfo2.StreamName;
						mimeType = imageInfo2.MimeType;
					}
				}
				break;
			}
			case Image.SourceType.Embedded:
			{
				string imageValue = ImageValue;
				if (imageValue != null && m_renderingContext.EmbeddedImages != null)
				{
					ImageInfo imageInfo = m_renderingContext.EmbeddedImages[imageValue];
					if (imageInfo != null)
					{
						streamName = imageInfo.StreamName;
						mimeType = imageInfo.MimeType;
					}
				}
				break;
			}
			case Image.SourceType.Database:
				streamName = ImageValue;
				break;
			}
		}

		private void GetImageMimeType(out string mimeType)
		{
			if (m_transparent)
			{
				mimeType = "image/gif";
				return;
			}
			string streamName = null;
			GetImageInfo(out streamName, out mimeType);
			if (mimeType != null)
			{
				return;
			}
			if (streamName != null)
			{
				mimeType = m_renderingContext.GetChunkMimeType(streamName, Microsoft.ReportingServices.ReportProcessing.ReportProcessing.ReportChunkTypes.Image);
				return;
			}
			mimeType = null;
			if (GetUrlString() == null)
			{
				ImageData data = Data;
				if (data != null)
				{
					mimeType = data.MIMEType;
				}
			}
		}

		private string GetUrlString()
		{
			if (m_imageType != 0)
			{
				return null;
			}
			return ImageValue;
		}
	}
}
