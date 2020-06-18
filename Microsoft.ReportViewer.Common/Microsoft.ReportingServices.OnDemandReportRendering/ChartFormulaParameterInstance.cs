namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class ChartFormulaParameterInstance : BaseInstance
	{
		private ChartFormulaParameter m_chartFormulaParameterDef;

		private object m_value;

		public object Value
		{
			get
			{
				if (m_value == null && !m_chartFormulaParameterDef.ChartDef.IsOldSnapshot)
				{
					m_value = m_chartFormulaParameterDef.ChartFormulaParameterDef.EvaluateValue(ReportScopeInstance, m_chartFormulaParameterDef.ChartDef.RenderingContext.OdpContext).Value;
				}
				return m_value;
			}
		}

		internal ChartFormulaParameterInstance(ChartFormulaParameter chartFormulaParameterDef)
			: base(chartFormulaParameterDef.ReportScope)
		{
			m_chartFormulaParameterDef = chartFormulaParameterDef;
		}

		protected override void ResetInstanceCache()
		{
			m_value = null;
		}
	}
}
