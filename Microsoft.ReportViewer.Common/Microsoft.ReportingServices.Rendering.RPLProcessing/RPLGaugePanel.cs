namespace Microsoft.ReportingServices.Rendering.RPLProcessing
{
	internal sealed class RPLGaugePanel : RPLItem
	{
		internal RPLGaugePanel()
		{
			m_rplElementProps = new RPLGaugePanelProps();
			m_rplElementProps.Definition = new RPLItemPropsDef();
		}

		internal RPLGaugePanel(long startOffset, RPLContext context)
			: base(startOffset, context)
		{
		}
	}
}
