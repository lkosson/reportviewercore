using System;
using System.Runtime.Serialization;

namespace Microsoft.ReportingServices.Diagnostics.Utilities
{
	[Serializable]
	internal sealed class EmptyRoleException : ReportCatalogException
	{
		public EmptyRoleException()
			: base(ErrorCode.rsEmptyRole, ErrorStrings.rsEmptyRole, null, null)
		{
		}

		private EmptyRoleException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
