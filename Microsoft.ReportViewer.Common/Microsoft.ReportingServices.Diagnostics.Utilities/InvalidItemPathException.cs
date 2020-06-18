using System;
using System.Runtime.Serialization;

namespace Microsoft.ReportingServices.Diagnostics.Utilities
{
	[Serializable]
	internal sealed class InvalidItemPathException : ReportCatalogException
	{
		public InvalidItemPathException(string invalidPath, string parameterName)
			: this(invalidPath, parameterName, null)
		{
		}

		public InvalidItemPathException(string invalidPath)
			: this(invalidPath, null, null)
		{
		}

		public InvalidItemPathException(string invalidPath, string parameterName, Exception innerException)
			: base(ErrorCode.rsInvalidItemPath, ErrorStrings.rsInvalidItemPath(invalidPath, CatalogItemNameUtility.MaxItemPathLength), innerException, null, null)
		{
		}

		private InvalidItemPathException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
