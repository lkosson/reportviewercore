using System;
using System.Runtime.Serialization;

namespace Microsoft.ReportingServices.Diagnostics.Utilities
{
	[Serializable]
	internal sealed class InvalidElementCombinationException : ReportCatalogException
	{
		public InvalidElementCombinationException(string elementName1, string elementName2)
			: base(ErrorCode.rsInvalidElementCombination, ErrorStrings.rsInvalidElementCombination(elementName1, elementName2), null, null)
		{
		}

		private InvalidElementCombinationException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
