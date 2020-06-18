using System.Data;

namespace Microsoft.ReportingServices.DataProcessing
{
	public interface IDbConnectionWrapper
	{
		System.Data.IDbConnection Connection
		{
			get;
		}
	}
}
