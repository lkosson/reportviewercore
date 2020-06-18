using Microsoft.Cloud.Platform.Utils;
using Microsoft.ReportingServices.Diagnostics.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;
using System.Xml;

namespace Microsoft.ReportingServices.Diagnostics
{
	internal abstract class RSRequestParameters : IReportParameterLookup
	{
		public enum CacheDeviceInfoTags
		{
			Parameters,
			ReplacementRoot,
			StreamRoot
		}

		private enum HttpMethod
		{
			GET,
			POST
		}

		private Hashtable m_reverseLookupParameters;

		protected NameValueCollection m_renderingParameters;

		private NameValueCollection m_reportParameters;

		private NameValueCollection m_catalogParameters;

		private DatasourceCredentialsCollection m_datasourcesCred;

		private NameValueCollection m_browserCapabilities;

		private string m_ServerVirtualRoot;

		private string m_SessionId;

		private const string ParametersXmlElement = "Parameters";

		private const string ParameterXmlElement = "Parameter";

		private const string NameXmlElement = "Name";

		private const string ValueXmlElement = "Value";

		private const string BrowserCapabilitiesXmlElement = "BrowserCapabilities";

		private const string DeviceInfoXmlElement = "DeviceInfo";

		public const string ReportParameterPrefix = "";

		public const string CatalogParameterPrefix = "rs:";

		public const string RenderingParameterPrefix = "rc:";

		public const string UserNameParameterPrefix = "dsu:";

		public const string PasswordParameterPrefix = "dsp:";

		public const string ParameterNullSuffix = ":isnull";

		public const string PowerViewParameterPrefix = "pv:";

		public const char PrefixTerminatorChar = ':';

		public const string FormatParamName = "Format";

		public const string EncodingParamName = "Encoding";

		public const string FullFeatureFormat = "HTML5";

		public const string OWCFormat = "HTMLOWC";

		public const string StreamRoot = "StreamRoot";

		public const string PrimaryVersion = "2015-02";

		public const string Version201411 = "2014-11";

		public const string Version201409iOSDev = "2014-09-iOSDev";

		public const string Version201409iOS = "2014-09-iOS";

		public const string Version201403 = "2014-03";

		public const string iOSDevVersion = "2014-09-iOSDev";

		public const string iOSVersion = "2014-09-iOS";

		public const string UpgradeVersion = "2014-11";

		public const string HelixVersion = "2014-03";

		public const string ShowHideToggleParamName = "ShowHideToggle";

		public const string SortIdParamName = "SortId";

		public const string ClearSortParamName = "ClearSort";

		public const string SortDirectionParamName = "SortDirection";

		public const string AllowNewSessionsParamName = "AllowNewSessions";

		public const string ResetSessionParamName = "ResetSession";

		public const string CommandParamName = "Command";

		public const string SessionIDParamName = "SessionID";

		public const string PowerViewSessionId = "ocp-sqlrs-session-id";

		public const string PowerViewRoutingToken = "ocp-sqlrs-rtoken";

		public const string ServiceApiVersion = "api-version";

		public const string ResponseApiVersion = "accept-api-version";

		public const string ResponseGroupVersion = "accept-api-version-group";

		public const string ReportId = "ReportId";

		public const string TileId = "TileId";

		public const string InstantiationMode = "InstantiationMode";

		public const string DashboardId = "DashboardId";

		public const string SaveType = "SaveType";

		public const string ReportName = "ReportName";

		public const string ImageIDParamName = "ImageID";

		public const string SnapshotParamName = "Snapshot";

		public const string ClearSessionParamName = "ClearSession";

		public const string ErrorResponseAsXml = "ErrorResponseAsXml";

		public const string StoredParametersID = "StoredParametersID";

		public const string ProgressiveSessionId = "ProgressiveSessionId";

		public const string ParamLanguage = "ParameterLanguage";

		public const string ReturnUrlParamName = "ReturnUrl";

		public const string RendererAccessCommand = "Get";

		public const string RunReportCommand = "Render";

		public const string ListChildrenCommand = "ListChildren";

		public const string GetResourceContentsCommand = "GetResourceContents";

		public const string GetDataSourceContentsCommand = "GetDataSourceContents";

		public const string GetModelDefinitionCommand = "GetModelDefinition";

		public const string DrillthroughCommand = "Drillthrough";

		public const string ExecuteQueryCommand = "ExecuteQuery";

		public const string BlankCommand = "Blank";

