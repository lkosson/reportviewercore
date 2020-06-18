using System;
using System.Globalization;
using System.Runtime.Serialization;

namespace Microsoft.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class ReportProcessingException_NonExistingFieldReference : Exception
	{
		internal ReportProcessingException_NonExistingFieldReference(string fieldName)
			: base(string.Format(CultureInfo.CurrentCulture, RPRes.rsNonExistingFieldReferenceByName(fieldName)))
		{
		}

		internal ReportProcessingException_NonExistingFieldReference()
			: base(string.Format(CultureInfo.CurrentCulture, RPRes.rsNonExistingFieldReference))
		{
		}

		private ReportProcessingException_NonExistingFieldReference(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
