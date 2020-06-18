using Microsoft.ReportingServices.Diagnostics.Utilities;
using Microsoft.ReportingServices.OnDemandProcessing.Scalability;
using Microsoft.ReportingServices.ReportIntermediateFormat.Persistence;
using Microsoft.ReportingServices.ReportProcessing;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.ReportIntermediateFormat
{
	internal abstract class VarBase : DataAggregate
	{
		private Microsoft.ReportingServices.ReportProcessing.DataAggregate.DataTypeCode m_expressionType;

		protected uint m_currentCount;

		protected Microsoft.ReportingServices.ReportProcessing.DataAggregate.DataTypeCode m_sumOfXType;

		protected object m_sumOfX;

		protected object m_sumOfXSquared;

		private static Declaration m_declaration = GetDeclaration();

		public override int Size => 12 + ItemSizes.SizeOf(m_sumOfX) + ItemSizes.SizeOf(m_sumOfXSquared);

		internal override void Init()
		{
			m_currentCount = 0u;
			m_sumOfXType = Microsoft.ReportingServices.ReportProcessing.DataAggregate.DataTypeCode.Null;
			m_sumOfX = null;
			m_sumOfXSquared = null;
		}

		internal override void Update(object[] expressions, IErrorContext iErrorContext)
		{
			object doubleOrDecimalData = expressions[0];
			Microsoft.ReportingServices.ReportProcessing.DataAggregate.DataTypeCode doubleOrDecimalType = DataAggregate.GetTypeCode(doubleOrDecimalData);
			if (!DataAggregate.IsNull(doubleOrDecimalType))
			{
				if (!DataTypeUtility.IsNumeric(doubleOrDecimalType))
				{
					iErrorContext.Register(ProcessingErrorCode.rsAggregateOfNonNumericData, Severity.Warning);
					throw new ReportProcessingException(ErrorCode.rsInvalidOperation);
				}
				DataAggregate.ConvertToDoubleOrDecimal(doubleOrDecimalType, doubleOrDecimalData, out doubleOrDecimalType, out doubleOrDecimalData);
				if (m_expressionType == Microsoft.ReportingServices.ReportProcessing.DataAggregate.DataTypeCode.Null)
				{
					m_expressionType = doubleOrDecimalType;
				}
				else if (doubleOrDecimalType != m_expressionType)
				{
					iErrorContext.Register(ProcessingErrorCode.rsAggregateOfMixedDataTypes, Severity.Warning);
					throw new ReportProcessingException(ErrorCode.rsInvalidOperation);
				}
				object obj = DataAggregate.Square(doubleOrDecimalType, doubleOrDecimalData);
				if (m_sumOfX == null)
				{
					m_sumOfXType = doubleOrDecimalType;
					m_sumOfX = doubleOrDecimalData;
					m_sumOfXSquared = obj;
				}
				else
				{
					m_sumOfX = DataAggregate.Add(m_sumOfXType, m_sumOfX, doubleOrDecimalType, doubleOrDecimalData);
					m_sumOfXSquared = DataAggregate.Add(m_sumOfXType, m_sumOfXSquared, doubleOrDecimalType, obj);
				}
				m_currentCount++;
			}
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			writer.RegisterDeclaration(m_declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.ExpressionType:
					writer.WriteEnum((int)m_expressionType);
					break;
				case MemberName.CurrentCount:
					writer.Write(m_currentCount);
					break;
				case MemberName.SumOfXType:
					writer.WriteEnum((int)m_sumOfXType);
					break;
				case MemberName.SumOfX:
					writer.Write(m_sumOfX);
					break;
				case MemberName.SumOfXSquared:
					writer.Write(m_sumOfXSquared);
					break;
				default:
					Global.Tracer.Assert(condition: false);
					break;
				}
			}
		}

		public override void Deserialize(IntermediateFormatReader reader)
		{
			reader.RegisterDeclaration(m_declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.ExpressionType:
					m_expressionType = (Microsoft.ReportingServices.ReportProcessing.DataAggregate.DataTypeCode)reader.ReadEnum();
					break;
				case MemberName.CurrentCount:
					m_currentCount = reader.ReadUInt32();
					break;
				case MemberName.SumOfXType:
					m_sumOfXType = (Microsoft.ReportingServices.ReportProcessing.DataAggregate.DataTypeCode)reader.ReadEnum();
					break;
				case MemberName.SumOfX:
					m_sumOfX = reader.ReadVariant();
					break;
				case MemberName.SumOfXSquared:
					m_sumOfXSquared = reader.ReadVariant();
					break;
				default:
					Global.Tracer.Assert(condition: false);
					break;
				}
			}
		}

		public override void ResolveReferences(Dictionary<Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
			Global.Tracer.Assert(condition: false, "VarBase should not resolve references");
		}

		public override Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.VarBase;
		}

		public static Declaration GetDeclaration()
		{
			if (m_declaration == null)
			{
				List<MemberInfo> list = new List<MemberInfo>();
				list.Add(new MemberInfo(MemberName.ExpressionType, Token.Enum));
				list.Add(new MemberInfo(MemberName.CurrentCount, Token.UInt32));
				list.Add(new MemberInfo(MemberName.SumOfXType, Token.Enum));
				list.Add(new MemberInfo(MemberName.SumOfX, Token.Object));
				list.Add(new MemberInfo(MemberName.SumOfXSquared, Token.Object));
				return new Declaration(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.VarBase, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.None, list);
			}
			return m_declaration;
		}
	}
}
