using Microsoft.ReportingServices.OnDemandReportRendering;
using Microsoft.ReportingServices.ReportIntermediateFormat;
using Microsoft.ReportingServices.ReportProcessing;
using System;

namespace Microsoft.ReportingServices.OnDemandProcessing
{
	internal sealed class StreamingImageCacheManager : ImageCacheManager
	{
		internal StreamingImageCacheManager(OnDemandMetadata odpMetadata, IChunkFactory chunkFactory)
			: base(odpMetadata, chunkFactory)
		{
		}

		protected override bool ExtractCachedExternalImagePropertiesIfValid(Microsoft.ReportingServices.ReportIntermediateFormat.ImageInfo imageInfo, out byte[] imageData, out string mimeType, out string streamName)
		{
			imageData = imageInfo.GetCachedImageData();
			if (imageData != null)
			{
				streamName = imageInfo.StreamName;
				mimeType = imageInfo.MimeType;
				return true;
			}
			streamName = null;
			mimeType = null;
			return false;
		}

		public override string AddExternalImage(string value, byte[] imageData, string mimeType)
		{
			Global.Tracer.Assert(imageData != null, "Missing imageData for external image");
			string text = ImageHelper.GenerateImageStreamName();
			if (!m_odpMetadata.TryGetExternalImage(value, out Microsoft.ReportingServices.ReportIntermediateFormat.ImageInfo imageInfo))
			{
				imageInfo = new Microsoft.ReportingServices.ReportIntermediateFormat.ImageInfo(text, mimeType);
				m_odpMetadata.AddExternalImage(value, imageInfo);
			}
			imageInfo.SetCachedImageData(imageData);
			return text;
		}

		public override byte[] GetCachedExternalImageBytes(string value)
		{
			Global.Tracer.Assert(condition: false, "Chunks are not supported in this processing mode.");
			throw new InvalidOperationException("Chunks are not supported in this processing mode.");
		}

		public override bool TryGetEmbeddedImage(string value, Microsoft.ReportingServices.OnDemandReportRendering.Image.EmbeddingModes embeddingMode, OnDemandProcessingContext odpContext, out byte[] imageData, out string mimeType, out string streamName)
		{
			if (embeddingMode == Microsoft.ReportingServices.OnDemandReportRendering.Image.EmbeddingModes.Package)
			{
				imageData = null;
				mimeType = null;
				streamName = null;
				return true;
			}
			Global.Tracer.Assert(condition: false, "Embedded Images are not supported in Streaming ODP mode.");
			throw new InvalidOperationException("Embedded Images are not supported in Streaming ODP mode.");
		}

		public override byte[] GetCachedEmbeddedImageBytes(string imageName, OnDemandProcessingContext odpContext)
		{
			Global.Tracer.Assert(condition: false, "Chunks are not supported in this processing mode.");
			throw new InvalidOperationException("Chunks are not supported in this processing mode.");
		}

		public override bool TryGetDatabaseImage(string uniqueName, out string streamName)
		{
			streamName = null;
			return false;
		}

		public override string AddDatabaseImage(string uniqueName, byte[] imageData, string mimeType, OnDemandProcessingContext odpContext)
		{
			return ImageHelper.GenerateImageStreamName();
		}

		public override byte[] GetCachedDatabaseImageBytes(string chunkName)
		{
			Global.Tracer.Assert(condition: false, "Chunks are not supported in this processing mode.");
			throw new InvalidOperationException("Chunks are not supported in this processing mode.");
		}

		public override string EnsureTransparentImageIsCached(string mimeType, byte[] imageData)
		{
			return ImageHelper.GenerateImageStreamName();
		}
	}
}
