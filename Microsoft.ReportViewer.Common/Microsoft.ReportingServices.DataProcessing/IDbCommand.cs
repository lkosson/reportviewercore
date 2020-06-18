using System;

namespace Microsoft.ReportingServices.DataProcessing
{
	public interface IDbCommand : IDisposable
	{
		string CommandText
		{
			get;
			set;
		}

		int CommandTimeout
		{
			get;
			set;
		}

		CommandType CommandType
		{
			get;
			set;
		}

		IDataParameterCollection Parameters
		{
			get;
		}

		IDbTransaction Transaction
		{
			get;
			set;
		}

		IDataReader ExecuteReader(CommandBehavior behavior);

		IDataParameter CreateParameter();

		void Cancel();
	}
}
