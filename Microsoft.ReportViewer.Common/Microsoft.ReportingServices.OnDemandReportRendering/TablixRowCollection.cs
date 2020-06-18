namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal abstract class TablixRowCollection : ReportElementCollectionBase<TablixRow>, IDataRegionRowCollection
	{
		protected Tablix m_owner;

		internal TablixRowCollection(Tablix owner)
		{
			m_owner = owner;
		}

		IDataRegionRow IDataRegionRowCollection.GetIfExists(int index)
		{
			if (index < 0 || index >= Count)
			{
				return null;
			}
			return this[index];
		}
	}
}
