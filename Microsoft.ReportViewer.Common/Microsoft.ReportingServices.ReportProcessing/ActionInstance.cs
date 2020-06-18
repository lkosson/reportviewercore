using Microsoft.ReportingServices.ReportProcessing.Persistence;
using System;

namespace Microsoft.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class ActionInstance
	{
		private ActionItemInstanceList m_actionItemsValues;

		private object[] m_styleAttributeValues;

		private int m_uniqueName;

		internal ActionItemInstanceList ActionItemsValues
		{
			get
			{
				return m_actionItemsValues;
			}
			set
			{
				m_actionItemsValues = value;
			}
		}

		internal object[] StyleAttributeValues
		{
			get
			{
				return m_styleAttributeValues;
			}
			set
			{
				m_styleAttributeValues = value;
			}
		}

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

		internal ActionInstance(ReportProcessing.ProcessingContext pc)
		{
			m_uniqueName = pc.CreateUniqueName();
		}

		internal ActionInstance(ActionItemInstance actionItemInstance)
		{
			m_actionItemsValues = new ActionItemInstanceList();
			m_actionItemsValues.Add(actionItemInstance);
		}

		internal ActionInstance()
		{
		}

		internal object GetStyleAttributeValue(int index)
		{
			Global.Tracer.Assert(m_styleAttributeValues != null && 0 <= index && index < m_styleAttributeValues.Length);
			return m_styleAttributeValues[index];
		}

		internal static Declaration GetDeclaration()
		{
			MemberInfoList memberInfoList = new MemberInfoList();
			memberInfoList.Add(new MemberInfo(MemberName.ActionItemList, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.ActionItemInstanceList));
			memberInfoList.Add(new MemberInfo(MemberName.StyleAttributeValues, Token.Array, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.Variant));
			memberInfoList.Add(new MemberInfo(MemberName.UniqueName, Token.Int32));
			return new Declaration(Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.None, memberInfoList);
		}
	}
}
