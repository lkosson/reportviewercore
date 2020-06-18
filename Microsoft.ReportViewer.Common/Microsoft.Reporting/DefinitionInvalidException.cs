using Microsoft.ReportingServices.Diagnostics.Utilities;
using System;
using System.Runtime.Serialization;

namespace Microsoft.Reporting
{
	[Serializable]
	internal sealed class DefinitionInvalidException : RSException
	{
		public DefinitionInvalidException(string reportPath, Exception inner)
			: base(ErrorCode.pvInvalidDefinition, ProcessingStrings.pvInvalidDefinition(reportPath), inner, null, null)
		{
		}

		private DefinitionInvalidException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
