using Microsoft.ReportingServices.DataProcessing;
using Microsoft.ReportingServices.Interfaces;
using System;

namespace Microsoft.ReportingServices.DataExtensions
{
	internal interface IDbConnectionTest : IDbConnection, IDisposable, IExtension
	{
		void TestConnection();
	}
}
