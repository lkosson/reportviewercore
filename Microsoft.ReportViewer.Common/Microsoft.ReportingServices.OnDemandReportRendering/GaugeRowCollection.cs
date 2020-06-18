using Microsoft.ReportingServices.ReportIntermediateFormat;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class GaugeRowCollection : IDataRegionRowCollection
	{
		private GaugePanel m_owner;

		private GaugeRow m_gaugeRow;

		private GaugeRowList m_gaugeRowCollectionDefs;

		public GaugeRow GaugeRow
		{
			get
			{
				if (m_gaugeRow == null && m_gaugeRowCollectionDefs.Count == 1)
				{
					m_gaugeRow = new GaugeRow(m_owner, m_gaugeRowCollectionDefs[0]);
				}
				return m_gaugeRow;
			}
		}

		public int Count => 1;

		internal GaugeRowCollection(GaugePanel owner, GaugeRowList gaugeRowCollectionDefs)
		{
			m_owner = owner;
			m_gaugeRowCollectionDefs = gaugeRowCollectionDefs;
		}

		internal GaugeRowCollection(GaugePanel owner)
		{
			m_owner = owner;
		}

		internal void SetNewContext()
		{
			if (m_gaugeRow != null)
			{
				m_gaugeRow.SetNewContext();
			}
		}

		IDataRegionRow IDataRegionRowCollection.GetIfExists(int rowIndex)
		{
			if (rowIndex == 0)
			{
				return GaugeRow;
			}
			return null;
		}
	}
}
