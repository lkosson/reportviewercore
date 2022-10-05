using Microsoft.ReportingServices.Diagnostics.Internal;
using Microsoft.ReportingServices.Diagnostics;
using Microsoft.ReportingServices.Interfaces;
using Microsoft.ReportingServices.OnDemandReportRendering;
using Microsoft.ReportingServices.Rendering.ExcelRenderer;
using Microsoft.ReportingServices.Rendering.RPLProcessing;
using Microsoft.ReportingServices.Rendering.SPBProcessing;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Xml.Linq;
using Microsoft.ReportingServices.Rendering.HPBProcessing;

namespace Extensions.ITextSharpRendering
{

	internal class ITextSharpRenderer : IRenderingExtension, IExtension
	{
		public string LocalizedName => "iTextSharp PDF";

		public void GetRenderingResource(CreateAndRegisterStream createAndRegisterStreamCallback, NameValueCollection deviceInfo)
		{
		}
		/*
		public bool Render(Microsoft.ReportingServices.OnDemandReportRendering.Report report, NameValueCollection reportServerParameters, NameValueCollection deviceInfo, NameValueCollection clientCapabilities, ref Hashtable renderProperties, CreateAndRegisterStream createAndRegisterStream)
		{
			var stream = createAndRegisterStream(report.Name, "pdf", null, "application/pdf", false, StreamOper.CreateAndRegister);
			using var sPBProcessing = new SPBProcessing(report, createAndRegisterStream, Double.MaxValue);
			var spbContext = new SPBContext();
			spbContext.StartPage = 0;
			spbContext.EndPage = 0;
			spbContext.MeasureItems = false;
			spbContext.AddSecondaryStreamNames = true;
			spbContext.AddToggledItems = true;
			spbContext.AddOriginalValue = true;
			sPBProcessing.SetContext(spbContext);
			while (!sPBProcessing.Done)
			{
				sPBProcessing.GetNextPage(out var rplReport);
				if (rplReport == null) continue;
				foreach (var page in rplReport.RPLPaginatedPages)
				{
					var rplSection = page.GetNextReportSection();
					while (rplSection != null)
					{
						foreach (var rplColumn in rplSection.Columns)
						{
							ProcessItem(rplColumn);
						}
						rplSection = page.GetNextReportSection();
					}
				}
				rplReport.Release();
			}
			return false;
		}

		private void ProcessItem(RPLItemMeasurement rplItem)
		{
			if (rplItem.Element is RPLContainer rplContainer)
			{
				foreach (var rplChild in rplContainer.Children)
				{
					ProcessItem(rplChild);
				}
			}
			else if (rplItem.Element is RPLTextBox rplTextBox)
			{
			}
			else if (rplItem.Element is RPLLine rplLine)
			{
			}
			else if (rplItem.Element is RPLSubReport rplSubReport)
			{
			}
			else if (rplItem.Element is RPLImage rplImage)
			{
			}
			else if (rplItem.Element is RPLChart rplChart || rplItem.Element is RPLGaugePanel rplGaugePanel || rplItem.Element is RPLMap rplMap)
			{
			}
			else if (rplItem.Element is RPLTablix rplTablix)
			{
				rplTablix.GetNextRow();
			}
		}
		*/

		public bool Render(Microsoft.ReportingServices.OnDemandReportRendering.Report report, NameValueCollection reportServerParameters, NameValueCollection deviceInfo, NameValueCollection clientCapabilities, ref Hashtable renderProperties, CreateAndRegisterStream createAndRegisterStream)
		{
			var stream = createAndRegisterStream(report.Name, "pdf", null, "application/pdf", false, StreamOper.CreateAndRegister);
			using var renderer = new PDFRenderer();
			using var writer = new PDFWriter(renderer, stream, false, createAndRegisterStream);
			using var hpbProcessing = new HPBProcessing(report, deviceInfo, createAndRegisterStream, ref renderProperties);
			hpbProcessing.SetContext(hpbProcessing.PaginationSettings.StartPage, hpbProcessing.PaginationSettings.EndPage);
			writer.BeginReport(hpbProcessing.PaginationSettings.DpiX, hpbProcessing.PaginationSettings.DpiY);
			int num = 0;
			while (true)
			{
				hpbProcessing.GetNextPage(out RPLReport rplReport);
				if (rplReport == null) break;
				renderer.ProcessPage(rplReport, num, null, null);
				rplReport.Release();
				rplReport = null;
				num++;
			}
			writer.EndReport();
			return false;
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
