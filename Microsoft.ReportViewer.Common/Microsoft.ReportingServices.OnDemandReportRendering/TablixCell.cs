using Microsoft.ReportingServices.ReportIntermediateFormat;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal abstract class TablixCell : IDataRegionCell, IDefinitionPath, IReportScope
	{
		private Cell m_cell;

		protected Tablix m_owner;

		protected int m_rowIndex;

		protected int m_columnIndex;

		protected CellContents m_cellContents;

		protected TablixCellInstance m_instance;

		protected string m_definitionPath;

		public abstract string ID
		{
			get;
		}

		public string DefinitionPath
		{
			get
			{
				if (m_definitionPath == null)
				{
					m_definitionPath = DefinitionPathConstants.GetTablixCellDefinitionPath(m_owner, m_rowIndex, m_columnIndex, isTablixBodyCell: true);
				}
				return m_definitionPath;
			}
		}

		public IDefinitionPath ParentDefinitionPath => m_owner;

		public abstract CellContents CellContents
		{
			get;
		}

		public abstract DataElementOutputTypes DataElementOutput
		{
			get;
		}

		public abstract string DataElementName
		{
			get;
		}

		IReportScopeInstance IReportScope.ReportScopeInstance => Instance;

		IRIFReportScope IReportScope.RIFReportScope => m_cell;

		internal Cell Cell => m_cell;

		public virtual TablixCellInstance Instance
		{
			get
			{
				if (m_owner.m_renderingContext.InstanceAccessDisallowed)
				{
					return null;
				}
				if (m_instance == null)
				{
					m_instance = new TablixCellInstance(this, m_owner, m_rowIndex, m_columnIndex);
				}
				return m_instance;
			}
		}

		internal TablixCell(Cell cell, Tablix owner, int rowIndex, int colIndex)
		{
			m_cell = cell;
			m_owner = owner;
			m_rowIndex = rowIndex;
			m_columnIndex = colIndex;
		}

		void IDataRegionCell.SetNewContext()
		{
			SetNewContext();
		}

		internal virtual void SetNewContext()
		{
			if (m_instance != null)
			{
				m_instance.SetNewContext();
			}
			if (m_cellContents != null)
			{
				m_cellContents.SetNewContext();
			}
			if (m_cell != null)
			{
				m_cell.ClearStreamingScopeInstanceBinding();
			}
		}
	}
}
