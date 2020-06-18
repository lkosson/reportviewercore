namespace Microsoft.Reporting.WinForms
{
	internal sealed class DataSourceWrapper : IDataSource
	{
		private readonly ReportDataSource m_ds;

		string IDataSource.Name => m_ds.Name;

		object IDataSource.Value => m_ds.Value;

		internal DataSourceWrapper(ReportDataSource ds)
		{
			m_ds = ds;
		}
	}
}
