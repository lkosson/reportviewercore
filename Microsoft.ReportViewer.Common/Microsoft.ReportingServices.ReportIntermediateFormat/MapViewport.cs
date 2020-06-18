using Microsoft.ReportingServices.OnDemandProcessing;
using Microsoft.ReportingServices.OnDemandReportRendering;
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
	internal sealed class MapViewport : MapSubItem, IPersistable
	{
		[NonSerialized]
		private static readonly Declaration m_Declaration = GetDeclaration();

		private ExpressionInfo m_mapCoordinateSystem;

		private ExpressionInfo m_mapProjection;

		private ExpressionInfo m_projectionCenterX;

		private ExpressionInfo m_projectionCenterY;

		private MapLimits m_mapLimits;

		private MapView m_mapView;

		private ExpressionInfo m_maximumZoom;

		private ExpressionInfo m_minimumZoom;

		private ExpressionInfo m_contentMargin;

		private MapGridLines m_mapMeridians;

		private MapGridLines m_mapParallels;

		private ExpressionInfo m_gridUnderContent;

		private ExpressionInfo m_simplificationResolution;

		internal ExpressionInfo MapCoordinateSystem
		{
			get
			{
				return m_mapCoordinateSystem;
			}
			set
			{
				m_mapCoordinateSystem = value;
			}
		}

		internal ExpressionInfo MapProjection
		{
			get
			{
				return m_mapProjection;
			}
			set
			{
				m_mapProjection = value;
			}
		}

		internal ExpressionInfo ProjectionCenterX
		{
			get
			{
				return m_projectionCenterX;
			}
			set
			{
				m_projectionCenterX = value;
			}
		}

		internal ExpressionInfo ProjectionCenterY
		{
			get
			{
				return m_projectionCenterY;
			}
			set
			{
				m_projectionCenterY = value;
			}
		}

		internal MapLimits MapLimits
		{
			get
			{
				return m_mapLimits;
			}
			set
			{
				m_mapLimits = value;
			}
		}

		internal MapView MapView
		{
			get
			{
				return m_mapView;
			}
			set
			{
				m_mapView = value;
			}
		}

		internal ExpressionInfo MaximumZoom
		{
			get
			{
				return m_maximumZoom;
			}
			set
			{
				m_maximumZoom = value;
			}
		}

		internal ExpressionInfo MinimumZoom
		{
			get
			{
				return m_minimumZoom;
			}
			set
			{
				m_minimumZoom = value;
			}
		}

		internal ExpressionInfo SimplificationResolution
		{
			get
			{
				return m_simplificationResolution;
			}
			set
			{
				m_simplificationResolution = value;
			}
		}

		internal ExpressionInfo ContentMargin
		{
			get
			{
				return m_contentMargin;
			}
			set
			{
				m_contentMargin = value;
			}
		}

		internal MapGridLines MapMeridians
		{
			get
			{
				return m_mapMeridians;
			}
			set
			{
				m_mapMeridians = value;
			}
		}

		internal MapGridLines MapParallels
		{
			get
			{
				return m_mapParallels;
			}
			set
			{
				m_mapParallels = value;
			}
		}

		internal ExpressionInfo GridUnderContent
		{
			get
			{
				return m_gridUnderContent;
			}
			set
			{
				m_gridUnderContent = value;
			}
		}

		internal new MapViewportExprHost ExprHost => (MapViewportExprHost)m_exprHost;

		internal MapViewport()
		{
		}

		internal MapViewport(Map map)
			: base(map)
		{
		}

		internal override void Initialize(InitializationContext context)
		{
			context.ExprHostBuilder.MapViewportStart();
			base.Initialize(context);
			if (m_mapCoordinateSystem != null)
			{
				m_mapCoordinateSystem.Initialize("MapCoordinateSystem", context);
				context.ExprHostBuilder.MapViewportMapCoordinateSystem(m_mapCoordinateSystem);
			}
			if (m_mapProjection != null)
			{
				m_mapProjection.Initialize("MapProjection", context);
				context.ExprHostBuilder.MapViewportMapProjection(m_mapProjection);
			}
			if (m_projectionCenterX != null)
			{
				m_projectionCenterX.Initialize("ProjectionCenterX", context);
				context.ExprHostBuilder.MapViewportProjectionCenterX(m_projectionCenterX);
			}
			if (m_projectionCenterY != null)
			{
				m_projectionCenterY.Initialize("ProjectionCenterY", context);
				context.ExprHostBuilder.MapViewportProjectionCenterY(m_projectionCenterY);
			}
			if (m_mapLimits != null)
			{
				m_mapLimits.Initialize(context);
			}
			if (m_mapView != null)
			{
				m_mapView.Initialize(context);
			}
			if (m_maximumZoom != null)
			{
				m_maximumZoom.Initialize("MaximumZoom", context);
				context.ExprHostBuilder.MapViewportMaximumZoom(m_maximumZoom);
			}
			if (m_minimumZoom != null)
			{
				m_minimumZoom.Initialize("MinimumZoom", context);
				context.ExprHostBuilder.MapViewportMinimumZoom(m_minimumZoom);
			}
			if (m_contentMargin != null)
			{
				m_contentMargin.Initialize("ContentMargin", context);
				context.ExprHostBuilder.MapViewportContentMargin(m_contentMargin);
			}
			if (m_mapMeridians != null)
			{
				m_mapMeridians.Initialize(context, isMeridian: true);
			}
			if (m_mapParallels != null)
			{
				m_mapParallels.Initialize(context, isMeridian: false);
			}
			if (m_gridUnderContent != null)
			{
				m_gridUnderContent.Initialize("GridUnderContent", context);
				context.ExprHostBuilder.MapViewportGridUnderContent(m_gridUnderContent);
			}
			if (m_simplificationResolution != null)
			{
				m_simplificationResolution.Initialize("SimplificationResolution", context);
				context.ExprHostBuilder.MapViewportSimplificationResolution(m_simplificationResolution);
			}
			context.ExprHostBuilder.MapViewportEnd();
		}

		internal override object PublishClone(AutomaticSubtotalContext context)
		{
			MapViewport mapViewport = (MapViewport)base.PublishClone(context);
			if (m_mapCoordinateSystem != null)
			{
				mapViewport.m_mapCoordinateSystem = (ExpressionInfo)m_mapCoordinateSystem.PublishClone(context);
			}
			if (m_mapProjection != null)
			{
				mapViewport.m_mapProjection = (ExpressionInfo)m_mapProjection.PublishClone(context);
			}
			if (m_projectionCenterX != null)
			{
				mapViewport.m_projectionCenterX = (ExpressionInfo)m_projectionCenterX.PublishClone(context);
			}
			if (m_projectionCenterY != null)
			{
				mapViewport.m_projectionCenterY = (ExpressionInfo)m_projectionCenterY.PublishClone(context);
			}
			if (m_mapLimits != null)
			{
				mapViewport.m_mapLimits = (MapLimits)m_mapLimits.PublishClone(context);
			}
			if (m_mapView != null)
			{
				mapViewport.m_mapView = (MapView)m_mapView.PublishClone(context);
			}
			if (m_maximumZoom != null)
			{
				mapViewport.m_maximumZoom = (ExpressionInfo)m_maximumZoom.PublishClone(context);
			}
			if (m_minimumZoom != null)
			{
				mapViewport.m_minimumZoom = (ExpressionInfo)m_minimumZoom.PublishClone(context);
			}
			if (m_contentMargin != null)
			{
				mapViewport.m_contentMargin = (ExpressionInfo)m_contentMargin.PublishClone(context);
			}
			if (m_mapMeridians != null)
			{
				mapViewport.m_mapMeridians = (MapGridLines)m_mapMeridians.PublishClone(context);
			}
			if (m_mapParallels != null)
			{
				mapViewport.m_mapParallels = (MapGridLines)m_mapParallels.PublishClone(context);
			}
			if (m_gridUnderContent != null)
			{
				mapViewport.m_gridUnderContent = (ExpressionInfo)m_gridUnderContent.PublishClone(context);
			}
			if (m_simplificationResolution != null)
			{
				mapViewport.m_simplificationResolution = (ExpressionInfo)m_simplificationResolution.PublishClone(context);
			}
			return mapViewport;
		}

		internal void SetExprHost(MapViewportExprHost exprHost, ObjectModelImpl reportObjectModel)
		{
			Global.Tracer.Assert(exprHost != null && reportObjectModel != null, "(exprHost != null && reportObjectModel != null)");
			SetExprHost((MapSubItemExprHost)exprHost, reportObjectModel);
			if (m_mapLimits != null && ExprHost.MapLimitsHost != null)
			{
				m_mapLimits.SetExprHost(ExprHost.MapLimitsHost, reportObjectModel);
			}
			if (m_mapMeridians != null && ExprHost.MapMeridiansHost != null)
			{
				m_mapMeridians.SetExprHost(ExprHost.MapMeridiansHost, reportObjectModel);
			}
			if (m_mapParallels != null && ExprHost.MapParallelsHost != null)
			{
				m_mapParallels.SetExprHost(ExprHost.MapParallelsHost, reportObjectModel);
			}
			if (m_mapView != null && ExprHost.MapViewHost != null)
			{
				m_mapView.SetExprHost(ExprHost.MapViewHost, reportObjectModel);
			}
		}

		internal new static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.MapCoordinateSystem, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.MapProjection, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.ProjectionCenterX, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.ProjectionCenterY, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.MapLimits, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapLimits));
			list.Add(new MemberInfo(MemberName.MapView, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapView));
			list.Add(new MemberInfo(MemberName.MaximumZoom, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.MinimumZoom, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.ContentMargin, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.MapMeridians, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapGridLines));
			list.Add(new MemberInfo(MemberName.MapParallels, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapGridLines));
			list.Add(new MemberInfo(MemberName.GridUnderContent, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.SimplificationResolution, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			return new Declaration(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapViewport, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapSubItem, list);
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.MapCoordinateSystem:
					writer.Write(m_mapCoordinateSystem);
					break;
				case MemberName.MapProjection:
					writer.Write(m_mapProjection);
					break;
				case MemberName.ProjectionCenterX:
					writer.Write(m_projectionCenterX);
					break;
				case MemberName.ProjectionCenterY:
					writer.Write(m_projectionCenterY);
					break;
				case MemberName.MapLimits:
					writer.Write(m_mapLimits);
					break;
				case MemberName.MapView:
					writer.Write(m_mapView);
					break;
				case MemberName.MaximumZoom:
					writer.Write(m_maximumZoom);
					break;
				case MemberName.MinimumZoom:
					writer.Write(m_minimumZoom);
					break;
				case MemberName.ContentMargin:
					writer.Write(m_contentMargin);
					break;
				case MemberName.MapMeridians:
					writer.Write(m_mapMeridians);
					break;
				case MemberName.MapParallels:
					writer.Write(m_mapParallels);
					break;
				case MemberName.GridUnderContent:
					writer.Write(m_gridUnderContent);
					break;
				case MemberName.SimplificationResolution:
					writer.Write(m_simplificationResolution);
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
				case MemberName.MapCoordinateSystem:
					m_mapCoordinateSystem = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.MapProjection:
					m_mapProjection = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.ProjectionCenterX:
					m_projectionCenterX = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.ProjectionCenterY:
					m_projectionCenterY = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.MapLimits:
					m_mapLimits = (MapLimits)reader.ReadRIFObject();
					break;
				case MemberName.MapView:
					m_mapView = (MapView)reader.ReadRIFObject();
					break;
				case MemberName.MaximumZoom:
					m_maximumZoom = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.MinimumZoom:
					m_minimumZoom = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.ContentMargin:
					m_contentMargin = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.MapMeridians:
					m_mapMeridians = (MapGridLines)reader.ReadRIFObject();
					break;
				case MemberName.MapParallels:
					m_mapParallels = (MapGridLines)reader.ReadRIFObject();
					break;
				case MemberName.GridUnderContent:
					m_gridUnderContent = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.SimplificationResolution:
					m_simplificationResolution = (ExpressionInfo)reader.ReadRIFObject();
					break;
				default:
					Global.Tracer.Assert(condition: false);
					break;
				}
			}
		}

		public override Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapViewport;
		}

		internal MapCoordinateSystem EvaluateMapCoordinateSystem(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_map, reportScopeInstance);
			return EnumTranslator.TranslateMapCoordinateSystem(context.ReportRuntime.EvaluateMapViewportMapCoordinateSystemExpression(this, m_map.Name), context.ReportRuntime);
		}

		internal MapProjection EvaluateMapProjection(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_map, reportScopeInstance);
			return EnumTranslator.TranslateMapProjection(context.ReportRuntime.EvaluateMapViewportMapProjectionExpression(this, m_map.Name), context.ReportRuntime);
		}

		internal double EvaluateProjectionCenterX(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_map, reportScopeInstance);
			return context.ReportRuntime.EvaluateMapViewportProjectionCenterXExpression(this, m_map.Name);
		}

		internal double EvaluateProjectionCenterY(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_map, reportScopeInstance);
			return context.ReportRuntime.EvaluateMapViewportProjectionCenterYExpression(this, m_map.Name);
		}

		internal double EvaluateMaximumZoom(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_map, reportScopeInstance);
			return context.ReportRuntime.EvaluateMapViewportMaximumZoomExpression(this, m_map.Name);
		}

		internal double EvaluateMinimumZoom(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_map, reportScopeInstance);
			return context.ReportRuntime.EvaluateMapViewportMinimumZoomExpression(this, m_map.Name);
		}

		internal string EvaluateContentMargin(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_map, reportScopeInstance);
			return context.ReportRuntime.EvaluateMapViewportContentMarginExpression(this, m_map.Name);
		}

		internal bool EvaluateGridUnderContent(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_map, reportScopeInstance);
			return context.ReportRuntime.EvaluateMapViewportGridUnderContentExpression(this, m_map.Name);
		}

		internal double EvaluateSimplificationResolution(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_map, reportScopeInstance);
			return context.ReportRuntime.EvaluateMapViewportSimplificationResolutionExpression(this, m_map.Name);
		}
	}
}
