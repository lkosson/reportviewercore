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
	internal sealed class Action : IPersistable
	{
		private List<ActionItem> m_actionItemList;

		private Style m_styleClass;

		private bool m_trackFieldsUsedInValueExpression;

		[NonSerialized]
		private ActionInfoExprHost m_exprHost;

		[NonSerialized]
		private StyleProperties m_sharedStyleProperties;

		[NonSerialized]
		private bool m_noNonSharedStyleProps;

		[NonSerialized]
		private static readonly Declaration m_Declaration = GetDeclaration();

		internal Style StyleClass
		{
			get
			{
				return m_styleClass;
			}
			set
			{
				m_styleClass = value;
			}
		}

		internal List<ActionItem> ActionItems
		{
			get
			{
				return m_actionItemList;
			}
			set
			{
				m_actionItemList = value;
			}
		}

		internal StyleProperties SharedStyleProperties
		{
			get
			{
				return m_sharedStyleProperties;
			}
			set
			{
				m_sharedStyleProperties = value;
			}
		}

		internal bool NoNonSharedStyleProps
		{
			get
			{
				return m_noNonSharedStyleProps;
			}
			set
			{
				m_noNonSharedStyleProps = value;
			}
		}

		internal bool TrackFieldsUsedInValueExpression
		{
			get
			{
				return m_trackFieldsUsedInValueExpression;
			}
			set
			{
				m_trackFieldsUsedInValueExpression = value;
			}
		}

		internal Action()
		{
			m_actionItemList = new List<ActionItem>();
		}

		internal Action(ActionItem actionItem, bool computed)
		{
			m_actionItemList = new List<ActionItem>();
			m_actionItemList.Add(actionItem);
		}

		internal void Initialize(InitializationContext context)
		{
			Microsoft.ReportingServices.RdlExpressions.ExprHostBuilder exprHostBuilder = context.ExprHostBuilder;
			exprHostBuilder.ActionInfoStart();
			if (m_actionItemList != null)
			{
				for (int i = 0; i < m_actionItemList.Count; i++)
				{
					m_actionItemList[i].Initialize(context);
				}
			}
			if (m_styleClass != null)
			{
				m_styleClass.Initialize(context);
			}
			exprHostBuilder.ActionInfoEnd();
		}

		internal void SetExprHost(ActionInfoExprHost exprHost, ObjectModelImpl reportObjectModel)
		{
			Global.Tracer.Assert(exprHost != null && reportObjectModel != null, "(null != exprHost && null != reportObjectModel)");
			m_exprHost = exprHost;
			m_exprHost.SetReportObjectModel(reportObjectModel);
			if (exprHost.ActionItemHostsRemotable != null)
			{
				Global.Tracer.Assert(m_actionItemList != null, "(m_actionItemList != null)");
				for (int num = m_actionItemList.Count - 1; num >= 0; num--)
				{
					m_actionItemList[num].SetExprHost(exprHost.ActionItemHostsRemotable, reportObjectModel);
				}
			}
			if (m_styleClass != null)
			{
				m_styleClass.SetStyleExprHost(m_exprHost);
			}
		}

		internal bool ResetObjectModelForDrillthroughContext(ObjectModelImpl objectModel, IActionOwner actionOwner)
		{
			if (actionOwner.FieldsUsedInValueExpression == null)
			{
				bool flag = false;
				if (m_actionItemList != null)
				{
					for (int i = 0; i < m_actionItemList.Count; i++)
					{
						if (m_actionItemList[i].DrillthroughParameters != null && 0 < m_actionItemList[i].DrillthroughParameters.Count)
						{
							flag = true;
							break;
						}
					}
				}
				if (flag)
				{
					objectModel.FieldsImpl.ResetFieldsUsedInExpression();
					objectModel.AggregatesImpl.ResetFieldsUsedInExpression();
					return true;
				}
			}
			return false;
		}

		internal void GetSelectedItemsForDrillthroughContext(ObjectModelImpl objectModel, IActionOwner actionOwner)
		{
		}

		public object PublishClone(AutomaticSubtotalContext context)
		{
			Action action = (Action)MemberwiseClone();
			if (m_actionItemList != null)
			{
				action.m_actionItemList = new List<ActionItem>(m_actionItemList.Count);
				foreach (ActionItem actionItem in m_actionItemList)
				{
					action.m_actionItemList.Add((ActionItem)actionItem.PublishClone(context));
				}
			}
			if (m_styleClass != null)
			{
				action.m_styleClass = (Style)m_styleClass.PublishClone(context);
			}
			if (m_sharedStyleProperties != null)
			{
				action.m_sharedStyleProperties = (StyleProperties)m_sharedStyleProperties.PublishClone(context);
			}
			return action;
		}

		internal static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.ActionItemList, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ActionItem));
			list.Add(new MemberInfo(MemberName.StyleClass, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Style));
			list.Add(new MemberInfo(MemberName.TrackFieldsUsedInValueExpression, Token.Boolean));
			return new Declaration(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Action, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.None, list);
		}

		public void Serialize(IntermediateFormatWriter writer)
		{
			writer.RegisterDeclaration(m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.ActionItemList:
					writer.Write(m_actionItemList);
					break;
				case MemberName.StyleClass:
					writer.Write(m_styleClass);
					break;
				case MemberName.TrackFieldsUsedInValueExpression:
					writer.Write(m_trackFieldsUsedInValueExpression);
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
				case MemberName.ActionItemList:
					m_actionItemList = reader.ReadGenericListOfRIFObjects<ActionItem>();
					break;
				case MemberName.StyleClass:
					m_styleClass = (Style)reader.ReadRIFObject();
					break;
				case MemberName.TrackFieldsUsedInValueExpression:
					m_trackFieldsUsedInValueExpression = reader.ReadBoolean();
					break;
				default:
					Global.Tracer.Assert(condition: false);
					break;
				}
			}
		}

		public void ResolveReferences(Dictionary<Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
		}

		public Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Action;
		}
	}
}
