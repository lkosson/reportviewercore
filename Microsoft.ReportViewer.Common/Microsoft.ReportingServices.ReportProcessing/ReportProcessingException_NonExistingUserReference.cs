using System;
using System.Globalization;
using System.Runtime.Serialization;

namespace Microsoft.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class ReportProcessingException_NonExistingUserReference : Exception
	{
		internal ReportProcessingException_NonExistingUserReference(string propName)
			: base(string.Format(CultureInfo.CurrentCulture, RPRes.rsNonExistingUserReference(propName)))
		{
		}

		internal ReportProcessingException_NonExistingUserReference(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
