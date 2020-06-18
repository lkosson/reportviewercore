using System.IO;

namespace Microsoft.ReportingServices.Rendering.WordRenderer.WordOpenXmlRenderer.Models
{
	internal sealed class OpenXmlParagraphIndentationModel
	{
		private double? _hanging;

		private double? _first;

		private double? _left;

		private double? _right;

		internal double Hanging
		{
			set
			{
				_hanging = value;
			}
		}

		internal double First
		{
			set
			{
				_first = value;
			}
		}

		internal double Left
		{
			set
			{
				_left = value;
			}
		}

		internal double Right
		{
			set
			{
				_right = value;
			}
		}

		public void Write(TextWriter writer)
		{
			if (_hanging.HasValue || _first.HasValue || _left.HasValue || _right.HasValue)
			{
				writer.Write("<w:ind");
				if (_left.HasValue)
				{
					writer.Write(" w:left=\"");
					writer.Write(WordOpenXmlUtils.TwipsToString(WordOpenXmlUtils.PointsToTwips(_left.Value), -31680, 31680));
					writer.Write("\"");
				}
				if (_right.HasValue)
				{
					writer.Write(" w:right=\"");
					writer.Write(WordOpenXmlUtils.TwipsToString(WordOpenXmlUtils.PointsToTwips(_right.Value), -31680, 31680));
					writer.Write("\"");
				}
				if (_hanging.HasValue)
				{
					writer.Write(" w:hanging=\"");
					writer.Write(WordOpenXmlUtils.TwipsToString(WordOpenXmlUtils.PointsToTwips(_hanging.Value), 0, 31680));
					writer.Write("\"");
				}
				if (_first.HasValue)
				{
					writer.Write(" w:firstLine=\"");
					writer.Write(WordOpenXmlUtils.TwipsToString(WordOpenXmlUtils.PointsToTwips(_first.Value), 0, 31680));
					writer.Write("\"");
				}
				writer.Write("/>");
			}
		}
	}
}
