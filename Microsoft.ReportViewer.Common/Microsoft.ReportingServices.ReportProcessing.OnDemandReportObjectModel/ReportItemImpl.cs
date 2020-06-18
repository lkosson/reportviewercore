using Microsoft.ReportingServices.OnDemandProcessing.TablixProcessing;
using Microsoft.ReportingServices.RdlExpressions;
using Microsoft.ReportingServices.ReportIntermediateFormat;
using Microsoft.ReportingServices.ReportProcessing.ReportObjectModel;

namespace Microsoft.ReportingServices.ReportProcessing.OnDemandReportObjectModel
{
	internal abstract class ReportItemImpl : Microsoft.ReportingServices.ReportProcessing.ReportObjectModel.ReportItem
	{
		internal Microsoft.ReportingServices.ReportIntermediateFormat.ReportItem m_item;

		internal Microsoft.ReportingServices.RdlExpressions.ReportRuntime m_reportRT;

		internal IErrorContext m_iErrorContext;

		internal IScope m_scope;

		internal string Name => m_item.Name;

		internal IScope Scope
		{
			set
			{
				m_scope = value;
			}
		}

		internal ReportItemImpl(Microsoft.ReportingServices.ReportIntermediateFormat.ReportItem itemDef, Microsoft.ReportingServices.RdlExpressions.ReportRuntime reportRT, IErrorContext iErrorContext)
		{
			Global.Tracer.Assert(itemDef != null, "(null != itemDef)");
			Global.Tracer.Assert(reportRT != null, "(null != reportRT)");
			Global.Tracer.Assert(iErrorContext != null, "(null != iErrorContext)");
			m_item = itemDef;
			m_reportRT = reportRT;
			m_iErrorContext = iErrorContext;
		}

		internal abstract void Reset();

		internal abstract void Reset(Microsoft.ReportingServices.RdlExpressions.VariantResult aResult);
	}
}
