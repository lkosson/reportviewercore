using Microsoft.ReportingServices.ReportIntermediateFormat;
using Microsoft.ReportingServices.ReportProcessing;
using Microsoft.ReportingServices.ReportRendering;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class ShimChartMember : ChartMember, IShimDataRegionMember
	{
		private bool m_isCategory;

		private int m_definitionStartIndex = -1;

		private int m_definitionEndIndex = -1;

		private Microsoft.ReportingServices.ReportRendering.ChartMember m_staticOrSubtotal;

		internal override string UniqueName => ID;

		public override string ID
		{
			get
			{
				if (IsStatic)
				{
					return base.DefinitionPath;
				}
				return ((Microsoft.ReportingServices.ReportRendering.ChartMember)m_group.CurrentShimRenderGroup).ID;
			}
		}

		public override ReportStringProperty Label
		{
			get
			{
				if (m_label == null)
				{
					Microsoft.ReportingServices.ReportProcessing.ExpressionInfo expressionInfo = null;
					expressionInfo = ((!IsStatic) ? ((Microsoft.ReportingServices.ReportRendering.ChartMember)m_group.CurrentShimRenderGroup).LabelDefinition : m_staticOrSubtotal.LabelDefinition);
					m_label = new ReportStringProperty(expressionInfo);
				}
				return m_label;
			}
		}

		internal object LabelInstanceValue
		{
			get
			{
				if (IsStatic)
				{
					return m_staticOrSubtotal.LabelValue;
				}
				return ((Microsoft.ReportingServices.ReportRendering.ChartMember)m_group.CurrentShimRenderGroup).LabelValue;
			}
		}

		public override string DataElementName
		{
			get
			{
				if (m_staticOrSubtotal != null)
				{
					return m_staticOrSubtotal.DataElementName;
				}
				if (m_group != null && m_group.CurrentShimRenderGroup != null)
				{
					return m_group.CurrentShimRenderGroup.DataCollectionName;
				}
				return null;
			}
		}

		public override DataElementOutputTypes DataElementOutput
		{
			get
			{
				if (m_staticOrSubtotal != null)
				{
					return (DataElementOutputTypes)m_staticOrSubtotal.DataElementOutput;
				}
				return DataElementOutputTypes.Output;
			}
		}

		public override ChartMemberCollection Children => m_children;

		public override CustomPropertyCollection CustomProperties
		{
			get
			{
				if (m_customPropertyCollection == null)
				{
					if (m_group != null && m_group.CustomProperties != null)
					{
						m_customPropertyCollection = m_group.CustomProperties;
					}
					else
					{
						m_customPropertyCollection = new CustomPropertyCollection();
					}
				}
				return m_customPropertyCollection;
			}
		}

		public override bool IsStatic => m_staticOrSubtotal != null;

		public override bool IsCategory => m_isCategory;

		public override int SeriesSpan
		{
			get
			{
				if (m_isCategory)
				{
					return 1;
				}
				return m_definitionEndIndex - m_definitionStartIndex;
			}
		}

		public override int CategorySpan
		{
			get
			{
				if (m_isCategory)
				{
					return m_definitionEndIndex - m_definitionStartIndex;
				}
				return 1;
			}
		}

		public override int MemberCellIndex => m_definitionStartIndex;

		public override bool IsTotal => false;

		internal override Microsoft.ReportingServices.ReportIntermediateFormat.ChartMember MemberDefinition => null;

		internal override IReportScope ReportScope => null;

		internal override IRIFReportScope RIFReportScope => null;

		internal override IReportScopeInstance ReportScopeInstance => null;

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
						ChartDynamicMemberInstance instance = new ChartDynamicMemberInstance(base.OwnerChart, this, new InternalShimDynamicMemberLogic(this));
						m_owner.RenderingContext.AddDynamicInstance(instance);
						m_instance = instance;
					}
				}
				return m_instance;
			}
		}

		internal int DefinitionStartIndex => m_definitionStartIndex;

		internal int DefinitionEndIndex => m_definitionEndIndex;

		internal Microsoft.ReportingServices.ReportRendering.ChartMember CurrentRenderChartMember
		{
			get
			{
				if (m_staticOrSubtotal != null)
				{
					return m_staticOrSubtotal;
				}
				return m_group.CurrentShimRenderGroup as Microsoft.ReportingServices.ReportRendering.ChartMember;
			}
		}

		internal ShimChartMember(IDefinitionPath parentDefinitionPath, Chart owner, ShimChartMember parent, int parentCollectionIndex, bool isCategory, Microsoft.ReportingServices.ReportRendering.ChartMember staticOrSubtotal)
			: base(parentDefinitionPath, owner, parent, parentCollectionIndex)
		{
			m_definitionStartIndex = owner.GetCurrentMemberCellDefinitionIndex();
			m_isCategory = isCategory;
			m_staticOrSubtotal = staticOrSubtotal;
			GenerateInnerHierarchy(owner, parent, isCategory, staticOrSubtotal.Children);
			m_definitionEndIndex = owner.GetCurrentMemberCellDefinitionIndex();
		}

		internal ShimChartMember(IDefinitionPath parentDefinitionPath, Chart owner, ShimChartMember parent, int parentCollectionIndex, bool isCategory, ShimRenderGroups renderGroups)
			: base(parentDefinitionPath, owner, parent, parentCollectionIndex)
		{
			m_definitionStartIndex = owner.GetCurrentMemberCellDefinitionIndex();
			m_isCategory = isCategory;
			m_group = new Group(owner, renderGroups);
			GenerateInnerHierarchy(owner, parent, isCategory, ((Microsoft.ReportingServices.ReportRendering.ChartMember)renderGroups[0]).Children);
			m_definitionEndIndex = owner.GetCurrentMemberCellDefinitionIndex();
		}

		private void GenerateInnerHierarchy(Chart owner, ShimChartMember parent, bool isCategory, Microsoft.ReportingServices.ReportRendering.ChartMemberCollection children)
		{
			if (children != null)
			{
				m_children = new ShimChartMemberCollection(this, owner, isCategory, this, children);
			}
			else
			{
				owner.GetAndIncrementMemberCellDefinitionIndex();
			}
		}

		internal bool SetNewContext(int index)
		{
			base.ResetContext();
			if (m_instance != null)
			{
				m_instance.SetNewContext();
			}
			if (m_group != null)
			{
				if (base.OwnerChart.RenderChart.NoRows)
				{
					return false;
				}
				if (index < 0 || index >= m_group.RenderGroups.Count)
				{
					return false;
				}
				m_group.CurrentRenderGroupIndex = index;
				UpdateInnerContext(m_group.RenderGroups[index] as Microsoft.ReportingServices.ReportRendering.ChartMember);
				return true;
			}
			return index <= 1;
		}

		internal override void ResetContext()
		{
			ResetContext(null, null);
		}

		internal void ResetContext(Microsoft.ReportingServices.ReportRendering.ChartMember staticOrSubtotal, ShimRenderGroups renderGroups)
		{
			if (m_group != null)
			{
				m_group.CurrentRenderGroupIndex = -1;
				if (renderGroups != null)
				{
					m_group.RenderGroups = renderGroups;
				}
			}
			else if (staticOrSubtotal != null)
			{
				m_staticOrSubtotal = staticOrSubtotal;
			}
			Microsoft.ReportingServices.ReportRendering.ChartMember currentRenderGroup = IsStatic ? m_staticOrSubtotal : (m_group.CurrentShimRenderGroup as Microsoft.ReportingServices.ReportRendering.ChartMember);
			UpdateInnerContext(currentRenderGroup);
		}

		private void UpdateInnerContext(Microsoft.ReportingServices.ReportRendering.ChartMember currentRenderGroup)
		{
			if (m_children != null)
			{
				((ShimChartMemberCollection)m_children).ResetContext(currentRenderGroup.Children);
			}
			else
			{
				((ShimChartSeriesCollection)base.OwnerChart.ChartData.SeriesCollection).UpdateCells(this);
			}
		}

		bool IShimDataRegionMember.SetNewContext(int index)
		{
			return SetNewContext(index);
		}

		void IShimDataRegionMember.ResetContext()
		{
			ResetContext();
		}
	}
}
