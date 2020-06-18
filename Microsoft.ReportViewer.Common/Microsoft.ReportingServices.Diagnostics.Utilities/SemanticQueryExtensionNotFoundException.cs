using System;
using System.Runtime.Serialization;

namespace Microsoft.ReportingServices.Diagnostics.Utilities
{
	[Serializable]
	internal sealed class SemanticQueryExtensionNotFoundException : ReportCatalogException
	{
		public SemanticQueryExtensionNotFoundException(string extension)
			: base(ErrorCode.rsSemanticQueryExtensionNotFound, ErrorStrings.rsSemanticQueryExtensionNotFound(extension), null, null)
		{
		}

		private SemanticQueryExtensionNotFoundException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
