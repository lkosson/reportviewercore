using Microsoft.ReportingServices.Diagnostics;
using Microsoft.ReportingServices.OnDemandReportRendering;
using Microsoft.ReportingServices.ReportIntermediateFormat;
using Microsoft.ReportingServices.ReportProcessing;
using System.Collections.Generic;
using System.IO;

namespace Microsoft.ReportingServices.OnDemandProcessing
{
	internal sealed class SnapshotImageCacheManager : ImageCacheManager
	{
		public SnapshotImageCacheManager(OnDemandMetadata odpMetadata, IChunkFactory chunkFactory)
			: base(odpMetadata, chunkFactory)
		{
		}

		protected override bool ExtractCachedExternalImagePropertiesIfValid(Microsoft.ReportingServices.ReportIntermediateFormat.ImageInfo imageInfo, out byte[] imageData, out string mimeType, out string streamName)
		{
			imageData = imageInfo.GetCachedImageData();
			streamName = imageInfo.StreamName;
			mimeType = imageInfo.MimeType;
			return true;
		}

		public override string AddExternalImage(string value, byte[] imageData, string mimeType)
		{
			Global.Tracer.Assert(imageData != null, "Missing imageData for external image");
			string text = ImageHelper.StoreImageDataInChunk(Microsoft.ReportingServices.ReportProcessing.ReportProcessing.ReportChunkTypes.Image, imageData, mimeType, m_odpMetadata, m_chunkFactory);
			Microsoft.ReportingServices.ReportIntermediateFormat.ImageInfo imageInfo = new Microsoft.ReportingServices.ReportIntermediateFormat.ImageInfo(text, mimeType);
			imageInfo.SetCachedImageData(imageData);
			m_odpMetadata.AddExternalImage(value, imageInfo);
			return text;
		}

		public override byte[] GetCachedExternalImageBytes(string value)
		{
			Microsoft.ReportingServices.ReportIntermediateFormat.ImageInfo imageInfo;
			bool condition = m_odpMetadata.TryGetExternalImage(value, out imageInfo);
			Global.Tracer.Assert(condition, "Missing ImageInfo for external image");
			byte[] array = ReadImageDataFromChunk(imageInfo.StreamName, Microsoft.ReportingServices.ReportProcessing.ReportProcessing.ReportChunkTypes.Image);
			if (array != null)
			{
				imageInfo.SetCachedImageData(array);
			}
			return array;
		}

		public override bool TryGetEmbeddedImage(string value, Microsoft.ReportingServices.OnDemandReportRendering.Image.EmbeddingModes embeddingMode, OnDemandProcessingContext odpContext, out byte[] imageData, out string mimeType, out string streamName)
		{
			Global.Tracer.Assert(embeddingMode == Microsoft.ReportingServices.OnDemandReportRendering.Image.EmbeddingModes.Inline, "Invalid image embedding mode");
			Microsoft.ReportingServices.ReportIntermediateFormat.ImageInfo value2 = null;
			Dictionary<string, Microsoft.ReportingServices.ReportIntermediateFormat.ImageInfo> embeddedImages = odpContext.EmbeddedImages;
			if (embeddedImages == null || !embeddedImages.TryGetValue(value, out value2))
			{
				imageData = null;
				mimeType = null;
				streamName = null;
				return false;
			}
			imageData = value2.GetCachedImageData();
			streamName = value2.StreamName;
			mimeType = value2.MimeType;
			return true;
		}

		public override byte[] GetCachedEmbeddedImageBytes(string imageName, OnDemandProcessingContext odpContext)
		{
			Microsoft.ReportingServices.ReportIntermediateFormat.ImageInfo value;
			bool condition = odpContext.EmbeddedImages.TryGetValue(imageName, out value);
			Global.Tracer.Assert(condition, "Missing ImageInfo for embedded image");
			byte[] array = ReadImageDataFromChunk(value.StreamName, Microsoft.ReportingServices.ReportProcessing.ReportProcessing.ReportChunkTypes.StaticImage);
			if (array != null)
			{
				value.SetCachedImageData(array);
			}
			return array;
		}

		public override bool TryGetDatabaseImage(string uniqueName, out string streamName)
		{
			return m_odpMetadata.ReportSnapshot.TryGetImageChunkName(uniqueName, out streamName);
		}

		public override string AddDatabaseImage(string uniqueName, byte[] imageData, string mimeType, OnDemandProcessingContext odpContext)
		{
			if (odpContext.IsPageHeaderFooter)
			{
				return null;
			}
			string text = ImageHelper.StoreImageDataInChunk(Microsoft.ReportingServices.ReportProcessing.ReportProcessing.ReportChunkTypes.Image, imageData, mimeType, m_odpMetadata, m_chunkFactory);
			m_odpMetadata.ReportSnapshot.AddImageChunkName(uniqueName, text);
			return text;
		}

		public override byte[] GetCachedDatabaseImageBytes(string chunkName)
		{
			return ReadImageDataFromChunk(chunkName, Microsoft.ReportingServices.ReportProcessing.ReportProcessing.ReportChunkTypes.Image);
		}

		public override string EnsureTransparentImageIsCached(string mimeType, byte[] imageData)
		{
			string text = m_odpMetadata.TransparentImageChunkName;
			if (text == null)
			{
				text = ImageHelper.StoreImageDataInChunk(Microsoft.ReportingServices.ReportProcessing.ReportProcessing.ReportChunkTypes.Image, imageData, mimeType, m_odpMetadata, m_chunkFactory);
				m_odpMetadata.TransparentImageChunkName = text;
			}
			return text;
		}

		private byte[] ReadImageDataFromChunk(string chunkName, Microsoft.ReportingServices.ReportProcessing.ReportProcessing.ReportChunkTypes chunkType)
		{
			byte[] array = null;
			string mimeType;
			Stream chunk = m_chunkFactory.GetChunk(chunkName, chunkType, ChunkMode.Open, out mimeType);
			Global.Tracer.Assert(chunk != null, "Could not find expected image data chunk.  Name='{0}', Type={1}", chunkName, chunkType);
			using (chunk)
			{
				return StreamSupport.ReadToEndUsingLength(chunk);
			}
		}
	}
}