		public const string SortCommand = "Sort";

		public const string StyleSheet = "StyleSheet";

		public const string StyleSheetImage = "StyleSheetImage";

		public const string GetComponentDefinitionCommand = "GetComponentDefinition";

		public const string GetDataSetDefinitionCommand = "GetDataSetDefinition";

		public const string Ascending = "Ascending";

		public const string Descending = "Descending";

		public const string DBUserParamName = "DBUser";

		public const string DBPasswordParamName = "DBPassword";

		public const string PersistStreams = "PersistStreams";

		public const string GetNextStream = "GetNextStream";

		public const string EntityID = "EntityID";

		public const string DrillType = "DrillType";

		public const string DataSourceName = "DataSourceName";

		public const string CommandText = "CommandText";

		public const string Timeout = "Timeout";

		public const string GetUserModel = "GetUserModel";

		public const string PerspectiveID = "PerspectiveID";

		public const string OmitModelDefinitions = "OmitModelDefinitions";

		public const string ModelMetadataVersion = "ModelMetadataVersion";

		public const string ItemPath = "ItemPath";

		public const string SourceReportUri = "SourceReportUri";

		public const string ReturnRawDataParamName = "ReturnRawData";

		public const string StyleSheetName = "Name";

		public const string StyleSheetImageName = "Name";

		public const string PaginationMode = "PageCountMode";

		public const string ActualPageMode = "Actual";

		public const string EstimatePageMode = "Estimate";

		internal const string IsCancellable = "IsCancellable";

		public const string RenderEditCommand = "RenderEdit";

		public const string GetModelCommand = "GetModel";

		public const string GetReportAndModelsCommand = "GetReportAndModels";

		public const string GetExternalImagesCommand = "GetExternalImages";

		public const string ExecuteQueriesCommand = "ExecuteQueries";

		public const string LogClientTraceEventsCommand = "LogClientTraceEvents";

		public const string CloseSessionCommand = "CloseSession";

		public const string CancelProgressiveSessionJobsCommand = "CancelProgressiveSessionJobs";

		public const string PowerViewCloseSession = "CloseSession";

		public const string PowerViewOpenSession = "OpenSession";

		public const string PowerViewLoadReport = "LoadReport";

		public const string PowerViewLogClientActivities = "LogClientActivities";

		public const string PowerViewLogClientTraces = "LogClientTraces";

		public const string LoadDocument = "LoadDocument";

		public const string SaveReport = "SaveReport";

		public const string ExecuteCommands = "ExecuteCommands";

		public const string ExecuteSemanticQuery = "ExecuteSemanticQuery";

		public const string GetDocument = "GetDocument";

		public const string GetEntityDataModel = "GetEntityDataModel";

		public const string GetVisual = "GetVisual";

		public const string GetSemanticQuery = "GetSemanticQuery";

		public const string Height = "Height";

		public const string Width = "Width";

		public const string VisualName = "VisualName";

		public const string SheetName = "SheetName";

		public const string IsNew = "IsNew";

		public const string SheetReportSectionMapping = "SheetReportSectionMapping";

		public const string ReportContentType = "ReportContentType";

		public const string Mode = "Mode";

		public const string ModelId = "ModelId";

		public const string IsCloudRlsEnabled = "IsCloudRlsEnabled";

		public const string UserPrincipalName = "UserPrincipalName";

		public const string IsCloudRoleLevelSecurityMembershipEnabled = "IsCloudRoleLevelSecurityMembershipEnabled";

		public const string ImpersonatedUserPrincipalName = "ImpersonatedUserPrincipalName";

		public const string ImpersonatedRoles = "ImpersonatedRoles";

		public const string TenantId = "TenantId";

		public const string ExecuteDaxQuery = "ExecuteDaxQuery";

		public static string PBIDeviceInfoStringFormat;

		private static readonly Dictionary<string, bool> m_crescentCommands;

		private static readonly Dictionary<string, HttpMethod> m_powerViewCommands;

		private static readonly List<string> m_powerViewServerVrmCommands;

		public NameValueCollection RenderingParameters => m_renderingParameters;

		public NameValueCollection ReportParameters => m_reportParameters;

