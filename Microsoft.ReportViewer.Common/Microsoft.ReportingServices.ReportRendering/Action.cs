using Microsoft.ReportingServices.Diagnostics;
using Microsoft.ReportingServices.Diagnostics.Utilities;
using Microsoft.ReportingServices.ReportProcessing;
using System.Collections.Specialized;

namespace Microsoft.ReportingServices.ReportRendering
{
	internal sealed class Action
	{
		internal ActionType m_actionType;

		internal MemberBase m_members;

		internal NameValueCollection m_parameters;

		internal DrillthroughParameters m_parameterNameObjectCollection;

		public ReportUrl HyperLinkURL
		{
			get
			{
				if (m_actionType == ActionType.DrillThrough || m_actionType == ActionType.BookmarkLink)
				{
					return null;
				}
				if (IsCustomControl)
				{
					return null;
				}
				ReportUrl reportUrl = Rendering.m_actionURL;
				if (Rendering.m_actionURL == null && Rendering.m_actionDef.HyperLinkURL != null)
				{
					string hyperLinkUrlValue = null;
					m_actionType = ActionType.HyperLink;
					if (Rendering.m_actionDef.HyperLinkURL.Type == ExpressionInfo.Types.Constant)
					{
						hyperLinkUrlValue = Rendering.m_actionDef.HyperLinkURL.Value;
					}
					else if (Rendering.m_actionInstance == null)
					{
						reportUrl = null;
					}
					else
					{
						hyperLinkUrlValue = Rendering.m_actionInstance.HyperLinkURL;
					}
					reportUrl = ReportUrl.BuildHyperLinkURL(hyperLinkUrlValue, Rendering.m_renderingContext);
					if (Rendering.m_renderingContext.CacheState)
					{
						Rendering.m_actionURL = reportUrl;
					}
				}
				return reportUrl;
			}
		}

		public ReportUrl DrillthroughReport
		{
			get
			{
				if (m_actionType == ActionType.HyperLink || m_actionType == ActionType.BookmarkLink)
				{
					return null;
				}
				if (IsCustomControl)
				{
					return null;
				}
				ReportUrl reportUrl = Rendering.m_actionURL;
				if (Rendering.m_actionURL == null && Rendering.m_actionDef.DrillthroughReportName != null)
				{
					string drillthroughPath = DrillthroughPath;
					m_actionType = ActionType.DrillThrough;
					if (drillthroughPath != null)
					{
						try
						{
							reportUrl = new ReportUrl(Rendering.m_renderingContext, drillthroughPath, checkProtocol: true, null, useReplacementRoot: true);
						}
						catch (ItemNotFoundException)
						{
							return null;
						}
					}
					if (Rendering.m_renderingContext.CacheState)
					{
						Rendering.m_actionURL = reportUrl;
					}
				}
				return reportUrl;
			}
		}

		internal DrillthroughParameters DrillthroughParameterNameObjectCollection
		{
			get
			{
				if (m_actionType == ActionType.HyperLink || m_actionType == ActionType.BookmarkLink)
				{
					return null;
				}
				DrillthroughParameters drillthroughParameters = m_parameterNameObjectCollection;
				if (!IsCustomControl && m_parameters == null)
				{
					ParameterValueList drillthroughParameters2 = Rendering.m_actionDef.DrillthroughParameters;
					if (drillthroughParameters2 != null && drillthroughParameters2.Count > 0)
					{
						m_actionType = ActionType.DrillThrough;
						drillthroughParameters = new DrillthroughParameters();
						for (int i = 0; i < drillthroughParameters2.Count; i++)
						{
							ParameterValue parameterValue = drillthroughParameters2[i];
							if (parameterValue.Omit == null || (Rendering.m_actionInstance == null && parameterValue.Omit.Type == ExpressionInfo.Types.Constant && !parameterValue.Omit.BoolValue) || !Rendering.m_actionInstance.DrillthroughParametersOmits[i])
							{
								drillthroughParameters.Add(value: (parameterValue.Value.Type == ExpressionInfo.Types.Constant) ? parameterValue.Value.Value : ((Rendering.m_actionInstance != null) ? Rendering.m_actionInstance.DrillthroughParametersValues[i] : null), key: parameterValue.Name);
							}
						}
						m_parameterNameObjectCollection = drillthroughParameters;
					}
				}
				return drillthroughParameters;
			}
		}

