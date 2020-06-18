using Microsoft.ReportingServices.Interfaces;
using System;

namespace Microsoft.ReportingServices.DataProcessing
{
	public interface IDbConnectionExtension : IDbConnection, IDisposable, IExtension
	{
		string Impersonate
		{
			set;
		}

		string UserName
		{
			set;
		}

		string Password
		{
			set;
		}

		bool IntegratedSecurity
		{
			get;
			set;
		}
	}
}
