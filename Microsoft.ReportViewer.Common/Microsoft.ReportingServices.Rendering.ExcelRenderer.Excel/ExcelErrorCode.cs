namespace Microsoft.ReportingServices.Rendering.ExcelRenderer.Excel
{
	internal enum ExcelErrorCode : byte
	{
		None = byte.MaxValue,
		NullError = 0,
		DivByZeroError = 7,
		ValueError = 0xF,
		RefError = 23,
		NameError = 29,
		NumError = 36,
		NAError = 42
	}
}
