using Microsoft.ReportingServices.ReportProcessing.ReportObjectModel;

namespace Microsoft.ReportingServices.ReportProcessing.OnDemandReportObjectModel
{
	internal abstract class OnDemandObjectModel : ObjectModel
	{
		public abstract Variables Variables
		{
			get;
		}

		public abstract Lookups Lookups
		{
			get;
		}

		public abstract object MinValue(params object[] arguments);

		public abstract object MaxValue(params object[] arguments);
	}
}
