using Microsoft.ReportingServices.Common;
using Microsoft.ReportingServices.Diagnostics;
using Microsoft.ReportingServices.Diagnostics.Internal;
using Microsoft.ReportingServices.Interfaces;
using Microsoft.ReportingServices.OnDemandReportRendering;
using Microsoft.ReportingServices.Rendering.ExcelRenderer.Excel;
using Microsoft.ReportingServices.Rendering.ExcelRenderer.Excel.BIFF8;
using Microsoft.ReportingServices.Rendering.ExcelRenderer.ExcelGenerator;
using Microsoft.ReportingServices.Rendering.ExcelRenderer.Layout;
using Microsoft.ReportingServices.Rendering.RPLProcessing;
using Microsoft.ReportingServices.Rendering.SPBProcessing;
using System;
using System.Collections;
using System.Collections.Specialized;
using System.IO;
using System.Reflection;
using System.Resources;
using System.Security.Permissions;
using System.Text;

namespace Microsoft.ReportingServices.Rendering.ExcelRenderer
{
	[StrongNameIdentityPermission(SecurityAction.InheritanceDemand, PublicKey = "0024000004800000940000000602000000240000525341310004000001000100272736ad6e5f9586bac2d531eabc3acc666c2f8ec879fa94f8f7b0327d2ff2ed523448f83c3d5c5dd2dfc7bc99c5286b2c125117bf5cbe242b9d41750732b2bdffe649c6efb8e5526d526fdd130095ecdb7bf210809c6cdad8824faa9ac0310ac3cba2aa0523567b2dfa7fe250b30facbd62d4ec99b94ac47c7d3b28f1f6e4c8")]
	[StrongNameIdentityPermission(SecurityAction.LinkDemand, PublicKey = "0024000004800000940000000602000000240000525341310004000001000100272736ad6e5f9586bac2d531eabc3acc666c2f8ec879fa94f8f7b0327d2ff2ed523448f83c3d5c5dd2dfc7bc99c5286b2c125117bf5cbe242b9d41750732b2bdffe649c6efb8e5526d526fdd130095ecdb7bf210809c6cdad8824faa9ac0310ac3cba2aa0523567b2dfa7fe250b30facbd62d4ec99b94ac47c7d3b28f1f6e4c8")]
	internal class ExcelRenderer : IRenderingExtension, IExtension
	{
		private static class RendererConstants
		{
			internal const string OMITDOCUMENTMAP = "OmitDocumentMap";

			internal const string OMITFORMULAS = "OmitFormulas";

			internal const string SIMPLEPAGEHEADERS = "SimplePageHeaders";

			internal const string SUPPRESSOUTLINES = "SuppressOutlines";
		}

		private bool m_omitFormula;

		private bool m_simplePageHeaders;

		private bool m_omitDocumentMap;

		private bool m_suppressOutlines;

		private bool m_addedDocMap;

		public virtual string LocalizedName => ExcelRenderRes.ExcelLocalizedName;

		private Stream CreateMemoryStream(string aName, string aExtension, Encoding aEncoding, string aMimeType, bool aWillSeek, StreamOper aOper)
		{
			return new MemoryStream();
		}

		protected virtual Stream CreateFinalOutputStream(string name, CreateAndRegisterStream createAndRegisterStream)
		{
			return createAndRegisterStream(name, "xls", null, "application/vnd.ms-excel", willSeek: false, StreamOper.CreateAndRegister);
		}

		internal virtual IExcelGenerator CreateExcelGenerator(ExcelGeneratorConstants.CreateTempStream createTempStream)
		{
			return new BIFF8Generator(createTempStream);
		}

		public void GetRenderingResource(CreateAndRegisterStream createAndRegisterStreamCallback, NameValueCollection deviceInfo)
		{
		}

		public bool Render(Microsoft.ReportingServices.OnDemandReportRendering.Report report, NameValueCollection reportServerParameters, NameValueCollection deviceInfo, NameValueCollection clientCapabilities, ref Hashtable renderProperties, CreateAndRegisterStream createAndRegisterStream)
		{
			try
			{
				ParseDeviceinfo(deviceInfo);
				Stream output = CreateFinalOutputStream(report.Name, createAndRegisterStream);
				MainEngine mainEngine = new MainEngine(createAndRegisterStream, this);
				if (report.HasDocumentMap && !m_omitDocumentMap)
				{
					m_addedDocMap = mainEngine.AddDocumentMap(report.DocumentMap);
					if (m_addedDocMap)
					{
						mainEngine.NextPage();
					}
				}
				using (Microsoft.ReportingServices.Rendering.SPBProcessing.SPBProcessing sPBProcessing = new Microsoft.ReportingServices.Rendering.SPBProcessing.SPBProcessing(report, createAndRegisterStream, double.MaxValue))
				{
					SPBContext sPBContext = new SPBContext();
					sPBContext.StartPage = 0;
					sPBContext.EndPage = 0;
					sPBContext.MeasureItems = false;
					sPBContext.AddSecondaryStreamNames = true;
					sPBContext.AddToggledItems = true;
					sPBContext.AddOriginalValue = true;
					sPBProcessing.SetContext(sPBContext);
					RPLReport rplReport = null;
					bool flag = true;
					while (!sPBProcessing.Done)
					{
						sPBProcessing.GetNextPage(out rplReport);
						if (rplReport == null)
						{
							continue;
						}
						if (flag)
						{
							flag = false;
							if (sPBProcessing.Done)
							{
								mainEngine.AdjustFirstWorksheetName(report.Name, m_addedDocMap);
							}
						}
						else
						{
							mainEngine.NextPage();
						}
						mainEngine.RenderRPLPage(rplReport, !m_simplePageHeaders, m_suppressOutlines);
						rplReport.Release();
						rplReport = null;
					}
					mainEngine.Save(output);
				}
				if (report.JobContext != null)
				{
					IJobContext jobContext = report.JobContext;
					lock (jobContext.SyncRoot)
					{
						if (jobContext.AdditionalInfo.ScalabilityTime == null)
						{
							jobContext.AdditionalInfo.ScalabilityTime = new ScaleTimeCategory();
						}
						jobContext.AdditionalInfo.ScalabilityTime.Rendering = mainEngine.TotalScaleTimeMs;
						if (jobContext.AdditionalInfo.EstimatedMemoryUsageKB == null)
						{
							jobContext.AdditionalInfo.EstimatedMemoryUsageKB = new EstimatedMemoryUsageKBCategory();
						}
						jobContext.AdditionalInfo.EstimatedMemoryUsageKB.Rendering = mainEngine.PeakMemoryUsageKB;
					}
				}
				mainEngine.Dispose();
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

		public void SetConfiguration(string configuration)
		{
		}

		private void ParseDeviceinfo(NameValueCollection deviceInfo)
		{
			if (deviceInfo != null)
			{
				string boolValue = deviceInfo["SuppressOutlines"];
				m_suppressOutlines = LayoutConvert.ParseBool(boolValue, defaultValue: false);
				string boolValue2 = deviceInfo["OmitDocumentMap"];
				m_omitDocumentMap = LayoutConvert.ParseBool(boolValue2, defaultValue: false);
				string boolValue3 = deviceInfo["OmitFormulas"];
				m_omitFormula = LayoutConvert.ParseBool(boolValue3, defaultValue: false);
				string boolValue4 = deviceInfo["SimplePageHeaders"];
				m_simplePageHeaders = LayoutConvert.ParseBool(boolValue4, defaultValue: false);
			}
		}
	}
}
