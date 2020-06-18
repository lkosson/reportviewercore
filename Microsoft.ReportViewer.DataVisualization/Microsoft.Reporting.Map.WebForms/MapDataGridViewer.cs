using Microsoft.SqlServer.Types;
using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;

namespace Microsoft.Reporting.Map.WebForms
{
	internal class MapDataGridViewer : Control
	{
		private class DoubleBufferedDataGrid : DataGrid
		{
			internal ArrayList selectedColumns = new ArrayList();

			public bool AllowMultipleSelection;

			public bool InteractiveSelection = true;

			public CurrencyManager CurrencyManager => base.ListManager;

			public int SelectedColumnIndex => (int)selectedColumns[0];

			public string SelectedColumnText
			{
				get
				{
					if (selectedColumns.Count == 0)
					{
						return "";
					}
					return base.TableStyles[0].GridColumnStyles[(int)selectedColumns[0]].HeaderText;
				}
			}

			public int[] SelectedColumnIndices => (int[])selectedColumns.ToArray(typeof(int));

			public string[] SelectedColumnsText
			{
				get
				{
					string[] array = new string[selectedColumns.Count];
					for (int i = 0; i < selectedColumns.Count; i++)
					{
						array[i] = base.TableStyles[0].GridColumnStyles[(int)selectedColumns[i]].HeaderText;
					}
					return array;
				}
			}

			public DoubleBufferedDataGrid()
			{
				SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.DoubleBuffer, value: true);
				base.TabStop = false;
			}

			protected override void OnMouseMove(MouseEventArgs e)
			{
				Point position = new Point(e.X, e.Y);
				if (HitTest(position).Type != HitTestType.RowResize)
				{
					base.OnMouseMove(e);
				}
			}

			protected override void OnMouseDown(MouseEventArgs e)
			{
				Point position = new Point(e.X, e.Y);
				HitTestInfo hitTestInfo = HitTest(position);
				if (hitTestInfo.Type == HitTestType.ColumnHeader || hitTestInfo.Type == HitTestType.Cell)
				{
					if (!InteractiveSelection)
					{
						if (hitTestInfo.Type == HitTestType.ColumnHeader)
						{
							base.OnMouseDown(e);
						}
						return;
					}
					bool flag = (Control.ModifierKeys & Keys.Shift) == Keys.Shift;
					bool flag2 = (Control.ModifierKeys & Keys.Control) == Keys.Control;
					if (AllowMultipleSelection)
					{
						if (!flag && !flag2)
						{
							selectedColumns.Clear();
						}
						if (flag && !flag2)
						{
							int num = (selectedColumns.Count > 0) ? ((int)selectedColumns[selectedColumns.Count - 1]) : (-1);
							if (hitTestInfo.Column > num)
							{
								for (int i = num + 1; i <= hitTestInfo.Column; i++)
								{
									if (!selectedColumns.Contains(i))
									{
										selectedColumns.Add(i);
									}
								}
							}
							else if (hitTestInfo.Column < num)
							{
								for (int num2 = num - 1; num2 >= hitTestInfo.Column; num2--)
								{
									if (!selectedColumns.Contains(num2))
									{
										selectedColumns.Add(num2);
									}
								}
							}
						}
					}
					if (flag2)
					{
						if (selectedColumns.Contains(hitTestInfo.Column))
						{
							selectedColumns.Remove(hitTestInfo.Column);
						}
						else
						{
							selectedColumns.Add(hitTestInfo.Column);
						}
					}
					if (!flag && !flag2)
					{
						selectedColumns.Clear();
						selectedColumns.Add(hitTestInfo.Column);
					}
					Invalidate();
				}
				else if (hitTestInfo.Type == HitTestType.RowResize)
				{
					return;
				}
				base.OnMouseDown(e);
			}

			public void SetColumnsSize()
			{
				Graphics graphics = null;
				StringFormat stringFormat = null;
				DataGridTableStyle dataGridTableStyle = base.TableStyles[0];
				try
				{
					graphics = Graphics.FromHwnd(base.Handle);
					stringFormat = new StringFormat(StringFormat.GenericTypographic);
					int num = (int)Math.Ceiling(graphics.MeasureString("WW", Font, base.Width, stringFormat).Width);
					float[] array = new float[dataGridTableStyle.GridColumnStyles.Count];
					for (int i = 0; i < dataGridTableStyle.GridColumnStyles.Count; i++)
					{
						array[i] = graphics.MeasureString(dataGridTableStyle.GridColumnStyles[i].HeaderText, Font, base.Width, stringFormat).Width + (float)num;
					}
					bool flag = true;
					foreach (DataRowView item in (DataView)base.DataSource)
					{
						for (int j = 0; j < dataGridTableStyle.GridColumnStyles.Count; j++)
						{
							string text = item[dataGridTableStyle.GridColumnStyles[j].HeaderText].ToString();
							if (text.Length > 50)
							{
								text = item[dataGridTableStyle.GridColumnStyles[j].HeaderText].ToString().Substring(0, 50);
							}
							SizeF sizeF = graphics.MeasureString(text, Font, base.Width, stringFormat);
							if (flag && dataGridTableStyle.AllowSorting)
							{
								sizeF.Width += 3.5f * (float)num;
							}
							else
							{
								sizeF.Width += num;
							}
							if (sizeF.Width > array[j])
							{
								array[j] = sizeF.Width;
							}
						}
						flag = false;
					}
					for (int k = 0; k < array.Length; k++)
					{
						dataGridTableStyle.GridColumnStyles[k].Width = (int)array[k];
					}
				}
				finally
				{
					graphics?.Dispose();
					stringFormat?.Dispose();
				}
			}

