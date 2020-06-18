using Microsoft.ReportingServices.ReportIntermediateFormat;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class ReportInstance : BaseInstance, IReportScopeInstance
	{
		private Report m_reportDef;

		private Microsoft.ReportingServices.ReportIntermediateFormat.ReportInstance m_reportInstance;

		private string m_language;

		private bool m_isNewContext = true;

		private string m_initialPageName;

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

		public string UniqueName
		{
			get
			{
				if (m_reportDef.IsOldSnapshot)
				{
					return m_reportDef.RenderReport.UniqueName;
				}
				return InstancePathItem.GenerateInstancePathString(m_reportDef.ReportDef.InstancePath) + "xA";
			}
		}

		public string Language
		{
			get
			{
				if (m_language == null)
				{
					if (m_reportDef.IsOldSnapshot)
					{
						return m_reportDef.RenderReport.ReportLanguage;
					}
					m_language = m_reportInstance.Language;
				}
				return m_language;
			}
		}

		public int AutoRefresh
		{
			get
			{
				if (m_reportDef.IsOldSnapshot)
				{
					return m_reportDef.RenderReport.AutoRefresh;
				}
				return m_reportDef.ReportDef.EvaluateAutoRefresh(this, m_reportDef.RenderingContext.OdpContext);
			}
		}

		public string InitialPageName
		{
			get
			{
				if (m_reportDef.IsOldSnapshot)
				{
					return null;
				}
				ExpressionInfo initialPageName = m_reportDef.ReportDef.InitialPageName;
				if (initialPageName != null)
				{
					if (!initialPageName.IsExpression)
					{
						m_initialPageName = initialPageName.StringValue;
					}
					else
					{
						m_initialPageName = m_reportDef.ReportDef.EvaluateInitialPageName(this, m_reportDef.RenderingContext.OdpContext);
					}
				}
				return m_initialPageName;
			}
		}

		internal Report ReportDef => m_reportDef;

		internal ReportInstance(Report reportDef, Microsoft.ReportingServices.ReportIntermediateFormat.ReportInstance reportInstance)
			: base(null)
		{
			m_reportDef = reportDef;
			m_reportInstance = reportInstance;
		}

		internal ReportInstance(Report reportDef)
			: base(null)
		{
			m_reportDef = reportDef;
			m_reportInstance = null;
		}

		protected override void ResetInstanceCache()
		{
		}

		public void ResetContext()
		{
			m_reportDef.SetNewContext();
		}

		internal override void SetNewContext()
		{
			base.SetNewContext();
			m_isNewContext = true;
		}
	}
}
