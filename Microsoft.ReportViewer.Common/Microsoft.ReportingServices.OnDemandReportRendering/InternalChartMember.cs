using Microsoft.ReportingServices.ReportIntermediateFormat;
using Microsoft.ReportingServices.ReportProcessing;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class InternalChartMember : ChartMember
	{
		private Microsoft.ReportingServices.ReportIntermediateFormat.ChartMember m_memberDef;

		private IReportScope m_reportScope;

		private bool m_customPropertyCollectionReady;

		private string m_uniqueName;

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

		public override ReportStringProperty Label
		{
			get
			{
				if (m_label == null)
				{
					m_label = new ReportStringProperty(m_memberDef.Label);
				}
				return m_label;
			}
		}

		public override string DataElementName => m_memberDef.DataElementName;

		public override DataElementOutputTypes DataElementOutput => m_memberDef.DataElementOutput;

		public override ChartMemberCollection Children
		{
			get
			{
				ChartMemberList chartMembers = m_memberDef.ChartMembers;
				if (chartMembers == null)
				{
					return null;
				}
				if (m_children == null)
				{
					m_children = new InternalChartMemberCollection(this, base.OwnerChart, this, chartMembers);
				}
				return m_children;
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

		public override bool IsCategory => m_memberDef.IsColumn;

		public override int SeriesSpan => m_memberDef.RowSpan;

		public override int CategorySpan => m_memberDef.ColSpan;

		public override int MemberCellIndex => m_memberDef.MemberCellIndex;

		public override bool IsTotal => m_memberDef.IsAutoSubtotal;

		internal override Microsoft.ReportingServices.ReportIntermediateFormat.ChartMember MemberDefinition => m_memberDef;

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

		public override CustomPropertyCollection CustomProperties
		{
			get
			{
				if (m_customPropertyCollection == null)
				{
					string objectName = (m_memberDef.Grouping != null) ? m_memberDef.Grouping.Name : base.OwnerChart.Name;
					m_customPropertyCollection = new CustomPropertyCollection(ReportScope.ReportScopeInstance, base.OwnerChart.RenderingContext, null, m_memberDef, ObjectType.Chart, objectName);
					m_customPropertyCollectionReady = true;
				}
				else if (!m_customPropertyCollectionReady)
				{
					string objectName2 = (m_memberDef.Grouping != null) ? m_memberDef.Grouping.Name : base.OwnerChart.Name;
					m_customPropertyCollection.UpdateCustomProperties(ReportScope.ReportScopeInstance, m_memberDef, base.OwnerChart.RenderingContext.OdpContext, ObjectType.Chart, objectName2);
					m_customPropertyCollectionReady = true;
				}
				return m_customPropertyCollection;
			}
		}

		public override ChartMemberInstance Instance
		{
			get
			{
				if (base.OwnerChart.RenderingContext.InstanceAccessDisallowed)
				{
					return null;
				}
				if (m_instance == null)
				{
					if (IsStatic)
					{
						m_instance = new ChartMemberInstance(base.OwnerChart, this);
					}
					else
					{
						ChartDynamicMemberInstance instance = new ChartDynamicMemberInstance(base.OwnerChart, this, BuildOdpMemberLogic(base.OwnerChart.RenderingContext.OdpContext));
						m_owner.RenderingContext.AddDynamicInstance(instance);
						m_instance = instance;
					}
				}
				return m_instance;
			}
		}

		internal InternalChartMember(IReportScope reportScope, IDefinitionPath parentDefinitionPath, Chart owner, ChartMember parent, Microsoft.ReportingServices.ReportIntermediateFormat.ChartMember memberDef, int parentCollectionIndex)
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
			m_uniqueName = null;
		}
	}
}
