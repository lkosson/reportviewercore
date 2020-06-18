using Microsoft.ReportingServices.Interfaces;
using Microsoft.ReportingServices.OnDemandReportRendering;
using Microsoft.ReportingServices.Rendering.ExcelRenderer.Excel;
using Microsoft.ReportingServices.Rendering.ExcelRenderer.Layout;
using Microsoft.ReportingServices.Rendering.ExcelRenderer.SPBIF;
using Microsoft.ReportingServices.Rendering.RPLProcessing;
using System;
using System.Collections.Generic;
using System.IO;

namespace Microsoft.ReportingServices.Rendering.ExcelRenderer
{
	internal sealed class MainEngine : IDisposable
	{
		private IExcelGenerator m_excel;

		private CreateAndRegisterStream m_streamDelegate;

		private long m_totalScaleTimeMs;

		private long m_peakMemoryUsageKB;

		private Dictionary<string, BorderInfo> m_sharedBorderCache = new Dictionary<string, BorderInfo>();

		private Dictionary<string, ImageInformation> m_sharedImageCache = new Dictionary<string, ImageInformation>();

		private Stream m_backgroundImage;

		private ushort m_backgroundImageWidth;

		private ushort m_backgroundImageHeight;

		internal long TotalScaleTimeMs => m_totalScaleTimeMs;

		internal long PeakMemoryUsageKB => m_peakMemoryUsageKB;

		internal MainEngine(CreateAndRegisterStream createStream, ExcelRenderer excelRenderer)
		{
			m_streamDelegate = createStream;
			m_excel = excelRenderer.CreateExcelGenerator(CreateTempStream);
		}

		public void Dispose()
		{
			foreach (KeyValuePair<string, ImageInformation> item in m_sharedImageCache)
			{
				ImageInformation value = item.Value;
				if (value != null && value.ImageData != null)
				{
					value.ImageData.Close();
					value.ImageData = null;
				}
			}
			if (m_backgroundImage != null)
			{
				m_backgroundImage.Close();
				m_backgroundImage = null;
			}
		}

		private Stream CreateTempStream(string name)
		{
			return m_streamDelegate(name, ".bin", null, "", willSeek: true, StreamOper.CreateOnly);
		}

		internal void AddBackgroundImage(RPLPageContent pageContent, RPLReportSection reportSection)
		{
			RPLImageData rPLImageData = (RPLImageData)((RPLBody)reportSection.Columns[0].Element).ElementProps.Style[33];
			if (rPLImageData == null)
			{
				rPLImageData = (RPLImageData)pageContent.PageLayout.Style[33];
			}
			if (rPLImageData != null && rPLImageData.ImageData != null && rPLImageData.ImageData.Length != 0)
			{
				m_excel.AddBackgroundImage(rPLImageData.ImageData, rPLImageData.ImageName, ref m_backgroundImage, ref m_backgroundImageWidth, ref m_backgroundImageHeight);
			}
		}

		internal bool AddDocumentMap(DocumentMap docMap)
		{
			if (docMap == null)
			{
				return false;
			}
			m_excel.SetCurrentSheetName(ExcelRenderRes.DocumentMap);
			int num = 0;
			m_excel.SetColumnExtents(0, m_excel.MaxColumns - 1);
			m_excel.SetSummaryRowAfter(after: false);
			m_excel.DefineCachedStyle("DocumentMapHyperlinkStyle");
			m_excel.GetCellStyle().Color = m_excel.AddColor("Blue");
			m_excel.GetCellStyle().Underline = Underline.Single;
			m_excel.GetCellStyle().Name = "Arial";
			m_excel.GetCellStyle().Size = 10.0;
			m_excel.EndCachedStyle();
			while (docMap.MoveNext() && num <= m_excel.MaxRows)
			{
				if (num % m_excel.RowBlockSize == 0)
				{
					for (int i = 0; i < m_excel.RowBlockSize; i++)
					{
						m_excel.AddRow(i + num);
					}
				}
				DocumentMapNode current = docMap.Current;
				m_excel.SetRowContext(num);
				m_excel.SetColumnContext(current.Level - 1);
				m_excel.SetCellValue(current.Label, TypeCode.String);
				if (num == 0)
				{
					m_excel.GetCellStyle().Bold = 700;
					m_excel.GetCellStyle().Name = "Arial";
					m_excel.GetCellStyle().Size = 10.0;
					m_excel.GetCellStyle().Color = m_excel.AddColor("Black");
				}
				else
				{
					m_excel.AddBookmarkLink(current.Label, current.Id);
					m_excel.UseCachedStyle("DocumentMapHyperlinkStyle");
				}
				m_excel.SetRowProperties(num, 240, (byte)(current.Level - 1), current.Level > 2, autoSize: false);
				m_excel.AddMergeCell(num, current.Level - 1, num, 32);
				num++;
			}
			for (int j = 0; j < m_excel.MaxColumns; j++)
			{
				m_excel.SetColumnProperties(j, 20.0, 0, collapsed: false);
			}
			return true;
		}

		internal void AdjustFirstWorksheetName(string reportName, bool addedDocMap)
		{
			m_excel.AdjustFirstWorksheetName(reportName, addedDocMap);
		}

		internal void NextPage()
		{
			m_excel.NextWorksheet();
		}

		internal void RenderRPLPage(RPLReport report, bool headerInBody, bool suppressOutlines)
		{
			string key = null;
			LayoutEngine layoutEngine = new LayoutEngine(report, headerInBody, m_streamDelegate);
			RPLPageContent rPLPageContent = report.RPLPaginatedPages[0];
			m_excel.GenerateWorksheetName(rPLPageContent.PageLayout.PageName);
			RPLReportSection nextReportSection = rPLPageContent.GetNextReportSection();
			AddBackgroundImage(rPLPageContent, nextReportSection);
			Reader.ReadReportMeasurements(report, layoutEngine, suppressOutlines, nextReportSection);
			try
			{
				layoutEngine.RenderPageToExcel(m_excel, key, m_sharedBorderCache, m_sharedImageCache);
				if (layoutEngine.ScalabilityCache != null)
				{
					m_totalScaleTimeMs += layoutEngine.ScalabilityCache.ScalabilityDurationMs;
					m_peakMemoryUsageKB = Math.Max(m_peakMemoryUsageKB, layoutEngine.ScalabilityCache.PeakMemoryUsageKBytes);
				}
			}
			finally
			{
				layoutEngine.Dispose();
			}
		}

		internal void Save(Stream output)
		{
			m_excel.SaveSpreadsheet(output, m_backgroundImage, m_backgroundImageWidth, m_backgroundImageHeight);
		}
	}
}
