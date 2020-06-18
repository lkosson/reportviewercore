using Microsoft.ReportingServices.ReportProcessing.Persistence;
using System;

namespace Microsoft.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class BookmarkInformation
	{
		private string m_id;

		private int m_page;

		internal string Id
		{
			get
			{
				return m_id;
			}
			set
			{
				m_id = value;
			}
		}

		internal int Page
		{
			get
			{
				return m_page;
			}
			set
			{
				m_page = value;
			}
		}

		internal BookmarkInformation()
		{
		}

		internal BookmarkInformation(string id, int page)
		{
			m_id = id;
			m_page = page;
		}

		internal static Declaration GetDeclaration()
		{
			MemberInfoList memberInfoList = new MemberInfoList();
			memberInfoList.Add(new MemberInfo(MemberName.Id, Token.String));
			memberInfoList.Add(new MemberInfo(MemberName.Page, Token.Int32));
			return new Declaration(Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.None, memberInfoList);
		}
	}
}
