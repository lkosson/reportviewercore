using Microsoft.ReportingServices.OnDemandProcessing;
using Microsoft.ReportingServices.OnDemandReportRendering;
using Microsoft.ReportingServices.ReportProcessing;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.ReportIntermediateFormat
{
	internal sealed class EmbeddedImageDataHandler : ImageDataHandler
	{
		public override Microsoft.ReportingServices.OnDemandReportRendering.Image.SourceType Source => Microsoft.ReportingServices.OnDemandReportRendering.Image.SourceType.Embedded;

		protected override ProcessingErrorCode ErrorCodeForSourceType => ProcessingErrorCode.rsInvalidEmbeddedImageProperty;

		private OnDemandProcessingContext OdpContext => m_reportElement.RenderingContext.OdpContext;

		public EmbeddedImageDataHandler(ReportElement reportElement, IBaseImage image)
			: base(reportElement, image)
		{
		}

		protected override string CalculateImageProperties(out string mimeType, out byte[] imageData, out string imageDataId, out List<string> fieldsUsedInValue, out bool isNullImage)
		{
			isNullImage = false;
			bool errorOccured;
			string valueAsString = m_image.GetValueAsString(out fieldsUsedInValue, out errorOccured);
			if (errorOccured)
			{
				return GetErrorImageProperties(out mimeType, out imageData, out imageDataId);
			}
			if (string.IsNullOrEmpty(valueAsString))
			{
				isNullImage = true;
				return GetTransparentImageProperties(out mimeType, out imageData, out imageDataId);
			}
			imageDataId = valueAsString;
			if (!base.CacheManager.TryGetEmbeddedImage(valueAsString, m_image.EmbeddingMode, OdpContext, out imageData, out mimeType, out string streamName))
			{
				OdpContext.ErrorContext.Register(ProcessingErrorCode.rsInvalidEmbeddedImageProperty, Severity.Warning, m_image.ObjectType, m_image.ObjectName, m_image.ImageDataPropertyName, valueAsString);
				return GetErrorImageProperties(out mimeType, out imageData, out imageDataId);
			}
			return streamName;
		}

		protected override byte[] LoadExistingImageData(string imageDataId)
		{
			return base.CacheManager.GetCachedEmbeddedImageBytes(imageDataId, OdpContext);
		}
	}
}
