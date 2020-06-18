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
	internal sealed class ActionItem : IPersistable
	{
		private ExpressionInfo m_hyperLinkURL;

		private ExpressionInfo m_drillthroughReportName;

		private List<ParameterValue> m_drillthroughParameters;

		private ExpressionInfo m_drillthroughBookmarkLink;

		private ExpressionInfo m_bookmarkLink;

		private ExpressionInfo m_label;

		private int m_exprHostID = -1;

		private int m_computedIndex = -1;

		[NonSerialized]
		private ActionExprHost m_exprHost;

		[NonSerialized]
		private static readonly Declaration m_Declaration = GetDeclaration();

		internal ExpressionInfo HyperLinkURL
		{
			get
			{
				return m_hyperLinkURL;
			}
			set
			{
				m_hyperLinkURL = value;
			}
		}

		internal ExpressionInfo DrillthroughReportName
		{
			get
			{
				return m_drillthroughReportName;
			}
			set
			{
				m_drillthroughReportName = value;
			}
		}

		internal List<ParameterValue> DrillthroughParameters
		{
			get
			{
				return m_drillthroughParameters;
			}
			set
			{
				m_drillthroughParameters = value;
			}
		}

		internal ExpressionInfo DrillthroughBookmarkLink
		{
			get
			{
				return m_drillthroughBookmarkLink;
			}
			set
			{
				m_drillthroughBookmarkLink = value;
			}
		}

		internal ExpressionInfo BookmarkLink
		{
			get
			{
				return m_bookmarkLink;
			}
			set
			{
				m_bookmarkLink = value;
			}
		}

		internal ExpressionInfo Label
		{
			get
			{
				return m_label;
			}
			set
			{
				m_label = value;
			}
		}

		internal int ComputedIndex
		{
			get
			{
				return m_computedIndex;
			}
			set
			{
				m_computedIndex = value;
			}
		}

		internal int ExprHostID
		{
			get
			{
				return m_exprHostID;
			}
			set
			{
				m_exprHostID = value;
			}
		}

		internal ActionExprHost ExprHost => m_exprHost;

		internal void Initialize(InitializationContext context)
		{
			context.ExprHostBuilder.ActionStart();
			if (m_hyperLinkURL != null)
			{
				m_hyperLinkURL.Initialize("Hyperlink", context);
				context.ExprHostBuilder.ActionHyperlink(m_hyperLinkURL);
			}
			if (m_drillthroughReportName != null)
			{
				m_drillthroughReportName.Initialize("DrillthroughReportName", context);
				context.ExprHostBuilder.ActionDrillThroughReportName(m_drillthroughReportName);
			}
			if (m_drillthroughParameters != null)
			{
				for (int i = 0; i < m_drillthroughParameters.Count; i++)
				{
					ParameterValue parameterValue = m_drillthroughParameters[i];
					context.ExprHostBuilder.ActionDrillThroughParameterStart();
					parameterValue.Initialize("DrillthroughParameters", context, queryParam: false);
					parameterValue.ExprHostID = context.ExprHostBuilder.ActionDrillThroughParameterEnd();
				}
			}
			if (m_drillthroughBookmarkLink != null)
			{
				m_drillthroughBookmarkLink.Initialize("BookmarkLink", context);
				context.ExprHostBuilder.ActionDrillThroughBookmarkLink(m_drillthroughBookmarkLink);
			}
			if (m_bookmarkLink != null)
			{
				m_bookmarkLink.Initialize("BookmarkLink", context);
				context.ExprHostBuilder.ActionBookmarkLink(m_bookmarkLink);
			}
			if (m_label != null)
			{
				m_label.Initialize("Label", context);
				context.ExprHostBuilder.GenericLabel(m_label);
			}
			m_exprHostID = context.ExprHostBuilder.ActionEnd();
		}

		internal void SetExprHost(IList<ActionExprHost> actionItemExprHosts, ObjectModelImpl reportObjectModel)
		{
			if (m_exprHostID < 0)
			{
				return;
			}
			Global.Tracer.Assert(actionItemExprHosts != null && reportObjectModel != null, "(actionItemExprHosts != null && reportObjectModel != null)");
			m_exprHost = actionItemExprHosts[m_exprHostID];
			m_exprHost.SetReportObjectModel(reportObjectModel);
			if (m_exprHost.DrillThroughParameterHostsRemotable != null)
			{
				Global.Tracer.Assert(m_drillthroughParameters != null, "(m_drillthroughParameters != null)");
				for (int num = m_drillthroughParameters.Count - 1; num >= 0; num--)
				{
					m_drillthroughParameters[num].SetExprHost(m_exprHost.DrillThroughParameterHostsRemotable, reportObjectModel);
				}
			}
		}

		internal string EvaluateHyperLinkURL(IReportScopeInstance romInstance, OnDemandProcessingContext context, IInstancePath ownerItem, Microsoft.ReportingServices.ReportProcessing.ObjectType objectType, string objectName)
		{
			context.SetupContext(ownerItem, romInstance);
			return context.ReportRuntime.EvaluateReportItemHyperlinkURLExpression(this, m_hyperLinkURL, objectType, objectName);
		}

		internal string EvaluateDrillthroughReportName(IReportScopeInstance romInstance, OnDemandProcessingContext context, IInstancePath ownerItem, Microsoft.ReportingServices.ReportProcessing.ObjectType objectType, string objectName)
		{
			context.SetupContext(ownerItem, romInstance);
			return context.ReportRuntime.EvaluateReportItemDrillthroughReportName(this, m_drillthroughReportName, objectType, objectName);
		}

		internal string EvaluateBookmarkLink(IReportScopeInstance romInstance, OnDemandProcessingContext context, IInstancePath ownerItem, Microsoft.ReportingServices.ReportProcessing.ObjectType objectType, string objectName)
		{
			context.SetupContext(ownerItem, romInstance);
			return context.ReportRuntime.EvaluateReportItemBookmarkLinkExpression(this, m_bookmarkLink, objectType, objectName);
		}

		internal string EvaluateLabel(IReportScopeInstance romInstance, OnDemandProcessingContext context, IInstancePath ownerItem, Microsoft.ReportingServices.ReportProcessing.ObjectType objectType, string objectName)
		{
			context.SetupContext(ownerItem, romInstance);
			return context.ReportRuntime.EvaluateActionLabelExpression(this, m_label, objectType, objectName);
		}

		internal object EvaluateDrillthroughParamValue(IReportScopeInstance romInstance, OnDemandProcessingContext context, IInstancePath ownerItem, List<string> fieldsUsedInOwnerValue, ParameterValue paramValue, Microsoft.ReportingServices.ReportProcessing.ObjectType objectType, string objectName)
		{
			context.SetupContext(ownerItem, romInstance);
			Microsoft.ReportingServices.RdlExpressions.ReportRuntime reportRuntime = context.ReportRuntime;
			reportRuntime.FieldsUsedInCurrentActionOwnerValue = fieldsUsedInOwnerValue;
			Microsoft.ReportingServices.RdlExpressions.ParameterValueResult parameterValueResult = reportRuntime.EvaluateParameterValueExpression(paramValue, objectType, objectName, "DrillthroughParameterValue");
			reportRuntime.FieldsUsedInCurrentActionOwnerValue = null;
			return parameterValueResult.Value;
		}

		internal bool EvaluateDrillthroughParamOmit(IReportScopeInstance romInstance, OnDemandProcessingContext context, IInstancePath ownerItem, ParameterValue paramValue, Microsoft.ReportingServices.ReportProcessing.ObjectType objectType, string objectName)
		{
			context.SetupContext(ownerItem, romInstance);
			return context.ReportRuntime.EvaluateParamValueOmitExpression(paramValue, objectType, objectName);
		}

		internal static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.HyperLinkURL, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.DrillthroughReportName, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.DrillthroughParameters, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ParameterValue));
			list.Add(new MemberInfo(MemberName.DrillthroughBookmarkLink, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.BookmarkLink, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.Label, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.ExprHostID, Token.Int32));
			list.Add(new MemberInfo(MemberName.Index, Token.Int32));
			return new Declaration(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ActionItem, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.None, list);
		}

		public void Serialize(IntermediateFormatWriter writer)
		{
			writer.RegisterDeclaration(m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.HyperLinkURL:
					writer.Write(m_hyperLinkURL);
					break;
				case MemberName.DrillthroughReportName:
					writer.Write(m_drillthroughReportName);
					break;
				case MemberName.DrillthroughParameters:
					writer.Write(m_drillthroughParameters);
					break;
				case MemberName.DrillthroughBookmarkLink:
					writer.Write(m_drillthroughBookmarkLink);
					break;
				case MemberName.BookmarkLink:
					writer.Write(m_bookmarkLink);
					break;
				case MemberName.Label:
					writer.Write(m_label);
					break;
				case MemberName.ExprHostID:
					writer.Write(m_exprHostID);
					break;
				case MemberName.Index:
					writer.Write(m_computedIndex);
					break;
				default:
					Global.Tracer.Assert(condition: false);
					break;
				}
			}
		}

		public void Deserialize(IntermediateFormatReader reader)
		{
			reader.RegisterDeclaration(m_Declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.HyperLinkURL:
					m_hyperLinkURL = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.DrillthroughReportName:
					m_drillthroughReportName = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.DrillthroughParameters:
					m_drillthroughParameters = reader.ReadGenericListOfRIFObjects<ParameterValue>();
					break;
				case MemberName.DrillthroughBookmarkLink:
					m_drillthroughBookmarkLink = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.BookmarkLink:
					m_bookmarkLink = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.Label:
					m_label = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.ExprHostID:
					m_exprHostID = reader.ReadInt32();
					break;
				case MemberName.Index:
					m_computedIndex = reader.ReadInt32();
					break;
				default:
					Global.Tracer.Assert(condition: false);
					break;
				}
			}
		}

		public void ResolveReferences(Dictionary<Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
			Global.Tracer.Assert(condition: false);
		}

		public Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ActionItem;
		}

		public object PublishClone(AutomaticSubtotalContext context)
		{
			ActionItem actionItem = (ActionItem)MemberwiseClone();
			if (m_hyperLinkURL != null)
			{
				actionItem.m_hyperLinkURL = (ExpressionInfo)m_hyperLinkURL.PublishClone(context);
			}
			if (m_drillthroughReportName != null)
			{
				actionItem.m_drillthroughReportName = (ExpressionInfo)m_drillthroughReportName.PublishClone(context);
			}
			if (m_drillthroughParameters != null)
			{
				actionItem.m_drillthroughParameters = new List<ParameterValue>(m_drillthroughParameters.Count);
				foreach (ParameterValue drillthroughParameter in m_drillthroughParameters)
				{
					actionItem.m_drillthroughParameters.Add((ParameterValue)drillthroughParameter.PublishClone(context));
				}
			}
			if (m_drillthroughBookmarkLink != null)
			{
				actionItem.m_drillthroughBookmarkLink = (ExpressionInfo)m_drillthroughBookmarkLink.PublishClone(context);
			}
			if (m_bookmarkLink != null)
			{
				actionItem.m_bookmarkLink = (ExpressionInfo)m_bookmarkLink.PublishClone(context);
			}
			if (m_label != null)
			{
				actionItem.m_label = (ExpressionInfo)m_label.PublishClone(context);
			}
			return actionItem;
		}
	}
}
