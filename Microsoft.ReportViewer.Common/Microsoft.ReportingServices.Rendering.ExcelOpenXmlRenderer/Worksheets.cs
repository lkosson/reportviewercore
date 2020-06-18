using Microsoft.ReportingServices.Rendering.ExcelOpenXmlRenderer.Model;
using Microsoft.ReportingServices.Rendering.ExcelRenderer.ExcelGenerator;

namespace Microsoft.ReportingServices.Rendering.ExcelOpenXmlRenderer
{
	internal sealed class Worksheets
	{
		private readonly IWorksheetsModel _model;

		public int Count => _model.Count;

		internal Worksheets(IWorksheetsModel model)
		{
			_model = model;
		}

		public Streamsheet CreateStreamsheet(string name, ExcelGeneratorConstants.CreateTempStream createTempStream)
		{
			return _model.CreateStreamsheet(name, createTempStream).Interface;
		}

		public override bool Equals(object obj)
		{
			Worksheets worksheets = obj as Worksheets;
			if (obj == null)
			{
				return false;
			}
			if (obj == this)
			{
				return true;
			}
			return worksheets._model.Equals(_model);
		}

		public override int GetHashCode()
		{
			return _model.GetHashCode();
		}
	}
}
