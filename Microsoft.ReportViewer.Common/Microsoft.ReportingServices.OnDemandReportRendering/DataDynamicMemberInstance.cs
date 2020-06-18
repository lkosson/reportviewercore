using Microsoft.ReportingServices.Common;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class DataDynamicMemberInstance : DataMemberInstance, IDynamicInstance, IReportScopeInstance
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

		internal DataDynamicMemberInstance(CustomReportItem owner, DataMember memberDef, InternalDynamicMemberLogic memberLogic)
			: base(owner, memberDef)
		{
			m_memberLogic = memberLogic;
			ResetContext();
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

		public void ResetContext()
		{
			m_memberLogic.ResetContext();
		}

		public bool MoveNext()
		{
			return m_memberLogic.MoveNext();
		}

		public int GetInstanceIndex()
		{
			return m_memberLogic.GetInstanceIndex();
		}

		public bool SetInstanceIndex(int index)
		{
			return m_memberLogic.SetInstanceIndex(index);
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
