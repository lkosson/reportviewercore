using Microsoft.ReportingServices.Common;
using Microsoft.ReportingServices.OnDemandProcessing;
using Microsoft.ReportingServices.ReportProcessing;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class InternalFullOdpDynamicMemberLogic : InternalDynamicMemberLogic
	{
		private readonly DataRegionMember m_memberDef;

		private readonly OnDemandProcessingContext m_odpContext;

		public InternalFullOdpDynamicMemberLogic(DataRegionMember memberDef, OnDemandProcessingContext odpContext)
		{
			m_memberDef = memberDef;
			m_odpContext = odpContext;
		}

		public override void ResetContext()
		{
			m_isNewContext = true;
			m_currentContext = -1;
			m_memberDef.DataRegionMemberDefinition.InstanceCount = -1;
			m_memberDef.DataRegionMemberDefinition.InstancePathItem.ResetContext();
		}

		public override bool MoveNext()
		{
			if (!IsContextValid(m_currentContext + 1))
			{
				return false;
			}
			m_isNewContext = true;
			m_currentContext++;
			m_memberDef.DataRegionMemberDefinition.InstancePathItem.MoveNext();
			m_memberDef.SetNewContext(fromMoveNext: true);
			return true;
		}

		public override bool SetInstanceIndex(int index)
		{
			if (index < 0)
			{
				ResetContext();
				return true;
			}
			if (IsContextValid(index))
			{
				m_currentContext = index;
				m_memberDef.DataRegionMemberDefinition.InstancePathItem.SetContext(m_currentContext);
				m_memberDef.SetNewContext(fromMoveNext: true);
				m_isNewContext = true;
				return true;
			}
			return false;
		}

		private bool IsContextValid(int context)
		{
			if (m_memberDef.DataRegionMemberDefinition.InstanceCount < 0)
			{
				m_odpContext.SetupContext(m_memberDef.DataRegionMemberDefinition, m_memberDef.ReportScopeInstance, context);
			}
			return context < m_memberDef.DataRegionMemberDefinition.InstanceCount;
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
