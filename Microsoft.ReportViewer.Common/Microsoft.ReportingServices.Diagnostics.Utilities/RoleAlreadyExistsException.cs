using System;
using System.Runtime.Serialization;

namespace Microsoft.ReportingServices.Diagnostics.Utilities
{
	[Serializable]
	internal sealed class RoleAlreadyExistsException : ReportCatalogException
	{
		public RoleAlreadyExistsException(string roleName)
			: base(ErrorCode.rsRoleAlreadyExists, ErrorStrings.rsRoleAlreadyExists(roleName), null, null)
		{
		}

		private RoleAlreadyExistsException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
