using System;
using System.Runtime.Serialization;

namespace Microsoft.ReportingServices.Diagnostics.Utilities
{
	[Serializable]
	internal sealed class InvalidMoveException : ReportCatalogException
	{
		public InvalidMoveException(string itemPath, string targetPath)
			: base(ErrorCode.rsInvalidMove, ErrorStrings.rsInvalidMove(itemPath, targetPath), null, null)
		{
		}

		private InvalidMoveException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
