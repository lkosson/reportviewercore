using Microsoft.ReportingServices.OnDemandProcessing;
using Microsoft.ReportingServices.OnDemandReportRendering;
using Microsoft.ReportingServices.RdlExpressions;
using Microsoft.ReportingServices.RdlExpressions.ExpressionHostObjectModel;
using Microsoft.ReportingServices.ReportIntermediateFormat.Persistence;
using Microsoft.ReportingServices.ReportProcessing;
using Microsoft.ReportingServices.ReportProcessing.OnDemandReportObjectModel;
using Microsoft.ReportingServices.ReportPublishing;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;

namespace Microsoft.ReportingServices.ReportIntermediateFormat
{
	internal sealed class Tablix : DataRegion, ICreateSubtotals, IPersistable
	{
		internal enum MarginPosition
		{
			TopMargin,
			BottomMargin,
			LeftMargin,
			RightMargin
		}

		internal class InitData
		{
			internal bool HasFixedColData;

			internal int FixedColStartIndex;

			internal int FixedColLength;

			internal bool HasFixedRowData;

			internal bool IsTopLevelDataRegion;

			internal IList<Pair<double, int>> RowHeaderLevelSizeList;

			internal IList<Pair<double, int>> ColumnHeaderLevelSizeList;
		}

		private class SizeCalculator
		{
			private Tablix m_tablix;

			internal SizeCalculator(Tablix tablix)
			{
				m_tablix = tablix;
			}

			internal void CalculateSizes(InitializationContext context)
			{
				if (m_tablix.Corner != null)
				{
					CalculateCornerSizes(context);
				}
				if (m_tablix.TablixRowMembers != null)
				{
					CalculateMemberSizes(context, m_tablix.TablixRowMembers, isColumn: false, 0);
				}
				if (m_tablix.TablixColumnMembers != null)
				{
					CalculateMemberSizes(context, m_tablix.TablixColumnMembers, isColumn: true, 0);
				}
				if (m_tablix.TablixRows != null)
				{
					CalculateCellSizes(context);
				}
			}

			private void CalculateCellSizes(InitializationContext context)
			{
				for (int i = 0; i < m_tablix.TablixRows.Count; i++)
				{
					TablixCellList tablixCells = m_tablix.TablixRows[i].TablixCells;
					for (int j = 0; j < tablixCells.Count; j++)
					{
						TablixCell tablixCell = tablixCells[j];
						if (tablixCell.CellContents != null)
						{
							double num = 0.0;
							double heightValue = m_tablix.TablixRows[i].HeightValue;
							for (int k = j; k < tablixCell.ColSpan + j; k++)
							{
								num += m_tablix.TablixColumns[k].WidthValue;
							}
							tablixCell.CellContents.CalculateSizes(num, heightValue, context, overwrite: true);
							if (tablixCell.AltCellContents != null)
							{
								tablixCell.AltCellContents.CalculateSizes(num, heightValue, context, overwrite: true);
							}
						}
					}
				}
			}

			private void CalculateMemberSizes(InitializationContext context, TablixMemberList members, bool isColumn, int index)
			{
				int num = index;
				for (int i = 0; i < members.Count; i++)
				{
					double num2 = 0.0;
					double num3 = 0.0;
					double num4 = 0.0;
					double num5 = 0.0;
					TablixMember tablixMember = members[i];
					if (tablixMember.TablixHeader != null && tablixMember.TablixHeader.CellContents != null)
					{
						if (isColumn)
						{
							num2 = tablixMember.TablixHeader.SizeValue;
							num5 = num2;
							for (int j = num; j < tablixMember.ColSpan + num; j++)
							{
								TablixColumn tablixColumn = m_tablix.TablixColumns[j];
								if (!tablixColumn.ForAutoSubtotal)
								{
									num4 += tablixColumn.WidthValue;
								}
								num3 += tablixColumn.WidthValue;
							}
						}
						else
						{
							num3 = tablixMember.TablixHeader.SizeValue;
							num4 += num3;
							for (int k = num; k < tablixMember.RowSpan + num; k++)
							{
								TablixRow tablixRow = m_tablix.TablixRows[k];
								if (!tablixRow.ForAutoSubtotal)
								{
									num5 += tablixRow.HeightValue;
								}
								num2 += tablixRow.HeightValue;
							}
						}
						Microsoft.ReportingServices.ReportProcessing.ObjectType objectType = tablixMember.TablixHeader.CellContents.ObjectType;
						if (objectType == Microsoft.ReportingServices.ReportProcessing.ObjectType.Chart || objectType == Microsoft.ReportingServices.ReportProcessing.ObjectType.GaugePanel || objectType == Microsoft.ReportingServices.ReportProcessing.ObjectType.Map)
						{
							tablixMember.TablixHeader.CellContents.CalculateSizes(num4, num5, context, overwrite: true);
						}
						else
						{
							tablixMember.TablixHeader.CellContents.CalculateSizes(num3, num2, context, overwrite: true);
						}
						if (tablixMember.TablixHeader.AltCellContents != null)
						{
							objectType = tablixMember.TablixHeader.AltCellContents.ObjectType;
							if (objectType == Microsoft.ReportingServices.ReportProcessing.ObjectType.Chart || objectType == Microsoft.ReportingServices.ReportProcessing.ObjectType.GaugePanel || objectType == Microsoft.ReportingServices.ReportProcessing.ObjectType.Map)
							{
								tablixMember.TablixHeader.AltCellContents.CalculateSizes(num4, num5, context, overwrite: true);
							}
							else
							{
								tablixMember.TablixHeader.AltCellContents.CalculateSizes(num3, num2, context, overwrite: true);
							}
						}
					}
					if (tablixMember.SubMembers != null)
					{
						CalculateMemberSizes(context, tablixMember.SubMembers, isColumn, num);
					}
					num = ((!isColumn) ? (num + tablixMember.RowSpan) : (num + tablixMember.ColSpan));
				}
			}

			private void CalculateCornerSizes(InitializationContext context)
			{
				double num = 0.0;
				double num2 = 0.0;
				for (int i = 0; i < m_tablix.Corner.Count; i++)
				{
					List<TablixCornerCell> list = m_tablix.Corner[i];
					for (int j = 0; j < list.Count; j++)
					{
						TablixCornerCell tablixCornerCell = list[j];
						if (tablixCornerCell.CellContents != null)
						{
							num = context.GetHeaderSize(m_tablix.InitializationData.ColumnHeaderLevelSizeList, i, tablixCornerCell.RowSpan);
							num2 = context.GetHeaderSize(m_tablix.InitializationData.RowHeaderLevelSizeList, j, tablixCornerCell.ColSpan);
							tablixCornerCell.CellContents.CalculateSizes(num2, num, context, overwrite: true);
							if (tablixCornerCell.AltCellContents != null)
							{
								tablixCornerCell.AltCellContents.CalculateSizes(num2, num, context, overwrite: true);
							}
						}
					}
				}
			}
		}

		private sealed class IndexInCollectionUpgrader
		{
			private Dictionary<Hashtable, int> m_indexInCollectionTable = new Dictionary<Hashtable, int>(InitializationContext.HashtableKeyComparer.Instance);

			private Hashtable m_groupingScopes = new Hashtable();

			internal void RegisterGroup(string groupName)
			{
				m_groupingScopes.Add(groupName, null);
			}

			internal void UnregisterGroup(string groupName)
			{
				m_groupingScopes.Remove(groupName);
			}

			internal void SetIndexInCollection(TablixCell indexedInCollection)
			{
				Hashtable key = (Hashtable)m_groupingScopes.Clone();
				if (m_indexInCollectionTable.TryGetValue(key, out int value))
				{
					value++;
					m_indexInCollectionTable[key] = value;
				}
				else
				{
					value = 0;
					m_indexInCollectionTable.Add(key, value);
				}
				indexedInCollection.IndexInCollection = value;
			}
		}

		private bool m_canScroll;

		private bool m_keepTogether;

		private TablixMemberList m_tablixColumnMembers;

		private TablixMemberList m_tablixRowMembers;

		private TablixRowList m_tablixRows;

		private List<TablixColumn> m_tablixColumns;

		private List<List<TablixCornerCell>> m_corner;

		private PageBreakLocation m_propagatedPageBreakLocation;

