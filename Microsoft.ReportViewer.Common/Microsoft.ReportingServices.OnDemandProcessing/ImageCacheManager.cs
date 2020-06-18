using Microsoft.ReportingServices.OnDemandReportRendering;
using Microsoft.ReportingServices.ReportIntermediateFormat;
using Microsoft.ReportingServices.ReportProcessing;

namespace Microsoft.ReportingServices.OnDemandProcessing
{
	internal abstract class ImageCacheManager
	{
		protected readonly OnDemandMetadata m_odpMetadata;

		protected readonly IChunkFactory m_chunkFactory;

		public ImageCacheManager(OnDemandMetadata odpMetadata, IChunkFactory chunkFactory)
		{
			m_odpMetadata = odpMetadata;
			m_chunkFactory = chunkFactory;
		}

		public bool TryGetExternalImage(string value, out byte[] imageData, out string mimeType, out string streamName, out bool wasError)
		{
			imageData = null;
			mimeType = null;
			streamName = null;
			if (m_odpMetadata.TryGetExternalImage(value, out Microsoft.ReportingServices.ReportIntermediateFormat.ImageInfo imageInfo))
			{
				if (imageInfo.ErrorOccurred)
				{
					wasError = true;
					return true;
				}
				wasError = false;
				return ExtractCachedExternalImagePropertiesIfValid(imageInfo, out imageData, out mimeType, out streamName);
			}
			wasError = false;
			return false;
		}

		protected abstract bool ExtractCachedExternalImagePropertiesIfValid(Microsoft.ReportingServices.ReportIntermediateFormat.ImageInfo imageInfo, out byte[] imageData, out string mimeType, out string streamName);

		public abstract string AddExternalImage(string value, byte[] imageData, string mimeType);

		public abstract byte[] GetCachedExternalImageBytes(string value);

		public void AddFailedExternalImage(string value)
		{
			Microsoft.ReportingServices.ReportIntermediateFormat.ImageInfo imageInfo = new Microsoft.ReportingServices.ReportIntermediateFormat.ImageInfo(null, null);
			imageInfo.ErrorOccurred = true;
			m_odpMetadata.AddExternalImage(value, imageInfo);
		}

		public abstract bool TryGetEmbeddedImage(string value, Microsoft.ReportingServices.OnDemandReportRendering.Image.EmbeddingModes embeddingMode, OnDemandProcessingContext odpContext, out byte[] imageData, out string mimeType, out string streamName);

		public abstract byte[] GetCachedEmbeddedImageBytes(string imageName, OnDemandProcessingContext odpContext);

		public abstract bool TryGetDatabaseImage(string uniqueName, out string streamName);

		public abstract string AddDatabaseImage(string uniqueName, byte[] imageData, string mimeType, OnDemandProcessingContext odpContext);

		public abstract byte[] GetCachedDatabaseImageBytes(string chunkName);

		public abstract string EnsureTransparentImageIsCached(string mimeType, byte[] imageData);
	}
}
