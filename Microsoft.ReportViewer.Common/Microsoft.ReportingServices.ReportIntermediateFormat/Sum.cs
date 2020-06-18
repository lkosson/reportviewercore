using Microsoft.ReportingServices.Diagnostics.Utilities;
using Microsoft.ReportingServices.OnDemandProcessing;
using Microsoft.ReportingServices.OnDemandProcessing.Scalability;
using Microsoft.ReportingServices.ReportIntermediateFormat.Persistence;
using Microsoft.ReportingServices.ReportProcessing;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.ReportIntermediateFormat
{
	internal class Sum : DataAggregate
	{
		private Microsoft.ReportingServices.ReportProcessing.DataAggregate.DataTypeCode m_expressionType;

		protected Microsoft.ReportingServices.ReportProcessing.DataAggregate.DataTypeCode m_currentTotalType;

		protected object m_currentTotal;

		private static Declaration m_declaration = GetDeclaration();

		internal override DataAggregateInfo.AggregateTypes AggregateType => DataAggregateInfo.AggregateTypes.Sum;

		public override int Size => 8 + ItemSizes.SizeOf(m_currentTotal);

		internal override void Init()
		{
			m_currentTotalType = Microsoft.ReportingServices.ReportProcessing.DataAggregate.DataTypeCode.Null;
			m_currentTotal = null;
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
				if (m_currentTotal == null)
				{
					m_currentTotalType = doubleOrDecimalType;
					m_currentTotal = doubleOrDecimalData;
				}
				else
				{
					m_currentTotal = DataAggregate.Add(m_currentTotalType, m_currentTotal, doubleOrDecimalType, doubleOrDecimalData);
				}
			}
		}

		internal override object Result()
		{
			return m_currentTotal;
		}

		internal override DataAggregate ConstructAggregator(OnDemandProcessingContext odpContext, DataAggregateInfo aggregateDef)
		{
			return new Sum();
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
				case MemberName.CurrentTotalType:
					writer.WriteEnum((int)m_currentTotalType);
					break;
				case MemberName.CurrentTotal:
					writer.Write(m_currentTotal);
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
				case MemberName.CurrentTotalType:
					m_currentTotalType = (Microsoft.ReportingServices.ReportProcessing.DataAggregate.DataTypeCode)reader.ReadEnum();
					break;
				case MemberName.CurrentTotal:
					m_currentTotal = reader.ReadVariant();
					break;
				default:
					Global.Tracer.Assert(condition: false);
					break;
				}
			}
		}

		public override void ResolveReferences(Dictionary<Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
			Global.Tracer.Assert(condition: false, "Sum should not resolve references");
		}

		public override Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Sum;
		}

		public static Declaration GetDeclaration()
		{
			if (m_declaration == null)
			{
				List<MemberInfo> list = new List<MemberInfo>();
				list.Add(new MemberInfo(MemberName.ExpressionType, Token.Enum));
				list.Add(new MemberInfo(MemberName.CurrentTotalType, Token.Enum));
				list.Add(new MemberInfo(MemberName.CurrentTotal, Token.Object));
				return new Declaration(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Sum, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.DataAggregate, list);
			}
			return m_declaration;
		}
	}
}
