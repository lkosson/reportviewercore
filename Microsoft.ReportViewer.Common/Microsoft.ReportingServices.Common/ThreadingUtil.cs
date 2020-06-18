namespace Microsoft.ReportingServices.Common
{
	internal static class ThreadingUtil
	{
		internal static T ReturnOnDemandValue<T>(ref T valueStorage, object valueLock, CreatorGetter<T> getValue)
		{
			if (valueStorage != null)
			{
				return valueStorage;
			}
			lock (valueLock)
			{
				if (valueStorage == null)
				{
					valueStorage = getValue();
				}
				return valueStorage;
			}
		}
	}
}
