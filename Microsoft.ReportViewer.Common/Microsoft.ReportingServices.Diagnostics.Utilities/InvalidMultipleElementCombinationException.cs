using System;
using System.Runtime.Serialization;

namespace Microsoft.ReportingServices.Diagnostics.Utilities
{
	[Serializable]
	internal sealed class InvalidMultipleElementCombinationException : ReportCatalogException
	{
		public InvalidMultipleElementCombinationException(string elementName1, string elementName2, string elementName3)
			: base(ErrorCode.rsInvalidMultipleElementCombination, ErrorStrings.rsInvalidMultipleElementCombination(elementName1, elementName2, elementName3), null, null)
		{
		}

		private InvalidMultipleElementCombinationException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
