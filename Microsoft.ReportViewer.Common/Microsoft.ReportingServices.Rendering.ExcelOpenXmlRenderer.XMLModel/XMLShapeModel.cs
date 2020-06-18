using Microsoft.ReportingServices.Rendering.ExcelOpenXmlRenderer.Model;
using Microsoft.ReportingServices.Rendering.ExcelOpenXmlRenderer.Parser.drawingml.x2006.spreadsheetDrawing;
using System.Globalization;

namespace Microsoft.ReportingServices.Rendering.ExcelOpenXmlRenderer.XMLModel
{
	internal abstract class XMLShapeModel : IShapeModel
	{
		protected readonly PartManager Manager;

		private AnchorModel _startPosition;

		private AnchorModel _endPosition;

		protected readonly CT_TwoCellAnchor TwoCellAnchor;

		public abstract string Hyperlink
		{
			set;
		}

		protected XMLShapeModel(PartManager manager, WsDrPart part, AnchorModel startAnchor, AnchorModel endAnchor)
		{
			Manager = manager;
			TwoCellAnchor = new CT_TwoCellAnchor();
			CT_Marker from = new CT_Marker
			{
				Col = startAnchor.Column,
				ColOff = "0",
				Row = startAnchor.Row,
				RowOff = "0"
			};
			CT_Marker to = new CT_Marker
			{
				Col = endAnchor.Column,
				ColOff = "0",
				Row = endAnchor.Row,
				RowOff = "0"
			};
			TwoCellAnchor.From = from;
			TwoCellAnchor.To = to;
			((CT_Drawing)part.Root).TwoCellAnchor.Add(TwoCellAnchor);
			_startPosition = startAnchor;
			_endPosition = endAnchor;
		}

		public void UpdateColumnOffset(double sizeInPoints, bool start)
		{
			long num = EmuConverter.PointsToEmus((long)sizeInPoints);
			if (start)
			{
				TwoCellAnchor.From.ColOff = ((long)((double)num * _startPosition.OffsetX)).ToString(CultureInfo.InvariantCulture);
			}
			else
			{
				TwoCellAnchor.To.ColOff = ((long)((double)num * _endPosition.OffsetX)).ToString(CultureInfo.InvariantCulture);
			}
		}

		public void UpdateRowOffset(double sizeInPoints, bool start)
		{
			long num = EmuConverter.PointsToEmus((long)sizeInPoints);
			if (start)
			{
				TwoCellAnchor.From.RowOff = ((long)((double)num * _startPosition.OffsetY)).ToString(CultureInfo.InvariantCulture);
			}
			else
			{
				TwoCellAnchor.To.RowOff = ((long)((double)num * _endPosition.OffsetY)).ToString(CultureInfo.InvariantCulture);
			}
		}
	}
}
