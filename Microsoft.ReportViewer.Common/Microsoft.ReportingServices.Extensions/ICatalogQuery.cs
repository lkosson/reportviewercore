using System.Collections;
using System.Data;

namespace Microsoft.ReportingServices.Extensions
{
	internal interface ICatalogQuery
	{
		void ExecuteNonQuery(string query, Hashtable parameters, CommandType type);

		IDataReader ExecuteReader(string query, Hashtable parameters, CommandType type);

		void Commit();
	}
}