		private int m_innerRowLevelWithPageBreak = -1;

		private int m_groupsBeforeRowHeaders;

		private bool m_layoutDirection;

		private bool m_repeatColumnHeaders;

		private bool m_repeatRowHeaders;

		private bool m_fixedColumnHeaders;

		private bool m_fixedRowHeaders;

		private bool m_omitBorderOnPageBreak;

		private bool m_hideStaticsIfNoRows = true;

		[Reference]
		private List<TextBox> m_inScopeTextBoxes;

		private int m_columnHeaderRowCount;

		private int m_rowHeaderColumnCount;

		private BandLayoutOptions m_bandLayout;

		private ExpressionInfo m_topMargin;

		private ExpressionInfo m_bottomMargin;

		private ExpressionInfo m_leftMargin;

		private ExpressionInfo m_rightMargin;

		private bool m_enableRowDrilldown;

		private bool m_enableColumnDrilldown;

		[NonSerialized]
		private InitData m_initData;

		[NonSerialized]
		private bool m_createdSubtotals;

		[NonSerialized]
		private static readonly Declaration m_Declaration = GetDeclaration();

		[NonSerialized]
		private bool m_computeHeight;

		[NonSerialized]
		private bool m_computeWidth;

		[NonSerialized]
		private TablixExprHost m_tablixExprHost;

		internal bool CanScroll
		{
			get
			{
				return m_canScroll;
			}
			set
			{
				m_canScroll = value;
			}
		}

		internal bool ComputeHeight
		{
			get
			{
				return m_computeHeight;
			}
			set
			{
				m_computeHeight = value;
			}
		}

		internal bool ComputeWidth
		{
			get
			{
				return m_computeWidth;
			}
			set
			{
				m_computeWidth = value;
			}
		}

		internal bool KeepTogether
		{
			get
			{
				return m_keepTogether;
			}
			set
			{
				m_keepTogether = value;
			}
		}

		internal override Microsoft.ReportingServices.ReportProcessing.ObjectType ObjectType => Microsoft.ReportingServices.ReportProcessing.ObjectType.Tablix;

		internal override HierarchyNodeList ColumnMembers => m_tablixColumnMembers;

		internal override HierarchyNodeList RowMembers => m_tablixRowMembers;

		internal override RowList Rows => m_tablixRows;

		internal TablixMemberList TablixColumnMembers
		{
			get
			{
				return m_tablixColumnMembers;
			}
			set
			{
				m_tablixColumnMembers = value;
			}
		}

		internal TablixMemberList TablixRowMembers
		{
			get
			{
				return m_tablixRowMembers;
			}
			set
			{
				m_tablixRowMembers = value;
			}
		}

		internal TablixRowList TablixRows
		{
			get
			{
				return m_tablixRows;
			}
			set
			{
				m_tablixRows = value;
			}
		}

		internal List<TablixColumn> TablixColumns
		{
			get
			{
				return m_tablixColumns;
			}
			set
			{
				m_tablixColumns = value;
			}
		}

		internal List<List<TablixCornerCell>> Corner
		{
			get
			{
				return m_corner;
			}
			set
			{
				m_corner = value;
			}
		}

