using Microsoft.ReportingServices.ReportIntermediateFormat;
using System.IO;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class MapTileLayerInstance : MapLayerInstance
	{
		private MapTileLayer m_defObject;

		private string m_serviceUrl;

		private MapTileStyle? m_tileStyle;

		private bool? m_useSecureConnection;

		public string ServiceUrl
		{
			get
			{
				if (m_serviceUrl == null)
				{
					m_serviceUrl = ((Microsoft.ReportingServices.ReportIntermediateFormat.MapTileLayer)m_defObject.MapLayerDef).EvaluateServiceUrl(ReportScopeInstance, m_defObject.MapDef.RenderingContext.OdpContext);
				}
				return m_serviceUrl;
			}
		}

		public MapTileStyle TileStyle
		{
			get
			{
				if (!m_tileStyle.HasValue)
				{
					m_tileStyle = ((Microsoft.ReportingServices.ReportIntermediateFormat.MapTileLayer)m_defObject.MapLayerDef).EvaluateTileStyle(ReportScopeInstance, m_defObject.MapDef.RenderingContext.OdpContext);
				}
				return m_tileStyle.Value;
			}
		}

		public bool UseSecureConnection
		{
			get
			{
				if (!m_useSecureConnection.HasValue)
				{
					m_useSecureConnection = ((Microsoft.ReportingServices.ReportIntermediateFormat.MapTileLayer)m_defObject.MapLayerDef).EvaluateUseSecureConnection(ReportScopeInstance, m_defObject.MapDef.RenderingContext.OdpContext);
				}
				return m_useSecureConnection.Value;
			}
		}

		internal MapTileLayerInstance(MapTileLayer defObject)
			: base(defObject)
		{
			m_defObject = defObject;
		}

		public Stream GetTileData(string url, out string mimeType)
		{
			return m_defObject.MapTileLayerDef.GetTileData(url, out mimeType, m_defObject.MapDef.RenderingContext);
		}

		public void SetTileData(string url, byte[] data, string mimeType)
		{
			m_defObject.MapTileLayerDef.SetTileData(url, data, mimeType, m_defObject.MapDef.RenderingContext);
		}

		protected override void ResetInstanceCache()
		{
			base.ResetInstanceCache();
			m_serviceUrl = null;
			m_tileStyle = null;
			m_useSecureConnection = null;
		}
	}
}
