using Microsoft.ReportingServices.ReportIntermediateFormat;
using Microsoft.ReportingServices.ReportProcessing;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class InternalDataMember : DataMember
	{
		private Microsoft.ReportingServices.ReportIntermediateFormat.DataMember m_memberDef;

		private IReportScope m_reportScope;

		private string m_uniqueName;

		private bool m_customPropertyCollectionReady;

		internal override string UniqueName
		{
			get
			{
				if (m_uniqueName != null)
				{
					m_uniqueName = m_memberDef.UniqueName;
				}
				return m_uniqueName;
			}
		}

		public override int ColSpan => m_memberDef.ColSpan;

		public override CustomPropertyCollection CustomProperties
		{
			get
			{
				if (m_customPropertyCollection == null)
				{
					string objectName = (m_memberDef.Grouping != null) ? m_memberDef.Grouping.Name : base.OwnerCri.Name;
					m_customPropertyCollection = new CustomPropertyCollection(ReportScope.ReportScopeInstance, base.OwnerCri.RenderingContext, null, m_memberDef, ObjectType.CustomReportItem, objectName);
					m_customPropertyCollectionReady = true;
				}
				else if (!m_customPropertyCollectionReady)
				{
					string objectName2 = (m_memberDef.Grouping != null) ? m_memberDef.Grouping.Name : base.OwnerCri.Name;
					m_customPropertyCollection.UpdateCustomProperties(ReportScope.ReportScopeInstance, m_memberDef, base.OwnerCri.RenderingContext.OdpContext, ObjectType.CustomReportItem, objectName2);
					m_customPropertyCollectionReady = true;
				}
				return m_customPropertyCollection;
			}
		}

		public override string ID => m_memberDef.RenderingModelID;

		public override bool IsColumn => m_memberDef.IsColumn;

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

		public override int MemberCellIndex => m_memberDef.MemberCellIndex;

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

		public override int RowSpan => m_memberDef.RowSpan;

		public override DataMemberInstance Instance
		{
			get
			{
				if (base.OwnerCri.RenderingContext.InstanceAccessDisallowed)
				{
					return null;
				}
				if (m_instance == null)
				{
					if (IsStatic)
					{
						m_instance = new DataMemberInstance(base.OwnerCri, this);
					}
					else
					{
						DataDynamicMemberInstance instance = new DataDynamicMemberInstance(base.OwnerCri, this, BuildOdpMemberLogic(base.OwnerCri.RenderingContext.OdpContext));
						m_owner.RenderingContext.AddDynamicInstance(instance);
						m_instance = instance;
					}
				}
				return m_instance;
			}
		}

		internal override Microsoft.ReportingServices.ReportIntermediateFormat.DataMember MemberDefinition => m_memberDef;

		public override DataMemberCollection Children
		{
			get
			{
				DataMemberList subMembers = m_memberDef.SubMembers;
				if (subMembers == null)
				{
					return null;
				}
				if (m_children == null)
				{
					m_children = new InternalDataMemberCollection(this, base.OwnerCri, this, subMembers);
				}
				return m_children;
			}
		}

		internal InternalDataMember(IReportScope reportScope, IDefinitionPath parentDefinitionPath, CustomReportItem owner, DataMember parent, Microsoft.ReportingServices.ReportIntermediateFormat.DataMember memberDef, int parentCollectionIndex)
			: base(parentDefinitionPath, owner, parent, parentCollectionIndex)
		{
			m_memberDef = memberDef;
			if (m_memberDef.IsStatic)
			{
				m_reportScope = reportScope;
			}
			m_group = new Group(owner, m_memberDef, this);
		}

		internal override void SetNewContext(bool fromMoveNext)
		{
			if (!fromMoveNext && m_instance != null && !IsStatic)
			{
				((IDynamicInstance)m_instance).ResetContext();
			}
			base.SetNewContext(fromMoveNext);
			m_customPropertyCollectionReady = false;
		}
	}
}
