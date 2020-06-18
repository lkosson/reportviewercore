using Microsoft.ReportingServices.Common;
using Microsoft.ReportingServices.Diagnostics.Utilities;
using System;
using System.Collections.Specialized;
using System.Text;

namespace Microsoft.ReportingServices.Diagnostics
{
	internal sealed class RSPathUtil : IPathManager
	{
		public static readonly RSPathUtil Instance = new RSPathUtil();

		private static readonly Uri m_absoluteUri = new Uri("http://q");

		private RSPathUtil()
		{
		}

		string IPathManager.RelativePathToAbsolutePath(string path, string reportPath)
		{
			if (new Uri(path, UriKind.RelativeOrAbsolute).IsAbsoluteUri)
			{
				return path;
			}
			string text = null;
			text = ((reportPath.Length != 0) ? reportPath : "/");
			return new Uri(new Uri("c:" + text), path).GetComponents(UriComponents.Path, UriFormat.Unescaped).Substring(2);
		}

		public bool IsSupportedUrl(string path, bool checkProtocol)
		{
			bool isInternal;
			return ((IPathManager)this).IsSupportedUrl(path, checkProtocol, out isInternal);
		}

		bool IPathManager.IsSupportedUrl(string path, bool checkProtocol, out bool isInternal)
		{
			isInternal = false;
			if (path.StartsWith("HTTP:", StringComparison.OrdinalIgnoreCase) || path.StartsWith("HTTPS:", StringComparison.OrdinalIgnoreCase) || path.StartsWith("FTP:", StringComparison.OrdinalIgnoreCase) || path.StartsWith("MAILTO:", StringComparison.OrdinalIgnoreCase) || path.StartsWith("NEWS:", StringComparison.OrdinalIgnoreCase) || path.StartsWith("FILE:", StringComparison.OrdinalIgnoreCase))
			{
				return true;
			}
			if (ContainsOtherProtocol(path))
			{
				if (!checkProtocol)
				{
					return true;
				}
				return false;
			}
			isInternal = true;
			return true;
		}

		string IPathManager.EnsureReportNamePath(string reportNamePath)
		{
			return reportNamePath;
		}

		StringBuilder IPathManager.ConstructUrlBuilder(IPathTranslator pathTranslator, string serverVirtualFolderUrl, string itemPath, bool alreadyEscaped, bool addItemPathAsQuery, bool forceAddItemPathAsQuery)
		{
			if (!alreadyEscaped)
			{
				serverVirtualFolderUrl = ((!string.IsNullOrEmpty(serverVirtualFolderUrl)) ? new Uri(serverVirtualFolderUrl).AbsoluteUri : "http://reportserver");
			}
			string value = UrlUtil.UrlEncode(itemPath);
			StringBuilder stringBuilder = new StringBuilder(serverVirtualFolderUrl);
			if (addItemPathAsQuery)
			{
				stringBuilder.Append("?");
			}
			stringBuilder.Append(value);
			return stringBuilder;
		}

		void IPathManager.ExtractFromUrl(string url, out string path, out NameValueCollection queryParameters)
		{
			RSTrace.CatalogTrace.Assert(condition: false, "RSPathUtil.ExtractFromUrl cannot be used in local mode due to client profile restrictions");
			throw new NotImplementedException("RSPathUtil.ExtractFromUrl cannot be used in local mode due to client profile restrictions");
		}

		private static bool ContainsOtherProtocol(string path)
		{
			Uri absoluteUri = m_absoluteUri;
			if (new Uri(absoluteUri, path).Scheme != absoluteUri.Scheme)
			{
				return true;
			}
			return false;
		}
	}
}