		public string ReportParametersXml
		{
			get
			{
				StringWriter stringWriter = new StringWriter(CultureInfo.InvariantCulture);
				XmlTextWriter xmlTextWriter = new XmlTextWriter(stringWriter);
				xmlTextWriter.Formatting = Formatting.Indented;
				xmlTextWriter.WriteStartElement("Parameters");
				for (int i = 0; i < m_reportParameters.Count; i++)
				{
					xmlTextWriter.WriteStartElement("Parameter");
					string key = m_reportParameters.GetKey(i);
					if (key != null)
					{
						xmlTextWriter.WriteElementString("Name", key);
					}
					string[] values = m_reportParameters.GetValues(i);
					xmlTextWriter.WriteStartElement("Values");
					if (values != null)
					{
						for (int j = 0; j < values.Length; j++)
						{
							xmlTextWriter.WriteElementString("Value", values[j]);
						}
					}
					xmlTextWriter.WriteEndElement();
					xmlTextWriter.WriteEndElement();
				}
				xmlTextWriter.WriteEndElement();
				return stringWriter.ToString();
			}
		}

		public NameValueCollection CatalogParameters => m_catalogParameters;

		public string FormatParamValue
		{
			get
			{
				if (CatalogParameters != null)
				{
					return CatalogParameters["Format"];
				}
				return null;
			}
			set
			{
				CatalogParameters["Format"] = value;
			}
		}

		public string SessionIDParamValue
		{
			get
			{
				return CatalogParameters["SessionID"];
			}
			set
			{
				CatalogParameters["SessionID"] = value;
			}
		}

		public string ImageIDParamValue
		{
			get
			{
				return CatalogParameters["ImageID"];
			}
			set
			{
				CatalogParameters["ImageID"] = value;
			}
		}

		public string ReturnUrlValue
		{
			get
			{
				return CatalogParameters["ReturnUrl"];
			}
			set
			{
				CatalogParameters["ReturnUrl"] = value;
			}
		}

		public string SortIdParamValue
		{
			get
			{
				return CatalogParameters["SortId"];
			}
			set
			{
				CatalogParameters["SortId"] = value;
			}
		}

		public string ShowHideToggleParamValue
		{
			get
			{
				return CatalogParameters["ShowHideToggle"];
			}
			set
			{
				CatalogParameters["ShowHideToggle"] = value;
			}
		}

		public string SnapshotParamValue
		{
			get
			{
				return CatalogParameters["Snapshot"];
			}
			set
			{
				CatalogParameters["Snapshot"] = value;
			}
		}

		public string ClearSessionParamValue
		{
			get
			{
				return CatalogParameters["ClearSession"];
			}
			set
			{
				CatalogParameters["ClearSession"] = value;
			}
		}

		public string AllowNewSessionsParamValue
		{
			get
			{
				return CatalogParameters["AllowNewSessions"];
			}
			set
			{
				CatalogParameters["AllowNewSessions"] = value;
			}
		}

		public string CommandParamValue
		{
			get
			{
				return CatalogParameters["Command"];
			}
			set
			{
				CatalogParameters["Command"] = value;
			}
		}

		public string EntityIdValue => CatalogParameters["EntityID"];

		public string DrillTypeValue => CatalogParameters["DrillType"];

		public string PaginationModeValue
		{
			get
			{
				return CatalogParameters["PageCountMode"];
			}
			set
			{
				CatalogParameters["PageCountMode"] = value;
			}
		}

		public DatasourceCredentialsCollection DatasourcesCred
		{
			get
			{
				return m_datasourcesCred;
			}
			set
			{
				m_datasourcesCred = value;
			}
		}

		public NameValueCollection BrowserCapabilities => m_browserCapabilities;

		public string ServerVirtualRoot
		{
			get
			{
				return m_ServerVirtualRoot;
			}
			set
			{
				m_ServerVirtualRoot = value;
			}
		}

		public string SessionId
		{
			get
			{
				return m_SessionId;
			}
			set
			{
				m_SessionId = value;
			}
		}

		string IReportParameterLookup.GetReportParamsInstanceId(NameValueCollection reportParameters)
		{
			if (m_reverseLookupParameters == null)
			{
				return null;
			}
			ReportParameterCollection key = new ReportParameterCollection(reportParameters);
			return (string)m_reverseLookupParameters[key];
		}

		public static string GetFallbackFormat()
		{
			return "HTML5";
		}

		public void ParseQueryString(NameValueCollection allParametersCollection, IParametersTranslator paramsTranslator, ExternalItemPath itemPath)
		{
			ParseQueryString(itemPath, paramsTranslator, allParametersCollection, out m_catalogParameters, out m_renderingParameters, out m_reportParameters, out m_datasourcesCred, out m_reverseLookupParameters);
			ApplyDefaultRenderingParameters();
		}

