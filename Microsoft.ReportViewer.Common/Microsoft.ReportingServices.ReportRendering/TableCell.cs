using Microsoft.ReportingServices.ReportProcessing;
using System.Globalization;

namespace Microsoft.ReportingServices.ReportRendering
{
	internal sealed class TableCell
	{
		private Microsoft.ReportingServices.ReportProcessing.Table m_tableDef;

		private int m_index;

		private TableCellCollection m_cells;

		public ReportItem ReportItem => m_cells.ReportItems?[m_index];

		public int ColSpan => m_cells.ColSpans[m_index];

		public string ID
		{
			get
			{
				string[] array = m_cells.RowDef.RenderingModelIDs;
				if (array == null)
				{
					array = new string[m_cells.RowDef.IDs.Count];
					m_cells.RowDef.RenderingModelIDs = array;
				}
				if (array[m_index] == null)
				{
					array[m_index] = m_cells.RowDef.IDs[m_index].ToString(CultureInfo.InvariantCulture);
				}
				return array[m_index];
			}
		}

		public object SharedRenderingInfo
		{
			get
			{
				int num = m_cells.RowDef.IDs[m_index];
				return m_cells.RenderingContext.RenderingInfoManager.SharedRenderingInfo[num];
			}
			set
			{
				int num = m_cells.RowDef.IDs[m_index];
				m_cells.RenderingContext.RenderingInfoManager.SharedRenderingInfo[num] = value;
			}
		}

		internal TableCell(Microsoft.ReportingServices.ReportProcessing.Table tableDef, int index, TableCellCollection cells)
		{
			m_tableDef = tableDef;
			m_index = index;
			m_cells = cells;
		}
	}
}
