using System;

namespace Microsoft.ReportingServices.DataProcessing
{
	public interface IDbTransaction : IDisposable
	{
		void Commit();

		void Rollback();
	}
}
