using Microsoft.ReportingServices.Rendering.ExcelOpenXmlRenderer.Model;

namespace Microsoft.ReportingServices.Rendering.ExcelOpenXmlRenderer.XMLModel
{
	internal interface ICellController
	{
		object Value
		{
			get;
			set;
		}

		Cell.CellValueType ValueType
		{
			get;
		}

		IStyleModel Style
		{
			get;
			set;
		}

		XMLCharacterRunManager CharManager
		{
			get;
		}

		void Cleanup();
	}
}
