using Microsoft.ReportingServices.ReportProcessing.Persistence;
using System;

namespace Microsoft.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class SubReportInstanceInfo : ReportItemInstanceInfo
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

		internal SubReportInstanceInfo(ReportProcessing.ProcessingContext pc, SubReport reportItemDef, SubReportInstance owner, int index)
			: base(pc, reportItemDef, owner, index)
		{
			m_noRows = pc.ReportRuntime.EvaluateSubReportNoRowsExpression(reportItemDef, reportItemDef.Name, "NoRows");
		}

		internal SubReportInstanceInfo(SubReport reportItemDef)
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
