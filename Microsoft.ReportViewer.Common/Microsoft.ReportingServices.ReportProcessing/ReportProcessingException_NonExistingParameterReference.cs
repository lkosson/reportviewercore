using System;
using System.Globalization;
using System.Runtime.Serialization;

namespace Microsoft.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class ReportProcessingException_NonExistingParameterReference : Exception
	{
		internal ReportProcessingException_NonExistingParameterReference(string paramName)
			: base(string.Format(CultureInfo.CurrentCulture, RPRes.rsNonExistingParameterReference(paramName)))
		{
		}

		private ReportProcessingException_NonExistingParameterReference(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
