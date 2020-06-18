using Microsoft.ReportingServices.Common;
using Microsoft.ReportingServices.Interfaces;
using Microsoft.ReportingServices.OnDemandReportRendering;
using System;
using System.Collections;
using System.Collections.Specialized;

namespace Microsoft.ReportingServices.Rendering.ImageRenderer
{
	internal class RendererBase : IRenderingExtension, IExtension
	{
		public virtual string LocalizedName => null;

		public void GetRenderingResource(CreateAndRegisterStream createAndRegisterStreamCallback, NameValueCollection deviceInfo)
		{
		}

		protected virtual void ParseDeviceInfo(ref NameValueCollection deviceInfo)
		{
		}

		protected static bool ParseDeviceInfoBoolean(string boolValue, bool defaultValue)
		{
			if (bool.TryParse(boolValue, out bool result))
			{
				return result;
			}
			return defaultValue;
		}

		protected static int ParseDeviceInfoInt32(string intValue, int defaultValue)
		{
			if (int.TryParse(intValue, out int result))
			{
				return result;
			}
			return defaultValue;
		}

		protected virtual void Render(Microsoft.ReportingServices.OnDemandReportRendering.Report report, NameValueCollection deviceInfo, Hashtable renderProperties, CreateAndRegisterStream createAndRegisterStream)
		{
		}

		public bool Render(Microsoft.ReportingServices.OnDemandReportRendering.Report report, NameValueCollection reportServerParameters, NameValueCollection deviceInfo, NameValueCollection clientCapabilities, ref Hashtable renderProperties, CreateAndRegisterStream createAndRegisterStream)
		{
			try
			{
				ParseDeviceInfo(ref deviceInfo);
				Render(report, deviceInfo, renderProperties, createAndRegisterStream);
				return true;
			}
			catch (ReportRenderingException)
			{
				throw;
			}
			catch (Exception ex2)
			{
				if (AsynchronousExceptionDetection.IsStoppingException(ex2))
				{
					throw;
				}
				throw new ReportRenderingException(ex2, unexpected: true);
			}
		}

		public bool RenderStream(string streamName, Microsoft.ReportingServices.OnDemandReportRendering.Report report, NameValueCollection reportServerParameters, NameValueCollection deviceInfo, NameValueCollection clientCapabilities, ref Hashtable renderProperties, CreateAndRegisterStream createAndRegisterStream)
		{
			return false;
		}

		public void SetConfiguration(string configuration)
		{
		}
	}
}
