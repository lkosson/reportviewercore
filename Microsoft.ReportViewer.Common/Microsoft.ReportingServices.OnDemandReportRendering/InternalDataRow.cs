using Microsoft.ReportingServices.ReportIntermediateFormat;
using Microsoft.ReportingServices.ReportProcessing;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class InternalDataRow : DataRow
	{
		private CustomDataRow m_rowDef;

		public override DataCell this[int index]
		{
			get
			{
				if (index < 0 || index >= Count)
				{
					throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidParameterRange, index, 0, Count);
				}
				if (m_cachedDataCells == null)
				{
					m_cachedDataCells = new DataCell[Count];
				}
				if (m_cachedDataCells[index] == null)
				{
					m_cachedDataCells[index] = new InternalDataCell(m_owner, m_rowIndex, index, m_rowDef.DataCells[index]);
				}
				return m_cachedDataCells[index];
			}
		}

		public override int Count => m_rowDef.Cells.Count;

		internal InternalDataRow(CustomReportItem owner, int rowIndex, CustomDataRow rowDef)
			: base(owner, rowIndex)
		{
			m_rowDef = rowDef;
		}
	}
}
