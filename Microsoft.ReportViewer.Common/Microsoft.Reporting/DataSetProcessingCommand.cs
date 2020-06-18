using Microsoft.ReportingServices.DataProcessing;
using System;

namespace Microsoft.Reporting
{
	internal class DataSetProcessingCommand : IDbCommand, IDisposable
	{
		private IDataReader m_dataReader;

		private IDbTransaction m_transaction;

		private DataSetProcessingParameters m_parameters;

		public string CommandText
		{
			get
			{
				return null;
			}
			set
			{
			}
		}

		public int CommandTimeout
		{
			get
			{
				return 0;
			}
			set
			{
			}
		}

		public CommandType CommandType
		{
			get
			{
				return CommandType.Text;
			}
			set
			{
			}
		}

		public IDataParameterCollection Parameters => m_parameters;

		public IDbTransaction Transaction
		{
			get
			{
				return m_transaction;
			}
			set
			{
				m_transaction = value;
			}
		}

		internal DataSetProcessingCommand(IDataReader dataReader)
		{
			m_dataReader = dataReader;
			m_parameters = new DataSetProcessingParameters();
		}

		public IDataReader ExecuteReader(CommandBehavior behavior)
		{
			return m_dataReader;
		}

		public IDataParameter CreateParameter()
		{
			return new DataSetProcessingParameter();
		}

		public void Cancel()
		{
		}

		public void Dispose()
		{
			GC.SuppressFinalize(this);
		}
	}
}
