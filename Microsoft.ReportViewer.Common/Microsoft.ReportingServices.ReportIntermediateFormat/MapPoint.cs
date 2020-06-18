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
	internal sealed class MapPoint : MapSpatialElement, IPersistable
	{
		[NonSerialized]
		private static readonly Declaration m_Declaration = GetDeclaration();

		private ExpressionInfo m_useCustomPointTemplate;

		private MapPointTemplate m_mapPointTemplate;

		internal ExpressionInfo UseCustomPointTemplate
		{
			get
			{
				return m_useCustomPointTemplate;
			}
			set
			{
				m_useCustomPointTemplate = value;
			}
		}

		internal MapPointTemplate MapPointTemplate
		{
			get
			{
				return m_mapPointTemplate;
			}
			set
			{
				m_mapPointTemplate = value;
			}
		}

		internal new MapPointExprHost ExprHost => (MapPointExprHost)m_exprHost;

		internal MapPoint()
		{
		}

		internal MapPoint(MapPointLayer mapPointLayer, Map map)
			: base(mapPointLayer, map)
		{
		}

		internal override void Initialize(InitializationContext context, int index)
		{
			context.ExprHostBuilder.MapPointStart(index.ToString(CultureInfo.InvariantCulture.NumberFormat));
			base.Initialize(context, index);
			if (m_useCustomPointTemplate != null)
			{
				m_useCustomPointTemplate.Initialize("UseCustomPointTemplate", context);
				context.ExprHostBuilder.MapPointUseCustomPointTemplate(m_useCustomPointTemplate);
			}
			if (m_mapPointTemplate != null)
			{
				m_mapPointTemplate.Initialize(context);
			}
			m_exprHostID = context.ExprHostBuilder.MapPointEnd();
		}

		internal override object PublishClone(AutomaticSubtotalContext context)
		{
			MapPoint mapPoint = (MapPoint)base.PublishClone(context);
			if (m_useCustomPointTemplate != null)
			{
				mapPoint.m_useCustomPointTemplate = (ExpressionInfo)m_useCustomPointTemplate.PublishClone(context);
			}
			if (m_mapPointTemplate != null)
			{
				mapPoint.m_mapPointTemplate = (MapPointTemplate)m_mapPointTemplate.PublishClone(context);
			}
			return mapPoint;
		}

		internal void SetExprHost(MapPointExprHost exprHost, ObjectModelImpl reportObjectModel)
		{
			Global.Tracer.Assert(exprHost != null && reportObjectModel != null, "(exprHost != null && reportObjectModel != null)");
			SetExprHost((MapSpatialElementExprHost)exprHost, reportObjectModel);
			if (m_mapPointTemplate != null && ExprHost.MapPointTemplateHost != null)
			{
				m_mapPointTemplate.SetExprHost(ExprHost.MapPointTemplateHost, reportObjectModel);
			}
		}

		internal new static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.UseCustomPointTemplate, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.MapPointTemplate, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapPointTemplate));
			return new Declaration(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapPoint, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapSpatialElement, list);
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.UseCustomPointTemplate:
					writer.Write(m_useCustomPointTemplate);
					break;
				case MemberName.MapPointTemplate:
					writer.Write(m_mapPointTemplate);
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
				case MemberName.UseCustomPointTemplate:
					m_useCustomPointTemplate = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.MapPointTemplate:
					m_mapPointTemplate = (MapPointTemplate)reader.ReadRIFObject();
					break;
				default:
					Global.Tracer.Assert(condition: false);
					break;
				}
			}
		}

		public override Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapPoint;
		}

		internal bool EvaluateUseCustomPointTemplate(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.InstancePath, reportScopeInstance);
			return context.ReportRuntime.EvaluateMapPointUseCustomPointTemplateExpression(this, m_map.Name);
		}
	}
}
