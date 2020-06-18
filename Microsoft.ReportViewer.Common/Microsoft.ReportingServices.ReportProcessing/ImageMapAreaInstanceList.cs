using Microsoft.ReportingServices.ReportProcessing.Persistence;
using System;
using System.Collections;

namespace Microsoft.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class ImageMapAreaInstanceList : ArrayList
	{
		private int m_uniqueName;

		internal new ImageMapAreaInstance this[int index] => (ImageMapAreaInstance)base[index];

		internal int UniqueName
		{
			get
			{
				return m_uniqueName;
			}
			set
			{
				m_uniqueName = value;
			}
		}

		internal ImageMapAreaInstanceList()
		{
		}

		internal ImageMapAreaInstanceList(int capacity)
			: base(capacity)
		{
		}

		internal static Declaration GetDeclaration()
		{
			MemberInfoList memberInfoList = new MemberInfoList();
			memberInfoList.Add(new MemberInfo(MemberName.UniqueName, Token.Int32));
			return new Declaration(Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.None, memberInfoList);
		}
	}
}