		public string GetImageUrl(bool useSessionId, string imageId, ICatalogItemContext ctx)
		{
			string text = null;
			if (m_renderingParameters != null)
			{
				text = m_renderingParameters["StreamRoot"];
			}
			if (text != null && text != string.Empty)
			{
				StringBuilder stringBuilder = new StringBuilder(text);
				if (imageId != null)
				{
					stringBuilder.Append(imageId);
				}
				return stringBuilder.ToString();
			}
			CatalogItemUrlBuilder catalogItemUrlBuilder = new CatalogItemUrlBuilder(ctx);
			string snapshotParamValue = SnapshotParamValue;
			if (snapshotParamValue != null)
			{
				catalogItemUrlBuilder.AppendCatalogParameter("Snapshot", snapshotParamValue);
			}
			string sessionIDParamValue = SessionIDParamValue;
			if (sessionIDParamValue != null)
			{
				catalogItemUrlBuilder.AppendCatalogParameter("SessionID", sessionIDParamValue);
			}
			else if (useSessionId && m_SessionId != null)
			{
				catalogItemUrlBuilder.AppendCatalogParameter("SessionID", m_SessionId);
			}
			string formatParamValue = FormatParamValue;
			if (formatParamValue != null)
			{
				catalogItemUrlBuilder.AppendCatalogParameter("Format", formatParamValue);
			}
			catalogItemUrlBuilder.AppendCatalogParameter("ImageID", imageId);
			return catalogItemUrlBuilder.ToString();
		}

		public static NameValueCollection ExtractReportParameters(NameValueCollection allParametersCollection, ref bool[] whichParamsAreShared, out NameValueCollection otherParameters)
		{
			NameValueCollection nameValueCollection = new NameValueCollection();
			otherParameters = new NameValueCollection();
			List<bool> list = new List<bool>();
			for (int i = 0; i < allParametersCollection.Count; i++)
			{
				string text = allParametersCollection.GetKey(i);
				string[] array = allParametersCollection.GetValues(i);
				if (array == null || text == null)
				{
					continue;
				}
				if (StringSupport.EndsWith(text, ":isnull", ignoreCase: true, CultureInfo.InvariantCulture))
				{
					text = text.Substring(0, text.Length - ":isnull".Length);
					array = new string[1];
				}
				if (StringSupport.EndsWith(text, ":isnull", ignoreCase: true, CultureInfo.InvariantCulture))
				{
					text = text.Substring(0, text.Length - ":isnull".Length);
					array = new string[1];
				}
				if (StringSupport.StartsWith(text, "rs:", ignoreCase: true, CultureInfo.InvariantCulture) || StringSupport.StartsWith(text, "rc:", ignoreCase: true, CultureInfo.InvariantCulture) || StringSupport.StartsWith(text, "dsu:", ignoreCase: true, CultureInfo.InvariantCulture) || StringSupport.StartsWith(text, "dsp:", ignoreCase: true, CultureInfo.InvariantCulture))
				{
					if (!TryToAddToCollection(text, array, null, allowMultiValue: false, otherParameters))
					{
						throw new InternalCatalogException("expected to add parameter to collection" + text.MarkAsUserContent());
					}
					continue;
				}
				if (!TryToAddToCollection(text, array, "", allowMultiValue: true, nameValueCollection))
				{
					throw new InternalCatalogException("expected to add parameter to collection" + text.MarkAsUserContent());
				}
				if (whichParamsAreShared != null && whichParamsAreShared.Length != 0)
				{
					list.Add(whichParamsAreShared[i]);
				}
			}
			if (whichParamsAreShared != null && whichParamsAreShared.Length != 0)
			{
				whichParamsAreShared = list.ToArray();
			}
			return nameValueCollection;
		}

