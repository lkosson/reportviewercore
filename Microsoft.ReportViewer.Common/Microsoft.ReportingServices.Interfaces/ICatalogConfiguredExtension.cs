using System.Collections.Generic;
using System.Security.Permissions;

namespace Microsoft.ReportingServices.Interfaces
{
	public interface ICatalogConfiguredExtension
	{
		void SetCatalogConfiguration(IDictionary<string, string> configuration);

		IEnumerable<string> EnumerateRequiredProperties();
	}
}
