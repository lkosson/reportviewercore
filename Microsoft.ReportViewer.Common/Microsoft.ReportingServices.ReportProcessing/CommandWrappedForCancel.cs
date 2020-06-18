using Microsoft.ReportingServices.DataExtensions;
using Microsoft.ReportingServices.DataProcessing;
using System;

namespace Microsoft.ReportingServices.ReportProcessing
{
	internal sealed class CommandWrappedForCancel : IDbCommand, IDisposable
	{
		private readonly IDbCommand m_command;

		private readonly IProcessingDataExtensionConnection m_dataExtensionConnection;

		private readonly IProcessingDataSource m_dataSourceObj;

		private readonly DataSourceInfo m_dataSourceInfo;

		private readonly string m_datasetName;

		private readonly IDbConnection m_connection;

		public string CommandText
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public int CommandTimeout
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public CommandType CommandType
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public IDataParameterCollection Parameters
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public IDbTransaction Transaction
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		internal CommandWrappedForCancel(IDbCommand command, IProcessingDataExtensionConnection dataExtensionConnection, IProcessingDataSource dataSourceObj, DataSourceInfo dataSourceInfo, string datasetName, IDbConnection connection)
		{
			m_command = command;
			m_dataExtensionConnection = dataExtensionConnection;
			m_dataSourceObj = dataSourceObj;
			m_dataSourceInfo = dataSourceInfo;
			m_datasetName = datasetName;
			m_connection = connection;
		}

		public IDataReader ExecuteReader(CommandBehavior behavior)
		{
			throw new NotImplementedException();
		}

		public IDataParameter CreateParameter()
		{
			throw new NotImplementedException();
		}

		public void Cancel()
		{
			if (m_command is IDbImpersonationNeededForCommandCancel)
			{
				m_dataExtensionConnection.HandleImpersonation(m_dataSourceObj, m_dataSourceInfo, m_datasetName, m_connection, delegate
				{
					m_command.Cancel();
				});
			}
			else
			{
				m_command.Cancel();
			}
		}

		public void Dispose()
		{
			throw new NotImplementedException();
		}
	}
}