		public static NameValueCollection ShallowXmlToNameValueCollection(string xml, string topElementTag)
		{
			NameValueCollection nameValueCollection = new NameValueCollection();
			if (xml == null || xml == string.Empty)
			{
				return nameValueCollection;
			}
			XmlTextReader xmlTextReader = XmlUtil.SafeCreateXmlTextReader(xml);
			try
			{
				xmlTextReader.MoveToContent();
				if (xmlTextReader.NodeType != XmlNodeType.Element || string.Compare(xmlTextReader.Name, topElementTag, StringComparison.Ordinal) != 0)
				{
					throw new InvalidXmlException();
				}
				while (xmlTextReader.Read())
				{
					if (xmlTextReader.IsStartElement())
					{
						bool isEmptyElement = xmlTextReader.IsEmptyElement;
						string name = xmlTextReader.Name;
						name = XmlUtil.DecodePropertyName(name);
						string value = xmlTextReader.ReadString();
						if (nameValueCollection.GetValues(name) != null)
						{
							throw new InvalidXmlException();
						}
						nameValueCollection[name] = value;
						if (!isEmptyElement && xmlTextReader.IsStartElement())
						{
							throw new InvalidXmlException();
						}
					}
				}
				return nameValueCollection;
			}
			catch (XmlException ex)
			{
				throw new MalformedXmlException(ex);
			}
		}

		public static NameValueCollection DeepXmlToNameValueCollection(string xml, string topElementTag, string eachElementTag, string nameElementTag, string valueElementTag)
		{
			NameValueCollection nameValueCollection = new NameValueCollection(StringComparer.InvariantCulture);
			if (xml == null || xml == string.Empty)
			{
				return nameValueCollection;
			}
			XmlTextReader xmlTextReader = XmlUtil.SafeCreateXmlTextReader(xml);
			try
			{
				xmlTextReader.MoveToContent();
				if (xmlTextReader.NodeType != XmlNodeType.Element || string.Compare(xmlTextReader.Name, topElementTag, StringComparison.Ordinal) != 0)
				{
					throw new InvalidXmlException();
				}
				while (xmlTextReader.Read())
				{
					if (!xmlTextReader.IsStartElement())
					{
						continue;
					}
					if (xmlTextReader.IsEmptyElement || string.Compare(xmlTextReader.Name, eachElementTag, StringComparison.Ordinal) != 0)
					{
						throw new InvalidXmlException();
					}
					xmlTextReader.Read();
					string text = null;
					string value = null;
					while (xmlTextReader.IsStartElement())
					{
						bool isEmptyElement = xmlTextReader.IsEmptyElement;
						string name = xmlTextReader.Name;
						string text2 = xmlTextReader.ReadString();
						if (string.Compare(name, nameElementTag, StringComparison.Ordinal) == 0)
						{
							text = text2;
						}
						else if (string.Compare(name, valueElementTag, StringComparison.Ordinal) == 0)
						{
							value = text2;
						}
						if (!isEmptyElement)
						{
							xmlTextReader.ReadEndElement();
						}
						else
						{
							xmlTextReader.Read();
						}
					}
					if (text == null)
					{
						throw new InvalidXmlException();
					}
					nameValueCollection.Add(text, value);
				}
				return nameValueCollection;
			}
			catch (XmlException ex)
			{
				throw new MalformedXmlException(ex);
			}
		}

		public static string NameValueCollectionToShallowXml(NameValueCollection parameters, string topElementTag)
		{
			StringWriter stringWriter = new StringWriter(CultureInfo.InvariantCulture);
			XmlTextWriter xmlTextWriter = new XmlTextWriter(stringWriter);
			xmlTextWriter.Formatting = Formatting.Indented;
			xmlTextWriter.WriteStartElement(topElementTag);
			for (int i = 0; i < parameters.Count; i++)
			{
				string key = parameters.GetKey(i);
				string text = parameters.Get(i);
				if (key != null && text != null)
				{
					if (string.IsNullOrEmpty(key))
					{
						throw new InternalCatalogException("Empty Property Name");
					}
					string text2 = XmlUtil.EncodePropertyName(key);
					RSTrace.CatalogTrace.Assert(!string.IsNullOrEmpty(text2), "encodedName");
					xmlTextWriter.WriteStartElement(text2);
					xmlTextWriter.WriteString(text);
					xmlTextWriter.WriteEndElement();
				}
			}
			xmlTextWriter.WriteEndElement();
			return stringWriter.ToString();
		}

		public static string NameValueCollectionToDeepXml(NameValueCollection parameters, string topElementTag, string eachElementTag, string nameElementTag, string valueElementTag)
		{
			StringWriter stringWriter = new StringWriter(CultureInfo.InvariantCulture);
			XmlTextWriter xmlTextWriter = new XmlTextWriter(stringWriter);
			xmlTextWriter.Formatting = Formatting.Indented;
			xmlTextWriter.WriteStartElement(topElementTag);
			for (int i = 0; i < parameters.Count; i++)
			{
				xmlTextWriter.WriteStartElement(eachElementTag);
				string key = parameters.GetKey(i);
				if (key != null)
				{
					xmlTextWriter.WriteElementString(nameElementTag, key);
				}
				string text = parameters.Get(i);
				if (text != null)
				{
					xmlTextWriter.WriteElementString(valueElementTag, text);
				}
				xmlTextWriter.WriteEndElement();
			}
			xmlTextWriter.WriteEndElement();
			return stringWriter.ToString();
		}

