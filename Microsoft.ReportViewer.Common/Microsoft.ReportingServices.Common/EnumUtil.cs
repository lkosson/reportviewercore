using System;

namespace Microsoft.ReportingServices.Common
{
	internal static class EnumUtil
	{
		public static bool IsDefined<T>(T value)
		{
			return Enum.IsDefined(typeof(T), value);
		}

		public static bool TryParse<T>(string value, out T enumValue)
		{
			try
			{
				enumValue = (T)Enum.Parse(typeof(T), value);
				return true;
			}
			catch (ArgumentException)
			{
				enumValue = default(T);
				return false;
			}
		}
	}
}
