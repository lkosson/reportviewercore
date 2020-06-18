using Microsoft.ReportingServices.Diagnostics.Utilities;
using Microsoft.ReportingServices.OnDemandProcessing;
using Microsoft.ReportingServices.ReportIntermediateFormat.Persistence;
using Microsoft.ReportingServices.ReportProcessing;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.ReportIntermediateFormat
{
	internal class Var : VarBase
	{
		private static Declaration m_declaration = GetDeclaration();

		internal override DataAggregateInfo.AggregateTypes AggregateType => DataAggregateInfo.AggregateTypes.Var;

		internal override object Result()
		{
			if (1 == m_currentCount)
			{
				return null;
			}
			switch (m_sumOfXType)
			{
			case Microsoft.ReportingServices.ReportProcessing.DataAggregate.DataTypeCode.Null:
				return null;
			case Microsoft.ReportingServices.ReportProcessing.DataAggregate.DataTypeCode.Double:
				return ((double)m_currentCount * (double)m_sumOfXSquared - (double)m_sumOfX * (double)m_sumOfX) / (double)(m_currentCount * (m_currentCount - 1));
			case Microsoft.ReportingServices.ReportProcessing.DataAggregate.DataTypeCode.Decimal:
				return ((decimal)m_currentCount * (decimal)m_sumOfXSquared - (decimal)m_sumOfX * (decimal)m_sumOfX) / (decimal)(m_currentCount * (m_currentCount - 1));
			default:
				Global.Tracer.Assert(condition: false);
				throw new ReportProcessingException(ErrorCode.rsInvalidOperation);
			}
		}

		internal override DataAggregate ConstructAggregator(OnDemandProcessingContext odpContext, DataAggregateInfo aggregateDef)
		{
			return new Var();
		}

		public override Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Var;
		}

		public new static Declaration GetDeclaration()
		{
			if (m_declaration == null)
			{
				List<MemberInfo> memberInfoList = new List<MemberInfo>();
				return new Declaration(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Var, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.VarBase, memberInfoList);
			}
			return m_declaration;
		}
	}
}
