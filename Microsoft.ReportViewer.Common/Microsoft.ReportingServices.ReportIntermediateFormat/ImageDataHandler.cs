using Microsoft.ReportingServices.OnDemandProcessing;
using Microsoft.ReportingServices.OnDemandReportRendering;
using Microsoft.ReportingServices.ReportProcessing;
using System;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.ReportIntermediateFormat
{
	internal abstract class ImageDataHandler
	{
		protected readonly IBaseImage m_image;

		protected readonly ReportElement m_reportElement;

		private string m_mimeType;

		private byte[] m_imageData;

		private string m_imageDataId;

		private string m_streamName;

		private List<string> m_fieldsUsedInValue;

		private bool m_isNullImage;

		private bool m_isCachePopulated;

		private static readonly byte[] TransparentImageBytes = new byte[43]
		{
			71,
			73,
			70,
			56,
			57,
			97,
			1,
			0,
			1,
			0,
			240,
			0,
			0,
			219,
			223,
			239,
			0,
			0,
			0,
			33,
			249,
			4,
			1,
			0,
			0,
			0,
			0,
			44,
			0,
			0,
			0,
			0,
			1,
			0,
			1,
			0,
			0,
			2,
			2,
			68,
			1,
			0,
			59
		};

		private static readonly string TransparentImageMimeType = "image/gif";

		public abstract Microsoft.ReportingServices.OnDemandReportRendering.Image.SourceType Source
		{
			get;
		}

		public string MIMEType
		{
			get
			{
				EnsureCacheIsPopulated();
				return m_mimeType;
			}
		}

		public byte[] ImageData
		{
			get
			{
				EnsureCacheIsPopulated();
				if (m_imageData == null && m_imageDataId != null)
				{
					m_imageData = LoadExistingImageData(m_imageDataId);
					if (m_imageData == null)
					{
						m_reportElement.RenderingContext.OdpContext.ErrorContext.Register(ErrorCodeForSourceType, Severity.Warning, m_image.ObjectType, m_image.ObjectName, m_image.ImageDataPropertyName, m_imageDataId);
					}
					m_imageDataId = null;
				}
				return m_imageData;
			}
		}

		public string ImageDataId
		{
			get
			{
				if (m_imageDataId == null)
				{
					m_imageDataId = GetImageDataId();
				}
				return m_imageDataId;
			}
		}

		public string StreamName
		{
			get
			{
				EnsureCacheIsPopulated();
				return m_streamName;
			}
		}

		public List<string> FieldsUsedInValue
		{
			get
			{
				EnsureCacheIsPopulated();
				return m_fieldsUsedInValue;
			}
		}

		public bool IsNullImage
		{
			get
			{
				if (GetIsNullImage())
				{
					return true;
				}
				EnsureCacheIsPopulated();
				return m_isNullImage;
			}
		}

		protected abstract ProcessingErrorCode ErrorCodeForSourceType
		{
			get;
		}

		protected ImageCacheManager CacheManager => m_reportElement.RenderingContext.OdpContext.ImageCacheManager;

		public ImageDataHandler(ReportElement reportElement, IBaseImage image)
		{
			m_reportElement = reportElement;
			m_image = image;
		}

		private bool GetIsNullImage()
		{
			if (string.IsNullOrEmpty(m_image.Value.ExpressionString))
			{
				return true;
			}
			return false;
		}

		public void ClearCache()
		{
			m_mimeType = null;
			m_imageData = null;
			m_imageDataId = null;
			m_streamName = null;
			m_fieldsUsedInValue = null;
			m_isNullImage = false;
			m_isCachePopulated = false;
		}

		private void EnsureCacheIsPopulated()
		{
			if (!m_isCachePopulated)
			{
				m_isCachePopulated = true;
				m_streamName = GetCalculatedImageProperties(out m_mimeType, out m_imageData, out m_imageDataId, out m_fieldsUsedInValue, out m_isNullImage);
			}
		}

		private string GetCalculatedImageProperties(out string mimeType, out byte[] imageData, out string imageDataId, out List<string> fieldsUsedInValue, out bool isNullImage)
		{
			if (GetIsNullImage())
			{
				fieldsUsedInValue = null;
				imageDataId = null;
				isNullImage = true;
				return m_image.GetTransparentImageProperties(out mimeType, out imageData);
			}
			return CalculateImageProperties(out mimeType, out imageData, out imageDataId, out fieldsUsedInValue, out isNullImage);
		}

		protected abstract string CalculateImageProperties(out string mimeType, out byte[] imageData, out string imageDataId, out List<string> fieldsUsedInValue, out bool isNullImage);

		protected virtual string GetImageDataId()
		{
			EnsureCacheIsPopulated();
			return m_imageDataId;
		}

		protected abstract byte[] LoadExistingImageData(string imageDataId);

		protected string GetTransparentImageProperties(out string mimeType, out byte[] imageData, out string imageDataId)
		{
			imageDataId = null;
			return m_image.GetTransparentImageProperties(out mimeType, out imageData);
		}

		protected string GetErrorImageProperties(out string mimeType, out byte[] imageData, out string imageDataId)
		{
			mimeType = null;
			imageData = null;
			imageDataId = null;
			return null;
		}

		public string LoadAndCacheTransparentImage(out string mimeType, out byte[] imageData)
		{
			imageData = new byte[TransparentImageBytes.Length];
			Array.Copy(TransparentImageBytes, imageData, imageData.Length);
			mimeType = TransparentImageMimeType;
			return CacheManager.EnsureTransparentImageIsCached(mimeType, imageData);
		}
	}
}