			public void SelectColumnByIndex(int index)
			{
				SelectColumnByIndex(new int[1]
				{
					index
				});
			}

			public void SelectColumnByIndex(ICollection indices)
			{
				selectedColumns.Clear();
				foreach (int index in indices)
				{
					if (!selectedColumns.Contains(index))
					{
						selectedColumns.Add(index);
					}
				}
				Invalidate();
			}

			public void SelectColumnByName(string name)
			{
				SelectColumnByName(new string[1]
				{
					name
				});
			}

			public void SelectColumnByName(ICollection names)
			{
				ArrayList arrayList = new ArrayList();
				selectedColumns.Clear();
				foreach (DataGridColumnStyle gridColumnStyle in base.TableStyles[0].GridColumnStyles)
				{
					arrayList.Add(gridColumnStyle.HeaderText);
				}
				foreach (string name in names)
				{
					int num = arrayList.IndexOf(name);
					if (num >= 0 && !selectedColumns.Contains(num))
					{
						selectedColumns.Add(num);
					}
				}
				Invalidate();
			}

			public void ScrollToColumn(string name)
			{
				if (!base.HorizScrollBar.Visible)
				{
					return;
				}
				int num = -1;
				for (int i = 0; i < base.TableStyles[0].GridColumnStyles.Count; i++)
				{
					if (base.TableStyles[0].GridColumnStyles[i].HeaderText == name)
					{
						num = i;
					}
				}
				if (num != -1)
				{
					base.CurrentCell = new DataGridCell(0, 0);
					base.CurrentCell = new DataGridCell(0, num);
				}
			}
		}

		internal class NonEditableCellColumn : DataGridTextBoxColumn
		{
			private bool rightAligned;

			private int columnIndex = -1;

			public NonEditableCellColumn(int colIndex, bool rightAligned)
			{
				this.rightAligned = rightAligned;
				columnIndex = colIndex;
			}

			protected override void Edit(CurrencyManager source, int rowNum, Rectangle bounds, bool readOnly, string instantText, bool cellIsVisible)
			{
			}

			protected override void Paint(Graphics g, Rectangle bounds, CurrencyManager source, int rowNum, Brush backBrush, Brush foreBrush, bool alignToRight)
			{
				if (DataGridTableStyle != null && DataGridTableStyle.DataGrid != null)
				{
					DoubleBufferedDataGrid doubleBufferedDataGrid = DataGridTableStyle.DataGrid as DoubleBufferedDataGrid;
					if (doubleBufferedDataGrid != null && doubleBufferedDataGrid.selectedColumns.Contains(columnIndex))
					{
						backBrush = new SolidBrush(doubleBufferedDataGrid.SelectionBackColor);
						foreBrush = new SolidBrush(doubleBufferedDataGrid.SelectionForeColor);
					}
				}
				string text = GetText(GetColumnValueAtRow(source, rowNum));
				if (text.Length > 50)
				{
					text = text.Substring(0, 50) + "...";
				}
				PaintText(g, bounds, text, backBrush, foreBrush, rightAligned);
			}

			protected override void Paint(Graphics g, Rectangle bounds, CurrencyManager source, int rowNum, bool alignToRight)
			{
				string text = GetText(GetColumnValueAtRow(source, rowNum));
				if (text.Length > 50)
				{
					text = text.Substring(0, 50) + "...";
				}
				PaintText(g, bounds, text, alignToRight);
			}

			private string GetText(object value)
			{
				if (value is DBNull)
				{
					return NullText;
				}
				if (!string.IsNullOrEmpty(base.Format) && value is IFormattable)
				{
					try
					{
						return ((IFormattable)value).ToString(base.Format, base.FormatInfo);
					}
					catch
					{
					}
				}
				else
				{
					TypeConverter converter = TypeDescriptor.GetConverter(PropertyDescriptor.PropertyType);
					if (converter != null && converter.CanConvertTo(typeof(string)))
					{
						return (string)converter.ConvertTo(value, typeof(string));
					}
				}
				if (value == null)
				{
					return "";
				}
				return value.ToString();
			}
		}

