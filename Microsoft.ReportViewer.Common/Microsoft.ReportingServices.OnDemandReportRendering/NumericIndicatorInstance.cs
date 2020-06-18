namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class NumericIndicatorInstance : GaugePanelItemInstance
	{
		private NumericIndicator m_defObject;

		internal NumericIndicatorInstance(NumericIndicator defObject)
			: base(defObject)
		{
			m_defObject = defObject;
		}

		protected override void ResetInstanceCache()
		{
			base.ResetInstanceCache();
		}
	}
}