		public NameValueCollection DrillthroughParameters
		{
			get
			{
				if (m_actionType == ActionType.HyperLink || m_actionType == ActionType.BookmarkLink)
				{
					return null;
				}
				NameValueCollection nameValueCollection = m_parameters;
				if (!IsCustomControl && m_parameters == null)
				{
					ParameterValueList drillthroughParameters = Rendering.m_actionDef.DrillthroughParameters;
					if (drillthroughParameters != null && drillthroughParameters.Count > 0)
					{
						m_actionType = ActionType.DrillThrough;
						nameValueCollection = new NameValueCollection();
						bool[] array = new bool[drillthroughParameters.Count];
						for (int i = 0; i < drillthroughParameters.Count; i++)
						{
							ParameterValue parameterValue = drillthroughParameters[i];
							if (parameterValue.Value != null && parameterValue.Value.Type == ExpressionInfo.Types.Token)
							{
								array[i] = true;
							}
							else
							{
								array[i] = false;
							}
							if (parameterValue.Omit != null && (Rendering.m_actionInstance != null || parameterValue.Omit.Type != ExpressionInfo.Types.Constant || parameterValue.Omit.BoolValue) && Rendering.m_actionInstance.DrillthroughParametersOmits[i])
							{
								continue;
							}
							object obj = (parameterValue.Value.Type == ExpressionInfo.Types.Constant) ? parameterValue.Value.Value : ((Rendering.m_actionInstance != null) ? Rendering.m_actionInstance.DrillthroughParametersValues[i] : null);
							if (obj == null)
							{
								nameValueCollection.Add(parameterValue.Name, null);
								continue;
							}
							object[] array2 = obj as object[];
							if (array2 != null)
							{
								for (int j = 0; j < array2.Length; j++)
								{
									nameValueCollection.Add(parameterValue.Name, array2[j].ToString());
								}
							}
							else
							{
								nameValueCollection.Add(parameterValue.Name, obj.ToString());
							}
						}
						bool replaced = false;
						if (Rendering.m_renderingContext.StoreServerParameters != null && DrillthroughPath != null)
						{
							string drillthroughPath = DrillthroughPath;
							ICatalogItemContext subreportContext = Rendering.m_renderingContext.TopLevelReportContext.GetSubreportContext(drillthroughPath);
							nameValueCollection = Rendering.m_renderingContext.StoreServerParameters(subreportContext, nameValueCollection, array, out replaced);
						}
						if (Rendering.m_renderingContext.CacheState)
						{
							m_parameters = nameValueCollection;
						}
					}
				}
				return nameValueCollection;
			}
			set
			{
				if (!IsCustomControl)
				{
					throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidOperation);
				}
				m_parameters = value;
			}
		}

		public string DrillthroughID
		{
			get
			{
				if (IsCustomControl)
				{
					return null;
				}
				if (m_actionType == ActionType.HyperLink || m_actionType == ActionType.BookmarkLink)
				{
					return null;
				}
				if (DrillthroughReport != null)
				{
					m_actionType = ActionType.DrillThrough;
					return Rendering.m_drillthroughId;
				}
				return null;
			}
		}

