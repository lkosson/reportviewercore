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
	internal sealed class MapTitle : MapDockableSubItem, IPersistable
	{
		[NonSerialized]
		private static readonly Declaration m_Declaration = GetDeclaration();

		private ExpressionInfo m_text;

		private ExpressionInfo m_angle;

		private ExpressionInfo m_textShadowOffset;

		private string m_name;

		internal string Name
		{
			get
			{
				return m_name;
			}
			set
			{
				m_name = value;
			}
		}

		internal ExpressionInfo Text
		{
			get
			{
				return m_text;
			}
			set
			{
				m_text = value;
			}
		}

		internal ExpressionInfo Angle
		{
			get
			{
				return m_angle;
			}
			set
			{
				m_angle = value;
			}
		}

		internal ExpressionInfo TextShadowOffset
		{
			get
			{
				return m_textShadowOffset;
			}
			set
			{
				m_textShadowOffset = value;
			}
		}

		internal new MapTitleExprHost ExprHost => (MapTitleExprHost)m_exprHost;

		internal MapTitle()
		{
		}

		internal MapTitle(Map map, int id)
			: base(map, id)
		{
		}

		internal override void Initialize(InitializationContext context)
		{
			context.ExprHostBuilder.MapTitleStart(m_name);
			base.Initialize(context);
			if (m_text != null)
			{
				m_text.Initialize("Text", context);
				context.ExprHostBuilder.MapTitleText(m_text);
			}
			if (m_angle != null)
			{
				m_angle.Initialize("Angle", context);
				context.ExprHostBuilder.MapTitleAngle(m_angle);
			}
			if (m_textShadowOffset != null)
			{
				m_textShadowOffset.Initialize("TextShadowOffset", context);
				context.ExprHostBuilder.MapTitleTextShadowOffset(m_textShadowOffset);
			}
			m_exprHostID = context.ExprHostBuilder.MapTitleEnd();
		}

		internal override object PublishClone(AutomaticSubtotalContext context)
		{
			MapTitle mapTitle = (MapTitle)base.PublishClone(context);
			if (m_text != null)
			{
				mapTitle.m_text = (ExpressionInfo)m_text.PublishClone(context);
			}
			if (m_angle != null)
			{
				mapTitle.m_angle = (ExpressionInfo)m_angle.PublishClone(context);
			}
			if (m_textShadowOffset != null)
			{
				mapTitle.m_textShadowOffset = (ExpressionInfo)m_textShadowOffset.PublishClone(context);
			}
			return mapTitle;
		}

		internal void SetExprHost(MapTitleExprHost exprHost, ObjectModelImpl reportObjectModel)
		{
			Global.Tracer.Assert(exprHost != null && reportObjectModel != null, "(exprHost != null && reportObjectModel != null)");
			SetExprHost((MapDockableSubItemExprHost)exprHost, reportObjectModel);
		}

		internal new static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.Name, Token.String));
			list.Add(new MemberInfo(MemberName.Text, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.Angle, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.TextShadowOffset, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			return new Declaration(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapTitle, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapDockableSubItem, list);
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.Name:
					writer.Write(m_name);
					break;
				case MemberName.Text:
					writer.Write(m_text);
					break;
				case MemberName.Angle:
					writer.Write(m_angle);
					break;
				case MemberName.TextShadowOffset:
					writer.Write(m_textShadowOffset);
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
				case MemberName.Name:
					m_name = reader.ReadString();
					break;
				case MemberName.Text:
					m_text = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.Angle:
					m_angle = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.TextShadowOffset:
					m_textShadowOffset = (ExpressionInfo)reader.ReadRIFObject();
					break;
				default:
					Global.Tracer.Assert(condition: false);
					break;
				}
			}
		}

		public override Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapTitle;
		}

		internal string EvaluateText(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_map, reportScopeInstance);
			Microsoft.ReportingServices.RdlExpressions.VariantResult result = context.ReportRuntime.EvaluateMapTitleTextExpression(this, m_map.Name);
			return m_map.GetFormattedStringFromValue(ref result, context);
		}

		internal double EvaluateAngle(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_map, reportScopeInstance);
			return context.ReportRuntime.EvaluateMapTitleAngleExpression(this, m_map.Name);
		}

		internal string EvaluateTextShadowOffset(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_map, reportScopeInstance);
			return context.ReportRuntime.EvaluateMapTitleTextShadowOffsetExpression(this, m_map.Name);
		}
	}
}
