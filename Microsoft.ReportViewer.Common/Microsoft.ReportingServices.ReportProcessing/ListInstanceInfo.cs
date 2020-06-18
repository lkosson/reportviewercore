using Microsoft.ReportingServices.ReportProcessing.Persistence;
using System;

namespace Microsoft.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class ListInstanceInfo : ReportItemInstanceInfo
	{
		private string m_noRows;

		internal string NoRows
		{
			get
			{
				return m_noRows;
			}
			set
			{
				m_noRows = value;
			}
		}

		internal ListInstanceInfo(ReportProcessing.ProcessingContext pc, List reportItemDef, ListInstance owner)
			: base(pc, reportItemDef, owner, addToChunk: true)
		{
			m_noRows = pc.ReportRuntime.EvaluateDataRegionNoRowsExpression(reportItemDef, reportItemDef.ObjectType, reportItemDef.Name, "NoRows");
		}

		internal ListInstanceInfo(List reportItemDef)
			: base(reportItemDef)
		{
		}

		internal new static Declaration GetDeclaration()
		{
			MemberInfoList memberInfoList = new MemberInfoList();
			memberInfoList.Add(new MemberInfo(MemberName.NoRows, Token.String));
			return new Declaration(Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.ReportItemInstanceInfo, memberInfoList);
		}
	}
}
