using Microsoft.ReportingServices.Diagnostics.Utilities;
using Microsoft.ReportingServices.OnDemandProcessing;
using Microsoft.ReportingServices.ReportIntermediateFormat.Persistence;
using Microsoft.ReportingServices.ReportProcessing;
using System;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.ReportIntermediateFormat
{
	internal sealed class StDevP : VarP
	{
		private static Declaration m_declaration = GetDeclaration();

		internal override DataAggregateInfo.AggregateTypes AggregateType => DataAggregateInfo.AggregateTypes.StDevP;

		internal override object Result()
		{
			switch (m_sumOfXType)
			{
			case Microsoft.ReportingServices.ReportProcessing.DataAggregate.DataTypeCode.Null:
				return null;
			case Microsoft.ReportingServices.ReportProcessing.DataAggregate.DataTypeCode.Double:
				return Math.Sqrt((double)base.Result());
			case Microsoft.ReportingServices.ReportProcessing.DataAggregate.DataTypeCode.Decimal:
				return Math.Sqrt(Convert.ToDouble((decimal)base.Result()));
			default:
				Global.Tracer.Assert(condition: false);
				throw new ReportProcessingException(ErrorCode.rsInvalidOperation);
			}
		}

		internal override DataAggregate ConstructAggregator(OnDemandProcessingContext odpContext, DataAggregateInfo aggregateDef)
		{
			return new StDevP();
		}

		public override Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.StDevP;
		}

		public new static Declaration GetDeclaration()
		{
			if (m_declaration == null)
			{
				List<MemberInfo> memberInfoList = new List<MemberInfo>();
				return new Declaration(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.StDevP, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.VarP, memberInfoList);
			}
			return m_declaration;
		}
	}
}
