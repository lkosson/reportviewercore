using Microsoft.ReportingServices.Diagnostics;
using Microsoft.ReportingServices.Diagnostics.Utilities;
using Microsoft.ReportingServices.ReportProcessing;
using System;
using System.Globalization;
using System.IO;
using System.Runtime.Serialization;
using System.Security;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;

namespace Microsoft.ReportingServices.DataExtensions
{
	[Serializable]
	internal sealed class DataSourceInfo
	{
		public enum CredentialsRetrievalOption
		{
			Unknown,
			Prompt,
			Store,
			Integrated,
			None,
			ServiceAccount,
			SecureStore
		}

		[Flags]
		private enum DataSourceFlags
		{
			Enabled = 0x1,
			ReferenceIsValid = 0x2,
			ImpersonateUser = 0x4,
			WindowsCredentials = 0x8,
			IsModel = 0x10,
			ConnectionStringUseridReference = 0x20
		}

		private const DataSourceFlags DefaultFlags = DataSourceFlags.Enabled | DataSourceFlags.ReferenceIsValid;

		internal const string ExtensionXmlTag = "Extension";

		internal const string ConnectionStringXmlTag = "ConnectString";

		internal const string UseOriginalConnectStringXmlTag = "UseOriginalConnectString";

		internal const string OriginalConnectStringExpressionBasedXmlTag = "OriginalConnectStringExpressionBased";

		internal const string CredentialRetrievalXmlTag = "CredentialRetrieval";

		internal const string ImpersonateUserXmlTag = "ImpersonateUser";

		internal const string PromptXmlTag = "Prompt";

		internal const string WindowsCredentialsXmlTag = "WindowsCredentials";

		internal const string UserNameXmlTag = "UserName";

		internal const string PasswordXmlTag = "Password";

		internal const string EnabledXmlTag = "Enabled";

		internal const string NameXmlTag = "Name";

		internal const string SecureStoreLookupXmlTag = "SecureStoreLookup";

		internal const string TargetApplicationIdXmlTag = "TargetApplicationId";

		internal const string DataSourcesXmlTag = "DataSources";

		internal const string DataSourceXmlTag = "DataSource";

		internal const string DataSourceDefinitionXmlTag = "DataSourceDefinition";

		internal const string m_dataSourceReferenceXmlTag = "DataSourceReference";

		internal const string InvalidDataSourceReferenceXmlTag = "InvalidDataSourceReference";

		internal const string XmlNameSpace = "http://schemas.microsoft.com/sqlserver/reporting/2006/03/reportdatasource";

		private Guid m_id;

		private string m_name;

		private string m_originalName;

		private string m_extension;

		private byte[] m_connectionStringEncrypted;

		private byte[] m_originalConnectionStringEncrypted;

		private bool m_originalConnectStringExpressionBased;

		private string m_dataSourceReference;

		private Guid m_linkID;

		private Guid m_DataSourceWithCredentialsId;

		private byte[] m_secDesc;

		private CredentialsRetrievalOption m_credentialsRetrieval;

		private string m_prompt;

		private byte[] m_userNameEncrypted;

		private byte[] m_passwordEncrypted;

		private DataSourceFlags m_flags;

		private Guid m_modelID = Guid.Empty;

		private DateTime? m_modelLastUpdatedTime;

		private bool m_isEmbeddedInModel;

		private bool m_isModelSecurityUsed;

		private string m_tenantName;

		[NonSerialized]
		private bool m_isExternalDataSource;

		[NonSerialized]
		private bool m_isFullyFormedExternalDataSource;

		[NonSerialized]
		private bool m_isMultidimensional;

		[NonSerialized]
		private IServiceEndpoint m_serviceEndpoint;

		[NonSerialized]
		private SecureStringWrapper m_passwordSecureString;

		[NonSerialized]
		private SecureStoreLookup m_secureStoreLookup;

		[NonSerialized]
		private DataSourceFaultContext m_dataSourceFaultContext;

		[NonSerialized]
		private string m_modelPerspectiveName;

		[NonSerialized]
		private static Regex m_useridDetectionRegex;

		[NonSerialized]
		private const RegexOptions CompiledRegexOptions = RegexOptions.ExplicitCapture | RegexOptions.Compiled | RegexOptions.Singleline;

		[NonSerialized]
		private const string UseridPattern = "{{[\\s]*[uU][sS][eE][rR][iI][dD][\\s]*}}";

		public Guid ID
		{
			get
			{
				return m_id;
			}
			set
			{
				m_id = value;
			}
		}

