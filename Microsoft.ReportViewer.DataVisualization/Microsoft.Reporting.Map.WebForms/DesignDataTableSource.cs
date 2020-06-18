using System;
using System.Data;

namespace Microsoft.Reporting.Map.WebForms
{
	internal class DesignDataTableSource : DataTable, IDesignTimeDataSource, IDisposable
	{
		public DesignDataTableSource(MapCore mapCore, object originalDataSource)
		{
			DataBindingHelper.InitDesignDataTable(originalDataSource, string.Empty, this);
		}
	}
}
