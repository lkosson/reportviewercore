using Microsoft.ReportingServices.ReportProcessing.ExprHostObjectModel;
using Microsoft.ReportingServices.ReportProcessing.Persistence;
using Microsoft.ReportingServices.ReportProcessing.ReportObjectModel;
using Microsoft.ReportingServices.ReportRendering;
using System;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class Action
	{
		private ActionItemList m_actionItemList;

		private Style m_styleClass;

		private int m_computedActionItemsCount;

		[NonSerialized]
		private ActionInfoExprHost m_exprHost;

		[NonSerialized]
		private StyleProperties m_sharedStyleProperties;

		[NonSerialized]
		private bool m_noNonSharedStyleProps;

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

		internal ActionItemList ActionItems
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

		internal int ComputedActionItemsCount
		{
			get
			{
				return m_computedActionItemsCount;
			}
			set
			{
				m_computedActionItemsCount = value;
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

		internal Action(ActionItem actionItem, bool computed)
		{
			m_actionItemList = new ActionItemList();
			m_actionItemList.Add(actionItem);
			if (computed)
			{
				m_computedActionItemsCount = 1;
			}
		}

		internal Action()
		{
			m_actionItemList = new ActionItemList();
		}

		internal void Initialize(InitializationContext context)
		{
			ExprHostBuilder exprHostBuilder = context.ExprHostBuilder;
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
			Global.Tracer.Assert(exprHost != null && reportObjectModel != null);
			m_exprHost = exprHost;
			m_exprHost.SetReportObjectModel(reportObjectModel);
			if (exprHost.ActionItemHostsRemotable != null)
			{
				Global.Tracer.Assert(m_actionItemList != null);
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

		internal void SetExprHost(ActionExprHost exprHost, ObjectModelImpl reportObjectModel)
		{
			if (exprHost != null)
			{
				Global.Tracer.Assert(m_actionItemList != null);
				for (int num = m_actionItemList.Count - 1; num >= 0; num--)
				{
					m_actionItemList[num].SetExprHost(exprHost, reportObjectModel);
				}
			}
		}

		internal static Declaration GetDeclaration()
		{
			MemberInfoList memberInfoList = new MemberInfoList();
			memberInfoList.Add(new MemberInfo(MemberName.ActionItemList, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.ActionItemList));
			memberInfoList.Add(new MemberInfo(MemberName.StyleClass, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.Style));
			memberInfoList.Add(new MemberInfo(MemberName.CoumputedActionsCount, Token.Int32));
			return new Declaration(Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.None, memberInfoList);
		}

		internal void ProcessDrillthroughAction(ReportProcessing.ProcessingContext processingContext, int uniqueName)
		{
			if (m_actionItemList != null && m_actionItemList.Count != 0)
			{
				for (int i = 0; i < m_actionItemList.Count; i++)
				{
					m_actionItemList[i].ProcessDrillthroughAction(processingContext, uniqueName, i);
				}
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
					objectModel.FieldsImpl.ResetUsedInExpression();
					objectModel.AggregatesImpl.ResetUsedInExpression();
					return true;
				}
			}
			return false;
		}

		internal void GetSelectedItemsForDrillthroughContext(ObjectModelImpl objectModel, IActionOwner actionOwner)
		{
			if (actionOwner.FieldsUsedInValueExpression == null)
			{
				actionOwner.FieldsUsedInValueExpression = new List<string>();
				objectModel.FieldsImpl.AddFieldsUsedInExpression(actionOwner.FieldsUsedInValueExpression);
				objectModel.AggregatesImpl.AddFieldsUsedInExpression(actionOwner.FieldsUsedInValueExpression);
			}
		}
	}
}
