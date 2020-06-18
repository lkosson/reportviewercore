using Microsoft.ReportingServices.OnDemandProcessing;
using Microsoft.ReportingServices.OnDemandReportRendering;
using Microsoft.ReportingServices.RdlExpressions.ExpressionHostObjectModel;
using Microsoft.ReportingServices.ReportIntermediateFormat.Persistence;
using Microsoft.ReportingServices.ReportProcessing;
using Microsoft.ReportingServices.ReportProcessing.OnDemandReportObjectModel;
using Microsoft.ReportingServices.ReportPublishing;
using System;
using System.Collections.Generic;
using System.IO;

namespace Microsoft.ReportingServices.ReportIntermediateFormat
{
	[Serializable]
	internal sealed class MapTileLayer : MapLayer, IPersistable
	{
		[NonSerialized]
		private static readonly Declaration m_Declaration = GetDeclaration();

		private ExpressionInfo m_serviceUrl;

		private ExpressionInfo m_tileStyle;

		private ExpressionInfo m_useSecureConnection;

		private List<MapTile> m_mapTiles;

		internal ExpressionInfo ServiceUrl
		{
			get
			{
				return m_serviceUrl;
			}
			set
			{
				m_serviceUrl = value;
			}
		}

		internal ExpressionInfo TileStyle
		{
			get
			{
				return m_tileStyle;
			}
			set
			{
				m_tileStyle = value;
			}
		}

		internal ExpressionInfo UseSecureConnection
		{
			get
			{
				return m_useSecureConnection;
			}
			set
			{
				m_useSecureConnection = value;
			}
		}

		internal List<MapTile> MapTiles
		{
			get
			{
				return m_mapTiles;
			}
			set
			{
				m_mapTiles = value;
			}
		}

		internal new MapTileLayerExprHost ExprHost => (MapTileLayerExprHost)m_exprHost;

		internal MapTileLayer()
		{
		}

		internal MapTileLayer(Map map)
			: base(map)
		{
		}

		internal override void Initialize(InitializationContext context)
		{
			context.ExprHostBuilder.MapTileLayerStart(base.Name);
			base.Initialize(context);
			if (m_serviceUrl != null)
			{
				m_serviceUrl.Initialize("ServiceUrl", context);
				context.ExprHostBuilder.MapTileLayerServiceUrl(m_serviceUrl);
			}
			if (m_tileStyle != null)
			{
				m_tileStyle.Initialize("TileStyle", context);
				context.ExprHostBuilder.MapTileLayerTileStyle(m_tileStyle);
			}
			if (m_useSecureConnection != null)
			{
				m_useSecureConnection.Initialize("UseSecureConnection", context);
				context.ExprHostBuilder.MapTileLayerUseSecureConnection(m_useSecureConnection);
			}
			if (m_mapTiles != null)
			{
				for (int i = 0; i < m_mapTiles.Count; i++)
				{
					m_mapTiles[i].Initialize(context, i);
				}
			}
			m_exprHostID = context.ExprHostBuilder.MapTileLayerEnd();
		}

		internal override object PublishClone(AutomaticSubtotalContext context)
		{
			MapTileLayer mapTileLayer = (MapTileLayer)base.PublishClone(context);
			if (m_serviceUrl != null)
			{
				mapTileLayer.m_serviceUrl = (ExpressionInfo)m_serviceUrl.PublishClone(context);
			}
			if (m_tileStyle != null)
			{
				mapTileLayer.m_tileStyle = (ExpressionInfo)m_tileStyle.PublishClone(context);
			}
			if (m_mapTiles != null)
			{
				mapTileLayer.m_mapTiles = new List<MapTile>(m_mapTiles.Count);
				foreach (MapTile mapTile in m_mapTiles)
				{
					mapTileLayer.m_mapTiles.Add((MapTile)mapTile.PublishClone(context));
				}
			}
			if (m_useSecureConnection != null)
			{
				mapTileLayer.m_useSecureConnection = (ExpressionInfo)m_useSecureConnection.PublishClone(context);
			}
			return mapTileLayer;
		}

