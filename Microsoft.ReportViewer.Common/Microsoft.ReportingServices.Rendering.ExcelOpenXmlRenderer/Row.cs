using Microsoft.ReportingServices.Rendering.ExcelOpenXmlRenderer.Model;

namespace Microsoft.ReportingServices.Rendering.ExcelOpenXmlRenderer
{
	internal sealed class Row
	{
		private readonly IRowModel _model;

		public Cell this[int index] => _model.getCell(index).Interface;

		public double Height
		{
			set
			{
				_model.Height = value;
			}
		}

		public bool Hidden
		{
			set
			{
				_model.Hidden = value;
			}
		}

		public bool OutlineCollapsed
		{
			set
			{
				_model.OutlineCollapsed = value;
			}
		}

		public int OutlineLevel
		{
			set
			{
				_model.OutlineLevel = value;
			}
		}

		public int RowNumber => _model.RowNumber;

		public bool CustomHeight
		{
			set
			{
				_model.CustomHeight = value;
			}
		}

		internal Row(IRowModel model)
		{
			_model = model;
		}

		public void ClearCell(int column)
		{
			_model.ClearCell(column);
		}

		public override bool Equals(object obj)
		{
			if (obj == null || !(obj is Row))
			{
				return false;
			}
			if (obj == this)
			{
				return true;
			}
			return ((Row)obj)._model.Equals(_model);
		}

		public override int GetHashCode()
		{
			return _model.GetHashCode();
		}
	}
}
