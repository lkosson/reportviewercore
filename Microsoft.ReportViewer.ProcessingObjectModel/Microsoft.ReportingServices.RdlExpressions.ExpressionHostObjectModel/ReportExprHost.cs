using Microsoft.ReportingServices.ReportProcessing.OnDemandReportObjectModel;
using System;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.RdlExpressions.ExpressionHostObjectModel
{
	public abstract class ReportExprHost : ReportItemExprHost
	{
		protected CustomCodeProxyBase m_codeProxyBase;

		public IndexedExprHost VariableValueHosts;

		[CLSCompliant(false)]
		protected IList<AggregateParamExprHost> m_aggregateParamHostsRemotable;

		[CLSCompliant(false)]
		protected IList<LookupExprHost> m_lookupExprHostsRemotable;

		[CLSCompliant(false)]
		protected IList<LookupDestExprHost> m_lookupDestExprHostsRemotable;

		[CLSCompliant(false)]
		protected IList<ReportParamExprHost> m_reportParameterHostsRemotable;

		[CLSCompliant(false)]
		protected IList<DataSourceExprHost> m_dataSourceHostsRemotable;

		[CLSCompliant(false)]
		protected IList<DataSetExprHost> m_dataSetHostsRemotable;

		[CLSCompliant(false)]
		protected IList<StyleExprHost> m_pageSectionHostsRemotable;

		protected StyleExprHost m_pageHost;

		[CLSCompliant(false)]
		protected IList<StyleExprHost> m_pageHostsRemotable;

		[CLSCompliant(false)]
		protected IList<ReportSectionExprHost> m_reportSectionHostsRemotable;

		[CLSCompliant(false)]
		protected IList<ReportItemExprHost> m_lineHostsRemotable;

		[CLSCompliant(false)]
		protected IList<ReportItemExprHost> m_rectangleHostsRemotable;

		[CLSCompliant(false)]
		protected IList<TextBoxExprHost> m_textBoxHostsRemotable;

		[CLSCompliant(false)]
		protected IList<ImageExprHost> m_imageHostsRemotable;

		[CLSCompliant(false)]
		protected IList<SubreportExprHost> m_subreportHostsRemotable;

		[CLSCompliant(false)]
		protected IList<TablixExprHost> m_tablixHostsRemotable;

		[CLSCompliant(false)]
		protected IList<ChartExprHost> m_chartHostsRemotable;

		[CLSCompliant(false)]
		protected IList<GaugePanelExprHost> m_gaugePanelHostsRemotable;

		[CLSCompliant(false)]
		protected IList<MapExprHost> m_mapHostsRemotable;

		[CLSCompliant(false)]
		protected IList<MapDataRegionExprHost> m_mapDataRegionHostsRemotable;

		[CLSCompliant(false)]
		protected IList<CustomReportItemExprHost> m_customReportItemHostsRemotable;

		public virtual object ReportLanguageExpr => null;

		public virtual object AutoRefreshExpr => null;

		public virtual object InitialPageNameExpr => null;

		internal IList<AggregateParamExprHost> AggregateParamHostsRemotable => m_aggregateParamHostsRemotable;

		[CLSCompliant(false)]
		public IList<LookupExprHost> LookupExprHostsRemotable => m_lookupExprHostsRemotable;

		[CLSCompliant(false)]
		public IList<LookupDestExprHost> LookupDestExprHostsRemotable => m_lookupDestExprHostsRemotable;

		internal IList<ReportParamExprHost> ReportParameterHostsRemotable => m_reportParameterHostsRemotable;

		internal IList<DataSourceExprHost> DataSourceHostsRemotable => m_dataSourceHostsRemotable;

		internal IList<DataSetExprHost> DataSetHostsRemotable => m_dataSetHostsRemotable;

		internal IList<StyleExprHost> PageSectionHostsRemotable => m_pageSectionHostsRemotable;

		internal virtual StyleExprHost PageHost => m_pageHost;

		[CLSCompliant(false)]
		public IList<StyleExprHost> PageHostsRemotable => m_pageHostsRemotable;

		[CLSCompliant(false)]
		public IList<ReportSectionExprHost> ReportSectionHostsRemotable => m_reportSectionHostsRemotable;

		internal IList<ReportItemExprHost> LineHostsRemotable => m_lineHostsRemotable;

		internal IList<ReportItemExprHost> RectangleHostsRemotable => m_rectangleHostsRemotable;

		internal IList<TextBoxExprHost> TextBoxHostsRemotable => m_textBoxHostsRemotable;

		internal IList<ImageExprHost> ImageHostsRemotable => m_imageHostsRemotable;

		internal IList<SubreportExprHost> SubreportHostsRemotable => m_subreportHostsRemotable;

		internal IList<TablixExprHost> TablixHostsRemotable => m_tablixHostsRemotable;

		internal IList<ChartExprHost> ChartHostsRemotable => m_chartHostsRemotable;

		internal IList<GaugePanelExprHost> GaugePanelHostsRemotable => m_gaugePanelHostsRemotable;

		internal IList<MapExprHost> MapHostsRemotable => m_mapHostsRemotable;

		internal IList<MapDataRegionExprHost> MapDataRegionHostsRemotable => m_mapDataRegionHostsRemotable;

		internal IList<CustomReportItemExprHost> CustomReportItemHostsRemotable => m_customReportItemHostsRemotable;

		protected ReportExprHost(object reportObjectModel)
		{
			SetReportObjectModel((OnDemandObjectModel)reportObjectModel);
		}

		internal void CustomCodeOnInit()
		{
			if (m_codeProxyBase != null)
			{
				m_codeProxyBase.CallOnInit();
			}
		}
	}
}
