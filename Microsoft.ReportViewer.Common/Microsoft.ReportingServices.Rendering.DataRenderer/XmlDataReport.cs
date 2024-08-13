using System;
using System.Collections;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Xsl;
using Microsoft.ReportingServices.Common;
using Microsoft.ReportingServices.Diagnostics;
using Microsoft.ReportingServices.Diagnostics.Utilities;
using Microsoft.ReportingServices.Interfaces;
using Microsoft.ReportingServices.OnDemandReportRendering;

namespace Microsoft.ReportingServices.Rendering.DataRenderer;

sealed class XmlDataReport : DataRendererBase
{
	private const bool DoAttributesPass = true;

	private const string DefaultXSLT = null;

	private const string XmlMIMEType = "text/xml";

	private const string DefaultMIMEType = "text/xml";

	private const bool DefaultUseFormattedValues = false;

	private const bool DefaultIndented = false;

	private const bool DefaultNoSchema = false;

	private const bool DefaultNoNamespace = false;

	private const string DefaultDataFileExtension = "xml";

	private const string XsdFileExtension = "xsd";

	private const string DIUseFormattedValues = "UseFormattedValues";

	private const string DIIndented = "Indented";

	private const string DIEncoding = "Encoding";

	private const string DIOmitSchema = "OmitSchema";

	private const string DIOmitNamespace = "OmitNamespace";

	private const string DIMIMEType = "MIMEType";

	private const string DIFileExtension = "FileExtension";

	private const string DIXSLT = "XSLT";

	private const string DISchema = "Schema";

	private static readonly Encoding DefaultEncoding = Encoding.UTF8;

	private string m_schemaName;

	private bool m_useFormattedValues;

	private bool m_indented;

	private Encoding m_encoding = DefaultEncoding;

	private bool m_noSchema;

	private bool m_noNamespace;

	private string m_mimeType = "text/xml";

	private string m_dataFileExtension = "xml";

	private string m_innerXSLT;

	private string m_outerXSLT;

	public override string LocalizedName => StringResources.LocalizedXmlRendererName;

	protected override void InternalRender(Microsoft.ReportingServices.OnDemandReportRendering.Report report, NameValueCollection reportServerParameters, NameValueCollection deviceInfo, NameValueCollection clientCapabilities, CreateAndRegisterStream createStream)
	{
		ReadReportAttributes(report);
		bool flag = ParseDeviceInfo(deviceInfo);
		if (m_innerXSLT != null || m_outerXSLT != null || m_noNamespace)
		{
			m_noSchema = true;
		}
		if (m_schemaName == null)
		{
			m_schemaName = XmlConvert.EncodeLocalName(report.Name ?? string.Empty);
		}
		if (flag)
		{
			RenderData(report, createStream, reportServerParameters);
		}
		else
		{
			RenderSchema(report, createStream);
		}
	}

