using Microsoft.ReportingServices.ReportIntermediateFormat;
using Microsoft.ReportingServices.ReportProcessing;
using Microsoft.ReportingServices.ReportRendering;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal abstract class DataRegionInstance : ReportItemInstance, IReportScopeInstance
	{
		private string m_noRowsMessage;

		private bool m_isNewContext = true;

		private bool? m_noRows;

		private string m_pageName;

		private bool m_pageNameEvaluated;

		private readonly DataRegion m_dataRegionDef;

		string IReportScopeInstance.UniqueName => base.UniqueName;

		bool IReportScopeInstance.IsNewContext
		{
			get
			{
				return m_isNewContext;
			}
			set
			{
				m_isNewContext = value;
			}
		}

		IReportScope IReportScopeInstance.ReportScope => m_reportScope;

		public string NoRowsMessage
		{
			get
			{
				if (m_noRowsMessage == null)
				{
					if (m_reportElementDef.IsOldSnapshot)
					{
						m_noRowsMessage = ((Microsoft.ReportingServices.ReportRendering.DataRegion)m_reportElementDef.RenderReportItem).NoRowMessage;
					}
					else
					{
						Microsoft.ReportingServices.ReportIntermediateFormat.DataRegion dataRegion = (Microsoft.ReportingServices.ReportIntermediateFormat.DataRegion)m_reportElementDef.ReportItemDef;
						if (dataRegion.NoRowsMessage != null)
						{
							if (!dataRegion.NoRowsMessage.IsExpression)
							{
								m_noRowsMessage = dataRegion.NoRowsMessage.StringValue;
							}
							else
							{
								m_noRowsMessage = dataRegion.EvaluateNoRowsMessage(this, m_reportElementDef.RenderingContext.OdpContext);
							}
						}
					}
				}
				return m_noRowsMessage;
			}
		}

		public bool NoRows
		{
			get
			{
				if (!m_noRows.HasValue)
				{
					if (m_reportElementDef.IsOldSnapshot)
					{
						m_noRows = ((Microsoft.ReportingServices.ReportRendering.DataRegion)m_reportElementDef.RenderReportItem).NoRows;
					}
					else
					{
						m_reportElementDef.RenderingContext.OdpContext.SetupContext(m_reportElementDef.ReportItemDef, this);
						m_noRows = ((Microsoft.ReportingServices.ReportIntermediateFormat.DataRegion)m_reportElementDef.ReportItemDef).NoRows;
					}
				}
				return m_noRows.Value;
			}
		}

		public override VisibilityInstance Visibility
		{
			get
			{
				if (m_reportElementDef.IsOldSnapshot && ((DataRegion)m_reportElementDef).DataRegionType == DataRegion.Type.List)
				{
					return null;
				}
				return base.Visibility;
			}
		}

		public string PageName
		{
			get
			{
				if (!m_pageNameEvaluated)
				{
					if (m_reportElementDef.IsOldSnapshot)
					{
						m_pageName = null;
					}
					else
					{
						m_pageNameEvaluated = true;
						Microsoft.ReportingServices.ReportIntermediateFormat.DataRegion dataRegion = (Microsoft.ReportingServices.ReportIntermediateFormat.DataRegion)m_reportElementDef.ReportItemDef;
						Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo pageName = dataRegion.PageName;
						if (pageName != null)
						{
							if (pageName.IsExpression)
							{
								m_pageName = dataRegion.EvaluatePageName(ReportScopeInstance, m_reportElementDef.RenderingContext.OdpContext);
							}
							else
							{
								m_pageName = pageName.StringValue;
							}
						}
					}
				}
				return m_pageName;
			}
		}

		internal DataRegionInstance(DataRegion reportItemDef)
			: base(reportItemDef)
		{
			m_dataRegionDef = reportItemDef;
		}

		internal override void SetNewContext()
		{
			m_isNewContext = true;
			m_noRowsMessage = null;
			m_noRows = null;
			m_pageNameEvaluated = false;
			m_pageName = null;
			base.SetNewContext();
		}

		internal void DoneSettingScopeID()
		{
			if (m_reportElementDef.RenderingContext.OdpContext.QueryRestartInfo.QueryRestartPosition.Count > 0)
			{
				m_reportElementDef.RenderingContext.OdpContext.QueryRestartInfo.EnableQueryRestart();
				m_reportElementDef.RenderingContext.OdpContext.SetupContext(m_dataRegionDef.ReportItemDef, (IReportScopeInstance)m_dataRegionDef.Instance);
				m_reportElementDef.RenderingContext.OdpContext.QueryRestartInfo.RomBasedRestart();
				return;
			}
			throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidScopeIDNotSet, m_dataRegionDef.Name);
		}
	}
}
