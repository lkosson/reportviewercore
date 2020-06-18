using System;

namespace Microsoft.Reporting.Map.WebForms
{
	internal static class DateTimeHelper
	{
		public static string ToOdataJson(DateTime dateTime)
		{
			DateTime dateTime2 = new DateTime(1970, 1, 1);
			return "\\/Date(" + new TimeSpan(dateTime.ToUniversalTime().Ticks - dateTime2.Ticks).TotalMilliseconds.ToString("#") + ")\\/";
		}

		public static DateTime FromOdataJson(string jsonDate)
		{
			jsonDate = jsonDate.Replace("/Date(", "").Replace(")/", "");
			long num = 0L;
			long num2 = 0L;
			int num3 = jsonDate.IndexOf("+");
			int num4 = jsonDate.IndexOf("-");
			if (num3 > 0)
			{
				num = long.Parse(jsonDate.Substring(0, num3));
				num2 = long.Parse(jsonDate.Substring(num4)) / 100;
			}
			else if (num4 > 0)
			{
				num = long.Parse(jsonDate.Substring(0, num4));
				num2 = long.Parse(jsonDate.Substring(num4)) / 100;
			}
			else
			{
				num = long.Parse(jsonDate);
			}
			return new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddMilliseconds(num).AddHours(num2);
		}
	}
}
