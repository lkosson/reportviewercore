using Microsoft.ReportingServices.ReportProcessing;
using Microsoft.ReportingServices.ReportRendering;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class ShimMatrixMemberCollection : ShimMemberCollection
	{
		private int m_definitionStartIndex = -1;

		private int m_definitionEndIndex = -1;

		private int m_dynamicSubgroupChildIndex = -1;

		private int m_subtotalChildIndex = -1;

		private double m_sizeDelta;

		public override TablixMember this[int index]
		{
			get
			{
				if (index < 0 || index >= Count)
				{
					throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidParameterRange, index, 0, Count);
				}
				return (TablixMember)m_children[index];
			}
		}

		internal override double SizeDelta => m_sizeDelta;

		public override int Count => m_children.Length;

		internal PageBreakLocation PropagatedGroupBreakLocation
		{
			get
			{
				if (m_dynamicSubgroupChildIndex < 0)
				{
					return PageBreakLocation.None;
				}
				return ((TablixMember)m_children[m_dynamicSubgroupChildIndex]).PropagatedGroupBreak;
			}
		}

		internal ShimMatrixMemberCollection(IDefinitionPath parentDefinitionPath, Tablix owner, bool isColumnGroup, ShimMatrixMember parent, MatrixMemberCollection renderMemberCollection, MatrixMemberInfoCache matrixMemberCellIndexes)
			: base(parentDefinitionPath, owner, isColumnGroup)
		{
			m_definitionStartIndex = owner.GetCurrentMemberCellDefinitionIndex();
			int count = renderMemberCollection.Count;
			if (renderMemberCollection[0].IsStatic)
			{
				m_children = new ShimMatrixMember[count];
				for (int i = 0; i < count; i++)
				{
					m_children[i] = new ShimMatrixMember(this, owner, parent, i, isColumnGroup, i, renderMemberCollection[i], isAfterSubtotal: false, matrixMemberCellIndexes);
				}
			}
			else
			{
				m_dynamicSubgroupChildIndex = 0;
				bool flag = renderMemberCollection.MatrixHeadingDef.Subtotal != null;
				bool flag2 = flag && renderMemberCollection.MatrixHeadingDef.Subtotal.Position == Subtotal.PositionType.After;
				m_children = new ShimMatrixMember[(!flag) ? 1 : 2];
				if (flag)
				{
					m_subtotalChildIndex = 0;
					if (flag2)
					{
						m_subtotalChildIndex++;
					}
					else
					{
						m_dynamicSubgroupChildIndex++;
					}
				}
				if (flag)
				{
					Microsoft.ReportingServices.ReportRendering.ReportItem reportItem = renderMemberCollection[m_subtotalChildIndex].ReportItem;
					if (reportItem != null)
					{
						if (isColumnGroup)
						{
							m_sizeDelta += reportItem.Width.ToMillimeters();
						}
						else
						{
							m_sizeDelta += reportItem.Height.ToMillimeters();
						}
					}
				}
				if (flag && !flag2)
				{
					m_children[m_subtotalChildIndex] = new ShimMatrixMember(this, owner, parent, m_subtotalChildIndex, isColumnGroup, 0, renderMemberCollection[0], flag2, matrixMemberCellIndexes);
				}
				ShimRenderGroups renderGroups = new ShimRenderGroups(renderMemberCollection, flag && !flag2, flag && flag2);
				ShimMatrixMember shimMatrixMember = (ShimMatrixMember)(m_children[m_dynamicSubgroupChildIndex] = new ShimMatrixMember(this, owner, parent, m_dynamicSubgroupChildIndex, isColumnGroup, m_dynamicSubgroupChildIndex, renderGroups, matrixMemberCellIndexes));
				if (flag && flag2)
				{
					m_children[m_subtotalChildIndex] = new ShimMatrixMember(this, owner, parent, m_subtotalChildIndex, isColumnGroup, count - 1, renderMemberCollection[count - 1], flag2, matrixMemberCellIndexes);
				}
				m_sizeDelta += shimMatrixMember.SizeDelta;
			}
			m_definitionEndIndex = owner.GetCurrentMemberCellDefinitionIndex();
		}

		internal void UpdateContext(MatrixMemberInfoCache matrixMemberCellIndexes)
		{
			if (m_children != null)
			{
				if (m_isColumnGroup)
				{
					ResetContext(base.OwnerTablix.RenderMatrix.ColumnMemberCollection, matrixMemberCellIndexes);
				}
				else
				{
					ResetContext(base.OwnerTablix.RenderMatrix.RowMemberCollection);
				}
			}
		}

		internal void ResetContext(MatrixMemberCollection newRenderMemberCollection)
		{
			ResetContext(newRenderMemberCollection, null);
		}

		internal void ResetContext(MatrixMemberCollection newRenderMemberCollection, MatrixMemberInfoCache matrixMemberCellIndexes)
		{
			if (m_children == null)
			{
				return;
			}
			MatrixMember staticOrSubtotal = null;
			int newAfterSubtotalCollectionIndex = -1;
			ShimRenderGroups renderGroups = null;
			if (newRenderMemberCollection != null)
			{
				renderGroups = new ShimRenderGroups(newRenderMemberCollection, m_subtotalChildIndex == 0, 1 == m_subtotalChildIndex);
				int count = newRenderMemberCollection.Count;
				if (m_subtotalChildIndex == 0)
				{
					staticOrSubtotal = newRenderMemberCollection[0];
				}
				else if (1 == m_subtotalChildIndex)
				{
					staticOrSubtotal = newRenderMemberCollection[count - 1];
					newAfterSubtotalCollectionIndex = count - 1;
				}
			}
			if (m_dynamicSubgroupChildIndex >= 0)
			{
				((ShimMatrixMember)m_children[m_dynamicSubgroupChildIndex]).ResetContext(null, -1, renderGroups, matrixMemberCellIndexes);
				if (m_subtotalChildIndex >= 0)
				{
					((ShimMatrixMember)m_children[m_subtotalChildIndex]).ResetContext(staticOrSubtotal, newAfterSubtotalCollectionIndex, null, matrixMemberCellIndexes);
				}
			}
			else
			{
				for (int i = 0; i < m_children.Length; i++)
				{
					staticOrSubtotal = newRenderMemberCollection?[i];
					((ShimMatrixMember)m_children[i]).ResetContext(staticOrSubtotal, -1, null, matrixMemberCellIndexes);
				}
			}
		}
	}
}
