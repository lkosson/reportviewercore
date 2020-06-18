using Microsoft.ReportingServices.Diagnostics;

namespace Microsoft.Reporting
{
	internal sealed class ControlProcessingConfiguration : BaseLocalProcessingConfiguration
	{
		private IMapTileServerConfiguration m_mapTileServerConfiguration;

		public override IMapTileServerConfiguration MapTileServerConfiguration => m_mapTileServerConfiguration;

		public void SetMapTileServerConfiguration(IMapTileServerConfiguration serverConfig)
		{
			m_mapTileServerConfiguration = serverConfig;
		}
	}
}