		public void SetRenderingParameters(string renderingParametersXml)
		{
			m_renderingParameters = ShallowXmlToNameValueCollection(renderingParametersXml, "DeviceInfo");
			ApplyDefaultRenderingParameters();
		}

		public void SetRenderingParameters(NameValueCollection otherParams)
		{
			if (otherParams != null)
			{
				m_renderingParameters = new NameValueCollection(otherParams);
			}
			else
			{
				m_renderingParameters = new NameValueCollection();
			}
		}

		protected abstract void ApplyDefaultRenderingParameters();

		public void SetReportParameters(string reportParametersXml)
		{
			m_reportParameters = DeepXmlToNameValueCollection(reportParametersXml, "Parameters", "Parameter", "Name", "Value");
		}

		public void SetReportParameters(NameValueCollection allReportParameters)
		{
			m_reportParameters = allReportParameters;
			if (m_reportParameters == null)
			{
				m_reportParameters = new NameValueCollection();
			}
		}

		public void SetReportParameters(NameValueCollection allReportParameters, IParametersTranslator paramsTranslator)
		{
			if (allReportParameters != null)
			{
				string text = allReportParameters["rs:StoredParametersID"];
				if (text != null)
				{
					paramsTranslator.GetParamsInstance(text, out ExternalItemPath _, out NameValueCollection parameters);
					if (parameters == null)
					{
						throw new StoredParameterNotFoundException(text.MarkAsPrivate());
					}
					NameValueCollection nameValueCollection = new NameValueCollection();
					foreach (string item in parameters)
					{
						string[] values = parameters.GetValues(item);
						TryToAddToCollection(item, values, "", allowMultiValue: true, nameValueCollection);
					}
					m_reportParameters = nameValueCollection;
					return;
				}
			}
			SetReportParameters(allReportParameters);
		}

		public void SetCatalogParameters(string catalogParametersXml)
		{
			m_catalogParameters = ShallowXmlToNameValueCollection(catalogParametersXml, "Parameters");
		}

		public void DetectFormatIfNotPresent()
		{
			GuessFormatIfNotPresent(m_catalogParameters);
		}

		public void SetBrowserCapabilities(NameValueCollection browserCapabilities)
		{
			m_browserCapabilities = browserCapabilities;
		}

