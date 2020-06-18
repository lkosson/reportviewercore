using Microsoft.ReportingServices.ReportIntermediateFormat;
using Microsoft.ReportingServices.ReportRendering;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal abstract class DataCell : IReportScope, IDataRegionCell
	{
		protected int m_rowIndex;

		protected int m_columnIndex;

		protected CustomReportItem m_owner;

		protected DataValueCollection m_dataValues;

		protected DataCellInstance m_instance;

		public abstract DataValueCollection DataValues
		{
			get;
		}

		internal abstract Microsoft.ReportingServices.ReportIntermediateFormat.DataCell DataCellDef
		{
			get;
		}

		internal abstract Microsoft.ReportingServices.ReportRendering.DataCell RenderItem
		{
			get;
		}

		internal CustomReportItem CriDef => m_owner;

		public DataCellInstance Instance
		{
			get
			{
				if (m_owner.RenderingContext.InstanceAccessDisallowed)
				{
					return null;
				}
				if (m_instance == null)
				{
					m_instance = new DataCellInstance(this);
				}
				return m_instance;
			}
		}

		IReportScopeInstance IReportScope.ReportScopeInstance => Instance;

		IRIFReportScope IReportScope.RIFReportScope => RIFReportScope;

		internal virtual IRIFReportScope RIFReportScope => null;

		internal DataCell(CustomReportItem owner, int rowIndex, int colIndex)
		{
			m_owner = owner;
			m_rowIndex = rowIndex;
			m_columnIndex = colIndex;
			m_dataValues = null;
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
			if (m_dataValues != null)
			{
				m_dataValues.SetNewContext();
			}
		}
	}
}
