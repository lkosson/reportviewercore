using Microsoft.ReportingServices.OnDemandProcessing;
using Microsoft.ReportingServices.OnDemandReportRendering;
using Microsoft.ReportingServices.RdlExpressions;
using Microsoft.ReportingServices.RdlExpressions.ExpressionHostObjectModel;
using Microsoft.ReportingServices.ReportIntermediateFormat.Persistence;
using Microsoft.ReportingServices.ReportProcessing;
using Microsoft.ReportingServices.ReportProcessing.OnDemandReportObjectModel;
using Microsoft.ReportingServices.ReportPublishing;
using System;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.ReportIntermediateFormat
{
	[Serializable]
	internal sealed class Map : ReportItem, IActionOwner, IPersistable, IPageBreakOwner
	{
		private List<MapDataRegion> m_mapDataRegions;

		private MapViewport m_mapViewport;

		private List<MapLayer> m_mapLayers;

		private List<MapLegend> m_mapLegends;

		private List<MapTitle> m_mapTitles;

		private MapDistanceScale m_mapDistanceScale;

		private MapColorScale m_mapColorScale;

		private MapBorderSkin m_mapBorderSkin;

		private ExpressionInfo m_antiAliasing;

		private ExpressionInfo m_textAntiAliasingQuality;

		private ExpressionInfo m_shadowIntensity;

		private Action m_action;

		private int m_maximumSpatialElementCount = 20000;

		private int m_maximumTotalPointCount = 1000000;

		private ExpressionInfo m_tileLanguage;

		private PageBreak m_pageBreak;

		private ExpressionInfo m_pageName;

		[NonSerialized]
		private static readonly Declaration m_Declaration = GetDeclaration();

		[NonSerialized]
		private MapExprHost m_exprHost;

		[NonSerialized]
		private int m_actionOwnerCounter;

		[NonSerialized]
		private Formatter m_formatter;

		internal override Microsoft.ReportingServices.ReportProcessing.ObjectType ObjectType => Microsoft.ReportingServices.ReportProcessing.ObjectType.Map;

		internal MapViewport MapViewport
		{
			get
			{
				return m_mapViewport;
			}
			set
			{
				m_mapViewport = value;
			}
		}

		internal List<MapLayer> MapLayers
		{
			get
			{
				return m_mapLayers;
			}
			set
			{
				m_mapLayers = value;
			}
		}

		internal List<MapLegend> MapLegends
		{
			get
			{
				return m_mapLegends;
			}
			set
			{
				m_mapLegends = value;
			}
		}

		internal List<MapTitle> MapTitles
		{
			get
			{
				return m_mapTitles;
			}
			set
			{
				m_mapTitles = value;
			}
		}

		internal MapDistanceScale MapDistanceScale
		{
			get
			{
				return m_mapDistanceScale;
			}
			set
			{
				m_mapDistanceScale = value;
			}
		}

		internal MapColorScale MapColorScale
		{
			get
			{
				return m_mapColorScale;
			}
			set
			{
				m_mapColorScale = value;
			}
		}

		internal MapBorderSkin MapBorderSkin
		{
			get
			{
				return m_mapBorderSkin;
			}
			set
			{
				m_mapBorderSkin = value;
			}
		}

		internal ExpressionInfo AntiAliasing
		{
			get
			{
				return m_antiAliasing;
			}
			set
			{
				m_antiAliasing = value;
			}
		}

		internal ExpressionInfo TextAntiAliasingQuality
		{
			get
			{
				return m_textAntiAliasingQuality;
			}
			set
			{
				m_textAntiAliasingQuality = value;
			}
		}

		internal ExpressionInfo ShadowIntensity
		{
			get
			{
				return m_shadowIntensity;
			}
			set
			{
				m_shadowIntensity = value;
			}
		}

		internal ExpressionInfo TileLanguage
		{
			get
			{
				return m_tileLanguage;
			}
			set
			{
				m_tileLanguage = value;
			}
		}

		internal int MaximumSpatialElementCount
		{
			get
			{
				return m_maximumSpatialElementCount;
			}
			set
			{
				m_maximumSpatialElementCount = value;
			}
		}

		internal int MaximumTotalPointCount
		{
			get
			{
				return m_maximumTotalPointCount;
			}
			set
			{
				m_maximumTotalPointCount = value;
			}
		}

		internal List<MapDataRegion> MapDataRegions
		{
			get
			{
				return m_mapDataRegions;
			}
			set
			{
				m_mapDataRegions = value;
			}
		}

		internal Action Action
		{
			get
			{
				return m_action;
			}
			set
			{
				m_action = value;
			}
		}

		Action IActionOwner.Action => m_action;

		internal ExpressionInfo PageName
		{
			get
			{
				return m_pageName;
			}
			set
			{
				m_pageName = value;
			}
		}

		internal PageBreak PageBreak
		{
			get
			{
				return m_pageBreak;
			}
			set
			{
				m_pageBreak = value;
			}
		}

		PageBreak IPageBreakOwner.PageBreak
		{
			get
			{
				return m_pageBreak;
			}
			set
			{
				m_pageBreak = value;
			}
		}

		Microsoft.ReportingServices.ReportProcessing.ObjectType IPageBreakOwner.ObjectType => Microsoft.ReportingServices.ReportProcessing.ObjectType.Map;

		string IPageBreakOwner.ObjectName => m_name;

		IInstancePath IPageBreakOwner.InstancePath => this;

		List<string> IActionOwner.FieldsUsedInValueExpression
		{
			get
			{
				return null;
			}
			set
			{
			}
		}

		internal MapExprHost MapExprHost => m_exprHost;

		internal Map(ReportItem parent)
			: base(parent)
		{
		}

		internal Map(int id, ReportItem parent)
			: base(id, parent)
		{
		}

		internal override bool Initialize(InitializationContext context)
		{
			context.ObjectType = ObjectType;
			context.ObjectName = m_name;
			context.ExprHostBuilder.MapStart(m_name);
			base.Initialize(context);
			if (m_visibility != null)
			{
				m_visibility.Initialize(context);
			}
			context.IsTopLevelCellContents = false;
			if (m_mapViewport != null)
			{
				m_mapViewport.Initialize(context);
			}
			if (m_mapLayers != null)
			{
				for (int i = 0; i < m_mapLayers.Count; i++)
				{
					m_mapLayers[i].Initialize(context);
				}
			}
			if (m_mapLegends != null)
			{
				for (int j = 0; j < m_mapLegends.Count; j++)
				{
					m_mapLegends[j].Initialize(context);
				}
			}
			if (m_mapTitles != null)
			{
				for (int k = 0; k < m_mapTitles.Count; k++)
				{
					m_mapTitles[k].Initialize(context);
				}
			}
			if (m_mapDistanceScale != null)
			{
				m_mapDistanceScale.Initialize(context);
			}
			if (m_mapColorScale != null)
			{
				m_mapColorScale.Initialize(context);
			}
			if (m_mapBorderSkin != null)
			{
				m_mapBorderSkin.Initialize(context);
			}
			if (m_antiAliasing != null)
			{
				m_antiAliasing.Initialize("AntiAliasing", context);
				context.ExprHostBuilder.MapAntiAliasing(m_antiAliasing);
			}
			if (m_textAntiAliasingQuality != null)
			{
				m_textAntiAliasingQuality.Initialize("TextAntiAliasingQuality", context);
				context.ExprHostBuilder.MapTextAntiAliasingQuality(m_textAntiAliasingQuality);
			}
			if (m_shadowIntensity != null)
			{
				m_shadowIntensity.Initialize("ShadowIntensity", context);
				context.ExprHostBuilder.MapShadowIntensity(m_shadowIntensity);
			}
			if (m_tileLanguage != null)
			{
				m_tileLanguage.Initialize("TileLanguage", context);
				context.ExprHostBuilder.MapTileLanguage(m_tileLanguage);
			}
			if (m_mapDataRegions != null)
			{
				for (int l = 0; l < m_mapDataRegions.Count; l++)
				{
					m_mapDataRegions[l].Initialize(context);
				}
			}
			if (m_action != null)
			{
				m_action.Initialize(context);
			}
			if (m_pageBreak != null)
			{
				m_pageBreak.Initialize(context);
			}
			if (m_pageName != null)
			{
				m_pageName.Initialize("PageName", context);
				context.ExprHostBuilder.PageName(m_pageName);
			}
			base.ExprHostID = context.ExprHostBuilder.MapEnd();
			return false;
		}

		internal override void TraverseScopes(IRIFScopeVisitor visitor)
		{
			if (m_mapDataRegions != null)
			{
				for (int i = 0; i < m_mapDataRegions.Count; i++)
				{
					m_mapDataRegions[i].TraverseScopes(visitor);
				}
			}
		}

		internal bool ContainsMapDataRegion()
		{
			if (m_mapDataRegions != null)
			{
				return m_mapDataRegions.Count != 0;
			}
			return false;
		}

		internal override object PublishClone(AutomaticSubtotalContext context)
		{
			Map map2 = context.CurrentMapClone = (Map)base.PublishClone(context);
			if (m_mapDataRegions != null)
			{
				map2.m_mapDataRegions = new List<MapDataRegion>(m_mapDataRegions.Count);
				foreach (MapDataRegion mapDataRegion in m_mapDataRegions)
				{
					map2.m_mapDataRegions.Add((MapDataRegion)mapDataRegion.PublishClone(context));
				}
			}
			if (m_mapLayers != null)
			{
				map2.m_mapLayers = new List<MapLayer>(m_mapLayers.Count);
				foreach (MapLayer mapLayer in m_mapLayers)
				{
					map2.m_mapLayers.Add((MapLayer)mapLayer.PublishClone(context));
				}
			}
			if (m_mapViewport != null)
			{
				map2.m_mapViewport = (MapViewport)m_mapViewport.PublishClone(context);
			}
			if (m_mapLegends != null)
			{
				map2.m_mapLegends = new List<MapLegend>(m_mapLegends.Count);
				foreach (MapLegend mapLegend in m_mapLegends)
				{
					map2.m_mapLegends.Add((MapLegend)mapLegend.PublishClone(context));
				}
			}
			if (m_mapTitles != null)
			{
				map2.m_mapTitles = new List<MapTitle>(m_mapTitles.Count);
				foreach (MapTitle mapTitle in m_mapTitles)
				{
					map2.m_mapTitles.Add((MapTitle)mapTitle.PublishClone(context));
				}
			}
			if (m_mapDistanceScale != null)
			{
				map2.m_mapDistanceScale = (MapDistanceScale)m_mapDistanceScale.PublishClone(context);
			}
			if (m_mapColorScale != null)
			{
				map2.m_mapColorScale = (MapColorScale)m_mapColorScale.PublishClone(context);
			}
			if (m_mapBorderSkin != null)
			{
				map2.m_mapBorderSkin = (MapBorderSkin)m_mapBorderSkin.PublishClone(context);
			}
			if (m_antiAliasing != null)
			{
				map2.m_antiAliasing = (ExpressionInfo)m_antiAliasing.PublishClone(context);
			}
			if (m_textAntiAliasingQuality != null)
			{
				map2.m_textAntiAliasingQuality = (ExpressionInfo)m_textAntiAliasingQuality.PublishClone(context);
			}
			if (m_shadowIntensity != null)
			{
				map2.m_shadowIntensity = (ExpressionInfo)m_shadowIntensity.PublishClone(context);
			}
			if (m_tileLanguage != null)
			{
				map2.m_tileLanguage = (ExpressionInfo)m_tileLanguage.PublishClone(context);
			}
			if (m_action != null)
			{
				map2.m_action = (Action)m_action.PublishClone(context);
			}
			if (m_pageBreak != null)
			{
				map2.m_pageBreak = (PageBreak)m_pageBreak.PublishClone(context);
			}
			if (m_pageName != null)
			{
				map2.m_pageName = (ExpressionInfo)m_pageName.PublishClone(context);
			}
			return map2;
		}

		internal MapAntiAliasing EvaluateAntiAliasing(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(this, reportScopeInstance);
			return EnumTranslator.TranslateMapAntiAliasing(context.ReportRuntime.EvaluateMapAntiAliasingExpression(this, base.Name), context.ReportRuntime);
		}

		internal MapTextAntiAliasingQuality EvaluateTextAntiAliasingQuality(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(this, reportScopeInstance);
			return EnumTranslator.TranslateMapTextAntiAliasingQuality(context.ReportRuntime.EvaluateMapTextAntiAliasingQualityExpression(this, base.Name), context.ReportRuntime);
		}

		internal double EvaluateShadowIntensity(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(this, reportScopeInstance);
			return context.ReportRuntime.EvaluateMapShadowIntensityExpression(this, base.Name);
		}

		internal string EvaluateTileLanguage(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(this, reportScopeInstance);
			return context.ReportRuntime.EvaluateMapTileLanguageExpression(this, base.Name);
		}

		internal string EvaluatePageName(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(this, reportScopeInstance);
			return context.ReportRuntime.EvaluateMapPageNameExpression(this, m_pageName, m_name);
		}

		internal new static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.MapDataRegions, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapDataRegion));
			list.Add(new MemberInfo(MemberName.MapViewport, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapViewport));
			list.Add(new MemberInfo(MemberName.MapLayers, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapLayer));
			list.Add(new MemberInfo(MemberName.MapLegends, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapLegend));
			list.Add(new MemberInfo(MemberName.MapTitles, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapTitle));
			list.Add(new MemberInfo(MemberName.MapDistanceScale, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapDistanceScale));
			list.Add(new MemberInfo(MemberName.MapColorScale, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapColorScale));
			list.Add(new MemberInfo(MemberName.MapBorderSkin, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapBorderSkin));
			list.Add(new ReadOnlyMemberInfo(MemberName.PageBreakLocation, Token.Enum));
			list.Add(new MemberInfo(MemberName.AntiAliasing, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.TextAntiAliasingQuality, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.ShadowIntensity, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.MaximumSpatialElementCount, Token.Int32));
			list.Add(new MemberInfo(MemberName.MaximumTotalPointCount, Token.Int32));
			list.Add(new MemberInfo(MemberName.Action, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Action));
			list.Add(new MemberInfo(MemberName.TileLanguage, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.PageBreak, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.PageBreak));
			list.Add(new MemberInfo(MemberName.PageName, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			return new Declaration(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Map, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ReportItem, list);
		}

		internal int GenerateActionOwnerID()
		{
			return ++m_actionOwnerCounter;
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.MapDataRegions:
					writer.Write(m_mapDataRegions);
					break;
				case MemberName.MapViewport:
					writer.Write(m_mapViewport);
					break;
				case MemberName.MapLayers:
					writer.Write(m_mapLayers);
					break;
				case MemberName.MapLegends:
					writer.Write(m_mapLegends);
					break;
				case MemberName.MapTitles:
					writer.Write(m_mapTitles);
					break;
				case MemberName.MapDistanceScale:
					writer.Write(m_mapDistanceScale);
					break;
				case MemberName.MapColorScale:
					writer.Write(m_mapColorScale);
					break;
				case MemberName.MapBorderSkin:
					writer.Write(m_mapBorderSkin);
					break;
				case MemberName.AntiAliasing:
					writer.Write(m_antiAliasing);
					break;
				case MemberName.TextAntiAliasingQuality:
					writer.Write(m_textAntiAliasingQuality);
					break;
				case MemberName.ShadowIntensity:
					writer.Write(m_shadowIntensity);
					break;
				case MemberName.MaximumSpatialElementCount:
					writer.Write(m_maximumSpatialElementCount);
					break;
				case MemberName.MaximumTotalPointCount:
					writer.Write(m_maximumTotalPointCount);
					break;
				case MemberName.Action:
					writer.Write(m_action);
					break;
				case MemberName.TileLanguage:
					writer.Write(m_tileLanguage);
					break;
				case MemberName.PageBreak:
					writer.Write(m_pageBreak);
					break;
				case MemberName.PageName:
					writer.Write(m_pageName);
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
				case MemberName.MapDataRegions:
					m_mapDataRegions = reader.ReadGenericListOfRIFObjects<MapDataRegion>();
					break;
				case MemberName.MapViewport:
					m_mapViewport = (MapViewport)reader.ReadRIFObject();
					break;
				case MemberName.MapLayers:
					m_mapLayers = reader.ReadGenericListOfRIFObjects<MapLayer>();
					break;
				case MemberName.MapLegends:
					m_mapLegends = reader.ReadGenericListOfRIFObjects<MapLegend>();
					break;
				case MemberName.MapTitles:
					m_mapTitles = reader.ReadGenericListOfRIFObjects<MapTitle>();
					break;
				case MemberName.MapDistanceScale:
					m_mapDistanceScale = (MapDistanceScale)reader.ReadRIFObject();
					break;
				case MemberName.MapColorScale:
					m_mapColorScale = (MapColorScale)reader.ReadRIFObject();
					break;
				case MemberName.MapBorderSkin:
					m_mapBorderSkin = (MapBorderSkin)reader.ReadRIFObject();
					break;
				case MemberName.PageBreakLocation:
					m_pageBreak = new PageBreak();
					m_pageBreak.BreakLocation = (PageBreakLocation)reader.ReadEnum();
					break;
				case MemberName.AntiAliasing:
					m_antiAliasing = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.TextAntiAliasingQuality:
					m_textAntiAliasingQuality = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.ShadowIntensity:
					m_shadowIntensity = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.MaximumSpatialElementCount:
					m_maximumSpatialElementCount = reader.ReadInt32();
					break;
				case MemberName.MaximumTotalPointCount:
					m_maximumTotalPointCount = reader.ReadInt32();
					break;
				case MemberName.Action:
					m_action = (Action)reader.ReadRIFObject();
					break;
				case MemberName.TileLanguage:
					m_tileLanguage = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.PageBreak:
					m_pageBreak = (PageBreak)reader.ReadRIFObject();
					break;
				case MemberName.PageName:
					m_pageName = (ExpressionInfo)reader.ReadRIFObject();
					break;
				default:
					Global.Tracer.Assert(condition: false);
					break;
				}
			}
		}

		public override void ResolveReferences(Dictionary<Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
			base.ResolveReferences(memberReferencesCollection, referenceableItems);
		}

		public override Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Map;
		}

		internal override void SetExprHost(ReportExprHost reportExprHost, ObjectModelImpl reportObjectModel)
		{
			if (base.ExprHostID < 0)
			{
				return;
			}
			Global.Tracer.Assert(reportExprHost != null && reportObjectModel != null);
			m_exprHost = reportExprHost.MapHostsRemotable[base.ExprHostID];
			ReportItemSetExprHost(m_exprHost, reportObjectModel);
			if (m_mapViewport != null && m_exprHost.MapViewportHost != null)
			{
				m_mapViewport.SetExprHost(m_exprHost.MapViewportHost, reportObjectModel);
			}
			IList<MapPolygonLayerExprHost> mapPolygonLayersHostsRemotable = m_exprHost.MapPolygonLayersHostsRemotable;
			IList<MapPointLayerExprHost> mapPointLayersHostsRemotable = m_exprHost.MapPointLayersHostsRemotable;
			IList<MapLineLayerExprHost> mapLineLayersHostsRemotable = m_exprHost.MapLineLayersHostsRemotable;
			IList<MapTileLayerExprHost> mapTileLayersHostsRemotable = m_exprHost.MapTileLayersHostsRemotable;
			if (m_mapLayers != null)
			{
				for (int i = 0; i < m_mapLayers.Count; i++)
				{
					MapLayer mapLayer = m_mapLayers[i];
					if (mapLayer == null || mapLayer.ExpressionHostID <= -1)
					{
						continue;
					}
					if (mapLayer is MapPolygonLayer)
					{
						if (mapPolygonLayersHostsRemotable != null)
						{
							mapLayer.SetExprHost(mapPolygonLayersHostsRemotable[mapLayer.ExpressionHostID], reportObjectModel);
						}
					}
					else if (mapLayer is MapPointLayer)
					{
						if (mapPointLayersHostsRemotable != null)
						{
							mapLayer.SetExprHost(mapPointLayersHostsRemotable[mapLayer.ExpressionHostID], reportObjectModel);
						}
					}
					else if (mapLayer is MapLineLayer)
					{
						if (mapLineLayersHostsRemotable != null)
						{
							mapLayer.SetExprHost(mapLineLayersHostsRemotable[mapLayer.ExpressionHostID], reportObjectModel);
						}
					}
					else if (mapLayer is MapTileLayer && mapTileLayersHostsRemotable != null)
					{
						mapLayer.SetExprHost(mapTileLayersHostsRemotable[mapLayer.ExpressionHostID], reportObjectModel);
					}
				}
			}
			IList<MapLegendExprHost> mapLegendsHostsRemotable = m_exprHost.MapLegendsHostsRemotable;
			if (m_mapLegends != null && mapLegendsHostsRemotable != null)
			{
				for (int j = 0; j < m_mapLegends.Count; j++)
				{
					MapLegend mapLegend = m_mapLegends[j];
					if (mapLegend != null && mapLegend.ExpressionHostID > -1)
					{
						mapLegend.SetExprHost(mapLegendsHostsRemotable[mapLegend.ExpressionHostID], reportObjectModel);
					}
				}
			}
			IList<MapTitleExprHost> mapTitlesHostsRemotable = m_exprHost.MapTitlesHostsRemotable;
			if (m_mapTitles != null && mapTitlesHostsRemotable != null)
			{
				for (int k = 0; k < m_mapTitles.Count; k++)
				{
					MapTitle mapTitle = m_mapTitles[k];
					if (mapTitle != null && mapTitle.ExpressionHostID > -1)
					{
						mapTitle.SetExprHost(mapTitlesHostsRemotable[mapTitle.ExpressionHostID], reportObjectModel);
					}
				}
			}
			if (m_mapDistanceScale != null && m_exprHost.MapDistanceScaleHost != null)
			{
				m_mapDistanceScale.SetExprHost(m_exprHost.MapDistanceScaleHost, reportObjectModel);
			}
			if (m_mapColorScale != null && m_exprHost.MapColorScaleHost != null)
			{
				m_mapColorScale.SetExprHost(m_exprHost.MapColorScaleHost, reportObjectModel);
			}
			if (m_mapBorderSkin != null && m_exprHost.MapBorderSkinHost != null)
			{
				m_mapBorderSkin.SetExprHost(m_exprHost.MapBorderSkinHost, reportObjectModel);
			}
			if (m_action != null && m_exprHost.ActionInfoHost != null)
			{
				m_action.SetExprHost(m_exprHost.ActionInfoHost, reportObjectModel);
			}
			if (m_pageBreak != null && m_exprHost.PageBreakExprHost != null)
			{
				m_pageBreak.SetExprHost(m_exprHost.PageBreakExprHost, reportObjectModel);
			}
		}

		internal string GetFormattedStringFromValue(ref Microsoft.ReportingServices.RdlExpressions.VariantResult result, OnDemandProcessingContext context)
		{
			string result2 = null;
			if (result.ErrorOccurred)
			{
				result2 = RPRes.rsExpressionErrorValue;
			}
			else if (result.Value != null)
			{
				result2 = Formatter.Format(result.Value, ref m_formatter, base.StyleClass, null, context, ObjectType, base.Name);
			}
			return result2;
		}
	}
}
