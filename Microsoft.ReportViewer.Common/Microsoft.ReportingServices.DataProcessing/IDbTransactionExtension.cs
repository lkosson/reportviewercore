using System;

namespace Microsoft.ReportingServices.DataProcessing
{
	public interface IDbTransactionExtension : IDbTransaction, IDisposable
	{
		bool AllowMultiConnection
		{
			get;
		}
	}
}
