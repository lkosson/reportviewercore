using Microsoft.ReportingServices.ReportIntermediateFormat;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class RectangleInstance : ReportItemInstance
	{
		private bool m_pageNameEvaluated;

		private string m_pageName;

		public override VisibilityInstance Visibility
		{
			get
			{
				if (((Rectangle)m_reportElementDef).IsListContentsRectangle)
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
						Microsoft.ReportingServices.ReportIntermediateFormat.Rectangle rectangle = (Microsoft.ReportingServices.ReportIntermediateFormat.Rectangle)m_reportElementDef.ReportItemDef;
						ExpressionInfo pageName = rectangle.PageName;
						if (pageName != null)
						{
							if (pageName.IsExpression)
							{
								m_pageName = rectangle.EvaluatePageName(ReportScopeInstance, m_reportElementDef.RenderingContext.OdpContext);
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

		internal RectangleInstance(Rectangle reportItemDef)
			: base(reportItemDef)
		{
		}

		protected override void ResetInstanceCache()
		{
			base.ResetInstanceCache();
			m_pageNameEvaluated = false;
			m_pageName = null;
		}
	}
}