		public Guid DataSourceWithCredentialsID
		{
			get
			{
				return m_DataSourceWithCredentialsId;
			}
			set
			{
				m_DataSourceWithCredentialsId = value;
			}
		}

		public string Name
		{
			get
			{
				return m_name;
			}
			set
			{
				m_name = value;
			}
		}

		public string PromptIdentifier => OriginalName;

		public string OriginalName
		{
			get
			{
				return m_originalName;
			}
			set
			{
				m_originalName = value;
			}
		}

		public string Extension
		{
			get
			{
				return m_extension;
			}
			set
			{
				m_extension = value;
			}
		}

		public byte[] ConnectionStringEncrypted => m_connectionStringEncrypted;

		public bool UseOriginalConnectionString => m_connectionStringEncrypted == null;

		public byte[] OriginalConnectionStringEncrypted => m_originalConnectionStringEncrypted;

		public bool OriginalConnectStringExpressionBased => m_originalConnectStringExpressionBased;

		internal bool IsExternalDataSource
		{
			get
			{
				return m_isExternalDataSource;
			}
			set
			{
				m_isExternalDataSource = value;
			}
		}

		internal bool IsFullyFormedExternalDataSource
		{
			get
			{
				return m_isFullyFormedExternalDataSource;
			}
			set
			{
				m_isFullyFormedExternalDataSource = value;
			}
		}

		internal bool IsMultiDimensional
		{
			get
			{
				return m_isMultidimensional;
			}
			set
			{
				m_isMultidimensional = value;
			}
		}

		public string DataSourceReference
		{
			get
			{
				return m_dataSourceReference;
			}
			set
			{
				m_dataSourceReference = value;
			}
		}

		public Guid LinkID => m_linkID;

		public bool ReferenceByPath
		{
			get
			{
				if (DataSourceReference != null && LinkID == Guid.Empty)
				{
					return ReferenceIsValid;
				}
				return false;
			}
		}

		public bool IsReference
		{
			get
			{
				if (DataSourceReference == null && !(LinkID != Guid.Empty))
				{
					return !ReferenceIsValid;
				}
				return true;
			}
		}

		public byte[] SecurityDescriptor => m_secDesc;

		public CredentialsRetrievalOption CredentialsRetrieval
		{
			get
			{
				return m_credentialsRetrieval;
			}
			set
			{
				m_credentialsRetrieval = value;
			}
		}

		public bool ImpersonateUser
		{
			get
			{
				return (m_flags & DataSourceFlags.ImpersonateUser) != 0;
			}
			set
			{
				if (value)
				{
					m_flags |= DataSourceFlags.ImpersonateUser;
				}
				else
				{
					m_flags &= ~DataSourceFlags.ImpersonateUser;
				}
			}
		}

		public string Prompt
		{
			get
			{
				return m_prompt;
			}
			set
			{
				m_prompt = value;
			}
		}

		public bool WindowsCredentials
		{
			get
			{
				return (m_flags & DataSourceFlags.WindowsCredentials) != 0;
			}
			set
			{
				if (value)
				{
					m_flags |= DataSourceFlags.WindowsCredentials;
				}
				else
				{
					m_flags &= ~DataSourceFlags.WindowsCredentials;
				}
			}
		}

		public byte[] UserNameEncrypted => m_userNameEncrypted;

		public byte[] PasswordEncrypted => m_passwordEncrypted;

		public SecureStoreLookup SecureStoreLookup => m_secureStoreLookup;

		public DataSourceFaultContext DataSourceFaultContext => m_dataSourceFaultContext;

		public bool IsCredentialSet
		{
			get;
			set;
		}

		public bool Enabled
		{
			get
			{
				return (m_flags & DataSourceFlags.Enabled) != 0;
			}
			set
			{
				if (value)
				{
					m_flags |= DataSourceFlags.Enabled;
				}
				else
				{
					m_flags &= ~DataSourceFlags.Enabled;
				}
			}
		}

		public bool IsModel
		{
			get
			{
				return StaticIsModel((int)m_flags);
			}
			set
			{
				if (value)
				{
					m_flags |= DataSourceFlags.IsModel;
				}
				else
				{
					m_flags &= ~DataSourceFlags.IsModel;
				}
			}
		}

		public bool IsModelSecurityUsed => m_isModelSecurityUsed;

		public IServiceEndpoint ServiceEndpoint
		{
			get
			{
				return m_serviceEndpoint;
			}
			set
			{
				m_serviceEndpoint = value;
			}
		}

		public string TenantName
		{
			get
			{
				return m_tenantName;
			}
			set
			{
				m_tenantName = value;
			}
		}

