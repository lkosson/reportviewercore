namespace Microsoft.ReportingServices.Rendering.RPLProcessing
{
	internal class RPLSubReport : RPLContainer
	{
		internal RPLSubReport()
		{
			m_rplElementProps = new RPLSubReportProps();
			m_rplElementProps.Definition = new RPLSubReportPropsDef();
		}

		internal RPLSubReport(long startOffset, RPLContext context, RPLItemMeasurement[] children)
			: base(startOffset, context, children)
		{
		}

		internal RPLSubReport(RPLItemProps rplElementProps)
			: base(rplElementProps)
		{
		}
	}
}
