using Microsoft.ReportingServices.ReportProcessing.Persistence;
using System;

namespace Microsoft.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class ActiveXControlInstance : ReportItemInstance
	{
		internal ActiveXControlInstanceInfo InstanceInfo
		{
			get
			{
				if (m_instanceInfo is OffsetInfo)
				{
					Global.Tracer.Assert(condition: false, string.Empty);
					return null;
				}
				return (ActiveXControlInstanceInfo)m_instanceInfo;
			}
		}

		internal ActiveXControlInstance(ReportProcessing.ProcessingContext pc, ActiveXControl reportItemDef, int index)
			: base(pc.CreateUniqueName(), reportItemDef)
		{
			m_instanceInfo = new ActiveXControlInstanceInfo(pc, reportItemDef, this, index);
		}

		internal ActiveXControlInstance()
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
			return reader.ReadActiveXControlInstanceInfo((ActiveXControl)m_reportItemDef);
		}
	}
}
