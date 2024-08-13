using System;
using System.Collections;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Text;
using Microsoft.ReportingServices.Common;
using Microsoft.ReportingServices.Diagnostics;
using Microsoft.ReportingServices.Diagnostics.Utilities;
using Microsoft.ReportingServices.Interfaces;
using Microsoft.ReportingServices.OnDemandReportRendering;

namespace Microsoft.ReportingServices.Rendering.DataRenderer;

sealed class CsvReport : DataRendererBase
{
	private const string DIExcelMode = "ExcelMode";

	private const string DIUseFormattedValues = "UseFormattedValues";

	private const string DIEncoding = "Encoding";

	private const string DINoHeader = "NoHeader";

	private const string DIExtension = "Extension";

	private const string DIFileExtension = "FileExtension";

	private const string DISuppressLineBreaks = "SuppressLineBreaks";

	private const string DIQualifier = "Qualifier";

	private const string DIRecordDelimiter = "RecordDelimiter";

	private const string DIFieldDelimiter = "FieldDelimiter";

	private const string FileExtension = "csv";

	private const string Qualifier = "\"";

	private const string RecordDelimiter = "\r\n";

	private const string FieldDelimiter = ",";

	private const string MimeType = "text/csv";

	private bool m_excelMode = true;

	private bool m_useFormattedValues;

	private Encoding m_encoding = Encoding.UTF8;

	private bool m_noHeader;

	private string m_fileExtension = "csv";

	private bool m_suppressLineBreaks;

	private string m_qualifier = "\"";

	private string m_recordDelimiter = "\r\n";

	private string m_fieldDelimiter = ",";

	private string m_mimeType = "text/csv";

	public override string LocalizedName => StringResources.LocalizedCsvRendererName;

	protected override void InternalRender(Microsoft.ReportingServices.OnDemandReportRendering.Report report, NameValueCollection reportServerParameters, NameValueCollection deviceInfo, NameValueCollection clientCapabilities, CreateAndRegisterStream createAndRegisterStream)
	{
		ParseDeviceInfo(deviceInfo);
		if (m_fieldDelimiter == m_recordDelimiter)
		{
			throw new ReportRenderingException(StringResources.rrSameDelimiterError);
		}
		if (m_qualifier == m_fieldDelimiter || m_qualifier == m_recordDelimiter)
		{
			throw new ReportRenderingException(StringResources.rrSameQualiferDelimiterError);
		}
		if (m_qualifier.Length <= 0 || (m_suppressLineBreaks && (m_qualifier.Trim().Length <= 0 || m_qualifier.IndexOf('\n') >= 0)))
		{
			throw new ReportRenderingException(StringResources.rrInvalidQualifier);
		}
		Stream stream = createAndRegisterStream(report.Name, m_fileExtension, m_encoding, m_mimeType, willSeek: false, StreamOper.CreateAndRegister);
		CsvVisitor csvVisitor = new CsvVisitor(stream, m_fieldDelimiter, m_recordDelimiter, m_qualifier, m_suppressLineBreaks, m_encoding);
		CsvColumnHeaderHandler csvColumnHeaderHandler = new CsvColumnHeaderHandler(csvVisitor, m_noHeader, m_excelMode);
		ReportWalker reportWalker = new ReportWalker(csvColumnHeaderHandler, csvColumnHeaderHandler, csvColumnHeaderHandler, csvColumnHeaderHandler, instanceWalk: false, m_excelMode);
		reportWalker.WalkReport(report);
		int num = csvColumnHeaderHandler.Columns - csvColumnHeaderHandler.HeaderOnlyColumns;
		if (num > 0)
		{
			CsvHandler csvHandler = new CsvHandler(csvVisitor, m_excelMode, m_useFormattedValues);
			ReportWalker reportWalker2 = new ReportWalker(csvHandler, csvHandler, csvHandler, csvHandler, instanceWalk: true, m_excelMode);
			reportWalker2.WalkReport(report);
		}
		if (m_excelMode)
		{
			IEnumerator topLevelDataRegionsOrMaps = reportWalker.TopLevelDataRegionsOrMaps;
			while (topLevelDataRegionsOrMaps.MoveNext())
			{
				CsvColumnHeaderHandler csvColumnHeaderHandler2 = new CsvColumnHeaderHandler(csvVisitor, m_noHeader, m_excelMode);
				ReportWalker reportWalker3 = new ReportWalker(csvColumnHeaderHandler2, csvColumnHeaderHandler2, csvColumnHeaderHandler2, csvColumnHeaderHandler2, instanceWalk: false, walkTopLevelOnly: false);
				reportWalker3.WalkDataRegionOrMap((ReportItem)topLevelDataRegionsOrMaps.Current);
				int num2 = csvColumnHeaderHandler2.Columns - csvColumnHeaderHandler2.HeaderOnlyColumns;
				if (num2 > 0)
				{
					CsvHandler csvHandler2 = new CsvHandler(csvVisitor, m_excelMode, m_useFormattedValues);
					ReportWalker reportWalker4 = new ReportWalker(csvHandler2, csvHandler2, csvHandler2, csvHandler2, instanceWalk: true, walkTopLevelOnly: false);
					reportWalker4.WalkDataRegionOrMap((ReportItem)topLevelDataRegionsOrMaps.Current);
				}
				else if (csvColumnHeaderHandler2.HeaderOnlyColumns > 0)
				{
					csvVisitor.EndRegion();
				}
			}
		}
		stream.Flush();
	}

	private void ParseDeviceInfo(NameValueCollection deviceInfo)
	{
		if (deviceInfo == null)
		{
			return;
		}
		if (deviceInfo["ExcelMode"] != null && !bool.TryParse(deviceInfo["ExcelMode"], out m_excelMode))
		{
			m_excelMode = true;
		}
		m_useFormattedValues = (m_excelMode ? true : false);
		if (deviceInfo["UseFormattedValues"] != null && !bool.TryParse(deviceInfo["UseFormattedValues"], out m_useFormattedValues))
		{
			m_useFormattedValues = false;
		}
		if (deviceInfo["NoHeader"] != null && !bool.TryParse(deviceInfo["NoHeader"], out m_noHeader))
		{
			m_noHeader = false;
		}
		if (deviceInfo["SuppressLineBreaks"] != null && !bool.TryParse(deviceInfo["SuppressLineBreaks"], out m_suppressLineBreaks))
		{
			m_suppressLineBreaks = false;
		}
		if (deviceInfo["Encoding"] != null)
		{
			try
			{
				m_encoding = Encoding.GetEncoding(deviceInfo["Encoding"]);
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
				}
			}
		}
		m_fileExtension = ExtractStringFromDeviceInfo(deviceInfo, "FileExtension");
		if (m_fileExtension == null)
		{
			m_fileExtension = ExtractStringFromDeviceInfo(deviceInfo, "Extension");
			if (m_fileExtension == null)
			{
				m_fileExtension = "csv";
			}
		}
		m_qualifier = ExtractStringFromDeviceInfo(deviceInfo, "Qualifier");
		if (m_qualifier == null)
		{
			m_qualifier = "\"";
		}
		m_recordDelimiter = ExtractStringFromDeviceInfo(deviceInfo, "RecordDelimiter");
		if (m_recordDelimiter == null)
		{
			m_recordDelimiter = "\r\n";
		}
		m_fieldDelimiter = ExtractStringFromDeviceInfo(deviceInfo, "FieldDelimiter");
		if (m_fieldDelimiter == null)
		{
			m_fieldDelimiter = ",";
		}
	}
}
