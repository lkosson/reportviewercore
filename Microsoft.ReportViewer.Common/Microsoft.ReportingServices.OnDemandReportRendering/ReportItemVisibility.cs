namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class ReportItemVisibility : Visibility
	{
		private ReportItem m_owner;

		public override ReportBoolProperty Hidden
		{
			get
			{
				if (m_startHidden == null)
				{
					if (m_owner.IsOldSnapshot)
					{
						m_startHidden = Visibility.GetStartHidden(m_owner.RenderReportItem.ReportItemDef.Visibility);
					}
					else
					{
						m_startHidden = Visibility.GetStartHidden(m_owner.ReportItemDef.Visibility);
					}
				}
				return m_startHidden;
			}
		}

		public override string ToggleItem
		{
			get
			{
				if (m_owner.IsOldSnapshot)
				{
					if (m_owner.RenderReportItem.ReportItemDef.Visibility != null)
					{
						return m_owner.RenderReportItem.ReportItemDef.Visibility.Toggle;
					}
				}
				else if (m_owner.ReportItemDef.Visibility != null)
				{
					return m_owner.ReportItemDef.Visibility.Toggle;
				}
				return null;
			}
		}

		public override SharedHiddenState HiddenState
		{
			get
			{
				if (m_owner.IsOldSnapshot)
				{
					return Visibility.GetHiddenState(m_owner.RenderReportItem.ReportItemDef.Visibility);
				}
				return Visibility.GetHiddenState(m_owner.ReportItemDef.Visibility);
			}
		}

		public override bool RecursiveToggleReceiver
		{
			get
			{
				if (m_owner.IsOldSnapshot)
				{
					if (m_owner.RenderReportItem.ReportItemDef.Visibility != null)
					{
						return m_owner.RenderReportItem.ReportItemDef.Visibility.RecursiveReceiver;
					}
				}
				else if (m_owner.ReportItemDef.Visibility != null)
				{
					return m_owner.ReportItemDef.Visibility.RecursiveReceiver;
				}
				return false;
			}
		}

		public ReportItemVisibility(ReportItem owner)
		{
			m_owner = owner;
		}
	}
}
