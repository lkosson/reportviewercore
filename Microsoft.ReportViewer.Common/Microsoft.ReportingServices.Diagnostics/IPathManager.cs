using System.Collections.Specialized;
using System.Text;

namespace Microsoft.ReportingServices.Diagnostics
{
	internal interface IPathManager
	{
		string RelativePathToAbsolutePath(string relativePath, string reportPath);

		bool IsSupportedUrl(string path, bool checkProtocol, out bool isInternal);

		string EnsureReportNamePath(string reportNamePath);

		StringBuilder ConstructUrlBuilder(IPathTranslator pathTranslator, string serverVirtualFolderUrl, string itemPath, bool alreadyEscaped, bool addItemPathAsQuery, bool forceAddItemPathAsQuery);

		void ExtractFromUrl(string url, out string path, out NameValueCollection queryParameters);
	}
}
