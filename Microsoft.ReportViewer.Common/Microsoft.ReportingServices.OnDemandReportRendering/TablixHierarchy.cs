using Microsoft.ReportingServices.ReportIntermediateFormat;
using Microsoft.ReportingServices.ReportRendering;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class TablixHierarchy : MemberHierarchy<TablixMember>
	{
		private Tablix OwnerTablix => m_owner as Tablix;

		public TablixMemberCollection MemberCollection
		{
			get
			{
				if (m_members == null)
				{
					if (OwnerTablix.IsOldSnapshot)
					{
						switch (OwnerTablix.SnapshotTablixType)
						{
						case DataRegion.Type.List:
							if (m_isColumn)
							{
								m_members = new ShimListMemberCollection(this, OwnerTablix);
							}
							else
							{
								m_members = new ShimListMemberCollection(this, OwnerTablix, OwnerTablix.RenderList.Contents);
							}
							break;
						case DataRegion.Type.Table:
							OwnerTablix.ResetMemberCellDefinitionIndex(0);
							m_members = new ShimTableMemberCollection(this, OwnerTablix, m_isColumn);
							break;
						case DataRegion.Type.Matrix:
						{
							OwnerTablix.ResetMemberCellDefinitionIndex(0);
							MatrixMemberCollection renderMemberCollection = m_isColumn ? OwnerTablix.RenderMatrix.ColumnMemberCollection : OwnerTablix.RenderMatrix.RowMemberCollection;
							m_members = new ShimMatrixMemberCollection(this, OwnerTablix, m_isColumn, null, renderMemberCollection, CreateMatrixMemberCache());
							break;
						}
						}
						if (!m_isColumn)
						{
							CalculatePropagatedPageBreak();
						}
					}
					else
					{
						Microsoft.ReportingServices.ReportIntermediateFormat.Tablix tablixDef = OwnerTablix.TablixDef;
						if (tablixDef.TablixColumns != null)
						{
							m_members = new InternalTablixMemberCollection(this, OwnerTablix, null, m_isColumn ? tablixDef.TablixColumnMembers : tablixDef.TablixRowMembers);
						}
					}
				}
				return (TablixMemberCollection)m_members;
			}
		}

		internal TablixHierarchy(Tablix owner, bool isColumn)
			: base((ReportItem)owner, isColumn)
		{
		}

		private void CalculatePropagatedPageBreak()
		{
			Microsoft.ReportingServices.ReportRendering.DataRegion dataRegion = (Microsoft.ReportingServices.ReportRendering.DataRegion)m_owner.RenderReportItem;
			bool thisOrAnscestorHasToggle = dataRegion.SharedHidden == Microsoft.ReportingServices.ReportRendering.SharedHiddenState.Sometimes;
			PageBreakLocation pageBreakLocation = PageBreakHelper.GetPageBreakLocation(dataRegion.PageBreakAtStart, dataRegion.PageBreakAtEnd);
			if (m_members != null && m_members.Count > 0)
			{
				pageBreakLocation = PageBreakHelper.MergePageBreakLocations(CalculatePropagatedPageBreak(m_members, thisOrAnscestorHasToggle, OwnerTablix.SnapshotTablixType == DataRegion.Type.Table), pageBreakLocation);
			}
			OwnerTablix.SetPageBreakLocation(pageBreakLocation);
		}

		private PageBreakLocation CalculatePropagatedPageBreak(DataRegionMemberCollection<TablixMember> members, bool thisOrAnscestorHasToggle, bool isTable)
		{
			PageBreakLocation result = PageBreakLocation.None;
			bool flag = false;
			ShimTablixMember shimTablixMember = null;
			for (int i = 0; i < members.Count; i++)
			{
				ShimTablixMember shimTablixMember2 = (ShimTablixMember)members[i];
				if (shimTablixMember2.IsStatic)
				{
					if (isTable)
					{
						if (shimTablixMember2.RepeatOnNewPage)
						{
							flag = true;
						}
					}
					else if (shimTablixMember2.Children != null && shimTablixMember2.Children.Count > 0)
					{
						result = CalculatePropagatedPageBreak(shimTablixMember2.Children, thisOrAnscestorHasToggle, isTable: false);
					}
					continue;
				}
				shimTablixMember = shimTablixMember2;
				break;
			}
			if (shimTablixMember != null)
			{
				thisOrAnscestorHasToggle |= (shimTablixMember.Visibility != null && shimTablixMember.Visibility.HiddenState == SharedHiddenState.Sometimes);
				PageBreakLocation pageBreakLocation = PageBreakLocation.None;
				Microsoft.ReportingServices.ReportRendering.Group currentShimRenderGroup = shimTablixMember.Group.CurrentShimRenderGroup;
				if (currentShimRenderGroup != null)
				{
					pageBreakLocation = PageBreakHelper.GetPageBreakLocation(currentShimRenderGroup.PageBreakAtStart, currentShimRenderGroup.PageBreakAtEnd);
				}
				if (shimTablixMember.Children != null)
				{
					pageBreakLocation = PageBreakHelper.MergePageBreakLocations(CalculatePropagatedPageBreak(shimTablixMember.Children, thisOrAnscestorHasToggle, isTable), pageBreakLocation);
				}
				shimTablixMember.SetPropagatedPageBreak(pageBreakLocation);
				if ((!isTable || flag) && pageBreakLocation != 0)
				{
					if (!thisOrAnscestorHasToggle)
					{
						result = pageBreakLocation;
					}
					shimTablixMember.SetPropagatedPageBreak(PageBreakLocation.Between);
				}
			}
			return result;
		}

		internal override void ResetContext()
		{
			ResetContext(clearCache: true);
		}

		internal void ResetContext(bool clearCache)
		{
			if (clearCache)
			{
				OwnerTablix.ResetMemberCellDefinitionIndex(0);
			}
			if (m_members == null || !OwnerTablix.IsOldSnapshot)
			{
				return;
			}
			switch (OwnerTablix.SnapshotTablixType)
			{
			case DataRegion.Type.List:
				if (!m_isColumn)
				{
					((ShimListMemberCollection)m_members).UpdateContext(OwnerTablix.RenderList.Contents);
				}
				break;
			case DataRegion.Type.Table:
				if (!m_isColumn)
				{
					((ShimTableMemberCollection)m_members).UpdateContext();
				}
				break;
			case DataRegion.Type.Matrix:
			{
				MatrixMemberInfoCache matrixMemberCellIndexes = null;
				if (clearCache && m_isColumn)
				{
					matrixMemberCellIndexes = CreateMatrixMemberCache();
				}
				((ShimMatrixMemberCollection)m_members).UpdateContext(matrixMemberCellIndexes);
				break;
			}
			}
		}

		private MatrixMemberInfoCache CreateMatrixMemberCache()
		{
			if (m_isColumn)
			{
				MatrixMemberCollection columnMemberCollection = OwnerTablix.RenderMatrix.ColumnMemberCollection;
				if (columnMemberCollection.MatrixHeadingDef.SubHeading != null)
				{
					OwnerTablix.MatrixMemberColIndexes = new MatrixMemberInfoCache(-1, columnMemberCollection.Count);
				}
				else
				{
					OwnerTablix.MatrixMemberColIndexes = new MatrixMemberInfoCache(0, -1);
				}
				return OwnerTablix.MatrixMemberColIndexes;
			}
			return null;
		}
	}
}
