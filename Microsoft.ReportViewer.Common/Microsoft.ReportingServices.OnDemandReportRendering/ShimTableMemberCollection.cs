using Microsoft.ReportingServices.ReportProcessing;
using Microsoft.ReportingServices.ReportRendering;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class ShimTableMemberCollection : ShimMemberCollection
	{
		private int m_rowDefinitionStartIndex = -1;

		private int m_rowDefinitionEndIndex = -1;

		private int m_dynamicSubgroupChildIndex = -1;

		public override TablixMember this[int index]
		{
			get
			{
				if (index < 0 || index >= Count)
				{
					throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidParameterRange, index, 0, Count);
				}
				return (TablixMember)m_children[index];
			}
		}

		public override int Count => m_children.Length;

		internal PageBreakLocation PropagatedGroupBreakLocation
		{
			get
			{
				if (m_dynamicSubgroupChildIndex < 0)
				{
					return PageBreakLocation.None;
				}
				return ((TablixMember)m_children[m_dynamicSubgroupChildIndex]).PropagatedGroupBreak;
			}
		}

		internal ShimTableMemberCollection(IDefinitionPath parentDefinitionPath, Tablix owner, bool isColumnGroup)
			: base(parentDefinitionPath, owner, isColumnGroup)
		{
			if (m_isColumnGroup)
			{
				int count = owner.RenderTable.Columns.Count;
				m_children = new ShimTableMember[count];
				for (int i = 0; i < count; i++)
				{
					m_children[i] = new ShimTableMember(this, owner, i, owner.RenderTable.Columns);
				}
			}
			else
			{
				m_rowDefinitionStartIndex = owner.GetCurrentMemberCellDefinitionIndex();
				m_children = CreateInnerHierarchy(owner, null, owner.RenderTable.TableHeader, owner.RenderTable.TableFooter, owner.RenderTable.TableGroups, owner.RenderTable.DetailRows, ref m_dynamicSubgroupChildIndex);
				m_rowDefinitionEndIndex = owner.GetCurrentMemberCellDefinitionIndex();
			}
		}

		internal ShimTableMemberCollection(IDefinitionPath parentDefinitionPath, Tablix owner, ShimTableMember parent, Microsoft.ReportingServices.ReportRendering.TableGroup tableGroup)
			: base(parentDefinitionPath, owner, isColumnGroup: false)
		{
			m_rowDefinitionStartIndex = owner.GetCurrentMemberCellDefinitionIndex();
			m_children = CreateInnerHierarchy(owner, parent, tableGroup.GroupHeader, tableGroup.GroupFooter, tableGroup.SubGroups, tableGroup.DetailRows, ref m_dynamicSubgroupChildIndex);
			m_rowDefinitionEndIndex = owner.GetCurrentMemberCellDefinitionIndex();
		}

		internal ShimTableMemberCollection(IDefinitionPath parentDefinitionPath, Tablix owner, ShimTableMember parent, TableDetailRowCollection detailRows)
			: base(parentDefinitionPath, owner, isColumnGroup: false)
		{
			m_rowDefinitionStartIndex = owner.GetCurrentMemberCellDefinitionIndex();
			int count = detailRows.Count;
			m_children = new ShimTableMember[count];
			for (int i = 0; i < count; i++)
			{
				m_children[i] = new ShimTableMember(this, owner, parent, i, detailRows[i], KeepWithGroup.None, isFixedTableHeader: false);
			}
			m_rowDefinitionEndIndex = owner.GetCurrentMemberCellDefinitionIndex();
		}

		private TablixMember[] CreateInnerHierarchy(Tablix owner, ShimTableMember parent, TableHeaderFooterRows headerRows, TableHeaderFooterRows footerRows, TableGroupCollection subGroups, TableRowsCollection detailRows, ref int dynamicSubgroupChildIndex)
		{
			List<ShimTableMember> list = new List<ShimTableMember>();
			bool noKeepWith = subGroups == null && detailRows == null;
			CreateHeaderFooter(list, headerRows, DetermineKeepWithGroup(isHeader: true, headerRows, noKeepWith), owner, parent, parent == null && owner.RenderTable.FixedHeader);
			if (subGroups != null)
			{
				dynamicSubgroupChildIndex = list.Count;
				CreateInnerDynamicGroups(list, subGroups, owner, parent);
			}
			else if (detailRows != null)
			{
				dynamicSubgroupChildIndex = list.Count;
				list.Add(new ShimTableMember(this, owner, parent, dynamicSubgroupChildIndex, detailRows));
			}
			CreateHeaderFooter(list, footerRows, DetermineKeepWithGroup(isHeader: false, footerRows, noKeepWith), owner, parent, isFixedTableHeader: false);
			return list.ToArray();
		}

		private static KeepWithGroup DetermineKeepWithGroup(bool isHeader, TableHeaderFooterRows rows, bool noKeepWith)
		{
			if (noKeepWith || rows == null || !rows.RepeatOnNewPage)
			{
				return KeepWithGroup.None;
			}
			if (isHeader)
			{
				return KeepWithGroup.After;
			}
			return KeepWithGroup.Before;
		}

		private void CreateHeaderFooter(List<ShimTableMember> rowGroups, TableHeaderFooterRows headerFooterRows, KeepWithGroup keepWithGroup, Tablix owner, ShimTableMember parent, bool isFixedTableHeader)
		{
			if (headerFooterRows != null)
			{
				int count = headerFooterRows.Count;
				int num = rowGroups.Count;
				for (int i = 0; i < count; i++)
				{
					rowGroups.Add(new ShimTableMember(this, owner, parent, num, headerFooterRows[i], keepWithGroup, isFixedTableHeader));
					num++;
				}
			}
		}

		private void CreateInnerDynamicGroups(List<ShimTableMember> rowGroups, TableGroupCollection renderGroupCollection, Tablix owner, ShimTableMember parent)
		{
			if (renderGroupCollection != null)
			{
				ShimTableMember item = new ShimTableMember(this, owner, parent, rowGroups.Count, new ShimRenderGroups(renderGroupCollection));
				rowGroups.Add(item);
			}
		}

		internal void UpdateContext()
		{
			if (m_children != null)
			{
				UpdateHeaderFooter(base.OwnerTablix.RenderTable.TableHeader, base.OwnerTablix.RenderTable.TableFooter);
				if (m_dynamicSubgroupChildIndex >= 0)
				{
					((ShimTableMember)m_children[m_dynamicSubgroupChildIndex]).ResetContext(base.OwnerTablix.RenderTable.TableGroups, base.OwnerTablix.RenderTable.DetailRows);
				}
			}
		}

		internal void UpdateHeaderFooter(TableHeaderFooterRows headerRows, TableHeaderFooterRows footerRows)
		{
			if (m_children != null && (headerRows != null || footerRows != null))
			{
				int num = headerRows?.Count ?? 0;
				int num2 = footerRows?.Count ?? 0;
				int num3 = m_children.Length;
				for (int i = 0; i < num; i++)
				{
					((ShimTableMember)m_children[i]).UpdateRow(headerRows[i]);
				}
				for (int num4 = num2; num4 > 0; num4--)
				{
					((ShimTableMember)m_children[num3 - num4]).UpdateRow(footerRows[num2 - num4]);
				}
			}
		}

		internal void UpdateDetails(TableDetailRowCollection newRenderDetails)
		{
			if (m_children == null || newRenderDetails == null)
			{
				throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidOperation);
			}
			for (int i = 0; i < m_children.Length; i++)
			{
				((ShimTableMember)m_children[i]).UpdateRow(newRenderDetails[i]);
			}
		}

		internal void ResetContext(Microsoft.ReportingServices.ReportRendering.TableGroup newRenderGroup)
		{
			if (m_children != null)
			{
				for (int i = 0; i < m_children.Length; i++)
				{
					((ShimTableMember)m_children[i]).ResetContext(newRenderGroup?.SubGroups, newRenderGroup?.DetailRows);
				}
			}
		}
	}
}
