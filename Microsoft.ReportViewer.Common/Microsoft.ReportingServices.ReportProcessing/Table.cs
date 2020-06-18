using Microsoft.ReportingServices.ReportProcessing.ExprHostObjectModel;
using Microsoft.ReportingServices.ReportProcessing.Persistence;
using Microsoft.ReportingServices.ReportProcessing.ReportObjectModel;
using Microsoft.ReportingServices.ReportRendering;
using System;

namespace Microsoft.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class Table : DataRegion, IPageBreakItem, IRunningValueHolder
	{
		private sealed class OWCFlagsCalculator
		{
			private const uint MaxNumberOfTextBoxAndCheckBox = 1u;

			private uint m_numberOfTextBoxAndCheckBox;

			private bool m_useOWC = true;

			private bool m_owcNonSharedStyles;

			private OWCFlagsCalculator()
			{
			}

			internal static void Calculate(Table table, out bool useOWC, out bool owcNonSharedStyles)
			{
				OWCFlagsCalculator oWCFlagsCalculator = new OWCFlagsCalculator();
				oWCFlagsCalculator.CalculateOWCFlags(table);
				useOWC = oWCFlagsCalculator.m_useOWC;
				owcNonSharedStyles = oWCFlagsCalculator.m_owcNonSharedStyles;
			}

			private void CalculateOWCFlags(Table table)
			{
				CalculateOWCFlags(table.HeaderRows);
				if (IsFinish())
				{
					return;
				}
				CalculateOWCFlags(table.TableGroups);
				if (!IsFinish())
				{
					CalculateOWCFlags(table.TableDetail);
					if (!IsFinish())
					{
						CalculateOWCFlags(table.FooterRows);
					}
				}
			}

			private void CalculateOWCFlags(TableGroup tableGroup)
			{
				if (tableGroup == null)
				{
					return;
				}
				CalculateOWCFlags(tableGroup.HeaderRows);
				if (!IsFinish())
				{
					CalculateOWCFlags(tableGroup.SubGroup);
					if (!IsFinish())
					{
						CalculateOWCFlags(tableGroup.FooterRows);
					}
				}
			}

			private void CalculateOWCFlags(TableDetail tableDetail)
			{
				if (tableDetail != null)
				{
					CalculateOWCFlags(tableDetail.DetailRows);
					IsFinish();
				}
			}

			private void CalculateOWCFlags(TableRowList tableRows)
			{
				if (tableRows == null)
				{
					return;
				}
				for (int i = 0; i < tableRows.Count; i++)
				{
					CalculateOWCFlags(tableRows[i]);
					if (IsFinish())
					{
						break;
					}
				}
			}

			private void CalculateOWCFlags(TableRow tableRow)
			{
				if (tableRow == null || tableRow.ReportItems == null)
				{
					return;
				}
				for (int i = 0; i < tableRow.ReportItems.Count; i++)
				{
					m_numberOfTextBoxAndCheckBox = 0u;
					CalculateOWCFlags(tableRow.ReportItems[i]);
					if (IsFinish())
					{
						break;
					}
				}
			}

			private void CalculateOWCFlags(ReportItem item)
			{
				if (item == null)
				{
					return;
				}
				if ((item is TextBox || item is CheckBox || item is Rectangle) && item.Visibility != null)
				{
					m_useOWC = false;
					return;
				}
				if (item is TextBox || item is CheckBox)
				{
					m_numberOfTextBoxAndCheckBox++;
					if (m_numberOfTextBoxAndCheckBox > 1)
					{
						m_useOWC = false;
						return;
					}
					if (item.StyleClass != null && item.StyleClass.ExpressionList != null && 0 < item.StyleClass.ExpressionList.Count)
					{
						m_owcNonSharedStyles = true;
					}
				}
				if (item is TextBox && ((TextBox)item).IsToggle)
				{
					m_useOWC = false;
					return;
				}
				if (item is Rectangle)
				{
					Rectangle rectangle = (Rectangle)item;
					if (rectangle.ReportItems != null)
					{
						for (int i = 0; i < rectangle.ReportItems.Count; i++)
						{
							CalculateOWCFlags(rectangle.ReportItems[i]);
							if (IsFinish())
							{
								return;
							}
						}
					}
				}
				if (item is Image || item is SubReport || item is ActiveXControl || item is DataRegion)
				{
					m_useOWC = false;
				}
			}

			private bool IsFinish()
			{
				return !m_useOWC;
			}
		}

		private sealed class TopLevelItemsSizes
		{
			private TableColumnList m_tableColumns;

			private InitializationContext m_context;

			private TopLevelItemsSizes(TableColumnList tableColumns, InitializationContext context)
			{
				m_tableColumns = tableColumns;
				m_context = context;
			}

			internal static void Calculate(Table table, InitializationContext context)
			{
				new TopLevelItemsSizes(table.TableColumns, context).CalculateSizes(table);
			}

			private void CalculateSizes(Table table)
			{
				CalculateSizes(table.HeaderRows);
				CalculateSizes(table.TableGroups);
				CalculateSizes(table.TableDetail);
				CalculateSizes(table.FooterRows);
			}

			private void CalculateSizes(TableGroup tableGroup)
			{
				if (tableGroup != null)
				{
					CalculateSizes(tableGroup.HeaderRows);
					CalculateSizes(tableGroup.SubGroup);
					CalculateSizes(tableGroup.FooterRows);
				}
			}

			private void CalculateSizes(TableDetail tableDetail)
			{
				if (tableDetail != null)
				{
					CalculateSizes(tableDetail.DetailRows);
				}
			}

			private void CalculateSizes(TableRowList tableRows)
			{
				if (tableRows != null)
				{
					for (int i = 0; i < tableRows.Count; i++)
					{
						CalculateSizes(tableRows[i]);
					}
				}
			}

			private void CalculateSizes(TableRow tableRow)
			{
				if (tableRow == null || tableRow.ReportItems == null)
				{
					return;
				}
				int num = 0;
				double num2 = 0.0;
				int num3 = 0;
				for (int i = 0; i < tableRow.ReportItems.Count; i++)
				{
					num2 = 0.0;
					for (num3 = tableRow.ColSpans[i]; num3 > 0; num3--)
					{
						num2 += m_tableColumns[num].WidthValue;
						num++;
					}
					CalculateSizes(tableRow.ReportItems[i], num2, tableRow.HeightValue);
				}
			}

			private void CalculateSizes(ReportItem item, double width, double height)
			{
				item?.CalculateSizes(width, height, m_context, overwrite: true);
			}
		}

		private TableColumnList m_tableColumns;

		private TableRowList m_headerRows;

		private bool m_headerRepeatOnNewPage;

		private TableGroup m_tableGroups;

		private TableDetail m_tableDetail;

		private TableGroup m_detailGroup;

		private TableRowList m_footerRows;

		private bool m_footerRepeatOnNewPage;

		private bool m_propagatedPageBreakAtStart;

		private bool m_groupPageBreakAtStart;

		private bool m_propagatedPageBreakAtEnd;

		private bool m_groupPageBreakAtEnd;

		private bool m_fillPage;

		private bool m_useOWC;

		private bool m_owcNonSharedStyles;

		private RunningValueInfoList m_runningValues;

		private string m_detailDataElementName;

		private string m_detailDataCollectionName;

		private DataElementOutputTypes m_detailDataElementOutput;

		private bool m_fixedHeader;

		[NonSerialized]
		private TableExprHost m_exprHost;

		[NonSerialized]
		private int m_currentPage = -1;

		[NonSerialized]
		private bool m_hasFixedColumnHeaders;

		[NonSerialized]
		private bool[] m_columnsStartHidden;

		internal override ObjectType ObjectType => ObjectType.Table;

		internal TableColumnList TableColumns
		{
			get
			{
				return m_tableColumns;
			}
			set
			{
				m_tableColumns = value;
			}
		}

		internal TableRowList HeaderRows
		{
			get
			{
				return m_headerRows;
			}
			set
			{
				m_headerRows = value;
			}
		}

		internal bool HeaderRepeatOnNewPage
		{
			get
			{
				return m_headerRepeatOnNewPage;
			}
			set
			{
				m_headerRepeatOnNewPage = value;
			}
		}

		internal TableGroup TableGroups
		{
			get
			{
				return m_tableGroups;
			}
			set
			{
				m_tableGroups = value;
			}
		}

		internal TableDetail TableDetail
		{
			get
			{
				return m_tableDetail;
			}
			set
			{
				m_tableDetail = value;
			}
		}

		internal TableGroup DetailGroup
		{
			get
			{
				return m_detailGroup;
			}
			set
			{
				m_detailGroup = value;
			}
		}

		internal TableRowList FooterRows
		{
			get
			{
				return m_footerRows;
			}
			set
			{
				m_footerRows = value;
			}
		}

		internal bool FooterRepeatOnNewPage
		{
			get
			{
				return m_footerRepeatOnNewPage;
			}
			set
			{
				m_footerRepeatOnNewPage = value;
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

		internal bool GroupBreakAtStart
		{
			get
			{
				return m_groupPageBreakAtStart;
			}
			set
			{
				m_groupPageBreakAtStart = value;
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

		internal bool GroupBreakAtEnd
		{
			get
			{
				return m_groupPageBreakAtEnd;
			}
			set
			{
				m_groupPageBreakAtEnd = value;
			}
		}

		internal bool FillPage
		{
			get
			{
				return m_fillPage;
			}
			set
			{
				m_fillPage = value;
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

		internal bool OWCNonSharedStyles
		{
			get
			{
				return m_owcNonSharedStyles;
			}
			set
			{
				m_owcNonSharedStyles = value;
			}
		}

		internal RunningValueInfoList RunningValues
		{
			get
			{
				return m_runningValues;
			}
			set
			{
				m_runningValues = value;
			}
		}

		internal string DetailDataElementName
		{
			get
			{
				return m_detailDataElementName;
			}
			set
			{
				m_detailDataElementName = value;
			}
		}

		internal string DetailDataCollectionName
		{
			get
			{
				return m_detailDataCollectionName;
			}
			set
			{
				m_detailDataCollectionName = value;
			}
		}

		internal DataElementOutputTypes DetailDataElementOutput
		{
			get
			{
				return m_detailDataElementOutput;
			}
			set
			{
				m_detailDataElementOutput = value;
			}
		}

		internal TableExprHost TableExprHost => m_exprHost;

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

		protected override DataRegionExprHost DataRegionExprHost => m_exprHost;

		internal bool FixedHeader
		{
			get
			{
				return m_fixedHeader;
			}
			set
			{
				m_fixedHeader = value;
			}
		}

		internal bool HasFixedColumnHeaders
		{
			get
			{
				return m_hasFixedColumnHeaders;
			}
			set
			{
				m_hasFixedColumnHeaders = value;
			}
		}

		internal double HeaderHeightValue
		{
			get
			{
				if (m_headerRows != null)
				{
					return m_headerRows.GetHeightValue();
				}
				return 0.0;
			}
		}

		internal double DetailHeightValue
		{
			get
			{
				if (m_tableDetail != null && m_tableDetail.DetailRows != null)
				{
					return m_tableDetail.DetailRows.GetHeightValue();
				}
				return 0.0;
			}
		}

		internal bool[] ColumnsStartHidden
		{
			get
			{
				return m_columnsStartHidden;
			}
			set
			{
				m_columnsStartHidden = value;
			}
		}

		internal Table(ReportItem parent)
			: base(parent)
		{
		}

		internal Table(int id, ReportItem parent)
			: base(id, parent)
		{
			m_runningValues = new RunningValueInfoList();
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
			context.ExprHostBuilder.TableStart(m_name);
			base.Initialize(context);
			context.RegisterRunningValues(m_runningValues);
			if (m_visibility != null)
			{
				m_visibility.Initialize(context, isContainer: true, tableRowCol: false);
			}
			InitializeTableColumns(context, ref m_widthValue, out bool[] tableColumnVisibility);
			m_width = Converter.ConvertSize(m_widthValue);
			InitializeHeaderAndFooter(m_tableColumns.Count, context, ref m_heightValue, tableColumnVisibility);
			context.Location &= ~LocationFlags.InMatrixCellTopLevelItem;
			InitializeTableGroupsOrDetail(m_tableColumns.Count, context, ref m_heightValue, tableColumnVisibility);
			m_height = Converter.ConvertSize(m_heightValue);
			if (m_visibility != null)
			{
				m_visibility.UnRegisterReceiver(context);
			}
			context.UnRegisterRunningValues(m_runningValues);
			OWCFlagsCalculator.Calculate(this, out m_useOWC, out m_owcNonSharedStyles);
			if (!context.ErrorContext.HasError)
			{
				TopLevelItemsSizes.Calculate(this, context);
			}
			base.ExprHostID = context.ExprHostBuilder.TableEnd();
		}

		protected override void DataRendererInitialize(InitializationContext context)
		{
			base.DataRendererInitialize(context);
			CLSNameValidator.ValidateDataElementName(ref m_detailDataElementName, "Detail", context.ObjectType, context.ObjectName, "DetailDataElementName", context.ErrorContext);
			CLSNameValidator.ValidateDataElementName(ref m_detailDataCollectionName, m_detailDataElementName + "_Collection", context.ObjectType, context.ObjectName, "DetailDataCollectionName", context.ErrorContext);
		}

		internal override void RegisterReceiver(InitializationContext context)
		{
			RegisterHeaderAndFooter(context);
			if (m_visibility != null)
			{
				m_visibility.RegisterReceiver(context, isContainer: true);
			}
			RegisterTableColumnsReceiver(context);
			RegisterHeaderAndFooterReceiver(context);
			RegisterTableGroupsOrDetailReceiver(context);
			if (m_visibility != null)
			{
				m_visibility.UnRegisterReceiver(context);
			}
			UnRegisterHeaderAndFooter(context);
		}

		internal void RegisterHeaderAndFooter(InitializationContext context)
		{
			if (m_headerRows != null)
			{
				m_headerRows.Register(context);
			}
			if (m_footerRows != null)
			{
				m_footerRows.Register(context);
			}
		}

		internal void UnRegisterHeaderAndFooter(InitializationContext context)
		{
			if (m_footerRows != null)
			{
				m_footerRows.UnRegister(context);
			}
			if (m_headerRows != null)
			{
				m_headerRows.UnRegister(context);
			}
		}

		private void InitializeTableColumns(InitializationContext context, ref double tableWidth, out bool[] tableColumnVisibility)
		{
			context.ExprHostBuilder.TableColumnVisibilityHiddenExpressionsStart();
			tableColumnVisibility = new bool[m_tableColumns.Count];
			for (int i = 0; i < m_tableColumns.Count; i++)
			{
				m_tableColumns[i].Initialize(context);
				tableWidth += m_tableColumns[i].WidthValue;
				tableColumnVisibility[i] = (m_tableColumns[i].Visibility == null || m_tableColumns[i].Visibility.Hidden == null || m_tableColumns[i].Visibility.Toggle != null || (ExpressionInfo.Types.Constant == m_tableColumns[i].Visibility.Hidden.Type && !m_tableColumns[i].Visibility.Hidden.BoolValue));
			}
			context.ExprHostBuilder.TableColumnVisibilityHiddenExpressionsEnd();
		}

		private void RegisterTableColumnsReceiver(InitializationContext context)
		{
			for (int i = 0; i < m_tableColumns.Count; i++)
			{
				m_tableColumns[i].RegisterReceiver(context);
			}
		}

		private void InitializeHeaderAndFooter(int numberOfColumns, InitializationContext context, ref double tableHeight, bool[] tableColumnVisibility)
		{
			context.ExprHostBuilder.TableRowVisibilityHiddenExpressionsStart();
			if (m_headerRows != null)
			{
				for (int i = 0; i < m_headerRows.Count; i++)
				{
					Global.Tracer.Assert(m_headerRows[i] != null);
					m_headerRows[i].Initialize(registerRunningValues: false, numberOfColumns, context, ref tableHeight, tableColumnVisibility);
				}
			}
			if (m_footerRows != null)
			{
				for (int j = 0; j < m_footerRows.Count; j++)
				{
					Global.Tracer.Assert(m_footerRows[j] != null);
					m_footerRows[j].Initialize(registerRunningValues: false, numberOfColumns, context, ref tableHeight, tableColumnVisibility);
				}
			}
			context.ExprHostBuilder.TableRowVisibilityHiddenExpressionsEnd();
		}

		private void RegisterHeaderAndFooterReceiver(InitializationContext context)
		{
			if (m_headerRows != null)
			{
				for (int i = 0; i < m_headerRows.Count; i++)
				{
					Global.Tracer.Assert(m_headerRows[i] != null);
					m_headerRows[i].RegisterReceiver(context);
				}
			}
			if (m_footerRows != null)
			{
				for (int j = 0; j < m_footerRows.Count; j++)
				{
					Global.Tracer.Assert(m_footerRows[j] != null);
					m_footerRows[j].RegisterReceiver(context);
				}
			}
		}

		private void InitializeTableGroupsOrDetail(int numberOfColumns, InitializationContext context, ref double tableHeight, bool[] tableColumnVisibility)
		{
			TableGroup tableGroup = m_detailGroup;
			if (tableGroup != null && m_tableGroups == null)
			{
				m_tableGroups = m_detailGroup;
				tableGroup = null;
			}
			if (m_tableGroups != null)
			{
				m_tableGroups.Initialize(numberOfColumns, m_tableDetail, tableGroup, context, ref tableHeight, tableColumnVisibility);
			}
			else if (m_tableDetail != null)
			{
				m_tableDetail.Initialize(numberOfColumns, context, ref tableHeight, tableColumnVisibility);
			}
		}

		private void RegisterTableGroupsOrDetailReceiver(InitializationContext context)
		{
			if (m_tableGroups != null)
			{
				m_tableGroups.RegisterReceiver(context, m_tableDetail);
			}
			else if (m_tableDetail != null)
			{
				m_tableDetail.RegisterReceiver(context);
			}
		}

		RunningValueInfoList IRunningValueHolder.GetRunningValueList()
		{
			return m_runningValues;
		}

		void IRunningValueHolder.ClearIfEmpty()
		{
			Global.Tracer.Assert(m_runningValues != null);
			if (m_runningValues.Count == 0)
			{
				m_runningValues = null;
			}
		}

		internal void CalculatePropagatedFlags()
		{
			bool flag = true;
			if (m_tableGroups != null)
			{
				m_tableGroups.CalculatePropagatedFlags(out m_groupPageBreakAtStart, out m_groupPageBreakAtEnd);
				if (m_tableGroups.HeaderRows != null)
				{
					flag = m_tableGroups.HeaderRepeatOnNewPage;
				}
				m_propagatedPageBreakAtStart = (m_tableGroups.Grouping.PageBreakAtStart || (m_tableGroups.PropagatedPageBreakAtStart && flag));
				flag = true;
				if (m_tableGroups.FooterRows != null)
				{
					flag = m_tableGroups.FooterRepeatOnNewPage;
				}
				m_propagatedPageBreakAtEnd = (m_tableGroups.Grouping.PageBreakAtEnd || (m_tableGroups.PropagatedPageBreakAtEnd && flag));
			}
		}

		bool IPageBreakItem.IgnorePageBreaks()
		{
			if (m_pagebreakState == PageBreakStates.Unknown)
			{
				m_pagebreakState = PageBreakStates.CanIgnore;
				if (SharedHiddenState.Never == Visibility.GetSharedHidden(m_visibility))
				{
					if (m_tableColumns != null)
					{
						int i;
						for (i = 0; i < m_tableColumns.Count && SharedHiddenState.Never != Visibility.GetSharedHidden(m_tableColumns[i].Visibility); i++)
						{
						}
						if (i < m_tableColumns.Count)
						{
							m_pagebreakState = PageBreakStates.CannotIgnore;
						}
					}
					if (PageBreakStates.CannotIgnore == m_pagebreakState)
					{
						if (m_tableGroups == null)
						{
							if (m_tableDetail != null)
							{
								if (SharedHiddenState.Never != Visibility.GetSharedHidden(m_tableDetail.Visibility))
								{
									m_pagebreakState = PageBreakStates.CanIgnore;
								}
								for (int j = 0; j < m_tableDetail.DetailRows.Count; j++)
								{
									if (SharedHiddenState.Never != Visibility.GetSharedHidden(m_tableDetail.DetailRows[j].Visibility))
									{
										m_pagebreakState = PageBreakStates.CanIgnore;
									}
								}
							}
						}
						else if (SharedHiddenState.Never != Visibility.GetSharedHidden(m_tableGroups.Visibility))
						{
							m_pagebreakState = PageBreakStates.CanIgnore;
						}
					}
				}
			}
			if (PageBreakStates.CanIgnore == m_pagebreakState)
			{
				return true;
			}
			return false;
		}

		internal override void SetExprHost(ReportExprHost reportExprHost, ObjectModelImpl reportObjectModel)
		{
			if (base.ExprHostID >= 0)
			{
				Global.Tracer.Assert(reportExprHost != null && reportObjectModel != null);
				m_exprHost = reportExprHost.TableHostsRemotable[base.ExprHostID];
				DataRegionSetExprHost(m_exprHost, reportObjectModel);
				if (m_exprHost.TableColumnVisibilityHiddenExpressions != null)
				{
					m_exprHost.TableColumnVisibilityHiddenExpressions.SetReportObjectModel(reportObjectModel);
				}
				if (m_exprHost.TableRowVisibilityHiddenExpressions != null)
				{
					m_exprHost.TableRowVisibilityHiddenExpressions.SetReportObjectModel(reportObjectModel);
				}
			}
		}

		internal new static Declaration GetDeclaration()
		{
			MemberInfoList memberInfoList = new MemberInfoList();
			memberInfoList.Add(new MemberInfo(MemberName.TableColumns, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.TableColumnList));
			memberInfoList.Add(new MemberInfo(MemberName.HeaderRows, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.TableRowList));
			memberInfoList.Add(new MemberInfo(MemberName.HeaderRepeatOnNewPage, Token.Boolean));
			memberInfoList.Add(new MemberInfo(MemberName.TableGroups, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.TableGroup));
			memberInfoList.Add(new MemberInfo(MemberName.TableDetail, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.TableDetail));
			memberInfoList.Add(new MemberInfo(MemberName.DetailGroup, Token.Reference, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.TableGroup));
			memberInfoList.Add(new MemberInfo(MemberName.FooterRows, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.TableRowList));
			memberInfoList.Add(new MemberInfo(MemberName.FooterRepeatOnNewPage, Token.Boolean));
			memberInfoList.Add(new MemberInfo(MemberName.PropagatedPageBreakAtStart, Token.Boolean));
			memberInfoList.Add(new MemberInfo(MemberName.GroupPageBreakAtStart, Token.Boolean));
			memberInfoList.Add(new MemberInfo(MemberName.PropagatedPageBreakAtEnd, Token.Boolean));
			memberInfoList.Add(new MemberInfo(MemberName.GroupPageBreakAtEnd, Token.Boolean));
			memberInfoList.Add(new MemberInfo(MemberName.FillPage, Token.Boolean));
			memberInfoList.Add(new MemberInfo(MemberName.UseOwc, Token.Boolean));
			memberInfoList.Add(new MemberInfo(MemberName.OwcNonSharedStyles, Token.Boolean));
			memberInfoList.Add(new MemberInfo(MemberName.RunningValues, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.RunningValueInfoList));
			memberInfoList.Add(new MemberInfo(MemberName.DetailDataElementName, Token.String));
			memberInfoList.Add(new MemberInfo(MemberName.DetailDataCollectionName, Token.String));
			memberInfoList.Add(new MemberInfo(MemberName.DetailDataElementOutput, Token.Enum));
			memberInfoList.Add(new MemberInfo(MemberName.FixedHeader, Token.Boolean));
			return new Declaration(Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.DataRegion, memberInfoList);
		}
	}
}
