using Microsoft.ReportingServices.OnDemandProcessing.Scalability;
using Microsoft.ReportingServices.ReportIntermediateFormat.Persistence;
using Microsoft.ReportingServices.ReportProcessing;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.ReportIntermediateFormat
{
	[Serializable]
	internal sealed class ScopeLookupTable : IStorable, IPersistable
	{
		private int m_lookupInt;

		private Hashtable m_lookupTable;

		[NonSerialized]
		private static readonly Declaration m_Declaration = GetDeclaration();

		internal Hashtable LookupTable
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

		internal int LookupInt
		{
			get
			{
				return m_lookupInt;
			}
			set
			{
				m_lookupInt = value;
			}
		}

		public int Size => 4 + ItemSizes.SizeOf(m_lookupTable);

		internal void Clear()
		{
			m_lookupTable?.Clear();
		}

		internal void Add(GroupingList scopeDefs, List<object>[] scopeValues, int value)
		{
			if (scopeValues == null || scopeValues.Length == 0)
			{
				Global.Tracer.Assert(scopeDefs == null || scopeDefs.Count == 0, "(null == scopeDefs || 0 == scopeDefs.Count)");
				m_lookupInt = value;
				return;
			}
			bool lookup = true;
			if (m_lookupTable == null)
			{
				m_lookupTable = new Hashtable();
				lookup = false;
			}
			Hashtable hashEntries = m_lookupTable;
			int num = 0;
			int num2 = 0;
			for (int i = 0; i < scopeValues.Length; i++)
			{
				List<object> list = scopeValues[i];
				if (list == null)
				{
					num2++;
					continue;
				}
				num = list.Count;
				if (i == scopeValues.Length - 1)
				{
					num--;
				}
				GetNullScopeEntries(num2, ref hashEntries, ref lookup);
				for (int j = 0; j < num; j++)
				{
					Hashtable hashtable = (!lookup) ? null : ((Hashtable)hashEntries[list[j]]);
					if (hashtable == null)
					{
						hashtable = new Hashtable();
						hashEntries.Add(list[j], hashtable);
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

		internal int Lookup(GroupingList scopeDefs, List<object>[] scopeValues)
		{
			object obj = null;
			if (scopeValues == null || scopeValues.Length == 0)
			{
				Global.Tracer.Assert(scopeDefs == null || scopeDefs.Count == 0, "(null == scopeDefs || 0 == scopeDefs.Count)");
				obj = m_lookupInt;
			}
			else
			{
				Hashtable hashtable = m_lookupTable;
				int num = 0;
				for (int i = 0; i < scopeValues.Length; i++)
				{
					List<object> list = scopeValues[i];
					if (list == null)
					{
						num++;
						continue;
					}
					hashtable = (Hashtable)hashtable[num];
					for (int j = 0; j < list.Count; j++)
					{
						obj = hashtable[list[j]];
						if (i < scopeValues.Length - 1 || j < list.Count - 1)
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
			Global.Tracer.Assert(obj is int, "(value is int)");
			return (int)obj;
		}

		internal static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.LookupInt, Token.Int32));
			list.Add(new MemberInfo(MemberName.LookupTable, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.NLevelVariantHashtable, Token.Object));
			return new Declaration(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ScopeLookupTable, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.None, list);
		}

		public void Serialize(IntermediateFormatWriter writer)
		{
			writer.RegisterDeclaration(m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.LookupInt:
					writer.Write(m_lookupInt);
					break;
				case MemberName.LookupTable:
					writer.WriteNLevelVariantHashtable(m_lookupTable);
					break;
				default:
					Global.Tracer.Assert(condition: false);
					break;
				}
			}
		}

		public void Deserialize(IntermediateFormatReader reader)
		{
			reader.RegisterDeclaration(m_Declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.LookupInt:
					m_lookupInt = reader.ReadInt32();
					break;
				case MemberName.LookupTable:
					m_lookupTable = reader.ReadNLevelVariantHashtable();
					break;
				default:
					Global.Tracer.Assert(condition: false);
					break;
				}
			}
		}

		public void ResolveReferences(Dictionary<Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
		}

		public Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ScopeLookupTable;
		}
	}
}
