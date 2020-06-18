using Microsoft.ReportingServices.ReportProcessing;

namespace Microsoft.ReportingServices.ReportRendering
{
	internal sealed class TableRowsCollection
	{
		private Table m_owner;

		private TableDetail m_detailDef;

		private TableDetailInstanceList m_detailInstances;

		private TableDetailRowCollection[] m_rows;

		private TableDetailRowCollection m_firstRows;

		public TableDetailRowCollection this[int index]
		{
			get
			{
				if (index < 0 || index >= Count)
				{
					throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidParameterRange, index, 0, Count);
				}
				TableDetailRowCollection tableDetailRowCollection = null;
				if (index == 0)
				{
					tableDetailRowCollection = m_firstRows;
				}
				else if (m_rows != null)
				{
					tableDetailRowCollection = m_rows[index - 1];
				}
				if (tableDetailRowCollection == null)
				{
					TableRowInstance[] rowInstances = null;
					TableDetailInstance tableDetailInstance = null;
					if (m_detailInstances != null && index < m_detailInstances.Count)
					{
						tableDetailInstance = m_detailInstances[index];
						rowInstances = tableDetailInstance.DetailRowInstances;
					}
					else
					{
						Global.Tracer.Assert(m_detailInstances == null || m_detailInstances.Count == 0);
					}
					tableDetailRowCollection = new TableDetailRowCollection(m_owner, m_detailDef.DetailRows, rowInstances, tableDetailInstance);
					if (m_owner.RenderingContext.CacheState)
					{
						if (index == 0)
						{
							m_firstRows = tableDetailRowCollection;
						}
						else
						{
							if (m_rows == null)
							{
								m_rows = new TableDetailRowCollection[m_detailInstances.Count - 1];
							}
							m_rows[index - 1] = tableDetailRowCollection;
						}
					}
				}
				return tableDetailRowCollection;
			}
		}

		public int Count
		{
			get
			{
				if (m_detailInstances == null || m_detailInstances.Count == 0)
				{
					return 1;
				}
				return m_detailInstances.Count;
			}
		}

		internal TableDetail DetailDefinition => m_detailDef;

		internal TableRowsCollection(Table owner, TableDetail detailDef, TableDetailInstanceList detailInstances)
		{
			m_owner = owner;
			m_detailInstances = detailInstances;
			m_detailDef = detailDef;
		}
	}
}