	private void RenderData(Microsoft.ReportingServices.OnDemandReportRendering.Report report, CreateAndRegisterStream createStream, NameValueCollection reportServerParameters)
	{
		XmlWriter xmlWriter = null;
		if (m_innerXSLT == null && m_outerXSLT == null)
		{
			xmlWriter = CreateXmlWriter(CreateDataStream(report.Name, createStream), null);
			RenderReportData(report, reportServerParameters, xmlWriter);
			return;
		}
		XslCompiledTransform xslCompiledTransform = null;
		XslCompiledTransform xslCompiledTransform2 = null;
		if (m_innerXSLT != null)
		{
			xslCompiledTransform = new XslCompiledTransform();
			LoadXslt(report, m_innerXSLT, xslCompiledTransform);
		}
		if (m_outerXSLT != null)
		{
			if (xslCompiledTransform == null)
			{
				xslCompiledTransform = new XslCompiledTransform();
				LoadXslt(report, m_outerXSLT, xslCompiledTransform);
			}
			else
			{
				xslCompiledTransform2 = new XslCompiledTransform();
				LoadXslt(report, m_outerXSLT, xslCompiledTransform2);
			}
		}
		Stream stream = null;
		Stream stream2 = null;
		try
		{
			stream = CreateTempStream("temp1Stream", createStream);
			RenderReportData(report, reportServerParameters, CreateXmlWriter(stream, null));
			stream.Position = 0L;
			if (xslCompiledTransform2 == null)
			{
				xmlWriter = CreateXmlWriter(CreateDataStream(report.Name, createStream), xslCompiledTransform.OutputSettings);
				try
				{
					xslCompiledTransform.Transform(XmlUtil.SafeCreateXmlTextReader(stream), null, xmlWriter);
				}
				catch (XmlException innerException)
				{
					throw new ReportRenderingException(StringResources.rrBadXsltTransformation, innerException);
				}
			}
			else
			{
				xmlWriter = CreateXmlWriter(CreateDataStream(report.Name, createStream), xslCompiledTransform2.OutputSettings);
				stream2 = CreateTempStream("temp2Stream", createStream);
				try
				{
					xslCompiledTransform.Transform(XmlUtil.SafeCreateXmlTextReader(stream), null, stream2);
					stream2.Position = 0L;
					xslCompiledTransform2.Transform(XmlUtil.SafeCreateXmlTextReader(stream2), null, xmlWriter);
				}
				catch (XmlException innerException2)
				{
					throw new ReportRenderingException(StringResources.rrBadXsltTransformation, innerException2);
				}
			}
			xmlWriter.Flush();
		}
		finally
		{
			stream?.Close();
			stream2?.Close();
		}
	}

	private void RenderReportData(Microsoft.ReportingServices.OnDemandReportRendering.Report report, NameValueCollection reportServerParameters, XmlWriter outputStream)
	{
		string schemaLocation = null;
		if (!m_noSchema)
		{
			CatalogItemUrlBuilder catalogItemUrlBuilder = new CatalogItemUrlBuilder(report.GetReportUrl(addReportParameters: false));
			catalogItemUrlBuilder.AppendCatalogParameters(reportServerParameters);
			catalogItemUrlBuilder.AppendRenderingParameter("Schema", bool.TrueString);
			schemaLocation = catalogItemUrlBuilder.ToString();
		}
		if (m_noNamespace)
		{
			m_schemaName = null;
		}
		XmlDataVisitor xmlDataVisitor = new XmlDataVisitor(outputStream, m_schemaName, schemaLocation);
		XmlReportHandler xmlReportHandler = new XmlReportHandler(report, xmlDataVisitor, m_useFormattedValues, aDoAttributesPass: true);
		xmlReportHandler.ProcessReport();
		xmlDataVisitor.Flush();
	}

	private void RenderSchema(Microsoft.ReportingServices.OnDemandReportRendering.Report report, CreateAndRegisterStream createStream)
	{
		XmlTypeHandler xmlTypeHandler = new XmlTypeHandler();
		if (!m_useFormattedValues)
		{
			ReportWalker reportWalker = new ReportWalker(xmlTypeHandler, xmlTypeHandler, xmlTypeHandler, null, instanceWalk: false, walkTopLevelOnly: true);
			reportWalker.WalkReport(report);
			IEnumerator topLevelDataRegionsOrMaps = reportWalker.TopLevelDataRegionsOrMaps;
			ReportWalker reportWalker2 = new ReportWalker(xmlTypeHandler, xmlTypeHandler, xmlTypeHandler, null, instanceWalk: true, walkTopLevelOnly: false);
			while (topLevelDataRegionsOrMaps.MoveNext())
			{
				reportWalker2.WalkDataRegionOrMap((ReportItem)topLevelDataRegionsOrMaps.Current);
			}
		}
		XmlWriter xw = CreateXmlWriter(CreateSchemaStream(createStream), null);
		XmlSchemaVisitor xmlSchemaVisitor = new XmlSchemaVisitor(xw, m_schemaName, xmlTypeHandler.TextBoxTypes);
		XmlReportHandler xmlReportHandler = new XmlReportHandler(report, xmlSchemaVisitor, m_useFormattedValues, aDoAttributesPass: true);
		xmlReportHandler.ProcessReport();
		xmlSchemaVisitor.Flush();
	}

