using Microsoft.ReportingServices.ReportProcessing;

namespace Microsoft.ReportingServices.ReportRendering
{
	internal sealed class SizeCollection
	{
		private Matrix m_owner;

		private bool m_widthsCollection;

		private ReportSizeCollection m_reportSizeCollection;

		public ReportSize this[int index]
		{
			get
			{
				if (index < 0 || index >= Count)
				{
					throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidParameterRange, index, 0, Count);
				}
				ReportSize reportSize = null;
				if (m_reportSizeCollection != null && m_reportSizeCollection[index] != null)
				{
					reportSize = m_reportSizeCollection[index];
				}
				if (reportSize == null)
				{
					Microsoft.ReportingServices.ReportProcessing.Matrix matrix = (Microsoft.ReportingServices.ReportProcessing.Matrix)m_owner.ReportItemDef;
					MatrixInstance matrixInstance = (MatrixInstance)m_owner.ReportItemInstance;
					ReportSizeCollection reportSizeCollection = null;
					reportSizeCollection = ((!m_widthsCollection) ? matrix.CellHeightsForRendering : matrix.CellWidthsForRendering);
					Global.Tracer.Assert(reportSizeCollection != null);
					if (m_owner.NoRows || matrixInstance == null || matrixInstance.Cells.Count == 0)
					{
						reportSize = reportSizeCollection[index];
					}
					else if ((m_widthsCollection && matrix.StaticColumns == null) || (!m_widthsCollection && matrix.StaticRows == null))
					{
						reportSize = reportSizeCollection[0];
					}
					else
					{
						bool cacheState = m_owner.RenderingContext.CacheState;
						m_owner.RenderingContext.CacheState = true;
						MatrixCellCollection cellCollection = m_owner.CellCollection;
						MatrixCell matrixCell = null;
						matrixCell = ((!m_widthsCollection) ? cellCollection[index, 0] : cellCollection[0, index]);
						reportSize = reportSizeCollection[matrixCell.ColumnIndex];
						m_owner.RenderingContext.CacheState = cacheState;
					}
					if (m_owner.RenderingContext.CacheState)
					{
						if (m_reportSizeCollection == null)
						{
							m_reportSizeCollection = new ReportSizeCollection(Count);
						}
						m_reportSizeCollection[index] = reportSize;
					}
				}
				return reportSize;
			}
		}

		public int Count
		{
			get
			{
				MatrixInstance matrixInstance = (MatrixInstance)m_owner.ReportItemInstance;
				if (m_owner.NoRows || matrixInstance == null || matrixInstance.Cells.Count == 0)
				{
					Microsoft.ReportingServices.ReportProcessing.Matrix matrix = (Microsoft.ReportingServices.ReportProcessing.Matrix)m_owner.ReportItemDef;
					ReportSizeCollection reportSizeCollection = null;
					reportSizeCollection = ((!m_widthsCollection) ? matrix.CellHeightsForRendering : matrix.CellWidthsForRendering);
					Global.Tracer.Assert(reportSizeCollection != null);
					return reportSizeCollection.Count;
				}
				if (m_widthsCollection)
				{
					return m_owner.CellColumns;
				}
				return m_owner.CellRows;
			}
		}

		internal SizeCollection(Matrix owner, bool widthsCollection)
		{
			m_owner = owner;
			m_widthsCollection = widthsCollection;
		}
	}
}
