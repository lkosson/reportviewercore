using Microsoft.ReportingServices.ReportProcessing;
using System.Globalization;

namespace Microsoft.ReportingServices.ReportRendering
{
	internal sealed class ActionInfo
	{
		private MemberBase m_members;

		public ActionCollection Actions
		{
			get
			{
				if (IsCustomControl)
				{
					return Processing.m_actionCollection;
				}
				ActionCollection actionCollection = Rendering.m_actionCollection;
				if (Rendering.m_actionCollection == null)
				{
					ActionItemInstanceList actionItemInstanceList = null;
					if (Rendering.m_actionInfoInstance != null)
					{
						actionItemInstanceList = Rendering.m_actionInfoInstance.ActionItemsValues;
					}
					actionCollection = new ActionCollection(Rendering.m_actionInfoDef.ActionItems, actionItemInstanceList, Rendering.m_ownerUniqueName, Rendering.m_renderingContext);
					if (Rendering.m_renderingContext.CacheState)
					{
						Rendering.m_actionCollection = actionCollection;
					}
				}
				return actionCollection;
			}
			set
			{
				if (!IsCustomControl)
				{
					throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidOperation);
				}
				Processing.m_actionCollection = value;
			}
		}

		public ActionStyle Style
		{
			get
			{
				if (IsCustomControl)
				{
					return Processing.m_style;
				}
				if (Rendering.m_actionInfoDef.StyleClass == null)
				{
					return null;
				}
				ActionStyle actionStyle = Rendering.m_style;
				if (Rendering.m_style == null)
				{
					actionStyle = new ActionStyle(this, Rendering.m_renderingContext);
					if (Rendering.m_renderingContext.CacheState)
					{
						Rendering.m_style = actionStyle;
					}
				}
				return actionStyle;
			}
			set
			{
				if (!IsCustomControl)
				{
					throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidOperation);
				}
				Processing.m_style = value;
			}
		}

		internal Microsoft.ReportingServices.ReportProcessing.Action ActionInfoDef => Rendering.m_actionInfoDef;

		internal ActionInstance ActionInfoInstance => Rendering.m_actionInfoInstance;

		private bool IsCustomControl => m_members.IsCustomControl;

		private ActionInfoRendering Rendering
		{
			get
			{
				Global.Tracer.Assert(!m_members.IsCustomControl);
				ActionInfoRendering actionInfoRendering = m_members as ActionInfoRendering;
				if (actionInfoRendering == null)
				{
					throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidOperation);
				}
				return actionInfoRendering;
			}
		}

		private ActionInfoProcessing Processing
		{
			get
			{
				Global.Tracer.Assert(m_members.IsCustomControl);
				ActionInfoProcessing actionInfoProcessing = m_members as ActionInfoProcessing;
				if (actionInfoProcessing == null)
				{
					throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidOperation);
				}
				return actionInfoProcessing;
			}
		}

		public ActionInfo()
		{
			m_members = new ActionInfoProcessing();
			Global.Tracer.Assert(m_members.IsCustomControl);
		}

		internal ActionInfo(Microsoft.ReportingServices.ReportProcessing.Action actionDef, ActionInstance actionInstance, string ownerUniqueName, RenderingContext renderingContext)
		{
			m_members = new ActionInfoRendering();
			Global.Tracer.Assert(!m_members.IsCustomControl);
			Rendering.m_actionInfoDef = actionDef;
			Rendering.m_actionInfoInstance = actionInstance;
			Rendering.m_renderingContext = renderingContext;
			Rendering.m_ownerUniqueName = ownerUniqueName;
		}

		internal ActionInfo DeepClone()
		{
			if (!IsCustomControl)
			{
				throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidOperation);
			}
			ActionInfo actionInfo = new ActionInfo();
			Global.Tracer.Assert(m_members != null && m_members is ActionInfoProcessing);
			actionInfo.m_members = Processing.DeepClone();
			return actionInfo;
		}

		internal void Deconstruct(int uniqueName, ref Microsoft.ReportingServices.ReportProcessing.Action action, out ActionInstance actionInstance, Microsoft.ReportingServices.ReportProcessing.CustomReportItem context)
		{
			Global.Tracer.Assert(IsCustomControl && context != null);
			actionInstance = null;
			if (Processing.m_actionCollection == null || Processing.m_actionCollection.Count == 0)
			{
				if (action == null)
				{
					return;
				}
				Global.Tracer.Assert(action.ActionItems != null && 0 < action.ActionItems.Count);
				int count = action.ActionItems.Count;
				actionInstance = new ActionInstance();
				actionInstance.UniqueName = uniqueName;
				actionInstance.ActionItemsValues = new ActionItemInstanceList(count);
				for (int i = 0; i < count; i++)
				{
					ActionItemInstance actionItemInstance = new ActionItemInstance();
					if (action.ActionItems[i].DrillthroughParameters != null)
					{
						int count2 = action.ActionItems[i].DrillthroughParameters.Count;
						actionItemInstance.DrillthroughParametersValues = new object[count2];
						actionItemInstance.DrillthroughParametersOmits = new BoolList(count2);
					}
					actionInstance.ActionItemsValues.Add(actionItemInstance);
				}
				return;
			}
			bool flag = action == null;
			int count3 = Processing.m_actionCollection.Count;
			Global.Tracer.Assert(1 <= count3);
			if (flag)
			{
				action = new Microsoft.ReportingServices.ReportProcessing.Action();
				action.ActionItems = new ActionItemList(count3);
				action.ComputedActionItemsCount = count3;
			}
			else if (count3 != action.ComputedActionItemsCount)
			{
				context.ProcessingContext.ErrorContext.Register(ProcessingErrorCode.rsCRIRenderItemProperties, Severity.Error, context.CustomObjectType, context.CustomObjectName, context.Type, context.Name, "Actions", action.ComputedActionItemsCount.ToString(CultureInfo.InvariantCulture), count3.ToString(CultureInfo.InvariantCulture));
				throw new ReportProcessingException(context.ProcessingContext.ErrorContext.Messages);
			}
			actionInstance = new ActionInstance();
			actionInstance.UniqueName = uniqueName;
			actionInstance.ActionItemsValues = new ActionItemInstanceList(count3);
			for (int j = 0; j < count3; j++)
			{
				Action action2 = Processing.m_actionCollection[j];
				ActionItem actionItem = null;
				if (flag)
				{
					actionItem = new ActionItem();
					actionItem.ComputedIndex = j;
					actionItem.Label = new ExpressionInfo(ExpressionInfo.Types.Expression);
					switch (action2.m_actionType)
					{
					case ActionType.HyperLink:
						actionItem.HyperLinkURL = new ExpressionInfo(ExpressionInfo.Types.Expression);
						break;
					case ActionType.DrillThrough:
						actionItem.DrillthroughReportName = new ExpressionInfo(ExpressionInfo.Types.Expression);
						if (action2.m_parameters != null && 0 < action2.m_parameters.Count)
						{
							int count4 = action2.m_parameters.Count;
							actionItem.DrillthroughParameters = new ParameterValueList(count4);
							for (int k = 0; k < count4; k++)
							{
								ParameterValue parameterValue = new ParameterValue();
								parameterValue.Name = action2.m_parameters.GetKey(k);
								parameterValue.Omit = new ExpressionInfo(ExpressionInfo.Types.Constant);
								parameterValue.Omit.BoolValue = false;
								parameterValue.Value = new ExpressionInfo(ExpressionInfo.Types.Expression);
								actionItem.DrillthroughParameters.Add(parameterValue);
							}
						}
						break;
					case ActionType.BookmarkLink:
						actionItem.BookmarkLink = new ExpressionInfo(ExpressionInfo.Types.Expression);
						break;
					}
					action.ActionItems.Add(actionItem);
				}
				else
				{
					actionItem = action.ActionItems[j];
				}
				Global.Tracer.Assert(actionItem != null);
				ActionItemInstance actionItemInstance2 = new ActionItemInstance();
				actionItemInstance2.Label = action2.Processing.m_label;
				switch (action2.m_actionType)
				{
				case ActionType.HyperLink:
					actionItemInstance2.HyperLinkURL = action2.Processing.m_action;
					break;
				case ActionType.DrillThrough:
					actionItemInstance2.DrillthroughReportName = action2.Processing.m_action;
					if (action2.m_parameters != null)
					{
						int count5 = action2.m_parameters.Count;
						if (actionItem.DrillthroughParameters == null && 0 < count5)
						{
							context.ProcessingContext.ErrorContext.Register(ProcessingErrorCode.rsCRIRenderItemProperties, Severity.Error, context.CustomObjectType, context.CustomObjectName, context.Type, context.Name, "Action.DrillthroughParameters", "0", count5.ToString(CultureInfo.InvariantCulture));
							throw new ReportProcessingException(context.ProcessingContext.ErrorContext.Messages);
						}
						if (count5 != actionItem.DrillthroughParameters.Count)
						{
							context.ProcessingContext.ErrorContext.Register(ProcessingErrorCode.rsCRIRenderItemProperties, Severity.Error, context.CustomObjectType, context.CustomObjectName, context.Type, context.Name, "Action.DrillthroughParameters", actionItem.DrillthroughParameters.Count.ToString(CultureInfo.InvariantCulture), count5.ToString(CultureInfo.InvariantCulture));
							throw new ReportProcessingException(context.ProcessingContext.ErrorContext.Messages);
						}
						Global.Tracer.Assert(0 < count5);
						actionItemInstance2.DrillthroughParametersValues = new object[count5];
						actionItemInstance2.DrillthroughParametersOmits = new BoolList(count5);
						DrillthroughParameters drillthroughParameters = new DrillthroughParameters(count5);
						for (int l = 0; l < count5; l++)
						{
							actionItemInstance2.DrillthroughParametersValues[l] = action2.m_parameters.GetValues(l);
							actionItemInstance2.DrillthroughParametersOmits.Add(false);
							drillthroughParameters.Add(actionItem.DrillthroughParameters[l].Name, actionItemInstance2.DrillthroughParametersValues[l]);
						}
						DrillthroughInformation drillthroughInfo = new DrillthroughInformation(actionItemInstance2.DrillthroughReportName, drillthroughParameters, null);
						string drillthroughId = uniqueName.ToString(CultureInfo.InvariantCulture) + ":" + j.ToString(CultureInfo.InvariantCulture);
						context.ProcessingContext.DrillthroughInfo.AddDrillthrough(drillthroughId, drillthroughInfo);
					}
					break;
				case ActionType.BookmarkLink:
					actionItemInstance2.BookmarkLink = action2.Processing.m_action;
					break;
				}
				actionInstance.ActionItemsValues.Add(actionItemInstance2);
			}
			Global.Tracer.Assert(action != null && actionInstance != null && Processing.m_actionCollection != null);
			Microsoft.ReportingServices.ReportProcessing.Style style = action.StyleClass;
			object[] styleAttributeValues = null;
			Microsoft.ReportingServices.ReportProcessing.CustomReportItem.DeconstructRenderStyle(flag, Processing.m_sharedStyles, Processing.m_nonSharedStyles, ref style, out styleAttributeValues, context);
			action.StyleClass = style;
			actionInstance.StyleAttributeValues = styleAttributeValues;
		}
	}
}
