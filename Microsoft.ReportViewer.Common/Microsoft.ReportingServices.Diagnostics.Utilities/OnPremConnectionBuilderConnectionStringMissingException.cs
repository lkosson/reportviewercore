using System;
using System.Runtime.Serialization;

namespace Microsoft.ReportingServices.Diagnostics.Utilities
{
	[Serializable]
	internal sealed class OnPremConnectionBuilderConnectionStringMissingException : ReportCatalogException
	{
		public OnPremConnectionBuilderConnectionStringMissingException()
			: base(ErrorCode.rsOnPremConnectionBuilderConnectionStringMissing, ErrorStrings.rsOnPremConnectionBuilderConnectionStringMissing, null, null)
		{
		}

		private OnPremConnectionBuilderConnectionStringMissingException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
