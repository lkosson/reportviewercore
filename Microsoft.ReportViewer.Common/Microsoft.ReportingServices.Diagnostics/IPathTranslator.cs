using System;

namespace Microsoft.ReportingServices.Diagnostics
{
	internal interface IPathTranslator
	{
		string PathToInternal(string source);

		string PathToExternal(string source);

		void SetExternalRoot(string path);

		void SetExternalRoot(CatalogItemPath path, int zone);

		Uri GetExternalRoot();

		ExternalItemPath CatalogToExternal(string source);

		ExternalItemPath CatalogToExternal(CatalogItemPath source);

		ExternalItemPath CatalogToExternal(CatalogItemPath source, int externalRootZone);

		string ExternalToCatalog(string source);

		int GetExternalRootZone(ExternalItemPath path);

		string GetPublicUrl(string url, bool noThrow);
	}
}
