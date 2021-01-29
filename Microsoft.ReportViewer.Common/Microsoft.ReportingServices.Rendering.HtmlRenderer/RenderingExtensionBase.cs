using Microsoft.ReportingServices.Common;
using Microsoft.ReportingServices.Diagnostics;
using Microsoft.ReportingServices.Interfaces;
using Microsoft.ReportingServices.OnDemandReportRendering;
using System;
using System.Collections;
using System.Collections.Specialized;
using System.IO;
using System.Resources;
using System.Security.Permissions;

namespace Microsoft.ReportingServices.Rendering.HtmlRenderer
{
	[StrongNameIdentityPermission(SecurityAction.InheritanceDemand, PublicKey = "0024000004800000940000000602000000240000525341310004000001000100272736ad6e5f9586bac2d531eabc3acc666c2f8ec879fa94f8f7b0327d2ff2ed523448f83c3d5c5dd2dfc7bc99c5286b2c125117bf5cbe242b9d41750732b2bdffe649c6efb8e5526d526fdd130095ecdb7bf210809c6cdad8824faa9ac0310ac3cba2aa0523567b2dfa7fe250b30facbd62d4ec99b94ac47c7d3b28f1f6e4c8")]
	[StrongNameIdentityPermission(SecurityAction.LinkDemand, PublicKey = "0024000004800000940000000602000000240000525341310004000001000100272736ad6e5f9586bac2d531eabc3acc666c2f8ec879fa94f8f7b0327d2ff2ed523448f83c3d5c5dd2dfc7bc99c5286b2c125117bf5cbe242b9d41750732b2bdffe649c6efb8e5526d526fdd130095ecdb7bf210809c6cdad8824faa9ac0310ac3cba2aa0523567b2dfa7fe250b30facbd62d4ec99b94ac47c7d3b28f1f6e4c8")]
	abstract class RenderingExtensionBase : IRenderingExtension, IExtension, ITotalPages
	{
		protected const string m_htmlMimeType = "text/html";

		public abstract string LocalizedName
		{
			get;
		}

		public bool Render(Microsoft.ReportingServices.OnDemandReportRendering.Report report, NameValueCollection reportServerParameters, NameValueCollection deviceInfo, NameValueCollection clientCapabilities, ref Hashtable renderProperties, CreateAndRegisterStream createAndRegisterStream)
		{
			try
			{
				deviceInfo = ((deviceInfo == null) ? new NameValueCollection() : deviceInfo);
				reportServerParameters = ((reportServerParameters == null) ? new NameValueCollection() : reportServerParameters);
				clientCapabilities = ((clientCapabilities == null) ? new NameValueCollection() : clientCapabilities);
				return InternalRender(report, reportServerParameters, deviceInfo, clientCapabilities, ref renderProperties, createAndRegisterStream);
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
			try
			{
				deviceInfo = ((deviceInfo == null) ? new NameValueCollection() : deviceInfo);
				reportServerParameters = ((reportServerParameters == null) ? new NameValueCollection() : reportServerParameters);
				clientCapabilities = ((clientCapabilities == null) ? new NameValueCollection() : clientCapabilities);
				return InternalRenderStream(streamName, report, reportServerParameters, deviceInfo, clientCapabilities, ref renderProperties, createAndRegisterStream);
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

		public void GetRenderingResource(CreateAndRegisterStream createAndRegisterStreamCallback, NameValueCollection deviceInfo)
		{
			try
			{
				ROMReport.GetRenderingResource(createAndRegisterStreamCallback, deviceInfo["GetImage"]);
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

		internal static void LoadImageResource(string resourceName, ResourceManager resourceManager, CreateAndRegisterStream createAndRegisterStreamCallback)
		{
			Stream stream = createAndRegisterStreamCallback(resourceName, "gif", null, "image/gif", willSeek: false, StreamOper.CreateAndRegister);
			System.Drawing.Image image = (System.Drawing.Image)resourceManager.GetObject(resourceName);
			image.Save(stream, image.RawFormat);
		}

		public virtual void SetConfiguration(string configuration)
		{
		}

		protected abstract bool InternalRender(Microsoft.ReportingServices.OnDemandReportRendering.Report report, NameValueCollection reportServerParameters, NameValueCollection deviceInfo, NameValueCollection clientCapabilities, ref Hashtable renderProperties, CreateAndRegisterStream createAndRegisterStream);

		protected abstract bool InternalRenderStream(string streamName, Microsoft.ReportingServices.OnDemandReportRendering.Report report, NameValueCollection reportServerParameters, NameValueCollection deviceInfo, NameValueCollection clientCapabilities, ref Hashtable renderProperties, CreateAndRegisterStream createAndRegisterStream);
	}
}
