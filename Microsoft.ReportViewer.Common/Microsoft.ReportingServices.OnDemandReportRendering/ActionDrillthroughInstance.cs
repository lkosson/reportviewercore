using Microsoft.ReportingServices.Diagnostics.Utilities;
using Microsoft.ReportingServices.ReportProcessing;
using Microsoft.ReportingServices.ReportRendering;
using System.Collections.Specialized;
using System.Globalization;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class ActionDrillthroughInstance : BaseInstance
	{
		private bool m_isOldSnapshot;

		private Microsoft.ReportingServices.ReportRendering.Action m_renderAction;

		private string m_reportName;

		private string m_drillthroughID;

		private string m_drillthroughUrl;

		private bool m_drillthroughUrlEvaluated;

		private ActionDrillthrough m_actionDef;

		private int m_index = -1;

		public string ReportName
		{
			get
			{
				if (m_reportName == null)
				{
					if (m_isOldSnapshot)
					{
						if (m_renderAction != null)
						{
							m_reportName = m_renderAction.DrillthroughPath;
						}
					}
					else
					{
						Global.Tracer.Assert(m_actionDef.ReportName != null, "(m_actionDef.ReportName != null)");
						if (!m_actionDef.ReportName.IsExpression)
						{
							m_reportName = m_actionDef.ReportName.Value;
						}
						else if (m_actionDef.Owner.ReportElementOwner == null || m_actionDef.Owner.ReportElementOwner.CriOwner == null)
						{
							ActionInfo owner = m_actionDef.Owner;
							m_reportName = m_actionDef.ActionItemDef.EvaluateDrillthroughReportName(ReportScopeInstance, owner.RenderingContext.OdpContext, owner.InstancePath, owner.ObjectType, owner.ObjectName);
						}
					}
				}
				return m_reportName;
			}
			set
			{
				ReportElement reportElementOwner = m_actionDef.Owner.ReportElementOwner;
				Global.Tracer.Assert(m_actionDef.ReportName != null, "(m_actionDef.ReportName != null)");
				if (!m_actionDef.Owner.IsChartConstruction && (reportElementOwner.CriGenerationPhase == ReportElement.CriGenerationPhases.None || (reportElementOwner.CriGenerationPhase == ReportElement.CriGenerationPhases.Instance && !m_actionDef.ReportName.IsExpression)))
				{
					throw new RenderingObjectModelException(RPRes.rsErrorDuringROMWriteback);
				}
				m_reportName = value;
				m_drillthroughUrl = null;
			}
		}

		public string DrillthroughID
		{
			get
			{
				if (m_drillthroughID == null)
				{
					if (m_isOldSnapshot)
					{
						if (m_renderAction != null)
						{
							m_drillthroughID = m_renderAction.DrillthroughID;
						}
					}
					else if (ReportName != null)
					{
						m_drillthroughID = ((m_actionDef.Owner.ROMActionOwner != null) ? m_actionDef.Owner.ROMActionOwner.UniqueName : m_actionDef.Owner.InstancePath.UniqueName);
						m_drillthroughID = m_drillthroughID + ":" + m_index.ToString(CultureInfo.InvariantCulture);
					}
				}
				return m_drillthroughID;
			}
		}

		public string DrillthroughUrl
		{
			get
			{
				if (!m_drillthroughUrlEvaluated)
				{
					m_drillthroughUrlEvaluated = true;
					if (ReportName != null)
					{
						if (m_isOldSnapshot)
						{
							if (m_renderAction != null)
							{
								try
								{
									ReportUrlBuilder urlBuilder = m_renderAction.DrillthroughReport.GetUrlBuilder(null, useReplacementRoot: true);
									urlBuilder.AddParameters(m_renderAction.DrillthroughParameters, UrlParameterType.ReportParameter);
									m_drillthroughUrl = urlBuilder.ToUri().AbsoluteUri;
								}
								catch (ItemNotFoundException)
								{
									m_drillthroughUrl = null;
								}
							}
						}
						else
						{
							try
							{
								NameValueCollection parameters = null;
								if (m_actionDef.Parameters != null)
								{
									parameters = m_actionDef.Parameters.ToNameValueCollection;
								}
								m_drillthroughUrl = ReportUrl.BuildDrillthroughUrl(m_actionDef.PathResolutionContext, ReportName, parameters);
							}
							catch (ItemNotFoundException)
							{
								m_drillthroughUrl = null;
							}
						}
					}
				}
				return m_drillthroughUrl;
			}
		}

		internal ActionDrillthroughInstance(IReportScope reportScope, ActionDrillthrough actionDef, int index)
			: base(reportScope)
		{
			m_isOldSnapshot = false;
			m_actionDef = actionDef;
			m_index = index;
		}

		internal ActionDrillthroughInstance(Microsoft.ReportingServices.ReportRendering.Action renderAction)
			: base(null)
		{
			m_isOldSnapshot = true;
			m_renderAction = renderAction;
		}

		internal void Update(Microsoft.ReportingServices.ReportRendering.Action newAction)
		{
			m_renderAction = newAction;
			ResetInstanceCache();
		}

		protected override void ResetInstanceCache()
		{
			m_reportName = null;
			m_drillthroughID = null;
			m_drillthroughUrl = null;
			m_drillthroughUrlEvaluated = false;
		}
	}
}
