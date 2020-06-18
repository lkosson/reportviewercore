using System;

namespace Microsoft.ReportingServices.ReportProcessing.Persistence
{
	[AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
	internal sealed class ReferenceAttribute : Attribute
	{
	}
}
