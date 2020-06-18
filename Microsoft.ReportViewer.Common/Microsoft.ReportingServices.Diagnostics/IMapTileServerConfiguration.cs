namespace Microsoft.ReportingServices.Diagnostics
{
	internal interface IMapTileServerConfiguration
	{
		int MaxConnections
		{
			get;
		}

		int Timeout
		{
			get;
		}

		string AppID
		{
			get;
		}

		MapTileCacheLevel CacheLevel
		{
			get;
		}
	}
}
