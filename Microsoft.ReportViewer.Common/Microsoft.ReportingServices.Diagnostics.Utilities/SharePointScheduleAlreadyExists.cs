using System;
using System.Runtime.Serialization;

namespace Microsoft.ReportingServices.Diagnostics.Utilities
{
	[Serializable]
	internal sealed class SharePointScheduleAlreadyExists : ReportCatalogException
	{
		public SharePointScheduleAlreadyExists(string name, string path)
			: base(ErrorCode.rsScheduleAlreadyExists, ErrorStrings.rsSharePoitScheduleAlreadyExists(name, path), null, null)
		{
		}

		private SharePointScheduleAlreadyExists(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
