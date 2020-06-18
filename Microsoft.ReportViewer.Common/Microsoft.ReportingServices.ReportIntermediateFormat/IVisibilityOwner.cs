using Microsoft.ReportingServices.OnDemandReportRendering;
using Microsoft.ReportingServices.ReportIntermediateFormat.Persistence;

namespace Microsoft.ReportingServices.ReportIntermediateFormat
{
	internal interface IVisibilityOwner : IReferenceable
	{
		Visibility Visibility
		{
			get;
		}

		IVisibilityOwner ContainingDynamicVisibility
		{
			get;
			set;
		}

		IVisibilityOwner ContainingDynamicColumnVisibility
		{
			get;
			set;
		}

		IVisibilityOwner ContainingDynamicRowVisibility
		{
			get;
			set;
		}

		string SenderUniqueName
		{
			get;
		}

		bool ComputeHidden(RenderingContext renderingContext, ToggleCascadeDirection direction);

		bool ComputeDeepHidden(RenderingContext renderingContext, ToggleCascadeDirection direction);

		bool ComputeStartHidden(RenderingContext renderingContext);
	}
}
