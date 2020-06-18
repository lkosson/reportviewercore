namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal abstract class ChartObjectCollectionItem<T> where T : BaseInstance
	{
		protected T m_instance;

		internal virtual void SetNewContext()
		{
			if (m_instance != null)
			{
				m_instance.SetNewContext();
			}
		}
	}
}
