namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal abstract class MapObjectCollectionItem : IMapObjectCollectionItem
	{
		protected BaseInstance m_instance;

		void IMapObjectCollectionItem.SetNewContext()
		{
			SetNewContext();
		}

		internal virtual void SetNewContext()
		{
			if (m_instance != null)
			{
				m_instance.SetNewContext();
			}
		}
	}
}
