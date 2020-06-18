using Microsoft.ReportingServices.Common;
using System;
using System.Threading;

namespace Microsoft.ReportingServices.ReportPublishing
{
	internal sealed class DataSetValidator
	{
		internal static uint LOCALE_SYSTEM_DEFAULT = 2048u;

		private DataSetValidator()
		{
		}

		internal static bool ValidateCollation(string collation, out uint lcid)
		{
			lcid = LOCALE_SYSTEM_DEFAULT;
			if (collation == null)
			{
				return true;
			}
			object obj = RdlCollations.Collations[collation];
			if (obj == null)
			{
				return false;
			}
			lcid = (uint)obj;
			return true;
		}

		internal static uint LCIDfromRDLCollation(string collation)
		{
			if (collation != null && ValidateCollation(collation, out uint lcid))
			{
				return lcid;
			}
			return (uint)Thread.CurrentThread.CurrentCulture.LCID;
		}

		internal static uint GetSQLSortCompareMask(bool caseSensitivity, bool accentSensitivity, bool kanatypeSensitivity, bool widthSensitivity)
		{
			uint num = 0u;
			if (!caseSensitivity)
			{
				num |= 1;
			}
			if (!accentSensitivity)
			{
				num |= 2;
			}
			if (!kanatypeSensitivity)
			{
				num |= 0x10000;
			}
			if (!widthSensitivity)
			{
				num |= 0x20000;
			}
			return num;
		}

		internal static void GetCompareFlagsfromAutoDetectedCollation(string autoCollation, out bool caseSensitivity, out bool accentSensitivity, out bool kanatypeSensitivity, out bool widthSensitivity)
		{
			caseSensitivity = false;
			accentSensitivity = false;
			kanatypeSensitivity = false;
			widthSensitivity = false;
			if (autoCollation != null)
			{
				caseSensitivity = (0 < autoCollation.IndexOf("_CS", StringComparison.Ordinal));
				accentSensitivity = (0 < autoCollation.IndexOf("_AS", StringComparison.Ordinal));
				kanatypeSensitivity = (0 < autoCollation.IndexOf("_KS", StringComparison.Ordinal));
				widthSensitivity = (0 < autoCollation.IndexOf("_WS", StringComparison.Ordinal));
			}
		}
	}
}
