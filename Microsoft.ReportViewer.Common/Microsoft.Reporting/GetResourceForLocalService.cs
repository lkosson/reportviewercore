using Microsoft.ReportingServices.Diagnostics;
using Microsoft.ReportingServices.ReportProcessing;

namespace Microsoft.Reporting
{
	internal sealed class GetResourceForLocalService : IGetResource
	{
		private ILocalCatalog m_catalog;

		public GetResourceForLocalService(ILocalCatalog catalog)
		{
			m_catalog = catalog;
		}

		public void GetResource(ICatalogItemContext reportContext, string imageUrl, out byte[] resource, out string mimeType, out bool registerWarning, out bool registerInvalidSizeWarning)
		{
			registerInvalidSizeWarning = false;
			ICatalogItemContext catalogItemContext = null;
			if (reportContext != null)
			{
				catalogItemContext = reportContext.Combine(imageUrl);
			}
			if (catalogItemContext != null)
			{
				resource = m_catalog.GetResource(catalogItemContext.ItemPathAsString, out mimeType);
				registerWarning = (resource == null);
				return;
			}
			try
			{
				registerWarning = false;
				resource = ExternalResourceLoader.GetExternalResource(imageUrl, impersonate: true, null, null, null, 600, ExternalResourceLoader.MaxResourceSizeUnlimited, null, out mimeType, out registerInvalidSizeWarning);
			}
			finally
			{
				registerWarning = true;
			}
		}
	}
}
