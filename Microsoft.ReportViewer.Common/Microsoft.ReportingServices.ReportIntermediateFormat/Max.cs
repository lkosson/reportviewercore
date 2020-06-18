using Microsoft.ReportingServices.Common;
using Microsoft.ReportingServices.Diagnostics.Utilities;
using Microsoft.ReportingServices.OnDemandProcessing;
using Microsoft.ReportingServices.OnDemandProcessing.Scalability;
using Microsoft.ReportingServices.ReportIntermediateFormat.Persistence;
using Microsoft.ReportingServices.ReportProcessing;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.ReportIntermediateFormat
{
	internal sealed class Max : DataAggregate
	{
		private Microsoft.ReportingServices.ReportProcessing.DataAggregate.DataTypeCode m_expressionType;

		private object m_currentMax;

		private IDataComparer m_comparer;

		private static Declaration m_declaration = GetDeclaration();

		internal override DataAggregateInfo.AggregateTypes AggregateType => DataAggregateInfo.AggregateTypes.Max;

		public override int Size => 4 + ItemSizes.SizeOf(m_currentMax) + ItemSizes.ReferenceSize;

		internal Max()
		{
		}

		internal Max(IDataComparer comparer)
		{
			m_currentMax = null;
			m_comparer = comparer;
		}

		internal override void Init()
		{
			m_currentMax = null;
		}

		internal override void Update(object[] expressions, IErrorContext iErrorContext)
		{
			object obj = expressions[0];
			Microsoft.ReportingServices.ReportProcessing.DataAggregate.DataTypeCode typeCode = DataAggregate.GetTypeCode(obj);
			if (DataAggregate.IsNull(typeCode))
			{
				return;
			}
			if (!DataAggregate.IsVariant(typeCode) || DataTypeUtility.IsSpatial(typeCode))
			{
				iErrorContext.Register(ProcessingErrorCode.rsMinMaxOfNonSortableData, Severity.Warning);
				throw new ReportProcessingException(ErrorCode.rsInvalidOperation);
			}
			if (m_currentMax == null)
			{
				m_currentMax = obj;
				m_expressionType = typeCode;
				return;
			}
			bool validComparisonResult;
			int num = m_comparer.Compare(m_currentMax, obj, throwExceptionOnComparisonFailure: false, extendedTypeComparisons: false, out validComparisonResult);
			if (!validComparisonResult)
			{
				if (typeCode != m_expressionType)
				{
					iErrorContext.Register(ProcessingErrorCode.rsAggregateOfMixedDataTypes, Severity.Warning);
					throw new ReportProcessingException(ErrorCode.rsInvalidOperation);
				}
				iErrorContext.Register(ProcessingErrorCode.rsMinMaxOfNonSortableData, Severity.Warning);
			}
			else if (num < 0)
			{
				m_currentMax = obj;
				m_expressionType = typeCode;
			}
		}

		internal override object Result()
		{
			return m_currentMax;
		}

		internal override DataAggregate ConstructAggregator(OnDemandProcessingContext odpContext, DataAggregateInfo aggregateDef)
		{
			return new Max(odpContext.ProcessingComparer);
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			writer.RegisterDeclaration(m_declaration);
			IScalabilityCache scalabilityCache = writer.PersistenceHelper as IScalabilityCache;
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.ExpressionType:
					writer.WriteEnum((int)m_expressionType);
					break;
				case MemberName.CurrentMax:
					writer.Write(m_currentMax);
					break;
				case MemberName.Comparer:
				{
					int value = scalabilityCache.StoreStaticReference(m_comparer);
					writer.Write(value);
					break;
				}
				default:
					Global.Tracer.Assert(condition: false);
					break;
				}
			}
		}

		public override void Deserialize(IntermediateFormatReader reader)
		{
			reader.RegisterDeclaration(m_declaration);
			IScalabilityCache scalabilityCache = reader.PersistenceHelper as IScalabilityCache;
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.ExpressionType:
					m_expressionType = (Microsoft.ReportingServices.ReportProcessing.DataAggregate.DataTypeCode)reader.ReadEnum();
					break;
				case MemberName.CurrentMax:
					m_currentMax = reader.ReadVariant();
					break;
				case MemberName.Comparer:
				{
					int id = reader.ReadInt32();
					m_comparer = (Microsoft.ReportingServices.ReportProcessing.ReportProcessing.ProcessingComparer)scalabilityCache.FetchStaticReference(id);
					break;
				}
				default:
					Global.Tracer.Assert(condition: false);
					break;
				}
			}
		}

		public override void ResolveReferences(Dictionary<Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
			Global.Tracer.Assert(condition: false, "Max should not resolve references");
		}

		public override Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Max;
		}

		public static Declaration GetDeclaration()
		{
			if (m_declaration == null)
			{
				List<MemberInfo> list = new List<MemberInfo>();
				list.Add(new MemberInfo(MemberName.ExpressionType, Token.Enum));
				list.Add(new MemberInfo(MemberName.CurrentMax, Token.Object));
				list.Add(new MemberInfo(MemberName.Comparer, Token.Int32));
				return new Declaration(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Max, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.None, list);
			}
			return m_declaration;
		}
	}
}
