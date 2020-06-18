using Microsoft.ReportingServices.OnDemandProcessing;
using Microsoft.ReportingServices.OnDemandReportRendering;
using Microsoft.ReportingServices.RdlExpressions.ExpressionHostObjectModel;
using Microsoft.ReportingServices.ReportIntermediateFormat.Persistence;
using Microsoft.ReportingServices.ReportProcessing;
using Microsoft.ReportingServices.ReportProcessing.OnDemandReportObjectModel;
using Microsoft.ReportingServices.ReportPublishing;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Microsoft.ReportingServices.ReportIntermediateFormat
{
	[Serializable]
	internal sealed class MapPolygon : MapSpatialElement, IPersistable
	{
		[NonSerialized]
		private static readonly Declaration m_Declaration = GetDeclaration();

		private ExpressionInfo m_useCustomPolygonTemplate;

		private MapPolygonTemplate m_mapPolygonTemplate;

		private ExpressionInfo m_useCustomCenterPointTemplate;

		private MapPointTemplate m_mapCenterPointTemplate;

		internal ExpressionInfo UseCustomPolygonTemplate
		{
			get
			{
				return m_useCustomPolygonTemplate;
			}
			set
			{
				m_useCustomPolygonTemplate = value;
			}
		}

		internal MapPolygonTemplate MapPolygonTemplate
		{
			get
			{
				return m_mapPolygonTemplate;
			}
			set
			{
				m_mapPolygonTemplate = value;
			}
		}

		internal ExpressionInfo UseCustomCenterPointTemplate
		{
			get
			{
				return m_useCustomCenterPointTemplate;
			}
			set
			{
				m_useCustomCenterPointTemplate = value;
			}
		}

		internal MapPointTemplate MapCenterPointTemplate
		{
			get
			{
				return m_mapCenterPointTemplate;
			}
			set
			{
				m_mapCenterPointTemplate = value;
			}
		}

		internal new MapPolygonExprHost ExprHost => (MapPolygonExprHost)m_exprHost;

		internal MapPolygon()
		{
		}

		internal MapPolygon(MapPolygonLayer mapPolygonLayer, Map map)
			: base(mapPolygonLayer, map)
		{
		}

		internal override void Initialize(InitializationContext context, int index)
		{
			context.ExprHostBuilder.MapPolygonStart(index.ToString(CultureInfo.InvariantCulture.NumberFormat));
			base.Initialize(context, index);
			if (m_useCustomPolygonTemplate != null)
			{
				m_useCustomPolygonTemplate.Initialize("UseCustomPolygonTemplate", context);
				context.ExprHostBuilder.MapPolygonUseCustomPolygonTemplate(m_useCustomPolygonTemplate);
			}
			if (m_mapPolygonTemplate != null)
			{
				m_mapPolygonTemplate.Initialize(context);
			}
			if (m_useCustomCenterPointTemplate != null)
			{
				m_useCustomCenterPointTemplate.Initialize("UseCustomPointTemplate", context);
				context.ExprHostBuilder.MapPolygonUseCustomCenterPointTemplate(m_useCustomCenterPointTemplate);
			}
			if (m_mapCenterPointTemplate != null)
			{
				m_mapCenterPointTemplate.Initialize(context);
			}
			m_exprHostID = context.ExprHostBuilder.MapPolygonEnd();
		}

		internal override object PublishClone(AutomaticSubtotalContext context)
		{
			MapPolygon mapPolygon = (MapPolygon)base.PublishClone(context);
			if (m_useCustomPolygonTemplate != null)
			{
				mapPolygon.m_useCustomPolygonTemplate = (ExpressionInfo)m_useCustomPolygonTemplate.PublishClone(context);
			}
			if (m_mapPolygonTemplate != null)
			{
				mapPolygon.m_mapPolygonTemplate = (MapPolygonTemplate)m_mapPolygonTemplate.PublishClone(context);
			}
			if (m_useCustomCenterPointTemplate != null)
			{
				mapPolygon.m_useCustomCenterPointTemplate = (ExpressionInfo)m_useCustomCenterPointTemplate.PublishClone(context);
			}
			if (m_mapCenterPointTemplate != null)
			{
				mapPolygon.m_mapCenterPointTemplate = (MapPointTemplate)m_mapCenterPointTemplate.PublishClone(context);
			}
			return mapPolygon;
		}

		internal void SetExprHost(MapPolygonExprHost exprHost, ObjectModelImpl reportObjectModel)
		{
			Global.Tracer.Assert(exprHost != null && reportObjectModel != null, "(exprHost != null && reportObjectModel != null)");
			SetExprHost((MapSpatialElementExprHost)exprHost, reportObjectModel);
			if (m_mapPolygonTemplate != null && ExprHost.MapPolygonTemplateHost != null)
			{
				m_mapPolygonTemplate.SetExprHost(ExprHost.MapPolygonTemplateHost, reportObjectModel);
			}
			if (m_mapCenterPointTemplate != null && ExprHost.MapPointTemplateHost != null)
			{
				m_mapCenterPointTemplate.SetExprHost(ExprHost.MapPointTemplateHost, reportObjectModel);
			}
		}

		internal new static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.UseCustomPolygonTemplate, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.MapPolygonTemplate, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapPolygonTemplate));
			list.Add(new MemberInfo(MemberName.UseCustomPointTemplate, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.MapPointTemplate, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapPointTemplate));
			return new Declaration(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapPolygon, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapSpatialElement, list);
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.UseCustomPolygonTemplate:
					writer.Write(m_useCustomPolygonTemplate);
					break;
				case MemberName.MapPolygonTemplate:
					writer.Write(m_mapPolygonTemplate);
					break;
				case MemberName.UseCustomPointTemplate:
					writer.Write(m_useCustomCenterPointTemplate);
					break;
				case MemberName.MapPointTemplate:
					writer.Write(m_mapCenterPointTemplate);
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
				case MemberName.UseCustomPolygonTemplate:
					m_useCustomPolygonTemplate = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.MapPolygonTemplate:
					m_mapPolygonTemplate = (MapPolygonTemplate)reader.ReadRIFObject();
					break;
				case MemberName.UseCustomPointTemplate:
					m_useCustomCenterPointTemplate = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.MapPointTemplate:
					m_mapCenterPointTemplate = (MapPointTemplate)reader.ReadRIFObject();
					break;
				default:
					Global.Tracer.Assert(condition: false);
					break;
				}
			}
		}

		public override Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapPolygon;
		}

		internal bool EvaluateUseCustomPolygonTemplate(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.InstancePath, reportScopeInstance);
			return context.ReportRuntime.EvaluateMapPolygonUseCustomPolygonTemplateExpression(this, m_map.Name);
		}

		internal bool EvaluateUseCustomCenterPointTemplate(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.InstancePath, reportScopeInstance);
			return context.ReportRuntime.EvaluateMapPolygonUseCustomPointTemplateExpression(this, m_map.Name);
		}
	}
}
