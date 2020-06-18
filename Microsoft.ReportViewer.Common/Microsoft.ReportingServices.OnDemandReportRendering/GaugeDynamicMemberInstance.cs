using Microsoft.ReportingServices.Common;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal class GaugeDynamicMemberInstance : GaugeMemberInstance, IDynamicInstance, IReportScopeInstance
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

		internal GaugeDynamicMemberInstance(GaugePanel owner, GaugeMember memberDef, InternalDynamicMemberLogic memberLogic)
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

		public void ResetContext()
		{
			m_memberLogic.ResetContext();
		}

		void IDynamicInstance.ResetContext()
		{
			ResetContext();
		}

		bool IDynamicInstance.MoveNext()
		{
			return MoveNext();
		}

		int IDynamicInstance.GetInstanceIndex()
		{
			return GetInstanceIndex();
		}

		bool IDynamicInstance.SetInstanceIndex(int index)
		{
			return SetInstanceIndex(index);
		}

		ScopeID IDynamicInstance.GetScopeID()
		{
			return GetScopeID();
		}

		void IDynamicInstance.SetScopeID(ScopeID scopeID)
		{
			SetScopeID(scopeID);
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
