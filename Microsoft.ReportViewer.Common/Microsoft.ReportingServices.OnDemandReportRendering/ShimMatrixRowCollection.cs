using Microsoft.ReportingServices.ReportProcessing;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class ShimMatrixRowCollection : TablixRowCollection
	{
		private List<ShimMatrixRow> m_rows;

		public override TablixRow this[int index]
		{
			get
			{
				if (index < 0 || index >= Count)
				{
					throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidParameterRange, index, 0, Count);
				}
				return m_rows[index];
			}
		}

		public override int Count => m_rows.Count;

		internal ShimMatrixRowCollection(Tablix owner)
			: base(owner)
		{
			m_rows = new List<ShimMatrixRow>();
			AppendMatrixRows(null, owner.RowHierarchy.MemberCollection as ShimMatrixMemberCollection, owner.InSubtotal);
		}

		private void AppendMatrixRows(ShimMatrixMember rowParentMember, ShimMatrixMemberCollection rowMembers, bool inSubtotalRow)
		{
			if (rowMembers == null)
			{
				m_rows.Add(new ShimMatrixRow(m_owner, m_rows.Count, rowParentMember, inSubtotalRow));
				return;
			}
			int count = rowMembers.Count;
			for (int i = 0; i < count; i++)
			{
				ShimMatrixMember shimMatrixMember = rowMembers[i] as ShimMatrixMember;
				AppendMatrixRows(shimMatrixMember, shimMatrixMember.Children as ShimMatrixMemberCollection, inSubtotalRow || shimMatrixMember.CurrentRenderMatrixMember.IsTotal);
			}
		}

		internal void UpdateCells(ShimMatrixMember innermostMember)
		{
			if (innermostMember == null || innermostMember.Children != null)
			{
				return;
			}
			if (!innermostMember.IsColumn)
			{
				int memberCellIndex = innermostMember.MemberCellIndex;
				int count = m_rows[memberCellIndex].Count;
				for (int i = 0; i < count; i++)
				{
					((ShimMatrixCell)m_rows[memberCellIndex][i]).ResetCellContents();
				}
			}
			else
			{
				int memberCellIndex2 = innermostMember.MemberCellIndex;
				int count2 = m_rows.Count;
				for (int j = 0; j < count2; j++)
				{
					((ShimMatrixCell)m_rows[j][memberCellIndex2]).ResetCellContents();
				}
			}
		}
	}
}
