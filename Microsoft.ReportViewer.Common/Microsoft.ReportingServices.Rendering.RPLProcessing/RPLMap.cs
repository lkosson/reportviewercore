namespace Microsoft.ReportingServices.Rendering.RPLProcessing
{
	internal sealed class RPLMap : RPLItem
	{
		internal RPLMap()
		{
			m_rplElementProps = new RPLMapProps();
			m_rplElementProps.Definition = new RPLItemPropsDef();
		}

		internal RPLMap(long startOffset, RPLContext context)
			: base(startOffset, context)
		{
		}
	}
}
