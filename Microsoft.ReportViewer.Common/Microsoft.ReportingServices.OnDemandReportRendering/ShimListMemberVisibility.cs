using Microsoft.ReportingServices.ReportRendering;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class ShimListMemberVisibility : ShimMemberVisibility
	{
		private ShimListMember m_owner;

		public override ReportBoolProperty Hidden
		{
			get
			{
				if (m_startHidden == null && m_owner.Group != null)
				{
					m_startHidden = Visibility.GetStartHidden(m_owner.OwnerTablix.RenderList.ReportItemDef.Visibility);
				}
				return m_startHidden;
			}
		}

		public override string ToggleItem
		{
			get
			{
				if (m_owner.Group != null && m_owner.OwnerTablix.RenderList.ReportItemDef.Visibility != null)
				{
					return m_owner.OwnerTablix.RenderList.ReportItemDef.Visibility.Toggle;
				}
				return null;
			}
		}

		public override bool RecursiveToggleReceiver
		{
			get
			{
				if (m_owner.Group != null && m_owner.OwnerTablix.RenderList.ReportItemDef.Visibility != null)
				{
					return m_owner.OwnerTablix.RenderList.ReportItemDef.Visibility.RecursiveReceiver;
				}
				return false;
			}
		}

		public override SharedHiddenState HiddenState
		{
			get
			{
				if (m_owner.Group != null)
				{
					return Visibility.GetHiddenState(m_owner.OwnerTablix.RenderList.ReportItemDef.Visibility);
				}
				return SharedHiddenState.Never;
			}
		}

		public ShimListMemberVisibility(ShimListMember owner)
		{
			m_owner = owner;
		}

		internal override bool GetInstanceHidden()
		{
			if (m_owner.Group != null)
			{
				return m_owner.Group.CurrentShimRenderGroup.Hidden;
			}
			return false;
		}

		internal override bool GetInstanceStartHidden()
		{
			if (m_owner.Group != null)
			{
				if (((ListContent)m_owner.Group.CurrentShimRenderGroup).InstanceInfo != null)
				{
					return ((ListContent)m_owner.Group.CurrentShimRenderGroup).InstanceInfo.StartHidden;
				}
				return GetInstanceHidden();
			}
			return false;
		}
	}
}
