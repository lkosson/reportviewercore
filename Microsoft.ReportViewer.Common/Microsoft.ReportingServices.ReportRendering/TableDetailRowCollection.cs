using Microsoft.ReportingServices.ReportProcessing;

namespace Microsoft.ReportingServices.ReportRendering
{
	internal sealed class TableDetailRowCollection : TableRowCollection
	{
		private TableDetailInstance m_detailInstance;

		private TableDetailInstanceInfo m_detailInstanceInfo;

		public override TableRow this[int index]
		{
			get
			{
				if (index < 0 || index >= m_rowDefs.Count)
				{
					throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidParameterRange, index, 0, base.Count);
				}
				TableDetailRow tableDetailRow = null;
				if (m_rows != null)
				{
					tableDetailRow = (TableDetailRow)m_rows[index];
				}
				if (tableDetailRow == null)
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
					tableDetailRow = new TableDetailRow(m_owner, m_rowDefs[index], rowInstance, this);
					if (m_owner.RenderingContext.CacheState)
					{
						if (m_rows == null)
						{
							m_rows = new TableDetailRow[m_rowDefs.Count];
						}
						m_rows[index] = tableDetailRow;
					}
				}
				return tableDetailRow;
			}
		}

		internal TableDetailInstance DetailInstance => m_detailInstance;

		internal TableDetailInstanceInfo InstanceInfo
		{
			get
			{
				if (m_detailInstance == null)
				{
					return null;
				}
				if (m_detailInstanceInfo == null)
				{
					m_detailInstanceInfo = m_detailInstance.GetInstanceInfo(m_owner.RenderingContext.ChunkManager);
				}
				return m_detailInstanceInfo;
			}
		}

		internal bool Hidden
		{
			get
			{
				bool result = false;
				TableDetail tableDetail = ((Microsoft.ReportingServices.ReportProcessing.Table)m_owner.ReportItemDef).TableDetail;
				if (DetailInstance == null)
				{
					result = RenderingContext.GetDefinitionHidden(tableDetail.Visibility);
				}
				else if (tableDetail.Visibility != null)
				{
					result = ((tableDetail.Visibility.Toggle == null) ? InstanceInfo.StartHidden : m_owner.RenderingContext.IsItemHidden(DetailInstance.UniqueName, potentialSender: false));
				}
				return result;
			}
		}

		internal TableDetailRowCollection(Table owner, TableRowList rowDefs, TableRowInstance[] rowInstances, TableDetailInstance detailInstance)
			: base(owner, rowDefs, rowInstances)
		{
			m_detailInstance = detailInstance;
		}
	}
}
