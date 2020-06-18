using System;
using System.Runtime.Serialization;

namespace Microsoft.ReportingServices.Diagnostics.Utilities
{
	[Serializable]
	internal sealed class UnrecognizedXmlElementException : ReportCatalogException
	{
		public UnrecognizedXmlElementException(string elementName)
			: base(ErrorCode.rsUnrecognizedXmlElement, ErrorStrings.rsUnrecognizedXmlElement(elementName), null, null)
		{
		}

		private UnrecognizedXmlElementException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
