using Microsoft.ReportingServices.ReportProcessing.ReportObjectModel;
using System;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.ReportProcessing.ExprHostObjectModel
{
	public abstract class ReportExprHost : ReportItemExprHost
	{
		protected CustomCodeProxyBase m_codeProxyBase;

		protected AggregateParamExprHost[] AggregateParamHosts;

		[CLSCompliant(false)]
		protected IList<AggregateParamExprHost> m_aggregateParamHostsRemotable;

		protected ReportParamExprHost[] ReportParameterHosts;

		[CLSCompliant(false)]
		protected IList<ReportParamExprHost> m_reportParameterHostsRemotable;

		protected DataSourceExprHost[] DataSourceHosts;

		[CLSCompliant(false)]
		protected IList<DataSourceExprHost> m_dataSourceHostsRemotable;

		protected DataSetExprHost[] DataSetHosts;

		[CLSCompliant(false)]
		protected IList<DataSetExprHost> m_dataSetHostsRemotable;

		protected StyleExprHost[] PageSectionHosts;

		[CLSCompliant(false)]
		protected IList<StyleExprHost> m_pageSectionHostsRemotable;

		protected ReportItemExprHost[] LineHosts;

		[CLSCompliant(false)]
		protected IList<ReportItemExprHost> m_lineHostsRemotable;

		protected ReportItemExprHost[] RectangleHosts;

		[CLSCompliant(false)]
		protected IList<ReportItemExprHost> m_rectangleHostsRemotable;

		protected TextBoxExprHost[] TextBoxHosts;

		[CLSCompliant(false)]
		protected IList<TextBoxExprHost> m_textBoxHostsRemotable;

		protected ImageExprHost[] ImageHosts;

		[CLSCompliant(false)]
		protected IList<ImageExprHost> m_imageHostsRemotable;

		protected SubreportExprHost[] SubreportHosts;

		[CLSCompliant(false)]
		protected IList<SubreportExprHost> m_subreportHostsRemotable;

		protected ActiveXControlExprHost[] ActiveXControlHosts;

		[CLSCompliant(false)]
		protected IList<ActiveXControlExprHost> m_activeXControlHostsRemotable;

		protected ListExprHost[] ListHosts;

		[CLSCompliant(false)]
		protected IList<ListExprHost> m_listHostsRemotable;

		protected MatrixExprHost[] MatrixHosts;

		[CLSCompliant(false)]
		protected IList<MatrixExprHost> m_matrixHostsRemotable;

		protected ChartExprHost[] ChartHosts;

		[CLSCompliant(false)]
		protected IList<ChartExprHost> m_chartHostsRemotable;

		protected TableExprHost[] TableHosts;

		[CLSCompliant(false)]
		protected IList<TableExprHost> m_tableHostsRemotable;

		protected OWCChartExprHost[] OWCChartHosts;

		[CLSCompliant(false)]
		protected IList<OWCChartExprHost> m_OWCChartHostsRemotable;

		[CLSCompliant(false)]
		protected IList<CustomReportItemExprHost> m_customReportItemHostsRemotable;

		public virtual object ReportLanguageExpr => null;

		internal IList<AggregateParamExprHost> AggregateParamHostsRemotable
		{
			get
			{
				if (m_aggregateParamHostsRemotable == null && AggregateParamHosts != null)
				{
					m_aggregateParamHostsRemotable = new RemoteArrayWrapper<AggregateParamExprHost>(AggregateParamHosts);
				}
				return m_aggregateParamHostsRemotable;
			}
		}

		internal IList<ReportParamExprHost> ReportParameterHostsRemotable
		{
			get
			{
				if (m_reportParameterHostsRemotable == null && ReportParameterHosts != null)
				{
					m_reportParameterHostsRemotable = new RemoteArrayWrapper<ReportParamExprHost>(ReportParameterHosts);
				}
				return m_reportParameterHostsRemotable;
			}
		}

		internal IList<DataSourceExprHost> DataSourceHostsRemotable
		{
			get
			{
				if (m_dataSourceHostsRemotable == null && DataSourceHosts != null)
				{
					m_dataSourceHostsRemotable = new RemoteArrayWrapper<DataSourceExprHost>(DataSourceHosts);
				}
				return m_dataSourceHostsRemotable;
			}
		}

		internal IList<DataSetExprHost> DataSetHostsRemotable
		{
			get
			{
				if (m_dataSetHostsRemotable == null && DataSetHosts != null)
				{
					m_dataSetHostsRemotable = new RemoteArrayWrapper<DataSetExprHost>(DataSetHosts);
				}
				return m_dataSetHostsRemotable;
			}
		}

		internal IList<StyleExprHost> PageSectionHostsRemotable
		{
			get
			{
				if (m_pageSectionHostsRemotable == null && PageSectionHosts != null)
				{
					m_pageSectionHostsRemotable = new RemoteArrayWrapper<StyleExprHost>(PageSectionHosts);
				}
				return m_pageSectionHostsRemotable;
			}
		}

		internal IList<ReportItemExprHost> LineHostsRemotable
		{
			get
			{
				if (m_lineHostsRemotable == null && LineHosts != null)
				{
					m_lineHostsRemotable = new RemoteArrayWrapper<ReportItemExprHost>(LineHosts);
				}
				return m_lineHostsRemotable;
			}
		}

		internal IList<ReportItemExprHost> RectangleHostsRemotable
		{
			get
			{
				if (m_rectangleHostsRemotable == null && RectangleHosts != null)
				{
					m_rectangleHostsRemotable = new RemoteArrayWrapper<ReportItemExprHost>(RectangleHosts);
				}
				return m_rectangleHostsRemotable;
			}
		}

		internal IList<TextBoxExprHost> TextBoxHostsRemotable
		{
			get
			{
				if (m_textBoxHostsRemotable == null && TextBoxHosts != null)
				{
					m_textBoxHostsRemotable = new RemoteArrayWrapper<TextBoxExprHost>(TextBoxHosts);
				}
				return m_textBoxHostsRemotable;
			}
		}

		internal IList<ImageExprHost> ImageHostsRemotable
		{
			get
			{
				if (m_imageHostsRemotable == null && ImageHosts != null)
				{
					m_imageHostsRemotable = new RemoteArrayWrapper<ImageExprHost>(ImageHosts);
				}
				return m_imageHostsRemotable;
			}
		}

		internal IList<SubreportExprHost> SubreportHostsRemotable
		{
			get
			{
				if (m_subreportHostsRemotable == null && SubreportHosts != null)
				{
					m_subreportHostsRemotable = new RemoteArrayWrapper<SubreportExprHost>(SubreportHosts);
				}
				return m_subreportHostsRemotable;
			}
		}

		internal IList<ActiveXControlExprHost> ActiveXControlHostsRemotable
		{
			get
			{
				if (m_activeXControlHostsRemotable == null && ActiveXControlHosts != null)
				{
					m_activeXControlHostsRemotable = new RemoteArrayWrapper<ActiveXControlExprHost>(ActiveXControlHosts);
				}
				return m_activeXControlHostsRemotable;
			}
		}

		internal IList<ListExprHost> ListHostsRemotable
		{
			get
			{
				if (m_listHostsRemotable == null && ListHosts != null)
				{
					m_listHostsRemotable = new RemoteArrayWrapper<ListExprHost>(ListHosts);
				}
				return m_listHostsRemotable;
			}
		}

		internal IList<MatrixExprHost> MatrixHostsRemotable
		{
			get
			{
				if (m_matrixHostsRemotable == null && MatrixHosts != null)
				{
					m_matrixHostsRemotable = new RemoteArrayWrapper<MatrixExprHost>(MatrixHosts);
				}
				return m_matrixHostsRemotable;
			}
		}

		internal IList<ChartExprHost> ChartHostsRemotable
		{
			get
			{
				if (m_chartHostsRemotable == null && ChartHosts != null)
				{
					m_chartHostsRemotable = new RemoteArrayWrapper<ChartExprHost>(ChartHosts);
				}
				return m_chartHostsRemotable;
			}
		}

		internal IList<TableExprHost> TableHostsRemotable
		{
			get
			{
				if (m_tableHostsRemotable == null && TableHosts != null)
				{
					m_tableHostsRemotable = new RemoteArrayWrapper<TableExprHost>(TableHosts);
				}
				return m_tableHostsRemotable;
			}
		}

		internal IList<OWCChartExprHost> OWCChartHostsRemotable
		{
			get
			{
				if (m_OWCChartHostsRemotable == null && OWCChartHosts != null)
				{
					m_OWCChartHostsRemotable = new RemoteArrayWrapper<OWCChartExprHost>(OWCChartHosts);
				}
				return m_OWCChartHostsRemotable;
			}
		}

		internal IList<CustomReportItemExprHost> CustomReportItemHostsRemotable => m_customReportItemHostsRemotable;

		protected ReportExprHost()
		{
		}

		protected ReportExprHost(object reportObjectModel)
		{
			SetReportObjectModel((ObjectModel)reportObjectModel);
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
