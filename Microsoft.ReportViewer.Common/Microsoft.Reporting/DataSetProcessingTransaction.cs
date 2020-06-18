using Microsoft.ReportingServices.DataProcessing;
using System;

namespace Microsoft.Reporting
{
	internal class DataSetProcessingTransaction : IDbTransaction, IDisposable
	{
		public void Commit()
		{
		}

		public void Rollback()
		{
		}

		public void Dispose()
		{
			GC.SuppressFinalize(this);
		}
	}
}
