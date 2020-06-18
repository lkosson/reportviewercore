using System;

namespace Microsoft.ReportingServices.ReportIntermediateFormat.Persistence
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
	internal sealed class PersistedWithinRequestOnlyAttribute : Attribute
	{
	}
}