	private XmlWriter CreateXmlWriter(Stream outputStream, XmlWriterSettings xws)
	{
		if (xws == null)
		{
			xws = new XmlWriterSettings();
			xws.Encoding = m_encoding;
			xws.Indent = m_indented;
		}
		return XmlWriter.Create(outputStream, xws);
	}

	private Stream CreateDataStream(string aStreamName, CreateAndRegisterStream createStream)
	{
		return createStream(aStreamName, m_dataFileExtension, m_encoding, m_mimeType, willSeek: false, StreamOper.CreateAndRegister);
	}

	private Stream CreateSchemaStream(CreateAndRegisterStream createStream)
	{
		return createStream(m_schemaName, "xsd", m_encoding, m_mimeType, willSeek: false, StreamOper.CreateAndRegister);
	}

	private Stream CreateTempStream(string name, CreateAndRegisterStream createStream)
	{
		return createStream(name, null, m_encoding, null, willSeek: true, StreamOper.CreateOnly);
	}

	private void LoadXslt(Microsoft.ReportingServices.OnDemandReportRendering.Report report, string xsltPath, XslCompiledTransform xslt)
	{
		try
		{
			byte[] resource = null;
			if (report.GetResource(xsltPath, out resource, out var _) && resource != null)
			{
				using XmlReader stylesheet = XmlUtil.SafeCreateXmlTextReader(new MemoryStream(resource));
				xslt.Load(stylesheet);
			}
			if (resource == null)
			{
				throw new ReportRenderingException(StringResources.rrBadXsltPath);
			}
		}
		catch (Exception innerException)
		{
			throw new ReportRenderingException(StringResources.rrCanNotLoadXSLT(xsltPath), innerException);
		}
	}

	private void ReadReportAttributes(Microsoft.ReportingServices.OnDemandReportRendering.Report report)
	{
		m_schemaName = report.DataSchema;
		m_innerXSLT = report.DataTransform;
	}

	private bool ParseDeviceInfo(NameValueCollection deviceInfo)
	{
		bool result = true;
		if (deviceInfo == null)
		{
			return result;
		}
		if (deviceInfo["UseFormattedValues"] != null && !bool.TryParse(deviceInfo["UseFormattedValues"], out m_useFormattedValues))
		{
			m_useFormattedValues = false;
		}
		if (deviceInfo["Indented"] != null && !bool.TryParse(deviceInfo["Indented"], out m_indented))
		{
			m_indented = false;
		}
		if (deviceInfo["OmitSchema"] != null && !bool.TryParse(deviceInfo["OmitSchema"], out m_noSchema))
		{
			m_noSchema = false;
		}
		if (deviceInfo["OmitNamespace"] != null && !bool.TryParse(deviceInfo["OmitNamespace"], out m_noNamespace))
		{
			m_noNamespace = false;
		}
		if (deviceInfo["Schema"] != null && bool.TryParse(deviceInfo["Schema"], out result))
		{
			result = !result;
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
				m_encoding = DefaultEncoding;
			}
		}
		m_outerXSLT = ExtractStringFromDeviceInfo(deviceInfo, "XSLT");
		if (m_outerXSLT == null)
		{
			m_outerXSLT = null;
		}
		m_mimeType = ExtractStringFromDeviceInfo(deviceInfo, "MIMEType");
		if (m_mimeType == null)
		{
			m_mimeType = "text/xml";
		}
		m_dataFileExtension = ExtractStringFromDeviceInfo(deviceInfo, "FileExtension");
		if (m_dataFileExtension == null)
		{
			m_dataFileExtension = "xml";
		}
		return result;
	}
}
