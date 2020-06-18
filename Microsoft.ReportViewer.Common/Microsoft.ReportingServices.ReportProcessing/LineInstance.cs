using Microsoft.ReportingServices.ReportProcessing.Persistence;
using System;

namespace Microsoft.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class LineInstance : ReportItemInstance
	{
		internal LineInstance(ReportProcessing.ProcessingContext pc, Line reportItemDef, int index)
			: base(pc.CreateUniqueName(), reportItemDef)
		{
			m_instanceInfo = new LineInstanceInfo(pc, reportItemDef, this, index);
		}

		internal LineInstance()
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
			return reader.ReadLineInstanceInfo((Line)m_reportItemDef);
		}
	}
}
