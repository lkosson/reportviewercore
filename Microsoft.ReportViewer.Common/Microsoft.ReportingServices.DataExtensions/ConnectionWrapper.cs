using Microsoft.ReportingServices.DataProcessing;
using Microsoft.ReportingServices.Diagnostics.Utilities;
using Microsoft.ReportingServices.Interfaces;
using System;
using System.Data;

namespace Microsoft.ReportingServices.DataExtensions
{
	internal class ConnectionWrapper : BaseDataWrapper, Microsoft.ReportingServices.DataProcessing.IDbConnection, IDisposable, IExtension
	{
		protected bool m_wrappedManagedProvider;

		public virtual string ConnectionString
		{
			get
			{
				return UnderlyingConnection.ConnectionString;
			}
			set
			{
				UnderlyingConnection.ConnectionString = value;
			}
		}

		public virtual int ConnectionTimeout => UnderlyingConnection.ConnectionTimeout;

		public System.Data.IDbConnection UnderlyingConnection
		{
			get
			{
				RSTrace.DataExtensionTracer.Assert(base.UnderlyingObject != null, "If the underlying connection is not provided in the constructor it must be set before accessing it.");
				return (System.Data.IDbConnection)base.UnderlyingObject;
			}
		}

		public virtual string LocalizedName => null;

		public bool WrappedManagedProvider
		{
			get
			{
				return m_wrappedManagedProvider;
			}
			internal set
			{
				m_wrappedManagedProvider = value;
			}
		}

		public ConnectionWrapper(System.Data.IDbConnection underlyingConnection)
			: base(underlyingConnection)
		{
		}

		public virtual void Open()
		{
			UnderlyingConnection.Open();
		}

		public virtual void Close()
		{
			UnderlyingConnection.Close();
		}

		public virtual Microsoft.ReportingServices.DataProcessing.IDbCommand CreateCommand()
		{
			return new CommandWrapper(UnderlyingConnection.CreateCommand());
		}

		public virtual Microsoft.ReportingServices.DataProcessing.IDbTransaction BeginTransaction()
		{
			return new TransactionWrapper(UnderlyingConnection.BeginTransaction());
		}

		public virtual void SetConfiguration(string configInfo)
		{
		}
	}
}
