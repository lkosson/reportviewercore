using System.Collections.Generic;

namespace Microsoft.ReportingServices.ReportIntermediateFormat.Persistence
{
	internal class Declaration
	{
		private List<MemberInfo> m_memberInfoList = new List<MemberInfo>();

		private ObjectType m_type;

		private ObjectType m_baseType;

		private Pair<bool, int>[] m_usableMembers;

		private bool m_hasSkippedMembers;

		internal List<MemberInfo> MemberInfoList => m_memberInfoList;

		internal ObjectType ObjectType => m_type;

		internal ObjectType BaseObjectType => m_baseType;

		internal bool RegisteredCurrentDeclaration => m_usableMembers != null;

		internal bool HasSkippedMembers => m_hasSkippedMembers;

		internal Declaration(ObjectType type, ObjectType baseType, List<MemberInfo> memberInfoList)
		{
			m_type = type;
			m_baseType = baseType;
			m_memberInfoList = memberInfoList;
		}

		internal bool IsMemberSkipped(int index)
		{
			if (m_hasSkippedMembers)
			{
				return m_usableMembers[index].First;
			}
			return false;
		}

		internal int MembersToSkip(int index)
		{
			if (m_hasSkippedMembers)
			{
				return m_usableMembers[index].Second;
			}
			return 0;
		}

		internal void RegisterCurrentDeclaration(Declaration currentDeclaration)
		{
			m_hasSkippedMembers = false;
			m_usableMembers = new Pair<bool, int>[m_memberInfoList.Count];
			int num = 0;
			for (int num2 = m_memberInfoList.Count - 1; num2 >= 0; num2--)
			{
				if (currentDeclaration.Contains(m_memberInfoList[num2]))
				{
					num = 0;
				}
				else
				{
					m_hasSkippedMembers = true;
					num++;
					m_usableMembers[num2].Second = num;
					m_usableMembers[num2].First = true;
				}
			}
			if (!m_hasSkippedMembers)
			{
				m_usableMembers = new Pair<bool, int>[0];
			}
		}

		private bool Contains(MemberInfo otherMember)
		{
			return m_memberInfoList.Contains(otherMember);
		}

		internal Declaration CreateFilteredDeclarationForWriteVersion(int compatVersion)
		{
			List<MemberInfo> list = new List<MemberInfo>(m_memberInfoList.Count);
			for (int i = 0; i < m_memberInfoList.Count; i++)
			{
				MemberInfo memberInfo = m_memberInfoList[i];
				if (memberInfo.IsWrittenForCompatVersion(compatVersion))
				{
					list.Add(memberInfo);
				}
			}
			return new Declaration(m_type, m_baseType, list);
		}
	}
}
