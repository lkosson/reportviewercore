using Microsoft.ReportingServices.ReportIntermediateFormat;
using Microsoft.ReportingServices.ReportRendering;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class ShimDataMember : DataMember, IShimDataRegionMember
	{
		private bool m_isColumn;

		private bool m_isStatic;

		private int m_staticIndex = -1;

		private int m_definitionStartIndex = -1;

		private int m_definitionEndIndex = -1;

		private Microsoft.ReportingServices.ReportRendering.DataMemberCollection m_renderMembers;

		internal override string UniqueName => ID;

		public override string ID
		{
			get
			{
				if (m_isStatic)
				{
					return m_renderMembers[m_staticIndex].ID;
				}
				return ((Microsoft.ReportingServices.ReportRendering.DataMember)m_group.CurrentShimRenderGroup).ID;
			}
		}

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

		public override bool IsStatic => m_isStatic;

		public override bool IsColumn => m_isColumn;

		public override int RowSpan
		{
			get
			{
				if (m_isColumn)
				{
					if (m_isStatic)
					{
						return m_renderMembers[m_staticIndex].MemberHeadingSpan;
					}
					return ((Microsoft.ReportingServices.ReportRendering.DataMember)m_group.CurrentShimRenderGroup).MemberHeadingSpan;
				}
				return m_definitionEndIndex - m_definitionStartIndex;
			}
		}

		public override int ColSpan
		{
			get
			{
				if (m_isColumn)
				{
					return m_definitionEndIndex - m_definitionStartIndex;
				}
				if (IsStatic)
				{
					return m_renderMembers[m_staticIndex].MemberHeadingSpan;
				}
				return ((Microsoft.ReportingServices.ReportRendering.DataMember)m_group.CurrentShimRenderGroup).MemberHeadingSpan;
			}
		}

		public override int MemberCellIndex => m_definitionStartIndex;

		internal override Microsoft.ReportingServices.ReportIntermediateFormat.DataMember MemberDefinition => null;

		internal override IRIFReportScope RIFReportScope => null;

		internal override IReportScopeInstance ReportScopeInstance => null;

		internal override IReportScope ReportScope => null;

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
						DataDynamicMemberInstance instance = new DataDynamicMemberInstance(base.OwnerCri, this, new InternalShimDynamicMemberLogic(this));
						base.OwnerCri.RenderingContext.AddDynamicInstance(instance);
						m_instance = instance;
					}
				}
				return m_instance;
			}
		}

		internal int DefinitionStartIndex => m_definitionStartIndex;

		internal int DefinitionEndIndex => m_definitionEndIndex;

		internal Microsoft.ReportingServices.ReportRendering.DataMember CurrentRenderDataMember
		{
			get
			{
				if (m_isStatic)
				{
					return m_renderMembers[m_staticIndex];
				}
				return m_group.CurrentShimRenderGroup as Microsoft.ReportingServices.ReportRendering.DataMember;
			}
		}

		internal ShimDataMember(IDefinitionPath parentDefinitionPath, CustomReportItem owner, ShimDataMember parent, int parentCollectionIndex, bool isColumn, bool isStatic, Microsoft.ReportingServices.ReportRendering.DataMemberCollection renderMembers, int staticIndex)
			: base(parentDefinitionPath, owner, parent, parentCollectionIndex)
		{
			m_definitionStartIndex = owner.GetCurrentMemberCellDefinitionIndex();
			m_isColumn = isColumn;
			m_isStatic = isStatic;
			m_renderMembers = renderMembers;
			m_staticIndex = staticIndex;
			DataGroupingCollection children;
			if (isStatic)
			{
				children = renderMembers[staticIndex].Children;
			}
			else
			{
				m_group = new Group(owner, new ShimRenderGroups(renderMembers));
				children = renderMembers[0].Children;
			}
			if (children != null)
			{
				m_children = new ShimDataMemberCollection(this, owner, isColumn, this, children);
			}
			else
			{
				owner.GetAndIncrementMemberCellDefinitionIndex();
			}
			m_definitionEndIndex = owner.GetCurrentMemberCellDefinitionIndex();
		}

		internal bool SetNewContext(int index)
		{
			base.ResetContext();
			if (m_instance != null)
			{
				m_instance.SetNewContext();
			}
			if (m_isStatic)
			{
				return index <= 1;
			}
			if (base.OwnerCri.RenderCri.CustomData.NoRows)
			{
				return false;
			}
			if (index < 0 || index >= m_group.RenderGroups.Count)
			{
				return false;
			}
			m_group.CurrentRenderGroupIndex = index;
			UpdateInnerContext(m_group.RenderGroups[index] as Microsoft.ReportingServices.ReportRendering.DataMember);
			return true;
		}

		internal override void ResetContext()
		{
			ResetContext(null);
		}

		internal void ResetContext(Microsoft.ReportingServices.ReportRendering.DataMemberCollection renderMembers)
		{
			if (renderMembers != null)
			{
				m_renderMembers = renderMembers;
			}
			if (m_group != null)
			{
				m_group.CurrentRenderGroupIndex = -1;
			}
			Microsoft.ReportingServices.ReportRendering.DataMember currentRenderMember = IsStatic ? m_renderMembers[m_staticIndex] : (m_group.CurrentShimRenderGroup as Microsoft.ReportingServices.ReportRendering.DataMember);
			UpdateInnerContext(currentRenderMember);
		}

		private void UpdateInnerContext(Microsoft.ReportingServices.ReportRendering.DataMember currentRenderMember)
		{
			if (m_children != null)
			{
				((ShimDataMemberCollection)m_children).ResetContext(currentRenderMember.Children);
			}
			else
			{
				((ShimDataRowCollection)base.OwnerCri.CustomData.RowCollection).UpdateCells(this);
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
