using Microsoft.ReportingServices.Common;
using Microsoft.ReportingServices.ReportProcessing;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class InternalShimDynamicMemberLogic : InternalDynamicMemberLogic
	{
		private readonly IShimDataRegionMember m_shimMember;

		public InternalShimDynamicMemberLogic(IShimDataRegionMember shimMember)
		{
			m_shimMember = shimMember;
		}

		public override void ResetContext()
		{
			m_isNewContext = true;
			m_currentContext = -1;
			m_shimMember.ResetContext();
		}

		public override bool MoveNext()
		{
			if (!m_shimMember.SetNewContext(m_currentContext + 1))
			{
				return false;
			}
			m_currentContext++;
			return true;
		}

		public override bool SetInstanceIndex(int index)
		{
			if (index < 0)
			{
				ResetContext();
				return true;
			}
			if (m_shimMember.SetNewContext(index))
			{
				m_currentContext = index;
				return true;
			}
			return false;
		}

		internal override ScopeID GetScopeID()
		{
			throw new RenderingObjectModelException(ProcessingErrorCode.rsNotSupportedInStreamingMode, "GetScopeID");
		}

		internal override ScopeID GetLastScopeID()
		{
			throw new RenderingObjectModelException(ProcessingErrorCode.rsNotSupportedInStreamingMode, "GetLastScopeID");
		}

		internal override void SetScopeID(ScopeID scopeID)
		{
			throw new RenderingObjectModelException(ProcessingErrorCode.rsNotSupportedInStreamingMode, "SetScopeID");
		}
	}
}