		public string BookmarkLink
		{
			get
			{
				if (m_actionType == ActionType.HyperLink || m_actionType == ActionType.DrillThrough)
				{
					return null;
				}
				string result = null;
				if (IsCustomControl)
				{
					result = Processing.m_action;
				}
				else if (Rendering.m_actionDef.BookmarkLink != null)
				{
					m_actionType = ActionType.BookmarkLink;
					result = ((Rendering.m_actionDef.BookmarkLink.Type == ExpressionInfo.Types.Constant) ? Rendering.m_actionDef.BookmarkLink.Value : ((Rendering.m_actionInstance != null) ? Rendering.m_actionInstance.BookmarkLink : null));
				}
				return result;
			}
			set
			{
				if (!IsCustomControl)
				{
					throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidOperation);
				}
				m_actionType = ActionType.BookmarkLink;
				Processing.m_action = value;
			}
		}

		public string Label
		{
			get
			{
				string result = null;
				if (IsCustomControl)
				{
					result = Processing.m_label;
				}
				else if (Rendering.m_actionDef.Label != null)
				{
					result = ((Rendering.m_actionDef.Label.Type == ExpressionInfo.Types.Constant) ? Rendering.m_actionDef.Label.Value : ((Rendering.m_actionInstance != null) ? Rendering.m_actionInstance.Label : null));
				}
				return result;
			}
			set
			{
				if (!IsCustomControl)
				{
					throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidOperation);
				}
				Processing.m_label = value;
			}
		}

		internal ActionItem ActionDefinition
		{
			get
			{
				if (IsCustomControl)
				{
					throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidOperation);
				}
				return Rendering.m_actionDef;
			}
		}

		internal ActionItemInstance ActionInstance
		{
			get
			{
				if (IsCustomControl)
				{
					throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidOperation);
				}
				return Rendering.m_actionInstance;
			}
		}

		internal ParameterValueList DrillthroughParameterValueList
		{
			get
			{
				if (m_actionType == ActionType.HyperLink || m_actionType == ActionType.BookmarkLink)
				{
					return null;
				}
				return Rendering.m_actionDef.DrillthroughParameters;
			}
		}

		internal string DrillthroughPath
		{
			get
			{
				string text = null;
				if (Rendering.m_actionDef.DrillthroughReportName.Type == ExpressionInfo.Types.Constant)
				{
					return Rendering.m_actionDef.DrillthroughReportName.Value;
				}
				if (Rendering.m_actionInstance == null)
				{
					return null;
				}
				return Rendering.m_actionInstance.DrillthroughReportName;
			}
		}

		private bool IsCustomControl => m_members.IsCustomControl;

		private ActionRendering Rendering
		{
			get
			{
				Global.Tracer.Assert(!m_members.IsCustomControl);
				ActionRendering actionRendering = m_members as ActionRendering;
				if (actionRendering == null)
				{
					throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidOperation);
				}
				return actionRendering;
			}
		}

		internal ActionProcessing Processing
		{
			get
			{
				Global.Tracer.Assert(m_members.IsCustomControl);
				ActionProcessing actionProcessing = m_members as ActionProcessing;
				if (actionProcessing == null)
				{
					throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidOperation);
				}
				return actionProcessing;
			}
		}

		public Action()
		{
			m_members = new ActionProcessing();
			Global.Tracer.Assert(IsCustomControl);
		}

		internal Action(ActionItem actionItemDef, ActionItemInstance actionItemInstance, string drillthroughId, RenderingContext renderingContext)
		{
			m_members = new ActionRendering();
			Global.Tracer.Assert(!IsCustomControl);
			Rendering.m_actionDef = actionItemDef;
			Rendering.m_actionInstance = actionItemInstance;
			Rendering.m_renderingContext = renderingContext;
			Rendering.m_drillthroughId = drillthroughId;
		}

		public void SetHyperlinkAction(string hyperlink)
		{
			SetHyperlinkAction(hyperlink, null);
		}

		public void SetHyperlinkAction(string hyperlink, string label)
		{
			if (!IsCustomControl)
			{
				throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidOperation);
			}
			m_actionType = ActionType.HyperLink;
			Processing.m_action = hyperlink;
			Processing.m_label = label;
		}

		public void SetDrillthroughAction(string reportName)
		{
			SetDrillthroughAction(reportName, null, null);
		}

		public void SetDrillthroughAction(string reportName, NameValueCollection parameters)
		{
			SetDrillthroughAction(reportName, parameters, null);
		}

		public void SetDrillthroughAction(string reportName, NameValueCollection parameters, string label)
		{
			if (!IsCustomControl)
			{
				throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidOperation);
			}
			if (parameters != null && reportName == null)
			{
				throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidParameterValue, "reportName");
			}
			m_actionType = ActionType.DrillThrough;
			Processing.m_action = reportName;
			Processing.m_label = label;
			m_parameters = parameters;
		}

		public void SetBookmarkAction(string bookmark)
		{
			SetBookmarkAction(bookmark, null);
		}

		public void SetBookmarkAction(string bookmark, string label)
		{
			m_actionType = ActionType.BookmarkLink;
			Processing.m_action = bookmark;
			Processing.m_label = label;
		}

		internal Action DeepClone()
		{
			if (!IsCustomControl)
			{
				throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidOperation);
			}
			Global.Tracer.Assert(m_members != null && m_members is ActionProcessing);
			Action action = new Action();
			action.m_actionType = m_actionType;
			action.m_members = Processing.DeepClone();
			if (ActionType.DrillThrough == m_actionType && m_parameters != null)
			{
				action.m_parameters = new NameValueCollection(m_parameters);
			}
			return action;
		}
	}
}
