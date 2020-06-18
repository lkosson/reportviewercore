namespace Microsoft.ReportingServices.Rendering.ExcelOpenXmlRenderer
{
	internal interface IDeepCloneable<T>
	{
		T DeepClone();
	}
}
