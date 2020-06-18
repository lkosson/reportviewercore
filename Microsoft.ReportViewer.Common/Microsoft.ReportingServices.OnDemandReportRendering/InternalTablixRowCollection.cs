using Microsoft.ReportingServices.ReportIntermediateFormat;
using Microsoft.ReportingServices.ReportProcessing;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class InternalTablixRowCollection : TablixRowCollection
	{
		private TablixRowList m_rowDefs;

		private TablixRow[] m_rowROMDefs;

		public override TablixRow this[int index]
		{
			get
			{
				if (index < 0 || index >= Count)
				{
					throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidParameterRange, index, 0, Count);
				}
				if (m_rowROMDefs[index] == null)
				{
					m_rowROMDefs[index] = new InternalTablixRow(m_owner, index, m_rowDefs[index]);
				}
				return m_rowROMDefs[index];
			}
		}

		public override int Count => m_rowDefs.Count;

		internal InternalTablixRowCollection(Tablix owner, TablixRowList rowDefs)
			: base(owner)
		{
			m_rowDefs = rowDefs;
			m_rowROMDefs = new TablixRow[rowDefs.Count];
		}

		internal IDataRegionRow GetIfExists(int index)
		{
			if (m_rowROMDefs != null && index >= 0 && index < Count)
			{
				return m_rowROMDefs[index];
			}
			return null;
		}
	}
}
