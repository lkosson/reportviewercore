using System;

namespace Microsoft.ReportingServices.ReportProcessing
{
	[Flags]
	internal enum LocationFlags
	{
		None = 0x1,
		InDataSet = 0x2,
		InDataRegion = 0x4,
		InGrouping = 0x8,
		InDetail = 0x10,
		InMatrixCell = 0x20,
		InPageSection = 0x40,
		InMatrixSubtotal = 0x80,
		InMatrixCellTopLevelItem = 0x100,
		InMatrixOrTable = 0x200,
		InMatrixGroupHeader = 0x400
	}
}