		public Guid ModelID
		{
			get
			{
				return m_modelID;
			}
			set
			{
				m_modelID = value;
			}
		}

		public DateTime? ModelLastUpdatedTime
		{
			get
			{
				return m_modelLastUpdatedTime;
			}
			set
			{
				m_modelLastUpdatedTime = value;
			}
		}

		public string ModelPerspectiveName
		{
			get
			{
				return m_modelPerspectiveName;
			}
			set
			{
				m_modelPerspectiveName = value;
			}
		}

		public bool ReferenceIsValid
		{
			get
			{
				return (m_flags & DataSourceFlags.ReferenceIsValid) != 0;
			}
			set
			{
				if (value)
				{
					m_flags |= DataSourceFlags.ReferenceIsValid;
				}
				else
				{
					m_flags &= ~DataSourceFlags.ReferenceIsValid;
				}
			}
		}

		public bool NeedPrompt
		{
			get
			{
				if (CredentialsRetrieval == CredentialsRetrievalOption.Prompt && m_userNameEncrypted == null)
				{
					return true;
				}
				return false;
			}
		}

		public int FlagsForCatalogSerialization
		{
			get
			{
				DataSourceFlags dataSourceFlags = m_flags;
				if (!m_isEmbeddedInModel)
				{
					dataSourceFlags &= ~DataSourceFlags.IsModel;
				}
				return (int)dataSourceFlags;
			}
		}

		public bool HasConnectionStringUseridReference => (m_flags & DataSourceFlags.ConnectionStringUseridReference) != 0;

		private static Regex UseridDetectionRegex
		{
			get
			{
				if (m_useridDetectionRegex == null)
				{
					m_useridDetectionRegex = new Regex("{{[\\s]*[uU][sS][eE][rR][iI][dD][\\s]*}}", RegexOptions.ExplicitCapture | RegexOptions.Compiled | RegexOptions.Singleline);
				}
				return m_useridDetectionRegex;
			}
		}

		public void LinkToStandAlone(DataSourceInfo standAlone, string standAlonePath, Guid standAloneCatalogItemId)
		{
			m_name = standAlone.m_name;
			m_extension = standAlone.m_extension;
			m_connectionStringEncrypted = standAlone.m_connectionStringEncrypted;
			m_dataSourceReference = standAlonePath;
			m_linkID = standAloneCatalogItemId;
			m_secDesc = standAlone.m_secDesc;
			m_credentialsRetrieval = standAlone.CredentialsRetrieval;
			m_prompt = standAlone.m_prompt;
			m_userNameEncrypted = standAlone.m_userNameEncrypted;
			m_passwordEncrypted = standAlone.m_passwordEncrypted;
			Enabled = standAlone.Enabled;
			ImpersonateUser = standAlone.ImpersonateUser;
			WindowsCredentials = standAlone.WindowsCredentials;
		}

		public void LinkModelToDataSource(DataSourceInfo standAlone, Guid modelID)
		{
			m_DataSourceWithCredentialsId = standAlone.m_DataSourceWithCredentialsId;
			m_extension = standAlone.m_extension;
			m_connectionStringEncrypted = standAlone.m_connectionStringEncrypted;
			m_credentialsRetrieval = standAlone.CredentialsRetrieval;
			m_prompt = standAlone.Prompt;
			m_userNameEncrypted = standAlone.m_userNameEncrypted;
			m_passwordEncrypted = standAlone.m_passwordEncrypted;
			Enabled = standAlone.Enabled;
			ImpersonateUser = standAlone.ImpersonateUser;
			m_flags = standAlone.m_flags;
			m_modelID = modelID;
			m_isEmbeddedInModel = false;
			IsModel = true;
		}

		public void InitializeAsEmbeddedInModel(Guid modelID)
		{
			m_modelID = modelID;
			m_isEmbeddedInModel = true;
			IsModel = true;
		}

		public void CopyFrom(DataSourceInfo copy, string referencePath, Guid linkToCatalogItemId, bool isEmbeddedInModel)
		{
			LinkToStandAlone(copy, referencePath, linkToCatalogItemId);
			m_flags = copy.m_flags;
			m_modelID = copy.ModelID;
			m_modelLastUpdatedTime = copy.ModelLastUpdatedTime;
			m_isEmbeddedInModel = isEmbeddedInModel;
			if (isEmbeddedInModel)
			{
				IsModel = true;
			}
		}

