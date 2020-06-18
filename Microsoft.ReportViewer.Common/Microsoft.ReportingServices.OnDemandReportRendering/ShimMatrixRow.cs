using Microsoft.ReportingServices.ReportProcessing;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class ShimMatrixRow : TablixRow
	{
		private List<ShimMatrixCell> m_cells;

		private ReportSize m_height;

		public override TablixCell this[int index]
		{
			get
			{
				if (index < 0 || index >= Count)
				{
					throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidParameterRange, index, 0, Count);
				}
				return m_cells[index];
			}
		}

		public override int Count => m_cells.Count;

		public override ReportSize Height
		{
			get
			{
				if (m_height == null)
				{
					int index = m_owner.MatrixRowDefinitionMapping[m_rowIndex];
					m_height = new ReportSize(m_owner.RenderMatrix.CellHeights[index]);
				}
				return m_height;
			}
		}

		internal ShimMatrixRow(Tablix owner, int rowIndex, ShimMatrixMember rowParentMember, bool inSubtotalRow)
			: base(owner, rowIndex)
		{
			m_cells = new List<ShimMatrixCell>();
			GenerateMatrixCells(rowParentMember, null, owner.ColumnHierarchy.MemberCollection as ShimMatrixMemberCollection, inSubtotalRow, inSubtotalRow);
		}

		private void GenerateMatrixCells(ShimMatrixMember rowParentMember, ShimMatrixMember colParentMember, ShimMatrixMemberCollection columnMembers, bool inSubtotalRow, bool inSubtotalColumn)
		{
			if (columnMembers == null)
			{
				m_cells.Add(new ShimMatrixCell(m_owner, m_rowIndex, m_cells.Count, rowParentMember, colParentMember, inSubtotalRow || inSubtotalColumn));
				return;
			}
			int count = columnMembers.Count;
			for (int i = 0; i < count; i++)
			{
				ShimMatrixMember shimMatrixMember = columnMembers[i] as ShimMatrixMember;
				GenerateMatrixCells(rowParentMember, shimMatrixMember, shimMatrixMember.Children as ShimMatrixMemberCollection, inSubtotalRow, inSubtotalColumn || shimMatrixMember.CurrentRenderMatrixMember.IsTotal);
			}
		}
	}
}
