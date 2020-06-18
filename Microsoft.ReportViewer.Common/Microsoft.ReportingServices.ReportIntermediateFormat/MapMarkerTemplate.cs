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
	internal sealed class MapMarkerTemplate : MapPointTemplate, IPersistable
	{
		[NonSerialized]
		private static readonly Declaration m_Declaration = GetDeclaration();

		private MapMarker m_mapMarker;

		internal MapMarker MapMarker
		{
			get
			{
				return m_mapMarker;
			}
			set
			{
				m_mapMarker = value;
			}
		}

		internal new MapMarkerTemplateExprHost ExprHost => (MapMarkerTemplateExprHost)m_exprHost;

		internal MapMarkerTemplate()
		{
		}

		internal MapMarkerTemplate(MapVectorLayer mapVectorLayer, Map map, int id)
			: base(mapVectorLayer, map, id)
		{
		}

		internal override void Initialize(InitializationContext context)
		{
			context.ExprHostBuilder.MapMarkerTemplateStart();
			base.Initialize(context);
			if (m_mapMarker != null)
			{
				m_mapMarker.Initialize(context);
			}
			context.ExprHostBuilder.MapMarkerTemplateEnd();
		}

		internal override object PublishClone(AutomaticSubtotalContext context)
		{
			MapMarkerTemplate mapMarkerTemplate = (MapMarkerTemplate)base.PublishClone(context);
			if (m_mapMarker != null)
			{
				mapMarkerTemplate.m_mapMarker = (MapMarker)m_mapMarker.PublishClone(context);
			}
			return mapMarkerTemplate;
		}

		internal override void SetExprHost(MapPointTemplateExprHost exprHost, ObjectModelImpl reportObjectModel)
		{
			Global.Tracer.Assert(exprHost != null && reportObjectModel != null, "(exprHost != null && reportObjectModel != null)");
			base.SetExprHost(exprHost, reportObjectModel);
			if (m_mapMarker != null && ExprHost.MapMarkerHost != null)
			{
				m_mapMarker.SetExprHost(ExprHost.MapMarkerHost, reportObjectModel);
			}
		}

		internal new static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.MapMarker, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapMarker));
			return new Declaration(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapMarkerTemplate, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapPointTemplate, list);
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(m_Declaration);
			while (writer.NextMember())
			{
				MemberName memberName = writer.CurrentMember.MemberName;
				if (memberName == MemberName.MapMarker)
				{
					writer.Write(m_mapMarker);
				}
				else
				{
					Global.Tracer.Assert(condition: false);
				}
			}
		}

		public override void Deserialize(IntermediateFormatReader reader)
		{
			base.Deserialize(reader);
			reader.RegisterDeclaration(m_Declaration);
			while (reader.NextMember())
			{
				MemberName memberName = reader.CurrentMember.MemberName;
				if (memberName == MemberName.MapMarker)
				{
					m_mapMarker = (MapMarker)reader.ReadRIFObject();
				}
				else
				{
					Global.Tracer.Assert(condition: false);
				}
			}
		}

		public override Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapMarkerTemplate;
		}
	}
}
