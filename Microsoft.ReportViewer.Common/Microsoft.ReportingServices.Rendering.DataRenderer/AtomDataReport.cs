using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Xml;
using Microsoft.ReportingServices.Common;
using Microsoft.ReportingServices.Diagnostics;
using Microsoft.ReportingServices.Diagnostics.Utilities;
using Microsoft.ReportingServices.Interfaces;
using Microsoft.ReportingServices.OnDemandReportRendering;

namespace Microsoft.ReportingServices.Rendering.DataRenderer;

[ExtensionLocalizedName(typeof(StringResources), "LocalizedAtomDataRendererName")]
public sealed class AtomDataReport : DataRendererBase
{
	private enum RenderingMode
	{
		ServiceDocument,
		DataFeed,
		Metadata,
		Batch
	}

	private enum DataFeedIdentifierType
	{
		None,
		DefinitionPath,
		ReportItemPath
	}

	private const string DIEncoding = "Encoding";

	internal const string DIDataFeed = "DataFeed";

	internal const string DIItemPath = "ItemPath";

	internal const string DIQueryTop = "Top";

	internal const string DIQuerySelect = "Select";

	private const string DIVMetadata = "Metadata";

	private const string BatchBoundaryToken = "batch_36522ad7-fc75-4b56-8c71-56071383e77b";

	private const string BatchBoundaryMarkup = "--";

	private const string FileExtensionDataFeed = "atom";

	private const string FileExtensionServiceDocument = "atomsvc";

	private const string FileExtensionMetadata = "xml";

	private const string FileExtensionBatch = "txt";

	private const string MimeTypeDataFeed = "application/atom+xml";

	private const string MimeTypeServiceDocument = "application/atomsvc+xml";

	private const string MimeTypeMetadata = "application/xml";

	private const string MimeTypeBatch = "multipart/mixed; boundary=batch_36522ad7-fc75-4b56-8c71-56071383e77b";

	private const char Comma = ',';

	private Encoding m_encoding = Encoding.UTF8;

	private string m_fileExtension = "atomsvc";

	private string m_mimeType = "application/atomsvc+xml";

	private string[] m_dataFeeds;

	private DataFeedIdentifierType m_dataFeedIdentifierType;

	private Dictionary<string, object> m_dataFeedQueries;

	public override string LocalizedName => StringResources.LocalizedAtomDataRendererName;

	private RenderingMode Mode
	{
		get
		{
			if (m_dataFeeds == null || m_dataFeeds.Length == 0)
			{
				return RenderingMode.ServiceDocument;
			}
			if (m_dataFeeds.Length == 1)
			{
				if (string.Compare(m_dataFeeds[0], "Metadata", StringComparison.OrdinalIgnoreCase) == 0)
				{
					return RenderingMode.Metadata;
				}
				return RenderingMode.DataFeed;
			}
			return RenderingMode.Batch;
		}
	}

	protected override void InternalRender(Microsoft.ReportingServices.OnDemandReportRendering.Report report, NameValueCollection reportServerParameters, NameValueCollection deviceInfo, NameValueCollection clientCapabilities, CreateAndRegisterStream createAndRegisterStream)
	{
		ParseDeviceInfo(deviceInfo);
		switch (Mode)
		{
		case RenderingMode.ServiceDocument:
			m_fileExtension = "atomsvc";
			m_mimeType = "application/atomsvc+xml";
			break;
		case RenderingMode.DataFeed:
			m_fileExtension = "atom";
			m_mimeType = "application/atom+xml";
			break;
		case RenderingMode.Metadata:
			m_fileExtension = "xml";
			m_mimeType = "application/xml";
			break;
		case RenderingMode.Batch:
			m_fileExtension = "txt";
			m_mimeType = "multipart/mixed; boundary=batch_36522ad7-fc75-4b56-8c71-56071383e77b";
			break;
		}
		Stream stream = createAndRegisterStream(report.Name, m_fileExtension, m_encoding, m_mimeType, willSeek: false, StreamOper.CreateAndRegister);
		if (Mode == RenderingMode.Batch)
		{
			WriteBatch(stream, report, reportServerParameters, m_dataFeeds);
		}
		else
		{
			using XmlWriter xmlWriter = CreateXmlWriter(stream, disableUTF8Bom: false);
			xmlWriter.WriteStartDocument(standalone: true);
			switch (Mode)
			{
			case RenderingMode.ServiceDocument:
				WriteAtomServiceDocument(xmlWriter, report, reportServerParameters);
				break;
			case RenderingMode.DataFeed:
				WriteAtomDataFeed(xmlWriter, report, reportServerParameters, m_dataFeeds[0]);
				break;
			case RenderingMode.Metadata:
				WriteCSDLMetadataDocument(xmlWriter, report, reportServerParameters);
				break;
			}
			xmlWriter.Flush();
		}
		stream.Flush();
	}

