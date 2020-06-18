namespace Microsoft.ReportingServices.Rendering.RPLProcessing
{
	internal sealed class RPLRectangle : RPLContainer
	{
		internal RPLRectangle()
		{
			m_rplElementProps = new RPLItemProps();
			m_rplElementProps.Definition = new RPLRectanglePropsDef();
		}

		internal RPLRectangle(long startOffset, RPLContext context, RPLItemMeasurement[] children)
			: base(startOffset, context, children)
		{
		}

		internal RPLRectangle(RPLItemProps rplElementProps)
			: base(rplElementProps)
		{
		}
	}
}
