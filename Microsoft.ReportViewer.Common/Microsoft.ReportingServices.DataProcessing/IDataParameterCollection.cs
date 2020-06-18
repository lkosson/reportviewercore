using System.Collections;

namespace Microsoft.ReportingServices.DataProcessing
{
	public interface IDataParameterCollection : IEnumerable
	{
		int Add(IDataParameter parameter);
	}
}
