using Microsoft.ReportingServices.Common;
using System.Collections.Specialized;
using System.Text;

namespace Microsoft.ReportingServices.Diagnostics
{
	internal sealed class CatalogItemUrlBuilder
	{
		private StringBuilder m_urlString;

		private IReportParameterLookup m_paramLookup;

		private IPathTranslator m_pathTranslator;

		private IPathManager m_pathManager;

		private static readonly string EncodedParameterNullSuffix = UrlUtil.UrlEncode(":isnull");

		private static readonly string EncodedCatalogParameterPrefix = UrlUtil.UrlEncode("rs:");

		private static readonly string EncodedRenderingParameterPrefix = UrlUtil.UrlEncode("rc:");

		private static readonly string EncodedReportParameterPrefix = UrlUtil.UrlEncode("");

		private static readonly string EncodedUserNameParameterPrefix = UrlUtil.UrlEncode("dsu:");

		public static string NameValueCollectionToQueryString(NameValueCollection parameters)
		{
			if (parameters == null)
			{
				return null;
			}
			StringBuilder stringBuilder = new StringBuilder();
			bool flag = true;
			for (int i = 0; i < parameters.Count; i++)
			{
				string key = parameters.GetKey(i);
				string[] values = parameters.GetValues(i);
				if (values == null || values.Length == 0)
				{
					if (!flag)
					{
						stringBuilder.Append("&");
					}
					flag = false;
					stringBuilder.Append(UrlUtil.UrlEncode(key));
					continue;
				}
				for (int j = 0; j < values.Length; j++)
				{
					if (!flag)
					{
						stringBuilder.Append("&");
					}
					flag = false;
					stringBuilder.Append(UrlUtil.UrlEncode(key));
					stringBuilder.Append("=");
					stringBuilder.Append(UrlUtil.UrlEncode(values[j]));
				}
			}
			return stringBuilder.ToString();
		}

		private CatalogItemUrlBuilder(IPathManager pathManager)
		{
			m_pathManager = pathManager;
		}

		public CatalogItemUrlBuilder(string urlString)
		{
			m_urlString = new StringBuilder(urlString);
		}

		public CatalogItemUrlBuilder(ICatalogItemContext ctx)
			: this(ctx, isFolder: false)
		{
		}

		public CatalogItemUrlBuilder(ICatalogItemContext ctx, IReportParameterLookup paramLookup)
			: this(ctx, isFolder: false)
		{
			m_paramLookup = paramLookup;
		}

		public CatalogItemUrlBuilder(ICatalogItemContext ctx, bool isFolder)
		{
			m_pathTranslator = ctx.PathTranslator;
			m_pathManager = ctx.PathManager;
			Construct(ctx.HostRootUri, ctx.HostSpecificItemPath, alreadyEscaped: false, addItemPathAsQuery: true, isFolder);
		}

		public static CatalogItemUrlBuilder CreateNonServerBuilder(string serverVirtualFolderUrl, string itemPath, bool alreadyEscaped, bool addItemPathAsQuery)
		{
			CatalogItemUrlBuilder catalogItemUrlBuilder = new CatalogItemUrlBuilder(RSPathUtil.Instance);
			catalogItemUrlBuilder.Construct(serverVirtualFolderUrl, itemPath, alreadyEscaped, addItemPathAsQuery, isFolder: false);
			return catalogItemUrlBuilder;
		}

		private void Construct(string serverVirtualFolderUrl, string itemPath, bool alreadyEscaped, bool addItemPathAsQuery, bool isFolder)
		{
			m_urlString = m_pathManager.ConstructUrlBuilder(m_pathTranslator, serverVirtualFolderUrl, itemPath, alreadyEscaped, addItemPathAsQuery, isFolder);
		}

		public override string ToString()
		{
			return m_urlString.ToString();
		}

		public void AppendUnparsedParameters(NameValueCollection parameters)
		{
			for (int i = 0; i < parameters.Count; i++)
			{
				string key = parameters.GetKey(i);
				string[] values = parameters.GetValues(i);
				if (key == null)
				{
					continue;
				}
				if (values != null)
				{
					for (int j = 0; j < values.Length; j++)
					{
						AppendOneParameter(string.Empty, key, values[j], addNullSuffix: false);
					}
				}
				else
				{
					AppendOneParameter(string.Empty, key, null, addNullSuffix: false);
				}
			}
		}

		public void AppendReportParameter(string name, string val)
		{
			NameValueCollection nameValueCollection = new NameValueCollection();
			nameValueCollection.Add(name, val);
			if (!ReplaceParametersWithExecParameterId(nameValueCollection))
			{
				AppendOneParameter(EncodedReportParameterPrefix, name, val);
			}
		}

		private void InternalAppendReportParameters(NameValueCollection parameters)
		{
			AppendParameterCollection(EncodedReportParameterPrefix, parameters);
		}

		public void AppendReportParameters(NameValueCollection parameters)
		{
			if (!ReplaceParametersWithExecParameterId(parameters))
			{
				InternalAppendReportParameters(parameters);
			}
		}

		private bool ReplaceParametersWithExecParameterId(NameValueCollection parameters)
		{
			string text = null;
			if (m_paramLookup != null && parameters != null)
			{
				text = m_paramLookup.GetReportParamsInstanceId(parameters);
			}
			if (text != null)
			{
				AppendCatalogParameter("StoredParametersID", text);
				return true;
			}
			return false;
		}

		public void AppendRenderingParameter(string name, string val)
		{
			AppendOneParameter(EncodedRenderingParameterPrefix, name, val);
		}

		public void AppendRenderingParameters(NameValueCollection parameters)
		{
			AppendParameterCollection(EncodedRenderingParameterPrefix, parameters);
		}

		public void AppendCatalogParameter(string name, string val)
		{
			AppendOneParameter(EncodedCatalogParameterPrefix, name, val);
		}

		public void AppendCatalogParameters(NameValueCollection parameters)
		{
			AppendParameterCollection(EncodedCatalogParameterPrefix, parameters);
		}

		private void AppendParameterCollection(string encodedPrefix, NameValueCollection parameters)
		{
			if (parameters == null)
			{
				return;
			}
			for (int i = 0; i < parameters.Count; i++)
			{
				string key = parameters.GetKey(i);
				string[] values = parameters.GetValues(i);
				if (values == null)
				{
					AppendOneParameter(encodedPrefix, key, null);
					continue;
				}
				for (int j = 0; j < values.Length; j++)
				{
					AppendOneParameter(encodedPrefix, key, values[j]);
				}
			}
		}

		private void AppendOneParameter(string encodedPrefix, string name, string val)
		{
			AppendOneParameter(encodedPrefix, name, val, addNullSuffix: true);
		}

		private void AppendOneParameter(string encodedPrefix, string name, string val, bool addNullSuffix)
		{
			m_urlString.Append('&');
			if (val != null)
			{
				m_urlString.Append(encodedPrefix);
				m_urlString.Append(EncodeUrlParameter(name));
				m_urlString.Append("=");
				m_urlString.Append(EncodeUrlParameter(val));
				return;
			}
			m_urlString.Append(encodedPrefix);
			m_urlString.Append(EncodeUrlParameter(name));
			if (addNullSuffix)
			{
				m_urlString.Append(EncodedParameterNullSuffix);
			}
			m_urlString.Append("=");
			m_urlString.Append(bool.TrueString);
		}

		private static string EncodeUrlParameter(string param)
		{
			return UrlUtil.UrlEncode(param).Replace("'", "%27");
		}
	}
}
