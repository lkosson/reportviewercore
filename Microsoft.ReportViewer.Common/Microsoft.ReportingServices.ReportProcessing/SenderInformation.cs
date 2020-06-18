using Microsoft.ReportingServices.ReportProcessing.Persistence;
using System;

namespace Microsoft.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class SenderInformation
	{
		private bool m_startHidden;

		private IntList m_receiverUniqueNames;

		private int[] m_containerUniqueNames;

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

		internal IntList ReceiverUniqueNames
		{
			get
			{
				return m_receiverUniqueNames;
			}
			set
			{
				m_receiverUniqueNames = value;
			}
		}

		internal int[] ContainerUniqueNames
		{
			get
			{
				return m_containerUniqueNames;
			}
			set
			{
				m_containerUniqueNames = value;
			}
		}

		internal SenderInformation()
		{
		}

		internal SenderInformation(bool startHidden, int[] containerUniqueNames)
		{
			m_startHidden = startHidden;
			m_receiverUniqueNames = new IntList();
			m_containerUniqueNames = containerUniqueNames;
		}

		internal static Declaration GetDeclaration()
		{
			MemberInfoList memberInfoList = new MemberInfoList();
			memberInfoList.Add(new MemberInfo(MemberName.Hidden, Token.Boolean));
			memberInfoList.Add(new MemberInfo(MemberName.ReceiverUniqueNames, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.IntList));
			memberInfoList.Add(new MemberInfo(MemberName.ContainerUniqueNames, Token.TypedArray));
			return new Declaration(Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.None, memberInfoList);
		}
	}
}
