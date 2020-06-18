using System;
using System.Runtime.Serialization;
using System.Xml;

namespace Microsoft.ReportingServices.Diagnostics.Utilities
{
	[Serializable]
	internal sealed class MalformedXmlException : ReportCatalogException
	{
		public MalformedXmlException(XmlException ex)
			: base(ErrorCode.rsMalformedXml, ErrorStrings.rsMalformedXml(ex.Message), ex, null)
		{
		}

		private MalformedXmlException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
