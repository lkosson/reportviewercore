using Microsoft.ReportingServices.ReportProcessing.Persistence;
using System;

namespace Microsoft.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class ListContentInstanceInfo : InstanceInfo
	{
		private bool m_startHidden;

		private string m_label;

		private DataValueInstanceList m_customPropertyInstances;

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

		internal string Label
		{
			get
			{
				return m_label;
			}
			set
			{
				m_label = value;
			}
		}

		internal DataValueInstanceList CustomPropertyInstances
		{
			get
			{
				return m_customPropertyInstances;
			}
			set
			{
				m_customPropertyInstances = value;
			}
		}

		internal ListContentInstanceInfo(ReportProcessing.ProcessingContext pc, ListContentInstance owner, List listDef)
		{
			if (pc.ShowHideType != 0)
			{
				m_startHidden = pc.ProcessReceiver(owner.UniqueName, listDef.Visibility, listDef.ExprHost, listDef.ObjectType, listDef.Name);
			}
			listDef.StartHidden = m_startHidden;
			if (listDef.Grouping != null)
			{
				if (listDef.Grouping.GroupLabel != null)
				{
					m_label = pc.NavigationInfo.RegisterLabel(pc.ReportRuntime.EvaluateGroupingLabelExpression(listDef.Grouping, listDef.ObjectType, listDef.Name));
				}
				if (listDef.Grouping.CustomProperties != null)
				{
					m_customPropertyInstances = listDef.Grouping.CustomProperties.EvaluateExpressions(listDef.ObjectType, listDef.Name, listDef.Grouping.Name + ".", pc);
				}
			}
			pc.ChunkManager.AddInstance(this, owner, pc.InPageSection);
		}

		internal ListContentInstanceInfo()
		{
		}

		internal new static Declaration GetDeclaration()
		{
			MemberInfoList memberInfoList = new MemberInfoList();
			memberInfoList.Add(new MemberInfo(MemberName.StartHidden, Token.Boolean));
			memberInfoList.Add(new MemberInfo(MemberName.Label, Token.String));
			memberInfoList.Add(new MemberInfo(MemberName.CustomPropertyInstances, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.DataValueInstanceList));
			return new Declaration(Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.InstanceInfo, memberInfoList);
		}
	}
}
