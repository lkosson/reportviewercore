using Microsoft.ReportingServices.ReportIntermediateFormat;
using System;

namespace Microsoft.ReportingServices.OnDemandProcessing
{
	internal interface IRecordRowReader : IDisposable
	{
		RecordRow RecordRow
		{
			get;
		}

		bool GetNextRow();

		bool MoveToFirstRow();

		void Close();
	}
}
