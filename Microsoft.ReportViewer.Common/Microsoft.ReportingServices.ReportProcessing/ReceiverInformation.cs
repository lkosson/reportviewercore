using Microsoft.ReportingServices.ReportProcessing.Persistence;
using System;

namespace Microsoft.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class ReceiverInformation
	{
		private bool m_startHidden;

		private int m_senderUniqueName;

		internal bool StartHidden
		{
			get
			{
				return m_startHidden;
			}
			set
			{
				m_startHidden = value;
			}
		}

		internal int SenderUniqueName
		{
			get
			{
				return m_senderUniqueName;
			}
			set
			{
				m_senderUniqueName = value;
			}
		}

		internal ReceiverInformation()
		{
		}

		internal ReceiverInformation(bool startHidden, int senderUniqueName)
		{
			m_startHidden = startHidden;
			m_senderUniqueName = senderUniqueName;
		}

		internal static Declaration GetDeclaration()
		{
			MemberInfoList memberInfoList = new MemberInfoList();
			memberInfoList.Add(new MemberInfo(MemberName.StartHidden, Token.Boolean));
			memberInfoList.Add(new MemberInfo(MemberName.SenderUniqueName, Token.Int32));
			return new Declaration(Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.None, memberInfoList);
		}
	}
}
