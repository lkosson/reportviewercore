using Microsoft.ReportingServices.Common;
using Microsoft.ReportingServices.OnDemandProcessing;
using Microsoft.ReportingServices.ReportProcessing;
using System;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class InternalStreamingOdpDynamicMemberLogic : InternalStreamingOdpDynamicMemberLogicBase
	{
		public InternalStreamingOdpDynamicMemberLogic(DataRegionMember memberDef, OnDemandProcessingContext odpContext)
			: base(memberDef, odpContext)
		{
		}

		public override bool MoveNext()
		{
			ResetScopeID();
			return MoveNextCore(null);
		}

		internal bool RomBasedRestart(ScopeID targetScopeID)
		{
			if (targetScopeID == null)
			{
				return false;
			}
			try
			{
				IEqualityComparer<object> processingComparer = m_odpContext.ProcessingComparer;
				bool result = true;
				while (!targetScopeID.Equals(GetScopeID(), processingComparer) && (result = MoveNext()))
				{
				}
				return result;
			}
			catch (Exception)
			{
				throw new RenderingObjectModelException(ProcessingErrorCode.rsRombasedRestartFailedTypeMismatch, m_memberDef.Group.Name);
			}
		}

		internal override void SetScopeID(ScopeID scopeID)
		{
			if (m_grouping.IsDetail)
			{
				throw new RenderingObjectModelException(ProcessingErrorCode.rsDetailGroupsNotSupportedInStreamingMode, "SetScopeID");
			}
			if (scopeID == null)
			{
				throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidScopeID, "SetScopeID");
			}
			if (!m_odpContext.QueryRestartInfo.TryAddScopeID(scopeID, m_memberDef.DataRegionMemberDefinition, this))
			{
				throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidScopeIDOrder, m_grouping.Name, "SetScopeID");
			}
		}
	}
}
