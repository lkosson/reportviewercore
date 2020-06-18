namespace Microsoft.ReportingServices.Diagnostics
{
	internal enum MapTileCacheLevel
	{
		Default,
		BypassCache,
		CacheOnly,
		CacheIfAvailable,
		Revalidate,
		Reload,
		NoCacheNoStore
	}
}
