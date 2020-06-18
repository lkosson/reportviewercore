namespace Microsoft.ReportingServices.Rendering.ExcelOpenXmlRenderer.Model
{
	internal interface IColumnModel
	{
		ColumnProperties Interface
		{
			get;
		}

		double Width
		{
			set;
		}

		bool Hidden
		{
			set;
		}

		int OutlineLevel
		{
			set;
		}

		bool OutlineCollapsed
		{
			set;
		}
	}
}
