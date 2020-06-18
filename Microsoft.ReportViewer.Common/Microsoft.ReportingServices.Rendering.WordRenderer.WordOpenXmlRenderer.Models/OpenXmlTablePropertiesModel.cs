using System.IO;

namespace Microsoft.ReportingServices.Rendering.WordRenderer.WordOpenXmlRenderer.Models
{
	internal sealed class OpenXmlTablePropertiesModel : IHaveABorderAndShading
	{
		private string _bgColor;

		private OpenXmlBorderPropertiesModel _borderTop;

		private OpenXmlBorderPropertiesModel _borderBottom;

		private OpenXmlBorderPropertiesModel _borderLeft;

		private OpenXmlBorderPropertiesModel _borderRight;

		private bool _autofit;

		public string BackgroundColor
		{
			get
			{
				return _bgColor;
			}
			set
			{
				_bgColor = value;
			}
		}

		public OpenXmlBorderPropertiesModel BorderTop
		{
			get
			{
				if (_borderTop == null)
				{
					_borderTop = new OpenXmlBorderPropertiesModel();
				}
				return _borderTop;
			}
		}

		public OpenXmlBorderPropertiesModel BorderBottom
		{
			get
			{
				if (_borderBottom == null)
				{
					_borderBottom = new OpenXmlBorderPropertiesModel();
				}
				return _borderBottom;
			}
		}

		public OpenXmlBorderPropertiesModel BorderLeft
		{
			get
			{
				if (_borderLeft == null)
				{
					_borderLeft = new OpenXmlBorderPropertiesModel();
				}
				return _borderLeft;
			}
		}

		public OpenXmlBorderPropertiesModel BorderRight
		{
			get
			{
				if (_borderRight == null)
				{
					_borderRight = new OpenXmlBorderPropertiesModel();
				}
				return _borderRight;
			}
		}

		static OpenXmlTablePropertiesModel()
		{
		}

		public OpenXmlTablePropertiesModel(AutoFit autofit)
		{
			_autofit = (autofit == AutoFit.True || autofit == AutoFit.Default);
		}

		public void Write(TextWriter output)
		{
			output.Write("<w:tblPr>");
			if (_borderTop != null || _borderBottom != null || _borderLeft != null || _borderRight != null)
			{
				output.Write("<w:tblBorders>");
				if (_borderTop != null)
				{
					_borderTop.Write(output, "top");
				}
				if (_borderLeft != null)
				{
					_borderLeft.Write(output, "left");
				}
				if (_borderBottom != null)
				{
					_borderBottom.Write(output, "bottom");
				}
				if (_borderRight != null)
				{
					_borderRight.Write(output, "right");
				}
				output.Write("</w:tblBorders>");
			}
			if (_bgColor != null)
			{
				output.Write("<w:shd w:val=\"clear\" w:fill=\"" + _bgColor + "\"/>");
			}
			if (!_autofit)
			{
				output.Write("<w:tblLayout w:type=\"fixed\"/>");
			}
			output.Write("<w:tblCellMar><w:top w:w=\"0\" w:type=\"dxa\"/><w:left w:w=\"0\" w:type=\"dxa\"/><w:bottom w:w=\"0\" w:type=\"dxa\"/><w:right w:w=\"0\" w:type=\"dxa\"/></w:tblCellMar></w:tblPr>");
		}
	}
}
