using Microsoft.ReportingServices.ReportProcessing;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class ShimDataRowCollection : DataRowCollection
	{
		private List<ShimDataRow> m_dataRows;

		public override DataRow this[int index]
		{
			get
			{
				if (index < 0 || index >= Count)
				{
					throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidParameterRange, index, 0, Count);
				}
				return m_dataRows[index];
			}
		}

		public override int Count => m_dataRows.Count;

		internal ShimDataRowCollection(CustomReportItem owner)
			: base(owner)
		{
			m_dataRows = new List<ShimDataRow>();
			AppendDataRows(null, owner.CustomData.DataRowHierarchy.MemberCollection as ShimDataMemberCollection);
		}

		private void AppendDataRows(ShimDataMember rowParentMember, ShimDataMemberCollection rowMembers)
		{
			if (rowMembers == null)
			{
				m_dataRows.Add(new ShimDataRow(m_owner, m_dataRows.Count, rowParentMember));
				return;
			}
			int count = rowMembers.Count;
			for (int i = 0; i < count; i++)
			{
				ShimDataMember shimDataMember = rowMembers[i] as ShimDataMember;
				AppendDataRows(shimDataMember, shimDataMember.Children as ShimDataMemberCollection);
			}
		}

		internal void UpdateCells(ShimDataMember innermostMember)
		{
			if (innermostMember == null || innermostMember.Children != null)
			{
				return;
			}
			if (!innermostMember.IsColumn)
			{
				int memberCellIndex = innermostMember.MemberCellIndex;
				int count = m_dataRows[memberCellIndex].Count;
				for (int i = 0; i < count; i++)
				{
					((ShimDataCell)m_dataRows[memberCellIndex][i]).SetNewContext();
				}
			}
			else
			{
				int memberCellIndex2 = innermostMember.MemberCellIndex;
				int count2 = m_dataRows.Count;
				for (int j = 0; j < count2; j++)
				{
					((ShimDataCell)m_dataRows[j][memberCellIndex2]).SetNewContext();
				}
			}
		}
	}
}
