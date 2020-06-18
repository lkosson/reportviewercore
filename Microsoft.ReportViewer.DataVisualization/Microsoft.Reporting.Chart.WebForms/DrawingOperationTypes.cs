using System;

namespace Microsoft.Reporting.Chart.WebForms
{
	[Flags]
	internal enum DrawingOperationTypes
	{
		DrawElement = 0x1,
		CalcElementPath = 0x2
	}
}
