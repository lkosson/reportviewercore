using System;
using System.Runtime.Serialization;

namespace Microsoft.ReportingServices.Diagnostics.Utilities
{
	[Serializable]
	internal sealed class IdentityClaimsMissingOrInvalidException : ReportCatalogException
	{
		public IdentityClaimsMissingOrInvalidException(string identityClaims)
			: base(ErrorCode.rsIdentityClaimsMissingOrInvalid, ErrorStrings.rsIdentityClaimsMissingOrInvalid((identityClaims == null) ? string.Empty : identityClaims), null, null)
		{
		}

		private IdentityClaimsMissingOrInvalidException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
