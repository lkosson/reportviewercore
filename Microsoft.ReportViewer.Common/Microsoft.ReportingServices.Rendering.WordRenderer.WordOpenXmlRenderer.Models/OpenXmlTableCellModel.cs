using System.Collections.Generic;
using System.IO;

namespace Microsoft.ReportingServices.Rendering.WordRenderer.WordOpenXmlRenderer.Models
{
	internal sealed class OpenXmlTableCellModel
	{
		private enum BorderOverride : byte
		{
			UseCellBorder,
			ClearBorder,
			UseTableBorder
		}

		public interface ICellContent
		{
			void Write(TextWriter writer);
		}

		private readonly OpenXmlTableCellModel _containingCell;

		private readonly OpenXmlTablePropertiesModel _tableProperties;

		private IList<ICellContent> _contents;

		private TextWriter _textWriter;

		private BorderOverride _overrideTopBorder;

		private BorderOverride _overrideBottomBorder;

		private BorderOverride _overrideLeftBorder;

		private BorderOverride _overrideRightBorder;

		private OpenXmlTableCellPropertiesModel _cellProperties;

		public OpenXmlTableCellModel ContainingCell => _containingCell;

		public OpenXmlTableCellPropertiesModel CellProperties => _cellProperties;

		public OpenXmlTableCellModel(OpenXmlTableCellModel containingCell, OpenXmlTablePropertiesModel tableProperties, TextWriter textWriter)
		{
			_containingCell = containingCell;
			_tableProperties = tableProperties;
			_textWriter = textWriter;
			_contents = new List<ICellContent>();
			_textWriter.Write("<w:tc>");
			_cellProperties = new OpenXmlTableCellPropertiesModel();
		}

		public void BlockBorderAt(TableData.Positions side)
		{
			switch (side)
			{
			case TableData.Positions.Top:
				ClearBorder(ref _overrideTopBorder);
				break;
			case TableData.Positions.Bottom:
				ClearBorder(ref _overrideBottomBorder);
				break;
			case TableData.Positions.Left:
				ClearBorder(ref _overrideLeftBorder);
				break;
			case TableData.Positions.Right:
				ClearBorder(ref _overrideRightBorder);
				break;
			}
		}

		public void UseTopTableBorder()
		{
			_overrideTopBorder = BorderOverride.UseTableBorder;
		}

		public void UseBottomTableBorder()
		{
			_overrideBottomBorder = BorderOverride.UseTableBorder;
		}

		public void UseLeftTableBorder()
		{
			_overrideLeftBorder = BorderOverride.UseTableBorder;
		}

		public void UseRightTableBorder()
		{
			_overrideRightBorder = BorderOverride.UseTableBorder;
		}

		private void ClearBorder(ref BorderOverride borderOverride)
		{
			if (borderOverride == BorderOverride.UseCellBorder)
			{
				borderOverride = BorderOverride.ClearBorder;
			}
		}

		private void Flush()
		{
			if (_cellProperties != null)
			{
				OverrideBorders();
				CellProperties.Write(_textWriter);
				_cellProperties = null;
			}
			foreach (ICellContent content in _contents)
			{
				content.Write(_textWriter);
			}
			_contents = new List<ICellContent>();
		}

		private void OverrideBorders()
		{
			if (_overrideTopBorder == BorderOverride.ClearBorder)
			{
				CellProperties.ClearBorderTop();
			}
			else if (_overrideTopBorder == BorderOverride.UseTableBorder)
			{
				UpdateBorder(CellProperties.BorderTop, _tableProperties.BorderTop);
			}
			if (_overrideBottomBorder == BorderOverride.ClearBorder)
			{
				CellProperties.ClearBorderBottom();
			}
			else if (_overrideBottomBorder == BorderOverride.UseTableBorder)
			{
				UpdateBorder(CellProperties.BorderBottom, _tableProperties.BorderBottom);
			}
			if (_overrideLeftBorder == BorderOverride.ClearBorder)
			{
				CellProperties.ClearBorderLeft();
			}
			else if (_overrideLeftBorder == BorderOverride.UseTableBorder)
			{
				UpdateBorder(CellProperties.BorderLeft, _tableProperties.BorderLeft);
			}
			if (_overrideRightBorder == BorderOverride.ClearBorder)
			{
				CellProperties.ClearBorderRight();
			}
			else if (_overrideRightBorder == BorderOverride.UseTableBorder)
			{
				UpdateBorder(CellProperties.BorderRight, _tableProperties.BorderRight);
			}
		}

		private void UpdateBorder(OpenXmlBorderPropertiesModel cellBorder, OpenXmlBorderPropertiesModel tableBorder)
		{
			cellBorder.Color = tableBorder.Color;
			cellBorder.Style = tableBorder.Style;
			cellBorder.WidthInEighthPoints = tableBorder.WidthInEighthPoints;
		}

		public void PrepareForNestedTable()
		{
			Flush();
		}

		public void AddContent(ICellContent openXmlParagraphModel)
		{
			_contents.Add(openXmlParagraphModel);
		}

		public void WriteCloseTag(bool emptyLayoutCell)
		{
			bool num = _contents.Count > 0;
			Flush();
			if (!num)
			{
				if (emptyLayoutCell)
				{
					OpenXmlParagraphModel.WriteEmptyLayoutCellParagraph(_textWriter);
				}
				else
				{
					OpenXmlParagraphModel.WriteEmptyParagraph(_textWriter);
				}
			}
			_textWriter.Write("</w:tc>");
		}
	}
}
