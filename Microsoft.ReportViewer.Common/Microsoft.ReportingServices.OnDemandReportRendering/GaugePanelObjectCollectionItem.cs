namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal abstract class GaugePanelObjectCollectionItem
	{
		protected BaseInstance m_instance;

		internal virtual void SetNewContext()
		{
			if (m_instance != null)
			{
				m_instance.SetNewContext();
			}
		}
	}
}
