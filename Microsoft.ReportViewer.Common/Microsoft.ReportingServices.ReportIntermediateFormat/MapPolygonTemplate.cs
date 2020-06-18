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
	internal sealed class MapPolygonTemplate : MapSpatialElementTemplate, IPersistable
	{
		[NonSerialized]
		private static readonly Declaration m_Declaration = GetDeclaration();

		private ExpressionInfo m_scaleFactor;

		private ExpressionInfo m_centerPointOffsetX;

		private ExpressionInfo m_centerPointOffsetY;

		private ExpressionInfo m_showLabel;

		private ExpressionInfo m_labelPlacement;

		internal ExpressionInfo ScaleFactor
		{
			get
			{
				return m_scaleFactor;
			}
			set
			{
				m_scaleFactor = value;
			}
		}

		internal ExpressionInfo CenterPointOffsetX
		{
			get
			{
				return m_centerPointOffsetX;
			}
			set
			{
				m_centerPointOffsetX = value;
			}
		}

		internal ExpressionInfo CenterPointOffsetY
		{
			get
			{
				return m_centerPointOffsetY;
			}
			set
			{
				m_centerPointOffsetY = value;
			}
		}

		internal ExpressionInfo ShowLabel
		{
			get
			{
				return m_showLabel;
			}
			set
			{
				m_showLabel = value;
			}
		}

		internal ExpressionInfo LabelPlacement
		{
			get
			{
				return m_labelPlacement;
			}
			set
			{
				m_labelPlacement = value;
			}
		}

		internal new MapPolygonTemplateExprHost ExprHost => (MapPolygonTemplateExprHost)m_exprHost;

		internal MapPolygonTemplate()
		{
		}

		internal MapPolygonTemplate(MapPolygonLayer mapPolygonLayer, Map map, int id)
			: base(mapPolygonLayer, map, id)
		{
		}

		internal override void Initialize(InitializationContext context)
		{
			context.ExprHostBuilder.MapPolygonTemplateStart();
			base.Initialize(context);
			if (m_scaleFactor != null)
			{
				m_scaleFactor.Initialize("ScaleFactor", context);
				context.ExprHostBuilder.MapPolygonTemplateScaleFactor(m_scaleFactor);
			}
			if (m_centerPointOffsetX != null)
			{
				m_centerPointOffsetX.Initialize("CenterPointOffsetX", context);
				context.ExprHostBuilder.MapPolygonTemplateCenterPointOffsetX(m_centerPointOffsetX);
			}
			if (m_centerPointOffsetY != null)
			{
				m_centerPointOffsetY.Initialize("CenterPointOffsetY", context);
				context.ExprHostBuilder.MapPolygonTemplateCenterPointOffsetY(m_centerPointOffsetY);
			}
			if (m_showLabel != null)
			{
				m_showLabel.Initialize("ShowLabel", context);
				context.ExprHostBuilder.MapPolygonTemplateShowLabel(m_showLabel);
			}
			if (m_labelPlacement != null)
			{
				m_labelPlacement.Initialize("LabelPlacement", context);
				context.ExprHostBuilder.MapPolygonTemplateLabelPlacement(m_labelPlacement);
			}
			context.ExprHostBuilder.MapPolygonTemplateEnd();
		}

		internal override object PublishClone(AutomaticSubtotalContext context)
		{
			MapPolygonTemplate mapPolygonTemplate = (MapPolygonTemplate)base.PublishClone(context);
			if (m_scaleFactor != null)
			{
				mapPolygonTemplate.m_scaleFactor = (ExpressionInfo)m_scaleFactor.PublishClone(context);
			}
			if (m_centerPointOffsetX != null)
			{
				mapPolygonTemplate.m_centerPointOffsetX = (ExpressionInfo)m_centerPointOffsetX.PublishClone(context);
			}
			if (m_centerPointOffsetY != null)
			{
				mapPolygonTemplate.m_centerPointOffsetY = (ExpressionInfo)m_centerPointOffsetY.PublishClone(context);
			}
			if (m_showLabel != null)
			{
				mapPolygonTemplate.m_showLabel = (ExpressionInfo)m_showLabel.PublishClone(context);
			}
			if (m_labelPlacement != null)
			{
				mapPolygonTemplate.m_labelPlacement = (ExpressionInfo)m_labelPlacement.PublishClone(context);
			}
			return mapPolygonTemplate;
		}

		internal void SetExprHost(MapPolygonTemplateExprHost exprHost, ObjectModelImpl reportObjectModel)
		{
			Global.Tracer.Assert(exprHost != null && reportObjectModel != null, "(exprHost != null && reportObjectModel != null)");
			SetExprHost((MapSpatialElementTemplateExprHost)exprHost, reportObjectModel);
		}

		internal new static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.ScaleFactor, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.CenterPointOffsetX, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.CenterPointOffsetY, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.ShowLabel, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.LabelPlacement, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			return new Declaration(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapPolygonTemplate, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapSpatialElementTemplate, list);
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.ScaleFactor:
					writer.Write(m_scaleFactor);
					break;
				case MemberName.CenterPointOffsetX:
					writer.Write(m_centerPointOffsetX);
					break;
				case MemberName.CenterPointOffsetY:
					writer.Write(m_centerPointOffsetY);
					break;
				case MemberName.ShowLabel:
					writer.Write(m_showLabel);
					break;
				case MemberName.LabelPlacement:
					writer.Write(m_labelPlacement);
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
				case MemberName.ScaleFactor:
					m_scaleFactor = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.CenterPointOffsetX:
					m_centerPointOffsetX = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.CenterPointOffsetY:
					m_centerPointOffsetY = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.ShowLabel:
					m_showLabel = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.LabelPlacement:
					m_labelPlacement = (ExpressionInfo)reader.ReadRIFObject();
					break;
				default:
					Global.Tracer.Assert(condition: false);
					break;
				}
			}
		}

		public override Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapPolygonTemplate;
		}

		internal double EvaluateScaleFactor(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.InstancePath, reportScopeInstance);
			return context.ReportRuntime.EvaluateMapPolygonTemplateScaleFactorExpression(this, m_map.Name);
		}

		internal double EvaluateCenterPointOffsetX(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.InstancePath, reportScopeInstance);
			return context.ReportRuntime.EvaluateMapPolygonTemplateCenterPointOffsetXExpression(this, m_map.Name);
		}

		internal double EvaluateCenterPointOffsetY(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.InstancePath, reportScopeInstance);
			return context.ReportRuntime.EvaluateMapPolygonTemplateCenterPointOffsetYExpression(this, m_map.Name);
		}

		internal MapAutoBool EvaluateShowLabel(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.InstancePath, reportScopeInstance);
			return EnumTranslator.TranslateMapAutoBool(context.ReportRuntime.EvaluateMapPolygonTemplateShowLabelExpression(this, m_map.Name), context.ReportRuntime);
		}

		internal MapPolygonLabelPlacement EvaluateLabelPlacement(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.InstancePath, reportScopeInstance);
			return EnumTranslator.TranslateMapPolygonLabelPlacement(context.ReportRuntime.EvaluateMapPolygonTemplateLabelPlacementExpression(this, m_map.Name), context.ReportRuntime);
		}
	}
}
