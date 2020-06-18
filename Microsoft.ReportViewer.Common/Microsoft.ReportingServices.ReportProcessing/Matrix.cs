using Microsoft.ReportingServices.ReportProcessing.ExprHostObjectModel;
using Microsoft.ReportingServices.ReportProcessing.Persistence;
using Microsoft.ReportingServices.ReportProcessing.ReportObjectModel;
using Microsoft.ReportingServices.ReportRendering;
using System;

namespace Microsoft.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class Matrix : Pivot, IPageBreakItem
	{
		private sealed class OWCFlagsCalculator
		{
			private bool m_useOWC = true;

			private StringList m_owcCellNames = new StringList();

			private BoolList m_owcGroupExpression = new BoolList();

			private int m_staticHeadingCount;

			private OWCFlagsCalculator()
			{
			}

			internal static void Calculate(Matrix matrix)
			{
				Global.Tracer.Assert(matrix != null);
				OWCFlagsCalculator oWCFlagsCalculator = new OWCFlagsCalculator();
				oWCFlagsCalculator.CalculateOWCFlags(matrix);
				if (!oWCFlagsCalculator.m_useOWC)
				{
					return;
				}
				matrix.UseOWC = oWCFlagsCalculator.m_useOWC;
				matrix.OwcCellNames = oWCFlagsCalculator.m_owcCellNames;
				int num = 0;
				for (MatrixHeading matrixHeading = matrix.Rows; matrixHeading != null; matrixHeading = matrixHeading.SubHeading)
				{
					if (matrixHeading.Grouping != null)
					{
						matrixHeading.OwcGroupExpression = oWCFlagsCalculator.m_owcGroupExpression[num];
						num++;
					}
				}
				for (MatrixHeading matrixHeading2 = matrix.Columns; matrixHeading2 != null; matrixHeading2 = matrixHeading2.SubHeading)
				{
					if (matrixHeading2.Grouping != null)
					{
						matrixHeading2.OwcGroupExpression = oWCFlagsCalculator.m_owcGroupExpression[num];
						num++;
					}
				}
			}

			private void CalculateOWCFlags(Matrix matrix)
			{
				CalculateOWCFlags(matrix.Rows);
				if (IsFinish())
				{
					return;
				}
				CalculateOWCFlags(matrix.Columns);
				if (IsFinish() || matrix.CellReportItems == null)
				{
					return;
				}
				int num = 0;
				while (true)
				{
					if (num < matrix.CellReportItems.Count)
					{
						DetectIllegalReportItems(matrix.CellReportItems[num]);
						if (IsFinish())
						{
							return;
						}
						TextBox textBox = FindNotAlwaysHiddenTextBox(matrix.CellReportItems[num]);
						if (IsFinish())
						{
							return;
						}
						if (textBox == null)
						{
							m_useOWC = false;
							return;
						}
						Global.Tracer.Assert(textBox.Value != null);
						DataAggregateInfo sumAggregateWithoutScope = textBox.Value.GetSumAggregateWithoutScope();
						if (sumAggregateWithoutScope == null)
						{
							break;
						}
						Global.Tracer.Assert(sumAggregateWithoutScope.Expressions != null);
						Global.Tracer.Assert(1 == sumAggregateWithoutScope.Expressions.Length);
						if (ExpressionInfo.Types.Field == sumAggregateWithoutScope.Expressions[0].Type)
						{
							m_owcCellNames.Add(sumAggregateWithoutScope.Expressions[0].Value);
						}
						else
						{
							m_owcCellNames.Add(textBox.Name);
						}
						num++;
						continue;
					}
					return;
				}
				m_useOWC = false;
			}

			private void CalculateOWCFlags(MatrixHeading heading)
			{
				if (heading == null)
				{
					return;
				}
				if (heading.Grouping == null)
				{
					m_staticHeadingCount++;
					if (m_staticHeadingCount > 1)
					{
						m_useOWC = false;
						return;
					}
					if (heading.SubHeading != null)
					{
						m_useOWC = false;
						return;
					}
					if (heading.ReportItems != null)
					{
						for (int i = 0; i < heading.ReportItems.Count; i++)
						{
							DetectIllegalReportItems(heading.ReportItems[i]);
							if (IsFinish())
							{
								return;
							}
							TextBox textBox = FindNotAlwaysHiddenTextBox(heading.ReportItems[i]);
							if (IsFinish())
							{
								return;
							}
							if (textBox == null)
							{
								m_useOWC = false;
								return;
							}
						}
					}
				}
				else
				{
					ExpressionInfo expressionInfo = null;
					if (heading.Grouping.GroupExpressions != null)
					{
						if (heading.Grouping.GroupExpressions.Count != 1)
						{
							m_useOWC = false;
							return;
						}
						expressionInfo = heading.Grouping.GroupExpressions[0];
					}
					DetectIllegalReportItems(heading.ReportItem);
					if (IsFinish())
					{
						return;
					}
					TextBox textBox2 = FindNotAlwaysHiddenTextBox(heading.ReportItem);
					if (IsFinish())
					{
						return;
					}
					if (textBox2 == null)
					{
						m_useOWC = false;
						return;
					}
					Global.Tracer.Assert(expressionInfo != null);
					Global.Tracer.Assert(textBox2.Value != null);
					if (expressionInfo.OriginalText != textBox2.Value.OriginalText)
					{
						m_owcGroupExpression.Add(true);
					}
					else
					{
						m_owcGroupExpression.Add(false);
					}
				}
				CalculateOWCFlags(heading.SubHeading);
			}

			private void DetectIllegalReportItems(ReportItem reportItem)
			{
				if (reportItem is DataRegion || reportItem is Image || reportItem is SubReport || reportItem is ActiveXControl || reportItem is CheckBox)
				{
					m_useOWC = false;
				}
				else if (reportItem is Rectangle)
				{
					DetectIllegalReportItems(((Rectangle)reportItem).ReportItems);
				}
			}

			private void DetectIllegalReportItems(ReportItemCollection reportItems)
			{
				if (reportItems == null)
				{
					return;
				}
				for (int i = 0; i < reportItems.Count; i++)
				{
					DetectIllegalReportItems(reportItems[i]);
					if (IsFinish())
					{
						break;
					}
				}
			}

			private TextBox FindNotAlwaysHiddenTextBox(ReportItem reportItem)
			{
				if (reportItem is TextBox)
				{
					if (Visibility.GetSharedHidden(reportItem.Visibility) != 0)
					{
						return (TextBox)reportItem;
					}
				}
				else if (reportItem is Rectangle)
				{
					return FindNotAlwaysHiddenTextBox(((Rectangle)reportItem).ReportItems);
				}
				return null;
			}

			private TextBox FindNotAlwaysHiddenTextBox(ReportItemCollection reportItems)
			{
				if (reportItems == null)
				{
					return null;
				}
				TextBox textBox = null;
				for (int i = 0; i < reportItems.Count; i++)
				{
					ReportItem reportItem = reportItems[i];
					TextBox textBox2 = FindNotAlwaysHiddenTextBox(reportItem);
					if (IsFinish())
					{
						return null;
					}
					if (textBox2 != null)
					{
						if (textBox != null)
						{
							m_useOWC = false;
							return null;
						}
						textBox = textBox2;
					}
				}
				return textBox;
			}

			private bool IsFinish()
			{
				return !m_useOWC;
			}
		}

		private sealed class TopLevelItemsSizes
		{
			private MatrixColumnList m_columns;

			private MatrixRowList m_rows;

			private InitializationContext m_context;

			private TopLevelItemsSizes(MatrixColumnList columns, MatrixRowList rows, InitializationContext context)
			{
				m_columns = columns;
				m_rows = rows;
				m_context = context;
			}

			internal static void Calculate(Matrix matrix, double cornerWidth, double cornerHeight, double colsWidth, double rowsHeight, InitializationContext context)
			{
				new TopLevelItemsSizes(matrix.MatrixColumns, matrix.MatrixRows, context).CalculateSizes(matrix, cornerWidth, cornerHeight, colsWidth, rowsHeight);
			}

			private void CalculateSizes(Matrix matrix, double cornerWidth, double cornerHeight, double colsWidth, double rowsHeight)
			{
				CalculateCorner(matrix, cornerWidth, cornerHeight);
				CalculateColumns(matrix.Columns, colsWidth);
				CalculateRows(matrix.Rows, rowsHeight);
				CalculateCells(matrix);
			}

			private void CalculateCorner(Matrix matrix, double width, double height)
			{
				if (matrix.CornerReportItems != null && 0 < matrix.CornerReportItems.Count)
				{
					CalculateSize(matrix.CornerReportItems[0], width, height);
				}
			}

			private void CalculateCells(Matrix matrix)
			{
				int num = 0;
				for (int i = 0; i < m_rows.Count; i++)
				{
					for (int j = 0; j < m_columns.Count; j++)
					{
						CalculateSize(matrix.CellReportItems[num], m_columns[j].WidthValue, m_rows[i].HeightValue);
						num++;
					}
				}
			}

			private void CalculateColumns(MatrixHeading column, double width)
			{
				if (column == null)
				{
					return;
				}
				double num = width;
				if (column.Grouping == null)
				{
					if (column.ReportItems != null)
					{
						for (int i = 0; i < column.ReportItems.Count; i++)
						{
							CalculateSize(column.ReportItems[i], m_columns[i].WidthValue, column.SizeValue);
							if (m_columns[i].WidthValue < num)
							{
								num = m_columns[i].WidthValue;
							}
						}
					}
				}
				else
				{
					if (column.Subtotal != null)
					{
						CalculateSize(column.Subtotal.ReportItem, width, column.SizeValue);
					}
					CalculateSize(column.ReportItem, width, column.SizeValue);
				}
				CalculateColumns(column.SubHeading, num);
			}

			private void CalculateRows(MatrixHeading row, double height)
			{
				if (row == null)
				{
					return;
				}
				double num = height;
				if (row.Grouping == null)
				{
					if (row.ReportItems != null)
					{
						for (int i = 0; i < row.ReportItems.Count; i++)
						{
							CalculateSize(row.ReportItems[i], row.SizeValue, m_rows[i].HeightValue);
							if (m_rows[i].HeightValue < num)
							{
								num = m_rows[i].HeightValue;
							}
						}
					}
				}
				else
				{
					if (row.Subtotal != null)
					{
						CalculateSize(row.Subtotal.ReportItem, row.SizeValue, height);
					}
					CalculateSize(row.ReportItem, row.SizeValue, height);
				}
				CalculateRows(row.SubHeading, num);
			}

			private void CalculateSize(ReportItem item, double width, double height)
			{
				item?.CalculateSizes(width, height, m_context, overwrite: true);
			}
		}

		private MatrixHeading m_columns;

		private MatrixHeading m_rows;

		private ReportItemCollection m_cornerReportItems;

		private ReportItemCollection m_cellReportItems;

		private IntList m_cellIDs;

		private bool m_propagatedPageBreakAtStart;

		private bool m_propagatedPageBreakAtEnd;

		private int m_innerRowLevelWithPageBreak = -1;

		private MatrixRowList m_matrixRows;

		private MatrixColumnList m_matrixColumns;

		private int m_groupsBeforeRowHeaders;

		private bool m_layoutDirection;

		[Reference]
		private MatrixHeading m_staticColumns;

		[Reference]
		private MatrixHeading m_staticRows;

		private bool m_useOWC;

		private StringList m_owcCellNames;

		private string m_cellDataElementName;

		private bool m_columnGroupingFixedHeader;

		private bool m_rowGroupingFixedHeader;

		[NonSerialized]
		private bool m_firstInstance = true;

		[NonSerialized]
		private BoolList m_firstCellInstances;

		[NonSerialized]
		private MatrixExprHost m_exprHost;

		[NonSerialized]
		private int m_currentPage = -1;

		[NonSerialized]
		private int m_cellPage = -1;

		[NonSerialized]
		private ReportProcessing.PageTextboxes m_cellPageTextboxes;

		[NonSerialized]
		private ReportProcessing.PageTextboxes m_columnHeaderPageTextboxes;

		[NonSerialized]
		private ReportProcessing.PageTextboxes m_rowHeaderPageTextboxes;

		[NonSerialized]
		private NonComputedUniqueNames m_cornerNonComputedUniqueNames;

		[NonSerialized]
		private bool m_inOutermostSubtotalCell;

		[NonSerialized]
		private ReportSizeCollection m_cellHeightsForRendering;

		[NonSerialized]
		private ReportSizeCollection m_cellWidthsForRendering;

		[NonSerialized]
		private string[] m_cellIDsForRendering;

		internal override ObjectType ObjectType => ObjectType.Matrix;

		internal ReportItemCollection CornerReportItems
		{
			get
			{
				return m_cornerReportItems;
			}
			set
			{
				m_cornerReportItems = value;
			}
		}

		internal ReportItem CornerReportItem
		{
			get
			{
				if (m_cornerReportItems != null && 0 < m_cornerReportItems.Count)
				{
					return m_cornerReportItems[0];
				}
				return null;
			}
		}

		internal override PivotHeading PivotColumns => m_columns;

		internal override PivotHeading PivotRows => m_rows;

		internal MatrixHeading Columns
		{
			get
			{
				return m_columns;
			}
			set
			{
				m_columns = value;
			}
		}

		internal MatrixHeading Rows
		{
			get
			{
				return m_rows;
			}
			set
			{
				m_rows = value;
			}
		}

		internal ReportItemCollection CellReportItems
		{
			get
			{
				return m_cellReportItems;
			}
			set
			{
				m_cellReportItems = value;
			}
		}

		internal override RunningValueInfoList PivotCellRunningValues => m_cellReportItems.RunningValues;

		internal IntList CellIDs
		{
			get
			{
				return m_cellIDs;
			}
			set
			{
				m_cellIDs = value;
			}
		}

		internal bool PropagatedPageBreakAtStart
		{
			get
			{
				return m_propagatedPageBreakAtStart;
			}
			set
			{
				m_propagatedPageBreakAtStart = value;
			}
		}

		internal bool PropagatedPageBreakAtEnd
		{
			get
			{
				return m_propagatedPageBreakAtEnd;
			}
			set
			{
				m_propagatedPageBreakAtEnd = value;
			}
		}

		internal int InnerRowLevelWithPageBreak
		{
			get
			{
				return m_innerRowLevelWithPageBreak;
			}
			set
			{
				m_innerRowLevelWithPageBreak = value;
			}
		}

		internal MatrixRowList MatrixRows
		{
			get
			{
				return m_matrixRows;
			}
			set
			{
				m_matrixRows = value;
			}
		}

		internal MatrixColumnList MatrixColumns
		{
			get
			{
				return m_matrixColumns;
			}
			set
			{
				m_matrixColumns = value;
			}
		}

		internal int GroupsBeforeRowHeaders
		{
			get
			{
				return m_groupsBeforeRowHeaders;
			}
			set
			{
				m_groupsBeforeRowHeaders = value;
			}
		}

		internal bool LayoutDirection
		{
			get
			{
				return m_layoutDirection;
			}
			set
			{
				m_layoutDirection = value;
			}
		}

		internal override PivotHeading PivotStaticColumns => m_staticColumns;

		internal override PivotHeading PivotStaticRows => m_staticRows;

		internal MatrixHeading StaticColumns
		{
			get
			{
				return m_staticColumns;
			}
			set
			{
				m_staticColumns = value;
			}
		}

		internal MatrixHeading StaticRows
		{
			get
			{
				return m_staticRows;
			}
			set
			{
				m_staticRows = value;
			}
		}

		internal bool UseOWC
		{
			get
			{
				return m_useOWC;
			}
			set
			{
				m_useOWC = value;
			}
		}

		internal StringList OwcCellNames
		{
			get
			{
				return m_owcCellNames;
			}
			set
			{
				m_owcCellNames = value;
			}
		}

		internal string CellDataElementName
		{
			get
			{
				return m_cellDataElementName;
			}
			set
			{
				m_cellDataElementName = value;
			}
		}

		internal bool FirstInstance
		{
			get
			{
				return m_firstInstance;
			}
			set
			{
				m_firstInstance = value;
			}
		}

		internal BoolList FirstCellInstances
		{
			get
			{
				return m_firstCellInstances;
			}
			set
			{
				m_firstCellInstances = value;
			}
		}

		internal MatrixExprHost MatrixExprHost => m_exprHost;

		protected override DataRegionExprHost DataRegionExprHost => m_exprHost;

		internal int CurrentPage
		{
			get
			{
				return m_currentPage;
			}
			set
			{
				m_currentPage = value;
			}
		}

		internal NonComputedUniqueNames CornerNonComputedUniqueNames
		{
			get
			{
				return m_cornerNonComputedUniqueNames;
			}
			set
			{
				m_cornerNonComputedUniqueNames = value;
			}
		}

		internal bool InOutermostSubtotalCell
		{
			get
			{
				return m_inOutermostSubtotalCell;
			}
			set
			{
				m_inOutermostSubtotalCell = value;
			}
		}

		internal ReportSizeCollection CellHeightsForRendering
		{
			get
			{
				if (m_cellHeightsForRendering == null)
				{
					m_cellHeightsForRendering = new ReportSizeCollection(m_matrixRows.Count);
					for (int i = 0; i < m_matrixRows.Count; i++)
					{
						MatrixRow matrixRow = m_matrixRows[i];
						m_cellHeightsForRendering[i] = new ReportSize(matrixRow.Height, matrixRow.HeightValue);
					}
				}
				return m_cellHeightsForRendering;
			}
		}

		internal ReportSizeCollection CellWidthsForRendering
		{
			get
			{
				if (m_cellWidthsForRendering == null)
				{
					m_cellWidthsForRendering = new ReportSizeCollection(m_matrixColumns.Count);
					for (int i = 0; i < m_matrixColumns.Count; i++)
					{
						MatrixColumn matrixColumn = m_matrixColumns[i];
						m_cellWidthsForRendering[i] = new ReportSize(matrixColumn.Width, matrixColumn.WidthValue);
					}
				}
				return m_cellWidthsForRendering;
			}
		}

		internal string[] CellIDsForRendering
		{
			get
			{
				return m_cellIDsForRendering;
			}
			set
			{
				m_cellIDsForRendering = value;
			}
		}

		internal bool ColumnGroupingFixedHeader
		{
			get
			{
				return m_columnGroupingFixedHeader;
			}
			set
			{
				m_columnGroupingFixedHeader = value;
			}
		}

		internal bool RowGroupingFixedHeader
		{
			get
			{
				return m_rowGroupingFixedHeader;
			}
			set
			{
				m_rowGroupingFixedHeader = value;
			}
		}

		internal ReportProcessing.PageTextboxes CellPageTextboxes => m_cellPageTextboxes;

		internal ReportProcessing.PageTextboxes ColumnHeaderPageTextboxes => m_columnHeaderPageTextboxes;

		internal ReportProcessing.PageTextboxes RowHeaderPageTextboxes => m_rowHeaderPageTextboxes;

		internal int CellPage
		{
			get
			{
				if (0 > m_cellPage)
				{
					m_cellPage = m_currentPage;
				}
				return m_cellPage;
			}
			set
			{
				m_cellPage = value;
			}
		}

		internal Matrix(ReportItem parent)
			: base(parent)
		{
		}

		internal Matrix(int id, int idForCornerReportItems, int idForCellReportItems, ReportItem parent)
			: base(id, parent)
		{
			m_cornerReportItems = new ReportItemCollection(idForCornerReportItems, normal: false);
			m_cellReportItems = new ReportItemCollection(idForCellReportItems, normal: false);
		}

		internal override bool Initialize(InitializationContext context)
		{
			context.Location |= LocationFlags.InMatrixOrTable;
			context.ObjectType = ObjectType;
			context.ObjectName = m_name;
			context.RegisterDataRegion(this);
			InternalInitialize(context);
			context.UnRegisterDataRegion(this);
			return false;
		}

		private void InternalInitialize(InitializationContext context)
		{
			context.Location = (context.Location | LocationFlags.InDataSet | LocationFlags.InDataRegion);
			context.ExprHostBuilder.MatrixStart(m_name);
			base.Initialize(context);
			context.RegisterRunningValues(m_runningValues);
			if (m_visibility != null)
			{
				m_visibility.Initialize(context, isContainer: true, tableRowCol: false);
			}
			CornerInitialize(context);
			context.Location &= ~LocationFlags.InMatrixCellTopLevelItem;
			bool computedSubtotal = false;
			bool flag = false;
			context.Location |= LocationFlags.InMatrixGroupHeader;
			ColumnsInitialize(context, out int expectedNumberOfMatrixColumns, out double size, out computedSubtotal);
			flag = computedSubtotal;
			RowsInitialize(context, out int expectedNumberOfMatrixRows, out double size2, out computedSubtotal);
			context.Location &= ~LocationFlags.InMatrixGroupHeader;
			if (computedSubtotal)
			{
				flag = true;
			}
			MatrixCellInitialize(context, expectedNumberOfMatrixColumns, expectedNumberOfMatrixRows, flag, out double totalCellHeight, out double totalCellWidth);
			if (m_visibility != null)
			{
				m_visibility.UnRegisterReceiver(context);
			}
			context.UnRegisterRunningValues(m_runningValues);
			CopyHeadingAggregates(m_rows);
			m_rows.TransferHeadingAggregates();
			CopyHeadingAggregates(m_columns);
			m_columns.TransferHeadingAggregates();
			m_heightValue = size + totalCellHeight;
			m_height = Converter.ConvertSize(m_heightValue);
			m_widthValue = size2 + totalCellWidth;
			m_width = Converter.ConvertSize(m_widthValue);
			if (!context.ErrorContext.HasError)
			{
				TopLevelItemsSizes.Calculate(this, size2, size, totalCellWidth, totalCellHeight, context);
			}
			base.ExprHostID = context.ExprHostBuilder.MatrixEnd();
		}

		internal override void SetExprHost(ReportExprHost reportExprHost, ObjectModelImpl reportObjectModel)
		{
			if (base.ExprHostID >= 0)
			{
				Global.Tracer.Assert(reportExprHost != null && reportObjectModel != null);
				m_exprHost = reportExprHost.MatrixHostsRemotable[base.ExprHostID];
				DataRegionSetExprHost(m_exprHost, reportObjectModel);
			}
		}

		protected override void DataRendererInitialize(InitializationContext context)
		{
			base.DataRendererInitialize(context);
			CLSNameValidator.ValidateDataElementName(ref m_cellDataElementName, "Cell", context.ObjectType, context.ObjectName, "CellDataElementName", context.ErrorContext);
		}

		internal override void RegisterReceiver(InitializationContext context)
		{
			context.RegisterReportItems(m_cornerReportItems);
			if (m_visibility != null)
			{
				m_visibility.RegisterReceiver(context, isContainer: true);
			}
			m_cornerReportItems.RegisterReceiver(context);
			ColumnsRegisterReceiver(context);
			RowsRegisterReceiver(context);
			MatrixCellRegisterReceiver(context);
			if (m_visibility != null)
			{
				m_visibility.UnRegisterReceiver(context);
			}
			context.UnRegisterReportItems(m_cornerReportItems);
		}

		internal void CalculatePropagatedFlags()
		{
			MatrixHeading matrixHeading = m_rows;
			int num = 0;
			do
			{
				if (matrixHeading.Grouping != null)
				{
					if (matrixHeading.Grouping.PageBreakAtStart)
					{
						m_propagatedPageBreakAtStart = true;
						m_innerRowLevelWithPageBreak = num;
					}
					if (matrixHeading.Grouping.PageBreakAtEnd)
					{
						m_propagatedPageBreakAtEnd = true;
						m_innerRowLevelWithPageBreak = num;
					}
				}
				matrixHeading = matrixHeading.SubHeading;
				num++;
			}
			while (matrixHeading != null);
		}

		private void CornerInitialize(InitializationContext context)
		{
			m_cornerReportItems.Initialize(context, registerRunningValues: false);
		}

		private void ColumnsInitialize(InitializationContext context, out int expectedNumberOfMatrixColumns, out double size, out bool computedSubtotal)
		{
			computedSubtotal = false;
			size = 0.0;
			m_columns.DynamicInitialize(column: true, 0, context, ref size);
			m_columns.StaticInitialize(context);
			expectedNumberOfMatrixColumns = ((m_staticColumns == null) ? 1 : m_staticColumns.NumberOfStatics);
			if (m_columns.Grouping == null)
			{
				Global.Tracer.Assert(m_columns.ReportItems != null);
				context.SpecialTransferRunningValues(m_columns.ReportItems.RunningValues);
			}
			else if (m_columns.Subtotal != null)
			{
				Global.Tracer.Assert(m_columns.Subtotal.ReportItems != null);
				context.SpecialTransferRunningValues(m_columns.Subtotal.ReportItems.RunningValues);
				computedSubtotal = m_columns.Subtotal.Computed;
			}
		}

		private void ColumnsRegisterReceiver(InitializationContext context)
		{
			m_columns.DynamicRegisterReceiver(context);
			m_columns.StaticRegisterReceiver(context);
		}

		private void RowsInitialize(InitializationContext context, out int expectedNumberOfMatrixRows, out double size, out bool computedSubtotal)
		{
			computedSubtotal = false;
			size = 0.0;
			m_rows.DynamicInitialize(column: false, 0, context, ref size);
			m_rows.StaticInitialize(context);
			expectedNumberOfMatrixRows = ((m_staticRows == null) ? 1 : m_staticRows.NumberOfStatics);
			if (m_rows.Grouping == null)
			{
				Global.Tracer.Assert(m_rows.ReportItems != null);
				context.SpecialTransferRunningValues(m_rows.ReportItems.RunningValues);
			}
			else if (m_rows.Subtotal != null)
			{
				Global.Tracer.Assert(m_rows.Subtotal.ReportItems != null);
				context.SpecialTransferRunningValues(m_rows.Subtotal.ReportItems.RunningValues);
				computedSubtotal = m_rows.Subtotal.Computed;
			}
		}

		private void RowsRegisterReceiver(InitializationContext context)
		{
			m_rows.DynamicRegisterReceiver(context);
			m_rows.StaticRegisterReceiver(context);
		}

		private void MatrixCellInitialize(InitializationContext context, int expectedNumberOfMatrixColumns, int expectedNumberOfMatrixRows, bool computedSubtotal, out double totalCellHeight, out double totalCellWidth)
		{
			if (expectedNumberOfMatrixColumns != m_matrixColumns.Count)
			{
				context.ErrorContext.Register(ProcessingErrorCode.rsWrongNumberOfMatrixColumns, Severity.Error, context.ObjectType, context.ObjectName, "MatrixColumns");
			}
			if (expectedNumberOfMatrixRows != m_matrixRows.Count)
			{
				context.ErrorContext.Register(ProcessingErrorCode.rsWrongNumberOfMatrixRows, Severity.Error, context.ObjectType, context.ObjectName, "MatrixRows");
			}
			for (int i = 0; i < m_matrixRows.Count; i++)
			{
				if (expectedNumberOfMatrixColumns != m_matrixRows[i].NumberOfMatrixCells)
				{
					context.ErrorContext.Register(ProcessingErrorCode.rsWrongNumberOfMatrixCells, Severity.Error, context.ObjectType, context.ObjectName, "MatrixCells");
				}
			}
			totalCellHeight = 0.0;
			totalCellWidth = 0.0;
			for (int j = 0; j < m_matrixColumns.Count; j++)
			{
				m_matrixColumns[j].Initialize(context);
				totalCellWidth = Math.Round(totalCellWidth + m_matrixColumns[j].WidthValue, Validator.DecimalPrecision);
			}
			for (int k = 0; k < m_matrixRows.Count; k++)
			{
				m_matrixRows[k].Initialize(context);
				totalCellHeight = Math.Round(totalCellHeight + m_matrixRows[k].HeightValue, Validator.DecimalPrecision);
			}
			context.Location = (context.Location | LocationFlags.InMatrixCell | LocationFlags.InMatrixCellTopLevelItem);
			context.MatrixName = m_name;
			context.RegisterTablixCellScope(m_columns.SubHeading == null && m_columns.Grouping == null, m_cellAggregates, m_cellPostSortAggregates);
			for (MatrixHeading matrixHeading = m_rows; matrixHeading != null; matrixHeading = matrixHeading.SubHeading)
			{
				context.RegisterReportItems(matrixHeading.ReportItems);
				if (matrixHeading.Grouping != null)
				{
					context.Location |= LocationFlags.InGrouping;
					context.RegisterGroupingScopeForTablixCell(matrixHeading.Grouping.Name, column: false, matrixHeading.Grouping.SimpleGroupExpressions, matrixHeading.Aggregates, matrixHeading.PostSortAggregates, matrixHeading.RecursiveAggregates, matrixHeading.Grouping);
				}
			}
			if (m_rows.Grouping != null && m_rows.Subtotal != null && m_staticRows != null)
			{
				context.CopyRunningValues(StaticRows.ReportItems.RunningValues, m_aggregates);
			}
			for (MatrixHeading matrixHeading = m_columns; matrixHeading != null; matrixHeading = matrixHeading.SubHeading)
			{
				context.RegisterReportItems(matrixHeading.ReportItems);
				if (matrixHeading.Grouping != null)
				{
					context.Location |= LocationFlags.InGrouping;
					context.RegisterGroupingScopeForTablixCell(matrixHeading.Grouping.Name, column: true, matrixHeading.Grouping.SimpleGroupExpressions, matrixHeading.Aggregates, matrixHeading.PostSortAggregates, matrixHeading.RecursiveAggregates, matrixHeading.Grouping);
				}
			}
			if (m_columns.Grouping != null && m_columns.Subtotal != null && m_staticColumns != null)
			{
				context.CopyRunningValues(StaticColumns.ReportItems.RunningValues, m_aggregates);
			}
			if (computedSubtotal)
			{
				m_cellReportItems.MarkChildrenComputed();
			}
			context.RegisterReportItems(m_cellReportItems);
			OWCFlagsCalculator.Calculate(this);
			bool registerHiddenReceiver = context.RegisterHiddenReceiver;
			context.RegisterHiddenReceiver = false;
			context.RegisterScopeInMatrixCell(base.Name, "0_CellScope" + base.Name, registerMatrixCellScope: true);
			m_cellReportItems.Initialize(context, registerRunningValues: true);
			if (context.IsRunningValueDirectionColumn())
			{
				m_processingInnerGrouping = ProcessingInnerGroupings.Row;
			}
			context.UpdateScopesInMatrixCells(base.Name, GenerateUserSortGroupingList(ProcessingInnerGroupings.Row == m_processingInnerGrouping));
			context.TextboxesWithDetailSortExpressionInitialize();
			context.RegisterHiddenReceiver = registerHiddenReceiver;
			for (MatrixHeading matrixHeading = m_rows; matrixHeading != null; matrixHeading = matrixHeading.SubHeading)
			{
				context.UnRegisterReportItems(matrixHeading.ReportItems);
				if (matrixHeading.Grouping != null)
				{
					context.UnRegisterGroupingScopeForTablixCell(matrixHeading.Grouping.Name, column: false);
					context.ProcessUserSortInnerScope(matrixHeading.Grouping.Name, isMatrixGroup: true, isMatrixColumnGroup: false);
				}
			}
			for (MatrixHeading matrixHeading = m_rows; matrixHeading != null; matrixHeading = matrixHeading.SubHeading)
			{
				if (matrixHeading.Grouping != null)
				{
					context.ValidateUserSortInnerScope(matrixHeading.Grouping.Name);
				}
			}
			for (MatrixHeading matrixHeading = m_columns; matrixHeading != null; matrixHeading = matrixHeading.SubHeading)
			{
				context.UnRegisterReportItems(matrixHeading.ReportItems);
				if (matrixHeading.Grouping != null)
				{
					context.UnRegisterGroupingScopeForTablixCell(matrixHeading.Grouping.Name, column: true);
					context.ProcessUserSortInnerScope(matrixHeading.Grouping.Name, isMatrixGroup: true, isMatrixColumnGroup: true);
				}
			}
			for (MatrixHeading matrixHeading = m_columns; matrixHeading != null; matrixHeading = matrixHeading.SubHeading)
			{
				if (matrixHeading.Grouping != null)
				{
					context.ValidateUserSortInnerScope(matrixHeading.Grouping.Name);
				}
			}
			m_cellReportItems.RegisterReceiver(context);
			context.UnRegisterReportItems(m_cellReportItems);
			context.UnRegisterTablixCellScope();
		}

		private GroupingList GenerateUserSortGroupingList(bool rowIsInnerGrouping)
		{
			GroupingList groupingList = new GroupingList();
			for (MatrixHeading matrixHeading = rowIsInnerGrouping ? m_rows : m_columns; matrixHeading != null; matrixHeading = matrixHeading.SubHeading)
			{
				if (matrixHeading.Grouping != null)
				{
					groupingList.Add(matrixHeading.Grouping);
				}
			}
			for (MatrixHeading matrixHeading = rowIsInnerGrouping ? m_columns : m_rows; matrixHeading != null; matrixHeading = matrixHeading.SubHeading)
			{
				if (matrixHeading.Grouping != null)
				{
					groupingList.Add(matrixHeading.Grouping);
				}
			}
			return groupingList;
		}

		private void MatrixCellRegisterReceiver(InitializationContext context)
		{
			context.RegisterReportItems(m_cellReportItems);
			m_cellReportItems.RegisterReceiver(context);
			context.UnRegisterReportItems(m_cellReportItems);
		}

		bool IPageBreakItem.IgnorePageBreaks()
		{
			if (m_pagebreakState == PageBreakStates.Unknown)
			{
				if (SharedHiddenState.Never != Visibility.GetSharedHidden(m_visibility))
				{
					m_pagebreakState = PageBreakStates.CanIgnore;
				}
				else if (SharedHiddenState.Never != Visibility.GetSharedHidden(m_rows.Visibility))
				{
					m_pagebreakState = PageBreakStates.CanIgnore;
				}
				else
				{
					m_pagebreakState = PageBreakStates.CannotIgnore;
				}
			}
			if (PageBreakStates.CanIgnore == m_pagebreakState)
			{
				return true;
			}
			return false;
		}

		internal ReportItem GetCellReportItem(int rowIndex, int columnIndex)
		{
			int index = rowIndex * m_matrixColumns.Count + columnIndex;
			return m_cellReportItems[index];
		}

		internal void InitializePageSectionProcessing()
		{
			m_cellPageTextboxes = new ReportProcessing.PageTextboxes();
			m_columnHeaderPageTextboxes = new ReportProcessing.PageTextboxes();
			m_rowHeaderPageTextboxes = new ReportProcessing.PageTextboxes();
		}

		internal new static Declaration GetDeclaration()
		{
			MemberInfoList memberInfoList = new MemberInfoList();
			memberInfoList.Add(new MemberInfo(MemberName.Columns, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.MatrixHeading));
			memberInfoList.Add(new MemberInfo(MemberName.Rows, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.MatrixHeading));
			memberInfoList.Add(new MemberInfo(MemberName.CornerReportItems, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.ReportItemCollection));
			memberInfoList.Add(new MemberInfo(MemberName.CellReportItems, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.ReportItemCollection));
			memberInfoList.Add(new MemberInfo(MemberName.CellIDs, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.IntList));
			memberInfoList.Add(new MemberInfo(MemberName.PropagatedPageBreakAtStart, Token.Boolean));
			memberInfoList.Add(new MemberInfo(MemberName.PropagatedPageBreakAtEnd, Token.Boolean));
			memberInfoList.Add(new MemberInfo(MemberName.InnerRowLevelWithPageBreak, Token.Int32));
			memberInfoList.Add(new MemberInfo(MemberName.MatrixRows, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.MatrixRowList));
			memberInfoList.Add(new MemberInfo(MemberName.MatrixColumns, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.MatrixColumnList));
			memberInfoList.Add(new MemberInfo(MemberName.GroupsBeforeRowHeaders, Token.Int32));
			memberInfoList.Add(new MemberInfo(MemberName.LayoutDirection, Token.Boolean));
			memberInfoList.Add(new MemberInfo(MemberName.StaticColumns, Token.Reference, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.MatrixHeading));
			memberInfoList.Add(new MemberInfo(MemberName.StaticRows, Token.Reference, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.MatrixHeading));
			memberInfoList.Add(new MemberInfo(MemberName.UseOwc, Token.Boolean));
			memberInfoList.Add(new MemberInfo(MemberName.OwcCellNames, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.StringList));
			memberInfoList.Add(new MemberInfo(MemberName.CellDataElementName, Token.String));
			memberInfoList.Add(new MemberInfo(MemberName.ColumnGroupingFixedHeader, Token.Boolean));
			memberInfoList.Add(new MemberInfo(MemberName.RowGroupingFixedHeader, Token.Boolean));
			return new Declaration(Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.Pivot, memberInfoList);
		}
	}
}
