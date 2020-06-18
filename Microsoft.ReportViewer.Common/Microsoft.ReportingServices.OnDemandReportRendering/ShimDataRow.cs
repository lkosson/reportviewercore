using Microsoft.ReportingServices.ReportProcessing;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class ShimDataRow : DataRow
	{
		private List<ShimDataCell> m_cells;

		public override DataCell this[int index]
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

		internal ShimDataRow(CustomReportItem owner, int rowIndex, ShimDataMember parentDataMember)
			: base(owner, rowIndex)
		{
			m_cells = new List<ShimDataCell>();
			GenerateDataCells(parentDataMember, null, owner.CustomData.DataColumnHierarchy.MemberCollection as ShimDataMemberCollection);
		}

		private void GenerateDataCells(ShimDataMember rowParentMember, ShimDataMember columnParentMember, ShimDataMemberCollection columnMembers)
		{
			if (columnMembers == null)
			{
				m_cells.Add(new ShimDataCell(m_owner, m_rowIndex, m_cells.Count, rowParentMember, columnParentMember));
				return;
			}
			int count = columnMembers.Count;
			for (int i = 0; i < count; i++)
			{
				ShimDataMember shimDataMember = columnMembers[i] as ShimDataMember;
				GenerateDataCells(rowParentMember, shimDataMember, shimDataMember.Children as ShimDataMemberCollection);
			}
		}
	}
}