		internal void SetExprHost(MapTileLayerExprHost exprHost, ObjectModelImpl reportObjectModel)
		{
			Global.Tracer.Assert(exprHost != null && reportObjectModel != null, "(exprHost != null && reportObjectModel != null)");
			base.SetExprHost(exprHost, reportObjectModel);
			IList<MapTileExprHost> mapTilesHostsRemotable = ExprHost.MapTilesHostsRemotable;
			if (m_mapTiles == null || mapTilesHostsRemotable == null)
			{
				return;
			}
			for (int i = 0; i < m_mapTiles.Count; i++)
			{
				MapTile mapTile = m_mapTiles[i];
				if (mapTile != null && mapTile.ExpressionHostID > -1)
				{
					mapTile.SetExprHost(mapTilesHostsRemotable[mapTile.ExpressionHostID], reportObjectModel);
				}
			}
		}

		internal new static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.ServiceUrl, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.TileStyle, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.MapTiles, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapTile));
			list.Add(new MemberInfo(MemberName.UseSecureConnection, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			return new Declaration(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapTileLayer, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapLayer, list);
		}

		internal Stream GetTileData(string url, out string mimeType, Microsoft.ReportingServices.OnDemandReportRendering.RenderingContext renderingContext)
		{
			if (renderingContext.OdpContext.OdpMetadata.TryGetExternalImage(url, out ImageInfo imageInfo))
			{
				return renderingContext.OdpContext.ChunkFactory.GetChunk(imageInfo.StreamName, Microsoft.ReportingServices.ReportProcessing.ReportProcessing.ReportChunkTypes.Image, ChunkMode.Open, out mimeType);
			}
			mimeType = null;
			return null;
		}

		internal void SetTileData(string url, byte[] data, string mimeType, Microsoft.ReportingServices.OnDemandReportRendering.RenderingContext renderingContext)
		{
			string text = Guid.NewGuid().ToString("N");
			ImageInfo imageInfo = new ImageInfo(text, "");
			renderingContext.OdpContext.OdpMetadata.AddExternalImage(url, imageInfo);
			using (Stream stream = renderingContext.OdpContext.ChunkFactory.CreateChunk(text, Microsoft.ReportingServices.ReportProcessing.ReportProcessing.ReportChunkTypes.Image, mimeType))
			{
				stream.Write(data, 0, data.Length);
			}
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.ServiceUrl:
					writer.Write(m_serviceUrl);
					break;
				case MemberName.TileStyle:
					writer.Write(m_tileStyle);
					break;
				case MemberName.UseSecureConnection:
					writer.Write(m_useSecureConnection);
					break;
				case MemberName.MapTiles:
					writer.Write(m_mapTiles);
					break;
				default:
					Global.Tracer.Assert(condition: false);
					break;
				}
			}
		}

		public override void Deserialize(IntermediateFormatReader reader)
		{
			base.Deserialize(reader);
			reader.RegisterDeclaration(m_Declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.ServiceUrl:
					m_serviceUrl = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.TileStyle:
					m_tileStyle = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.UseSecureConnection:
					m_useSecureConnection = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.MapTiles:
					m_mapTiles = reader.ReadGenericListOfRIFObjects<MapTile>();
					break;
				default:
					Global.Tracer.Assert(condition: false);
					break;
				}
			}
		}

		public override Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapTileLayer;
		}

		internal string EvaluateServiceUrl(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_map, reportScopeInstance);
			return context.ReportRuntime.EvaluateMapTileLayerServiceUrlExpression(this, m_map.Name);
		}

		internal MapTileStyle EvaluateTileStyle(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_map, reportScopeInstance);
			return EnumTranslator.TranslateMapTileStyle(context.ReportRuntime.EvaluateMapTileLayerTileStyleExpression(this, m_map.Name), context.ReportRuntime);
		}

		internal bool EvaluateUseSecureConnection(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_map, reportScopeInstance);
			return context.ReportRuntime.EvaluateMapTileLayerUseSecureConnectionExpression(this, m_map.Name);
		}
	}
}
