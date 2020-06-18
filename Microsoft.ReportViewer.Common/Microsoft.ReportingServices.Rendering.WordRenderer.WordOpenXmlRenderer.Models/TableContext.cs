namespace Microsoft.ReportingServices.Rendering.WordRenderer.WordOpenXmlRenderer.Models
{
	internal sealed class TableContext
	{
		internal enum State
		{
			InBody,
			InHeaderFooter,
			InTable,
			InRow,
			InCell
		}

		private OpenXmlTableModel _currentTable;

		private OpenXmlTableRowModel _currentRow;

		private OpenXmlTableCellModel _currentCell;

		private State _state;

		private InterleavingWriter _interleavingWriter;

		private bool _startedInHeaderFooter;

		private int _depth;

		public OpenXmlTableModel CurrentTable => _currentTable;

		public OpenXmlTableRowModel CurrentRow => _currentRow;

		public OpenXmlTableCellModel CurrentCell => _currentCell;

		public State Location => _state;

		public int Depth => _depth;

		public TableContext(InterleavingWriter interleavingWriter, bool inHeaderFooter)
		{
			_interleavingWriter = interleavingWriter;
			_state = (inHeaderFooter ? State.InHeaderFooter : State.InBody);
			_startedInHeaderFooter = inHeaderFooter;
			_depth = 0;
		}

		public void WriteTableBegin(float left, bool layoutTable, AutoFit autofit)
		{
			if (_state == State.InBody || _state == State.InHeaderFooter || _state == State.InCell)
			{
				if (_state == State.InCell)
				{
					_currentCell.PrepareForNestedTable();
				}
				_ = _state;
				_state = State.InTable;
				_currentTable = new OpenXmlTableModel(CurrentTable, _interleavingWriter, autofit);
				_depth++;
			}
			else
			{
				WordOpenXmlUtils.FailCodingError();
			}
		}

		public void WriteTableRowBegin(float left, float height, float[] columnWidths)
		{
			if (_state == State.InTable)
			{
				_currentTable.WriteProperties();
				int[] array = new int[columnWidths.Length];
				int num = 0;
				for (int i = 0; i < array.Length; i++)
				{
					int num2 = WordOpenXmlUtils.ToTwips(columnWidths[i]);
					if ((float)(num + num2) > 31680f)
					{
						num2 = (int)(31680f - (float)num);
					}
					num += num2;
					array[i] = num2;
				}
				_currentTable.TableGrid.AddRow(array);
				_state = State.InRow;
				_currentRow = new OpenXmlTableRowModel(CurrentRow, _interleavingWriter);
				_currentRow.RowProperties.Height = height;
				_currentRow.ColumnWidths = array;
				_currentRow.RowProperties.RowIndent = left;
			}
			else
			{
				WordOpenXmlUtils.FailCodingError();
			}
		}

		public void WriteTableCellBegin(int cellIndex, int numColumns, bool firstVertMerge, bool firstHorzMerge, bool vertMerge, bool horzMerge)
		{
			if (_state == State.InRow)
			{
				_state = State.InCell;
				_currentCell = new OpenXmlTableCellModel(CurrentCell, _currentTable.TableProperties, _interleavingWriter.TextWriter);
				_currentCell.CellProperties.Width = _currentRow.ColumnWidths[cellIndex];
				if (firstHorzMerge)
				{
					_currentCell.CellProperties.HorizontalMerge = OpenXmlTableCellPropertiesModel.MergeState.Start;
				}
				else if (horzMerge)
				{
					_currentCell.CellProperties.HorizontalMerge = OpenXmlTableCellPropertiesModel.MergeState.Continue;
				}
				if (firstVertMerge)
				{
					_currentCell.CellProperties.VerticalMerge = OpenXmlTableCellPropertiesModel.MergeState.Start;
				}
				else if (vertMerge)
				{
					_currentCell.CellProperties.VerticalMerge = OpenXmlTableCellPropertiesModel.MergeState.Continue;
				}
				_currentCell.CellProperties.BackgroundColor = _currentTable.TableProperties.BackgroundColor;
			}
			else
			{
				WordOpenXmlUtils.FailCodingError();
			}
		}

		public void WriteTableEnd()
		{
			if (_state == State.InTable)
			{
				CurrentTable.WriteCloseTag();
				_currentTable = CurrentTable.ContainingTable;
				if (CurrentTable == null)
				{
					_state = (_startedInHeaderFooter ? State.InHeaderFooter : State.InBody);
				}
				else
				{
					_state = State.InCell;
				}
				_depth--;
			}
			else
			{
				WordOpenXmlUtils.FailCodingError();
			}
		}

		public void WriteTableRowEnd()
		{
			if (_state == State.InRow)
			{
				CurrentRow.WriteCloseTag();
				_currentRow = CurrentRow.ContainingRow;
				_state = State.InTable;
			}
			else
			{
				WordOpenXmlUtils.FailCodingError();
			}
		}

		public void WriteTableCellEnd(int cellIndex, BorderContext borderContext, bool emptyLayoutCell)
		{
			if (_state == State.InCell)
			{
				CurrentCell.WriteCloseTag(emptyLayoutCell);
				_currentCell = CurrentCell.ContainingCell;
				_state = State.InRow;
			}
			else
			{
				WordOpenXmlUtils.FailCodingError();
			}
		}
	}
}
