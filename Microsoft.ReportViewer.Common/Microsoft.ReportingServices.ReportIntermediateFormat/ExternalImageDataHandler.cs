using Microsoft.ReportingServices.OnDemandReportRendering;
using Microsoft.ReportingServices.ReportProcessing;
using Microsoft.ReportingServices.ReportPublishing;
using System;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.ReportIntermediateFormat
{
	internal sealed class ExternalImageDataHandler : ImageDataHandler
	{
		public override Microsoft.ReportingServices.OnDemandReportRendering.Image.SourceType Source => Microsoft.ReportingServices.OnDemandReportRendering.Image.SourceType.External;

		protected override ProcessingErrorCode ErrorCodeForSourceType => ProcessingErrorCode.rsInvalidExternalImageProperty;

		public ExternalImageDataHandler(ReportElement reportElement, IBaseImage image)
			: base(reportElement, image)
		{
		}

		protected override string GetImageDataId()
		{
			List<string> fieldsUsedInValue;
			bool errorOccured;
			string text = m_image.GetValueAsString(out fieldsUsedInValue, out errorOccured);
			if (errorOccured || string.IsNullOrEmpty(text))
			{
				text = null;
			}
			return text;
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
			Microsoft.ReportingServices.OnDemandReportRendering.RenderingContext renderingContext = m_reportElement.RenderingContext;
			string streamName = null;
			imageData = null;
			imageDataId = valueAsString;
			if (base.CacheManager.TryGetExternalImage(valueAsString, out imageData, out mimeType, out streamName, out bool wasError))
			{
				if (wasError)
				{
					imageDataId = null;
				}
			}
			else if (!GetExternalImage(renderingContext, valueAsString, out imageData, out mimeType) || imageData == null)
			{
				renderingContext.OdpContext.ErrorContext.Register(ProcessingErrorCode.rsInvalidExternalImageProperty, Severity.Warning, m_image.ObjectType, m_image.ObjectName, m_image.ImageDataPropertyName, streamName);
				base.CacheManager.AddFailedExternalImage(valueAsString);
				streamName = null;
				imageDataId = null;
			}
			else
			{
				streamName = base.CacheManager.AddExternalImage(valueAsString, imageData, mimeType);
			}
			return streamName;
		}

		private bool GetExternalImage(Microsoft.ReportingServices.OnDemandReportRendering.RenderingContext renderingContext, string path, out byte[] imageData, out string mimeType)
		{
			imageData = null;
			mimeType = null;
			try
			{
				if (!renderingContext.OdpContext.TopLevelContext.ReportContext.IsSupportedProtocol(path, protocolRestriction: true))
				{
					renderingContext.OdpContext.ErrorContext.Register(ProcessingErrorCode.rsUnsupportedProtocol, Severity.Error, m_image.ObjectType, m_image.ObjectName, m_image.ImageDataPropertyName, path, "http://, https://, ftp://, file:, mailto:, or news:");
				}
				else
				{
					renderingContext.OdpContext.GetResource(path, out imageData, out mimeType, out bool registerInvalidSizeWarning);
					if (imageData != null && !Microsoft.ReportingServices.ReportPublishing.Validator.ValidateMimeType(mimeType))
					{
						renderingContext.OdpContext.ErrorContext.Register(ProcessingErrorCode.rsInvalidMIMEType, Severity.Warning, m_image.ObjectType, m_image.ObjectName, "MIMEType", mimeType);
						mimeType = null;
						imageData = null;
					}
					if (registerInvalidSizeWarning)
					{
						renderingContext.OdpContext.ErrorContext.Register(ProcessingErrorCode.rsSandboxingExternalResourceExceedsMaximumSize, Severity.Warning, m_image.ObjectType, m_image.ObjectName, m_image.ImageDataPropertyName);
					}
				}
			}
			catch (Exception ex)
			{
				renderingContext.OdpContext.ErrorContext.Register(ProcessingErrorCode.rsInvalidImageReference, Severity.Warning, m_image.ObjectType, m_image.ObjectName, m_image.ImageDataPropertyName, ex.Message);
				return false;
			}
			return true;
		}

		protected override byte[] LoadExistingImageData(string imageDataId)
		{
			return base.CacheManager.GetCachedExternalImageBytes(imageDataId);
		}
	}
}
