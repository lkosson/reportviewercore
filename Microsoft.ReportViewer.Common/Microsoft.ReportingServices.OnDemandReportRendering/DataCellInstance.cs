using System.Globalization;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class DataCellInstance : BaseInstance, IReportScopeInstance
	{
		private DataCell m_dataCellDef;

		private bool m_isNewContext = true;

		string IReportScopeInstance.UniqueName
		{
			get
			{
				if (m_dataCellDef.CriDef.IsOldSnapshot)
				{
					return m_dataCellDef.CriDef.ID + "i" + m_dataCellDef.RenderItem.RowIndex.ToString(CultureInfo.InvariantCulture) + "x" + m_dataCellDef.RenderItem.ColumnIndex.ToString(CultureInfo.InvariantCulture);
				}
				return m_dataCellDef.DataCellDef.UniqueName;
			}
		}

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

		internal DataCellInstance(DataCell dataCellDef)
			: base(dataCellDef)
		{
			m_dataCellDef = dataCellDef;
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
