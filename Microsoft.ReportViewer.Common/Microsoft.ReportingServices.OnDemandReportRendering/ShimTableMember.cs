using Microsoft.ReportingServices.ReportRendering;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class ShimTableMember : ShimTablixMember
	{
		private bool m_isDetailGroup;

		private bool m_isFixedHeader;

		private KeepWithGroup m_keepWithGroup;

		private int m_rowDefinitionStartIndex = -1;

		private int m_rowDefinitionEndIndex = -1;

		private TableColumn m_column;

		private TableRowsCollection m_renderDetails;

		private TableRow m_innerStaticRow;

		public override KeepWithGroup KeepWithGroup => m_keepWithGroup;

		public override bool RepeatOnNewPage => m_keepWithGroup != KeepWithGroup.None;

		public override string DataElementName
		{
			get
			{
				if (m_isDetailGroup)
				{
					return base.OwnerTablix.RenderTable.DetailDataCollectionName;
				}
				return base.DataElementName;
			}
		}

		public override TablixMemberCollection Children
		{
			get
			{
				if (IsColumn)
				{
					return null;
				}
				return m_children;
			}
		}

		public override bool FixedData => m_isFixedHeader;

		public override bool IsStatic
		{
			get
			{
				if (IsColumn)
				{
					return true;
				}
				if (m_isDetailGroup || (m_group != null && m_group.RenderGroups != null))
				{
					return false;
				}
				return true;
			}
		}

		public override bool IsColumn => m_column != null;

		internal override int RowSpan
		{
			get
			{
				if (IsColumn)
				{
					return 0;
				}
				if (IsStatic)
				{
					return 1;
				}
				return m_rowDefinitionEndIndex - m_rowDefinitionStartIndex;
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

		public override int MemberCellIndex => m_rowDefinitionStartIndex;

		public override TablixHeader TablixHeader => null;

		public override bool IsTotal => false;

		public override Visibility Visibility
		{
			get
			{
				if (m_visibility == null)
				{
					if (IsColumn && m_column.ColumnDefinition.Visibility != null)
					{
						m_visibility = new ShimTableMemberVisibility(this, ShimTableMemberVisibility.Mode.StaticColumn);
					}
					else if (!IsColumn && m_group != null)
					{
						if (m_isDetailGroup && m_renderDetails.DetailDefinition.Visibility != null)
						{
							m_visibility = new ShimTableMemberVisibility(this, ShimTableMemberVisibility.Mode.TableDetails);
						}
						else if (!m_isDetailGroup && m_group.CurrentShimRenderGroup.m_visibilityDef != null)
						{
							m_visibility = new ShimTableMemberVisibility(this, ShimTableMemberVisibility.Mode.TableGroup);
						}
					}
					else if (!IsColumn && m_innerStaticRow != null && m_innerStaticRow.m_rowDef.Visibility != null)
					{
						m_visibility = new ShimTableMemberVisibility(this, ShimTableMemberVisibility.Mode.StaticRow);
					}
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

		public override bool HideIfNoRows
		{
			get
			{
				if (IsStatic && m_parent == null)
				{
					return false;
				}
				return true;
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

		internal int RowDefinitionEndIndex => m_rowDefinitionEndIndex;

		internal string DetailInstanceUniqueName
		{
			get
			{
				if (!m_isDetailGroup)
				{
					return null;
				}
				if (m_group.CurrentRenderGroupIndex < 0)
				{
					return null;
				}
				string uniqueName = m_renderDetails[m_group.CurrentRenderGroupIndex][0].UniqueName;
				return m_owner.RenderingContext.GenerateShimUniqueName(uniqueName);
			}
		}

		internal TableRowsCollection RenderTableDetails
		{
			get
			{
				if (!IsColumn)
				{
					return m_renderDetails;
				}
				return null;
			}
		}

		internal TableGroup RenderTableGroup
		{
			get
			{
				if (!IsColumn && !m_isDetailGroup && m_group != null)
				{
					return (TableGroup)m_group.CurrentShimRenderGroup;
				}
				return null;
			}
		}

		internal TableRow RenderTableRow
		{
			get
			{
				if (!IsColumn)
				{
					return m_innerStaticRow;
				}
				return null;
			}
		}

		internal TableColumn RenderTableColumn
		{
			get
			{
				if (IsColumn)
				{
					return m_column;
				}
				return null;
			}
		}

		internal ShimTableMember(IDefinitionPath parentDefinitionPath, Tablix owner, ShimTableMember parent, int parentCollectionIndex, TableRow staticRow, KeepWithGroup keepWithGroup, bool isFixedTableHeader)
			: base(parentDefinitionPath, owner, parent, parentCollectionIndex, isColumn: false)
		{
			m_innerStaticRow = staticRow;
			m_rowDefinitionStartIndex = owner.GetAndIncrementMemberCellDefinitionIndex();
			m_rowDefinitionEndIndex = owner.GetCurrentMemberCellDefinitionIndex();
			m_keepWithGroup = keepWithGroup;
			m_isFixedHeader = isFixedTableHeader;
		}

		internal ShimTableMember(IDefinitionPath parentDefinitionPath, Tablix owner, ShimTableMember parent, int parentCollectionIndex, ShimRenderGroups renderGroups)
			: base(parentDefinitionPath, owner, parent, parentCollectionIndex, isColumn: false)
		{
			m_rowDefinitionStartIndex = owner.GetCurrentMemberCellDefinitionIndex();
			if (renderGroups != null)
			{
				m_children = new ShimTableMemberCollection(this, (Tablix)m_owner, this, (TableGroup)renderGroups[0]);
			}
			m_group = new Group(owner, renderGroups, this);
			m_rowDefinitionEndIndex = owner.GetCurrentMemberCellDefinitionIndex();
		}

		internal ShimTableMember(IDefinitionPath parentDefinitionPath, Tablix owner, ShimTableMember parent, int parentCollectionIndex, TableRowsCollection renderRows)
			: base(parentDefinitionPath, owner, parent, parentCollectionIndex, isColumn: false)
		{
			m_rowDefinitionStartIndex = owner.GetCurrentMemberCellDefinitionIndex();
			m_isDetailGroup = true;
			m_renderDetails = renderRows;
			m_children = new ShimTableMemberCollection(this, (Tablix)m_owner, this, renderRows[0]);
			m_group = new Group(owner, this);
			m_rowDefinitionEndIndex = owner.GetCurrentMemberCellDefinitionIndex();
		}

		internal ShimTableMember(IDefinitionPath parentDefinitionPath, Tablix owner, int columnIndex, TableColumnCollection columns)
			: base(parentDefinitionPath, owner, null, columnIndex, isColumn: true)
		{
			m_column = columns[columnIndex];
			m_isFixedHeader = m_column.FixedHeader;
			m_rowDefinitionStartIndex = (m_rowDefinitionEndIndex = columnIndex);
		}

		internal override bool SetNewContext(int index)
		{
			base.ResetContext();
			if (m_instance != null)
			{
				m_instance.SetNewContext();
			}
			if (m_isDetailGroup)
			{
				if (base.OwnerTablix.RenderTable.NoRows)
				{
					return false;
				}
				if (m_renderDetails == null || index < 0 || index >= m_renderDetails.Count)
				{
					return false;
				}
				bool num = m_group.CurrentRenderGroupIndex == -1 && index == 0;
				m_group.CurrentRenderGroupIndex = index;
				if (!num)
				{
					((ShimTableMemberCollection)m_children).UpdateDetails(m_renderDetails[index]);
				}
				return true;
			}
			if (m_group != null && m_group.RenderGroups != null)
			{
				if (base.OwnerTablix.RenderTable.NoRows)
				{
					return false;
				}
				if (index < 0 || index >= m_group.RenderGroups.Count)
				{
					return false;
				}
				TableGroup tableGroup = m_group.RenderGroups[index] as TableGroup;
				if (tableGroup.InstanceInfo == null)
				{
					return false;
				}
				m_group.CurrentRenderGroupIndex = index;
				((ShimTableMemberCollection)m_children).UpdateHeaderFooter(tableGroup.GroupHeader, tableGroup.GroupFooter);
				((ShimTableMemberCollection)m_children).ResetContext(tableGroup);
				return true;
			}
			return index <= 1;
		}

		internal void UpdateRow(TableRow newTableRow)
		{
			m_innerStaticRow = newTableRow;
			((ShimTableRow)((ShimTableRowCollection)base.OwnerTablix.Body.RowCollection)[m_rowDefinitionStartIndex]).UpdateCells(newTableRow);
		}

		internal override void ResetContext()
		{
			base.ResetContext();
			if (m_group.CurrentRenderGroupIndex >= 0)
			{
				ResetContext(null, null);
			}
		}

		internal void ResetContext(TableGroupCollection newRenderSubGroups, TableRowsCollection newRenderDetails)
		{
			if (m_isDetailGroup)
			{
				m_group.CurrentRenderGroupIndex = -1;
				_ = m_rowDefinitionEndIndex;
				_ = m_rowDefinitionStartIndex;
				if (newRenderDetails != null)
				{
					m_renderDetails = newRenderDetails;
				}
				((ShimTableMemberCollection)m_children).UpdateDetails(m_renderDetails[0]);
			}
			else if (m_group != null && m_group.RenderGroups != null)
			{
				m_group.CurrentRenderGroupIndex = -1;
				if (newRenderSubGroups != null)
				{
					m_group.RenderGroups = new ShimRenderGroups(newRenderSubGroups);
				}
				if (m_children != null)
				{
					TableGroup tableGroup = m_group.CurrentShimRenderGroup as TableGroup;
					((ShimTableMemberCollection)m_children).UpdateHeaderFooter(tableGroup.GroupHeader, tableGroup.GroupFooter);
					((ShimTableMemberCollection)m_children).ResetContext(null);
				}
			}
			SetNewContext(fromMoveNext: true);
		}
	}
}