		public DataSourceInfo(SerializationInfo info, StreamingContext context)
		{
			m_id = (Guid)info.GetValue("id", typeof(Guid));
			m_DataSourceWithCredentialsId = (Guid)info.GetValue("originalid", typeof(Guid));
			m_name = (string)info.GetValue("name", typeof(string));
			m_originalName = (string)info.GetValue("originalname", typeof(string));
			m_extension = (string)info.GetValue("extension", typeof(string));
			m_connectionStringEncrypted = (byte[])info.GetValue("connectionstringencrypted", typeof(byte[]));
			m_originalConnectionStringEncrypted = (byte[])info.GetValue("originalconnectionstringencrypted", typeof(byte[]));
			m_originalConnectStringExpressionBased = (bool)info.GetValue("originalConnectStringExpressionBased", typeof(bool));
			m_dataSourceReference = (string)info.GetValue("datasourcereference", typeof(string));
			m_linkID = (Guid)info.GetValue("linkid", typeof(Guid));
			m_secDesc = (byte[])info.GetValue("secdesc", typeof(byte[]));
			m_credentialsRetrieval = (CredentialsRetrievalOption)info.GetValue("credentialsretrieval", typeof(CredentialsRetrievalOption));
			m_prompt = (string)info.GetValue("prompt", typeof(string));
			m_userNameEncrypted = (byte[])info.GetValue("usernameencrypted", typeof(byte[]));
			m_passwordEncrypted = (byte[])info.GetValue("passwordencrypted", typeof(byte[]));
			m_flags = (DataSourceFlags)info.GetValue("datasourceflags", typeof(DataSourceFlags));
			m_modelID = (Guid)info.GetValue("modelid", typeof(Guid));
			m_modelLastUpdatedTime = (DateTime?)info.GetValue("modellastupdatedtime", typeof(DateTime?));
			m_isEmbeddedInModel = (bool)info.GetValue("isembeddedinmodel", typeof(bool));
		}

		public static DataSourceInfo ParseDataSourceNode(XmlNode node, bool clientLoad, IDataProtection dataProtection)
		{
			return ParseDataSourceNode(node, clientLoad, allowNoName: false, dataProtection);
		}

		public static DataSourceInfo ParseDataSourceNode(XmlNode node, bool clientLoad, bool allowNoName, IDataProtection dataProtection)
		{
			if (node.Name != "DataSource")
			{
				throw new InvalidXmlException();
			}
			XmlNode xmlNode = node.SelectSingleNode("Name");
			node.SelectSingleNode("Extension");
			XmlNode xmlNode2 = node.SelectSingleNode("DataSourceDefinition");
			XmlNode xmlNode3 = node.SelectSingleNode("DataSourceReference");
			DataSourceInfo result = null;
			if ((!allowNoName && xmlNode == null) || xmlNode2 == null == (xmlNode3 == null))
			{
				bool flag = true;
				if (clientLoad && node.SelectSingleNode("InvalidDataSourceReference") != null)
				{
					flag = false;
					result = new DataSourceInfo((xmlNode == null) ? "" : xmlNode.InnerText);
				}
				if (flag)
				{
					throw new InvalidXmlException();
				}
			}
			string text = (xmlNode == null) ? "" : xmlNode.InnerText;
			if (xmlNode2 != null)
			{
				result = new DataSourceInfo(text, text, xmlNode2.OuterXml, dataProtection);
			}
			else if (xmlNode3 != null)
			{
				result = new DataSourceInfo(text, xmlNode3.InnerText, Guid.Empty);
			}
			return result;
		}

		public DataSourceInfo(string name, string originalName, string dataSourceDefinition, IDataProtection dataProtection)
			: this(name, originalName)
		{
			XmlDocument xmlDocument = new XmlDocument();
			XmlNode xmlNode = null;
			try
			{
				XmlUtil.SafeOpenXmlDocumentString(xmlDocument, dataSourceDefinition);
			}
			catch (XmlException ex)
			{
				throw new MalformedXmlException(ex);
			}
			try
			{
				xmlNode = xmlDocument.SelectSingleNode("/DataSourceDefinition");
				if (xmlNode == null)
				{
					XmlNamespaceManager xmlNamespaceManager = new XmlNamespaceManager(xmlDocument.NameTable);
					xmlNamespaceManager.AddNamespace("rds", GetXmlNamespace());
					xmlNode = xmlDocument.SelectSingleNode("/rds:" + GetDataSourceDefinitionXmlTag(), xmlNamespaceManager);
				}
			}
			catch (XmlException)
			{
				throw new InvalidXmlException();
			}
			ParseAndValidate(xmlNode, dataProtection);
		}

		public DataSourceInfo(string name, string originalName, XmlNode root, IDataProtection dataProtection)
			: this(name, originalName)
		{
			ParseAndValidate(root, dataProtection);
		}

