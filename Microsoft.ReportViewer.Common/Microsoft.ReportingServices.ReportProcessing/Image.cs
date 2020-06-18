using Microsoft.ReportingServices.ReportProcessing.ExprHostObjectModel;
using Microsoft.ReportingServices.ReportProcessing.Persistence;
using Microsoft.ReportingServices.ReportProcessing.ReportObjectModel;
using System;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class Image : ReportItem, IActionOwner
	{
		internal enum SourceType
		{
			External,
			Embedded,
			Database
		}

		public enum Sizings
		{
			AutoSize,
			Fit,
			FitProportional,
			Clip
		}

		private Action m_action;

		private SourceType m_source;

		private ExpressionInfo m_value;

		private ExpressionInfo m_MIMEType;

		private Sizings m_sizing;

		[NonSerialized]
		private ImageExprHost m_exprHost;

		[NonSerialized]
		private List<string> m_fieldsUsedInValueExpression;

		internal override ObjectType ObjectType => ObjectType.Image;

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

		internal SourceType Source
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

		internal Sizings Sizing
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
				m_visibility.Initialize(context, isContainer: false, tableRowCol: false);
			}
			if (m_action != null)
			{
				m_action.Initialize(context);
			}
			if (m_value != null)
			{
				m_value.Initialize("Value", context);
				context.ExprHostBuilder.GenericValue(m_value);
				if (ExpressionInfo.Types.Constant == m_value.Type && m_source == SourceType.External && !context.ReportContext.IsSupportedProtocol(m_value.Value, protocolRestriction: true))
				{
					context.ErrorContext.Register(ProcessingErrorCode.rsUnsupportedProtocol, Severity.Error, ObjectType, m_name, "Value", m_value.Value, "http://, https://, ftp://, file:, mailto:, or news:");
				}
			}
			if (m_MIMEType != null)
			{
				m_MIMEType.Initialize("MIMEType", context);
				context.ExprHostBuilder.ImageMIMEType(m_MIMEType);
			}
			if (SourceType.Embedded == m_source)
			{
				Global.Tracer.Assert(m_value != null);
				PublishingValidator.ValidateEmbeddedImageName(m_value, context.EmbeddedImages, ObjectType, m_name, "Value", context.ErrorContext);
			}
			base.ExprHostID = context.ExprHostBuilder.ImageEnd();
			return true;
		}

		internal override void SetExprHost(ReportExprHost reportExprHost, ObjectModelImpl reportObjectModel)
		{
			if (base.ExprHostID < 0)
			{
				return;
			}
			Global.Tracer.Assert(reportExprHost != null && reportObjectModel != null);
			m_exprHost = reportExprHost.ImageHostsRemotable[base.ExprHostID];
			ReportItemSetExprHost(m_exprHost, reportObjectModel);
			if (m_action != null)
			{
				if (m_exprHost.ActionInfoHost != null)
				{
					m_action.SetExprHost(m_exprHost.ActionInfoHost, reportObjectModel);
				}
				else if (m_exprHost.ActionHost != null)
				{
					m_action.SetExprHost(m_exprHost.ActionHost, reportObjectModel);
				}
			}
		}

		internal override void ProcessDrillthroughAction(ReportProcessing.ProcessingContext processingContext, NonComputedUniqueNames nonCompNames)
		{
			if (m_action != null && nonCompNames != null)
			{
				m_action.ProcessDrillthroughAction(processingContext, nonCompNames.UniqueName);
			}
		}

		internal new static Declaration GetDeclaration()
		{
			MemberInfoList memberInfoList = new MemberInfoList();
			memberInfoList.Add(new MemberInfo(MemberName.Action, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.Action));
			memberInfoList.Add(new MemberInfo(MemberName.Source, Token.Enum));
			memberInfoList.Add(new MemberInfo(MemberName.Value, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.ExpressionInfo));
			memberInfoList.Add(new MemberInfo(MemberName.MIMEType, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.ExpressionInfo));
			memberInfoList.Add(new MemberInfo(MemberName.Sizing, Token.Enum));
			return new Declaration(Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.ReportItem, memberInfoList);
		}
	}
}
