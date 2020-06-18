using Microsoft.ReportingServices.ReportProcessing;

namespace Microsoft.ReportingServices.ReportRendering
{
	internal sealed class Line : ReportItem
	{
		public bool Slant => ((Microsoft.ReportingServices.ReportProcessing.Line)base.ReportItemDef).LineSlant;

		internal Line(string uniqueName, int intUniqueName, Microsoft.ReportingServices.ReportProcessing.Line reportItemDef, LineInstance reportItemInstance, RenderingContext renderingContext)
			: base(uniqueName, intUniqueName, reportItemDef, reportItemInstance, renderingContext)
		{
		}
	}
}
