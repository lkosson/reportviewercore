namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class TablixCorner
	{
		private Tablix m_owner;

		private TablixCornerRowCollection m_rowCollection;

		public TablixCornerRowCollection RowCollection
		{
			get
			{
				if (m_rowCollection == null)
				{
					m_rowCollection = new TablixCornerRowCollection(m_owner);
				}
				return m_rowCollection;
			}
		}

		internal TablixCorner(Tablix owner)
		{
			m_owner = owner;
		}

		internal void ResetContext()
		{
			if (m_rowCollection != null)
			{
				m_rowCollection.ResetContext();
			}
		}

		internal void SetNewContext()
		{
			if (m_rowCollection != null)
			{
				m_rowCollection.SetNewContext();
			}
		}
	}
}
