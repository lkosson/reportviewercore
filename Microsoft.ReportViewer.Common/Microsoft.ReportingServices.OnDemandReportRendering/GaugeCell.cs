using Microsoft.ReportingServices.ReportIntermediateFormat;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class GaugeCell : IReportScope, IDataRegionCell
	{
		private GaugePanel m_owner;

		private Microsoft.ReportingServices.ReportIntermediateFormat.GaugeCell m_gaugeCellDef;

		private GaugeCellInstance m_instance;

		private List<GaugeInputValue> m_gaugeInputValues;

		internal Microsoft.ReportingServices.ReportIntermediateFormat.GaugeCell GaugeCellDef => m_gaugeCellDef;

		internal GaugePanel GaugePanelDef => m_owner;

		public GaugeCellInstance Instance
		{
			get
			{
				if (m_owner.RenderingContext.InstanceAccessDisallowed)
				{
					return null;
				}
				if (m_instance == null)
				{
					m_instance = new GaugeCellInstance(this);
				}
				return m_instance;
			}
		}

		IReportScopeInstance IReportScope.ReportScopeInstance => Instance;

		IRIFReportScope IReportScope.RIFReportScope => GaugeCellDef;

		internal GaugeCell(GaugePanel owner, Microsoft.ReportingServices.ReportIntermediateFormat.GaugeCell gaugeCellDef)
		{
			m_owner = owner;
			m_gaugeCellDef = gaugeCellDef;
		}

		void IDataRegionCell.SetNewContext()
		{
			SetNewContext();
		}

		internal void SetNewContext()
		{
			if (m_instance != null)
			{
				m_instance.SetNewContext();
			}
			List<GaugeInputValue> gaugeInputValues = GetGaugeInputValues();
			if (gaugeInputValues != null)
			{
				foreach (GaugeInputValue item in gaugeInputValues)
				{
					item.SetNewContext();
				}
			}
			if (m_gaugeCellDef != null)
			{
				m_gaugeCellDef.ClearStreamingScopeInstanceBinding();
			}
		}

		private List<GaugeInputValue> GetGaugeInputValues()
		{
			if (m_gaugeInputValues == null)
			{
				m_gaugeInputValues = m_owner.GetGaugeInputValues();
			}
			return m_gaugeInputValues;
		}
	}
}
