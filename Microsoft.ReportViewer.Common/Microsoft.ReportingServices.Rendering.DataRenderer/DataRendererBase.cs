using System;
using System.Collections;
using System.Collections.Specialized;
using System.Diagnostics;
using Microsoft.ReportingServices.Common;
using Microsoft.ReportingServices.Diagnostics.Utilities;
using Microsoft.ReportingServices.Interfaces;
using Microsoft.ReportingServices.OnDemandReportRendering;

namespace Microsoft.ReportingServices.Rendering.DataRenderer;

abstract class DataRendererBase : IRenderingExtension, IExtension
{
	public abstract string LocalizedName { get; }

	public bool Render(Microsoft.ReportingServices.OnDemandReportRendering.Report report, NameValueCollection reportServerParameters, NameValueCollection deviceInfo, NameValueCollection clientCapabilities, ref Hashtable renderProperties, CreateAndRegisterStream createAndRegisterStream)
	{
		try
		{
			deviceInfo = ((deviceInfo == null) ? new NameValueCollection() : deviceInfo);
			reportServerParameters = ((reportServerParameters == null) ? new NameValueCollection() : reportServerParameters);
			clientCapabilities = ((clientCapabilities == null) ? new NameValueCollection() : clientCapabilities);
			InternalRender(report, reportServerParameters, deviceInfo, clientCapabilities, createAndRegisterStream);
			return false;
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

	public void GetRenderingResource(CreateAndRegisterStream createAndRegisterStreamCallback, NameValueCollection deviceInfo)
	{
	}

	public virtual void SetConfiguration(string configuration)
	{
	}

	protected abstract void InternalRender(Microsoft.ReportingServices.OnDemandReportRendering.Report report, NameValueCollection reportServerParameters, NameValueCollection deviceInfo, NameValueCollection clientCapabilities, CreateAndRegisterStream createAndRegisterStream);

	protected string ExtractStringFromDeviceInfo(NameValueCollection deviceInfo, string key)
	{
		string result = null;
		if (deviceInfo == null || string.IsNullOrEmpty(key))
		{
			return result;
		}
		try
		{
			result = deviceInfo[key];
			return result;
		}
		catch (Exception ex)
		{
			if (AsynchronousExceptionDetection.IsStoppingException(ex))
			{
				throw;
			}
			if (RSTrace.RenderingTracer.TraceError)
			{
				RSTrace.RenderingTracer.Trace(TraceLevel.Error, ex.ToString());
				return result;
			}
			return result;
		}
	}
}
