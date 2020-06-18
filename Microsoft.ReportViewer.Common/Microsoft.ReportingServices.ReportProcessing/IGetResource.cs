using Microsoft.ReportingServices.Diagnostics;

namespace Microsoft.ReportingServices.ReportProcessing
{
	internal interface IGetResource
	{
		void GetResource(ICatalogItemContext reportContext, string path, out byte[] resource, out string mimeType, out bool registerExternalWarning, out bool registerInvalidSizeWarning);
	}
}
