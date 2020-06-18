using Microsoft.ReportingServices.Diagnostics.Utilities;
using System;
using System.Globalization;
using System.Threading;

namespace Microsoft.ReportingServices.ReportProcessing
{
	internal abstract class DataTypeUtility
	{
		internal static bool IsSpatial(DataAggregate.DataTypeCode typeCode)
		{
			if (typeCode != DataAggregate.DataTypeCode.SqlGeography)
			{
				return typeCode == DataAggregate.DataTypeCode.SqlGeometry;
			}
			return true;
		}

		internal static bool IsNumeric(DataAggregate.DataTypeCode typeCode)
		{
			if ((uint)(typeCode - 4) <= 8u || (uint)(typeCode - 14) <= 2u)
			{
				return true;
			}
			return false;
		}

		internal static bool IsFloat(DataAggregate.DataTypeCode typeCode)
		{
			if ((uint)(typeCode - 14) <= 1u)
			{
				return true;
			}
			return false;
		}

		internal static bool IsSigned(DataAggregate.DataTypeCode typeCode)
		{
			if ((uint)(typeCode - 4) <= 2u || typeCode == DataAggregate.DataTypeCode.SByte)
			{
				return true;
			}
			return false;
		}

		internal static bool IsUnsigned(DataAggregate.DataTypeCode typeCode)
		{
			if ((uint)(typeCode - 7) <= 3u)
			{
				return true;
			}
			return false;
		}

		internal static bool Is32BitOrLess(DataAggregate.DataTypeCode typeCode)
		{
			switch (typeCode)
			{
			case DataAggregate.DataTypeCode.Int16:
			case DataAggregate.DataTypeCode.Int32:
			case DataAggregate.DataTypeCode.UInt16:
			case DataAggregate.DataTypeCode.UInt32:
			case DataAggregate.DataTypeCode.Byte:
			case DataAggregate.DataTypeCode.SByte:
				return true;
			default:
				return false;
			}
		}

		internal static bool Is64BitOrLess(DataAggregate.DataTypeCode typeCode)
		{
			if ((uint)(typeCode - 4) <= 7u)
			{
				return true;
			}
			return false;
		}

		internal static double ConvertToDouble(DataAggregate.DataTypeCode typeCode, object data)
		{
			switch (typeCode)
			{
			case DataAggregate.DataTypeCode.Double:
				return (double)data;
			case DataAggregate.DataTypeCode.Int16:
				return (short)data;
			case DataAggregate.DataTypeCode.Int32:
				return (int)data;
			case DataAggregate.DataTypeCode.Int64:
				return (long)data;
			case DataAggregate.DataTypeCode.UInt16:
				return (int)(ushort)data;
			case DataAggregate.DataTypeCode.UInt32:
				return (uint)data;
			case DataAggregate.DataTypeCode.UInt64:
				return (ulong)data;
			case DataAggregate.DataTypeCode.Byte:
				return (int)(byte)data;
			case DataAggregate.DataTypeCode.SByte:
				return (sbyte)data;
			case DataAggregate.DataTypeCode.TimeSpan:
				return ((TimeSpan)data).Ticks;
			case DataAggregate.DataTypeCode.Single:
				return (float)data;
			case DataAggregate.DataTypeCode.Decimal:
				return Convert.ToDouble((decimal)data);
			default:
				Global.Tracer.Assert(condition: false);
				throw new ReportProcessingException(ErrorCode.rsInvalidOperation);
			}
		}

		internal static int ConvertToInt32(DataAggregate.DataTypeCode typeCode, object data, out bool valid)
		{
			valid = true;
			switch (typeCode)
			{
			case DataAggregate.DataTypeCode.Int16:
				return (short)data;
			case DataAggregate.DataTypeCode.Int32:
				return (int)data;
			case DataAggregate.DataTypeCode.UInt16:
				return (ushort)data;
			case DataAggregate.DataTypeCode.UInt32:
				if ((uint)data <= int.MaxValue)
				{
					return (int)data;
				}
				break;
			case DataAggregate.DataTypeCode.UInt64:
				if ((ulong)data <= int.MaxValue)
				{
					return (int)data;
				}
				break;
			case DataAggregate.DataTypeCode.Int64:
				if ((long)data <= int.MaxValue && (long)data >= int.MinValue)
				{
					return (int)data;
				}
				break;
			case DataAggregate.DataTypeCode.Byte:
				return (byte)data;
			case DataAggregate.DataTypeCode.SByte:
				return (sbyte)data;
			}
			valid = false;
			return 0;
		}

		internal static string ConvertToInvariantString(object o)
		{
			if (o == null)
			{
				return null;
			}
			string text = null;
			CultureInfo currentCulture = Thread.CurrentThread.CurrentCulture;
			Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
			try
			{
				return o.ToString();
			}
			finally
			{
				Thread.CurrentThread.CurrentCulture = currentCulture;
			}
		}

