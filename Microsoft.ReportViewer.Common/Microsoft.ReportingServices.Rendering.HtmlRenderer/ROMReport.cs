using Microsoft.ReportingServices.Interfaces;
using Microsoft.ReportingServices.OnDemandReportRendering;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Microsoft.ReportingServices.Rendering.HtmlRenderer
{
	internal class ROMReport : IReportWrapper
	{
		private const string PrintCabFileName = "RSClientPrint.cab";

		private static string m_productVersion;

		private static Encoding m_encoding;

		private static Dictionary<string, byte[]> m_imageMap;

		private Microsoft.ReportingServices.OnDemandReportRendering.Report m_report;

		public bool HasBookmarks => m_report.HasBookmarks;

		public string SortItem => m_report.SortItem;

		public string ShowHideToggle => m_report.ShowHideToggle;

		public Microsoft.ReportingServices.OnDemandReportRendering.Report Report => m_report;

		internal ROMReport(Microsoft.ReportingServices.OnDemandReportRendering.Report aReport)
		{
			m_report = aReport;
		}

		public string GetStreamUrl(bool useSessionId, string streamName)
		{
			return m_report.GetStreamUrl(useSessionId, streamName);
		}

		public string GetReportUrl(bool addParams)
		{
			return m_report.GetReportUrl(addParams);
		}

		public byte[] GetImageName(string imageID)
		{
			byte[] array = m_imageMap[imageID];
			if (array == null)
			{
				return new byte[0];
			}
			return array;
		}

		static ROMReport()
		{
			m_productVersion = "0.0.0.0";
			m_encoding = Encoding.UTF8;
			m_imageMap = new Dictionary<string, byte[]>();
			m_productVersion = GetProductVersion();
			HTMLRendererResources.PopulateResources(m_imageMap, m_productVersion);
		}

		private static string GetProductVersion()
		{
			return typeof(ROMReport).Assembly.GetName().Version.ToString();
		}

		private static string ParseFromUrl(string mappedID)
		{
			if (mappedID != null && mappedID.IndexOf(m_productVersion, StringComparison.Ordinal) == 0)
			{
				return mappedID.Substring(m_productVersion.Length);
			}
			if (mappedID != null && mappedID.EndsWith("RSClientPrint.cab", StringComparison.OrdinalIgnoreCase))
			{
				return "RSClientPrint.cab";
			}
			return null;
		}

		public static void GetRenderingResource(CreateAndRegisterStream createAndRegisterStreamCallback, string imageName)
		{
			if (imageName == null)
			{
				return;
			}
			imageName = imageName.TrimStart('/');
			string text = ParseFromUrl(imageName);
			if (text == null)
			{
				return;
			}
			string mimeType = "";
			using (Stream stream = HTMLRendererResources.GetStream(text, out mimeType))
			{
				if (stream != null)
				{
					Stream sink = createAndRegisterStreamCallback(text, "gif", null, mimeType, willSeek: false, StreamOper.CreateAndRegister);
					Utility.CopyStream(stream, sink);
				}
			}
		}
	}
}
