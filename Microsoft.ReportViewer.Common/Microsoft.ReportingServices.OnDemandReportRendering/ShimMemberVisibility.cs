namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal abstract class ShimMemberVisibility : Visibility
	{
		internal abstract bool GetInstanceHidden();

		internal abstract bool GetInstanceStartHidden();
	}
}
