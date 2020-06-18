using Microsoft.ReportingServices.ReportIntermediateFormat;
using Microsoft.ReportingServices.ReportProcessing;
using Microsoft.ReportingServices.ReportRendering;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class TablixCornerRow : ReportElementCollectionBase<TablixCornerCell>
	{
		private Tablix m_owner;

		private int m_rowIndex;

		private List<Microsoft.ReportingServices.ReportIntermediateFormat.TablixCornerCell> m_rowDef;

		private Microsoft.ReportingServices.ReportRendering.ReportItem m_cornerDef;

		private TablixCornerCell[] m_cellROMDefs;

		public override TablixCornerCell this[int index]
		{
			get
			{
				if (index < 0 || index >= Count)
				{
					throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidParameterRange, index, 0, Count);
				}
				if (m_cellROMDefs[index] == null)
				{
					if (m_owner.IsOldSnapshot)
					{
						if (m_rowIndex == 0 && index == 0)
						{
							m_cellROMDefs[index] = new TablixCornerCell(m_owner, m_rowIndex, index, m_cornerDef);
						}
					}
					else
					{
						Microsoft.ReportingServices.ReportIntermediateFormat.TablixCornerCell tablixCornerCell = m_rowDef[index];
						if (tablixCornerCell.RowSpan > 0 && tablixCornerCell.ColSpan > 0)
						{
							m_cellROMDefs[index] = new TablixCornerCell(m_owner, m_rowIndex, index, tablixCornerCell);
						}
					}
				}
				return m_cellROMDefs[index];
			}
		}

		public override int Count
		{
			get
			{
				if (m_owner.IsOldSnapshot)
				{
					if (DataRegion.Type.Matrix == m_owner.SnapshotTablixType && m_owner.RenderMatrix.Corner != null)
					{
						return m_owner.Rows;
					}
					return 0;
				}
				return m_rowDef.Count;
			}
		}

		internal TablixCornerRow(Tablix owner, int rowIndex, List<Microsoft.ReportingServices.ReportIntermediateFormat.TablixCornerCell> rowDef)
		{
			m_owner = owner;
			m_rowIndex = rowIndex;
			m_rowDef = rowDef;
			m_cellROMDefs = new TablixCornerCell[rowDef.Count];
		}

		internal TablixCornerRow(Tablix owner, int rowIndex, Microsoft.ReportingServices.ReportRendering.ReportItem cornerDef)
		{
			m_owner = owner;
			m_rowIndex = rowIndex;
			m_cornerDef = cornerDef;
			m_cellROMDefs = new TablixCornerCell[m_owner.Rows];
		}

		internal void SetNewContext()
		{
			if (!m_owner.IsOldSnapshot)
			{
				for (int i = 0; i < m_cellROMDefs.Length; i++)
				{
					m_cellROMDefs[i]?.CellContents.SetNewContext();
				}
			}
		}

		internal void UpdateRenderReportItem(Microsoft.ReportingServices.ReportRendering.ReportItem cornerDef)
		{
			m_cornerDef = cornerDef;
		}
	}
}
