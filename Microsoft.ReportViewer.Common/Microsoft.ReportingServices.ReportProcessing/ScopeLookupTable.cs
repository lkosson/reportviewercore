using Microsoft.ReportingServices.ReportProcessing.Persistence;
using System;
using System.Collections;

namespace Microsoft.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class ScopeLookupTable
	{
		private object m_lookupTable;

		internal object LookupTable
		{
			get
			{
				return m_lookupTable;
			}
			set
			{
				m_lookupTable = value;
			}
		}

		internal void Clear()
		{
			(m_lookupTable as Hashtable)?.Clear();
		}

		internal void Add(GroupingList scopeDefs, VariantList[] scopeValues, int value)
		{
			if (scopeValues == null || scopeValues.Length == 0)
			{
				Global.Tracer.Assert(scopeDefs == null || scopeDefs.Count == 0, "(null == scopeDefs || 0 == scopeDefs.Count)");
				m_lookupTable = value;
				return;
			}
			bool lookup = true;
			if (m_lookupTable == null)
			{
				m_lookupTable = new Hashtable();
				lookup = false;
			}
			Hashtable hashEntries = (Hashtable)m_lookupTable;
			int num = 0;
			int num2 = 0;
			for (int i = 0; i < scopeValues.Length; i++)
			{
				VariantList variantList = scopeValues[i];
				if (variantList == null)
				{
					num2++;
					continue;
				}
				num = variantList.Count;
				if (i == scopeValues.Length - 1)
				{
					num--;
				}
				GetNullScopeEntries(num2, ref hashEntries, ref lookup);
				for (int j = 0; j < num; j++)
				{
					Hashtable hashtable = (!lookup) ? null : ((Hashtable)hashEntries[variantList[j]]);
					if (hashtable == null)
					{
						hashtable = new Hashtable();
						hashEntries.Add(variantList[j], hashtable);
						lookup = false;
					}
					hashEntries = hashtable;
				}
				num2 = 0;
			}
			object key = 1;
			if (scopeValues[scopeValues.Length - 1] != null)
			{
				key = scopeValues[scopeValues.Length - 1][num];
			}
			else
			{
				GetNullScopeEntries(num2, ref hashEntries, ref lookup);
			}
			Global.Tracer.Assert(!hashEntries.Contains(key), "(!hashEntries.Contains(lastKey))");
			hashEntries.Add(key, value);
		}

		private void GetNullScopeEntries(int nullScopes, ref Hashtable hashEntries, ref bool lookup)
		{
			Hashtable hashtable = null;
			if (lookup)
			{
				hashtable = (Hashtable)hashEntries[nullScopes];
			}
			if (hashtable == null)
			{
				hashtable = new Hashtable();
				hashEntries.Add(nullScopes, hashtable);
				lookup = false;
			}
			hashEntries = hashtable;
		}

		internal int Lookup(GroupingList scopeDefs, VariantList[] scopeValues)
		{
			object obj = null;
			if (scopeValues == null || scopeValues.Length == 0)
			{
				Global.Tracer.Assert(scopeDefs == null || scopeDefs.Count == 0, "(null == scopeDefs || 0 == scopeDefs.Count)");
				obj = m_lookupTable;
			}
			else
			{
				Hashtable hashtable = (Hashtable)m_lookupTable;
				int num = 0;
				for (int i = 0; i < scopeValues.Length; i++)
				{
					VariantList variantList = scopeValues[i];
					if (variantList == null)
					{
						num++;
						continue;
					}
					hashtable = (Hashtable)hashtable[num];
					for (int j = 0; j < variantList.Count; j++)
					{
						obj = hashtable[variantList[j]];
						if (i < scopeValues.Length - 1 || j < variantList.Count - 1)
						{
							hashtable = (Hashtable)obj;
							Global.Tracer.Assert(hashtable != null, "(null != hashEntries)");
						}
					}
					num = 0;
				}
				if (scopeValues[scopeValues.Length - 1] == null)
				{
					hashtable = (Hashtable)hashtable[num];
					obj = hashtable[1];
				}
			}
			Global.Tracer.Assert(obj is int);
			return (int)obj;
		}

		internal static Declaration GetDeclaration()
		{
			MemberInfoList memberInfoList = new MemberInfoList();
			memberInfoList.Add(new MemberInfo(MemberName.LookupTable, Token.Object));
			return new Declaration(Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.None, memberInfoList);
		}
	}
}
