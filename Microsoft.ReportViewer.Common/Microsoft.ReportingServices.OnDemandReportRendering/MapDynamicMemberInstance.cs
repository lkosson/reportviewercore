using Microsoft.ReportingServices.Common;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class MapDynamicMemberInstance : MapMemberInstance, IDynamicInstance, IReportScopeInstance
	{
		private readonly InternalDynamicMemberLogic m_memberLogic;

		string IReportScopeInstance.UniqueName => m_memberDef.UniqueName;

		bool IReportScopeInstance.IsNewContext
		{
			get
			{
				return m_memberLogic.IsNewContext;
			}
			set
			{
				m_memberLogic.IsNewContext = value;
			}
		}

		IReportScope IReportScopeInstance.ReportScope => m_reportScope;

		internal MapDynamicMemberInstance(MapDataRegion owner, MapMember memberDef, InternalDynamicMemberLogic memberLogic)
			: base(owner, memberDef)
		{
			m_memberLogic = memberLogic;
			ResetContext();
		}

		public bool MoveNext()
		{
			return m_memberLogic.MoveNext();
		}

		public bool SetInstanceIndex(int index)
		{
			return m_memberLogic.SetInstanceIndex(index);
		}

		ScopeID IDynamicInstance.GetScopeID()
		{
			return GetScopeID();
		}

		void IDynamicInstance.SetScopeID(ScopeID scopeID)
		{
			SetScopeID(scopeID);
		}

		public void ResetContext()
		{
			m_memberLogic.ResetContext();
		}

		void IDynamicInstance.ResetContext()
		{
			ResetContext();
		}

		public int GetInstanceIndex()
		{
			return m_memberLogic.GetInstanceIndex();
		}

		internal ScopeID GetScopeID()
		{
			return m_memberLogic.GetScopeID();
		}

		internal void SetScopeID(ScopeID scopeID)
		{
			m_memberLogic.SetScopeID(scopeID);
		}
	}
}
