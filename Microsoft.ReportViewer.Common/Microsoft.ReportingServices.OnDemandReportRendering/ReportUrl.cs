using Microsoft.Cloud.Platform.Utils;
using Microsoft.ReportingServices.Diagnostics;
using Microsoft.ReportingServices.Diagnostics.Utilities;
using Microsoft.ReportingServices.ReportProcessing;
using Microsoft.ReportingServices.ReportRendering;
using System;
using System.Collections.Specialized;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class ReportUrl
	{
		private Microsoft.ReportingServices.ReportRendering.ReportUrl m_renderUrl;

		private Uri m_pathUri;

		private bool IsOldSnapshot => m_renderUrl != null;

		internal ReportUrl(Microsoft.ReportingServices.ReportRendering.ReportUrl renderUrl)
		{
			m_renderUrl = renderUrl;
		}

		internal ReportUrl(ICatalogItemContext itemContext, string initialUrl)
		{
			m_pathUri = new Uri(BuildPathUri(itemContext, initialUrl, null));
		}

		internal ReportUrl(ICatalogItemContext itemContext, string initialUrl, bool checkProtocol, NameValueCollection unparsedParameters)
		{
			m_pathUri = new Uri(BuildPathUri(itemContext, checkProtocol, initialUrl, unparsedParameters, out ICatalogItemContext _));
			if (m_pathUri != null && string.CompareOrdinal(m_pathUri.Scheme, "mailto") == 0)
			{
				_ = m_pathUri.AbsoluteUri;
			}
		}

		internal static string BuildPathUri(ICatalogItemContext currentICatalogItemContext, string initialUrl, NameValueCollection unparsedParameters)
		{
			ICatalogItemContext newContext;
			return BuildPathUri(currentICatalogItemContext, checkProtocol: true, initialUrl, unparsedParameters, out newContext);
		}

		internal static string BuildPathUri(ICatalogItemContext currentICatalogItemContext, bool checkProtocol, string initialUrl, NameValueCollection unparsedParameters, out ICatalogItemContext newContext)
		{
			newContext = null;
			if (currentICatalogItemContext == null)
			{
				return initialUrl;
			}
			string text = null;
			try
			{
				text = currentICatalogItemContext.CombineUrl(initialUrl, checkProtocol, unparsedParameters, out newContext);
			}
			catch (RSException)
			{
				throw;
			}
			catch (Exception)
			{
				throw new RenderingObjectModelException(ErrorCode.rsMalformattedURL);
			}
			if (!currentICatalogItemContext.IsSupportedProtocol(text, checkProtocol))
			{
				throw new RenderingObjectModelException(ErrorCode.rsUnsupportedURLProtocol, text.MarkAsPrivate());
			}
			return text;
		}

		internal static string BuildDrillthroughUrl(ICatalogItemContext currentCatalogItemContext, string initialUrl, NameValueCollection parameters)
		{
			if (BuildPathUri(currentCatalogItemContext, checkProtocol: true, initialUrl, parameters, out ICatalogItemContext newContext) == null || newContext == null)
			{
				return null;
			}
			CatalogItemUrlBuilder catalogItemUrlBuilder = new CatalogItemUrlBuilder(newContext, newContext.RSRequestParameters);
			catalogItemUrlBuilder.AppendReportParameters(parameters);
			return new Uri(catalogItemUrlBuilder.ToString()).AbsoluteUri;
		}

		internal static ReportUrl BuildHyperlinkUrl(RenderingContext renderingContext, ObjectType objectType, string objectName, string propertyName, ICatalogItemContext itemContext, string initialUrl)
		{
			ReportUrl result = null;
			if (initialUrl == null)
			{
				return null;
			}
			bool flag = false;
			try
			{
				string path = initialUrl;
				bool isRelative;
				bool num = itemContext.IsReportServerPathOrUrl(path, protocolRestriction: false, out isRelative);
				NameValueCollection queryParameters = null;
				if (num && isRelative)
				{
					itemContext.PathManager.ExtractFromUrl(path, out path, out queryParameters);
					if (path == null || path.Length == 0)
					{
						flag = true;
						path = null;
						result = null;
					}
				}
				if (path != null)
				{
					result = new ReportUrl(itemContext, path, checkProtocol: false, queryParameters);
				}
			}
			catch (ItemNotFoundException)
			{
				flag = true;
			}
			catch (RenderingObjectModelException)
			{
				flag = true;
			}
			catch (RSException)
			{
				throw;
			}
			catch (Exception)
			{
				flag = true;
			}
			if (flag)
			{
				renderingContext.OdpContext.ErrorContext.Register(ProcessingErrorCode.rsInvalidURLProtocol, Severity.Warning, objectType, objectName, propertyName, initialUrl.MarkAsPrivate());
			}
			return result;
		}

		public Uri ToUri()
		{
			if (IsOldSnapshot)
			{
				return m_renderUrl.ToUri();
			}
			return m_pathUri;
		}

		public override string ToString()
		{
			if (IsOldSnapshot)
			{
				return m_renderUrl.ToString();
			}
			return m_pathUri.AbsoluteUri;
		}
	}
}
