namespace Microsoft.ReportingServices.Rendering.ExcelOpenXmlRenderer.Model
{
	internal interface IShapeModel
	{
		string Hyperlink
		{
			set;
		}

		void UpdateColumnOffset(double sizeInPoints, bool start);

		void UpdateRowOffset(double sizeInPoints, bool start);
	}
}
