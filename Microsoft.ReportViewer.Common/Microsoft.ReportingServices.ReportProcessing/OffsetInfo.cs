using Microsoft.ReportingServices.ReportProcessing.Persistence;
using System;

namespace Microsoft.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class OffsetInfo : InfoBase
	{
		private long m_offset;

		internal long Offset
		{
			get
			{
				return m_offset;
			}
			set
			{
				m_offset = value;
			}
		}

		internal OffsetInfo()
		{
		}

		internal OffsetInfo(long offset)
		{
			m_offset = offset;
		}

		internal new static Declaration GetDeclaration()
		{
			MemberInfoList memberInfoList = new MemberInfoList();
			memberInfoList.Add(new MemberInfo(MemberName.Offset, Token.Int64));
			return new Declaration(Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.InfoBase, memberInfoList);
		}
	}
}
