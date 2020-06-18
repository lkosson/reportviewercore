namespace Microsoft.ReportingServices.Rendering.RPLProcessing
{
	internal sealed class RPLChart : RPLItem
	{
		internal RPLChart()
		{
			m_rplElementProps = new RPLChartProps();
			m_rplElementProps.Definition = new RPLItemPropsDef();
		}

		internal RPLChart(long startOffset, RPLContext context)
			: base(startOffset, context)
		{
		}
	}
}
