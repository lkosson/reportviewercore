using Microsoft.ReportingServices.ReportRendering;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class ShimMatrixMemberVisibility : ShimMemberVisibility
	{
		private ShimMatrixMember m_owner;

		public override ReportBoolProperty Hidden
		{
			get
			{
				if (m_startHidden == null)
				{
					m_startHidden = Visibility.GetStartHidden(m_owner.Group.CurrentShimRenderGroup.m_visibilityDef);
				}
				return m_startHidden;
			}
		}

		public override string ToggleItem
		{
			get
			{
				if (m_owner.Group.CurrentShimRenderGroup.m_visibilityDef != null)
				{
					return m_owner.Group.CurrentShimRenderGroup.m_visibilityDef.Toggle;
				}
				return null;
			}
		}

		public override bool RecursiveToggleReceiver
		{
			get
			{
				if (m_owner.Group.CurrentShimRenderGroup.m_visibilityDef != null)
				{
					return m_owner.Group.CurrentShimRenderGroup.m_visibilityDef.RecursiveReceiver;
				}
				return false;
			}
		}

		public override SharedHiddenState HiddenState => Visibility.GetHiddenState(m_owner.Group.CurrentShimRenderGroup.m_visibilityDef);

		public ShimMatrixMemberVisibility(ShimMatrixMember owner)
		{
			m_owner = owner;
		}

		internal override bool GetInstanceHidden()
		{
			return m_owner.Group.CurrentShimRenderGroup.Hidden;
		}

		internal override bool GetInstanceStartHidden()
		{
			if (m_owner.Group != null)
			{
				if (((MatrixMember)m_owner.Group.CurrentShimRenderGroup).InstanceInfo != null)
				{
					return ((MatrixMember)m_owner.Group.CurrentShimRenderGroup).InstanceInfo.StartHidden;
				}
				return GetInstanceHidden();
			}
			return false;
		}
	}
}
