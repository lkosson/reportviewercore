using Microsoft.ReportingServices.Common;
using Microsoft.ReportingServices.Interfaces;
using Microsoft.ReportingServices.OnDemandReportRendering;
using Microsoft.ReportingServices.Rendering.SPBProcessing;
using System;
using System.Collections;
using System.Collections.Specialized;

namespace Microsoft.ReportingServices.Rendering.WordRenderer
{
	internal abstract class WordDocumentRendererBase : IRenderingExtension, IExtension
	{
		public virtual string LocalizedName => WordRenderRes.WordLocalizedName;

		public WordDocumentRendererBase()
		{
		}

		internal abstract IWordWriter NewWordWriter();

		protected abstract WordRenderer NewWordRenderer(CreateAndRegisterStream createAndRegisterStream, DeviceInfo deviceInfoObj, Microsoft.ReportingServices.Rendering.SPBProcessing.SPBProcessing spbProcessing, IWordWriter writer, string reportName);

		public void GetRenderingResource(CreateAndRegisterStream createAndRegisterStreamCallback, NameValueCollection deviceInfo)
		{
		}

		public bool Render(Microsoft.ReportingServices.OnDemandReportRendering.Report report, NameValueCollection reportServerParameters, NameValueCollection deviceInfo, NameValueCollection clientCapabilities, ref Hashtable someProps, CreateAndRegisterStream createAndRegisterStream)
		{
			double pageHeight = double.MaxValue;
			using (Microsoft.ReportingServices.Rendering.SPBProcessing.SPBProcessing sPBProcessing = new Microsoft.ReportingServices.Rendering.SPBProcessing.SPBProcessing(report, createAndRegisterStream, pageHeight))
			{
				DeviceInfo deviceInfo2 = new DeviceInfo(deviceInfo);
				SPBContext sPBContext = new SPBContext();
				sPBContext.StartPage = 0;
				sPBContext.EndPage = 0;
				sPBContext.MeasureItems = false;
				sPBContext.AddSecondaryStreamNames = true;
				sPBContext.AddToggledItems = deviceInfo2.ExpandToggles;
				sPBContext.AddFirstPageHeaderFooter = true;
				sPBProcessing.SetContext(sPBContext);
				using (IWordWriter writer = NewWordWriter())
				{
					WordRenderer wordRenderer = NewWordRenderer(createAndRegisterStream, deviceInfo2, sPBProcessing, writer, report.Name);
					try
					{
						return wordRenderer.Render();
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
			}
		}

		public bool RenderStream(string streamName, Microsoft.ReportingServices.OnDemandReportRendering.Report report, NameValueCollection reportServerParameters, NameValueCollection deviceInfo, NameValueCollection clientCapabilities, ref Hashtable someProps, CreateAndRegisterStream createAndRegisterStream)
		{
			return false;
		}

		public void SetConfiguration(string configuration)
		{
		}
	}
}
