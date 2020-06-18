using Microsoft.ReportingServices.Diagnostics;
using Microsoft.ReportingServices.ReportProcessing;

namespace Microsoft.ReportingServices.OnDemandProcessing
{
	internal abstract class DataShapeAbortHelperBase : AbortHelper
	{
		public DataShapeAbortHelperBase(IJobContext jobContext, bool enforceSingleAbortException)
			: base(jobContext, enforceSingleAbortException, requireEventHandler: false)
		{
		}

		protected override ProcessingStatus GetStatus(string uniqueName)
		{
			Global.Tracer.Assert(uniqueName == null, "Data shape processing does not support sub-units.");
			return base.Status;
		}

		protected override void SetStatus(ProcessingStatus newStatus, string uniqueName)
		{
			Global.Tracer.Assert(uniqueName == null, "Data shape processing does not support sub-units.");
			base.Status = newStatus;
		}

		internal override void AddSubreportInstanceOrSharedDataSet(string uniqueName)
		{
			Global.Tracer.Assert(condition: false, "Data shape processing does nto support sub-units.");
		}
	}
}
