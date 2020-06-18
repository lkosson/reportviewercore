using System;

namespace Microsoft.Reporting.Chart.WebForms
{
	[Flags]
	internal enum LabelsAutoFitStyles
	{
		None = 0x0,
		IncreaseFont = 0x1,
		DecreaseFont = 0x2,
		OffsetLabels = 0x4,
		LabelsAngleStep30 = 0x8,
		LabelsAngleStep45 = 0x10,
		LabelsAngleStep90 = 0x20,
		WordWrap = 0x40
	}
}
