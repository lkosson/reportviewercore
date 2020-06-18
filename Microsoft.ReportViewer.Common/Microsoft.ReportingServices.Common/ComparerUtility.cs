using System;

namespace Microsoft.ReportingServices.Common
{
	internal static class ComparerUtility
	{
		internal static Type GetNumericDateTypeFromDataTypeCode(DataTypeCode typeCode)
		{
			switch (typeCode)
			{
			case DataTypeCode.DateTime:
				return typeof(DateTime);
			case DataTypeCode.Double:
				return typeof(double);
			case DataTypeCode.Decimal:
				return typeof(decimal);
			case DataTypeCode.Int64:
				return typeof(long);
			default:
				return null;
			}
		}

		internal static DataTypeCode GetCommonVariantConversionType(DataTypeCode x, DataTypeCode y)
		{
			if ((y == DataTypeCode.Double && IsComparableToReal(x)) || (x == DataTypeCode.Double && IsComparableToReal(y)))
			{
				return DataTypeCode.Double;
			}
			if ((y == DataTypeCode.Decimal && IsComparableToCurrency(x)) || (x == DataTypeCode.Decimal && IsComparableToCurrency(y)))
			{
				return DataTypeCode.Decimal;
			}
			if ((y == DataTypeCode.DateTime && IsNumericVariant(x)) || (x == DataTypeCode.DateTime && IsNumericVariant(y)))
			{
				return DataTypeCode.Double;
			}
			if ((y == DataTypeCode.Int64 && x == DataTypeCode.Int32) || (x == DataTypeCode.Int64 && y == DataTypeCode.Int32))
			{
				return DataTypeCode.Int64;
			}
			return DataTypeCode.Unknown;
		}

		private static bool IsComparableToReal(DataTypeCode typeCode)
		{
			if (typeCode - 6 <= DataTypeCode.Unknown || typeCode - 10 <= DataTypeCode.Unknown)
			{
				return true;
			}
			return false;
		}

		private static bool IsComparableToCurrency(DataTypeCode typeCode)
		{
			if (typeCode - 6 <= DataTypeCode.Unknown || typeCode == DataTypeCode.Decimal)
			{
				return true;
			}
			return false;
		}

		internal static bool IsNumericVariant(DataTypeCode typeCode)
		{
			if (typeCode - 6 <= DataTypeCode.Unknown || typeCode - 9 <= DataTypeCode.Unknown)
			{
				return true;
			}
			return false;
		}

		internal static bool IsNumericDateVariant(DataTypeCode typeCode)
		{
			if (typeCode == DataTypeCode.Empty || typeCode - 6 <= DataTypeCode.Unknown || typeCode - 9 <= DataTypeCode.Boolean)
			{
				return true;
			}
			return false;
		}

		internal static bool IsNonNumericVariant(DataTypeCode typeCode)
		{
			if (typeCode == DataTypeCode.Boolean || typeCode == DataTypeCode.String)
			{
				return true;
			}
			return false;
		}

		internal static bool IsNumericLessThanZero(object value)
		{
			if (value is int)
			{
				return (int)value < 0;
			}
			if (value is double)
			{
				return (double)value < 0.0;
			}
			if (value is float)
			{
				return (float)value < 0f;
			}
			if (value is decimal)
			{
				return (decimal)value < 0m;
			}
			if (value is short)
			{
				return (short)value < 0;
			}
			if (value is long)
			{
				return (long)value < 0;
			}
			if (value is ushort)
			{
				return (ushort)value < 0;
			}
			if (value is uint)
			{
				return (uint)value < 0;
			}
			if (value is ulong)
			{
				return (ulong)value < 0;
			}
			if (value is byte)
			{
				return (byte)value < 0;
			}
			if (value is sbyte)
			{
				return (sbyte)value < 0;
			}
			return false;
		}

		internal static bool IsLessThanReal(DataTypeCode typeCode)
		{
			if (typeCode - 6 <= DataTypeCode.Unknown || typeCode == DataTypeCode.Decimal)
			{
				return true;
			}
			return false;
		}

		internal static bool IsLessThanCurrency(DataTypeCode typeCode)
		{
			if (typeCode - 6 <= DataTypeCode.Unknown)
			{
				return true;
			}
			return false;
		}

		internal static bool IsLessThanInt64(DataTypeCode typeCode)
		{
			return typeCode == DataTypeCode.Int32;
		}
	}
}
