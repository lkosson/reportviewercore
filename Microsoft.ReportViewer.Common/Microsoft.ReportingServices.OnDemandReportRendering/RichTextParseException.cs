using System;
using System.Runtime.Serialization;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	[Serializable]
	internal class RichTextParseException : Exception
	{
		internal RichTextParseException(string message)
			: base(message)
		{
		}

		protected RichTextParseException(SerializationInfo serializationInfo, StreamingContext context)
			: base(serializationInfo, context)
		{
		}
	}
}
