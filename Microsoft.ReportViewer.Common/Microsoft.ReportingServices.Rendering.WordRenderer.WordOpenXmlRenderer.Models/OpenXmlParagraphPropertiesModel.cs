using System.IO;

namespace Microsoft.ReportingServices.Rendering.WordRenderer.WordOpenXmlRenderer.Models
{
	internal sealed class OpenXmlParagraphPropertiesModel
	{
		internal enum HorizontalAlignment
		{
			Center,
			Left,
			Right
		}

		private int? _listLevel;

		private int? _listStyleId;

		private HorizontalAlignment? _horizontalAlignment;

		private OpenXmlParagraphIndentationModel _indentation;

		private double? _pointsBefore;

		private double _lineHeight;

		private bool _lineSpacingAtLeast;

		public OpenXmlParagraphIndentationModel Indentation => _indentation;

		public bool RightToLeft
		{
			private get;
			set;
		}

		public int ListLevel
		{
			set
			{
				_listLevel = value;
			}
		}

		public int ListStyleId
		{
			set
			{
				_listStyleId = value;
			}
		}

		public HorizontalAlignment HorizontalAlign
		{
			set
			{
				_horizontalAlignment = value;
			}
		}

		public double PointsBefore
		{
			set
			{
				_pointsBefore = value;
			}
		}

		public double PointsAfter
		{
			private get;
			set;
		}

		public double LineHeight
		{
			set
			{
				_lineSpacingAtLeast = true;
				_lineHeight = value;
			}
		}

		public OpenXmlParagraphPropertiesModel()
		{
			_indentation = new OpenXmlParagraphIndentationModel();
			PointsAfter = 0.0;
			_lineSpacingAtLeast = false;
			_lineHeight = 12.0;
		}

		public void Write(TextWriter writer)
		{
			writer.Write("<w:pPr>");
			if (_listLevel.HasValue || _listStyleId.HasValue)
			{
				writer.Write("<w:numPr>");
				if (_listLevel.HasValue)
				{
					writer.Write("<w:ilvl w:val=\"");
					writer.Write(_listLevel);
					writer.Write("\"/>");
				}
				if (_listStyleId.HasValue)
				{
					writer.Write("<w:numId w:val=\"");
					writer.Write(_listStyleId);
					writer.Write("\"/>");
				}
				writer.Write("</w:numPr>");
			}
			if (RightToLeft)
			{
				writer.Write("<w:bidi/>");
			}
			writer.Write("<w:spacing ");
			if (_pointsBefore.HasValue)
			{
				writer.Write("w:before=\"");
				writer.Write(WordOpenXmlUtils.TwipsToString(WordOpenXmlUtils.PointsToTwips(_pointsBefore.Value), 0, 31680));
				writer.Write("\" ");
			}
			writer.Write("w:after=\"");
			writer.Write(WordOpenXmlUtils.TwipsToString(WordOpenXmlUtils.PointsToTwips(PointsAfter), 0, 31680));
			writer.Write("\" w:line=\"");
			writer.Write(WordOpenXmlUtils.TwipsToString(WordOpenXmlUtils.PointsToTwips(_lineHeight), 0, 31680));
			writer.Write(_lineSpacingAtLeast ? "\" w:lineRule=\"atLeast\"/>" : "\" w:lineRule=\"auto\"/>");
			_indentation.Write(writer);
			if (_horizontalAlignment.HasValue)
			{
				switch (_horizontalAlignment.Value)
				{
				case HorizontalAlignment.Center:
					writer.Write("<w:jc w:val=\"center\"/>");
					break;
				case HorizontalAlignment.Left:
					writer.Write("<w:jc w:val=\"left\"/>");
					break;
				case HorizontalAlignment.Right:
					writer.Write("<w:jc w:val=\"right\"/>");
					break;
				}
			}
			writer.Write("</w:pPr>");
		}
	}
}
