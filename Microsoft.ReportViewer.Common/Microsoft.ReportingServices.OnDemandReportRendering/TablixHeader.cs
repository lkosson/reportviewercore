using Microsoft.ReportingServices.ReportIntermediateFormat;
using Microsoft.ReportingServices.ReportRendering;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class TablixHeader : IDefinitionPath
	{
		private Tablix m_owner;

		private TablixMember m_tablixMember;

		private string m_definitionPath;

		private CellContents m_cellContents;

		private Microsoft.ReportingServices.ReportRendering.ReportItem m_cacheRenderReportItem;

		public string DefinitionPath
		{
			get
			{
				if (m_definitionPath == null)
				{
					m_definitionPath = ParentDefinitionPath.DefinitionPath + "xH";
				}
				return m_definitionPath;
			}
		}

		public IDefinitionPath ParentDefinitionPath => m_tablixMember;

		public ReportSize Size
		{
			get
			{
				if (m_owner.IsOldSnapshot)
				{
					if (m_owner.SnapshotTablixType != DataRegion.Type.Matrix)
					{
						return null;
					}
					ShimMatrixMember shimMatrixMember = m_tablixMember as ShimMatrixMember;
					if (shimMatrixMember.IsColumn)
					{
						return new ReportSize(shimMatrixMember.CurrentRenderMatrixMember.Height);
					}
					return new ReportSize(shimMatrixMember.CurrentRenderMatrixMember.Width);
				}
				Microsoft.ReportingServices.ReportIntermediateFormat.TablixHeader tablixHeader = m_tablixMember.MemberDefinition.TablixHeader;
				if (tablixHeader.SizeForRendering == null)
				{
					tablixHeader.SizeForRendering = new ReportSize(tablixHeader.Size, tablixHeader.SizeValue);
				}
				return tablixHeader.SizeForRendering;
			}
		}

		public CellContents CellContents
		{
			get
			{
				if (m_cellContents == null)
				{
					if (m_owner.IsOldSnapshot)
					{
						if (m_owner.SnapshotTablixType == DataRegion.Type.Matrix)
						{
							ShimMatrixMember shimMatrixMember = m_tablixMember as ShimMatrixMember;
							Microsoft.ReportingServices.ReportRendering.ReportItem renderReportItem = shimMatrixMember.IsStatic ? shimMatrixMember.m_staticOrSubtotal.ReportItem : ((MatrixMember)shimMatrixMember.Group.CurrentShimRenderGroup).ReportItem;
							m_cellContents = new CellContents(this, m_owner.InSubtotal, renderReportItem, shimMatrixMember.RowSpan, shimMatrixMember.ColSpan, m_owner.RenderingContext, shimMatrixMember.SizeDelta, shimMatrixMember.IsColumn);
						}
					}
					else
					{
						m_cellContents = new CellContents(m_tablixMember.ReportScope, this, m_tablixMember.MemberDefinition.TablixHeader.CellContents, m_tablixMember.RowSpan, m_tablixMember.ColSpan, m_owner.RenderingContext);
					}
				}
				else if (m_owner.IsOldSnapshot)
				{
					OnDemandUpdateCellContents();
				}
				return m_cellContents;
			}
		}

		internal TablixHeader(Tablix owner, TablixMember tablixMember)
		{
			m_owner = owner;
			m_tablixMember = tablixMember;
		}

		internal void SetNewContext()
		{
			if (m_cellContents != null)
			{
				m_cellContents.SetNewContext();
			}
		}

		internal void ResetCellContents()
		{
			m_cacheRenderReportItem = null;
		}

		private void OnDemandUpdateCellContents()
		{
			if (m_cacheRenderReportItem == null && m_cellContents != null)
			{
				m_cacheRenderReportItem = ((ShimMatrixMember)m_tablixMember).CurrentRenderMatrixMember.ReportItem;
				m_cellContents.UpdateRenderReportItem(m_cacheRenderReportItem);
			}
		}
	}
}
