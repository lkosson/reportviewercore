using Microsoft.ReportingServices.Diagnostics.Utilities;
using Microsoft.ReportingServices.OnDemandProcessing;
using Microsoft.ReportingServices.OnDemandProcessing.Scalability;
using Microsoft.ReportingServices.ReportIntermediateFormat.Persistence;
using Microsoft.ReportingServices.ReportProcessing;
using System;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.ReportIntermediateFormat
{
	internal abstract class DataAggregate : IStorable, IPersistable
	{
		internal abstract DataAggregateInfo.AggregateTypes AggregateType
		{
			get;
		}

		public abstract int Size
		{
			get;
		}

		internal abstract void Init();

		internal abstract void Update(object[] expressions, IErrorContext iErrorContext);

		internal abstract object Result();

		internal abstract DataAggregate ConstructAggregator(OnDemandProcessingContext odpContext, DataAggregateInfo aggregateDef);

		internal static Microsoft.ReportingServices.ReportProcessing.DataAggregate.DataTypeCode GetTypeCode(object o)
		{
			bool valid;
			return GetTypeCode(o, throwException: true, out valid);
		}

		internal static Microsoft.ReportingServices.ReportProcessing.DataAggregate.DataTypeCode GetTypeCode(object o, bool throwException, out bool valid)
		{
			valid = true;
			if (o is string)
			{
				return Microsoft.ReportingServices.ReportProcessing.DataAggregate.DataTypeCode.String;
			}
			if (o is int)
			{
				return Microsoft.ReportingServices.ReportProcessing.DataAggregate.DataTypeCode.Int32;
			}
			if (o is double)
			{
				return Microsoft.ReportingServices.ReportProcessing.DataAggregate.DataTypeCode.Double;
			}
			if (o == null || DBNull.Value == o)
			{
				return Microsoft.ReportingServices.ReportProcessing.DataAggregate.DataTypeCode.Null;
			}
			if (o is ushort)
			{
				return Microsoft.ReportingServices.ReportProcessing.DataAggregate.DataTypeCode.UInt16;
			}
			if (o is short)
			{
				return Microsoft.ReportingServices.ReportProcessing.DataAggregate.DataTypeCode.Int16;
			}
			if (o is long)
			{
				return Microsoft.ReportingServices.ReportProcessing.DataAggregate.DataTypeCode.Int64;
			}
			if (o is decimal)
			{
				return Microsoft.ReportingServices.ReportProcessing.DataAggregate.DataTypeCode.Decimal;
			}
			if (o is uint)
			{
				return Microsoft.ReportingServices.ReportProcessing.DataAggregate.DataTypeCode.UInt32;
			}
			if (o is ulong)
			{
				return Microsoft.ReportingServices.ReportProcessing.DataAggregate.DataTypeCode.UInt64;
			}
			if (o is byte)
			{
				return Microsoft.ReportingServices.ReportProcessing.DataAggregate.DataTypeCode.Byte;
			}
			if (o is sbyte)
			{
				return Microsoft.ReportingServices.ReportProcessing.DataAggregate.DataTypeCode.SByte;
			}
			if (o is DateTime)
			{
				return Microsoft.ReportingServices.ReportProcessing.DataAggregate.DataTypeCode.DateTime;
			}
			if (o is char)
			{
				return Microsoft.ReportingServices.ReportProcessing.DataAggregate.DataTypeCode.Char;
			}
			if (o is bool)
			{
				return Microsoft.ReportingServices.ReportProcessing.DataAggregate.DataTypeCode.Boolean;
			}
			if (o is TimeSpan)
			{
				return Microsoft.ReportingServices.ReportProcessing.DataAggregate.DataTypeCode.TimeSpan;
			}
			if (o is DateTimeOffset)
			{
				return Microsoft.ReportingServices.ReportProcessing.DataAggregate.DataTypeCode.DateTimeOffset;
			}
			if (o is float)
			{
				return Microsoft.ReportingServices.ReportProcessing.DataAggregate.DataTypeCode.Single;
			}
			if (o is byte[])
			{
				return Microsoft.ReportingServices.ReportProcessing.DataAggregate.DataTypeCode.ByteArray;
			}
			valid = false;
			if (throwException)
			{
				throw new ReportProcessingException(ErrorCode.rsInvalidOperation);
			}
			return Microsoft.ReportingServices.ReportProcessing.DataAggregate.DataTypeCode.Null;
		}

		protected static bool IsNull(Microsoft.ReportingServices.ReportProcessing.DataAggregate.DataTypeCode typeCode)
		{
			return typeCode == Microsoft.ReportingServices.ReportProcessing.DataAggregate.DataTypeCode.Null;
		}

		protected static bool IsVariant(Microsoft.ReportingServices.ReportProcessing.DataAggregate.DataTypeCode typeCode)
		{
			return Microsoft.ReportingServices.ReportProcessing.DataAggregate.DataTypeCode.ByteArray != typeCode;
		}

		protected static void ConvertToDoubleOrDecimal(Microsoft.ReportingServices.ReportProcessing.DataAggregate.DataTypeCode numericType, object numericData, out Microsoft.ReportingServices.ReportProcessing.DataAggregate.DataTypeCode doubleOrDecimalType, out object doubleOrDecimalData)
		{
			if (Microsoft.ReportingServices.ReportProcessing.DataAggregate.DataTypeCode.Decimal == numericType)
			{
				doubleOrDecimalType = Microsoft.ReportingServices.ReportProcessing.DataAggregate.DataTypeCode.Decimal;
				doubleOrDecimalData = numericData;
			}
			else
			{
				doubleOrDecimalType = Microsoft.ReportingServices.ReportProcessing.DataAggregate.DataTypeCode.Double;
				doubleOrDecimalData = DataTypeUtility.ConvertToDouble(numericType, numericData);
			}
		}

		protected static object Add(Microsoft.ReportingServices.ReportProcessing.DataAggregate.DataTypeCode xType, object x, Microsoft.ReportingServices.ReportProcessing.DataAggregate.DataTypeCode yType, object y)
		{
			if (Microsoft.ReportingServices.ReportProcessing.DataAggregate.DataTypeCode.Double == xType && Microsoft.ReportingServices.ReportProcessing.DataAggregate.DataTypeCode.Double == yType)
			{
				return (double)x + (double)y;
			}
			if (Microsoft.ReportingServices.ReportProcessing.DataAggregate.DataTypeCode.Decimal == xType && Microsoft.ReportingServices.ReportProcessing.DataAggregate.DataTypeCode.Decimal == yType)
			{
				return (decimal)x + (decimal)y;
			}
			Global.Tracer.Assert(condition: false);
			throw new ReportProcessingException(ErrorCode.rsInvalidOperation);
		}

		protected static object Square(Microsoft.ReportingServices.ReportProcessing.DataAggregate.DataTypeCode xType, object x)
		{
			if (Microsoft.ReportingServices.ReportProcessing.DataAggregate.DataTypeCode.Double == xType)
			{
				return (double)x * (double)x;
			}
			if (Microsoft.ReportingServices.ReportProcessing.DataAggregate.DataTypeCode.Decimal == xType)
			{
				return (decimal)x * (decimal)x;
			}
			Global.Tracer.Assert(condition: false);
			throw new ReportProcessingException(ErrorCode.rsInvalidOperation);
		}

		public abstract void Serialize(IntermediateFormatWriter writer);

		public abstract void Deserialize(IntermediateFormatReader reader);

		public abstract void ResolveReferences(Dictionary<Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems);

		public abstract Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType();
	}
}
