using System;
using System.Globalization;
using System.Runtime.Serialization;

namespace Microsoft.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class ReportProcessingException_NonExistingScopeReference : Exception
	{
		internal ReportProcessingException_NonExistingScopeReference(string scopeName)
			: base(string.Format(CultureInfo.CurrentCulture, RPRes.rsNonExistingScopeReference(scopeName)))
		{
		}

		private ReportProcessingException_NonExistingScopeReference(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
