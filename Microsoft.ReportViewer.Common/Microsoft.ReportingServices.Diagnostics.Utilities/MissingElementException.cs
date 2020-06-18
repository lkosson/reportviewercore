using System;
using System.Runtime.Serialization;

namespace Microsoft.ReportingServices.Diagnostics.Utilities
{
	[Serializable]
	internal sealed class MissingElementException : ReportCatalogException
	{
		private string m_elementName;

		public string MissingElementName => m_elementName;

		public MissingElementException(string elementName)
			: base(ErrorCode.rsMissingElement, ErrorStrings.rsMissingElement(elementName), null, null)
		{
			m_elementName = elementName;
		}

		private MissingElementException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