		public static bool HasPrefix(string name, string prefix, out string unprefixedName)
		{
			unprefixedName = name;
			if (prefix != null)
			{
				if (prefix.Length != 0)
				{
					if (!name.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
					{
						return false;
					}
					unprefixedName = name.Substring(prefix.Length);
				}
				else if (name.IndexOf(':') >= 0)
				{
					return false;
				}
			}
			return true;
		}

		public NameValueCollection GetAllParameters()
		{
			NameValueCollection nameValueCollection = new NameValueCollection();
			if (m_catalogParameters != null)
			{
				foreach (string catalogParameter in m_catalogParameters)
				{
					nameValueCollection["rs:" + catalogParameter] = m_catalogParameters[catalogParameter];
				}
			}
			if (m_reportParameters != null)
			{
				foreach (string reportParameter in m_reportParameters)
				{
					nameValueCollection[reportParameter] = m_reportParameters[reportParameter];
				}
			}
			if (m_renderingParameters != null)
			{
				foreach (string renderingParameter in m_renderingParameters)
				{
					nameValueCollection["rc:" + renderingParameter] = m_renderingParameters[renderingParameter];
				}
				return nameValueCollection;
			}
			return nameValueCollection;
		}

		private static void ResolveServerParameters(IParametersTranslator paramsTranslator, NameValueCollection allParametersCollection, NameValueCollection rsParameters, NameValueCollection rcParameters, NameValueCollection dsuParameters, NameValueCollection dspParameters, NameValueCollection reportParameters, out Hashtable reverseLookup, out ExternalItemPath itemPath)
		{
			reverseLookup = new Hashtable();
			itemPath = null;
			StringCollection stringCollection = new StringCollection();
			for (int i = 0; i < allParametersCollection.Count; i++)
			{
				string key = allParametersCollection.GetKey(i);
				if (key == null || StringComparer.OrdinalIgnoreCase.Compare(key, "rs:StoredParametersID") != 0)
				{
					continue;
				}
				string text = allParametersCollection[i];
				paramsTranslator.GetParamsInstance(text, out itemPath, out NameValueCollection parameters);
				if (parameters == null)
				{
					throw new StoredParameterNotFoundException(text.MarkAsPrivate());
				}
				reverseLookup.Add(new ReportParameterCollection(parameters), text);
				stringCollection.Add(key);
				foreach (string item in parameters)
				{
					string[] values = parameters.GetValues(item);
					if (!TryToAddToCollection(item, values, "rs:", allowMultiValue: false, rsParameters) && !TryToAddToCollection(item, values, "rc:", allowMultiValue: false, rcParameters) && !TryToAddToCollection(item, values, "dsu:", allowMultiValue: false, dsuParameters) && !TryToAddToCollection(item, values, "dsp:", allowMultiValue: false, dspParameters))
					{
						TryToAddToCollection(item, values, "", allowMultiValue: true, reportParameters);
					}
				}
			}
			StringEnumerator enumerator2 = stringCollection.GetEnumerator();
			try
			{
				while (enumerator2.MoveNext())
				{
					string current = enumerator2.Current;
					allParametersCollection.Remove(current);
				}
			}
			finally
			{
				(enumerator2 as IDisposable)?.Dispose();
			}
		}

		private static void ParseQueryString(ExternalItemPath itemPath, IParametersTranslator paramsTranslator, NameValueCollection allParametersCollection, out NameValueCollection rsParameters, out NameValueCollection rcParameters, out NameValueCollection reportParameters, out DatasourceCredentialsCollection dsParameters, out Hashtable reverseLookup)
		{
			dsParameters = null;
			reverseLookup = null;
			rsParameters = new NameValueCollection();
			rcParameters = new NameValueCollection();
			reportParameters = new NameValueCollection();
			NameValueCollection nameValueCollection = new NameValueCollection();
			NameValueCollection nameValueCollection2 = new NameValueCollection();
			ExternalItemPath itemPath2 = null;
			if (allParametersCollection == null)
			{
				return;
			}
			ResolveServerParameters(paramsTranslator, allParametersCollection, rsParameters, rcParameters, nameValueCollection, nameValueCollection2, reportParameters, out reverseLookup, out itemPath2);
			if (itemPath2 != null && Localization.CatalogCultureCompare(itemPath.Value, itemPath2.Value) != 0)
			{
				rsParameters = new NameValueCollection();
				rcParameters = new NameValueCollection();
				nameValueCollection = new NameValueCollection();
				nameValueCollection2 = new NameValueCollection();
				reportParameters = new NameValueCollection();
				reverseLookup = null;
				if (RSTrace.CatalogTrace.TraceInfo)
				{
					string message = string.Format(CultureInfo.InvariantCulture, "Requested item path '{0}' doesn't match stored parameters path '{1}'.", itemPath.Value.MarkAsPrivate(), itemPath2.Value.MarkAsPrivate());
					RSTrace.CatalogTrace.Trace(TraceLevel.Info, message);
				}
			}
			for (int i = 0; i < allParametersCollection.Count; i++)
			{
				string text = allParametersCollection.GetKey(i);
				string[] array = allParametersCollection.GetValues(i);
				if (array != null && text != null)
				{
					if (StringSupport.EndsWith(text, ":isnull", ignoreCase: true, CultureInfo.InvariantCulture))
					{
						text = text.Substring(0, text.Length - ":isnull".Length);
						array = new string[1];
					}
					if (!TryToAddToCollection(text, array, "rs:", allowMultiValue: false, rsParameters) && !TryToAddToCollection(text, array, "rc:", allowMultiValue: false, rcParameters) && !TryToAddToCollection(text, array, "dsu:", allowMultiValue: false, nameValueCollection) && !TryToAddToCollection(text, array, "dsp:", allowMultiValue: false, nameValueCollection2))
					{
						TryToAddToCollection(text, array, "", allowMultiValue: true, reportParameters);
					}
				}
			}
			dsParameters = new DatasourceCredentialsCollection(nameValueCollection, nameValueCollection2);
		}

		private static bool TryToAddToCollection(string name, string[] val, string prefix, bool allowMultiValue, NameValueCollection collection)
		{
			if (!HasPrefix(name, prefix, out string unprefixedName))
			{
				return false;
			}
			if (!allowMultiValue)
			{
				if (val.Length > 1)
				{
					return true;
				}
				if (collection.GetValues(unprefixedName) == null)
				{
					collection[unprefixedName] = val[0];
					return true;
				}
				if (val[0] == null)
				{
					collection[unprefixedName] = null;
					return true;
				}
				return true;
			}
			foreach (string value in val)
			{
				collection.Add(unprefixedName, value);
			}
			return true;
		}

		private static void GuessFormatIfNotPresent(NameValueCollection catalogParameters)
		{
			if (catalogParameters["Format"] == null)
			{
				catalogParameters["Format"] = GetFallbackFormat();
			}
		}

		static RSRequestParameters()
		{
			PBIDeviceInfoStringFormat = "<DeviceInfo>\r\n                        <UseFullUrls>True</UseFullUrls>\r\n                        <PageHeight>3.125in</PageHeight>\r\n                        <PageWidth>5.3125in</PageWidth>\r\n                        <ActiveXControls>False</ActiveXControls>\r\n                        <OutputFormat>PNG</OutputFormat>                   \r\n                        <ReportItemPath>{0}</ReportItemPath>\r\n                    </DeviceInfo>";
			m_crescentCommands = new Dictionary<string, bool>(8);
			m_powerViewCommands = new Dictionary<string, HttpMethod>(6);
			m_powerViewServerVrmCommands = new List<string>();
			m_crescentCommands.Add("RenderEdit", value: true);
			m_crescentCommands.Add("GetModel", value: true);
			m_crescentCommands.Add("ExecuteQueries", value: true);
			m_crescentCommands.Add("LogClientTraceEvents", value: true);
			m_crescentCommands.Add("CloseSession", value: true);
			m_crescentCommands.Add("CancelProgressiveSessionJobs", value: true);
			m_crescentCommands.Add("GetExternalImages", value: true);
			m_crescentCommands.Add("GetReportAndModels", value: true);
			m_powerViewCommands.Add("CloseSession", HttpMethod.POST);
			m_powerViewCommands.Add("OpenSession", HttpMethod.POST);
			m_powerViewCommands.Add("LogClientActivities", HttpMethod.POST);
			m_powerViewCommands.Add("LogClientTraces", HttpMethod.POST);
			m_powerViewCommands.Add("ExecuteCommands", HttpMethod.POST);
			m_powerViewCommands.Add("LoadDocument", HttpMethod.POST);
			m_powerViewCommands.Add("LoadReport", HttpMethod.POST);
			m_powerViewCommands.Add("GetVisual", HttpMethod.GET);
			m_powerViewCommands.Add("SaveReport", HttpMethod.POST);
			m_powerViewCommands.Add("ExecuteSemanticQuery", HttpMethod.POST);
			m_powerViewCommands.Add("GetEntityDataModel", HttpMethod.GET);
			m_powerViewCommands.Add("GetSemanticQuery", HttpMethod.POST);
			m_powerViewCommands.Add("GetDocument", HttpMethod.GET);
			m_powerViewServerVrmCommands.Add("ExecuteCommands");
			m_powerViewServerVrmCommands.Add("LoadDocument");
			m_powerViewServerVrmCommands.Add("LoadReport");
			m_powerViewServerVrmCommands.Add("GetVisual");
			m_powerViewServerVrmCommands.Add("ExecuteSemanticQuery");
			m_powerViewServerVrmCommands.Add("SaveReport");
			m_powerViewServerVrmCommands.Add("GetSemanticQuery");
			m_powerViewServerVrmCommands.Add("GetDocument");
		}

		internal static bool IsCrescentCommand(string command)
		{
			if (string.IsNullOrEmpty(command))
			{
				return false;
			}
			return m_crescentCommands.ContainsKey(command);
		}

		internal static bool IsPowerViewCommand(string command)
		{
			if (string.IsNullOrEmpty(command))
			{
				return false;
			}
			return m_powerViewCommands.ContainsKey(command);
		}

		internal static bool IsPowerViewVrmCommand(string command)
		{
			if (string.IsNullOrEmpty(command))
			{
				return false;
			}
			return m_powerViewServerVrmCommands.Contains(command);
		}

		internal static bool IsPOSTOnlyCommand(string command)
		{
			if (!IsPowerViewCommand(command))
			{
				return false;
			}
			return m_powerViewCommands[command] == HttpMethod.POST;
		}
	}
}
