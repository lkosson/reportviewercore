using System.Collections.Specialized;

namespace Microsoft.ReportingServices.Diagnostics
{
	internal interface ICatalogItemContext
	{
		string ItemPathAsString
		{
			get;
		}

		string ItemName
		{
			get;
		}

		string ParentPath
		{
			get;
		}

		string HostSpecificItemPath
		{
			get;
		}

		string StableItemPath
		{
			get;
		}

		RSRequestParameters RSRequestParameters
		{
			get;
		}

		string HostRootUri
		{
			get;
		}

		IPathTranslator PathTranslator
		{
			get;
		}

		IPathManager PathManager
		{
			get;
		}

		string MapUserProvidedPath(string path);

		ICatalogItemContext GetSubreportContext(string subreportPath);

		string AdjustSubreportOrDrillthroughReportPath(string reportPath);

		string CombineUrl(string url, bool protocolRestriction, NameValueCollection unparsedParameters, out ICatalogItemContext newContext);

		ICatalogItemContext Combine(string url);

		bool IsReportServerPathOrUrl(string pathOrUrl, bool protocolRestriction, out bool isRelative);

		bool IsSupportedProtocol(string path, bool protocolRestriction);

		bool IsSupportedProtocol(string path, bool protocolRestriction, out bool isRelative);
	}
}
