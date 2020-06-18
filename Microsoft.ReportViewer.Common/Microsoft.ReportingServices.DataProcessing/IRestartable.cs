using System.Collections.Generic;

namespace Microsoft.ReportingServices.DataProcessing
{
	internal interface IRestartable
	{
		IDataParameter[] StartAt(List<ScopeValueFieldName> scopeValueFieldNameCollection);
	}
}
