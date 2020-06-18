using Microsoft.ReportingServices.OnDemandReportRendering;
using Microsoft.ReportingServices.ReportProcessing;
using Microsoft.ReportingServices.ReportPublishing;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.ReportIntermediateFormat
{
	internal sealed class DatabaseImageDataHandler : ImageDataHandler
	{
		public override Microsoft.ReportingServices.OnDemandReportRendering.Image.SourceType Source => Microsoft.ReportingServices.OnDemandReportRendering.Image.SourceType.Database;

		protected override ProcessingErrorCode ErrorCodeForSourceType => ProcessingErrorCode.rsInvalidDatabaseImageProperty;

		public DatabaseImageDataHandler(ReportElement reportElement, IBaseImage image)
			: base(reportElement, image)
		{
		}

		protected override string CalculateImageProperties(out string mimeType, out byte[] imageData, out string imageDataId, out List<string> fieldsUsedInValue, out bool isNullImage)
		{
			fieldsUsedInValue = null;
			isNullImage = false;
			mimeType = m_image.GetMIMETypeValue();
			mimeType = Microsoft.ReportingServices.ReportPublishing.ProcessingValidator.ValidateMimeType(mimeType, m_image.ObjectType, m_image.ObjectName, m_image.MIMETypePropertyName, m_reportElement.RenderingContext.OdpContext.ErrorContext);
			if (mimeType == null)
			{
				return GetErrorImageProperties(out mimeType, out imageData, out imageDataId);
			}
			string instanceUniqueName = m_reportElement.InstanceUniqueName;
			if (base.CacheManager.TryGetDatabaseImage(instanceUniqueName, out string streamName))
			{
				imageData = null;
				imageDataId = streamName;
				return streamName;
			}
			imageData = m_image.GetImageData(out fieldsUsedInValue, out bool errorOccurred);
			if (errorOccurred)
			{
				m_reportElement.RenderingContext.OdpContext.ErrorContext.Register(ErrorCodeForSourceType, Severity.Warning, m_image.ObjectType, m_image.ObjectName, m_image.ImageDataPropertyName, m_image.Value.ExpressionString);
				return GetErrorImageProperties(out mimeType, out imageData, out imageDataId);
			}
			if (imageData == null || imageData.Length == 0)
			{
				isNullImage = true;
				return GetTransparentImageProperties(out mimeType, out imageData, out imageDataId);
			}
			return imageDataId = base.CacheManager.AddDatabaseImage(instanceUniqueName, imageData, mimeType, m_reportElement.RenderingContext.OdpContext);
		}

		protected override byte[] LoadExistingImageData(string imageDataId)
		{
			return base.CacheManager.GetCachedDatabaseImageBytes(imageDataId);
		}
	}
}
