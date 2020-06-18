using Microsoft.ReportingServices.ReportProcessing;

namespace Microsoft.ReportingServices.ReportRendering
{
	internal abstract class TableRowCollection
	{
		internal Table m_owner;

		internal TableRowList m_rowDefs;

		internal TableRowInstance[] m_rowInstances;

		internal TableRow[] m_rows;

		public virtual TableRow this[int index]
		{
			get
			{
				if (index < 0 || index >= m_rowDefs.Count)
				{
					throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidParameterRange, index, 0, Count);
				}
				TableRow tableRow = null;
				if (m_rows != null)
				{
					tableRow = m_rows[index];
				}
				if (tableRow == null)
				{
					TableRowInstance rowInstance = null;
					if (m_rowInstances != null && index < m_rowInstances.Length)
					{
						rowInstance = m_rowInstances[index];
					}
					else
					{
						Global.Tracer.Assert(m_rowInstances == null);
					}
					tableRow = new TableRow(m_owner, m_rowDefs[index], rowInstance);
					if (m_owner.RenderingContext.CacheState)
					{
						if (m_rows == null)
						{
							m_rows = new TableRow[m_rowDefs.Count];
						}
						m_rows[index] = tableRow;
					}
				}
				return tableRow;
			}
		}

		public int Count => m_rowDefs.Count;

		internal TableRowList DetailRowDefinitions => m_rowDefs;

		internal TableRowCollection(Table owner, TableRowList rowDefs, TableRowInstance[] rowInstances)
		{
			m_owner = owner;
			m_rowInstances = rowInstances;
			m_rowDefs = rowDefs;
		}
	}
}
