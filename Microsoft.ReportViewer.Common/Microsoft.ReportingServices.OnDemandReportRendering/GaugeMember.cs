using Microsoft.ReportingServices.ReportIntermediateFormat;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class GaugeMember : DataRegionMember
	{
		private GaugeMemberCollection m_children;

		private GaugeMemberInstance m_instance;

		private Microsoft.ReportingServices.ReportIntermediateFormat.GaugeMember m_memberDef;

		private IReportScope m_reportScope;

		private string m_uniqueName;

		public GaugeMember Parent => m_parent as GaugeMember;

		internal override string UniqueName
		{
			get
			{
				if (m_uniqueName == null)
				{
					m_uniqueName = m_memberDef.UniqueName;
				}
				return m_uniqueName;
			}
		}

		public override string ID => m_memberDef.RenderingModelID;

		public override bool IsStatic
		{
			get
			{
				if (m_memberDef.Grouping == null)
				{
					return true;
				}
				return false;
			}
		}

		public bool IsColumn => m_memberDef.IsColumn;

		public int RowSpan => m_memberDef.RowSpan;

		public int ColumnSpan => m_memberDef.ColSpan;

		public override int MemberCellIndex => m_memberDef.MemberCellIndex;

		internal Microsoft.ReportingServices.ReportIntermediateFormat.GaugeMember MemberDefinition => m_memberDef;

		internal override ReportHierarchyNode DataRegionMemberDefinition => MemberDefinition;

		internal override IReportScope ReportScope
		{
			get
			{
				if (IsStatic)
				{
					return m_reportScope;
				}
				return this;
			}
		}

		internal override IRIFReportScope RIFReportScope
		{
			get
			{
				if (IsStatic)
				{
					return m_reportScope.RIFReportScope;
				}
				return MemberDefinition;
			}
		}

		internal override IReportScopeInstance ReportScopeInstance
		{
			get
			{
				if (IsStatic)
				{
					return m_reportScope.ReportScopeInstance;
				}
				return (IReportScopeInstance)Instance;
			}
		}

		internal GaugePanel OwnerGaugePanel => m_owner as GaugePanel;

		public GaugeMemberInstance Instance
		{
			get
			{
				if (OwnerGaugePanel.RenderingContext.InstanceAccessDisallowed)
				{
					return null;
				}
				if (m_instance == null)
				{
					if (IsStatic)
					{
						m_instance = new GaugeMemberInstance(OwnerGaugePanel, this);
					}
					else
					{
						GaugeDynamicMemberInstance instance = new GaugeDynamicMemberInstance(OwnerGaugePanel, this, BuildOdpMemberLogic(OwnerGaugePanel.RenderingContext.OdpContext));
						m_owner.RenderingContext.AddDynamicInstance(instance);
						m_instance = instance;
					}
				}
				return m_instance;
			}
		}

		public GaugeMember ChildGaugeMember
		{
			get
			{
				if (m_children != null && m_children.Count == 1)
				{
					return m_children[0];
				}
				return null;
			}
		}

		internal override IDataRegionMemberCollection SubMembers
		{
			get
			{
				if (m_children == null && m_memberDef.InnerHierarchy != null)
				{
					GaugeMemberList gaugeMemberList = (GaugeMemberList)m_memberDef.InnerHierarchy;
					if (gaugeMemberList == null)
					{
						return null;
					}
					m_children = new GaugeMemberCollection(this, OwnerGaugePanel, this, gaugeMemberList);
				}
				return m_children;
			}
		}

		internal GaugeMember(IReportScope reportScope, IDefinitionPath parentDefinitionPath, GaugePanel owner, GaugeMember parent, Microsoft.ReportingServices.ReportIntermediateFormat.GaugeMember memberDef)
			: base(parentDefinitionPath, owner, parent, 0)
		{
			m_memberDef = memberDef;
			if (m_memberDef.IsStatic)
			{
				m_reportScope = reportScope;
			}
			m_group = new Group(owner, m_memberDef, this);
		}

		internal GaugeMember(IDefinitionPath parentDefinitionPath, GaugePanel owner, GaugeMember parent)
			: base(parentDefinitionPath, owner, parent, 0)
		{
		}

		internal override bool GetIsColumn()
		{
			return IsColumn;
		}

		internal override void SetNewContext(bool fromMoveNext)
		{
			if (!fromMoveNext && m_instance != null && !IsStatic)
			{
				((IDynamicInstance)m_instance).ResetContext();
			}
			base.SetNewContext(fromMoveNext);
			if (m_instance != null)
			{
				m_instance.SetNewContext();
			}
		}
	}
}
