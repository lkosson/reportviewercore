using Microsoft.ReportingServices.Diagnostics;
using System;

namespace Microsoft.Reporting
{
	[Serializable]
	internal sealed class LocalProcessingHostMapTileServerConfiguration : IMapTileServerConfiguration
	{
		private int m_maxConnections;

		private int m_timeout;

		private string m_appID;

		public int MaxConnections
		{
			get
			{
				return m_maxConnections;
			}
			set
			{
				if (!IsInRange(value, 1, int.MaxValue))
				{
					throw new ArgumentOutOfRangeException("value", ProcessingStrings.MapTileServerConfiguration_MaxConnectionsOutOfRange(1, int.MaxValue));
				}
				m_maxConnections = value;
			}
		}

		public int Timeout
		{
			get
			{
				return m_timeout;
			}
			set
			{
				if (!IsInRange(value, 1, int.MaxValue))
				{
					throw new ArgumentOutOfRangeException("value", ProcessingStrings.MapTileServerConfiguration_TimeoutOutOfRange(1, int.MaxValue));
				}
				m_timeout = value;
			}
		}

		public string AppID
		{
			get
			{
				return m_appID;
			}
			set
			{
				m_appID = value;
			}
		}

		public MapTileCacheLevel CacheLevel => MapTileCacheLevel.Default;

		internal LocalProcessingHostMapTileServerConfiguration()
		{
			MaxConnections = 2;
			Timeout = 10;
			AppID = "(Default)";
		}

		private static bool IsInRange(int value, int min, int max)
		{
			if (min <= value)
			{
				return value <= max;
			}
			return false;
		}
	}
}