		public DataSourceInfo(string name, string linkPath, Guid linkId, DataSourceInfo standAloneDatasource)
		{
			m_id = Guid.NewGuid();
			m_name = name;
			m_originalName = name;
			m_DataSourceWithCredentialsId = standAloneDatasource.m_DataSourceWithCredentialsId;
			InitDefaultsOnCreation();
			LinkToStandAlone(standAloneDatasource, linkPath, linkId);
			if (standAloneDatasource.IsModel)
			{
				IsModel = true;
				m_modelID = standAloneDatasource.ModelID;
			}
		}

		public DataSourceInfo(string name, string originalName)
		{
			m_id = Guid.NewGuid();
			m_name = name;
			m_originalName = originalName;
			InitDefaultsOnCreation();
		}

		public void ValidateDefinition(bool useOriginalConnectString)
		{
			if (Extension == null)
			{
				throw new MissingElementException("Extension");
			}
			if (CredentialsRetrieval != CredentialsRetrievalOption.Store && CredentialsRetrieval != CredentialsRetrievalOption.Prompt)
			{
				WindowsCredentials = false;
			}
			if (CredentialsRetrieval != CredentialsRetrievalOption.Store)
			{
				if (m_userNameEncrypted != null)
				{
					throw new InvalidElementCombinationException("UserName", "CredentialRetrieval");
				}
				if (m_passwordEncrypted != null)
				{
					throw new InvalidElementCombinationException("Password", "CredentialRetrieval");
				}
			}
			else if (m_userNameEncrypted == null)
			{
				throw new MissingElementException("UserName");
			}
			if (ImpersonateUser && CredentialsRetrieval != CredentialsRetrievalOption.Store && CredentialsRetrieval != CredentialsRetrievalOption.ServiceAccount)
			{
				throw new InvalidElementCombinationException("ImpersonateUser", "CredentialRetrieval");
			}
			if (!useOriginalConnectString && ConnectionStringEncrypted == null)
			{
				throw new MissingElementException("ConnectString");
			}
			if (CredentialsRetrieval == CredentialsRetrievalOption.Unknown)
			{
				throw new MissingElementException("CredentialRetrieval");
			}
			if (CredentialsRetrieval == CredentialsRetrievalOption.ServiceAccount && !ImpersonateUser)
			{
				throw new InvalidElementCombinationException("CredentialRetrieval", "ImpersonateUser");
			}
			if (CredentialsRetrieval == CredentialsRetrievalOption.SecureStore)
			{
				if (SecureStoreLookup == null)
				{
					throw new MissingElementException("SecureStoreLookup");
				}
				if (SecureStoreLookup.TargetApplicationId == null)
				{
					throw new MissingElementException("TargetApplicationId");
				}
			}
			if (SecureStoreLookup != null && CredentialsRetrieval != CredentialsRetrievalOption.SecureStore)
			{
				throw new InvalidElementCombinationException("SecureStoreLookup", "CredentialRetrieval");
			}
		}

		public DataSourceInfo(string originalName, string extension, string connectionString, bool integratedSecurity, string prompt, IDataProtection dataProtection)
		{
			m_id = Guid.NewGuid();
			m_name = originalName;
			m_originalName = originalName;
			InitDefaultsOnCreation();
			m_prompt = prompt;
			if (integratedSecurity)
			{
				m_credentialsRetrieval = CredentialsRetrievalOption.Integrated;
			}
			else
			{
				m_credentialsRetrieval = CredentialsRetrievalOption.Prompt;
			}
			m_extension = extension;
			SetConnectionString(connectionString, dataProtection);
		}

		public DataSourceInfo(string originalName, string extension, string connectionString, bool originalConnectStringExpressionBased, bool integratedSecurity, string prompt, IDataProtection dataProtection)
		{
			m_id = Guid.NewGuid();
			m_name = originalName;
			m_originalName = originalName;
			InitDefaultsOnCreation();
			m_prompt = prompt;
			if (integratedSecurity)
			{
				m_credentialsRetrieval = CredentialsRetrievalOption.Integrated;
			}
			else
			{
				m_credentialsRetrieval = CredentialsRetrievalOption.Prompt;
			}
			m_extension = extension;
			SetOriginalConnectionString(connectionString, dataProtection);
			m_originalConnectStringExpressionBased = originalConnectStringExpressionBased;
		}

		public DataSourceInfo(string originalName, string referenceName, Guid linkID)
		{
			m_id = Guid.NewGuid();
			m_name = originalName;
			m_originalName = originalName;
			InitDefaultsOnCreation();
			m_credentialsRetrieval = CredentialsRetrievalOption.Prompt;
			m_dataSourceReference = referenceName;
			m_linkID = linkID;
		}

