using System;

namespace Microsoft.ReportingServices.Diagnostics
{
	internal interface IDataShapeQueryAbortHelper : IDataShapeAbortHelper, IAbortHelper, IDisposable
	{
		IDataShapeAbortHelper CreateDataShapeAbortHelper();
	}
}
