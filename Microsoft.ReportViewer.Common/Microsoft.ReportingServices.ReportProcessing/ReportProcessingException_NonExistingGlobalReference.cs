using System;
using System.Globalization;
using System.Runtime.Serialization;

namespace Microsoft.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class ReportProcessingException_NonExistingGlobalReference : Exception
	{
		internal ReportProcessingException_NonExistingGlobalReference(string globalName)
			: base(string.Format(CultureInfo.CurrentCulture, RPRes.rsNonExistingGlobalReference(globalName)))
		{
		}

		internal ReportProcessingException_NonExistingGlobalReference(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