		public DataSourceInfo(string originalName, string referenceName, Guid linkID, bool isEmbeddedInModel)
			: this(originalName, referenceName, linkID)
		{
			m_isEmbeddedInModel = isEmbeddedInModel;
			IsModel = true;
		}

		public DataSourceInfo(string originalName)
		{
			m_id = Guid.NewGuid();
			InitDefaultsOnCreation();
			OriginalName = originalName;
			ReferenceIsValid = false;
		}

		public DataSourceInfo(string originalName, bool isEmbeddedInModel)
			: this(originalName)
		{
			m_isEmbeddedInModel = isEmbeddedInModel;
			IsModel = true;
		}

		public static string GetDataSourceReferenceXmlTag()
		{
			return "DataSourceReference";
		}

		public static string GetUserNameXmlTag()
		{
			return "UserName";
		}

		public static string GetDataSourceDefinitionXmlTag()
		{
			return "DataSourceDefinition";
		}

		public static string GetXmlNamespace()
		{
			return "http://schemas.microsoft.com/sqlserver/reporting/2006/03/reportdatasource";
		}

		public static string GetEnabledXmlTag()
		{
			return "Enabled";
		}

		public string GetConnectionString(IDataProtection dataProtection)
		{
			return dataProtection.UnprotectDataToString(m_connectionStringEncrypted, "ConnectionString");
		}

		public string GetOriginalConnectionString(IDataProtection dataProtection)
		{
			return dataProtection.UnprotectDataToString(m_originalConnectionStringEncrypted, "OriginalConnectionString");
		}

		public void SetConnectionString(string connectionString, IDataProtection dataProtection)
		{
			SetConnectionStringUseridReference(HasUseridReference(connectionString));
			m_connectionStringEncrypted = dataProtection.ProtectData(connectionString, "ConnectionString");
		}

		private void SetOriginalConnectionString(string connectionString, IDataProtection dataProtection)
		{
			SetConnectionStringUseridReference(HasUseridReference(connectionString));
			m_originalConnectionStringEncrypted = dataProtection.ProtectData(connectionString, "OriginalConnectionString");
		}

		internal void SetOriginalConnectionString(byte[] connectionStringEncrypted)
		{
			m_originalConnectionStringEncrypted = connectionStringEncrypted;
		}

		internal void SetOriginalConnectStringExpressionBased(bool expressionBased)
		{
			m_originalConnectStringExpressionBased = expressionBased;
		}

		public string GetUserName(IDataProtection dataProtection)
		{
			return dataProtection.UnprotectDataToString(m_userNameEncrypted, "UserName");
		}

		public void SetUserName(string userName, IDataProtection dataProtection)
		{
			m_userNameEncrypted = dataProtection.ProtectData(userName, "UserName");
		}

		public string GetUserNameOnly(IDataProtection dataProtection)
		{
			return GetUserNameOnly(GetUserName(dataProtection));
		}

		public static string GetUserNameOnly(string domainAndUserName)
		{
			if (domainAndUserName == null)
			{
				return null;
			}
			int num = domainAndUserName.IndexOf("\\", StringComparison.Ordinal);
			if (num < 0)
			{
				return domainAndUserName;
			}
			return domainAndUserName.Substring(num + 1);
		}

		public string GetDomainOnly(IDataProtection dataProtection)
		{
			return GetDomainOnly(GetUserName(dataProtection));
		}

		public static string GetDomainOnly(string domainAndUserName)
		{
			if (domainAndUserName == null)
			{
				return null;
			}
			int num = domainAndUserName.IndexOf("\\", StringComparison.Ordinal);
			if (num < 0)
			{
				return null;
			}
			return domainAndUserName.Substring(0, num);
		}

		public SecureStringWrapper GetPassword(IDataProtection dataProtection)
		{
			if (m_passwordSecureString == null && m_passwordEncrypted != null)
			{
				m_passwordSecureString = new SecureStringWrapper(dataProtection.UnprotectDataToString(m_passwordEncrypted, "Password"));
			}
			return m_passwordSecureString;
		}

		public string GetPasswordDecrypted(IDataProtection dataProtection)
		{
			return GetPassword(dataProtection)?.ToString();
		}

		public void SetPassword(string password, IDataProtection dataProtection)
		{
			m_passwordEncrypted = dataProtection.ProtectData(password, "Password");
		}

		public void SetPassword(SecureString password, IDataProtection dataProtection)
		{
			m_passwordSecureString = new SecureStringWrapper(password);
		}

