namespace Microsoft.ReportingServices.Rendering.RPLProcessing
{
	internal sealed class RPLBody : RPLContainer
	{
		internal RPLBody()
		{
			m_rplElementProps = new RPLItemProps();
			m_rplElementProps.Definition = new RPLItemPropsDef();
		}

		internal RPLBody(RPLItemProps props)
			: base(props)
		{
		}

		internal RPLBody(long startOffset, RPLContext context, RPLItemMeasurement[] children)
			: base(startOffset, context, children)
		{
		}
	}
}
