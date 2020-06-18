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
	internal sealed class MapLine : MapSpatialElement, IPersistable
	{
		[NonSerialized]
		private static readonly Declaration m_Declaration = GetDeclaration();

		private ExpressionInfo m_useCustomLineTemplate;

		private MapLineTemplate m_mapLineTemplate;

		internal ExpressionInfo UseCustomLineTemplate
		{
			get
			{
				return m_useCustomLineTemplate;
			}
			set
			{
				m_useCustomLineTemplate = value;
			}
		}

		internal MapLineTemplate MapLineTemplate
		{
			get
			{
				return m_mapLineTemplate;
			}
			set
			{
				m_mapLineTemplate = value;
			}
		}

		internal new MapLineExprHost ExprHost => (MapLineExprHost)m_exprHost;

		internal MapLine()
		{
		}

		internal MapLine(MapLineLayer mapLineLayer, Map map)
			: base(mapLineLayer, map)
		{
		}

		internal override void Initialize(InitializationContext context, int index)
		{
			context.ExprHostBuilder.MapLineStart(index.ToString(CultureInfo.InvariantCulture.NumberFormat));
			base.Initialize(context, index);
			if (m_useCustomLineTemplate != null)
			{
				m_useCustomLineTemplate.Initialize("UseCustomLineTemplate", context);
				context.ExprHostBuilder.MapLineUseCustomLineTemplate(m_useCustomLineTemplate);
			}
			if (m_mapLineTemplate != null)
			{
				m_mapLineTemplate.Initialize(context);
			}
			m_exprHostID = context.ExprHostBuilder.MapLineEnd();
		}

		internal override object PublishClone(AutomaticSubtotalContext context)
		{
			MapLine mapLine = (MapLine)base.PublishClone(context);
			if (m_useCustomLineTemplate != null)
			{
				mapLine.m_useCustomLineTemplate = (ExpressionInfo)m_useCustomLineTemplate.PublishClone(context);
			}
			if (m_mapLineTemplate != null)
			{
				mapLine.m_mapLineTemplate = (MapLineTemplate)m_mapLineTemplate.PublishClone(context);
			}
			return mapLine;
		}

		internal void SetExprHost(MapLineExprHost exprHost, ObjectModelImpl reportObjectModel)
		{
			Global.Tracer.Assert(exprHost != null && reportObjectModel != null, "(exprHost != null && reportObjectModel != null)");
			SetExprHost((MapSpatialElementExprHost)exprHost, reportObjectModel);
			if (m_mapLineTemplate != null && ExprHost.MapLineTemplateHost != null)
			{
				m_mapLineTemplate.SetExprHost(ExprHost.MapLineTemplateHost, reportObjectModel);
			}
		}

		internal new static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.UseCustomLineTemplate, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.MapLineTemplate, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapLineTemplate));
			return new Declaration(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapLine, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapSpatialElement, list);
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.UseCustomLineTemplate:
					writer.Write(m_useCustomLineTemplate);
					break;
				case MemberName.MapLineTemplate:
					writer.Write(m_mapLineTemplate);
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
				case MemberName.UseCustomLineTemplate:
					m_useCustomLineTemplate = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.MapLineTemplate:
					m_mapLineTemplate = (MapLineTemplate)reader.ReadRIFObject();
					break;
				default:
					Global.Tracer.Assert(condition: false);
					break;
				}
			}
		}

		public override Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapLine;
		}

		internal bool EvaluateUseCustomLineTemplate(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.InstancePath, reportScopeInstance);
			return context.ReportRuntime.EvaluateMapLineUseCustomLineTemplateExpression(this, m_map.Name);
		}
	}
}
