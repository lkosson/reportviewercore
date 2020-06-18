using Microsoft.ReportingServices.ReportIntermediateFormat;
using Microsoft.ReportingServices.ReportRendering;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class ShimDataCell : DataCell
	{
		private Microsoft.ReportingServices.ReportRendering.DataCell m_renderDataCell;

		private ShimDataMember m_rowParentMember;

		private ShimDataMember m_columnParentMember;

		public override DataValueCollection DataValues
		{
			get
			{
				if (m_dataValues == null)
				{
					m_dataValues = new DataValueCollection(m_owner.RenderingContext, CachedRenderDataCell);
				}
				else if (m_renderDataCell == null)
				{
					m_dataValues.UpdateDataCellValues(CachedRenderDataCell);
				}
				return m_dataValues;
			}
		}

		internal override Microsoft.ReportingServices.ReportIntermediateFormat.DataCell DataCellDef => null;

		internal override Microsoft.ReportingServices.ReportRendering.DataCell RenderItem => CachedRenderDataCell;

		private Microsoft.ReportingServices.ReportRendering.DataCell CachedRenderDataCell
		{
			get
			{
				if (m_renderDataCell == null)
				{
					int memberCellIndex = m_rowParentMember.CurrentRenderDataMember.MemberCellIndex;
					int memberCellIndex2 = m_columnParentMember.CurrentRenderDataMember.MemberCellIndex;
					m_renderDataCell = m_owner.RenderCri.CustomData.DataCells[memberCellIndex, memberCellIndex2];
				}
				return m_renderDataCell;
			}
		}

		internal ShimDataCell(CustomReportItem owner, int rowIndex, int colIndex, ShimDataMember rowParentMember, ShimDataMember columnParentMember)
			: base(owner, rowIndex, colIndex)
		{
			m_rowParentMember = rowParentMember;
			m_columnParentMember = columnParentMember;
		}

		internal override void SetNewContext()
		{
			base.SetNewContext();
			m_renderDataCell = null;
		}
	}
}
