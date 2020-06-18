using System;

namespace Microsoft.ReportingServices.ReportProcessing
{
	internal class ArrayHelper
	{
		internal static bool Equals(Array o1, Array o2)
		{
			if (o1.Length == o2.Length)
			{
				for (int i = 0; i < o1.Length; i++)
				{
					if (!o1.GetValue(i).Equals(o2.GetValue(i)))
					{
						return false;
					}
				}
				return true;
			}
			return false;
		}
	}
}
