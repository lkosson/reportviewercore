using System;

namespace Microsoft.ReportingServices.Rendering.ExcelRenderer.ExcelGenerator.BIFF8.Records
{
	internal static class RKEncoder
	{
		private static readonly double MAXRKVALUE = 1.7976931348623156E+306;

		internal static uint? EncodeRK(double aRKValue)
		{
			uint? num = null;
			if (aRKValue == 0.0)
			{
				aRKValue = 0.0;
			}
			uint? num2 = num = double2RK(aRKValue, 0u);
			if (num2.HasValue)
			{
				return num;
			}
			num2 = (num = longint2RK(aRKValue, 2L));
			if (num2.HasValue)
			{
				return num;
			}
			if (Math.Abs(aRKValue) > MAXRKVALUE)
			{
				return null;
			}
			double aRKValue2 = aRKValue * 100.0;
			num2 = (num = double2RK(aRKValue2, 1u));
			if (num2.HasValue)
			{
				return num;
			}
			return longint2RK(aRKValue2, 3L);
		}

		internal static double DecodeRK(int aBytes)
		{
			double num = 0.0;
			num = (((aBytes & 2) == 0) ? BitConverter.Int64BitsToDouble((long)(aBytes >> 2) << 34) : ((double)(aBytes >> 2)));
			if ((aBytes & 1) != 0)
			{
				num /= 100.0;
			}
			return num;
		}

		private static uint? double2RK(double aRKValue, uint aTypeMask)
		{
			long num = BitConverter.DoubleToInt64Bits(aRKValue);
			ulong num2 = (ulong)(num & uint.MaxValue);
			ulong num3 = (ulong)num >> 32;
			if (num2 == 0L && (num3 & 3) == 0L)
			{
				return (uint)((num3 & 4294967292u) | aTypeMask);
			}
			return null;
		}

		private static uint? longint2RK(double aRKValue, long aTypeMask)
		{
			long num = (long)Math.Round(aRKValue);
			if (aRKValue - (double)num == 0.0 && Math.Abs(num) <= 536870911 && (Math.Abs(num) & 3758096384u) == 0L)
			{
				return (uint)(((num << 2) & 4294967292u) | aTypeMask);
			}
			return null;
		}
	}
}
