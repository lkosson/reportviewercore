namespace Microsoft.ReportingServices.Rendering.RPLProcessing
{
	internal sealed class RPLHeaderFooter : RPLContainer
	{
		internal RPLHeaderFooter()
		{
			m_rplElementProps = new RPLItemProps();
			m_rplElementProps.Definition = new RPLHeaderFooterPropsDef();
		}

		internal RPLHeaderFooter(long startOffset, RPLContext context, RPLItemMeasurement[] children)
			: base(startOffset, context, children)
		{
		}
	}
}