		internal PageBreakLocation PropagatedPageBreakLocation
		{
			get
			{
				return m_propagatedPageBreakLocation;
			}
			set
			{
				m_propagatedPageBreakLocation = value;
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

		public bool RepeatColumnHeaders
		{
			get
			{
				return m_repeatColumnHeaders;
			}
			set
			{
				m_repeatColumnHeaders = value;
			}
		}

		public bool RepeatRowHeaders
		{
			get
			{
				return m_repeatRowHeaders;
			}
			set
			{
				m_repeatRowHeaders = value;
			}
		}

		internal bool FixedColumnHeaders
		{
			get
			{
				return m_fixedColumnHeaders;
			}
			set
			{
				m_fixedColumnHeaders = value;
			}
		}

		internal bool FixedRowHeaders
		{
			get
			{
				return m_fixedRowHeaders;
			}
			set
			{
				m_fixedRowHeaders = value;
			}
		}

		internal int ColumnHeaderRowCount
		{
			get
			{
				return m_columnHeaderRowCount;
			}
			set
			{
				m_columnHeaderRowCount = value;
			}
		}

		internal int RowHeaderColumnCount
		{
			get
			{
				return m_rowHeaderColumnCount;
			}
			set
			{
				m_rowHeaderColumnCount = value;
			}
		}

		internal bool OmitBorderOnPageBreak
		{
			get
			{
				return m_omitBorderOnPageBreak;
			}
			set
			{
				m_omitBorderOnPageBreak = value;
			}
		}

		internal bool HideStaticsIfNoRows
		{
			get
			{
				return m_hideStaticsIfNoRows;
			}
			set
			{
				m_hideStaticsIfNoRows = value;
			}
		}

		internal TablixExprHost TablixExprHost => m_tablixExprHost;

		internal BandLayoutOptions BandLayout
		{
			get
			{
				return m_bandLayout;
			}
			set
			{
				m_bandLayout = value;
			}
		}

		internal ExpressionInfo TopMargin
		{
			get
			{
				return m_topMargin;
			}
			set
			{
				m_topMargin = value;
			}
		}

		internal ExpressionInfo BottomMargin
		{
			get
			{
				return m_bottomMargin;
			}
			set
			{
				m_bottomMargin = value;
			}
		}

		internal ExpressionInfo LeftMargin
		{
			get
			{
				return m_leftMargin;
			}
			set
			{
				m_leftMargin = value;
			}
		}

		internal ExpressionInfo RightMargin
		{
			get
			{
				return m_rightMargin;
			}
			set
			{
				m_rightMargin = value;
			}
		}

		internal bool EnableRowDrilldown
		{
			get
			{
				return m_enableRowDrilldown;
			}
			set
			{
				m_enableRowDrilldown = value;
			}
		}

		internal bool EnableColumnDrilldown
		{
			get
			{
				return m_enableColumnDrilldown;
			}
			set
			{
				m_enableColumnDrilldown = value;
			}
		}

		protected override IndexedExprHost UserSortExpressionsHost
		{
			get
			{
				if (m_tablixExprHost == null)
				{
					return null;
				}
				return m_tablixExprHost.UserSortExpressionsHost;
			}
		}

		internal InitData InitializationData
		{
			get
			{
				if (m_initData == null)
				{
					m_initData = new InitData();
				}
				return m_initData;
			}
		}

		internal List<TextBox> InScopeTextBoxes => m_inScopeTextBoxes;

		internal Tablix(ReportItem parent)
			: base(parent)
		{
		}

		internal Tablix(int id, ReportItem parent)
			: base(id, parent)
		{
		}

		protected override List<ReportItem> ComputeDataRegionScopedItemsForDataProcessing()
		{
			List<ReportItem> results = base.ComputeDataRegionScopedItemsForDataProcessing();
			if (Corner != null)
			{
				for (int i = 0; i < Corner.Count; i++)
				{
					List<TablixCornerCell> list = Corner[i];
					if (list != null && list.Count != 0)
					{
						for (int j = 0; j < list.Count; j++)
						{
							DataRegion.MergeDataProcessingItems(list[j], ref results);
						}
					}
				}
			}
			return results;
		}

		protected override void TraverseDataRegionLevelScopes(IRIFScopeVisitor visitor)
		{
			if (m_corner == null)
			{
				return;
			}
			for (int i = 0; i < m_corner.Count; i++)
			{
				List<TablixCornerCell> list = m_corner[i];
				if (list != null)
				{
					for (int j = 0; j < list.Count; j++)
					{
						TraverseScopes(visitor, list[j], i, j);
					}
				}
			}
		}

		internal override bool Initialize(InitializationContext context)
		{
			context.Location |= Microsoft.ReportingServices.ReportPublishing.LocationFlags.InTablix;
			context.ObjectType = ObjectType;
			context.ObjectName = m_name;
			context.ColumnHeaderLevelSizeList = m_initData.ColumnHeaderLevelSizeList;
			context.RowHeaderLevelSizeList = m_initData.RowHeaderLevelSizeList;
			if (!context.RegisterDataRegion(this))
			{
				return false;
			}
			context.Location |= (Microsoft.ReportingServices.ReportPublishing.LocationFlags.InDataSet | Microsoft.ReportingServices.ReportPublishing.LocationFlags.InDataRegion);
			bool num = context.RegisterVisibility(m_visibility, this);
			context.ExprHostBuilder.DataRegionStart(Microsoft.ReportingServices.RdlExpressions.ExprHostBuilder.DataRegionMode.Tablix, m_name);
			base.Initialize(context);
			if (!context.ErrorContext.HasError)
			{
				new SizeCalculator(this).CalculateSizes(context);
			}
			InitializeBand(context);
			base.ExprHostID = context.ExprHostBuilder.DataRegionEnd(Microsoft.ReportingServices.RdlExpressions.ExprHostBuilder.DataRegionMode.Tablix);
			if (num)
			{
				context.UnRegisterVisibility(m_visibility, this);
			}
			context.UnRegisterDataRegion(this);
			return false;
		}

		private void InitializeBand(InitializationContext context)
		{
			ValidateMarginAndCreateExpression(m_topMargin, MarginPosition.TopMargin, context);
			ValidateMarginAndCreateExpression(m_bottomMargin, MarginPosition.BottomMargin, context);
			ValidateMarginAndCreateExpression(m_leftMargin, MarginPosition.LeftMargin, context);
			ValidateMarginAndCreateExpression(m_rightMargin, MarginPosition.RightMargin, context);
			if (m_bandLayout != null)
			{
				m_bandLayout.Initialize(this, context);
			}
		}

		private void ValidateMarginAndCreateExpression(ExpressionInfo marginExpression, MarginPosition position, InitializationContext context)
		{
			if (marginExpression != null)
			{
				string text = position.ToString();
				if (!marginExpression.IsExpression)
				{
					context.ValidateSize(marginExpression.OriginalText, text);
				}
				marginExpression.Initialize(text, context);
				context.ExprHostBuilder.MarginExpression(marginExpression, text);
			}
		}

		protected override void InitializeRVDirectionDependentItemsInCorner(InitializationContext context)
		{
			if (m_corner == null)
			{
				return;
			}
			foreach (List<TablixCornerCell> item in m_corner)
			{
				foreach (TablixCornerCell item2 in item)
				{
					if (item2.ColSpan > 0 && item2.RowSpan > 0)
					{
						if (item2.CellContents != null)
						{
							item2.CellContents.InitializeRVDirectionDependentItems(context);
						}
						if (item2.AltCellContents != null)
						{
							item2.AltCellContents.InitializeRVDirectionDependentItems(context);
						}
					}
				}
			}
		}

		protected override void InitializeRVDirectionDependentItems(int outerIndex, int innerIndex, InitializationContext context)
		{
			int index;
			int index2;
			if (m_processingInnerGrouping == ProcessingInnerGroupings.Row)
			{
				index = outerIndex;
				index2 = innerIndex;
			}
			else
			{
				index = innerIndex;
				index2 = outerIndex;
			}
			if (m_tablixRows == null)
			{
				return;
			}
			TablixCell tablixCell = m_tablixRows[index2].TablixCells[index];
			if (tablixCell != null)
			{
				tablixCell.InitializeRVDirectionDependentItems(context);
				if (context.HasUserSorts && !context.IsDataRegionScopedCell)
				{
					CopyCellAggregates(tablixCell);
				}
			}
		}

		protected override void DetermineGroupingExprValueCountInCorner(InitializationContext context, int groupingExprCount)
		{
			if (m_corner == null)
			{
				return;
			}
			foreach (List<TablixCornerCell> item in m_corner)
			{
				foreach (TablixCornerCell item2 in item)
				{
					if (item2.ColSpan > 0 && item2.RowSpan > 0)
					{
						if (item2.CellContents != null)
						{
							item2.CellContents.DetermineGroupingExprValueCount(context, groupingExprCount);
						}
						if (item2.AltCellContents != null)
						{
							item2.AltCellContents.DetermineGroupingExprValueCount(context, groupingExprCount);
						}
					}
				}
			}
		}

		protected override void DetermineGroupingExprValueCount(int outerIndex, int innerIndex, InitializationContext context, int groupingExprCount)
		{
			int index;
			int index2;
			if (m_processingInnerGrouping == ProcessingInnerGroupings.Row)
			{
				index = outerIndex;
				index2 = innerIndex;
			}
			else
			{
				index = innerIndex;
				index2 = outerIndex;
			}
			if (m_tablixRows != null)
			{
				m_tablixRows[index2].TablixCells[index]?.DetermineGroupingExprValueCount(context, groupingExprCount);
			}
		}

		internal static void ValidateKeepWithGroup(TablixMemberList members, InitializationContext context)
		{
			if (members == null || !HasDynamic(members))
			{
				return;
			}
			int num = -1;
			int num2 = -1;
			bool flag = false;
			bool? flag2 = null;
			for (int i = 0; i < members.Count; i++)
			{
				if (members[i].Grouping != null)
				{
					num = i;
					flag = false;
					num2 = -1;
					flag2 = null;
					continue;
				}
				if (!flag2.HasValue)
				{
					flag2 = members[i].RepeatOnNewPage;
				}
				else if (flag2 != members[i].RepeatOnNewPage)
				{
					context.ErrorContext.Register(ProcessingErrorCode.rsInvalidRepeatOnNewPage, Severity.Error, context.ObjectType, context.ObjectName, "TablixMember", "RepeatOnNewPage", flag2.Value ? "True" : "False", members[i].RepeatOnNewPage ? "True" : "False");
				}
				if (flag && members[i].KeepWithGroup != KeepWithGroup.After)
				{
					context.ErrorContext.Register(ProcessingErrorCode.rsInvalidKeepWithGroup, Severity.Error, context.ObjectType, context.ObjectName, "TablixMember", "KeepWithGroup", "After", (members[i].KeepWithGroup == KeepWithGroup.None) ? "None" : "Before");
				}
				else if (members[i].KeepWithGroup == KeepWithGroup.Before)
				{
					if (num == -1)
					{
						if (members[i].ParentMember != null)
						{
							members[i].KeepWithGroup = members[i].ParentMember.KeepWithGroup;
						}
						else
						{
							members[i].KeepWithGroup = KeepWithGroup.None;
						}
					}
					else if (num != i - 1 && members[i - 1].KeepWithGroup != KeepWithGroup.Before)
					{
						context.ErrorContext.Register(ProcessingErrorCode.rsInvalidKeepWithGroup, Severity.Error, context.ObjectType, context.ObjectName, "TablixMember", "KeepWithGroup", "Before", (members[i - 1].KeepWithGroup == KeepWithGroup.None) ? "None" : "After");
					}
				}
				else if (members[i].KeepWithGroup == KeepWithGroup.After)
				{
					flag = true;
					num2 = i;
				}
			}
			if (!flag)
			{
				return;
			}
			for (int j = num2; j < members.Count; j++)
			{
				if (members[j].ParentMember != null)
				{
					members[j].KeepWithGroup = members[j].ParentMember.KeepWithGroup;
				}
				else
				{
					members[j].KeepWithGroup = KeepWithGroup.None;
				}
			}
		}

		private static bool HasDynamic(TablixMemberList members)
		{
			foreach (TablixMember member in members)
			{
				if (!member.IsStatic)
				{
					return true;
				}
			}
			return false;
		}

		protected override void InitializeCorner(InitializationContext context)
		{
			if ((m_columnHeaderRowCount == 0 || m_rowHeaderColumnCount == 0) && m_corner == null)
			{
				return;
			}
			if (m_corner == null || m_corner.Count != m_columnHeaderRowCount)
			{
				context.ErrorContext.Register(ProcessingErrorCode.rsWrongNumberOfTablixCornerRows, Severity.Error, context.ObjectType, context.ObjectName, "TablixCornerRows");
			}
			if (m_corner == null)
			{
				return;
			}
			int[] array = new int[m_rowHeaderColumnCount];
			int[] array2 = new int[m_columnHeaderRowCount];
			for (int i = 0; i < m_corner.Count; i++)
			{
				List<TablixCornerCell> list = m_corner[i];
				for (int j = 0; j < list.Count; j++)
				{
					if (list.Count != m_rowHeaderColumnCount)
					{
						context.ErrorContext.Register(ProcessingErrorCode.rsWrongNumberOfTablixCornerCells, Severity.Error, context.ObjectType, context.ObjectName, "TablixCornerCell", i.ToString(CultureInfo.InvariantCulture.NumberFormat));
						return;
					}
					TablixCornerCell tablixCornerCell = list[j];
					Global.Tracer.Assert((tablixCornerCell.ColSpan == 0) ? (tablixCornerCell.RowSpan == 0) : (tablixCornerCell.RowSpan > 0), "(((cell.ColSpan == 0) ? (cell.RowSpan == 0) : cell.RowSpan > 0))");
					for (int k = 0; k < tablixCornerCell.ColSpan && j + k < array.Length; k++)
					{
						array[j + k] += tablixCornerCell.RowSpan;
					}
					for (int l = 0; l < tablixCornerCell.RowSpan && i + l < array2.Length; l++)
					{
						array2[i + l] += tablixCornerCell.ColSpan;
					}
					tablixCornerCell.Initialize(base.ID, -1, i, j, context);
				}
			}
			for (int m = 0; m < array.Length; m++)
			{
				if (array[m] != m_columnHeaderRowCount)
				{
					context.ErrorContext.Register(ProcessingErrorCode.rsInvalidTablixCornerRowSpans, Severity.Error, context.ObjectType, context.ObjectName, "TablixCornerRows", m.ToString(CultureInfo.InvariantCulture.NumberFormat));
				}
			}
			for (int n = 0; n < array2.Length; n++)
			{
				if (array2[n] != m_rowHeaderColumnCount)
				{
					context.ErrorContext.Register(ProcessingErrorCode.rsInvalidTablixCornerColumnSpans, Severity.Error, context.ObjectType, context.ObjectName, "TablixCornerCell", n.ToString(CultureInfo.InvariantCulture.NumberFormat));
				}
			}
		}

		protected override bool InitializeRows(InitializationContext context)
		{
			double num = 0.0;
			double num2 = 0.0;
			bool result = true;
			if ((m_tablixColumnMembers != null && m_tablixColumns == null) || (m_tablixColumnMembers == null && m_tablixColumns != null) || (m_tablixColumns != null && m_tablixColumns.Count != m_columnCount))
			{
				context.ErrorContext.Register(ProcessingErrorCode.rsWrongNumberOfTablixColumns, Severity.Error, context.ObjectType, context.ObjectName, "TablixColumns");
				result = false;
			}
			else if (m_tablixColumns != null)
			{
				foreach (TablixColumn tablixColumn in m_tablixColumns)
				{
					tablixColumn.Initialize(context);
					if (!tablixColumn.ForAutoSubtotal)
					{
						num += tablixColumn.WidthValue;
					}
				}
			}
			if ((m_tablixRowMembers != null && m_tablixRows == null) || (m_tablixRowMembers == null && m_tablixRows != null) || (m_tablixRows != null && m_tablixRows.Count != m_rowCount))
			{
				context.ErrorContext.Register(ProcessingErrorCode.rsWrongNumberOfTablixRows, Severity.Error, context.ObjectType, context.ObjectName, "TablixRows");
				result = false;
			}
			if (m_tablixRows != null)
			{
				int num3 = 0;
				for (int i = 0; i < m_tablixRows.Count; i++)
				{
					TablixRow tablixRow = TablixRows[i];
					if (tablixRow == null || tablixRow.Cells == null || tablixRow.Cells.Count != m_columnCount)
					{
						context.ErrorContext.Register(ProcessingErrorCode.rsWrongNumberOfTablixCells, Severity.Error, context.ObjectType, context.ObjectName, "TablixCells");
						result = false;
					}
					tablixRow.Initialize(context);
					if (!tablixRow.ForAutoSubtotal)
					{
						num2 += tablixRow.HeightValue;
					}
					num3 = 0;
					for (int j = 0; j < tablixRow.Cells.Count; j++)
					{
						TablixCell tablixCell = tablixRow.TablixCells[j];
						if (tablixCell.ColSpan > 1 && !ValidateColSpan(this, j, tablixCell.ColSpan))
						{
							context.ErrorContext.Register(ProcessingErrorCode.rsInvalidTablixCellColSpan, Severity.Error, context.ObjectType, context.ObjectName, "TablixCell");
							result = false;
						}
						num3 += tablixCell.ColSpan;
					}
					if (num3 != m_columnCount)
					{
						context.ErrorContext.Register(ProcessingErrorCode.rsInvalidTablixCellColSpans, Severity.Error, context.ObjectType, context.ObjectName, "TablixCells");
						result = false;
					}
				}
			}
			Math.Round(num2, 10);
			Math.Round(num, 10);
			if (!CanScroll)
			{
				m_heightValue = num2;
				m_widthValue = num;
			}
			else
			{
				if (ComputeHeight)
				{
					m_heightValue = num2;
				}
				else
				{
					m_heightValue = new ReportSize(m_height).ToMillimeters();
				}
				if (ComputeWidth)
				{
					m_widthValue = num;
				}
				else
				{
					m_widthValue = new ReportSize(m_width).ToMillimeters();
				}
			}
			return result;
		}

		private bool ValidateColSpan(Tablix tablix, int index, int colSpan)
		{
			int current = -1;
			foreach (TablixMember columnMember in tablix.ColumnMembers)
			{
				if (ValidateColSpan(columnMember, index, colSpan, ref current))
				{
					if (current >= index + colSpan - 1)
					{
						return true;
					}
					continue;
				}
				return false;
			}
			return false;
		}

		private bool ValidateColSpan(TablixMember aMember, int index, int colSpan, ref int current)
		{
			if (current >= index && !aMember.IsStatic)
			{
				return false;
			}
			if (aMember.SubMembers != null && aMember.SubMembers.Count > 0)
			{
				foreach (TablixMember subMember in aMember.SubMembers)
				{
					if (ValidateColSpan(subMember, index, colSpan, ref current))
					{
						if (current >= index + colSpan - 1)
						{
							return true;
						}
						continue;
					}
					return false;
				}
			}
			else
			{
				current++;
			}
			if (current < index)
			{
				return true;
			}
			return aMember.IsStatic;
		}

		protected override bool ValidateInnerStructure(InitializationContext context)
		{
			if (m_rowCount > 0 && m_columnCount > 0)
			{
				if (m_initData.IsTopLevelDataRegion)
				{
					if ((m_fixedRowHeaders && m_initData.HasFixedColData) || (m_fixedColumnHeaders && m_initData.HasFixedRowData))
					{
						context.ErrorContext.Register(ProcessingErrorCode.rsInvalidFixedHeaderOnOppositeHierarchy, Severity.Error, context.ObjectType, context.ObjectName, "FixedHeader", m_fixedColumnHeaders ? "FixedColumnHeaders" : "FixedRowHeaders");
					}
					if (m_initData.HasFixedRowData && !m_tablixRowMembers[0].FixedData)
					{
						context.ErrorContext.Register(ProcessingErrorCode.rsInvalidFixedDataRowPosition, Severity.Error, context.ObjectType, context.ObjectName, "FixedData");
					}
					if (m_initData.HasFixedColData && m_groupsBeforeRowHeaders > 0 && m_tablixColumnMembers[0].FixedData)
					{
						context.ErrorContext.Register(ProcessingErrorCode.rsInvalidFixedDataColumnPosition, Severity.Error, context.ObjectType, context.ObjectName, "FixedData");
					}
					if (m_initData.HasFixedColData)
					{
						for (int i = 0; i < m_tablixRows.Count; i++)
						{
							TablixRow tablixRow = m_tablixRows[i];
							int num = tablixRow.TablixCells[m_initData.FixedColStartIndex].ColSpan;
							if (num > 0)
							{
								for (int j = m_initData.FixedColStartIndex + 1; j < m_initData.FixedColStartIndex + m_initData.FixedColLength; j++)
								{
									num += tablixRow.TablixCells[j].ColSpan;
								}
							}
							if (num != m_initData.FixedColLength)
							{
								context.ErrorContext.Register(ProcessingErrorCode.rsInvalidFixedDataBodyCellSpans, Severity.Error, context.ObjectType, context.ObjectName, i.ToString(CultureInfo.InvariantCulture));
							}
						}
					}
				}
				else if (m_initData.HasFixedColData || m_initData.HasFixedRowData || m_fixedRowHeaders || m_fixedColumnHeaders)
				{
					context.ErrorContext.Register(ProcessingErrorCode.rsFixedHeadersInInnerDataRegion, Severity.Error, context.ObjectType, context.ObjectName, "FixedHeader");
				}
				if (m_groupsBeforeRowHeaders > 0 && m_tablixColumnMembers[0].Grouping == null)
				{
					m_groupsBeforeRowHeaders = 0;
				}
			}
			return true;
		}

		protected override bool InitializeColumnMembers(InitializationContext context)
		{
			if (m_tablixColumnMembers == null)
			{
				m_heightValue = 0.0;
				m_height = "0mm";
				return false;
			}
			bool num = base.InitializeColumnMembers(context);
			if (num && (!CanScroll || ComputeHeight))
			{
				m_heightValue += context.GetTotalHeaderSize(isColumnHierarchy: true, m_columnHeaderRowCount);
				m_heightValue = Math.Round(m_heightValue, 10);
				m_height = Microsoft.ReportingServices.ReportPublishing.Converter.ConvertSize(m_heightValue);
			}
			return num;
		}

		protected override bool InitializeRowMembers(InitializationContext context)
		{
			if (m_tablixColumnMembers == null)
			{
				m_widthValue = 0.0;
				m_width = "0mm";
				return false;
			}
			ValidateKeepWithGroup(m_tablixRowMembers, context);
			bool num = base.InitializeRowMembers(context);
			if (num && (!CanScroll || ComputeWidth))
			{
				m_widthValue += context.GetTotalHeaderSize(isColumnHierarchy: false, m_rowHeaderColumnCount);
				m_widthValue = Math.Round(m_widthValue, 10);
				m_width = Microsoft.ReportingServices.ReportPublishing.Converter.ConvertSize(m_widthValue);
			}
			return num;
		}

		protected override void InitializeData(InitializationContext context)
		{
			context.RegisterReportItems(m_tablixRows);
			base.InitializeData(context);
			context.UnRegisterReportItems(m_tablixRows);
		}

		internal bool ValidateBandReportItemReference(string reportItemName)
		{
			if (reportItemName == null)
			{
				return true;
			}
			return ContainsReportItemInCurrentScope(reportItemName, includeCorner: false, includeDynamics: true);
		}

		private bool IsOrContainsReportItemInCurrentScope(ReportItem currentItem, string reportItemName)
		{
			if (currentItem == null)
			{
				return false;
			}
			if (string.CompareOrdinal(currentItem.Name, reportItemName) == 0)
			{
				return true;
			}
			switch (currentItem.GetObjectType())
			{
			case Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Rectangle:
				return ContainsReportItemInCurrentScope(((Rectangle)currentItem).ReportItems, reportItemName);
			case Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Tablix:
			{
				Tablix tablix = (Tablix)currentItem;
				if (!tablix.HasFilters)
				{
					return tablix.ContainsReportItemInCurrentScope(reportItemName, includeCorner: true, includeDynamics: false);
				}
				break;
			}
			}
			return false;
		}

		private bool ContainsReportItemInCurrentScope(ReportItemCollection items, string reportItemName)
		{
			if (items == null)
			{
				return false;
			}
			foreach (ReportItem item in items)
			{
				if (IsOrContainsReportItemInCurrentScope(item, reportItemName))
				{
					return true;
				}
			}
			return false;
		}

		private bool ContainsReportItemInCurrentScope(string reportItemName, bool includeCorner, bool includeDynamics)
		{
			List<int> memberCellIndices = new List<int>();
			List<int> memberCellIndices2 = new List<int>();
			if ((!includeCorner || !CornerContainsReportItemInCurrentScope(reportItemName)) && !ContainsReportItemInCurrentScope(m_tablixRowMembers, reportItemName, includeDynamics, ref memberCellIndices) && !ContainsReportItemInCurrentScope(m_tablixColumnMembers, reportItemName, includeDynamics, ref memberCellIndices2))
			{
				return BodyContainsReportItemInCurrentScope(memberCellIndices, memberCellIndices2, reportItemName);
			}
			return true;
		}

		private bool CornerContainsReportItemInCurrentScope(string reportItemName)
		{
			if (m_corner == null)
			{
				return false;
			}
			foreach (List<TablixCornerCell> item in m_corner)
			{
				if (item == null)
				{
					continue;
				}
				foreach (TablixCornerCell item2 in item)
				{
					if (ContainsReportItemInCurrentScope(item2, reportItemName))
					{
						return true;
					}
				}
			}
			return false;
		}

		private bool ContainsReportItemInCurrentScope(TablixCellBase cell, string reportItemName)
		{
			if (cell == null)
			{
				return false;
			}
			if (!IsOrContainsReportItemInCurrentScope(cell.CellContents, reportItemName))
			{
				return IsOrContainsReportItemInCurrentScope(cell.AltCellContents, reportItemName);
			}
			return true;
		}

		private bool ContainsReportItemInCurrentScope(TablixHeader header, string reportItemName)
		{
			if (header == null)
			{
				return false;
			}
			if (!IsOrContainsReportItemInCurrentScope(header.CellContents, reportItemName))
			{
				return IsOrContainsReportItemInCurrentScope(header.AltCellContents, reportItemName);
			}
			return true;
		}

		private bool ContainsReportItemInCurrentScope(TablixMemberList members, string reportItemName, bool includeDynamics, ref List<int> memberCellIndices)
		{
			if (members == null)
			{
				return false;
			}
			foreach (TablixMember member in members)
			{
				if (member.IsStatic || includeDynamics)
				{
					if (ContainsReportItemInCurrentScope(member.TablixHeader, reportItemName) || ContainsReportItemInCurrentScope(member.SubMembers, reportItemName, includeDynamics, ref memberCellIndices))
					{
						return true;
					}
					if (member.IsLeaf)
					{
						memberCellIndices.Add(member.MemberCellIndex);
					}
				}
			}
			return false;
		}

		private bool BodyContainsReportItemInCurrentScope(List<int> rowCellIndices, List<int> colCellIndices, string reportItemName)
		{
			foreach (int rowCellIndex in rowCellIndices)
			{
				TablixRow tablixRow = TablixRows[rowCellIndex];
				if (tablixRow.Cells == null)
				{
					continue;
				}
				foreach (int colCellIndex in colCellIndices)
				{
					TablixCell cell = tablixRow.TablixCells[colCellIndex];
					if (ContainsReportItemInCurrentScope(cell, reportItemName))
					{
						return true;
					}
				}
			}
			return false;
		}

		internal override object PublishClone(AutomaticSubtotalContext context)
		{
			Tablix tablix = (Tablix)(context.CurrentDataRegionClone = (Tablix)base.PublishClone(context));
			if (m_tablixColumnMembers != null)
			{
				tablix.m_tablixColumnMembers = new TablixMemberList(m_tablixColumnMembers.Count);
				foreach (TablixMember tablixColumnMember in m_tablixColumnMembers)
				{
					tablix.m_tablixColumnMembers.Add(tablixColumnMember.PublishClone(context, tablix));
				}
			}
			if (m_tablixRowMembers != null)
			{
				tablix.m_tablixRowMembers = new TablixMemberList(m_tablixRowMembers.Count);
				foreach (TablixMember tablixRowMember in m_tablixRowMembers)
				{
					tablix.m_tablixRowMembers.Add(tablixRowMember.PublishClone(context, tablix));
				}
			}
			if (m_corner != null)
			{
				tablix.m_corner = new List<List<TablixCornerCell>>(m_corner.Count);
				foreach (List<TablixCornerCell> item in m_corner)
				{
					List<TablixCornerCell> list = new List<TablixCornerCell>(item.Count);
					foreach (TablixCornerCell item2 in item)
					{
						list.Add((TablixCornerCell)item2.PublishClone(context));
					}
					tablix.m_corner.Add(list);
				}
			}
			if (m_tablixRows != null)
			{
				tablix.m_tablixRows = new TablixRowList(m_tablixRows.Count);
				foreach (TablixRow tablixRow in m_tablixRows)
				{
					tablix.m_tablixRows.Add((TablixRow)tablixRow.PublishClone(context));
				}
			}
			if (m_tablixColumns != null)
			{
				tablix.m_tablixColumns = new List<TablixColumn>(m_tablixColumns.Count);
				foreach (TablixColumn tablixColumn in m_tablixColumns)
				{
					tablix.m_tablixColumns.Add((TablixColumn)tablixColumn.PublishClone(context));
				}
			}
			context.CreateSubtotalsDefinitions.Add(tablix);
			return tablix;
		}

		protected override ReportHierarchyNode CreateHierarchyNode(int id)
		{
			return new TablixMember(id, this);
		}

		protected override Row CreateRow(int id, int columnCount)
		{
			return new TablixRow(id)
			{
				Height = "0mm",
				TablixCells = new TablixCellList(columnCount)
			};
		}

		protected override Cell CreateCell(int id, int rowIndex, int colIndex)
		{
			TablixCell tablixCell = new TablixCell(id, this);
			if (rowIndex != -1)
			{
				tablixCell.ColSpan = 1;
				tablixCell.RowSpan = ((TablixCell)Rows[rowIndex].Cells[0]).RowSpan;
			}
			else if (colIndex != -1)
			{
				tablixCell.ColSpan = ((TablixCell)Rows[0].Cells[colIndex]).ColSpan;
				tablixCell.RowSpan = 1;
			}
			return tablixCell;
		}

		protected override void CreateDomainScopeRowsAndCells(AutomaticSubtotalContext context, ReportHierarchyNode member)
		{
			base.CreateDomainScopeRowsAndCells(context, member);
			if (member.IsColumn)
			{
				TablixColumn tablixColumn = new TablixColumn(context.GenerateID());
				tablixColumn.Width = "0mm";
				m_tablixColumns.Insert(ColumnMembers.GetMemberIndex(member), tablixColumn);
			}
		}

		public void CreateAutomaticSubtotals(AutomaticSubtotalContext context)
		{
			if (m_createdSubtotals || m_tablixRows == null || m_rowCount != m_tablixRows.Count || m_tablixColumns == null || m_tablixColumns.Count != m_columnCount)
			{
				return;
			}
			for (int i = 0; i < m_tablixRows.Count; i++)
			{
				if (m_tablixRows[i].Cells == null || m_tablixRows[i].Cells.Count != m_columnCount)
				{
					return;
				}
			}
			context.Location = Microsoft.ReportingServices.ReportPublishing.LocationFlags.None;
			context.ObjectType = ObjectType;
			context.ObjectName = "Tablix";
			context.CurrentDataRegion = this;
			context.OriginalRowCount = m_rowCount;
			context.OriginalColumnCount = m_columnCount;
			context.CellLists = new List<CellList>(m_tablixRows.Count);
			for (int j = 0; j < m_tablixRows.Count; j++)
			{
				context.CellLists.Add(new CellList());
			}
			context.TablixColumns = new List<TablixColumn>(m_tablixColumns.Count);
			context.Rows = new RowList(m_tablixRows.Count);
			context.CurrentScope = m_name;
			context.CurrentDataScope = this;
			context.StartIndex = 0;
			CreateAutomaticSubtotals(context, m_tablixColumnMembers, isColumn: true);
			context.StartIndex = 0;
			CreateAutomaticSubtotals(context, m_tablixRowMembers, isColumn: false);
			context.CurrentScope = null;
			context.CurrentDataScope = null;
			m_createdSubtotals = true;
		}

		private int CreateAutomaticSubtotals(AutomaticSubtotalContext context, TablixMemberList members, bool isColumn)
		{
			int num = 0;
			bool flag = AllSiblingsHaveConditionalOrToggleableVisibility(members);
			for (int i = 0; i < members.Count; i++)
			{
				TablixMember tablixMember = members[i];
				if (tablixMember.Grouping != null && tablixMember.HasToggleableVisibility && flag)
				{
					context.CurrentIndex = context.StartIndex;
					if (isColumn)
					{
						foreach (CellList cellList in context.CellLists)
						{
							cellList.Clear();
						}
						context.TablixColumns.Clear();
					}
					else
					{
						context.Rows.Clear();
					}
					int aDynamicsRemoved = 0;
					bool aAllWereDynamic = true;
					context.HeaderLevel = tablixMember.HeaderLevel;
					Global.Tracer.Assert(tablixMember.HeaderLevelHasStaticArray != null, "(member.HeaderLevelHasStaticArray != null)");
					context.HeaderLevelHasStaticArray = tablixMember.HeaderLevelHasStaticArray;
					BuildAndSetupAxisScopeTreeForAutoSubtotals(ref context, tablixMember);
					TablixMember tablixMember2 = tablixMember.CreateAutomaticSubtotalClone(context, tablixMember.ParentMember, isDynamicTarget: true, out aDynamicsRemoved, ref aAllWereDynamic);
					context.AdjustReferences();
					tablixMember2.IsAutoSubtotal = true;
					if (i + 1 < members.Count)
					{
						TablixMember tablixMember3 = members[i + 1];
						if (tablixMember3.IsStatic && tablixMember3.KeepWithGroup == KeepWithGroup.Before)
						{
							tablixMember2.KeepWithGroup = KeepWithGroup.Before;
							tablixMember2.RepeatOnNewPage = tablixMember3.RepeatOnNewPage;
						}
					}
					members.Insert(i + 1, tablixMember2);
					num = context.CurrentIndex - context.StartIndex;
					if (isColumn)
					{
						for (int j = 0; j < m_tablixRows.Count; j++)
						{
							m_tablixRows[j].Cells.InsertRange(context.CurrentIndex, context.CellLists[j]);
						}
						m_tablixColumns.InsertRange(context.CurrentIndex, context.TablixColumns);
						m_columnCount += num;
					}
					else
					{
						m_tablixRows.InsertRange(context.CurrentIndex, context.Rows);
						m_rowCount += num;
					}
					if (tablixMember.SubMembers != null)
					{
						context.CurrentScope = tablixMember.Grouping.Name;
						context.CurrentDataScope = tablixMember;
						int num2 = CreateAutomaticSubtotals(context, tablixMember.SubMembers, isColumn);
						if (isColumn)
						{
							tablixMember.ColSpan += num2;
						}
						else
						{
							tablixMember.RowSpan += num2;
						}
						num += num2;
					}
					else
					{
						context.StartIndex++;
					}
				}
				else if (tablixMember.SubMembers != null)
				{
					if (tablixMember.Grouping != null)
					{
						context.CurrentScope = tablixMember.Grouping.Name;
						context.CurrentDataScope = tablixMember;
					}
					int num3 = CreateAutomaticSubtotals(context, tablixMember.SubMembers, isColumn);
					if (isColumn)
					{
						tablixMember.ColSpan += num3;
					}
					else
					{
						tablixMember.RowSpan += num3;
					}
					num += num3;
				}
				else
				{
					context.StartIndex++;
				}
			}
			return num;
		}

		private bool AllSiblingsHaveConditionalOrToggleableVisibility(TablixMemberList members)
		{
			if (members.Count > 1)
			{
				for (int i = 0; i < members.Count; i++)
				{
					if (!members[i].HasConditionalOrToggleableVisibility)
					{
						return false;
					}
				}
			}
			return true;
		}

		internal void ValidateBandStructure(PublishingContextStruct context)
		{
			int num = 0;
			int num2 = 0;
			bool isdynamic = false;
			SetIgnoredPropertiesForBandingToDefault(context);
			if (LayoutDirection)
			{
				context.ErrorContext.Register(ProcessingErrorCode.rsInvalidBandInvalidLayoutDirection, Severity.Error, context.ObjectType, context.ObjectName, "Tablix");
			}
			foreach (TablixMember tablixColumnMember in TablixColumnMembers)
			{
				tablixColumnMember.ValidateTablixMemberForBanding(context, out isdynamic);
				if (isdynamic)
				{
					num++;
				}
			}
			foreach (TablixMember tablixRowMember in TablixRowMembers)
			{
				tablixRowMember.ValidateTablixMemberForBanding(context, out isdynamic);
				if (isdynamic)
				{
					num2++;
				}
			}
		}

		private void SetIgnoredPropertiesForBandingToDefault(PublishingContextStruct context)
		{
			if (GroupsBeforeRowHeaders != 0)
			{
				GroupsBeforeRowHeaders = 0;
				context.ErrorContext.Register(ProcessingErrorCode.rsBandIgnoredProperties, Severity.Warning, context.ObjectType, context.ObjectName, "GroupsBeforeRowHeaders");
			}
			if (RepeatColumnHeaders)
			{
				RepeatColumnHeaders = false;
				context.ErrorContext.Register(ProcessingErrorCode.rsBandIgnoredProperties, Severity.Warning, context.ObjectType, context.ObjectName, "RepeatColumnHeaders");
			}
			if (RepeatRowHeaders)
			{
				RepeatRowHeaders = false;
				context.ErrorContext.Register(ProcessingErrorCode.rsBandIgnoredProperties, Severity.Warning, context.ObjectType, context.ObjectName, "FixedColumnHeaders");
			}
			if (FixedColumnHeaders)
			{
				FixedColumnHeaders = false;
				context.ErrorContext.Register(ProcessingErrorCode.rsBandIgnoredProperties, Severity.Warning, context.ObjectType, context.ObjectName, "FixedColumnHeaders");
			}
			if (FixedRowHeaders)
			{
				FixedRowHeaders = false;
				context.ErrorContext.Register(ProcessingErrorCode.rsBandIgnoredProperties, Severity.Warning, context.ObjectType, context.ObjectName, "FixedRowHeaders");
			}
		}

		internal new static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.TablixColumnMembers, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.TablixMember));
			list.Add(new MemberInfo(MemberName.TablixRowMembers, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.TablixMember));
			list.Add(new MemberInfo(MemberName.TablixRows, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.TablixRow));
			list.Add(new MemberInfo(MemberName.TablixColumns, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.TablixColumn));
			list.Add(new MemberInfo(MemberName.TablixCornerCells, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.TablixCornerCell));
			list.Add(new MemberInfo(MemberName.PropagatedPageBreakLocation, Token.Enum));
			list.Add(new MemberInfo(MemberName.InnerRowLevelWithPageBreak, Token.Int32));
			list.Add(new MemberInfo(MemberName.GroupsBeforeRowHeaders, Token.Int32));
			list.Add(new MemberInfo(MemberName.LayoutDirection, Token.Boolean));
			list.Add(new MemberInfo(MemberName.RepeatColumnHeaders, Token.Boolean));
			list.Add(new MemberInfo(MemberName.RepeatRowHeaders, Token.Boolean));
			list.Add(new MemberInfo(MemberName.FixedColumnHeaders, Token.Boolean));
			list.Add(new MemberInfo(MemberName.FixedRowHeaders, Token.Boolean));
			list.Add(new MemberInfo(MemberName.OmitBorderOnPageBreak, Token.Boolean));
			list.Add(new MemberInfo(MemberName.HideStaticsIfNoRows, Token.Boolean));
			list.Add(new MemberInfo(MemberName.InScopeTextBoxes, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, Token.Reference, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.TextBox));
			list.Add(new MemberInfo(MemberName.ColumnHeaderRowCount, Token.Int32));
			list.Add(new MemberInfo(MemberName.RowHeaderColumnCount, Token.Int32));
			list.Add(new MemberInfo(MemberName.KeepTogether, Token.Boolean));
			list.Add(new MemberInfo(MemberName.CanScroll, Token.Boolean));
			list.Add(new MemberInfo(MemberName.BandLayout, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObject));
			list.Add(new MemberInfo(MemberName.TopMargin, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.BottomMargin, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.LeftMargin, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.RightMargin, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.EnableRowDrilldown, Token.Boolean, Lifetime.AddedIn(200)));
			list.Add(new MemberInfo(MemberName.EnableColumnDrilldown, Token.Boolean, Lifetime.AddedIn(200)));
			return new Declaration(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Tablix, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.DataRegion, list);
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.CanScroll:
					writer.Write(m_canScroll);
					break;
				case MemberName.KeepTogether:
					writer.Write(m_keepTogether);
					break;
				case MemberName.TablixColumnMembers:
					writer.Write(m_tablixColumnMembers);
					break;
				case MemberName.TablixRowMembers:
					writer.Write(m_tablixRowMembers);
					break;
				case MemberName.TablixRows:
					writer.Write(m_tablixRows);
					break;
				case MemberName.TablixColumns:
					writer.Write(m_tablixColumns);
					break;
				case MemberName.TablixCornerCells:
					writer.Write(m_corner);
					break;
				case MemberName.PropagatedPageBreakLocation:
					writer.WriteEnum((int)m_propagatedPageBreakLocation);
					break;
				case MemberName.InnerRowLevelWithPageBreak:
					writer.Write(m_innerRowLevelWithPageBreak);
					break;
				case MemberName.GroupsBeforeRowHeaders:
					writer.Write(m_groupsBeforeRowHeaders);
					break;
				case MemberName.LayoutDirection:
					writer.Write(m_layoutDirection);
					break;
				case MemberName.RepeatColumnHeaders:
					writer.Write(m_repeatColumnHeaders);
					break;
				case MemberName.RepeatRowHeaders:
					writer.Write(m_repeatRowHeaders);
					break;
				case MemberName.FixedColumnHeaders:
					writer.Write(m_fixedColumnHeaders);
					break;
				case MemberName.FixedRowHeaders:
					writer.Write(m_fixedRowHeaders);
					break;
				case MemberName.OmitBorderOnPageBreak:
					writer.Write(m_omitBorderOnPageBreak);
					break;
				case MemberName.HideStaticsIfNoRows:
					writer.Write(m_hideStaticsIfNoRows);
					break;
				case MemberName.InScopeTextBoxes:
					writer.WriteListOfReferences(m_inScopeTextBoxes);
					break;
				case MemberName.ColumnHeaderRowCount:
					writer.Write(m_columnHeaderRowCount);
					break;
				case MemberName.RowHeaderColumnCount:
					writer.Write(m_rowHeaderColumnCount);
					break;
				case MemberName.BandLayout:
					writer.Write(m_bandLayout);
					break;
				case MemberName.TopMargin:
					writer.Write(m_topMargin);
					break;
				case MemberName.BottomMargin:
					writer.Write(m_bottomMargin);
					break;
				case MemberName.LeftMargin:
					writer.Write(m_leftMargin);
					break;
				case MemberName.RightMargin:
					writer.Write(m_rightMargin);
					break;
				case MemberName.EnableRowDrilldown:
					writer.Write(m_enableRowDrilldown);
					break;
				case MemberName.EnableColumnDrilldown:
					writer.Write(m_enableColumnDrilldown);
					break;
				default:
					Global.Tracer.Assert(condition: false);
					break;
				}
			}
		}

		public override void Deserialize(IntermediateFormatReader reader)
		{
			base.Deserialize(reader);
			reader.RegisterDeclaration(m_Declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.CanScroll:
					m_canScroll = reader.ReadBoolean();
					break;
				case MemberName.KeepTogether:
					m_keepTogether = reader.ReadBoolean();
					break;
				case MemberName.TablixColumnMembers:
					m_tablixColumnMembers = reader.ReadListOfRIFObjects<TablixMemberList>();
					break;
				case MemberName.TablixRowMembers:
					m_tablixRowMembers = reader.ReadListOfRIFObjects<TablixMemberList>();
					break;
				case MemberName.TablixRows:
					m_tablixRows = reader.ReadListOfRIFObjects<TablixRowList>();
					break;
				case MemberName.TablixColumns:
					m_tablixColumns = reader.ReadGenericListOfRIFObjects<TablixColumn>();
					break;
				case MemberName.TablixCornerCells:
					m_corner = reader.ReadListOfListsOfRIFObjects<TablixCornerCell>();
					break;
				case MemberName.PropagatedPageBreakLocation:
					m_propagatedPageBreakLocation = (PageBreakLocation)reader.ReadEnum();
					break;
				case MemberName.InnerRowLevelWithPageBreak:
					m_innerRowLevelWithPageBreak = reader.ReadInt32();
					break;
				case MemberName.GroupsBeforeRowHeaders:
					m_groupsBeforeRowHeaders = reader.ReadInt32();
					break;
				case MemberName.LayoutDirection:
					m_layoutDirection = reader.ReadBoolean();
					break;
				case MemberName.RepeatColumnHeaders:
					m_repeatColumnHeaders = reader.ReadBoolean();
					break;
				case MemberName.RepeatRowHeaders:
					m_repeatRowHeaders = reader.ReadBoolean();
					break;
				case MemberName.FixedColumnHeaders:
					m_fixedColumnHeaders = reader.ReadBoolean();
					break;
				case MemberName.FixedRowHeaders:
					m_fixedRowHeaders = reader.ReadBoolean();
					break;
				case MemberName.OmitBorderOnPageBreak:
					m_omitBorderOnPageBreak = reader.ReadBoolean();
					break;
				case MemberName.HideStaticsIfNoRows:
					m_hideStaticsIfNoRows = reader.ReadBoolean();
					break;
				case MemberName.InScopeTextBoxes:
					m_inScopeTextBoxes = reader.ReadGenericListOfReferences<TextBox>(this);
					break;
				case MemberName.ColumnHeaderRowCount:
					m_columnHeaderRowCount = reader.ReadInt32();
					break;
				case MemberName.RowHeaderColumnCount:
					m_rowHeaderColumnCount = reader.ReadInt32();
					break;
				case MemberName.BandLayout:
					m_bandLayout = reader.ReadRIFObject<BandLayoutOptions>();
					break;
				case MemberName.TopMargin:
					m_topMargin = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.BottomMargin:
					m_bottomMargin = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.LeftMargin:
					m_leftMargin = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.RightMargin:
					m_rightMargin = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.EnableRowDrilldown:
					m_enableRowDrilldown = reader.ReadBoolean();
					break;
				case MemberName.EnableColumnDrilldown:
					m_enableColumnDrilldown = reader.ReadBoolean();
					break;
				default:
					Global.Tracer.Assert(condition: false);
					break;
				}
			}
			if (reader.IntermediateFormatVersion.CompareTo(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.IntermediateFormatVersion.RTM2008) < 0)
			{
				FixIndexInCollections();
			}
		}

		public override void ResolveReferences(Dictionary<Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
			base.ResolveReferences(memberReferencesCollection, referenceableItems);
			if (!memberReferencesCollection.TryGetValue(m_Declaration.ObjectType, out List<MemberReference> value))
			{
				return;
			}
			foreach (MemberReference item in value)
			{
				MemberName memberName = item.MemberName;
				if (memberName == MemberName.InScopeTextBoxes)
				{
					if (m_inScopeTextBoxes == null)
					{
						m_inScopeTextBoxes = new List<TextBox>();
					}
					Global.Tracer.Assert(referenceableItems.ContainsKey(item.RefID));
					Global.Tracer.Assert(referenceableItems[item.RefID] is TextBox);
					m_inScopeTextBoxes.Add((TextBox)referenceableItems[item.RefID]);
				}
				else
				{
					Global.Tracer.Assert(condition: false);
				}
			}
		}

		public override Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Tablix;
		}

		private void FixIndexInCollections()
		{
			IndexInCollectionUpgrader indexUpgrader = new IndexInCollectionUpgrader();
			if (m_tablixRowMembers == null || m_tablixColumnMembers == null)
			{
				return;
			}
			int rowIndex = 0;
			int colIndex = 0;
			foreach (TablixMember tablixRowMember in m_tablixRowMembers)
			{
				FixIndexInCollection(tablixRowMember, indexUpgrader, isColumn: false, ref rowIndex, ref colIndex);
			}
		}

		private void FixIndexInCollection(TablixMember member, IndexInCollectionUpgrader indexUpgrader, bool isColumn, ref int rowIndex, ref int colIndex)
		{
			if (!member.IsStatic)
			{
				indexUpgrader.RegisterGroup(member.Grouping.Name);
			}
			if (member.SubMembers != null && member.SubMembers.Count > 0)
			{
				foreach (TablixMember subMember in member.SubMembers)
				{
					FixIndexInCollection(subMember, indexUpgrader, isColumn, ref rowIndex, ref colIndex);
				}
			}
			else if (!isColumn)
			{
				colIndex = 0;
				foreach (TablixMember tablixColumnMember in m_tablixColumnMembers)
				{
					FixIndexInCollection(tablixColumnMember, indexUpgrader, isColumn: true, ref rowIndex, ref colIndex);
				}
				rowIndex++;
			}
			else
			{
				TablixCell tablixCell = m_tablixRows[rowIndex].TablixCells[colIndex];
				if (tablixCell != null)
				{
					indexUpgrader.SetIndexInCollection(tablixCell);
				}
				colIndex++;
			}
			if (!member.IsStatic)
			{
				indexUpgrader.UnregisterGroup(member.Grouping.Name);
			}
		}

		internal override void SetExprHost(ReportExprHost reportExprHost, ObjectModelImpl reportObjectModel)
		{
			if (base.ExprHostID >= 0)
			{
				Global.Tracer.Assert(reportExprHost != null && reportObjectModel != null, "(reportExprHost != null && reportObjectModel != null)");
				m_tablixExprHost = reportExprHost.TablixHostsRemotable[base.ExprHostID];
				DataRegionSetExprHost(m_tablixExprHost, m_tablixExprHost.SortHost, m_tablixExprHost.FilterHostsRemotable, m_tablixExprHost.UserSortExpressionsHost, m_tablixExprHost.PageBreakExprHost, m_tablixExprHost.JoinConditionExprHostsRemotable, reportObjectModel);
			}
		}

		internal override void DataRegionContentsSetExprHost(ObjectModelImpl reportObjectModel, bool traverseDataRegions)
		{
			if (m_corner != null)
			{
				for (int i = 0; i < m_corner.Count; i++)
				{
					List<TablixCornerCell> list = m_corner[i];
					for (int j = 0; j < list.Count; j++)
					{
						TablixCornerCell tablixCornerCell = list[j];
						if (tablixCornerCell != null && tablixCornerCell.CellContents != null)
						{
							reportObjectModel.OdpContext.RuntimeInitializeReportItemObjs(tablixCornerCell.CellContents, traverseDataRegions);
							if (tablixCornerCell.AltCellContents != null)
							{
								reportObjectModel.OdpContext.RuntimeInitializeReportItemObjs(tablixCornerCell.AltCellContents, traverseDataRegions);
							}
						}
					}
				}
			}
			if (m_tablixRows == null)
			{
				return;
			}
			IList<TablixCellExprHost> list2 = (m_tablixExprHost != null) ? m_tablixExprHost.CellHostsRemotable : null;
			for (int k = 0; k < m_tablixRows.Count; k++)
			{
				TablixRow tablixRow = m_tablixRows[k];
				Global.Tracer.Assert(tablixRow != null && tablixRow.Cells != null, "(null != row && null != row.Cells)");
				for (int l = 0; l < tablixRow.TablixCells.Count; l++)
				{
					TablixCell tablixCell = tablixRow.TablixCells[l];
					Global.Tracer.Assert(tablixCell != null, "(null != cell)");
					if (list2 != null && tablixCell.ExpressionHostID >= 0)
					{
						tablixCell.SetExprHost(list2[tablixCell.ExpressionHostID], reportObjectModel);
					}
					if (tablixCell.CellContents != null)
					{
						reportObjectModel.OdpContext.RuntimeInitializeReportItemObjs(tablixCell.CellContents, traverseDataRegions);
						if (tablixCell.AltCellContents != null)
						{
							reportObjectModel.OdpContext.RuntimeInitializeReportItemObjs(tablixCell.AltCellContents, traverseDataRegions);
						}
					}
				}
			}
		}

		internal override object EvaluateNoRowsMessageExpression()
		{
			return m_tablixExprHost.NoRowsExpr;
		}

		internal string EvaluateTablixMargin(IReportScopeInstance reportScopeInstance, MarginPosition marginPosition, OnDemandProcessingContext context)
		{
			context.SetupContext(this, reportScopeInstance);
			return context.ReportRuntime.EvaluateTablixMarginExpression(this, marginPosition);
		}

		protected override void AddInScopeTextBox(TextBox textbox)
		{
			if (m_inScopeTextBoxes == null)
			{
				m_inScopeTextBoxes = new List<TextBox>();
			}
			m_inScopeTextBoxes.Add(textbox);
		}

		internal override void ResetTextBoxImpls(OnDemandProcessingContext context)
		{
			if (m_inScopeTextBoxes != null)
			{
				for (int i = 0; i < m_inScopeTextBoxes.Count; i++)
				{
					m_inScopeTextBoxes[i].ResetTextBoxImpl(context);
				}
			}
		}
	}
}
