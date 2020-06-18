using Microsoft.ReportingServices.Rendering.ExcelOpenXmlRenderer.Model;

namespace Microsoft.ReportingServices.Rendering.ExcelOpenXmlRenderer
{
	internal sealed class Workbook
	{
		private readonly IStreambookModel _model;

		public Streamsheet this[int sheetOffset] => _model.getWorksheet(sheetOffset).Interface;

		internal IStreambookModel Model => _model;

		public Palette Palette => _model.Palette.Interface;

		public Worksheets Worksheets => _model.Worksheets.Interface;

		internal Workbook(IStreambookModel model)
		{
			_model = model;
		}

		public GlobalStyle CreateStyle()
		{
			return _model.createGlobalStyle().GlobalInterface;
		}

		public override bool Equals(object obj)
		{
			if (obj == null || !(obj is Workbook))
			{
				return false;
			}
			if (obj == this)
			{
				return true;
			}
			return ((Workbook)obj)._model.Equals(_model);
		}

		public override int GetHashCode()
		{
			return _model.GetHashCode();
		}
	}
}
