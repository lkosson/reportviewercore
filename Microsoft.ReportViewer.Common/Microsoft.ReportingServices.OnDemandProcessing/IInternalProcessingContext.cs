using Microsoft.ReportingServices.ReportProcessing;
using System;

namespace Microsoft.ReportingServices.OnDemandProcessing
{
	internal interface IInternalProcessingContext
	{
		ErrorContext ErrorContext
		{
			get;
		}

		bool SnapshotProcessing
		{
			get;
			set;
		}

		DateTime ExecutionTime
		{
			get;
		}

		bool EnableDataBackedParameters
		{
			get;
		}
	}
}
