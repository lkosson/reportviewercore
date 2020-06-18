using System;
using System.Globalization;
using System.Runtime.Serialization;

namespace Microsoft.ReportingServices.ReportIntermediateFormat.Persistence
{
	[Serializable]
	internal sealed class IncompatibleFormatVersionException : Exception
	{
		internal IncompatibleFormatVersionException(ObjectType declaredType, long streamPos)
			: base("The Intermediate Format Version is incompatible. The ObjectType read, at stream position " + streamPos.ToString(CultureInfo.InvariantCulture) + ", is " + declaredType.ToString() + ".")
		{
		}

		private IncompatibleFormatVersionException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
