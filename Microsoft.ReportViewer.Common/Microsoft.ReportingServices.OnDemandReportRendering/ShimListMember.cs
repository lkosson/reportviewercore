using Microsoft.ReportingServices.ReportRendering;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class ShimListMember : ShimTablixMember
	{
		public override string ID => base.DefinitionPath;

		public override TablixMemberCollection Children => null;

		public override bool IsStatic
		{
			get
			{
				if (m_group != null)
				{
					return m_group.RenderGroups == null;
				}
				return true;
			}
		}

		internal override int RowSpan
		{
			get
			{
				if (IsColumn)
				{
					return 0;
				}
				return 1;
			}
		}

		internal override int ColSpan
		{
			get
			{
				if (IsColumn)
				{
					return 1;
				}
				return 0;
			}
		}

		public override int MemberCellIndex => 0;

		public override bool KeepTogether => true;

		public override bool IsTotal => false;

		public override TablixHeader TablixHeader => null;

		public override Visibility Visibility
		{
			get
			{
				if (m_visibility == null && !IsColumn && base.OwnerTablix.RenderList.ReportItemDef.Visibility != null)
				{
					m_visibility = new ShimListMemberVisibility(this);
				}
				return m_visibility;
			}
		}

		internal override PageBreakLocation PropagatedGroupBreak
		{
			get
			{
				if (IsStatic)
				{
					return PageBreakLocation.None;
				}
				return m_propagatedPageBreak;
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
						TablixDynamicMemberInstance instance = new TablixDynamicMemberInstance(base.OwnerTablix, this, new InternalShimDynamicMemberLogic(this));
						m_owner.RenderingContext.AddDynamicInstance(instance);
						m_instance = instance;
					}
				}
				return m_instance;
			}
		}

		internal ShimListMember(IDefinitionPath parentDefinitionPath, Tablix owner, ShimRenderGroups renderGroups, int parentCollectionIndex, bool isColumn)
			: base(parentDefinitionPath, owner, null, parentCollectionIndex, isColumn)
		{
			m_group = new Group(owner, renderGroups, this);
		}

		internal override bool SetNewContext(int index)
		{
			base.ResetContext();
			if (m_instance != null)
			{
				m_instance.SetNewContext();
			}
			if (m_group != null && m_group.RenderGroups != null)
			{
				if (base.OwnerTablix.RenderList.NoRows)
				{
					return false;
				}
				if (index < 0 || index >= m_group.RenderGroups.Count)
				{
					return false;
				}
				m_group.CurrentRenderGroupIndex = index;
				((ShimListRow)((ShimListRowCollection)base.OwnerTablix.Body.RowCollection)[0]).UpdateCells(m_group.RenderGroups[index] as ListContent);
				return true;
			}
			return index <= 1;
		}

		internal override void ResetContext()
		{
			base.ResetContext();
			if (m_group.CurrentRenderGroupIndex >= 0)
			{
				ResetContext(null);
			}
		}

		internal void ResetContext(ShimRenderGroups renderGroups)
		{
			if (m_group != null)
			{
				m_group.CurrentRenderGroupIndex = -1;
				if (renderGroups != null)
				{
					m_group.RenderGroups = renderGroups;
				}
			}
		}
	}
}
