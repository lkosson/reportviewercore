using Microsoft.Cloud.Platform.Utils;
using Microsoft.ReportingServices.Diagnostics.Utilities;
using System;
using System.Collections.Specialized;

namespace Microsoft.ReportingServices.Diagnostics
{
	[Serializable]
	internal abstract class CatalogItemContextBase<TPathStorageType> : ICatalogItemContext
	{
		protected TPathStorageType m_reportDefinitionPath;

		protected TPathStorageType m_OriginalItemPath;

		protected TPathStorageType m_ItemPath;

		protected string m_ItemName;

		protected string m_ParentPath;

		protected CatalogItemContextBase<TPathStorageType> m_primaryContext;

		[NonSerialized]
		private RSRequestParameters m_parsedParameters;

		[NonSerialized]
		protected IPathManager m_pathManager;

		[NonSerialized]
		protected IPathTranslator m_PathTranslator;

		public RSRequestParameters RSRequestParameters
		{
			get
			{
				if (m_parsedParameters == null)
				{
					m_parsedParameters = CreateRequestParametersInstance();
				}
				return m_parsedParameters;
			}
		}

		public TPathStorageType ItemPath => m_ItemPath;

		public abstract string ItemPathAsString
		{
			get;
		}

		public virtual string HostSpecificItemPath => ItemPathAsString;

		public virtual string StableItemPath => ItemPathAsString;

		public string ItemName
		{
			get
			{
				if (m_ItemName == null)
				{
					CatalogItemNameUtility.SplitPath(ItemPathAsString, out m_ItemName, out string parentPath);
					m_ParentPath = parentPath;
				}
				return m_ItemName;
			}
		}

		public string ParentPath
		{
			get
			{
				if (m_ItemName == null)
				{
					CatalogItemNameUtility.SplitPath(ItemPathAsString, out m_ItemName, out string parentPath);
					m_ParentPath = parentPath;
				}
				return m_ParentPath;
			}
		}

		public abstract string HostRootUri
		{
			get;
		}

		public virtual IPathManager PathManager
		{
			get
			{
				if (m_pathManager == null)
				{
					m_pathManager = RSPathUtil.Instance;
				}
				return m_pathManager;
			}
		}

		protected TPathStorageType StoredItemPath => m_ItemPath;

		public TPathStorageType OriginalItemPath
		{
			get
			{
				return m_OriginalItemPath;
			}
			protected set
			{
				m_OriginalItemPath = value;
			}
		}

		public IPathTranslator PathTranslator => m_PathTranslator;

		public virtual string ReportDefinitionPath => ItemPathAsString;

		public abstract string MapUserProvidedPath(string path);

		public abstract bool IsReportServerPathOrUrl(string pathOrUrl, bool checkProtocol, out bool isRelative);

		public abstract bool IsSupportedProtocol(string path, bool checkProtocol);

		public abstract bool IsSupportedProtocol(string path, bool checkProtocol, out bool isRelative);

		public virtual string AdjustSubreportOrDrillthroughReportPath(string reportPath)
		{
			string text;
			try
			{
				text = MapUserProvidedPath(reportPath);
			}
			catch (UriFormatException)
			{
				return null;
			}
			CatalogItemContextBase<TPathStorageType> catalogItemContextBase = CreateContext(m_PathTranslator);
			if (!catalogItemContextBase.SetPath(text, ItemPathOptions.Default))
			{
				return null;
			}
			if (Localization.CatalogCultureCompare(text, catalogItemContextBase.ItemPathAsString) == 0)
			{
				return reportPath;
			}
			return catalogItemContextBase.ItemPathAsString;
		}

		public virtual ICatalogItemContext GetSubreportContext(string subreportPath)
		{
			CatalogItemContextBase<TPathStorageType> catalogItemContextBase = CreateContext(m_PathTranslator);
			InitSubreportContext(catalogItemContextBase, subreportPath);
			catalogItemContextBase.m_primaryContext = m_primaryContext;
			return catalogItemContextBase;
		}

		private void InitSubreportContext(CatalogItemContextBase<TPathStorageType> subreportContext, string subreportPath)
		{
			string path = MapUserProvidedPath(subreportPath);
			subreportContext.SetPath(path, ItemPathOptions.Validate);
			subreportContext.RSRequestParameters.SetCatalogParameters(null);
		}

		public virtual string CombineUrl(string url, bool checkProtocol, NameValueCollection unparsedParameters, out ICatalogItemContext newContext)
		{
			newContext = this;
			string text = new CatalogItemUrlBuilder(this).ToString();
			if (url == null)
			{
				return text;
			}
			if (string.Compare(url, text, StringComparison.Ordinal) == 0)
			{
				return text;
			}
			newContext = Combine(url, checkProtocol, externalFormat: true);
			if (newContext == null)
			{
				newContext = null;
				return url;
			}
			CatalogItemUrlBuilder catalogItemUrlBuilder = new CatalogItemUrlBuilder(newContext);
			if (unparsedParameters != null)
			{
				catalogItemUrlBuilder.AppendUnparsedParameters(unparsedParameters);
			}
			return catalogItemUrlBuilder.ToString();
		}

		protected abstract RSRequestParameters CreateRequestParametersInstance();

		public ICatalogItemContext Combine(string url)
		{
			return Combine(url, externalFormat: false);
		}

		public CatalogItemContextBase<TPathStorageType> Combine(string url, bool externalFormat)
		{
			return Combine(url, checkProtocol: true, externalFormat);
		}

		public CatalogItemContextBase<TPathStorageType> Combine(string url, bool checkProtocol, bool externalFormat)
		{
			if (!IsReportServerPathOrUrl(url, checkProtocol, out bool isRelative))
			{
				return null;
			}
			if (isRelative)
			{
				string text = MapUserProvidedPath(url);
				if (externalFormat && m_PathTranslator != null)
				{
					string text2 = m_PathTranslator.PathToExternal(text);
					if (text2 != null)
					{
						text = text2;
					}
				}
				CatalogItemContextBase<TPathStorageType> catalogItemContextBase = CreateContext(m_PathTranslator);
				ItemPathOptions itemPathOptions = ItemPathOptions.Validate;
				itemPathOptions = (ItemPathOptions)((int)itemPathOptions | (externalFormat ? 4 : 2));
				if (!catalogItemContextBase.SetPath(text, itemPathOptions))
				{
					throw new ItemNotFoundException(text.MarkAsPrivate());
				}
				catalogItemContextBase.RSRequestParameters.SetCatalogParameters(null);
				return catalogItemContextBase;
			}
			return null;
		}

		public abstract bool SetPath(string path, ItemPathOptions pathOptions);

		protected abstract CatalogItemContextBase<TPathStorageType> CreateContext(IPathTranslator pathTranslator);
	}
}
