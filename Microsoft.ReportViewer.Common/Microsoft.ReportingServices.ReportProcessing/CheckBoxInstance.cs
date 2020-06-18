using Microsoft.ReportingServices.ReportProcessing.Persistence;
using System;

namespace Microsoft.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class CheckBoxInstance : ReportItemInstance
	{
		internal CheckBoxInstanceInfo InstanceInfo
		{
			get
			{
				if (m_instanceInfo is OffsetInfo)
				{
					Global.Tracer.Assert(condition: false, string.Empty);
					return null;
				}
				return (CheckBoxInstanceInfo)m_instanceInfo;
			}
		}

		internal CheckBoxInstance(ReportProcessing.ProcessingContext pc, CheckBox reportItemDef, int index)
			: base(pc.CreateUniqueName(), reportItemDef)
		{
			m_instanceInfo = new CheckBoxInstanceInfo(pc, reportItemDef, this, index);
		}

		internal CheckBoxInstance()
		{
		}

		internal new static Declaration GetDeclaration()
		{
			MemberInfoList members = new MemberInfoList();
			return new Declaration(Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.ReportItemInstance, members);
		}

		internal override ReportItemInstanceInfo ReadInstanceInfo(IntermediateFormatReader reader)
		{
			Global.Tracer.Assert(m_instanceInfo is OffsetInfo);
			return reader.ReadCheckBoxInstanceInfo((CheckBox)m_reportItemDef);
		}
	}
}
