using System.IO;

namespace Microsoft.ReportingServices.ReportPublishing
{
	internal abstract class ReportUpgradeStrategy
	{
		internal abstract Stream Upgrade(Stream definitionStream);
	}
}
