namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal abstract class TablixRow : ReportElementCollectionBase<TablixCell>, IDataRegionRow
	{
		protected Tablix m_owner;

		protected int m_rowIndex;

		public abstract ReportSize Height
		{
			get;
		}

		internal TablixRow(Tablix owner, int rowIndex)
		{
			m_owner = owner;
			m_rowIndex = rowIndex;
		}

		IDataRegionCell IDataRegionRow.GetIfExists(int index)
		{
			return GetIfExists(index);
		}

		internal virtual IDataRegionCell GetIfExists(int index)
		{
			if (index < 0 || index >= Count)
			{
				return null;
			}
			return this[index];
		}
	}
}
