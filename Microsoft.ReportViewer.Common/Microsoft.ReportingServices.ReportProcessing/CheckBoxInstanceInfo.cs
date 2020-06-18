using Microsoft.ReportingServices.ReportProcessing.Persistence;
using System;

namespace Microsoft.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class CheckBoxInstanceInfo : ReportItemInstanceInfo
	{
		private bool m_value;

		private bool m_duplicate;

		internal bool Value
		{
			get
			{
				return m_value;
			}
			set
			{
				m_value = value;
			}
		}

		internal bool Duplicate
		{
			get
			{
				return m_duplicate;
			}
			set
			{
				m_duplicate = value;
			}
		}

		internal CheckBoxInstanceInfo(ReportProcessing.ProcessingContext pc, CheckBox reportItemDef, ReportItemInstance owner, int index)
			: base(pc, reportItemDef, owner, index)
		{
		}

		internal CheckBoxInstanceInfo(CheckBox reportItemDef)
			: base(reportItemDef)
		{
		}

		internal new static Declaration GetDeclaration()
		{
			MemberInfoList memberInfoList = new MemberInfoList();
			memberInfoList.Add(new MemberInfo(MemberName.Value, Token.Boolean));
			memberInfoList.Add(new MemberInfo(MemberName.Duplicate, Token.Boolean));
			return new Declaration(Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.ReportItemInstanceInfo, memberInfoList);
		}
	}
}
