using System;
using System.Runtime.Serialization;

namespace Microsoft.ReportingServices.Diagnostics.Utilities
{
	[Serializable]
	internal sealed class InvalidDataSetReferenceException : ReportCatalogException
	{
		public InvalidDataSetReferenceException(string dataSetName)
			: base(ErrorCode.rsInvalidDataSetReference, ErrorStrings.rsInvalidDataSetReference(dataSetName), null, null)
		{
		}

		private InvalidDataSetReferenceException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
