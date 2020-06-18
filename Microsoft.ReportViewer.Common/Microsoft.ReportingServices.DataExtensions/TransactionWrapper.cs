using Microsoft.ReportingServices.DataProcessing;
using Microsoft.ReportingServices.Diagnostics.Utilities;
using System;
using System.Data;
using System.Diagnostics;

namespace Microsoft.ReportingServices.DataExtensions
{
	internal class TransactionWrapper : BaseDataWrapper, Microsoft.ReportingServices.DataProcessing.IDbTransaction, IDisposable
	{
		protected internal System.Data.IDbTransaction UnderlyingTransaction => (System.Data.IDbTransaction)base.UnderlyingObject;

		protected internal TransactionWrapper(System.Data.IDbTransaction underlyingTransaction)
			: base(underlyingTransaction)
		{
		}

		public virtual void Commit()
		{
			UnderlyingTransaction.Commit();
		}

		public virtual void Rollback()
		{
			if (UnderlyingTransaction.Connection != null && UnderlyingTransaction.Connection.State != 0 && UnderlyingTransaction.Connection.State != ConnectionState.Broken)
			{
				UnderlyingTransaction.Rollback();
			}
			else if (RSTrace.DataExtensionTracer.TraceWarning)
			{
				RSTrace.DataExtensionTracer.Trace(TraceLevel.Warning, "TransactionWrapper.Rollback not called, connection is not valid");
			}
		}
	}
}
