using Microsoft.ReportingServices.ReportProcessing.Persistence;
using System;

namespace Microsoft.ReportingServices.ReportProcessing
{
	[Serializable]
	internal abstract class InstanceInfoOwner
	{
		protected InfoBase m_instanceInfo;

		internal OffsetInfo OffsetInfo
		{
			get
			{
				if (m_instanceInfo == null)
				{
					return null;
				}
				Global.Tracer.Assert(m_instanceInfo is OffsetInfo);
				return (OffsetInfo)m_instanceInfo;
			}
			set
			{
				m_instanceInfo = value;
			}
		}

		internal long ChunkOffset
		{
			get
			{
				if (m_instanceInfo == null || !(m_instanceInfo is OffsetInfo))
				{
					return 0L;
				}
				return ((OffsetInfo)m_instanceInfo).Offset;
			}
		}

		internal void SetOffset(long offset)
		{
			m_instanceInfo = new OffsetInfo(offset);
		}

		internal static Declaration GetDeclaration()
		{
			MemberInfoList memberInfoList = new MemberInfoList();
			memberInfoList.Add(new MemberInfo(MemberName.OffsetInfo, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.OffsetInfo));
			return new Declaration(Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.None, memberInfoList);
		}
	}
}