		public void SetPasswordFromDataSourceInfo(DataSourceInfo dsInfo)
		{
			m_passwordEncrypted = dsInfo.m_passwordEncrypted;
			if (dsInfo.m_passwordSecureString != null)
			{
				m_passwordSecureString = new SecureStringWrapper(dsInfo.m_passwordSecureString);
			}
		}

		public void ResetPassword()
		{
			m_passwordEncrypted = null;
			m_passwordSecureString = null;
		}

		public void SetSecureStoreLookupContext(SecureStoreLookup.LookupContextOptions lookupContext, string targetAppId)
		{
			m_secureStoreLookup = new SecureStoreLookup(lookupContext, targetAppId);
		}

		public void SetDataSourceFaultContext(ErrorCode errorCode, string errorString)
		{
			m_dataSourceFaultContext = new DataSourceFaultContext(errorCode, errorString);
		}

		public static bool StaticIsModel(int flags)
		{
			return (flags & 0x10) != 0;
		}

		public void ThrowIfNotUsable(ServerDataSourceSettings serverDatasourceSetting)
		{
			if (!Enabled)
			{
				throw new DataSourceDisabledException();
			}
			if (!ReferenceIsValid)
			{
				throw new InvalidDataSourceReferenceException(OriginalName);
			}
			if (!GoodForLiveExecution(serverDatasourceSetting.IsSurrogatePresent))
			{
				throw new InvalidDataSourceCredentialSettingException();
			}
			if (m_credentialsRetrieval == CredentialsRetrievalOption.Integrated && !serverDatasourceSetting.AllowIntegratedSecurity)
			{
				throw new WindowsIntegratedSecurityDisabledException();
			}
		}

		public bool GoodForLiveExecution(bool isSurrogatePresent)
		{
			if (!ReferenceIsValid || !Enabled)
			{
				return false;
			}
			if (!isSurrogatePresent && CredentialsRetrieval == CredentialsRetrievalOption.None)
			{
				return false;
			}
			return true;
		}

		public byte[] GetXmlBytes(IDataProtection dataProtection)
		{
			XmlWriterSettings xmlWriterSettings = new XmlWriterSettings();
			xmlWriterSettings.Indent = true;
			xmlWriterSettings.Encoding = Encoding.UTF8;
			MemoryStream memoryStream = new MemoryStream();
			XmlWriter xmlWriter = XmlWriter.Create(memoryStream, xmlWriterSettings);
			using (xmlWriter)
			{
				xmlWriter.WriteStartElement("DataSourceDefinition", "http://schemas.microsoft.com/sqlserver/reporting/2006/03/reportdatasource");
				xmlWriter.WriteElementString("Extension", Extension);
				xmlWriter.WriteElementString("ConnectString", GetConnectionString(dataProtection));
				xmlWriter.WriteElementString("CredentialRetrieval", CredentialsRetrieval.ToString());
				if (CredentialsRetrieval == CredentialsRetrievalOption.Prompt || CredentialsRetrieval == CredentialsRetrievalOption.Store)
				{
					xmlWriter.WriteElementString("WindowsCredentials", WindowsCredentials.ToString());
				}
				if (CredentialsRetrieval == CredentialsRetrievalOption.Prompt)
				{
					xmlWriter.WriteElementString("Prompt", string.IsNullOrEmpty(Prompt) ? "" : Prompt);
				}
				if (CredentialsRetrieval == CredentialsRetrievalOption.Store)
				{
					xmlWriter.WriteElementString("ImpersonateUser", ImpersonateUser.ToString());
				}
				xmlWriter.WriteElementString("Enabled", Enabled.ToString());
				xmlWriter.WriteEndElement();
				xmlWriter.Flush();
				return memoryStream.ToArray();
			}
		}

