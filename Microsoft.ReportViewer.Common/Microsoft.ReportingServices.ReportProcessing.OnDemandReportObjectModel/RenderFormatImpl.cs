using Microsoft.ReportingServices.Diagnostics;
using Microsoft.ReportingServices.OnDemandProcessing;
using Microsoft.ReportingServices.ReportProcessing.ReportObjectModel;
using System.Collections.Specialized;

namespace Microsoft.ReportingServices.ReportProcessing.OnDemandReportObjectModel
{
	internal sealed class RenderFormatImpl : RenderFormatImplBase
	{
		private OnDemandProcessingContext m_odpContext;

		private string m_format;

		private bool m_isInteractiveFormat;

		private ReadOnlyNameValueCollection m_deviceInfo;

		private ReadOnlyNameValueCollection m_emptyDeviceInfo;

		internal const string InteractivityRenderFormat = "RPL";

		internal override string Name
		{
			get
			{
				SetRenderFormatUsed();
				if (IsRenderFormatAccessEnabled())
				{
					return m_format;
				}
				return null;
			}
		}

		internal override bool IsInteractive
		{
			get
			{
				if (m_odpContext.IsTablixProcessingMode)
				{
					return false;
				}
				SetRenderFormatUsed();
				return IsInteractiveFormat();
			}
		}

		internal override ReadOnlyNameValueCollection DeviceInfo
		{
			get
			{
				SetRenderFormatUsed();
				if (IsRenderFormatAccessEnabled())
				{
					return m_deviceInfo;
				}
				return m_emptyDeviceInfo;
			}
		}

		internal RenderFormatImpl(OnDemandProcessingContext odpContext)
		{
			m_odpContext = odpContext;
			m_emptyDeviceInfo = new ReadOnlyNameValueCollection(new NameValueCollection(0));
			if (m_odpContext.TopLevelContext.ReportContext.RSRequestParameters != null)
			{
				NameValueCollection renderingParameters = m_odpContext.TopLevelContext.ReportContext.RSRequestParameters.RenderingParameters;
				if (renderingParameters != null)
				{
					m_deviceInfo = new ReadOnlyNameValueCollection(renderingParameters);
				}
			}
			m_format = NormalizeRenderFormat(m_odpContext.TopLevelContext.ReportContext, out m_isInteractiveFormat);
			if (m_deviceInfo == null)
			{
				m_deviceInfo = m_emptyDeviceInfo;
			}
		}

		internal static string NormalizeRenderFormat(ICatalogItemContext reportContext, out bool isInteractiveFormat)
		{
			string text = null;
			if (reportContext.RSRequestParameters != null)
			{
				text = reportContext.RSRequestParameters.FormatParamValue;
			}
			if (text == null)
			{
				text = "RPL";
				isInteractiveFormat = true;
			}
			else if (IsRenderFormat(text, "RPL") || IsRenderFormat(text, "RGDI") || IsRenderFormat(text, "HTML4.0") || IsRenderFormat(text, "HTML5") || IsRenderFormat(text, "MHTML"))
			{
				isInteractiveFormat = true;
			}
			else
			{
				isInteractiveFormat = false;
			}
			return text;
		}

		private bool IsRenderFormatAccessEnabled()
		{
			if (m_odpContext.IsTablixProcessingMode)
			{
				return false;
			}
			if (IsInteractiveFormat() && !m_odpContext.IsUnrestrictedRenderFormatReferenceMode)
			{
				return false;
			}
			return true;
		}

		private bool IsInteractiveFormat()
		{
			return m_isInteractiveFormat;
		}

		internal static bool IsRenderFormat(string format, string targetFormat)
		{
			return ReportProcessing.CompareWithInvariantCulture(format, targetFormat, ignoreCase: true) == 0;
		}

		private void RegisterWarning(string propertyName)
		{
			if (m_odpContext.ReportRuntime.RuntimeErrorContext != null)
			{
				m_odpContext.ReportRuntime.RuntimeErrorContext.Register(ProcessingErrorCode.rsInvalidRenderFormatUsage, Severity.Warning, m_odpContext.ReportRuntime.ObjectType, m_odpContext.ReportRuntime.ObjectName, m_odpContext.ReportRuntime.PropertyName);
			}
		}

		private void SetRenderFormatUsed()
		{
			if (!m_odpContext.IsTablixProcessingMode)
			{
				m_odpContext.HasRenderFormatDependencyInDocumentMap = true;
			}
		}
	}
}
