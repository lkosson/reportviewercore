using Microsoft.ReportingServices.ReportProcessing.Persistence;
using System;

namespace Microsoft.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class ImageInstance : ReportItemInstance
	{
		internal ImageInstanceInfo InstanceInfo
		{
			get
			{
				if (m_instanceInfo is OffsetInfo)
				{
					Global.Tracer.Assert(condition: false, string.Empty);
					return null;
				}
				return (ImageInstanceInfo)m_instanceInfo;
			}
		}

		internal ImageInstance(ReportProcessing.ProcessingContext pc, Image reportItemDef, int index)
			: base(pc.CreateUniqueName(), reportItemDef)
		{
			m_instanceInfo = new ImageInstanceInfo(pc, reportItemDef, this, index, customCreated: false);
		}

		internal ImageInstance(ReportProcessing.ProcessingContext pc, Image reportItemDef, int index, bool customCreated)
			: base(pc.CreateUniqueName(), reportItemDef)
		{
			m_instanceInfo = new ImageInstanceInfo(pc, reportItemDef, this, index, customCreated);
		}

		internal ImageInstance()
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
			return reader.ReadImageInstanceInfo((Image)m_reportItemDef);
		}
	}
}
