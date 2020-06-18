using Microsoft.ReportingServices.ReportProcessing.Persistence;
using System;

namespace Microsoft.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class TableGroupInstanceInfo : InstanceInfo
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

		internal TableGroupInstanceInfo(ReportProcessing.ProcessingContext pc, TableGroup tableGroupDef, TableGroupInstance owner)
		{
			if (pc.ShowHideType != 0)
			{
				m_startHidden = pc.ProcessReceiver(owner.UniqueName, tableGroupDef.Visibility, tableGroupDef.ExprHost, tableGroupDef.DataRegionDef.ObjectType, tableGroupDef.DataRegionDef.Name);
			}
			tableGroupDef.StartHidden = m_startHidden;
			if (tableGroupDef.Grouping.GroupLabel != null)
			{
				m_label = pc.NavigationInfo.RegisterLabel(pc.ReportRuntime.EvaluateGroupingLabelExpression(tableGroupDef.Grouping, tableGroupDef.DataRegionDef.ObjectType, tableGroupDef.DataRegionDef.Name));
			}
			if (tableGroupDef.Grouping.CustomProperties != null)
			{
				m_customPropertyInstances = tableGroupDef.Grouping.CustomProperties.EvaluateExpressions(tableGroupDef.DataRegionDef.ObjectType, tableGroupDef.DataRegionDef.Name, tableGroupDef.Grouping.Name + ".", pc);
			}
			pc.ChunkManager.AddInstance(this, owner, pc.InPageSection);
		}

		internal TableGroupInstanceInfo()
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
