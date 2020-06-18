using Microsoft.ReportingServices.Diagnostics;
using Microsoft.ReportingServices.Diagnostics.Utilities;
using Microsoft.ReportingServices.ReportProcessing;
using System;
using System.Collections.Specialized;

namespace Microsoft.ReportingServices.ReportRendering
{
	internal sealed class ReportUrl
	{
		private Uri m_pathUri;

		private string m_replacementRoot;

		private RenderingContext m_reportContext;

		private ICatalogItemContext m_newICatalogItemContext;

		internal ReportUrl(ICatalogItemContext catContext, string initialUrl)
		{
			m_pathUri = new Uri(BuildPathUri(catContext, initialUrl, null, out m_newICatalogItemContext));
		}

		internal ReportUrl(RenderingContext reportContext, string initialUrl)
		{
			m_reportContext = reportContext;
			m_pathUri = new Uri(BuildPathUri(reportContext.TopLevelReportContext, initialUrl, null, out m_newICatalogItemContext));
		}

		internal ReportUrl(RenderingContext reportContext, string initialUrl, bool checkProtocol, NameValueCollection unparsedParameters, bool useReplacementRoot)
		{
			m_reportContext = reportContext;
			m_pathUri = new Uri(BuildPathUri(reportContext.TopLevelReportContext, checkProtocol, initialUrl, unparsedParameters, out m_newICatalogItemContext));
			if (useReplacementRoot && reportContext.TopLevelReportContext.IsReportServerPathOrUrl(m_pathUri.AbsoluteUri, checkProtocol, out bool _))
			{
				m_replacementRoot = reportContext.ReplacementRoot;
			}
		}

		internal static string BuildPathUri(ICatalogItemContext currentICatalogItemContext, string initialUrl, NameValueCollection unparsedParameters, out ICatalogItemContext newContext)
		{
			return BuildPathUri(currentICatalogItemContext, checkProtocol: true, initialUrl, unparsedParameters, out newContext);
		}

		internal static string BuildPathUri(ICatalogItemContext currentCatalogItemContext, bool checkProtocol, string initialUrl, NameValueCollection unparsedParameters, out ICatalogItemContext newContext)
		{
			newContext = null;
			if (currentCatalogItemContext == null)
			{
				return initialUrl;
			}
			string text = null;
			try
			{
				text = currentCatalogItemContext.CombineUrl(initialUrl, checkProtocol, unparsedParameters, out newContext);
			}
			catch (UriFormatException)
			{
				throw new RenderingObjectModelException(ErrorCode.rsMalformattedURL);
			}
			if (!currentCatalogItemContext.IsSupportedProtocol(text, checkProtocol))
			{
				throw new RenderingObjectModelException(ErrorCode.rsUnsupportedURLProtocol, text);
			}
			return text;
		}

		internal static ReportUrl BuildHyperLinkURL(string hyperLinkUrlValue, RenderingContext renderingContext)
		{
			ReportUrl result = null;
			try
			{
				if (hyperLinkUrlValue != null)
				{
					if (renderingContext.TopLevelReportContext.IsReportServerPathOrUrl(hyperLinkUrlValue, protocolRestriction: false, out bool isRelative) && isRelative)
					{
						renderingContext.TopLevelReportContext.PathManager.ExtractFromUrl(hyperLinkUrlValue, out hyperLinkUrlValue, out NameValueCollection queryParameters);
						if (hyperLinkUrlValue == null || hyperLinkUrlValue.Length == 0)
						{
							return null;
						}
						return new ReportUrl(renderingContext, hyperLinkUrlValue, checkProtocol: false, queryParameters, useReplacementRoot: true);
					}
					return new ReportUrl(renderingContext, hyperLinkUrlValue, checkProtocol: false, null, useReplacementRoot: true);
				}
				return result;
			}
			catch
			{
				return null;
			}
		}

		public override string ToString()
		{
			return m_pathUri.AbsoluteUri;
		}

		public Uri ToUri()
		{
			Uri result = m_pathUri;
			if (m_replacementRoot != null)
			{
				result = new ReportUrlBuilder(m_reportContext, m_pathUri.AbsoluteUri, m_replacementRoot).ToUri();
			}
			return result;
		}

		public ReportUrlBuilder GetUrlBuilder(string initialUrl, bool useReplacementRoot)
		{
			return new ReportUrlBuilder(m_reportContext, m_newICatalogItemContext, initialUrl, useReplacementRoot ? m_replacementRoot : null);
		}
	}
}
