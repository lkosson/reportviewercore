using Microsoft.ReportingServices.ReportIntermediateFormat;
using Microsoft.ReportingServices.ReportProcessing;
using Microsoft.ReportingServices.ReportRendering;
using System;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class Tablix : DataRegion, IPageBreakItem
	{
		private TablixCorner m_corner;

		private TablixHierarchy m_columns;

		private TablixHierarchy m_rows;

		private TablixBody m_body;

		private int[] m_matrixRowDefinitionMapping;

		private int[] m_matrixColDefinitionMapping;

		private int m_memberCellDefinitionIndex;

		private MatrixMemberInfoCache m_matrixMemberColIndexes;

		private PageBreakLocation? m_propagatedPageBreak;

		private BandLayoutOptions m_bandLayout;

		private ReportSizeProperty m_topMargin;

		private ReportSizeProperty m_bottomMargin;

		private ReportSizeProperty m_leftMargin;

		private ReportSizeProperty m_rightMargin;

		public override DataElementOutputTypes DataElementOutput
		{
			get
			{
				if (m_isOldSnapshot && Type.List == m_snapshotDataRegionType && base.DataElementOutput == DataElementOutputTypes.Output)
				{
					return DataElementOutputTypes.ContentsOnly;
				}
				return base.DataElementOutput;
			}
		}

		public bool CanScroll
		{
			get
			{
				if (m_isOldSnapshot)
				{
					return false;
				}
				return TablixDef.CanScroll;
			}
		}

		public bool KeepTogether
		{
			get
			{
				if (m_isOldSnapshot)
				{
					return ((Microsoft.ReportingServices.ReportRendering.DataRegion)m_renderReportItem).KeepTogether;
				}
				return TablixDef.KeepTogether;
			}
		}

		public TablixLayoutDirection LayoutDirection
		{
			get
			{
				if (m_isOldSnapshot)
				{
					if (Type.Matrix == m_snapshotDataRegionType)
					{
						return (TablixLayoutDirection)RenderMatrix.LayoutDirection;
					}
					return TablixLayoutDirection.LTR;
				}
				if (TablixDef.LayoutDirection)
				{
					return TablixLayoutDirection.RTL;
				}
				return TablixLayoutDirection.LTR;
			}
		}

		public TablixCorner Corner
		{
			get
			{
				if (m_corner == null)
				{
					m_corner = new TablixCorner(this);
				}
				return m_corner;
			}
		}

		public TablixHierarchy ColumnHierarchy
		{
			get
			{
				if (m_columns == null)
				{
					if (m_isOldSnapshot)
					{
						m_columns = new TablixHierarchy(this, isColumn: true);
					}
					else
					{
						m_columns = new TablixHierarchy(this, isColumn: true);
					}
				}
				return m_columns;
			}
		}

		public TablixHierarchy RowHierarchy
		{
			get
			{
				if (m_rows == null)
				{
					if (m_isOldSnapshot)
					{
						m_rows = new TablixHierarchy(this, isColumn: false);
					}
					else
					{
						m_rows = new TablixHierarchy(this, isColumn: false);
					}
				}
				return m_rows;
			}
		}

		public TablixBody Body
		{
			get
			{
				if (m_body == null)
				{
					m_body = new TablixBody(this);
				}
				return m_body;
			}
		}

		public int Columns
		{
			get
			{
				if (m_isOldSnapshot)
				{
					switch (m_snapshotDataRegionType)
					{
					case Type.List:
						return 0;
					case Type.Table:
						return 0;
					case Type.Matrix:
						return RenderMatrix.Columns;
					}
				}
				return TablixDef.ColumnHeaderRowCount;
			}
		}

		public int Rows
		{
			get
			{
				if (m_isOldSnapshot)
				{
					switch (m_snapshotDataRegionType)
					{
					case Type.List:
						return 0;
					case Type.Table:
						return 0;
					case Type.Matrix:
						return RenderMatrix.Rows;
					}
				}
				return TablixDef.RowHeaderColumnCount;
			}
		}

		public int GroupsBeforeRowHeaders
		{
			get
			{
				if (m_isOldSnapshot)
				{
					if (Type.Matrix == m_snapshotDataRegionType)
					{
						return RenderMatrix.GroupsBeforeRowHeaders;
					}
					return 0;
				}
				return TablixDef.GroupsBeforeRowHeaders;
			}
		}

		public bool RepeatRowHeaders
		{
			get
			{
				if (m_isOldSnapshot)
				{
					return Type.Matrix == m_snapshotDataRegionType;
				}
				return TablixDef.RepeatRowHeaders;
			}
		}

		public bool RepeatColumnHeaders
		{
			get
			{
				if (m_isOldSnapshot)
				{
					return Type.Matrix == m_snapshotDataRegionType;
				}
				return TablixDef.RepeatColumnHeaders;
			}
		}

		public bool FixedRowHeaders
		{
			get
			{
				if (m_isOldSnapshot)
				{
					if (Type.Matrix == m_snapshotDataRegionType)
					{
						return RenderMatrix.RowGroupingFixedHeader;
					}
					if (Type.Table == m_snapshotDataRegionType)
					{
						if (RenderTable.FixedHeader)
						{
							return !RenderTable.HasFixedColumnHeaders;
						}
						return false;
					}
					return false;
				}
				return TablixDef.FixedRowHeaders;
			}
		}

		public bool FixedColumnHeaders
		{
			get
			{
				if (m_isOldSnapshot)
				{
					if (Type.Matrix == m_snapshotDataRegionType)
					{
						return RenderMatrix.ColumnGroupingFixedHeader;
					}
					if (Type.Table == m_snapshotDataRegionType)
					{
						if (RenderTable.FixedHeader)
						{
							return RenderTable.HasFixedColumnHeaders;
						}
						return false;
					}
					return false;
				}
				return TablixDef.FixedColumnHeaders;
			}
		}

		public bool OmitBorderOnPageBreak
		{
			get
			{
				if (m_isOldSnapshot)
				{
					return false;
				}
				return TablixDef.OmitBorderOnPageBreak;
			}
		}

		public BandLayoutOptions BandLayout
		{
			get
			{
				if (m_isOldSnapshot || TablixDef.BandLayout == null)
				{
					return null;
				}
				if (m_bandLayout == null)
				{
					m_bandLayout = new BandLayoutOptions(TablixDef.BandLayout);
				}
				return m_bandLayout;
			}
		}

		public ReportSizeProperty TopMargin => GetOrCreateMarginProperty(ref m_topMargin, TablixDef.TopMargin);

		public ReportSizeProperty BottomMargin => GetOrCreateMarginProperty(ref m_bottomMargin, TablixDef.BottomMargin);

		public ReportSizeProperty LeftMargin => GetOrCreateMarginProperty(ref m_leftMargin, TablixDef.LeftMargin);

		public ReportSizeProperty RightMargin => GetOrCreateMarginProperty(ref m_rightMargin, TablixDef.RightMargin);

		[Obsolete("Use PageBreak.BreakLocation instead.")]
		PageBreakLocation IPageBreakItem.PageBreakLocation
		{
			get
			{
				if (m_isOldSnapshot)
				{
					if (!m_propagatedPageBreak.HasValue)
					{
						m_propagatedPageBreak = PageBreakLocation.None;
						_ = RowHierarchy.MemberCollection;
					}
					return m_propagatedPageBreak.Value;
				}
				PageBreak pageBreak = base.PageBreak;
				if (pageBreak.HasEnabledInstance)
				{
					return pageBreak.BreakLocation;
				}
				return PageBreakLocation.None;
			}
		}

		public bool HideStaticsIfNoRows
		{
			get
			{
				if (m_isOldSnapshot)
				{
					if (Type.Table == m_snapshotDataRegionType)
					{
						if (RenderTable.TableHeader == null)
						{
							return RenderTable.TableFooter == null;
						}
						return false;
					}
					return true;
				}
				return TablixDef.HideStaticsIfNoRows;
			}
		}

		public override ReportSize Width
		{
			get
			{
				if (m_isOldSnapshot && Type.Matrix == m_snapshotDataRegionType && m_cachedWidth == null)
				{
					TablixHierarchy columnHierarchy = ColumnHierarchy;
					if (columnHierarchy != null && columnHierarchy.MemberCollection != null)
					{
						SetCachedWidth(columnHierarchy.MemberCollection.SizeDelta);
					}
				}
				return base.Width;
			}
		}

		public override ReportSize Height
		{
			get
			{
				if (m_isOldSnapshot && Type.Matrix == m_snapshotDataRegionType && m_cachedHeight == null)
				{
					TablixHierarchy rowHierarchy = RowHierarchy;
					if (rowHierarchy != null && rowHierarchy.MemberCollection != null)
					{
						SetCachedHeight(rowHierarchy.MemberCollection.SizeDelta);
					}
				}
				return base.Height;
			}
		}

		internal Microsoft.ReportingServices.ReportIntermediateFormat.Tablix TablixDef => (Microsoft.ReportingServices.ReportIntermediateFormat.Tablix)m_reportItemDef;

		internal override bool HasDataCells
		{
			get
			{
				if (m_body != null)
				{
					return m_body.HasRowCollection;
				}
				return false;
			}
		}

		internal override IDataRegionRowCollection RowCollection
		{
			get
			{
				if (m_body != null)
				{
					return m_body.RowCollection;
				}
				return null;
			}
		}

		internal Type SnapshotTablixType => m_snapshotDataRegionType;

		internal Microsoft.ReportingServices.ReportRendering.List RenderList
		{
			get
			{
				if (!m_isOldSnapshot || Type.List != m_snapshotDataRegionType)
				{
					throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidOperation);
				}
				return (Microsoft.ReportingServices.ReportRendering.List)m_renderReportItem;
			}
		}

		internal Microsoft.ReportingServices.ReportRendering.Table RenderTable
		{
			get
			{
				if (!m_isOldSnapshot || Type.Table != m_snapshotDataRegionType)
				{
					throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidOperation);
				}
				return (Microsoft.ReportingServices.ReportRendering.Table)m_renderReportItem;
			}
		}

		internal Microsoft.ReportingServices.ReportRendering.Matrix RenderMatrix
		{
			get
			{
				if (!m_isOldSnapshot || Type.Matrix != m_snapshotDataRegionType)
				{
					throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidOperation);
				}
				return (Microsoft.ReportingServices.ReportRendering.Matrix)m_renderReportItem;
			}
		}

		internal int[] MatrixRowDefinitionMapping
		{
			get
			{
				if (m_isOldSnapshot && Type.Matrix == m_snapshotDataRegionType && m_matrixRowDefinitionMapping == null)
				{
					m_matrixRowDefinitionMapping = CalculateMatrixDefinitionMapping(((Microsoft.ReportingServices.ReportProcessing.Matrix)RenderMatrix.ReportItemDef).Rows);
				}
				return m_matrixRowDefinitionMapping;
			}
		}

		internal int[] MatrixColDefinitionMapping
		{
			get
			{
				if (m_isOldSnapshot && Type.Matrix == m_snapshotDataRegionType && m_matrixColDefinitionMapping == null)
				{
					m_matrixColDefinitionMapping = CalculateMatrixDefinitionMapping(((Microsoft.ReportingServices.ReportProcessing.Matrix)RenderMatrix.ReportItemDef).Columns);
				}
				return m_matrixColDefinitionMapping;
			}
		}

		internal MatrixMemberInfoCache MatrixMemberColIndexes
		{
			get
			{
				return m_matrixMemberColIndexes;
			}
			set
			{
				m_matrixMemberColIndexes = value;
			}
		}

		internal Tablix(IDefinitionPath parentDefinitionPath, int indexIntoParentCollectionDef, Microsoft.ReportingServices.ReportIntermediateFormat.Tablix reportItemDef, RenderingContext renderingContext)
			: base(parentDefinitionPath, indexIntoParentCollectionDef, reportItemDef, renderingContext)
		{
		}

		internal Tablix(IDefinitionPath parentDefinitionPath, int indexIntoParentCollectionDef, bool inSubtotal, Microsoft.ReportingServices.ReportRendering.List renderList, RenderingContext renderingContext)
			: base(parentDefinitionPath, indexIntoParentCollectionDef, inSubtotal, renderList, renderingContext)
		{
			m_snapshotDataRegionType = Type.List;
		}

		internal Tablix(IDefinitionPath parentDefinitionPath, int indexIntoParentCollection, bool inSubtotal, Microsoft.ReportingServices.ReportRendering.Table renderTable, RenderingContext renderingContext)
			: base(parentDefinitionPath, indexIntoParentCollection, inSubtotal, renderTable, renderingContext)
		{
			m_snapshotDataRegionType = Type.Table;
		}

		internal Tablix(IDefinitionPath parentDefinitionPath, int indexIntoParentCollection, bool inSubtotal, Microsoft.ReportingServices.ReportRendering.Matrix renderMatrix, RenderingContext renderingContext)
			: base(parentDefinitionPath, indexIntoParentCollection, inSubtotal, renderMatrix, renderingContext)
		{
			m_snapshotDataRegionType = Type.Matrix;
		}

		private ReportSizeProperty GetOrCreateMarginProperty(ref ReportSizeProperty property, Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			if (m_isOldSnapshot || expression == null)
			{
				return null;
			}
			if (property == null)
			{
				property = new ReportSizeProperty(expression);
			}
			return property;
		}

		internal void SetPageBreakLocation(PageBreakLocation pageBreakLocation)
		{
			if (m_isOldSnapshot)
			{
				m_propagatedPageBreak = pageBreakLocation;
			}
		}

		internal override ReportItemInstance GetOrCreateInstance()
		{
			if (m_instance == null)
			{
				m_instance = new TablixInstance(this);
			}
			return m_instance;
		}

		internal override void UpdateRenderReportItem(Microsoft.ReportingServices.ReportRendering.ReportItem renderReportItem)
		{
			base.UpdateRenderReportItem(renderReportItem);
			if (renderReportItem != null)
			{
				m_matrixRowDefinitionMapping = null;
				m_matrixColDefinitionMapping = null;
				if (Type.Matrix == m_snapshotDataRegionType && m_corner != null)
				{
					m_corner.ResetContext();
				}
				if (m_rows != null)
				{
					m_rows.ResetContext(clearCache: true);
				}
				if (m_columns != null)
				{
					m_columns.ResetContext(clearCache: true);
				}
			}
			else
			{
				if (Type.Matrix == m_snapshotDataRegionType && m_corner != null)
				{
					m_corner.ResetContext();
				}
				if (m_rows != null)
				{
					m_rows.ResetContext(clearCache: false);
				}
				if (m_columns != null)
				{
					m_columns.ResetContext(clearCache: false);
				}
			}
		}

		internal int GetCurrentMemberCellDefinitionIndex()
		{
			return m_memberCellDefinitionIndex;
		}

		internal int GetAndIncrementMemberCellDefinitionIndex()
		{
			return m_memberCellDefinitionIndex++;
		}

		internal void ResetMemberCellDefinitionIndex(int startIndex)
		{
			m_memberCellDefinitionIndex = startIndex;
		}

		private int[] CalculateMatrixDefinitionMapping(MatrixHeading heading)
		{
			List<int> list = new List<int>();
			int definitionIndex = 0;
			AddInnerHierarchy(heading, list, ref definitionIndex);
			return list.ToArray();
		}

		private void AddInnerHierarchy(MatrixHeading heading, List<int> mapping, ref int definitionIndex)
		{
			if (heading == null)
			{
				mapping.Add(definitionIndex++);
			}
			else if (heading.Grouping == null)
			{
				AddInnerStatics(heading, mapping, ref definitionIndex);
			}
			else if (heading.Subtotal != null)
			{
				int num = definitionIndex;
				if (Subtotal.PositionType.Before == heading.Subtotal.Position)
				{
					AddInnerStatics(heading, mapping, ref definitionIndex);
					definitionIndex = num;
					AddInnerHierarchy(heading.SubHeading, mapping, ref definitionIndex);
				}
				else
				{
					AddInnerHierarchy(heading.SubHeading, mapping, ref definitionIndex);
					definitionIndex = num;
					AddInnerStatics(heading, mapping, ref definitionIndex);
				}
			}
			else
			{
				AddInnerHierarchy(heading.SubHeading, mapping, ref definitionIndex);
			}
		}

		private void AddInnerStatics(MatrixHeading heading, List<int> mapping, ref int definitionIndex)
		{
			if (heading == null)
			{
				mapping.Add(definitionIndex++);
			}
			else if (heading.Grouping == null)
			{
				int count = heading.ReportItems.Count;
				for (int i = 0; i < count; i++)
				{
					AddInnerHierarchy(heading.SubHeading, mapping, ref definitionIndex);
				}
			}
			else
			{
				AddInnerHierarchy(heading.SubHeading, mapping, ref definitionIndex);
			}
		}

		internal override void SetNewContextChildren()
		{
			if (m_corner != null)
			{
				m_corner.SetNewContext();
			}
			if (m_rows != null)
			{
				m_rows.SetNewContext();
			}
			if (m_columns != null)
			{
				m_columns.SetNewContext();
			}
			if (m_reportItemDef != null)
			{
				((Microsoft.ReportingServices.ReportIntermediateFormat.Tablix)m_reportItemDef).ResetTextBoxImpls(m_renderingContext.OdpContext);
			}
		}
	}
}
