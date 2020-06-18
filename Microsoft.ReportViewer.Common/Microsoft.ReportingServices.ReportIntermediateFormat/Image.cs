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
	internal sealed class Image : ReportItem, IActionOwner, IPersistable
	{
		private Action m_action;

		private Microsoft.ReportingServices.OnDemandReportRendering.Image.SourceType m_source;

		private ExpressionInfo m_value;

		private ExpressionInfo m_MIMEType;

		private List<ExpressionInfo> m_tags;

		private Microsoft.ReportingServices.OnDemandReportRendering.Image.EmbeddingModes m_embeddingMode;

		private Microsoft.ReportingServices.OnDemandReportRendering.Image.Sizings m_sizing;

		[NonSerialized]
		private ImageExprHost m_exprHost;

		[NonSerialized]
		private List<string> m_fieldsUsedInValueExpression;

		[NonSerialized]
		private static readonly Declaration m_Declaration = GetDeclaration();

		[NonSerialized]
		internal static readonly byte[] TransparentImageBytes = new byte[43]
		{
			71,
			73,
			70,
			56,
			57,
			97,
			1,
			0,
			1,
			0,
			240,
			0,
			0,
			219,
			223,
			239,
			0,
			0,
			0,
			33,
			249,
			4,
			1,
			0,
			0,
			0,
			0,
			44,
			0,
			0,
			0,
			0,
			1,
			0,
			1,
			0,
			0,
			2,
			2,
			68,
			1,
			0,
			59
		};

		internal override Microsoft.ReportingServices.ReportProcessing.ObjectType ObjectType => Microsoft.ReportingServices.ReportProcessing.ObjectType.Image;

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

		internal Microsoft.ReportingServices.OnDemandReportRendering.Image.SourceType Source
		{
			get
			{
				return m_source;
			}
			set
			{
				m_source = value;
			}
		}

		internal ExpressionInfo Value
		{
			get
			{
				return m_value;
			}
			set
			{
				m_value = value;
			}
		}

		internal ExpressionInfo MIMEType
		{
			get
			{
				return m_MIMEType;
			}
			set
			{
				m_MIMEType = value;
			}
		}

		internal Microsoft.ReportingServices.OnDemandReportRendering.Image.Sizings Sizing
		{
			get
			{
				return m_sizing;
			}
			set
			{
				m_sizing = value;
			}
		}

		internal List<ExpressionInfo> Tags
		{
			get
			{
				return m_tags;
			}
			set
			{
				m_tags = value;
			}
		}

		internal Microsoft.ReportingServices.OnDemandReportRendering.Image.EmbeddingModes EmbeddingMode
		{
			get
			{
				return m_embeddingMode;
			}
			set
			{
				m_embeddingMode = value;
			}
		}

		internal ImageExprHost ImageExprHost => m_exprHost;

		Action IActionOwner.Action => m_action;

		List<string> IActionOwner.FieldsUsedInValueExpression
		{
			get
			{
				return m_fieldsUsedInValueExpression;
			}
			set
			{
				m_fieldsUsedInValueExpression = value;
			}
		}

		internal Image(ReportItem parent)
			: base(parent)
		{
		}

		internal Image(int id, ReportItem parent)
			: base(id, parent)
		{
		}

		internal override bool Initialize(InitializationContext context)
		{
			context.ObjectType = ObjectType;
			context.ObjectName = m_name;
			context.ExprHostBuilder.ImageStart(m_name);
			base.Initialize(context);
			if (m_visibility != null)
			{
				m_visibility.Initialize(context);
			}
			if (m_action != null)
			{
				m_action.Initialize(context);
			}
			if (m_value != null)
			{
				m_value.Initialize("Value", context);
				context.ExprHostBuilder.GenericValue(m_value);
				if (ExpressionInfo.Types.Constant == m_value.Type && m_source == Microsoft.ReportingServices.OnDemandReportRendering.Image.SourceType.External && !context.ReportContext.IsSupportedProtocol(m_value.StringValue, protocolRestriction: true))
				{
					context.ErrorContext.Register(ProcessingErrorCode.rsUnsupportedProtocol, Severity.Error, ObjectType, m_name, "Value", m_value.StringValue, "http://, https://, ftp://, file:, mailto:, or news:");
				}
			}
			if (m_MIMEType != null)
			{
				m_MIMEType.Initialize("MIMEType", context);
				context.ExprHostBuilder.ImageMIMEType(m_MIMEType);
			}
			if (m_tags != null)
			{
				for (int i = 0; i < m_tags.Count; i++)
				{
					ExpressionInfo expressionInfo = m_tags[i];
					expressionInfo.Initialize("Tag", context);
					_ = expressionInfo.Type;
				}
			}
			if (Microsoft.ReportingServices.OnDemandReportRendering.Image.SourceType.Embedded == m_source && m_embeddingMode == Microsoft.ReportingServices.OnDemandReportRendering.Image.EmbeddingModes.Inline)
			{
				Global.Tracer.Assert(m_value != null, "(null != m_value)");
				Microsoft.ReportingServices.ReportPublishing.PublishingValidator.ValidateEmbeddedImageName(m_value, context.EmbeddedImages, ObjectType, m_name, "Value", context.ErrorContext);
			}
			base.ExprHostID = context.ExprHostBuilder.ImageEnd();
			return true;
		}

		internal override object PublishClone(AutomaticSubtotalContext context)
		{
			Image image = (Image)base.PublishClone(context);
			if (m_action != null)
			{
				image.m_action = (Action)m_action.PublishClone(context);
			}
			if (m_value != null)
			{
				image.m_value = (ExpressionInfo)m_value.PublishClone(context);
			}
			if (m_MIMEType != null)
			{
				image.m_MIMEType = (ExpressionInfo)m_MIMEType.PublishClone(context);
			}
			if (m_tags != null)
			{
				image.m_tags = new List<ExpressionInfo>(m_tags.Count);
				foreach (ExpressionInfo tag in m_tags)
				{
					image.m_tags.Add((ExpressionInfo)tag.PublishClone(context));
				}
			}
			if (m_fieldsUsedInValueExpression != null)
			{
				image.m_fieldsUsedInValueExpression = new List<string>(m_fieldsUsedInValueExpression.Count);
				{
					foreach (string item in m_fieldsUsedInValueExpression)
					{
						image.m_fieldsUsedInValueExpression.Add((string)item.Clone());
					}
					return image;
				}
			}
			return image;
		}

		[SkipMemberStaticValidation(MemberName.Tag)]
		internal new static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.Source, Token.Enum));
			list.Add(new MemberInfo(MemberName.Value, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.MIMEType, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.Sizing, Token.Enum));
			list.Add(new MemberInfo(MemberName.Action, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Action));
			list.Add(new MemberInfo(MemberName.Tag, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo, Lifetime.Spanning(100, 200)));
			list.Add(new MemberInfo(MemberName.Tags, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo, Lifetime.AddedIn(200)));
			list.Add(new MemberInfo(MemberName.EmbeddingMode, Token.Enum, Lifetime.AddedIn(200)));
			return new Declaration(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Image, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ReportItem, list);
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.Source:
					writer.WriteEnum((int)m_source);
					break;
				case MemberName.Value:
					writer.Write(m_value);
					break;
				case MemberName.MIMEType:
					writer.Write(m_MIMEType);
					break;
				case MemberName.Sizing:
					writer.WriteEnum((int)m_sizing);
					break;
				case MemberName.Action:
					writer.Write(m_action);
					break;
				case MemberName.Tag:
				{
					ExpressionInfo persistableObj = null;
					if (m_tags != null && m_tags.Count > 0)
					{
						persistableObj = m_tags[0];
					}
					writer.Write(persistableObj);
					break;
				}
				case MemberName.Tags:
					writer.Write(m_tags);
					break;
				case MemberName.EmbeddingMode:
					writer.WriteEnum((int)m_embeddingMode);
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
				case MemberName.Source:
					m_source = (Microsoft.ReportingServices.OnDemandReportRendering.Image.SourceType)reader.ReadEnum();
					break;
				case MemberName.Value:
					m_value = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.MIMEType:
					m_MIMEType = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.Sizing:
					m_sizing = (Microsoft.ReportingServices.OnDemandReportRendering.Image.Sizings)reader.ReadEnum();
					break;
				case MemberName.Action:
					m_action = (Action)reader.ReadRIFObject();
					break;
				case MemberName.Tag:
				{
					ExpressionInfo expressionInfo = (ExpressionInfo)reader.ReadRIFObject();
					if (expressionInfo != null)
					{
						m_tags = new List<ExpressionInfo>(1)
						{
							expressionInfo
						};
					}
					break;
				}
				case MemberName.Tags:
					m_tags = reader.ReadGenericListOfRIFObjects<ExpressionInfo>();
					break;
				case MemberName.EmbeddingMode:
					m_embeddingMode = (Microsoft.ReportingServices.OnDemandReportRendering.Image.EmbeddingModes)reader.ReadEnum();
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
			return Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Image;
		}

		internal override void SetExprHost(ReportExprHost reportExprHost, ObjectModelImpl reportObjectModel)
		{
			if (base.ExprHostID >= 0)
			{
				Global.Tracer.Assert(reportExprHost != null && reportObjectModel != null, "(reportExprHost != null && reportObjectModel != null)");
				m_exprHost = reportExprHost.ImageHostsRemotable[base.ExprHostID];
				ReportItemSetExprHost(m_exprHost, reportObjectModel);
				if (m_action != null && m_exprHost.ActionInfoHost != null)
				{
					m_action.SetExprHost(m_exprHost.ActionInfoHost, reportObjectModel);
				}
			}
		}

		internal bool ShouldTrackFieldsUsedInValue()
		{
			if (Action != null)
			{
				return Action.TrackFieldsUsedInValueExpression;
			}
			return false;
		}

		internal string EvaluateMimeTypeExpression(IReportScopeInstance romInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(this, romInstance);
			return context.ReportRuntime.EvaluateImageMIMETypeExpression(this);
		}

		internal byte[] EvaluateBinaryValueExpression(IReportScopeInstance romInstance, OnDemandProcessingContext context, out bool errOccurred)
		{
			context.SetupContext(this, romInstance);
			return context.ReportRuntime.EvaluateImageBinaryValueExpression(this, out errOccurred);
		}

		internal string EvaluateStringValueExpression(IReportScopeInstance romInstance, OnDemandProcessingContext context, out bool errOccurred)
		{
			context.SetupContext(this, romInstance);
			return context.ReportRuntime.EvaluateImageStringValueExpression(this, out errOccurred);
		}

		internal Microsoft.ReportingServices.RdlExpressions.VariantResult EvaluateTagExpression(ExpressionInfo tag, IReportScopeInstance romInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(this, romInstance);
			return context.ReportRuntime.EvaluateImageTagExpression(this, tag);
		}
	}
}