	private void ParseDeviceInfo(NameValueCollection deviceInfo)
	{
		if (deviceInfo == null)
		{
			return;
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
		m_dataFeedIdentifierType = DataFeedIdentifierType.None;
		m_dataFeeds = ExtractArrayFromDeviceInfo(deviceInfo, "ItemPath");
		if (m_dataFeeds != null)
		{
			m_dataFeedIdentifierType = DataFeedIdentifierType.ReportItemPath;
		}
		else
		{
			m_dataFeeds = ExtractArrayFromDeviceInfo(deviceInfo, "DataFeed");
			if (m_dataFeeds != null)
			{
				m_dataFeedIdentifierType = DataFeedIdentifierType.DefinitionPath;
			}
		}
		m_dataFeedQueries = ExtractDataFeedQueriesFromDeviceInfo(deviceInfo);
	}

	private Dictionary<string, object> ExtractDataFeedQueriesFromDeviceInfo(NameValueCollection deviceInfo)
	{
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		string text = ExtractStringFromDeviceInfo(deviceInfo, "Top");
		if (text != null && uint.TryParse(text, out var result))
		{
			dictionary.Add("Top", result);
		}
		string text2 = ExtractStringFromDeviceInfo(deviceInfo, "Select");
		if (text2 != null)
		{
			dictionary.Add("Select", text2.Split(','));
		}
		return dictionary;
	}

	private string[] ExtractArrayFromDeviceInfo(NameValueCollection deviceInfo, string key)
	{
		return ExtractStringFromDeviceInfo(deviceInfo, key)?.Split(',');
	}

	private void WriteAtomServiceDocument(XmlWriter xmlWriter, Microsoft.ReportingServices.OnDemandReportRendering.Report report, NameValueCollection reportServerParameters)
	{
		AtomServiceDocumentHandler atomServiceDocumentHandler = new AtomServiceDocumentHandlerSql11(null, null);
		ReportWalker reportWalker = new ReportWalker(atomServiceDocumentHandler, atomServiceDocumentHandler, atomServiceDocumentHandler, atomServiceDocumentHandler, instanceWalk: false, walkTopLevelOnly: true);
		reportWalker.WalkReport(report);
		IEnumerator topLevelDataRegionsOrMaps = reportWalker.TopLevelDataRegionsOrMaps;
		AtomServiceDocumentVisitor atomServiceDocumentVisitor = new AtomServiceDocumentVisitor(xmlWriter, reportServerParameters);
		atomServiceDocumentVisitor.CreateServiceDocument(report);
		Dictionary<string, int> feedNames = new Dictionary<string, int>();
		while (topLevelDataRegionsOrMaps.MoveNext())
		{
			AtomServiceDocumentHandler atomServiceDocumentHandler2 = new AtomServiceDocumentHandlerSql11(atomServiceDocumentVisitor, feedNames);
			ReportWalker reportWalker2 = new ReportWalker(atomServiceDocumentHandler2, atomServiceDocumentHandler2, atomServiceDocumentHandler2, atomServiceDocumentHandler2, instanceWalk: false, walkTopLevelOnly: false);
			reportWalker2.WalkDataRegionOrMap((ReportItem)topLevelDataRegionsOrMaps.Current);
			atomServiceDocumentHandler2.WriteCollection(atomServiceDocumentHandler.DynamicElementRoot);
		}
	}

	private void WriteAtomDataFeed(XmlWriter xmlWriter, Microsoft.ReportingServices.OnDemandReportRendering.Report report, NameValueCollection reportServerParameters, string dataFeed)
	{
		if (m_dataFeedIdentifierType == DataFeedIdentifierType.ReportItemPath)
		{
			AtomServiceDocumentHandler.DynamicElement dynamicElementTree = GetDynamicElementTree(report);
			dataFeed = AtomServiceDocumentHandler.GetDefinitionPath(dynamicElementTree, dataFeed);
			if (dataFeed == null)
			{
				throw new ReportRenderingException(StringResources.rrDataFeedNotFound);
			}
		}
		AtomDataFeedVisitor atomDataFeedVisitor = new AtomDataFeedVisitor(xmlWriter, reportServerParameters, report.Parameters, m_dataFeedQueries);
		atomDataFeedVisitor.CreateSyndicationFeed();
		atomDataFeedVisitor.ClearColumnNames();
		atomDataFeedVisitor.ClearFeedLevelRowContent();
		NullHandler nullHandler = new NullHandler();
		AtomDataFeedTextBoxHandler reportHandler = new AtomDataFeedTextBoxHandler(atomDataFeedVisitor);
		ReportWalker reportWalker = new ReportWalker(reportHandler, nullHandler, nullHandler, nullHandler, instanceWalk: false, walkTopLevelOnly: true);
		reportWalker.WalkReport(report);
		IEnumerator topLevelDataRegionsOrMaps = reportWalker.TopLevelDataRegionsOrMaps;
		while (topLevelDataRegionsOrMaps.MoveNext())
		{
			ReportItem reportItem = (ReportItem)topLevelDataRegionsOrMaps.Current;
			if (IsOnDefinitionPath(reportItem.DefinitionPath, dataFeed))
			{
				List<string> stepsToDefinitonPath = GetStepsToDefinitonPath(reportItem, dataFeed);
				if (stepsToDefinitonPath.Count == 0)
				{
					throw new ReportRenderingException(StringResources.rrDataFeedNotFound);
				}
				AtomDataFeedHeaderHandler atomDataFeedHeaderHandler = new AtomDataFeedHeaderHandler(atomDataFeedVisitor, stepsToDefinitonPath);
				ReportWalker reportWalker2 = new ReportWalker(atomDataFeedHeaderHandler, atomDataFeedHeaderHandler, atomDataFeedHeaderHandler, atomDataFeedHeaderHandler, instanceWalk: false, walkTopLevelOnly: false);
				reportWalker2.WalkDataRegionOrMap(reportItem);
				List<string> resolvedEdmTypes = ResolveDataTypes(reportItem, stepsToDefinitonPath);
				atomDataFeedVisitor.SetResolvedEdmTypes(resolvedEdmTypes);
				AtomDataFeedHandler atomDataFeedHandler = new AtomDataFeedHandler(atomDataFeedVisitor, stepsToDefinitonPath);
				ReportWalker dataRegionWalker = new ReportWalker(atomDataFeedHandler, atomDataFeedHandler, atomDataFeedHandler, atomDataFeedHandler, instanceWalk: true, walkTopLevelOnly: false);
				atomDataFeedVisitor.WriteDataRegionFeed(dataRegionWalker, reportItem);
				return;
			}
		}
		throw new ReportRenderingException(StringResources.rrDataFeedNotFound);
	}

	private void WriteCSDLMetadataDocument(XmlWriter xmlWriter, Microsoft.ReportingServices.OnDemandReportRendering.Report report, NameValueCollection reportServerParameters)
	{
		AtomCSDLDataVisitor atomCSDLDataVisitor = new AtomCSDLDataVisitor(xmlWriter, reportServerParameters, report.Parameters);
		atomCSDLDataVisitor.ClearColumnNames();
		atomCSDLDataVisitor.ClearFeedLevelRowContent();
		NullHandler nullHandler = new NullHandler();
		AtomDataFeedTextBoxHandler reportHandler = new AtomDataFeedTextBoxHandler(atomCSDLDataVisitor);
		ReportWalker reportWalker = new ReportWalker(reportHandler, nullHandler, nullHandler, nullHandler, instanceWalk: false, walkTopLevelOnly: true);
		reportWalker.WalkReport(report);
		IEnumerator topLevelDataRegionsOrMaps = reportWalker.TopLevelDataRegionsOrMaps;
		AtomCSDLSchemaVisitor atomCSDLSchemaVisitor = new AtomCSDLSchemaVisitor(xmlWriter, reportServerParameters);
		atomCSDLSchemaVisitor.CreateCSDLDocument(report);
		Dictionary<string, int> feedNames = new Dictionary<string, int>();
		atomCSDLSchemaVisitor.StartEntityContainer();
		while (topLevelDataRegionsOrMaps.MoveNext())
		{
			AtomServiceDocumentHandler atomServiceDocumentHandler = new AtomServiceDocumentHandlerSql11(atomCSDLSchemaVisitor, feedNames);
			ReportWalker reportWalker2 = new ReportWalker(atomServiceDocumentHandler, atomServiceDocumentHandler, atomServiceDocumentHandler, atomServiceDocumentHandler, instanceWalk: false, walkTopLevelOnly: false);
			reportWalker2.WalkDataRegionOrMap((ReportItem)topLevelDataRegionsOrMaps.Current);
			atomServiceDocumentHandler.WriteCollection(null);
		}
		atomCSDLSchemaVisitor.EndEntityContainer();
		topLevelDataRegionsOrMaps = reportWalker.TopLevelDataRegionsOrMaps;
		while (topLevelDataRegionsOrMaps.MoveNext())
		{
			ReportItem reportItem = (ReportItem)topLevelDataRegionsOrMaps.Current;
			foreach (string key in atomCSDLSchemaVisitor.EntityTypes.Keys)
			{
				if (IsOnDefinitionPath(reportItem.DefinitionPath, key))
				{
					atomCSDLDataVisitor.CreateSyndicationFeed();
					List<string> stepsToDefinitonPath = GetStepsToDefinitonPath(reportItem, key);
					AtomDataFeedHeaderHandler atomDataFeedHeaderHandler = new AtomDataFeedHeaderHandler(atomCSDLDataVisitor, stepsToDefinitonPath);
					ReportWalker reportWalker3 = new ReportWalker(atomDataFeedHeaderHandler, atomDataFeedHeaderHandler, atomDataFeedHeaderHandler, atomDataFeedHeaderHandler, instanceWalk: false, walkTopLevelOnly: false);
					reportWalker3.WalkDataRegionOrMap(reportItem);
					List<string> resolvedEdmTypes = ResolveDataTypes(reportItem, stepsToDefinitonPath);
					atomCSDLDataVisitor.SetResolvedEdmTypes(resolvedEdmTypes);
					AtomDataFeedHandler atomDataFeedHandler = new AtomDataFeedHandler(atomCSDLDataVisitor, stepsToDefinitonPath);
					ReportWalker dataRegionWalker = new ReportWalker(atomDataFeedHandler, atomDataFeedHandler, atomDataFeedHandler, atomDataFeedHandler, instanceWalk: true, walkTopLevelOnly: false);
					atomCSDLDataVisitor.WriteDataRegionFeed(dataRegionWalker, reportItem, atomCSDLSchemaVisitor.EntityTypes[key]);
					atomCSDLDataVisitor.ResetColumnNames();
				}
			}
		}
	}

	private List<string> ResolveDataTypes(ReportItem reportItem, List<string> definitionPathSteps)
	{
		AtomDataTypeVisitor atomDataTypeVisitor = new AtomDataTypeVisitor();
		AtomDataFeedHandler atomDataFeedHandler = new AtomDataFeedHandler(atomDataTypeVisitor, definitionPathSteps);
		ReportWalker dataRegionWalker = new ReportWalker(atomDataFeedHandler, atomDataFeedHandler, atomDataFeedHandler, atomDataFeedHandler, instanceWalk: true, walkTopLevelOnly: false);
		atomDataTypeVisitor.CreateSyndicationFeed();
		atomDataTypeVisitor.WriteDataRegionFeed(dataRegionWalker, reportItem);
		return atomDataTypeVisitor.ColumnEdmTypes;
	}

	private void WriteBatchItemHeader(StreamWriter streamWriter, bool isDataFeed)
	{
		streamWriter.Write("--");
		streamWriter.WriteLine("batch_36522ad7-fc75-4b56-8c71-56071383e77b");
		streamWriter.WriteLine("Content-Type: application/http");
		streamWriter.WriteLine("Content-Transfer-Encoding: binary");
		streamWriter.WriteLine();
		streamWriter.WriteLine("HTTP/1.1 200 OK");
		if (isDataFeed)
		{
			streamWriter.WriteLine("Content-Type: {0};type=feed", "application/atom+xml");
		}
		else
		{
			streamWriter.WriteLine("Content-Type: {0};charset={1}", "application/xml", m_encoding.WebName);
		}
		streamWriter.WriteLine();
		streamWriter.Flush();
	}

	private void WriteBatchItemTermination(StreamWriter streamWriter)
	{
		streamWriter.Write("--");
		streamWriter.Write("batch_36522ad7-fc75-4b56-8c71-56071383e77b");
		streamWriter.WriteLine("--");
		streamWriter.Flush();
	}

	private void WriteBatch(Stream outputStream, Microsoft.ReportingServices.OnDemandReportRendering.Report report, NameValueCollection reportServerParameters, string[] dataFeeds)
	{
		Hashtable hashtable = new Hashtable();
		foreach (string key in dataFeeds)
		{
			if (hashtable.ContainsKey(key))
			{
				throw new ReportRenderingException(StringResources.rrDuplicatedBatchItem);
			}
			hashtable.Add(key, null);
		}
		StreamWriter streamWriter = new StreamWriter(outputStream, m_encoding);
		foreach (string text in dataFeeds)
		{
			using XmlWriter xmlWriter = CreateXmlWriter(outputStream, disableUTF8Bom: true);
			if (string.Compare(text, "Metadata", StringComparison.OrdinalIgnoreCase) == 0)
			{
				WriteBatchItemHeader(streamWriter, isDataFeed: false);
				WriteCSDLMetadataDocument(xmlWriter, report, reportServerParameters);
			}
			else
			{
				WriteBatchItemHeader(streamWriter, isDataFeed: true);
				WriteAtomDataFeed(xmlWriter, report, reportServerParameters, text);
			}
			streamWriter.WriteLine();
			xmlWriter.Flush();
		}
		WriteBatchItemTermination(streamWriter);
	}

	private static AtomServiceDocumentHandler.DynamicElement GetDynamicElementTree(Microsoft.ReportingServices.OnDemandReportRendering.Report report)
	{
		AtomServiceDocumentHandlerSql11 atomServiceDocumentHandlerSql = new AtomServiceDocumentHandlerSql11(null, null);
		ReportWalker reportWalker = new ReportWalker(atomServiceDocumentHandlerSql, atomServiceDocumentHandlerSql, atomServiceDocumentHandlerSql, atomServiceDocumentHandlerSql, instanceWalk: false, walkTopLevelOnly: false);
		reportWalker.WalkReport(report);
		return atomServiceDocumentHandlerSql.DynamicElementRoot;
	}

	private List<string> GetStepsToDefinitonPath(ReportItem reportItem, string definitionPath)
	{
		List<string> list = new List<string>();
		AtomServiceDocumentVisitor visitor = new AtomServiceDocumentVisitor(null, null);
		AtomServiceDocumentHandler atomServiceDocumentHandler = new AtomServiceDocumentHandler(visitor);
		ReportWalker reportWalker = new ReportWalker(atomServiceDocumentHandler, atomServiceDocumentHandler, atomServiceDocumentHandler, atomServiceDocumentHandler, instanceWalk: false, walkTopLevelOnly: false);
		reportWalker.WalkDataRegionOrMap(reportItem);
		AtomServiceDocumentHandler.DynamicElement topLevelDynamicElement = atomServiceDocumentHandler.GetTopLevelDynamicElement();
		TraverseStepsToDefinitionPath(topLevelDynamicElement, definitionPath, list);
		return list;
	}

	private bool TraverseStepsToDefinitionPath(AtomServiceDocumentHandler.DynamicElement dynamicElement, string definitionPath, List<string> steps)
	{
		if (dynamicElement.DefinitionPath == definitionPath)
		{
			steps.Add(dynamicElement.DefinitionPath);
			return true;
		}
		if (dynamicElement.Children.Count > 0)
		{
			foreach (AtomServiceDocumentHandler.DynamicElement child in dynamicElement.Children)
			{
				if (TraverseStepsToDefinitionPath(child, definitionPath, steps))
				{
					steps.Insert(0, dynamicElement.DefinitionPath);
					return true;
				}
			}
		}
		return false;
	}

	public static bool IsOnDefinitionPath(string item, string path)
	{
		if (path.StartsWith(item, StringComparison.Ordinal))
		{
			if (path.Length <= item.Length)
			{
				return true;
			}
			if (!char.IsDigit(path[item.Length]))
			{
				return true;
			}
		}
		return false;
	}

	private XmlWriter CreateXmlWriter(Stream stream, bool disableUTF8Bom)
	{
		XmlWriterSettings xmlWriterSettings = new XmlWriterSettings();
		if (disableUTF8Bom && m_encoding is UTF8Encoding)
		{
			xmlWriterSettings.Encoding = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false);
		}
		else
		{
			xmlWriterSettings.Encoding = m_encoding;
		}
		xmlWriterSettings.NewLineHandling = NewLineHandling.Entitize;
		return XmlWriter.Create(stream, xmlWriterSettings);
	}
}
