using Microsoft.ReportingServices.ReportIntermediateFormat;
using Microsoft.ReportingServices.ReportProcessing;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class InternalDataRowCollection : DataRowCollection
	{
		private CustomDataRowList m_dataRowDefs;

		public override DataRow this[int index]
		{
			get
			{
				if (index < 0 || index >= Count)
				{
					throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidParameterRange, index, 0, Count);
				}
				if (m_cachedDataRows == null)
				{
					m_cachedDataRows = new DataRow[Count];
				}
				if (m_cachedDataRows[index] == null)
				{
					m_cachedDataRows[index] = new InternalDataRow(m_owner, index, m_dataRowDefs[index]);
				}
				return m_cachedDataRows[index];
			}
		}

		public override int Count => m_dataRowDefs.Count;

		internal InternalDataRowCollection(CustomReportItem owner, CustomDataRowList dataRowDefs)
			: base(owner)
		{
			m_dataRowDefs = dataRowDefs;
		}
	}
}
