using System;
using System.Globalization;

namespace Microsoft.Reporting.Chart.WebForms.Utilities
{
	internal static class ValueConverter
	{
		public static string FormatValue(Chart chart, object obj, double value, string format, ChartValueTypes valueType, ChartElementType elementType)
		{
			string text = format;
			string text2 = "";
			if (chart != null && chart.FormatNumberHandler != null)
			{
				int elementId = 0;
				if (obj is DataPoint)
				{
					elementId = ((DataPoint)obj).ElementId;
				}
				text2 = chart.FormatNumberHandler(obj, value, format, valueType, elementId, elementType);
				if (text2.Length > 0)
				{
					return text2;
				}
			}
			if (text != null && text.Length > 0)
			{
				int num = text.IndexOf('{', 0);
				if (num >= 0)
				{
					while (num >= 0)
					{
						if (!text.Substring(num).StartsWith("{0:", StringComparison.Ordinal))
						{
							if (num >= 1 && text.Substring(num - 1, 1) == "{")
							{
								continue;
							}
							text = text.Insert(num + 1, "0:");
						}
						num = text.IndexOf('{', num + 1);
					}
				}
				else
				{
					text = "{0:" + text + "}";
				}
			}
			switch (valueType)
			{
			case ChartValueTypes.DateTime:
			case ChartValueTypes.Date:
			case ChartValueTypes.DateTimeOffset:
				if (text.Length == 0)
				{
					text = "{0:d}";
					if (valueType == ChartValueTypes.DateTimeOffset)
					{
						text += " +0";
					}
				}
				text2 = string.Format(CultureInfo.CurrentCulture, text, DateTime.FromOADate(value));
				break;
			case ChartValueTypes.Time:
				if (text.Length == 0)
				{
					text = "{0:t}";
				}
				text2 = string.Format(CultureInfo.CurrentCulture, text, DateTime.FromOADate(value));
				break;
			default:
			{
				bool flag = false;
				if (text.Length == 0)
				{
					text = "{0:G}";
				}
				try
				{
					text2 = string.Format(CultureInfo.CurrentCulture, text, value);
				}
				catch (Exception)
				{
					flag = true;
				}
				if (flag)
				{
					flag = false;
					try
					{
						text2 = string.Format(CultureInfo.CurrentCulture, text, (long)value);
					}
					catch (Exception)
					{
						flag = true;
					}
				}
				if (flag)
				{
					return format;
				}
				break;
			}
			}
			return text2;
		}
	}
}
