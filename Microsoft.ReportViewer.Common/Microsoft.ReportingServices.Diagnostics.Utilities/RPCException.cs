using System;
using System.Runtime.Serialization;

namespace Microsoft.ReportingServices.Diagnostics.Utilities
{
	[Serializable]
	internal sealed class RPCException : ReportCatalogException
	{
		public RPCException(Exception exceptionFromRPC)
			: base(ErrorCode.rsRPCError, exceptionFromRPC.Message, exceptionFromRPC.InnerException, null)
		{
		}

		private RPCException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
