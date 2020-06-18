using System;

namespace Microsoft.ReportingServices.ReportProcessing
{
	internal abstract class DataAggregate
	{
		internal enum DataTypeCode
		{
			Null,
			String,
			Char,
			Boolean,
			Int16,
			Int32,
			Int64,
			UInt16,
			UInt32,
			UInt64,
			Byte,
			SByte,
			TimeSpan,
			DateTime,
			Single,
			Double,
			Decimal,
			ByteArray,
			DateTimeOffset,
			SqlGeography,
			SqlGeometry
		}

		internal abstract void Init();

		internal abstract void Update(object[] expressions, IErrorContext iErrorContext);

		internal abstract object Result();

		internal static DataTypeCode GetTypeCode(object o)
		{
			bool valid;
			return GetTypeCode(o, throwException: true, out valid);
		}

		internal static DataTypeCode GetTypeCode(object o, bool throwException, out bool valid)
		{
			valid = true;
			if (o is string)
			{
				return DataTypeCode.String;
			}
			if (o is int)
			{
				return DataTypeCode.Int32;
			}
			if (o is double)
			{
				return DataTypeCode.Double;
			}
			if (o == null || DBNull.Value == o)
			{
				return DataTypeCode.Null;
			}
			if (o is ushort)
			{
				return DataTypeCode.UInt16;
			}
			if (o is short)
			{
				return DataTypeCode.Int16;
			}
			if (o is long)
			{
				return DataTypeCode.Int64;
			}
			if (o is decimal)
			{
				return DataTypeCode.Decimal;
			}
			if (o is uint)
			{
				return DataTypeCode.UInt32;
			}
			if (o is ulong)
			{
				return DataTypeCode.UInt64;
			}
			if (o is byte)
			{
				return DataTypeCode.Byte;
			}
			if (o is sbyte)
			{
				return DataTypeCode.SByte;
			}
			if (o is DateTime)
			{
				return DataTypeCode.DateTime;
			}
			if (o is DateTimeOffset)
			{
				return DataTypeCode.DateTimeOffset;
			}
			if (o is char)
			{
				return DataTypeCode.Char;
			}
			if (o is bool)
			{
				return DataTypeCode.Boolean;
			}
			if (o is TimeSpan)
			{
				return DataTypeCode.TimeSpan;
			}
			if (o is float)
			{
				return DataTypeCode.Single;
			}
			if (o is byte[])
			{
				return DataTypeCode.ByteArray;
			}
			valid = false;
			if (throwException)
			{
				throw new InvalidOperationException();
			}
			return DataTypeCode.Null;
		}

		protected static bool IsNull(DataTypeCode typeCode)
		{
			return typeCode == DataTypeCode.Null;
		}

		protected static bool IsVariant(DataTypeCode typeCode)
		{
			return DataTypeCode.ByteArray != typeCode;
		}

		protected static void ConvertToDoubleOrDecimal(DataTypeCode numericType, object numericData, out DataTypeCode doubleOrDecimalType, out object doubleOrDecimalData)
		{
			if (DataTypeCode.Decimal == numericType)
			{
				doubleOrDecimalType = DataTypeCode.Decimal;
				doubleOrDecimalData = numericData;
			}
			else
			{
				doubleOrDecimalType = DataTypeCode.Double;
				doubleOrDecimalData = DataTypeUtility.ConvertToDouble(numericType, numericData);
			}
		}

		protected static object Add(DataTypeCode xType, object x, DataTypeCode yType, object y)
		{
			if (DataTypeCode.Double == xType && DataTypeCode.Double == yType)
			{
				return (double)x + (double)y;
			}
			if (DataTypeCode.Decimal == xType && DataTypeCode.Decimal == yType)
			{
				return (decimal)x + (decimal)y;
			}
			Global.Tracer.Assert(condition: false);
			throw new InvalidOperationException();
		}

		protected static object Square(DataTypeCode xType, object x)
		{
			if (DataTypeCode.Double == xType)
			{
				return (double)x * (double)x;
			}
			if (DataTypeCode.Decimal == xType)
			{
				return (decimal)x * (decimal)x;
			}
			Global.Tracer.Assert(condition: false);
			throw new InvalidOperationException();
		}
	}
}
