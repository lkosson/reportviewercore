using Microsoft.ReportingServices.Diagnostics;
using System;

namespace Microsoft.ReportingServices.OnDemandProcessing
{
	internal class DataShapeAbortHelper : DataShapeAbortHelperBase, IDataShapeAbortHelper, IAbortHelper, IDisposable
	{
		private readonly DataShapeQueryAbortHelper m_parentAbortHelper;

		public DataShapeAbortHelper(DataShapeQueryAbortHelper parentAbortHelper)
			: base(null, enforceSingleAbortException: true)
		{
			m_parentAbortHelper = parentAbortHelper;
		}

		public void ThrowIfAborted(CancelationTrigger cancelationTrigger)
		{
			ThrowIfAborted(cancelationTrigger, null);
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				m_parentAbortHelper.RemoveDataShapeAbortHelper(this);
			}
			base.Dispose(disposing);
		}
	}
}
