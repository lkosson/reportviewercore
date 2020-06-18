using System;
using System.Runtime.Serialization;
using System.Xml;

namespace Microsoft.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class RDLUpgradeException : XmlException
	{
		internal RDLUpgradeException(string msg)
			: base(msg)
		{
		}

		internal RDLUpgradeException(string msg, Exception inner)
			: base(msg, inner)
		{
		}

		private RDLUpgradeException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
