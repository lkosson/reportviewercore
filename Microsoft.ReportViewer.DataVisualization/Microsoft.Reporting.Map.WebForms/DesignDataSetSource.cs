using System;
using System.Collections.Specialized;
using System.Data;
using System.Globalization;

namespace Microsoft.Reporting.Map.WebForms
{
	internal class DesignDataSetSource : DataSet, IDesignTimeDataSource, IDisposable
	{
		public DesignDataSetSource(MapCore mapCore, object originalDataSource)
		{
			StringEnumerator enumerator = DataBindingHelper.GetDataSourceDataMembers(originalDataSource).GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					string current = enumerator.Current;
					DataTable dataTable = new DataTable(current)
					{
						Locale = CultureInfo.CurrentCulture
					};
					DataBindingHelper.InitDesignDataTable(originalDataSource, current, dataTable);
					base.Tables.Add(dataTable);
				}
			}
			finally
			{
				(enumerator as IDisposable)?.Dispose();
			}
		}
	}
}
