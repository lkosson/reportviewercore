using System.IO;

namespace Microsoft.ReportingServices.OnDemandProcessing.Scalability
{
	internal interface IStreamHandler
	{
		Stream OpenStream();
	}
}
