using System.Net.Cache;

namespace Microsoft.ReportingServices.Diagnostics
{
	internal static class MapTileServerConsts
	{
		internal const string MaxConnections = "MaxConnections";

		internal const string Timeout = "Timeout";

		internal const string AppID = "AppID";

		internal const string CacheLevel = "CacheLevel";

		internal const int MaxConnectionsDefault = 2;

		internal const int TimeoutDefault = 10;

		internal const string AppIDDefault = "(Default)";

		internal const MapTileCacheLevel CacheLevelDefault = MapTileCacheLevel.Default;

		internal const int MaxConnectionsMinValue = 1;

		internal const int MaxConnectionsMaxValue = int.MaxValue;

		internal const int TimeoutMinValue = 1;

		internal const int TimeoutMaxValue = int.MaxValue;

		internal static RequestCacheLevel ConvertFromMapTileCacheLevel(MapTileCacheLevel cacheLevel)
		{
			switch (cacheLevel)
			{
			case MapTileCacheLevel.BypassCache:
				return RequestCacheLevel.BypassCache;
			case MapTileCacheLevel.CacheIfAvailable:
				return RequestCacheLevel.CacheIfAvailable;
			case MapTileCacheLevel.CacheOnly:
				return RequestCacheLevel.CacheOnly;
			case MapTileCacheLevel.NoCacheNoStore:
				return RequestCacheLevel.NoCacheNoStore;
			case MapTileCacheLevel.Reload:
				return RequestCacheLevel.Reload;
			case MapTileCacheLevel.Revalidate:
				return RequestCacheLevel.Revalidate;
			default:
				return RequestCacheLevel.Default;
			}
		}
	}
}
