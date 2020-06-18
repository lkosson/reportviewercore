using Microsoft.ReportingServices.ReportProcessing.ExprHostObjectModel;
using Microsoft.ReportingServices.ReportProcessing.Persistence;
using Microsoft.ReportingServices.ReportProcessing.ReportObjectModel;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Microsoft.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class ActionItem
	{
		private ExpressionInfo m_hyperLinkURL;

		private ExpressionInfo m_drillthroughReportName;

		private ParameterValueList m_drillthroughParameters;

		private ExpressionInfo m_drillthroughBookmarkLink;

		private ExpressionInfo m_bookmarkLink;

		private ExpressionInfo m_label;

		private int m_exprHostID = -1;

		private int m_computedIndex = -1;

		[NonSerialized]
		private ActionExprHost m_exprHost;

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

		internal ParameterValueList DrillthroughParameters
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
					parameterValue.Initialize(context, queryParam: false);
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
			Global.Tracer.Assert(actionItemExprHosts != null && reportObjectModel != null);
			m_exprHost = actionItemExprHosts[m_exprHostID];
			m_exprHost.SetReportObjectModel(reportObjectModel);
			if (m_exprHost.DrillThroughParameterHostsRemotable != null)
			{
				Global.Tracer.Assert(m_drillthroughParameters != null);
				for (int num = m_drillthroughParameters.Count - 1; num >= 0; num--)
				{
					m_drillthroughParameters[num].SetExprHost(m_exprHost.DrillThroughParameterHostsRemotable, reportObjectModel);
				}
			}
		}

		internal void SetExprHost(ActionExprHost actionExprHost, ObjectModelImpl reportObjectModel)
		{
			Global.Tracer.Assert(actionExprHost != null && reportObjectModel != null);
			m_exprHost = actionExprHost;
			m_exprHost.SetReportObjectModel(reportObjectModel);
			if (m_exprHost.DrillThroughParameterHostsRemotable != null)
			{
				Global.Tracer.Assert(m_drillthroughParameters != null);
				for (int num = m_drillthroughParameters.Count - 1; num >= 0; num--)
				{
					m_drillthroughParameters[num].SetExprHost(m_exprHost.DrillThroughParameterHostsRemotable, reportObjectModel);
				}
			}
		}

		internal static Declaration GetDeclaration()
		{
			MemberInfoList memberInfoList = new MemberInfoList();
			memberInfoList.Add(new MemberInfo(MemberName.HyperLinkURL, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.ExpressionInfo));
			memberInfoList.Add(new MemberInfo(MemberName.DrillthroughReportName, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.ExpressionInfo));
			memberInfoList.Add(new MemberInfo(MemberName.DrillthroughParameters, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.ParameterValueList));
			memberInfoList.Add(new MemberInfo(MemberName.DrillthroughBookmarkLink, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.ExpressionInfo));
			memberInfoList.Add(new MemberInfo(MemberName.BookmarkLink, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.ExpressionInfo));
			memberInfoList.Add(new MemberInfo(MemberName.Label, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.ExpressionInfo));
			memberInfoList.Add(new MemberInfo(MemberName.ExprHostID, Token.Int32));
			memberInfoList.Add(new MemberInfo(MemberName.Index, Token.Int32));
			return new Declaration(Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.None, memberInfoList);
		}

		internal void ProcessDrillthroughAction(ReportProcessing.ProcessingContext processingContext, int ownerUniqueName, int index)
		{
			if (m_drillthroughReportName == null)
			{
				return;
			}
			Global.Tracer.Assert(m_drillthroughReportName.Type == ExpressionInfo.Types.Constant);
			if (m_drillthroughReportName.Value == null)
			{
				return;
			}
			DrillthroughParameters drillthroughParameters = null;
			if (m_drillthroughParameters != null)
			{
				ParameterValue parameterValue = null;
				for (int i = 0; i < m_drillthroughParameters.Count; i++)
				{
					parameterValue = m_drillthroughParameters[i];
					if (parameterValue.Omit != null)
					{
						Global.Tracer.Assert(parameterValue.Omit.Type == ExpressionInfo.Types.Constant);
						if (parameterValue.Omit.BoolValue)
						{
							continue;
						}
					}
					Global.Tracer.Assert(parameterValue.Value.Type == ExpressionInfo.Types.Constant);
					if (drillthroughParameters == null)
					{
						drillthroughParameters = new DrillthroughParameters();
					}
					drillthroughParameters.Add(parameterValue.Name, parameterValue.Value.Value);
				}
			}
			DrillthroughInformation drillthroughInfo = new DrillthroughInformation(m_drillthroughReportName.Value, drillthroughParameters, null);
			string drillthroughId = ownerUniqueName.ToString(CultureInfo.InvariantCulture) + ":" + index.ToString(CultureInfo.InvariantCulture);
			processingContext.DrillthroughInfo.AddDrillthrough(drillthroughId, drillthroughInfo);
		}
	}
}
