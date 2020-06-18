using Microsoft.ReportingServices.ReportProcessing.Persistence;
using System;

namespace Microsoft.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class TextBoxInstanceInfo : ReportItemInstanceInfo, IShowHideSender
	{
		private string m_formattedValue;

		private object m_originalValue;

		private bool m_duplicate;

		private ActionInstance m_action;

		private bool m_initialToggleState;

		internal string FormattedValue
		{
			get
			{
				return m_formattedValue;
			}
			set
			{
				m_formattedValue = value;
			}
		}

		internal object OriginalValue
		{
			get
			{
				return m_originalValue;
			}
			set
			{
				m_originalValue = value;
			}
		}

		internal bool Duplicate
		{
			get
			{
				return m_duplicate;
			}
			set
			{
				m_duplicate = value;
			}
		}

		internal ActionInstance Action
		{
			get
			{
				return m_action;
			}
			set
			{
				m_action = value;
			}
		}

		internal bool InitialToggleState
		{
			get
			{
				return m_initialToggleState;
			}
			set
			{
				m_initialToggleState = value;
			}
		}

		internal TextBoxInstanceInfo(ReportProcessing.ProcessingContext pc, TextBox reportItemDef, TextBoxInstance owner, int index)
			: base(pc, reportItemDef, owner, index)
		{
		}

		internal TextBoxInstanceInfo(TextBox reportItemDef)
			: base(reportItemDef)
		{
		}

		void IShowHideSender.ProcessSender(ReportProcessing.ProcessingContext context, int uniqueName)
		{
			m_initialToggleState = context.ProcessSender(uniqueName, m_startHidden, (TextBox)m_reportItemDef);
		}

		internal new static Declaration GetDeclaration()
		{
			MemberInfoList memberInfoList = new MemberInfoList();
			memberInfoList.Add(new MemberInfo(MemberName.FormattedValue, Token.String));
			memberInfoList.Add(new MemberInfo(MemberName.OriginalValue, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.Variant));
			memberInfoList.Add(new MemberInfo(MemberName.Duplicate, Token.Boolean));
			memberInfoList.Add(new MemberInfo(MemberName.Action, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.ActionInstance));
			memberInfoList.Add(new MemberInfo(MemberName.InitialToggleState, Token.Boolean));
			return new Declaration(Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.ReportItemInstanceInfo, memberInfoList);
		}
	}
}
