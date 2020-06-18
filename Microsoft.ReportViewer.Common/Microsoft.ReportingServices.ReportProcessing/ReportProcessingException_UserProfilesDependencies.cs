using System;
using System.Runtime.Serialization;

namespace Microsoft.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class ReportProcessingException_UserProfilesDependencies : Exception
	{
		internal ReportProcessingException_UserProfilesDependencies()
		{
		}

		private ReportProcessingException_UserProfilesDependencies(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
