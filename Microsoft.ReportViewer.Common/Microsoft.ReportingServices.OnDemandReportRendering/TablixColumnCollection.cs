using Microsoft.ReportingServices.ReportProcessing;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class TablixColumnCollection : ReportElementCollectionBase<TablixColumn>
	{
		private Tablix m_owner;

		private TablixColumn[] m_columns;

		public override TablixColumn this[int index]
		{
			get
			{
				if (index < 0 || index >= Count)
				{
					throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidParameterRange, index, 0, Count);
				}
				if (m_columns == null)
				{
					m_columns = new TablixColumn[Count];
				}
				if (m_columns[index] == null)
				{
					m_columns[index] = new TablixColumn(m_owner, index);
				}
				return m_columns[index];
			}
		}

		public override int Count
		{
			get
			{
				int result = 0;
				if (m_owner.IsOldSnapshot)
				{
					switch (m_owner.SnapshotTablixType)
					{
					case DataRegion.Type.List:
						result = 1;
						break;
					case DataRegion.Type.Table:
						result = m_owner.RenderTable.Columns.Count;
						break;
					case DataRegion.Type.Matrix:
						result = m_owner.MatrixColDefinitionMapping.Length;
						break;
					}
				}
				else
				{
					result = m_owner.TablixDef.TablixColumns.Count;
				}
				return result;
			}
		}

		internal TablixColumnCollection(Tablix owner)
		{
			m_owner = owner;
		}
	}
}
