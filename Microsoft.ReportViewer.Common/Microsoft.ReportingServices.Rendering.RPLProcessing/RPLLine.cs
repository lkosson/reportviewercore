namespace Microsoft.ReportingServices.Rendering.RPLProcessing
{
	internal sealed class RPLLine : RPLItem
	{
		internal RPLLine()
		{
			m_rplElementProps = new RPLLineProps();
			m_rplElementProps.Definition = new RPLLinePropsDef();
		}

		internal RPLLine(long startOffset, RPLContext context)
			: base(startOffset, context)
		{
		}
	}
}
