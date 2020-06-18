namespace Microsoft.ReportingServices.ReportProcessing.ReportObjectModel
{
	internal abstract class ReportItemImpl : ReportItem
	{
		internal Microsoft.ReportingServices.ReportProcessing.ReportItem m_item;

		internal ReportRuntime m_reportRT;

		internal IErrorContext m_iErrorContext;

		internal ReportProcessing.IScope m_scope;

		internal string Name => m_item.Name;

		internal ReportProcessing.IScope Scope
		{
			set
			{
				m_scope = value;
			}
		}

		internal ReportItemImpl(Microsoft.ReportingServices.ReportProcessing.ReportItem itemDef, ReportRuntime reportRT, IErrorContext iErrorContext)
		{
			Global.Tracer.Assert(itemDef != null, "(null != itemDef)");
			Global.Tracer.Assert(reportRT != null, "(null != reportRT)");
			Global.Tracer.Assert(iErrorContext != null, "(null != iErrorContext)");
			m_item = itemDef;
			m_reportRT = reportRT;
			m_iErrorContext = iErrorContext;
		}
	}
}
