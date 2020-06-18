using Microsoft.ReportingServices.OnDemandProcessing.Scalability;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.OnDemandProcessing.TablixProcessing
{
	internal sealed class DomainScopeContext
	{
		internal class DomainScopeInfo
		{
			private int m_currentKeyIndex = -1;

			private int m_keyCount;

			private object[] m_keys;

			internal DataFieldRow m_currentRow;

			internal object CurrentKey => m_keys[m_currentKeyIndex];

			internal DataFieldRow CurrentRow
			{
				get
				{
					return m_currentRow;
				}
				set
				{
					m_currentRow = value;
				}
			}

			internal void InitializeKeys(int count)
			{
				m_keys = new object[count];
				m_keyCount = 0;
				m_currentKeyIndex = -1;
			}

			internal void AddKey(object key)
			{
				m_keys[m_keyCount++] = key;
			}

			internal void RemoveKey()
			{
				m_keyCount--;
			}

			internal void MoveNext()
			{
				m_currentKeyIndex++;
			}

			internal void MovePrevious()
			{
				m_currentKeyIndex--;
			}
		}

		private Dictionary<int, IReference<RuntimeGroupRootObj>> m_domainScopes = new Dictionary<int, IReference<RuntimeGroupRootObj>>();

		private DomainScopeInfo m_currentDomainScopeInfo;

		internal DomainScopeInfo CurrentDomainScope
		{
			get
			{
				return m_currentDomainScopeInfo;
			}
			set
			{
				m_currentDomainScopeInfo = value;
			}
		}

		internal Dictionary<int, IReference<RuntimeGroupRootObj>> DomainScopes => m_domainScopes;

		internal void AddDomainScopes(IReference<RuntimeMemberObj>[] membersDef, int startIndex)
		{
			for (int i = startIndex; i < membersDef.Length; i++)
			{
				IReference<RuntimeMemberObj> reference = membersDef[i];
				using (reference.PinValue())
				{
					IReference<RuntimeGroupRootObj> groupRoot = ((RuntimeDataTablixMemberObj)reference.Value()).GroupRoot;
					using (groupRoot.PinValue())
					{
						m_domainScopes.Add(groupRoot.Value().HierarchyDef.OriginalScopeID, groupRoot);
					}
				}
			}
		}

		internal void RemoveDomainScopes(IReference<RuntimeMemberObj>[] membersDef, int startIndex)
		{
			for (int i = startIndex; i < membersDef.Length; i++)
			{
				IReference<RuntimeMemberObj> reference = membersDef[i];
				using (reference.PinValue())
				{
					IReference<RuntimeGroupRootObj> groupRoot = ((RuntimeDataTablixMemberObj)reference.Value()).GroupRoot;
					using (groupRoot.PinValue())
					{
						m_domainScopes.Remove(groupRoot.Value().HierarchyDef.OriginalScopeID);
					}
				}
			}
		}
	}
}
