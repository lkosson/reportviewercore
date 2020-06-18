using Microsoft.ReportingServices.Interfaces;
using System;

namespace Microsoft.ReportingServices.DataProcessing
{
	internal interface IDbPoolableConnection : IDbConnection, IDisposable, IExtension
	{
		bool IsAlive
		{
			get;
		}

		bool IsFromPool
		{
			get;
			set;
		}

		string GetConnectionStringForPooling();
	}
}
