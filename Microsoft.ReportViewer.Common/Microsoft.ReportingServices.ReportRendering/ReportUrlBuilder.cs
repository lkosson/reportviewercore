using Microsoft.ReportingServices.Common;
using Microsoft.ReportingServices.Diagnostics;
using System;
using System.Collections.Specialized;

namespace Microsoft.ReportingServices.ReportRendering
{
	internal sealed class ReportUrlBuilder
	{
		private string m_replacementRoot;

		private CatalogItemUrlBuilder m_catalogItemUrlBuilder;

		private bool m_hasReplacement;

		private bool m_useRepacementRoot = true;

		internal ReportUrlBuilder(RenderingContext reportContext, ICatalogItemContext changedContext, string initialUrl, string replacementRoot)
		{
			ReportUrl.BuildPathUri((changedContext == null) ? reportContext.TopLevelReportContext : changedContext, initialUrl, null, out ICatalogItemContext newContext);
			m_catalogItemUrlBuilder = new CatalogItemUrlBuilder(newContext, newContext.RSRequestParameters);
			m_replacementRoot = replacementRoot;
		}

		internal ReportUrlBuilder(RenderingContext reportContext, string initialUrl, string replacementRoot)
		{
			ICatalogItemContext topLevelReportContext = reportContext.TopLevelReportContext;
			ReportUrl.BuildPathUri(topLevelReportContext, initialUrl, null, out ICatalogItemContext _);
			m_catalogItemUrlBuilder = new CatalogItemUrlBuilder(topLevelReportContext, topLevelReportContext.RSRequestParameters);
			m_replacementRoot = replacementRoot;
		}

		internal ReportUrlBuilder(RenderingContext reportContext, string initialUrl, bool useReplacementRoot, bool addReportParameters)
		{
			ICatalogItemContext topLevelReportContext = reportContext.TopLevelReportContext;
			ICatalogItemContext newContext;
			string pathOrUrl = ReportUrl.BuildPathUri(topLevelReportContext, initialUrl, null, out newContext);
			m_catalogItemUrlBuilder = new CatalogItemUrlBuilder(topLevelReportContext, topLevelReportContext.RSRequestParameters);
			if (addReportParameters)
			{
				m_catalogItemUrlBuilder.AppendReportParameters(reportContext.TopLevelReportContext.RSRequestParameters.ReportParameters);
			}
			m_useRepacementRoot = useReplacementRoot;
			if (reportContext != null && reportContext.TopLevelReportContext.IsReportServerPathOrUrl(pathOrUrl, protocolRestriction: true, out bool _))
			{
				m_replacementRoot = reportContext.ReplacementRoot;
			}
		}

		public override string ToString()
		{
			return m_catalogItemUrlBuilder.ToString();
		}

		public Uri ToUri()
		{
			string uriString;
			if (m_replacementRoot != null)
			{
				if (m_useRepacementRoot)
				{
					AddReplacementRoot();
					uriString = m_replacementRoot + UrlUtil.UrlEncode(m_catalogItemUrlBuilder.ToString());
				}
				else
				{
					uriString = m_catalogItemUrlBuilder.ToString();
				}
			}
			else
			{
				uriString = m_catalogItemUrlBuilder.ToString();
			}
			return new Uri(uriString);
		}

		public void AddReplacementRoot()
		{
			if (!m_hasReplacement)
			{
				m_hasReplacement = true;
				if (m_replacementRoot != null)
				{
					m_catalogItemUrlBuilder.AppendRenderingParameter("ReplacementRoot", m_replacementRoot);
				}
			}
		}

		public void AddParameters(NameValueCollection urlParameters, UrlParameterType parameterType)
		{
			switch (parameterType)
			{
			case UrlParameterType.RenderingParameter:
				m_catalogItemUrlBuilder.AppendRenderingParameters(urlParameters);
				break;
			case UrlParameterType.ReportParameter:
				m_catalogItemUrlBuilder.AppendReportParameters(urlParameters);
				break;
			case UrlParameterType.ServerParameter:
				m_catalogItemUrlBuilder.AppendCatalogParameters(urlParameters);
				break;
			}
		}

		public void AddParameter(string name, string val, UrlParameterType parameterType)
		{
			switch (parameterType)
			{
			case UrlParameterType.RenderingParameter:
				m_catalogItemUrlBuilder.AppendRenderingParameter(name, val);
				break;
			case UrlParameterType.ReportParameter:
				m_catalogItemUrlBuilder.AppendReportParameter(name, val);
				break;
			case UrlParameterType.ServerParameter:
				m_catalogItemUrlBuilder.AppendCatalogParameter(name, val);
				break;
			}
		}
	}
}
