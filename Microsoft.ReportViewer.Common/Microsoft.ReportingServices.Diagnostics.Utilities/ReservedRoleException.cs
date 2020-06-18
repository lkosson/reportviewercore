using System;
using System.Runtime.Serialization;

namespace Microsoft.ReportingServices.Diagnostics.Utilities
{
	[Serializable]
	internal sealed class ReservedRoleException : ReportCatalogException
	{
		public ReservedRoleException(string roleName)
			: base(ErrorCode.rsReservedRole, ErrorStrings.rsReservedRole(roleName), null, null)
		{
		}

		private ReservedRoleException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
