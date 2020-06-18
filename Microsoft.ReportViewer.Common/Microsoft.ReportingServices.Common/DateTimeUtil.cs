using System;
using System.Globalization;

namespace Microsoft.ReportingServices.Common
{
	internal static class DateTimeUtil
	{
		internal static bool TryParseDateTime(string strDateTime, CultureInfo formatProvider, out DateTimeOffset dateTimeOffset, out bool hasTimeOffset)
		{
			hasTimeOffset = false;
			if (DateTimeOffset.TryParse(strDateTime, formatProvider, DateTimeStyles.None, out dateTimeOffset))
			{
				if (TimeSpan.TryParse(strDateTime, out TimeSpan _))
				{
					return false;
				}
				if (!DateTimeOffset.TryParse(strDateTime + " +0", formatProvider, DateTimeStyles.None, out DateTimeOffset _))
				{
					hasTimeOffset = true;
				}
				return true;
			}
			DateTimeFormatInfo dateTimeFormatInfo = (formatProvider != null) ? formatProvider.DateTimeFormat : CultureInfo.CurrentCulture.DateTimeFormat;
			string[] allDateTimePatterns = dateTimeFormatInfo.GetAllDateTimePatterns('d');
			if (!DateTimeOffset.TryParseExact(strDateTime, allDateTimePatterns, formatProvider, DateTimeStyles.None, out dateTimeOffset))
			{
				string[] allDateTimePatterns2 = dateTimeFormatInfo.GetAllDateTimePatterns('G');
				if (!DateTimeOffset.TryParseExact(strDateTime, allDateTimePatterns2, formatProvider, DateTimeStyles.None, out dateTimeOffset))
				{
					for (int i = 0; i < allDateTimePatterns2.Length; i++)
					{
						allDateTimePatterns2[i] += " zzz";
					}
					if (!DateTimeOffset.TryParseExact(strDateTime, allDateTimePatterns2, formatProvider, DateTimeStyles.None, out dateTimeOffset))
					{
						return false;
					}
					hasTimeOffset = true;
				}
			}
			return true;
		}
	}
}
