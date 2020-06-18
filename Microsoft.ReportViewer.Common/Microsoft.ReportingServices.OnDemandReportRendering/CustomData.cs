using Microsoft.ReportingServices.ReportIntermediateFormat;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class CustomData : IReportScopeInstance
	{
		private CustomReportItem m_owner;

		private DataHierarchy m_columns;

		private DataHierarchy m_rows;

		private DataRowCollection m_rowCollection;

		private bool m_isNewContext = true;

		public string DataSetName
		{
			get
			{
				if (m_owner.IsOldSnapshot)
				{
					return m_owner.RenderCri.CriDefinition.DataSetName;
				}
				return ((Microsoft.ReportingServices.ReportIntermediateFormat.DataRegion)m_owner.ReportItemDef).DataSetName;
			}
		}

		public DataHierarchy DataColumnHierarchy
		{
			get
			{
				if (m_columns == null)
				{
					if (m_owner.IsOldSnapshot)
					{
						if (m_owner.RenderCri.CustomData.DataColumnGroupings != null)
						{
							m_columns = new DataHierarchy(m_owner, isColumn: true);
						}
					}
					else if (m_owner.CriDef.DataColumnMembers != null)
					{
						m_columns = new DataHierarchy(m_owner, isColumn: true);
					}
				}
				return m_columns;
			}
		}

		public DataHierarchy DataRowHierarchy
		{
			get
			{
				if (m_rows == null)
				{
					if (m_owner.IsOldSnapshot)
					{
						if (m_owner.RenderCri.CustomData.DataRowGroupings != null)
						{
							m_rows = new DataHierarchy(m_owner, isColumn: false);
						}
					}
					else if (m_owner.CriDef.DataRowMembers != null)
					{
						m_rows = new DataHierarchy(m_owner, isColumn: false);
					}
				}
				return m_rows;
			}
		}

		internal bool HasDataRowCollection => m_rowCollection != null;

		public DataRowCollection RowCollection
		{
			get
			{
				if (m_rowCollection == null)
				{
					if (m_owner.IsOldSnapshot)
					{
						if (m_owner.RenderCri.CustomData.DataCells != null)
						{
							m_rowCollection = new ShimDataRowCollection(m_owner);
						}
					}
					else if (m_owner.CriDef.DataRows != null)
					{
						m_rowCollection = new InternalDataRowCollection(m_owner, m_owner.CriDef.DataRows);
					}
				}
				return m_rowCollection;
			}
		}

		string IReportScopeInstance.UniqueName => m_owner.InstanceUniqueName;

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

		IReportScope IReportScopeInstance.ReportScope => m_owner.ReportScope;

		internal CustomData(CustomReportItem owner)
		{
			m_owner = owner;
		}

		internal void SetNewContext()
		{
			m_isNewContext = true;
			if (m_rows != null)
			{
				m_rows.SetNewContext();
			}
			if (m_columns != null)
			{
				m_columns.SetNewContext();
			}
		}
	}
}