		private bool ParseDefinitionXml(XmlNode root, IDataProtection dataProtection)
		{
			try
			{
				if (root == null)
				{
					throw new InvalidXmlException();
				}
				string connectionString = null;
				bool flag = false;
				foreach (XmlNode childNode in root.ChildNodes)
				{
					string name = childNode.Name;
					string innerText = childNode.InnerText;
					switch (name)
					{
					case "Extension":
						Extension = innerText;
						break;
					case "ConnectString":
						connectionString = innerText;
						break;
					case "UseOriginalConnectString":
						try
						{
							flag = bool.Parse(innerText);
						}
						catch (ArgumentException)
						{
							throw new ElementTypeMismatchException("UseOriginalConnectString");
						}
						break;
					case "CredentialRetrieval":
						try
						{
							m_credentialsRetrieval = (CredentialsRetrievalOption)Enum.Parse(typeof(CredentialsRetrievalOption), innerText, ignoreCase: true);
						}
						catch (ArgumentException)
						{
							throw new ElementTypeMismatchException("CredentialRetrieval");
						}
						break;
					case "WindowsCredentials":
						try
						{
							WindowsCredentials = bool.Parse(innerText);
						}
						catch (Exception)
						{
							throw new ElementTypeMismatchException("WindowsCredentials");
						}
						break;
					case "ImpersonateUser":
						try
						{
							ImpersonateUser = bool.Parse(innerText);
						}
						catch (FormatException)
						{
							throw new ElementTypeMismatchException("ImpersonateUser");
						}
						break;
					case "Prompt":
						m_prompt = innerText;
						break;
					case "UserName":
						SetUserName(innerText, dataProtection);
						break;
					case "Password":
						SetPassword(innerText, dataProtection);
						break;
					case "Enabled":
						try
						{
							Enabled = bool.Parse(innerText);
						}
						catch (FormatException)
						{
							throw new ElementTypeMismatchException("Enabled");
						}
						break;
					default:
						throw new InvalidXmlException();
					case "OriginalConnectStringExpressionBased":
						break;
					}
				}
				if (flag)
				{
					SetConnectionString(null, dataProtection);
				}
				else
				{
					SetConnectionString(connectionString, dataProtection);
				}
				return flag;
			}
			catch (XmlException)
			{
				throw new InvalidXmlException();
			}
		}

		private void ParseAndValidate(XmlNode root, IDataProtection dataProtection)
		{
			bool useOriginalConnectString = ParseDefinitionXml(root, dataProtection);
			ValidateDefinition(useOriginalConnectString);
		}

		private void SetConnectionStringUseridReference(bool hasUseridReference)
		{
			if (hasUseridReference)
			{
				m_flags |= DataSourceFlags.ConnectionStringUseridReference;
			}
			else
			{
				m_flags &= ~DataSourceFlags.ConnectionStringUseridReference;
			}
		}

		internal static bool HasUseridReference(string connectionString)
		{
			if (string.IsNullOrEmpty(connectionString))
			{
				return false;
			}
			return UseridDetectionRegex.Matches(connectionString).Count > 0;
		}

		internal static string ReplaceAllUseridReferences(string originalConnectionString, string useridReplacementString)
		{
			if (string.IsNullOrEmpty(originalConnectionString))
			{
				return originalConnectionString;
			}
			return UseridDetectionRegex.Replace(originalConnectionString, useridReplacementString);
		}

		private void InitDefaultsOnCreation()
		{
			m_extension = null;
			m_dataSourceReference = null;
			m_linkID = Guid.Empty;
			m_credentialsRetrieval = CredentialsRetrievalOption.Unknown;
			m_prompt = string.Format(CultureInfo.CurrentCulture, RPRes.rsDataSourcePrompt);
			m_userNameEncrypted = null;
			m_passwordEncrypted = null;
			m_flags = (DataSourceFlags.Enabled | DataSourceFlags.ReferenceIsValid);
			m_originalConnectStringExpressionBased = false;
		}

		public DataSourceInfo(Guid id, Guid originalId, string name, string originalName, string extension, byte[] connectionStringEncrypted, byte[] originalConnectionStringEncypted, bool originalConnectStringExpressionBased, string dataSourceReference, Guid linkID, byte[] secDesc, CredentialsRetrievalOption credentialsRetrieval, string prompt, byte[] userNameEncrypted, byte[] passwordEncrypted, int flags, bool isModelSecurityUsed)
		{
			m_id = id;
			m_DataSourceWithCredentialsId = originalId;
			m_name = name;
			m_originalName = originalName;
			m_extension = extension;
			m_connectionStringEncrypted = connectionStringEncrypted;
			m_originalConnectionStringEncrypted = originalConnectionStringEncypted;
			m_originalConnectStringExpressionBased = originalConnectStringExpressionBased;
			m_dataSourceReference = dataSourceReference;
			m_linkID = linkID;
			m_secDesc = secDesc;
			m_credentialsRetrieval = credentialsRetrieval;
			m_prompt = prompt;
			if (m_credentialsRetrieval != CredentialsRetrievalOption.Store && (userNameEncrypted != null || passwordEncrypted != null))
			{
				throw new InternalCatalogException("unexpected data source type");
			}
			m_userNameEncrypted = userNameEncrypted;
			m_passwordEncrypted = passwordEncrypted;
			m_flags = (DataSourceFlags)flags;
			m_isModelSecurityUsed = isModelSecurityUsed;
		}
	}
}
