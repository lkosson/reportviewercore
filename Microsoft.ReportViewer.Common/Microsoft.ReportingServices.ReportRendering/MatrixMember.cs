using Microsoft.ReportingServices.ReportProcessing;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Microsoft.ReportingServices.ReportRendering
{
	internal sealed class MatrixMember : Group, IDocumentMapEntry
	{
		public enum SortOrders
		{
			None,
			Ascending,
			Descending
		}

		private MatrixHeading m_headingDef;

		private MatrixHeadingInstance m_headingInstance;

		private MatrixHeadingInstanceInfo m_headingInstanceInfo;

		private ReportItem m_reportItem;

		private MatrixMemberCollection m_children;

		private MatrixMember m_parent;

		private ReportSize m_width;

		private ReportSize m_height;

		private bool m_isSubtotal;

		private bool m_isParentSubTotal;

		private int m_index;

		private int m_cachedMemberCellIndex = -1;

		public override string ID
		{
			get
			{
				if (m_isSubtotal)
				{
					if (m_headingDef.Subtotal.RenderingModelID == null)
					{
						m_headingDef.Subtotal.RenderingModelID = m_headingDef.Subtotal.ID.ToString(CultureInfo.InvariantCulture);
					}
					return m_headingDef.Subtotal.RenderingModelID;
				}
				if (m_headingDef.Grouping == null)
				{
					if (m_headingDef.RenderingModelIDs == null)
					{
						m_headingDef.RenderingModelIDs = new string[m_headingDef.ReportItems.Count];
					}
					if (m_headingDef.RenderingModelIDs[m_index] == null)
					{
						m_headingDef.RenderingModelIDs[m_index] = m_headingDef.IDs[m_index].ToString(CultureInfo.InvariantCulture);
					}
					return m_headingDef.RenderingModelIDs[m_index];
				}
				if (m_headingDef.RenderingModelID == null)
				{
					m_headingDef.RenderingModelID = m_headingDef.ID.ToString(CultureInfo.InvariantCulture);
				}
				return m_headingDef.RenderingModelID;
			}
		}

		public object SharedRenderingInfo
		{
			get
			{
				int num = m_isSubtotal ? m_headingDef.Subtotal.ID : ((m_headingDef.Grouping != null) ? m_headingDef.ID : m_headingDef.IDs[m_index]);
				return base.OwnerDataRegion.RenderingContext.RenderingInfoManager.SharedRenderingInfo[num];
			}
			set
			{
				int num = m_isSubtotal ? m_headingDef.Subtotal.ID : ((m_headingDef.Grouping != null) ? m_headingDef.ID : m_headingDef.IDs[m_index]);
				base.OwnerDataRegion.RenderingContext.RenderingInfoManager.SharedRenderingInfo[num] = value;
			}
		}

		internal ReportSize Size
		{
			get
			{
				if (m_headingDef.SizeForRendering == null)
				{
					m_headingDef.SizeForRendering = new ReportSize(m_headingDef.Size, m_headingDef.SizeValue);
				}
				return m_headingDef.SizeForRendering;
			}
		}

		internal override TextBox ToggleParent
		{
			get
			{
				if (m_isSubtotal || IsStatic)
				{
					return null;
				}
				if (Visibility.HasToggle(m_visibilityDef))
				{
					return base.OwnerDataRegion.RenderingContext.GetToggleParent(m_uniqueName);
				}
				return null;
			}
		}

		public override bool HasToggle
		{
			get
			{
				if (m_isSubtotal || IsStatic)
				{
					return false;
				}
				return Visibility.HasToggle(m_visibilityDef);
			}
		}

		public override string ToggleItem
		{
			get
			{
				if (m_isSubtotal || IsStatic)
				{
					return null;
				}
				return base.ToggleItem;
			}
		}

		public override SharedHiddenState SharedHidden
		{
			get
			{
				if (m_isSubtotal)
				{
					if (!m_headingDef.Subtotal.AutoDerived)
					{
						return SharedHiddenState.Never;
					}
					return SharedHiddenState.Always;
				}
				if (IsStatic)
				{
					return SharedHiddenState.Never;
				}
				return Visibility.GetSharedHidden(m_visibilityDef);
			}
		}

		public override bool IsToggleChild
		{
			get
			{
				if (m_isSubtotal || IsStatic)
				{
					return false;
				}
				return base.OwnerDataRegion.RenderingContext.IsToggleChild(m_uniqueName);
			}
		}

		public override bool Hidden
		{
			get
			{
				if (m_isSubtotal)
				{
					return m_headingDef.Subtotal.AutoDerived;
				}
				if (m_headingInstance == null)
				{
					return RenderingContext.GetDefinitionHidden(m_headingDef.Visibility);
				}
				if (m_headingDef.Visibility == null)
				{
					return false;
				}
				if (m_headingDef.Visibility.Toggle != null)
				{
					return base.OwnerDataRegion.RenderingContext.IsItemHidden(m_headingInstance.UniqueName, potentialSender: false);
				}
				return InstanceInfo.StartHidden;
			}
		}

		public override CustomPropertyCollection CustomProperties
		{
			get
			{
				CustomPropertyCollection customPropertyCollection = m_customProperties;
				if (m_customProperties == null)
				{
					if (m_headingDef.Grouping == null || m_headingDef.Grouping.CustomProperties == null)
					{
						return null;
					}
					customPropertyCollection = ((m_headingInstance != null) ? new CustomPropertyCollection(m_headingDef.Grouping.CustomProperties, InstanceInfo.CustomPropertyInstances) : new CustomPropertyCollection(m_headingDef.Grouping.CustomProperties, null));
					if (base.OwnerDataRegion.RenderingContext.CacheState)
					{
						m_customProperties = customPropertyCollection;
					}
				}
				return customPropertyCollection;
			}
		}

		public ReportItem ReportItem
		{
			get
			{
				ReportItem reportItem = m_reportItem;
				if (m_reportItem == null)
				{
					Microsoft.ReportingServices.ReportProcessing.ReportItem reportItem2 = null;
					ReportItemInstance reportItemInstance = null;
					NonComputedUniqueNames nonComputedUniqueNames = null;
					reportItem2 = (m_isSubtotal ? m_headingDef.Subtotal.ReportItem : ((m_headingDef.Grouping != null) ? m_headingDef.ReportItem : m_headingDef.ReportItems[m_index]));
					if (m_headingInstance != null)
					{
						nonComputedUniqueNames = InstanceInfo.ContentUniqueNames;
						reportItemInstance = m_headingInstance.Content;
					}
					if (reportItem2 != null)
					{
						reportItem = ReportItem.CreateItem(0, reportItem2, reportItemInstance, base.OwnerDataRegion.RenderingContext, nonComputedUniqueNames);
					}
					if (base.OwnerDataRegion.RenderingContext.CacheState)
					{
						m_reportItem = reportItem;
					}
				}
				return reportItem;
			}
		}

		public override string Label
		{
			get
			{
				string result = null;
				if (m_groupingDef != null && m_groupingDef.GroupLabel != null)
				{
					result = ((m_groupingDef.GroupLabel.Type == ExpressionInfo.Types.Constant) ? m_groupingDef.GroupLabel.Value : ((m_headingInstance != null) ? InstanceInfo.Label : null));
				}
				return result;
			}
		}

		public bool InDocumentMap
		{
			get
			{
				if (m_headingInstance != null && m_groupingDef != null && m_groupingDef.GroupLabel != null)
				{
					return !m_isSubtotal;
				}
				return false;
			}
		}

		public MatrixMember Parent => m_parent;

		public MatrixMemberCollection Children
		{
			get
			{
				MatrixHeading matrixHeading = m_headingDef.SubHeading;
				if (matrixHeading == null)
				{
					return null;
				}
				MatrixMemberCollection matrixMemberCollection = m_children;
				if (m_children == null)
				{
					MatrixHeadingInstanceList headingInstances = null;
					if (m_headingInstance != null)
					{
						if (m_headingInstance.SubHeadingInstances == null || m_headingInstance.SubHeadingInstances.Count == 0)
						{
							return m_children;
						}
						headingInstances = m_headingInstance.SubHeadingInstances;
						if (m_headingInstance.IsSubtotal)
						{
							matrixHeading = (MatrixHeading)m_headingDef.GetInnerStaticHeading();
						}
					}
					else if (m_isSubtotal)
					{
						return m_children;
					}
					List<int> memberMapping = Matrix.CalculateMapping(matrixHeading, headingInstances, m_isSubtotal || m_isParentSubTotal);
					matrixMemberCollection = new MatrixMemberCollection((Matrix)base.OwnerDataRegion, this, matrixHeading, headingInstances, memberMapping, m_isSubtotal);
					if (base.OwnerDataRegion.RenderingContext.CacheState)
					{
						m_children = matrixMemberCollection;
					}
				}
				return matrixMemberCollection;
			}
		}

		public int MemberCellIndex
		{
			get
			{
				if (m_headingInstance == null)
				{
					if (m_headingDef.Grouping == null)
					{
						return m_index;
					}
					return 0;
				}
				return InstanceInfo.HeadingCellIndex;
			}
		}

		internal int CachedMemberCellIndex
		{
			get
			{
				if (m_cachedMemberCellIndex < 0)
				{
					m_cachedMemberCellIndex = MemberCellIndex;
				}
				return m_cachedMemberCellIndex;
			}
		}

		public int ColumnSpan
		{
			get
			{
				MatrixHeading matrixHeading = null;
				Microsoft.ReportingServices.ReportProcessing.Matrix matrix = (Microsoft.ReportingServices.ReportProcessing.Matrix)base.OwnerDataRegion.ReportItemDef;
				if (m_headingDef.IsColumn)
				{
					if (m_headingInstance != null)
					{
						return InstanceInfo.HeadingSpan;
					}
					matrixHeading = (MatrixHeading)m_headingDef.GetInnerStaticHeading();
					if (matrixHeading != null)
					{
						return matrix.MatrixColumns.Count;
					}
				}
				else if (m_isSubtotal || m_isParentSubTotal)
				{
					return m_headingDef.SubtotalSpan;
				}
				return 1;
			}
		}

		public int RowSpan
		{
			get
			{
				MatrixHeading matrixHeading = null;
				Microsoft.ReportingServices.ReportProcessing.Matrix matrix = (Microsoft.ReportingServices.ReportProcessing.Matrix)base.OwnerDataRegion.ReportItemDef;
				if (m_headingDef.IsColumn)
				{
					if (m_isSubtotal || m_isParentSubTotal)
					{
						return m_headingDef.SubtotalSpan;
					}
				}
				else
				{
					if (m_headingInstance != null)
					{
						return InstanceInfo.HeadingSpan;
					}
					matrixHeading = (MatrixHeading)m_headingDef.GetInnerStaticHeading();
					if (matrixHeading != null)
					{
						return matrix.MatrixRows.Count;
					}
				}
				return 1;
			}
		}

		public bool IsTotal => m_isSubtotal;

		public bool IsStatic
		{
			get
			{
				if (m_headingDef.Grouping == null)
				{
					return true;
				}
				return false;
			}
		}

		public ReportSize Width
		{
			get
			{
				ReportSize reportSize = m_width;
				if (m_width == null)
				{
					if (m_headingDef.IsColumn)
					{
						double num = 0.0;
						SizeCollection cellWidths = ((Matrix)base.OwnerDataRegion).CellWidths;
						MatrixHeading matrixHeading = (MatrixHeading)m_headingDef.GetInnerStaticHeading();
						if (matrixHeading == null)
						{
							num = (double)ColumnSpan * cellWidths[MemberCellIndex].ToMillimeters();
							num = Math.Round(num, Validator.DecimalPrecision);
						}
						else
						{
							for (int i = 0; i < ColumnSpan; i++)
							{
								num += cellWidths[MemberCellIndex + i].ToMillimeters();
								num = Math.Round(num, Validator.DecimalPrecision);
							}
						}
						reportSize = new ReportSize(num + "mm", num);
					}
					else if ((m_isSubtotal || m_isParentSubTotal) && 1 != m_headingDef.SubtotalSpan)
					{
						double num2 = 0.0;
						MatrixHeading matrixHeading2 = m_headingDef;
						for (int j = 0; j < m_headingDef.SubtotalSpan; j++)
						{
							num2 += matrixHeading2.SizeValue;
							num2 = Math.Round(num2, Validator.DecimalPrecision);
							matrixHeading2 = matrixHeading2.SubHeading;
						}
						reportSize = new ReportSize(num2 + "mm", num2);
					}
					else
					{
						reportSize = Size;
					}
					if (base.OwnerDataRegion.RenderingContext.CacheState)
					{
						m_width = reportSize;
					}
				}
				return reportSize;
			}
		}

		public ReportSize Height
		{
			get
			{
				ReportSize reportSize = m_height;
				if (m_height == null)
				{
					if (m_headingDef.IsColumn)
					{
						if ((m_isSubtotal || m_isParentSubTotal) && 1 != m_headingDef.SubtotalSpan)
						{
							double num = 0.0;
							MatrixHeading matrixHeading = m_headingDef;
							for (int i = 0; i < m_headingDef.SubtotalSpan; i++)
							{
								num += matrixHeading.SizeValue;
								num = Math.Round(num, Validator.DecimalPrecision);
								matrixHeading = matrixHeading.SubHeading;
							}
							reportSize = new ReportSize(num + "mm", num);
						}
						else
						{
							reportSize = Size;
						}
					}
					else
					{
						double num2 = 0.0;
						SizeCollection cellHeights = ((Matrix)base.OwnerDataRegion).CellHeights;
						MatrixHeading matrixHeading2 = (MatrixHeading)m_headingDef.GetInnerStaticHeading();
						if (matrixHeading2 == null)
						{
							num2 = (double)RowSpan * cellHeights[MemberCellIndex].ToMillimeters();
							num2 = Math.Round(num2, Validator.DecimalPrecision);
						}
						else
						{
							for (int j = 0; j < RowSpan; j++)
							{
								num2 += cellHeights[MemberCellIndex + j].ToMillimeters();
								num2 = Math.Round(num2, Validator.DecimalPrecision);
							}
						}
						reportSize = new ReportSize(num2 + "mm", num2);
					}
					if (base.OwnerDataRegion.RenderingContext.CacheState)
					{
						m_height = reportSize;
					}
				}
				return reportSize;
			}
		}

		public object GroupValue
		{
			get
			{
				if (!m_isSubtotal && m_headingDef.OwcGroupExpression)
				{
					return InstanceInfo.GroupExpressionValue;
				}
				return null;
			}
		}

		public SortOrders SortOrder
		{
			get
			{
				SortOrders result = SortOrders.None;
				if (!IsStatic)
				{
					BoolList boolList = (m_headingDef.Sorting == null) ? m_headingDef.Grouping.SortDirections : m_headingDef.Sorting.SortDirections;
					if (boolList != null && 0 < boolList.Count)
					{
						result = (boolList[0] ? SortOrders.Ascending : SortOrders.Descending);
					}
				}
				return result;
			}
		}

		public override string DataElementName
		{
			get
			{
				if (IsTotal)
				{
					return m_headingDef.Subtotal.DataElementName;
				}
				if (IsStatic)
				{
					return m_headingDef.ReportItems[m_index].DataElementName;
				}
				return base.DataElementName;
			}
		}

		public override DataElementOutputTypes DataElementOutput
		{
			get
			{
				Global.Tracer.Assert(!IsTotal || !IsStatic);
				if (IsTotal)
				{
					return m_headingDef.Subtotal.DataElementOutput;
				}
				if (IsStatic)
				{
					return DataElementOutputForStatic(null);
				}
				return base.DataElementOutput;
			}
		}

		internal MatrixHeadingInstanceInfo InstanceInfo
		{
			get
			{
				if (m_headingInstance == null)
				{
					return null;
				}
				if (m_headingInstanceInfo == null)
				{
					m_headingInstanceInfo = m_headingInstance.GetInstanceInfo(base.OwnerDataRegion.RenderingContext.ChunkManager);
				}
				return m_headingInstanceInfo;
			}
		}

		internal bool IsParentSubtotal => m_isParentSubTotal;

		internal MatrixMember(Matrix owner, MatrixMember parent, MatrixHeading headingDef, MatrixHeadingInstance headingInstance, bool isSubtotal, bool isParentSubTotal, int index)
			: base(owner, headingDef.Grouping, headingDef.Visibility)
		{
			m_parent = parent;
			m_headingDef = headingDef;
			m_headingInstance = headingInstance;
			m_isSubtotal = isSubtotal;
			m_isParentSubTotal = isParentSubTotal;
			m_index = index;
			if (m_headingInstance != null)
			{
				m_uniqueName = m_headingInstance.UniqueName;
			}
		}

		public DataElementOutputTypes DataElementOutputForStatic(MatrixMember staticHeading)
		{
			if (!IsStatic)
			{
				return DataElementOutput;
			}
			if (staticHeading != null && (!staticHeading.IsStatic || staticHeading.Parent == Parent))
			{
				staticHeading = null;
			}
			if (staticHeading != null)
			{
				int index;
				int index2;
				if (m_headingDef.IsColumn)
				{
					index = staticHeading.m_index;
					index2 = m_index;
				}
				else
				{
					index = m_index;
					index2 = staticHeading.m_index;
				}
				return GetDataElementOutputTypeFromCell(index, index2);
			}
			Microsoft.ReportingServices.ReportProcessing.Matrix matrix = (Microsoft.ReportingServices.ReportProcessing.Matrix)base.OwnerDataRegion.ReportItemDef;
			if (matrix.PivotStaticColumns == null || matrix.PivotStaticRows == null)
			{
				return GetDataElementOutputTypeFromCell(0, m_index);
			}
			Global.Tracer.Assert(matrix.PivotStaticColumns != null && matrix.PivotStaticRows != null);
			return GetDataElementOutputTypeForRowCol(m_index);
		}

		public bool IsRowMemberOnThisPage(int memberIndex, int pageNumber, out int startPage, out int endPage)
		{
			startPage = -1;
			endPage = -1;
			RenderingPagesRangesList childrenStartAndEndPages = m_headingInstance.ChildrenStartAndEndPages;
			if (childrenStartAndEndPages == null)
			{
				return true;
			}
			Global.Tracer.Assert(memberIndex >= 0 && memberIndex < childrenStartAndEndPages.Count);
			if (memberIndex >= childrenStartAndEndPages.Count)
			{
				return false;
			}
			RenderingPagesRanges renderingPagesRanges = childrenStartAndEndPages[memberIndex];
			startPage = renderingPagesRanges.StartPage;
			endPage = renderingPagesRanges.EndPage;
			if (pageNumber >= startPage)
			{
				return pageNumber <= endPage;
			}
			return false;
		}

		private DataElementOutputTypes GetDataElementOutputTypeFromCell(int rowIndex, int columnIndex)
		{
			return ((Microsoft.ReportingServices.ReportProcessing.Matrix)base.OwnerDataRegion.ReportItemDef).GetCellReportItem(rowIndex, columnIndex).DataElementOutput;
		}

		private DataElementOutputTypes GetDataElementOutputTypeForRowCol(int index)
		{
			Microsoft.ReportingServices.ReportProcessing.Matrix matrix = (Microsoft.ReportingServices.ReportProcessing.Matrix)base.OwnerDataRegion.ReportItemDef;
			int num;
			int num2;
			int count;
			if (m_headingDef.IsColumn)
			{
				num = 0;
				num2 = index;
				count = matrix.MatrixRows.Count;
			}
			else
			{
				num = index;
				num2 = 0;
				count = matrix.MatrixColumns.Count;
			}
			while (true)
			{
				if (matrix.GetCellReportItem(num, num2).DataElementOutput != DataElementOutputTypes.NoOutput)
				{
					return DataElementOutputTypes.Output;
				}
				if (m_headingDef.IsColumn)
				{
					num++;
					if (num >= count)
					{
						break;
					}
				}
				else
				{
					num2++;
					if (num2 >= count)
					{
						break;
					}
				}
			}
			return DataElementOutputTypes.NoOutput;
		}

		public void GetChildRowMembersOnPage(int page, out int startChild, out int endChild)
		{
			startChild = -1;
			endChild = -1;
			if (m_headingInstance != null)
			{
				RenderingPagesRangesList childrenStartAndEndPages = m_headingInstance.ChildrenStartAndEndPages;
				if (childrenStartAndEndPages != null)
				{
					RenderingContext.FindRange(childrenStartAndEndPages, 0, childrenStartAndEndPages.Count - 1, page, ref startChild, ref endChild);
				}
			}
		}
	}
}
