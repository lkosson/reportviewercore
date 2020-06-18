using System;
using System.Runtime.Serialization;

namespace Microsoft.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class ReportProcessingException_MissingAggregateDependency : Exception
	{
		internal ReportProcessingException_MissingAggregateDependency()
		{
		}

		private ReportProcessingException_MissingAggregateDependency(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
