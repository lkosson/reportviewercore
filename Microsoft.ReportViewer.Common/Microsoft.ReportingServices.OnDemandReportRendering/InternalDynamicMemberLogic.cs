using Microsoft.ReportingServices.Common;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal abstract class InternalDynamicMemberLogic
	{
		protected int m_currentContext = -1;

		protected bool m_isNewContext = true;

		public bool IsNewContext
		{
			get
			{
				return m_isNewContext;
			}
			set
			{
				m_isNewContext = value;
			}
		}

		public abstract bool MoveNext();

		public int GetInstanceIndex()
		{
			return m_currentContext;
		}

		public abstract bool SetInstanceIndex(int index);

		internal abstract ScopeID GetScopeID();

		internal abstract ScopeID GetLastScopeID();

		internal abstract void SetScopeID(ScopeID scopeID);

		public abstract void ResetContext();
	}
}
