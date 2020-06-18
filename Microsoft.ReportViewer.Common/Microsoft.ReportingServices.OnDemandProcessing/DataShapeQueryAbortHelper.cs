using Microsoft.ReportingServices.Diagnostics;
using System;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.OnDemandProcessing
{
	internal class DataShapeQueryAbortHelper : DataShapeAbortHelperBase, IDataShapeQueryAbortHelper, IDataShapeAbortHelper, IAbortHelper, IDisposable
	{
		private readonly List<IDataShapeAbortHelper> m_abortHelpers = new List<IDataShapeAbortHelper>();

		public DataShapeQueryAbortHelper(IJobContext jobContext, bool enforceSingleAbortException)
			: base(jobContext, enforceSingleAbortException)
		{
		}

		public void ThrowIfAborted(CancelationTrigger cancelationTrigger)
		{
			ThrowIfAborted(cancelationTrigger, null);
		}

		public override bool Abort(ProcessingStatus status)
		{
			bool flag = true;
			lock (m_abortHelpers)
			{
				foreach (IDataShapeAbortHelper abortHelper in m_abortHelpers)
				{
					flag &= abortHelper.Abort(status);
				}
				return flag & base.Abort(status);
			}
		}

		public IDataShapeAbortHelper CreateDataShapeAbortHelper()
		{
			DataShapeAbortHelper dataShapeAbortHelper = new DataShapeAbortHelper(this);
			lock (m_abortHelpers)
			{
				m_abortHelpers.Add(dataShapeAbortHelper);
				ProcessingStatus status = GetStatus(null);
				if (status != 0)
				{
					dataShapeAbortHelper.Abort(status);
					return dataShapeAbortHelper;
				}
				return dataShapeAbortHelper;
			}
		}

		public void RemoveDataShapeAbortHelper(IDataShapeAbortHelper helperToBeRemoved)
		{
			lock (m_abortHelpers)
			{
				m_abortHelpers.Remove(helperToBeRemoved);
			}
		}
	}
}
