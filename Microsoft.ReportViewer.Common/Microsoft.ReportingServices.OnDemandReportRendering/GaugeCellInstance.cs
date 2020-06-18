namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class GaugeCellInstance : BaseInstance, IReportScopeInstance
	{
		private GaugeCell m_gaugeCellDef;

		private bool m_isNewContext = true;

		string IReportScopeInstance.UniqueName => m_gaugeCellDef.GaugeCellDef.UniqueName;

		bool IReportScopeInstance.IsNewContext
		{
			get
			{
				return m_isNewContext;
			}
			set
			{
				m_isNewContext = value;
			}
		}

		IReportScope IReportScopeInstance.ReportScope => m_reportScope;

		internal GaugeCellInstance(GaugeCell gaugeCellDef)
			: base(gaugeCellDef)
		{
			m_gaugeCellDef = gaugeCellDef;
		}

		protected override void ResetInstanceCache()
		{
		}

		internal override void SetNewContext()
		{
			if (!m_isNewContext)
			{
				m_isNewContext = true;
				base.SetNewContext();
			}
		}
	}
}
