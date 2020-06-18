using System;
using System.Runtime.Serialization;

namespace Microsoft.ReportingServices.Diagnostics.Utilities
{
	[Serializable]
	internal sealed class DataSetParameterValueNotSetException : ReportCatalogException
	{
		public DataSetParameterValueNotSetException(string parameterName)
			: base(ErrorCode.rsDataSetParameterValueNotSet, ErrorStrings.rsDataSetParameterValueNotSet(parameterName), null, parameterName)
		{
		}

		private DataSetParameterValueNotSetException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
