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
	internal class MapPointTemplate : MapSpatialElementTemplate, IPersistable
	{
		[NonSerialized]
		private static readonly Declaration m_Declaration = GetDeclaration();

		private ExpressionInfo m_size;

		private ExpressionInfo m_labelPlacement;

		internal ExpressionInfo Size
		{
			get
			{
				return m_size;
			}
			set
			{
				m_size = value;
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

		internal new MapPointTemplateExprHost ExprHost => (MapPointTemplateExprHost)m_exprHost;

		internal MapPointTemplate()
		{
		}

		internal MapPointTemplate(MapVectorLayer mapVectorLayer, Map map, int id)
			: base(mapVectorLayer, map, id)
		{
		}

		internal override void Initialize(InitializationContext context)
		{
			base.Initialize(context);
			if (m_size != null)
			{
				m_size.Initialize("Size", context);
				context.ExprHostBuilder.MapPointTemplateSize(m_size);
			}
			if (m_labelPlacement != null)
			{
				m_labelPlacement.Initialize("LabelPlacement", context);
				context.ExprHostBuilder.MapPointTemplateLabelPlacement(m_labelPlacement);
			}
		}

		internal override object PublishClone(AutomaticSubtotalContext context)
		{
			MapPointTemplate mapPointTemplate = (MapPointTemplate)base.PublishClone(context);
			if (m_size != null)
			{
				mapPointTemplate.m_size = (ExpressionInfo)m_size.PublishClone(context);
			}
			if (m_labelPlacement != null)
			{
				mapPointTemplate.m_labelPlacement = (ExpressionInfo)m_labelPlacement.PublishClone(context);
			}
			return mapPointTemplate;
		}

		internal virtual void SetExprHost(MapPointTemplateExprHost exprHost, ObjectModelImpl reportObjectModel)
		{
			Global.Tracer.Assert(exprHost != null && reportObjectModel != null, "(exprHost != null && reportObjectModel != null)");
			SetExprHost((MapSpatialElementTemplateExprHost)exprHost, reportObjectModel);
		}

		internal new static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.Size, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.LabelPlacement, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			return new Declaration(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapPointTemplate, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapSpatialElementTemplate, list);
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.Size:
					writer.Write(m_size);
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
				case MemberName.Size:
					m_size = (ExpressionInfo)reader.ReadRIFObject();
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
			return Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapPointTemplate;
		}

		internal string EvaluateSize(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.InstancePath, reportScopeInstance);
			return context.ReportRuntime.EvaluateMapPointTemplateSizeExpression(this, m_map.Name);
		}

		internal MapPointLabelPlacement EvaluateLabelPlacement(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.InstancePath, reportScopeInstance);
			return EnumTranslator.TranslateMapPointLabelPlacement(context.ReportRuntime.EvaluateMapPointTemplateLabelPlacementExpression(this, m_map.Name), context.ReportRuntime);
		}
	}
}
