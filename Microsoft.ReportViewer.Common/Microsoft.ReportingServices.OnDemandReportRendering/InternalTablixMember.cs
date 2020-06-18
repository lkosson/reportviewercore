using Microsoft.ReportingServices.ReportIntermediateFormat;
using Microsoft.ReportingServices.ReportProcessing;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class InternalTablixMember : TablixMember
	{
		private Microsoft.ReportingServices.ReportIntermediateFormat.TablixMember m_memberDef;

		private bool m_customPropertyCollectionReady;

		private IReportScope m_reportScope;

		internal override string UniqueName => m_memberDef.UniqueName;

		public override string ID => m_memberDef.RenderingModelID;

		public override string DataElementName => m_memberDef.DataElementName;

		public override DataElementOutputTypes DataElementOutput => m_memberDef.DataElementOutput;

		public override TablixHeader TablixHeader
		{
			get
			{
				if (m_header == null && m_memberDef.TablixHeader != null)
				{
					m_header = new TablixHeader(base.OwnerTablix, this);
				}
				return m_header;
			}
		}

		public override TablixMemberCollection Children
		{
			get
			{
				TablixMemberList subMembers = m_memberDef.SubMembers;
				if (subMembers == null)
				{
					return null;
				}
				if (m_children == null)
				{
					m_children = new InternalTablixMemberCollection(this, base.OwnerTablix, this, subMembers);
				}
				return m_children;
			}
		}

		public override CustomPropertyCollection CustomProperties
		{
			get
			{
				if (m_customPropertyCollection == null)
				{
					string objectName = (m_memberDef.Grouping != null) ? m_memberDef.Grouping.Name : base.OwnerTablix.Name;
					m_customPropertyCollection = new CustomPropertyCollection(ReportScope.ReportScopeInstance, base.OwnerTablix.RenderingContext, null, m_memberDef, ObjectType.Tablix, objectName);
					m_customPropertyCollectionReady = true;
				}
				else if (!m_customPropertyCollectionReady)
				{
					string objectName2 = (m_memberDef.Grouping != null) ? m_memberDef.Grouping.Name : base.OwnerTablix.Name;
					m_customPropertyCollection.UpdateCustomProperties(ReportScope.ReportScopeInstance, m_memberDef, base.OwnerTablix.RenderingContext.OdpContext, ObjectType.Tablix, objectName2);
					m_customPropertyCollectionReady = true;
				}
				return m_customPropertyCollection;
			}
		}

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

		public override bool IsColumn => m_memberDef.IsColumn;

		internal override int RowSpan => m_memberDef.RowSpan;

		internal override int ColSpan => m_memberDef.ColSpan;

		public override int MemberCellIndex => m_memberDef.MemberCellIndex;

		public override bool IsTotal => m_memberDef.IsAutoSubtotal;

		internal override PageBreakLocation PropagatedGroupBreak
		{
			get
			{
				if (!IsStatic)
				{
					PageBreak pageBreak = m_group.PageBreak;
					if (pageBreak.Instance != null && !pageBreak.Instance.Disabled)
					{
						return pageBreak.BreakLocation;
					}
				}
				return PageBreakLocation.None;
			}
		}

		public override Visibility Visibility
		{
			get
			{
				if (m_visibility == null && m_memberDef.Visibility != null && !m_memberDef.IsAutoSubtotal)
				{
					m_visibility = new InternalTablixMemberVisibility(this);
				}
				return m_visibility;
			}
		}

		public override bool HideIfNoRows => m_memberDef.HideIfNoRows;

		public override bool KeepTogether => m_memberDef.KeepTogether;

		internal override Microsoft.ReportingServices.ReportIntermediateFormat.TablixMember MemberDefinition => m_memberDef;

		public override bool FixedData => MemberDefinition.FixedData;

		public override KeepWithGroup KeepWithGroup => MemberDefinition.KeepWithGroup;

		public override bool RepeatOnNewPage => MemberDefinition.RepeatOnNewPage;

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

		public override TablixMemberInstance Instance
		{
			get
			{
				if (base.OwnerTablix.RenderingContext.InstanceAccessDisallowed)
				{
					return null;
				}
				if (m_instance == null)
				{
					if (IsStatic)
					{
						m_instance = new TablixMemberInstance(base.OwnerTablix, this);
					}
					else
					{
						TablixDynamicMemberInstance instance = new TablixDynamicMemberInstance(base.OwnerTablix, this, BuildOdpMemberLogic(base.OwnerTablix.RenderingContext.OdpContext));
						m_owner.RenderingContext.AddDynamicInstance(instance);
						m_instance = instance;
					}
				}
				return m_instance;
			}
		}

		internal InternalTablixMember(IReportScope reportScope, IDefinitionPath parentDefinitionPath, Tablix owner, TablixMember parent, Microsoft.ReportingServices.ReportIntermediateFormat.TablixMember memberDef, int index)
			: base(parentDefinitionPath, owner, parent, index)
		{
			if (memberDef.IsStatic)
			{
				m_reportScope = reportScope;
			}
			m_owner = owner;
			m_memberDef = memberDef;
			if (m_memberDef.Grouping != null)
			{
				m_group = new Group(base.OwnerTablix, m_memberDef, this);
			}
			m_memberDef.ROMScopeInstance = ReportScope.ReportScopeInstance;
			m_memberDef.ResetVisibilityComputationCache();
		}

		internal override void SetNewContext(bool fromMoveNext)
		{
			if (!fromMoveNext && m_instance != null && !IsStatic)
			{
				((IDynamicInstance)m_instance).ResetContext();
			}
			base.SetNewContext(fromMoveNext);
			m_customPropertyCollectionReady = false;
			m_memberDef.ResetTextBoxImpls(m_owner.m_renderingContext.OdpContext);
		}
	}
}
