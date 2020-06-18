using Microsoft.ReportingServices.ReportRendering;
using System;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class ShimMatrixMember : ShimTablixMember
	{
		private int m_definitionStartIndex = -1;

		private int m_definitionEndIndex = -1;

		private int m_renderCollectionIndex = -1;

		private bool m_isAfterSubtotal;

		internal MatrixMember m_staticOrSubtotal;

		private MatrixMemberInfoCache m_currentMatrixMemberCellIndexes;

		internal double SizeDelta
		{
			get
			{
				if (m_children != null)
				{
					return m_children.SizeDelta;
				}
				return 0.0;
			}
		}

		public override bool HideIfNoRows
		{
			get
			{
				if ((m_parent == null || m_parent.IsStatic) && m_staticOrSubtotal != null && m_staticOrSubtotal.IsTotal)
				{
					return true;
				}
				return base.HideIfNoRows;
			}
		}

		public override string DataElementName
		{
			get
			{
				if (m_staticOrSubtotal != null)
				{
					return m_staticOrSubtotal.DataElementName;
				}
				return base.DataElementName;
			}
		}

		public override DataElementOutputTypes DataElementOutput
		{
			get
			{
				if (m_staticOrSubtotal != null)
				{
					return (DataElementOutputTypes)m_staticOrSubtotal.DataElementOutput;
				}
				return base.DataElementOutput;
			}
		}

		public override TablixHeader TablixHeader
		{
			get
			{
				if (m_header == null)
				{
					m_header = new TablixHeader(base.OwnerTablix, this);
				}
				return m_header;
			}
		}

		public override TablixMemberCollection Children => m_children;

		public override bool FixedData => false;

		public override bool IsStatic => m_staticOrSubtotal != null;

		internal override int RowSpan
		{
			get
			{
				if (m_isColumn)
				{
					return CurrentRenderMatrixMember.RowSpan;
				}
				return m_definitionEndIndex - m_definitionStartIndex;
			}
		}

		internal override int ColSpan
		{
			get
			{
				if (m_isColumn)
				{
					return m_definitionEndIndex - m_definitionStartIndex;
				}
				return CurrentRenderMatrixMember.ColumnSpan;
			}
		}

		public override int MemberCellIndex => m_definitionStartIndex;

		public override bool IsTotal
		{
			get
			{
				if (m_group == null && m_staticOrSubtotal != null && m_staticOrSubtotal.IsTotal && m_staticOrSubtotal.Hidden && m_staticOrSubtotal.SharedHidden == Microsoft.ReportingServices.ReportRendering.SharedHiddenState.Always)
				{
					return true;
				}
				return false;
			}
		}

		public override Visibility Visibility
		{
			get
			{
				if (m_visibility == null && m_group != null && m_group.CurrentShimRenderGroup.m_visibilityDef != null)
				{
					m_visibility = new ShimMatrixMemberVisibility(this);
				}
				return m_visibility;
			}
		}

		internal override PageBreakLocation PropagatedGroupBreak
		{
			get
			{
				if (IsStatic)
				{
					return PageBreakLocation.None;
				}
				return m_propagatedPageBreak;
			}
		}

		public override bool KeepTogether
		{
			get
			{
				if (m_isColumn)
				{
					return false;
				}
				return true;
			}
		}

		public override TablixMemberInstance Instance
		{
			get
			{
				if (base.OwnerTablix.RenderingContext.InstanceAccessDisallowed)
				{
					return null;
				}
				if (m_instance == null)
				{
					if (IsStatic)
					{
						m_instance = new TablixMemberInstance(base.OwnerTablix, this);
					}
					else
					{
						TablixDynamicMemberInstance instance = new TablixDynamicMemberInstance(base.OwnerTablix, this, new InternalShimDynamicMemberLogic(this));
						m_owner.RenderingContext.AddDynamicInstance(instance);
						m_instance = instance;
					}
				}
				return m_instance;
			}
		}

		internal int DefinitionStartIndex => m_definitionStartIndex;

		internal int DefinitionEndIndex => m_definitionEndIndex;

		internal int AdjustedRenderCollectionIndex
		{
			get
			{
				if (IsStatic)
				{
					return m_renderCollectionIndex;
				}
				return m_renderCollectionIndex + Math.Max(0, m_group.CurrentRenderGroupIndex);
			}
		}

		internal MatrixMemberInfoCache CurrentMatrixMemberCellIndexes => m_currentMatrixMemberCellIndexes;

		internal MatrixMember CurrentRenderMatrixMember
		{
			get
			{
				if (m_staticOrSubtotal != null)
				{
					return m_staticOrSubtotal;
				}
				return m_group.CurrentShimRenderGroup as MatrixMember;
			}
		}

		internal ShimMatrixMember(IDefinitionPath parentDefinitionPath, Tablix owner, ShimMatrixMember parent, int parentCollectionIndex, bool isColumn, int renderCollectionIndex, MatrixMember staticOrSubtotal, bool isAfterSubtotal, MatrixMemberInfoCache matrixMemberCellIndexes)
			: base(parentDefinitionPath, owner, parent, parentCollectionIndex, isColumn)
		{
			m_renderCollectionIndex = renderCollectionIndex;
			m_isAfterSubtotal = isAfterSubtotal;
			m_currentMatrixMemberCellIndexes = matrixMemberCellIndexes;
			m_definitionStartIndex = owner.GetCurrentMemberCellDefinitionIndex();
			m_staticOrSubtotal = staticOrSubtotal;
			GenerateInnerHierarchy(owner, parent, isColumn, staticOrSubtotal.Children);
			m_definitionEndIndex = owner.GetCurrentMemberCellDefinitionIndex();
		}

		internal ShimMatrixMember(IDefinitionPath parentDefinitionPath, Tablix owner, ShimMatrixMember parent, int parentCollectionIndex, bool isColumn, int renderCollectionIndex, ShimRenderGroups renderGroups, MatrixMemberInfoCache matrixMemberCellIndexes)
			: base(parentDefinitionPath, owner, parent, parentCollectionIndex, isColumn)
		{
			m_renderCollectionIndex = renderCollectionIndex;
			m_currentMatrixMemberCellIndexes = matrixMemberCellIndexes;
			m_definitionStartIndex = owner.GetCurrentMemberCellDefinitionIndex();
			m_group = new Group(owner, renderGroups, this);
			GenerateInnerHierarchy(owner, parent, isColumn, ((MatrixMember)m_group.CurrentShimRenderGroup).Children);
			m_definitionEndIndex = owner.GetCurrentMemberCellDefinitionIndex();
		}

		private void GenerateInnerHierarchy(Tablix owner, ShimMatrixMember parent, bool isColumn, MatrixMemberCollection children)
		{
			if (children != null)
			{
				MatrixMemberInfoCache matrixMemberInfoCache = null;
				if (m_isColumn)
				{
					matrixMemberInfoCache = ((children.MatrixHeadingDef.SubHeading == null) ? new MatrixMemberInfoCache((m_staticOrSubtotal != null) ? m_staticOrSubtotal.MemberCellIndex : AdjustedRenderCollectionIndex, -1) : new MatrixMemberInfoCache(-1, children.Count));
					m_currentMatrixMemberCellIndexes.Children[AdjustedRenderCollectionIndex] = matrixMemberInfoCache;
				}
				m_children = new ShimMatrixMemberCollection(this, owner, isColumn, this, children, matrixMemberInfoCache);
			}
			else
			{
				owner.GetAndIncrementMemberCellDefinitionIndex();
			}
		}

		internal override bool SetNewContext(int index)
		{
			base.ResetContext();
			if (m_instance != null)
			{
				m_instance.SetNewContext();
			}
			if (m_group != null)
			{
				if (base.OwnerTablix.RenderMatrix.NoRows)
				{
					return false;
				}
				if (index < 0 || index >= m_group.RenderGroups.Count)
				{
					return false;
				}
				m_group.CurrentRenderGroupIndex = index;
				MatrixMember currentRenderGroup = m_group.RenderGroups[index] as MatrixMember;
				UpdateMatrixMemberInfoCache(m_group.RenderGroups.MatrixMemberCollectionCount, null);
				UpdateContext(currentRenderGroup);
				return true;
			}
			return index <= 1;
		}

		internal override void ResetContext()
		{
			base.ResetContext();
			if (m_group.CurrentRenderGroupIndex >= 0)
			{
				ResetContext(null, -1, null, null);
			}
		}

		internal void ResetContext(MatrixMember staticOrSubtotal, int newAfterSubtotalCollectionIndex, ShimRenderGroups renderGroups, MatrixMemberInfoCache newMatrixMemberCellIndexes)
		{
			int currentAllocationSize = 1;
			if (m_group != null)
			{
				m_group.CurrentRenderGroupIndex = -1;
				if (renderGroups != null)
				{
					m_group.RenderGroups = renderGroups;
				}
				currentAllocationSize = m_group.RenderGroups.MatrixMemberCollectionCount;
			}
			else if (staticOrSubtotal != null)
			{
				m_staticOrSubtotal = staticOrSubtotal;
				if (m_isAfterSubtotal && newAfterSubtotalCollectionIndex >= 0)
				{
					m_renderCollectionIndex = newAfterSubtotalCollectionIndex;
				}
			}
			UpdateMatrixMemberInfoCache(currentAllocationSize, newMatrixMemberCellIndexes);
			if (IsStatic)
			{
				UpdateContext(m_staticOrSubtotal);
			}
			else if (!base.OwnerTablix.RenderMatrix.NoRows && m_group.RenderGroups != null && m_group.RenderGroups.Count > 0)
			{
				UpdateContext(m_group.RenderGroups[0] as MatrixMember);
			}
		}

		private void UpdateContext(MatrixMember currentRenderGroup)
		{
			if (m_header != null)
			{
				m_header.ResetCellContents();
			}
			if (m_children != null)
			{
				((ShimMatrixMemberCollection)m_children).ResetContext(currentRenderGroup.Children);
			}
			else
			{
				((ShimMatrixRowCollection)base.OwnerTablix.Body.RowCollection).UpdateCells(this);
			}
		}

		private void UpdateMatrixMemberInfoCache(int currentAllocationSize, MatrixMemberInfoCache newMatrixMemberCellIndexes)
		{
			if (!m_isColumn)
			{
				return;
			}
			MatrixMemberInfoCache matrixMemberInfoCache = (m_parent == null) ? null : ((ShimMatrixMember)m_parent).CurrentMatrixMemberCellIndexes;
			if (matrixMemberInfoCache == null)
			{
				if (newMatrixMemberCellIndexes != null)
				{
					m_currentMatrixMemberCellIndexes = newMatrixMemberCellIndexes;
				}
				return;
			}
			int adjustedRenderCollectionIndex = ((ShimMatrixMember)m_parent).AdjustedRenderCollectionIndex;
			MatrixMemberInfoCache matrixMemberInfoCache2 = matrixMemberInfoCache.Children[adjustedRenderCollectionIndex];
			if (matrixMemberInfoCache2 == null)
			{
				matrixMemberInfoCache2 = ((m_children == null) ? new MatrixMemberInfoCache(matrixMemberInfoCache.GetCellIndex((ShimMatrixMember)m_parent), -1) : new MatrixMemberInfoCache(-1, currentAllocationSize));
				matrixMemberInfoCache.Children[adjustedRenderCollectionIndex] = matrixMemberInfoCache2;
			}
			m_currentMatrixMemberCellIndexes = matrixMemberInfoCache2;
		}
	}
}
