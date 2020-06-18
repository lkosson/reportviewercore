namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class TablixCellInstance : BaseInstance, IReportScopeInstance
	{
		private TablixCell m_cellDef;

		private Tablix m_owner;

		private int m_rowIndex;

		private int m_columnIndex;

		private bool m_isNewContext = true;

		string IReportScopeInstance.UniqueName => m_cellDef.Cell.UniqueName;

		bool IReportScopeInstance.IsNewContext
		{
			get
			{
				return m_isNewContext;
			}
			set
			{
				m_isNewContext = value;
			}
		}

		IReportScope IReportScopeInstance.ReportScope => m_reportScope;

		internal TablixCellInstance(TablixCell cellDef, Tablix owner, int rowIndex, int colIndex)
			: base(cellDef)
		{
			m_cellDef = cellDef;
			m_owner = owner;
			m_rowIndex = rowIndex;
			m_columnIndex = colIndex;
		}

		internal override void SetNewContext()
		{
			if (!m_isNewContext)
			{
				m_isNewContext = true;
				base.SetNewContext();
			}
		}

		protected override void ResetInstanceCache()
		{
		}
	}
}
