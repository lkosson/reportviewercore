using Microsoft.ReportingServices.ReportProcessing;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class TablixCornerRowCollection : ReportElementCollectionBase<TablixCornerRow>
	{
		private Tablix m_owner;

		private TablixCornerRow[] m_cornerRows;

		public override TablixCornerRow this[int index]
		{
			get
			{
				if (index < 0 || index >= Count)
				{
					throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidParameterRange, index, 0, Count);
				}
				if (m_cornerRows == null)
				{
					m_cornerRows = new TablixCornerRow[Count];
				}
				TablixCornerRow tablixCornerRow = m_cornerRows[index];
				if (tablixCornerRow == null)
				{
					tablixCornerRow = (m_owner.IsOldSnapshot ? (m_cornerRows[index] = new TablixCornerRow(m_owner, index, m_owner.RenderMatrix.Corner)) : (m_cornerRows[index] = new TablixCornerRow(m_owner, index, m_owner.TablixDef.Corner[index])));
				}
				return tablixCornerRow;
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
						return m_owner.Columns;
					}
					return 0;
				}
				if (m_owner.TablixDef.Corner != null)
				{
					return m_owner.TablixDef.Corner.Count;
				}
				return 0;
			}
		}

		internal TablixCornerRowCollection(Tablix owner)
		{
			m_owner = owner;
		}

		internal void ResetContext()
		{
			if (m_owner.IsOldSnapshot && 0 < Count && m_cornerRows != null && m_cornerRows[0] != null)
			{
				m_cornerRows[0].UpdateRenderReportItem(m_owner.RenderMatrix.Corner);
			}
		}

		internal void SetNewContext()
		{
			if (!m_owner.IsOldSnapshot && m_cornerRows != null)
			{
				for (int i = 0; i < m_cornerRows.Length; i++)
				{
					m_cornerRows[i]?.SetNewContext();
				}
			}
		}
	}
}
