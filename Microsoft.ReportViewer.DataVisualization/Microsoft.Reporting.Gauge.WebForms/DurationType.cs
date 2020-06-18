using System;

namespace Microsoft.Reporting.Gauge.WebForms
{
	[Serializable]
	internal enum DurationType
	{
		Infinite,
		Milliseconds,
		Seconds,
		Minutes,
		Hours,
		Days,
		Count
	}
}
