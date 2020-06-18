using System;

namespace Microsoft.ReportingServices.Diagnostics
{
	internal interface IDataShapeAbortHelper : IAbortHelper, IDisposable
	{
		event EventHandler ProcessingAbortEvent;

		void ThrowIfAborted(CancelationTrigger cancelationTrigger);
	}
}
