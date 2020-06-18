using System.IO;

namespace Microsoft.ReportingServices.Rendering.WordRenderer.WordOpenXmlRenderer.Models
{
	internal sealed class OpenXmlTableCellPropertiesModel : IHaveABorderAndShading
	{
		public enum MergeState : byte
		{
			None,
			Continue,
			Start
		}

		public enum VerticalAlign : byte
		{
			Top,
			Bottom,
			Middle
		}

		public enum TextOrientationEnum : byte
		{
			Horizontal,
			Rotate270,
			Rotate90
		}

		private int _width;

		private MergeState _horizontalMerge;

		private MergeState _verticalMerge;

		private VerticalAlign _verticalAlignment;

		private TextOrientationEnum _textOrientation;

		private double _paddingTop;

		private double _paddingBottom;

		private double _paddingLeft;

		private double _paddingRight;

		private OpenXmlBorderPropertiesModel _borderTop;

		private OpenXmlBorderPropertiesModel _borderBottom;

		private OpenXmlBorderPropertiesModel _borderLeft;

		private OpenXmlBorderPropertiesModel _borderRight;

		private OpenXmlBorderPropertiesModel _borderDiagonalUp;

		private OpenXmlBorderPropertiesModel _borderDiagonalDown;

		private string _bgColor;

		public int Width
		{
			set
			{
				_width = value;
			}
		}

		public double PaddingTop
		{
			set
			{
				_paddingTop = ((value < 0.0) ? 0.0 : value);
			}
		}

		public double PaddingBottom
		{
			set
			{
				_paddingBottom = ((value < 0.0) ? 0.0 : value);
			}
		}

		public double PaddingLeft
		{
			set
			{
				_paddingLeft = ((value < 0.0) ? 0.0 : value);
			}
		}

		public double PaddingRight
		{
			set
			{
				_paddingRight = ((value < 0.0) ? 0.0 : value);
			}
		}

		public string BackgroundColor
		{
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

		public OpenXmlBorderPropertiesModel BorderDiagonalUp
		{
			get
			{
				if (_borderDiagonalUp == null)
				{
					_borderDiagonalUp = new OpenXmlBorderPropertiesModel();
				}
				return _borderDiagonalUp;
			}
		}

		public OpenXmlBorderPropertiesModel BorderDiagonalDown
		{
			get
			{
				if (_borderDiagonalDown == null)
				{
					_borderDiagonalDown = new OpenXmlBorderPropertiesModel();
				}
				return _borderDiagonalDown;
			}
		}

		public MergeState HorizontalMerge
		{
			set
			{
				_horizontalMerge = value;
			}
		}

		public MergeState VerticalMerge
		{
			set
			{
				_verticalMerge = value;
			}
		}

		public VerticalAlign VerticalAlignment
		{
			set
			{
				_verticalAlignment = value;
			}
		}

		public TextOrientationEnum TextOrientation
		{
			set
			{
				_textOrientation = value;
			}
		}

		public OpenXmlTableCellPropertiesModel()
		{
			_paddingTop = -1.0;
			_paddingBottom = -1.0;
			_paddingLeft = -1.0;
			_paddingRight = -1.0;
		}

		public void ClearBorderTop()
		{
			_borderTop.Style = OpenXmlBorderPropertiesModel.BorderStyle.None;
		}

		public void ClearBorderBottom()
		{
			_borderBottom.Style = OpenXmlBorderPropertiesModel.BorderStyle.None;
		}

		public void ClearBorderLeft()
		{
			_borderLeft.Style = OpenXmlBorderPropertiesModel.BorderStyle.None;
		}

		public void ClearBorderRight()
		{
			_borderRight.Style = OpenXmlBorderPropertiesModel.BorderStyle.None;
		}

		public void Write(TextWriter output)
		{
			output.Write("<w:tcPr>");
			output.Write("<w:tcW w:w=\"");
			output.Write(WordOpenXmlUtils.TwipsToString(_width, 0, 31680));
			output.Write("\" w:type=\"dxa\"/>");
			if (_horizontalMerge == MergeState.Start)
			{
				output.Write("<w:hMerge w:val=\"restart\"/>");
			}
			else if (_horizontalMerge == MergeState.Continue)
			{
				output.Write("<w:hMerge w:val=\"continue\"/>");
			}
			if (_verticalMerge == MergeState.Start)
			{
				output.Write("<w:vMerge w:val=\"restart\"/>");
			}
			else if (_verticalMerge == MergeState.Continue)
			{
				output.Write("<w:vMerge w:val=\"continue\"/>");
			}
			if (_borderTop != null || _borderBottom != null || _borderLeft != null || _borderRight != null || _borderDiagonalUp != null || _borderDiagonalDown != null)
			{
				output.Write("<w:tcBorders>");
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
				if (_borderDiagonalDown != null)
				{
					_borderDiagonalDown.Write(output, "tl2br");
				}
				if (_borderDiagonalUp != null)
				{
					_borderDiagonalUp.Write(output, "tr2bl");
				}
				output.Write("</w:tcBorders>");
			}
			if (_bgColor != null)
			{
				output.Write("<w:shd w:val=\"clear\" w:fill=\"");
				output.Write(_bgColor);
				output.Write("\"/>");
			}
			WritePadding(output);
			if (_textOrientation == TextOrientationEnum.Rotate90)
			{
				output.Write("<w:textDirection w:val=\"tbRlV\"/>");
			}
			else if (_textOrientation == TextOrientationEnum.Rotate270)
			{
				output.Write("<w:textDirection w:val=\"btLr\"/>");
			}
			if (_verticalAlignment == VerticalAlign.Bottom)
			{
				output.Write("<w:vAlign w:val=\"bottom\"/>");
			}
			else if (_verticalAlignment == VerticalAlign.Middle)
			{
				output.Write("<w:vAlign w:val=\"center\"/>");
			}
			output.Write("</w:tcPr>");
		}

		private void WritePadding(TextWriter output)
		{
			if (!(_paddingTop < 0.0) || !(_paddingBottom < 0.0) || !(_paddingLeft < 0.0) || !(_paddingRight < 0.0))
			{
				output.Write("<w:tcMar>");
				if (_paddingTop >= 0.0)
				{
					output.Write("<w:top w:w=\"");
					output.Write(CellPadding(_paddingTop));
					output.Write("\" w:type=\"dxa\"/>");
				}
				if (_paddingLeft >= 0.0)
				{
					output.Write("<w:left w:w=\"");
					output.Write(CellPadding(_paddingLeft));
					output.Write("\" w:type=\"dxa\"/>");
				}
				if (_paddingBottom >= 0.0)
				{
					output.Write("<w:bottom w:w=\"");
					output.Write(CellPadding(_paddingBottom));
					output.Write("\" w:type=\"dxa\"/>");
				}
				if (_paddingRight >= 0.0)
				{
					output.Write("<w:right w:w=\"");
					output.Write(CellPadding(_paddingRight));
					output.Write("\" w:type=\"dxa\"/>");
				}
				output.Write("</w:tcMar>");
			}
		}

		private string CellPadding(double points)
		{
			return WordOpenXmlUtils.TwipsToString(WordOpenXmlUtils.PointsToTwips(points), 0, 31680);
		}
	}
}
