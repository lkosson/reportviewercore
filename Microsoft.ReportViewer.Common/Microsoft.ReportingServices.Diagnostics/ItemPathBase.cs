using Microsoft.ReportingServices.Diagnostics.Utilities;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Microsoft.ReportingServices.Diagnostics
{
	internal abstract class ItemPathBase
	{
		protected readonly string m_value;

		protected string m_editSessionID;

		public string Value => m_value;

		public string EditSessionID
		{
			get
			{
				return m_editSessionID;
			}
			set
			{
				m_editSessionID = value;
			}
		}

		public bool IsEditSession => !string.IsNullOrEmpty(m_editSessionID);

		public virtual string FullEditSessionIdentifier
		{
			get
			{
				if (IsEditSession)
				{
					if (Value.Contains("|") || EditSessionID.Contains("|"))
					{
						throw new InternalCatalogException("Unexpected character in ItemPath or EditSessionID");
					}
					return string.Format(CultureInfo.InvariantCulture, "|{0}|@|{1}|", EditSessionID, Value);
				}
				return Value;
			}
		}

		protected ItemPathBase(string itemPath)
		{
			if (!ParseInternalItemPathParts(itemPath, out m_editSessionID, out m_value))
			{
				m_value = itemPath?.Trim();
				m_editSessionID = null;
			}
		}

		protected ItemPathBase(string itemPath, string editSessionID)
		{
			m_value = itemPath;
			m_editSessionID = editSessionID;
		}

		public static string SafeValue(ItemPathBase path)
		{
			return path?.Value;
		}

		public static string SafeEditSessionID(ItemPathBase path)
		{
			return path?.EditSessionID;
		}

		public static bool IsNullOrEmpty(ItemPathBase path)
		{
			if (path != null)
			{
				return string.IsNullOrEmpty(path.Value);
			}
			return true;
		}

		public override string ToString()
		{
			return m_value;
		}

		public static string GetLocalPath(string path)
		{
			return path;
		}

		public static int CatalogCompare(ItemPathBase a, string b)
		{
			int num = Localization.CatalogCultureCompare(SafeValue(a), b);
			if (num == 0 && a != null && a.IsEditSession)
			{
				return 1;
			}
			return num;
		}

		public static int CatalogCompare(ItemPathBase a, ItemPathBase b)
		{
			int num = Localization.CatalogCultureCompare(SafeValue(a), SafeValue(b));
			if (num == 0)
			{
				return string.CompareOrdinal(SafeEditSessionID(a), SafeEditSessionID(b));
			}
			return num;
		}

		public static string GetParentPathForSharePoint(string path)
		{
			return path;
		}

		public static string GetEditSessionID(string path)
		{
			string editSessionID = null;
			string itemPath = null;
			ParseInternalItemPathParts(path, out editSessionID, out itemPath);
			return editSessionID;
		}

		public static bool ParseInternalItemPathParts(string path, out string editSessionID, out string itemPath)
		{
			if (!string.IsNullOrEmpty(path))
			{
				Match match = Regex.Match(path, "  ^\\|\r\n        ([a-z0-9]{24})\r\n    \\|\r\n    @\r\n    \\|\r\n        ([^\\|]+)\r\n    \\|$", RegexOptions.Compiled | RegexOptions.IgnorePatternWhitespace);
				if (match.Success)
				{
					editSessionID = match.Groups[1].Value.Trim();
					itemPath = match.Groups[2].Value.Trim();
					return true;
				}
			}
			editSessionID = null;
			itemPath = null;
			return false;
		}
	}
}