		private DoubleBufferedDataGrid dataGrid;

		private bool interactiveSelection = true;

		private bool allowMultipleSelection;

		public bool InteractiveSelection
		{
			get
			{
				return interactiveSelection;
			}
			set
			{
				interactiveSelection = value;
				if (dataGrid != null)
				{
					dataGrid.InteractiveSelection = value;
				}
			}
		}

		public bool AllowMultipleSelection
		{
			get
			{
				return allowMultipleSelection;
			}
			set
			{
				allowMultipleSelection = value;
				if (dataGrid != null)
				{
					dataGrid.AllowMultipleSelection = value;
				}
			}
		}

		public int SelectedColumnIndex => dataGrid.SelectedColumnIndex;

		public string SelectedColumnText => dataGrid.SelectedColumnText;

		public int[] SelectedColumnIndices => dataGrid.SelectedColumnIndices;

		public string[] SelectedColumnsText => dataGrid.SelectedColumnsText;

		public DataGrid InternalGrid => dataGrid;

		public void Initialize(DataTable data)
		{
			base.Controls.Clear();
			if (data == null || data.Columns.Count == 0)
			{
				return;
			}
			dataGrid = new DoubleBufferedDataGrid();
			dataGrid.Dock = DockStyle.Fill;
			dataGrid.CaptionVisible = false;
			dataGrid.InteractiveSelection = InteractiveSelection;
			dataGrid.AllowMultipleSelection = AllowMultipleSelection;
			base.Controls.Add(dataGrid);
			DataView defaultView = data.DefaultView;
			if (data.Rows.Count > 1000)
			{
				DataTable dataTable = new DataTable();
				dataTable.Locale = CultureInfo.CurrentCulture;
				foreach (DataColumn column in data.Columns)
				{
					dataTable.Columns.Add(new DataColumn(column.ColumnName, column.DataType, column.Expression));
				}
				for (int i = 0; i < 1000; i++)
				{
					dataTable.ImportRow(data.Rows[i]);
				}
				defaultView = dataTable.DefaultView;
			}
			defaultView.AllowNew = false;
			defaultView.AllowDelete = false;
			defaultView.AllowEdit = false;
			dataGrid.DataSource = defaultView;
			DataGridTableStyle dataGridTableStyle = new DataGridTableStyle((CurrencyManager)new BindingContext()[dataGrid.DataSource]);
			dataGridTableStyle.GridColumnStyles.Clear();
			if (dataGrid.CurrencyManager == null)
			{
				return;
			}
			int num = 0;
			foreach (PropertyDescriptor itemProperty in dataGrid.CurrencyManager.GetItemProperties())
			{
				bool rightAligned = false;
				if ((itemProperty.PropertyType.IsPrimitive && itemProperty.PropertyType != typeof(char)) || itemProperty.PropertyType == typeof(decimal))
				{
					rightAligned = true;
				}
				NonEditableCellColumn nonEditableCellColumn = new NonEditableCellColumn(num++, rightAligned);
				nonEditableCellColumn.HeaderText = itemProperty.Name;
				nonEditableCellColumn.MappingName = itemProperty.Name;
				nonEditableCellColumn.NullText = "(null)";
				dataGridTableStyle.GridColumnStyles.Add(nonEditableCellColumn);
			}
			dataGridTableStyle.AllowSorting = true;
			foreach (DataColumn column2 in data.Columns)
			{
				if (column2.DataType == typeof(SqlGeometry) || column2.DataType == typeof(SqlGeography))
				{
					dataGridTableStyle.AllowSorting = false;
				}
			}
			dataGridTableStyle.RowHeadersVisible = false;
			dataGrid.TableStyles.Add(dataGridTableStyle);
			dataGrid.SetColumnsSize();
		}

		public void SelectColumnByIndex(int index)
		{
			if (dataGrid != null)
			{
				dataGrid.SelectColumnByIndex(index);
			}
		}

		public void SelectColumnByIndex(int[] indices)
		{
			if (dataGrid != null)
			{
				dataGrid.SelectColumnByIndex(indices);
			}
		}

		public void SelectColumnByName(string name)
		{
			if (dataGrid != null)
			{
				dataGrid.SelectColumnByName(name);
			}
		}

		public void SelectColumnByName(string[] names)
		{
			if (dataGrid != null)
			{
				dataGrid.SelectColumnByName(names);
			}
		}

		public void ScrollToColumn(string name)
		{
			if (dataGrid != null)
			{
				dataGrid.ScrollToColumn(name);
			}
		}
	}
}
