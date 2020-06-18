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
	internal sealed class MapLineTemplate : MapSpatialElementTemplate, IPersistable
	{
		[NonSerialized]
		private static readonly Declaration m_Declaration = GetDeclaration();

		private ExpressionInfo m_width;

		private ExpressionInfo m_labelPlacement;

		internal ExpressionInfo Width
		{
			get
			{
				return m_width;
			}
			set
			{
				m_width = value;
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

		internal new MapLineTemplateExprHost ExprHost => (MapLineTemplateExprHost)m_exprHost;

		internal MapLineTemplate()
		{
		}

		internal MapLineTemplate(MapLineLayer mapLineLayer, Map map, int id)
			: base(mapLineLayer, map, id)
		{
		}

		internal override void Initialize(InitializationContext context)
		{
			context.ExprHostBuilder.MapLineTemplateStart();
			base.Initialize(context);
			if (m_width != null)
			{
				m_width.Initialize("Width", context);
				context.ExprHostBuilder.MapLineTemplateWidth(m_width);
			}
			if (m_labelPlacement != null)
			{
				m_labelPlacement.Initialize("LabelPlacement", context);
				context.ExprHostBuilder.MapLineTemplateLabelPlacement(m_labelPlacement);
			}
			context.ExprHostBuilder.MapLineTemplateEnd();
		}

		internal override object PublishClone(AutomaticSubtotalContext context)
		{
			MapLineTemplate mapLineTemplate = (MapLineTemplate)base.PublishClone(context);
			if (m_width != null)
			{
				mapLineTemplate.m_width = (ExpressionInfo)m_width.PublishClone(context);
			}
			if (m_labelPlacement != null)
			{
				mapLineTemplate.m_labelPlacement = (ExpressionInfo)m_labelPlacement.PublishClone(context);
			}
			return mapLineTemplate;
		}

		internal void SetExprHost(MapLineTemplateExprHost exprHost, ObjectModelImpl reportObjectModel)
		{
			Global.Tracer.Assert(exprHost != null && reportObjectModel != null, "(exprHost != null && reportObjectModel != null)");
			SetExprHost((MapSpatialElementTemplateExprHost)exprHost, reportObjectModel);
		}

		internal new static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.Width, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.LabelPlacement, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			return new Declaration(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapLineTemplate, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapSpatialElementTemplate, list);
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.Width:
					writer.Write(m_width);
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
				case MemberName.Width:
					m_width = (ExpressionInfo)reader.ReadRIFObject();
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
			return Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapLineTemplate;
		}

		internal string EvaluateWidth(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.InstancePath, reportScopeInstance);
			return context.ReportRuntime.EvaluateMapLineTemplateWidthExpression(this, m_map.Name);
		}

		internal MapLineLabelPlacement EvaluateLabelPlacement(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.InstancePath, reportScopeInstance);
			return EnumTranslator.TranslateMapLineLabelPlacement(context.ReportRuntime.EvaluateMapLineTemplateLabelPlacementExpression(this, m_map.Name), context.ReportRuntime);
		}
	}
}