		internal static Type GetNumericTypeFromDataTypeCode(DataAggregate.DataTypeCode typeCode)
		{
			switch (typeCode)
			{
			case DataAggregate.DataTypeCode.Int32:
				return typeof(int);
			case DataAggregate.DataTypeCode.Int64:
				return typeof(long);
			case DataAggregate.DataTypeCode.UInt32:
				return typeof(uint);
			case DataAggregate.DataTypeCode.UInt64:
				return typeof(ulong);
			case DataAggregate.DataTypeCode.Single:
				return typeof(float);
			case DataAggregate.DataTypeCode.Double:
				return typeof(double);
			case DataAggregate.DataTypeCode.Decimal:
				return typeof(decimal);
			case DataAggregate.DataTypeCode.Int16:
				return typeof(short);
			case DataAggregate.DataTypeCode.UInt16:
				return typeof(ushort);
			default:
				return null;
			}
		}

		internal static DataAggregate.DataTypeCode CommonNumericDenominator(DataAggregate.DataTypeCode x, DataAggregate.DataTypeCode y)
		{
			if (!IsNumeric(x) || !IsNumeric(y))
			{
				return DataAggregate.DataTypeCode.Null;
			}
			if (x == y)
			{
				return x;
			}
			if (IsSigned(x) && IsSigned(y))
			{
				if (DataAggregate.DataTypeCode.Int64 == x || DataAggregate.DataTypeCode.Int64 == y)
				{
					return DataAggregate.DataTypeCode.Int64;
				}
				return DataAggregate.DataTypeCode.Int32;
			}
			if (IsUnsigned(x) && IsUnsigned(y))
			{
				if (DataAggregate.DataTypeCode.UInt64 == x || DataAggregate.DataTypeCode.UInt64 == y)
				{
					return DataAggregate.DataTypeCode.UInt64;
				}
				return DataAggregate.DataTypeCode.UInt32;
			}
			if (IsFloat(x) && IsFloat(y))
			{
				return DataAggregate.DataTypeCode.Double;
			}
			if (IsSigned(x) && IsUnsigned(y))
			{
				return CommonDataTypeSignedUnsigned(x, y);
			}
			if (IsUnsigned(x) && IsSigned(y))
			{
				return CommonDataTypeSignedUnsigned(y, x);
			}
			if ((Is32BitOrLess(x) && IsFloat(y)) || (Is32BitOrLess(y) && IsFloat(x)))
			{
				return DataAggregate.DataTypeCode.Double;
			}
			if ((Is64BitOrLess(x) && DataAggregate.DataTypeCode.Decimal == y) || (Is64BitOrLess(y) && DataAggregate.DataTypeCode.Decimal == x))
			{
				return DataAggregate.DataTypeCode.Decimal;
			}
			return DataAggregate.DataTypeCode.Null;
		}

		internal static bool IsNumericLessThanZero(object value, DataAggregate.DataTypeCode dataType)
		{
			switch (dataType)
			{
			case DataAggregate.DataTypeCode.Int32:
				return (int)value < 0;
			case DataAggregate.DataTypeCode.Double:
				return (double)value < 0.0;
			case DataAggregate.DataTypeCode.Single:
				return (float)value < 0f;
			case DataAggregate.DataTypeCode.Decimal:
				return (decimal)value < 0m;
			case DataAggregate.DataTypeCode.Int16:
				return (short)value < 0;
			case DataAggregate.DataTypeCode.Int64:
				return (long)value < 0;
			case DataAggregate.DataTypeCode.UInt16:
				return (ushort)value < 0;
			case DataAggregate.DataTypeCode.UInt32:
				return (uint)value < 0;
			case DataAggregate.DataTypeCode.UInt64:
				return (ulong)value < 0;
			case DataAggregate.DataTypeCode.Byte:
				return (byte)value < 0;
			case DataAggregate.DataTypeCode.SByte:
				return (sbyte)value < 0;
			default:
				return false;
			}
		}

		private static DataAggregate.DataTypeCode CommonDataTypeSignedUnsigned(DataAggregate.DataTypeCode signed, DataAggregate.DataTypeCode unsigned)
		{
			Global.Tracer.Assert(IsSigned(signed) && IsUnsigned(unsigned), "(IsSigned(signed) && IsUnsigned(unsigned))");
			if (DataAggregate.DataTypeCode.UInt64 == unsigned)
			{
				return DataAggregate.DataTypeCode.Null;
			}
			if (DataAggregate.DataTypeCode.UInt32 == unsigned)
			{
				return DataAggregate.DataTypeCode.Int64;
			}
			if (DataAggregate.DataTypeCode.Int64 == signed)
			{
				return DataAggregate.DataTypeCode.Int64;
			}
			return DataAggregate.DataTypeCode.Int32;
		}
	}
}
