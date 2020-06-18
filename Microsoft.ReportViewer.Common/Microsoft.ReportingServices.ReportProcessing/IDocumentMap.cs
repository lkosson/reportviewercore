using System;
using System.Collections;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.ReportProcessing
{
	internal interface IDocumentMap : IEnumerator<OnDemandDocumentMapNode>, IDisposable, IEnumerator
	{
		void Close();
	}
}
