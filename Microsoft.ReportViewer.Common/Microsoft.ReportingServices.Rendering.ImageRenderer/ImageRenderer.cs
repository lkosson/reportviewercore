using Microsoft.ReportingServices.Interfaces;
using Microsoft.ReportingServices.OnDemandReportRendering;
using Microsoft.ReportingServices.Rendering.HPBProcessing;
using Microsoft.ReportingServices.Rendering.RPLProcessing;
using System.Collections;
using System.Collections.Specialized;
using System.Globalization;
using System.IO;

namespace Microsoft.ReportingServices.Rendering.ImageRenderer
{
	internal sealed class ImageRenderer : RendererBase
	{
		private const int DEFAULT_PRINT_DPI = 300;

		private int m_printDpiX = 300;

		private int m_printDpiY = 300;

		public override string LocalizedName => ImageRendererRes.IMAGELocalizedName;

		protected override void Render(Microsoft.ReportingServices.OnDemandReportRendering.Report report, NameValueCollection deviceInfo, Hashtable renderProperties, CreateAndRegisterStream createAndRegisterStream)
		{
			PaginationSettings paginationSettings = new PaginationSettings(report, deviceInfo);
			string text = paginationSettings.OutputFormat.ToString().ToUpperInvariant();
			bool flag = true;
			if (text == "TIFF")
			{
				text = "TIF";
				flag = false;
			}
			else if (text == "EMFPLUS")
			{
				text = "EMF";
			}
			if (text == "EMF")
			{
				paginationSettings.DynamicImageDpiX = 96;
				paginationSettings.DynamicImageDpiY = 96;
				paginationSettings.DpiX = m_printDpiX;
				paginationSettings.DpiY = m_printDpiY;
				ImageWriter.GetScreenDpi(out int dpiX, out int dpiY);
				paginationSettings.MeasureImageDpiX = dpiX;
				paginationSettings.MeasureImageDpiY = dpiY;
			}
			else
			{
				paginationSettings.MeasureImageDpiX = paginationSettings.DpiX;
				paginationSettings.MeasureImageDpiY = paginationSettings.DpiY;
			}
			paginationSettings.MeasureTextDpi = paginationSettings.DpiX;
			Stream stream;
			using (Microsoft.ReportingServices.Rendering.HPBProcessing.HPBProcessing hPBProcessing = new Microsoft.ReportingServices.Rendering.HPBProcessing.HPBProcessing(report, paginationSettings, createAndRegisterStream, ref renderProperties))
			{
				hPBProcessing.SetContext(hPBProcessing.PaginationSettings.StartPage, hPBProcessing.PaginationSettings.EndPage);
				using (Renderer renderer = new Renderer(physicalPagination: true))
				{
					stream = createAndRegisterStream(report.Name + (flag ? ("_" + hPBProcessing.PaginationSettings.StartPage.ToString(CultureInfo.InvariantCulture)) : ""), text, null, "image/" + text, !flag, StreamOper.CreateAndRegister);
					using (ImageWriter imageWriter = new ImageWriter(renderer, stream, disposeRenderer: false, createAndRegisterStream, paginationSettings.MeasureImageDpiX, paginationSettings.MeasureImageDpiY))
					{
						imageWriter.OutputFormat = hPBProcessing.PaginationSettings.OutputFormat;
						hPBProcessing.PaginationSettings.UseGenericDefault = !imageWriter.IsEmf;
						imageWriter.BeginReport(hPBProcessing.PaginationSettings.DpiX, hPBProcessing.PaginationSettings.DpiY);
						int num = hPBProcessing.PaginationSettings.StartPage;
						while (true)
						{
							hPBProcessing.GetNextPage(out RPLReport rplReport);
							if (rplReport == null)
							{
								break;
							}
							if (flag && num > hPBProcessing.PaginationSettings.StartPage)
							{
								stream = (imageWriter.OutputStream = createAndRegisterStream(report.Name + "_" + num, text, null, "image/" + text, !flag, StreamOper.CreateForPersistedStreams));
							}
							renderer.ProcessPage(rplReport, num, hPBProcessing.SharedFontCache, hPBProcessing.GlyphCache);
							rplReport.Release();
							rplReport = null;
							num++;
						}
						imageWriter.EndReport();
					}
				}
			}
			stream.Flush();
		}

		private void ValidatePrintDpiValue(ref int currValue, int defaultValue)
		{
			if (currValue <= 0)
			{
				currValue = defaultValue;
			}
		}

		protected override void ParseDeviceInfo(ref NameValueCollection deviceInfo)
		{
			if (deviceInfo == null)
			{
				return;
			}
			for (int i = 0; i < deviceInfo.Count; i++)
			{
				string text = deviceInfo.Keys[i];
				string intValue = deviceInfo[i];
				string a = text.ToUpper(CultureInfo.InvariantCulture);
				if (!(a == "PRINTDPIX"))
				{
					if (a == "PRINTDPIY")
					{
						m_printDpiY = RendererBase.ParseDeviceInfoInt32(intValue, 300);
						ValidatePrintDpiValue(ref m_printDpiY, 300);
					}
				}
				else
				{
					m_printDpiX = RendererBase.ParseDeviceInfoInt32(intValue, 300);
					ValidatePrintDpiValue(ref m_printDpiX, 300);
				}
			}
		}
	}
}
