using Microsoft.ReportingServices.ReportProcessing;
using System;

namespace Microsoft.ReportingServices.RdlExpressions
{
	internal struct VariantResult
	{
		internal bool ErrorOccurred;

		internal DataFieldStatus FieldStatus;

		internal string ExceptionMessage;

		internal object Value;

		internal TypeCode TypeCode;

		internal VariantResult(bool errorOccurred, object v)
		{
			ErrorOccurred = errorOccurred;
			Value = v;
			FieldStatus = DataFieldStatus.None;
			ExceptionMessage = null;
			TypeCode = TypeCode.Empty;
		}
	}
}
