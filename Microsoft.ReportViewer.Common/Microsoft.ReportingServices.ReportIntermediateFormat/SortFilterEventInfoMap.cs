using Microsoft.ReportingServices.ReportIntermediateFormat.Persistence;
using Microsoft.ReportingServices.ReportProcessing;
using System;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.ReportIntermediateFormat
{
	[Serializable]
	internal sealed class SortFilterEventInfoMap : IPersistable
	{
		private Dictionary<string, SortFilterEventInfo> m_sortFilterEventInfos;

		[NonSerialized]
		private static readonly Declaration m_Declaration = GetDeclaration();

		internal SortFilterEventInfo this[string eventSourceUniqueName]
		{
			get
			{
				m_sortFilterEventInfos.TryGetValue(eventSourceUniqueName, out SortFilterEventInfo value);
				return value;
			}
		}

		internal int Count
		{
			get
			{
				if (m_sortFilterEventInfos == null)
				{
					return 0;
				}
				return m_sortFilterEventInfos.Count;
			}
		}

		internal SortFilterEventInfoMap()
		{
		}

		internal void Add(string eventSourceUniqueName, SortFilterEventInfo eventInfo)
		{
			if (m_sortFilterEventInfos == null)
			{
				m_sortFilterEventInfos = new Dictionary<string, SortFilterEventInfo>();
			}
			m_sortFilterEventInfos.Add(eventSourceUniqueName, eventInfo);
		}

		internal void Merge(SortFilterEventInfoMap partition)
		{
			if (partition.Count == 0)
			{
				return;
			}
			foreach (KeyValuePair<string, SortFilterEventInfo> sortFilterEventInfo in partition.m_sortFilterEventInfos)
			{
				Add(sortFilterEventInfo.Key, sortFilterEventInfo.Value);
			}
		}

		internal static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.SortFilterEventInfos, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.StringRIFObjectDictionary, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.SortFilterEventInfo));
			return new Declaration(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.SortFilterEventInfoMap, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.None, list);
		}

		public void Serialize(IntermediateFormatWriter writer)
		{
			writer.RegisterDeclaration(m_Declaration);
			while (writer.NextMember())
			{
				MemberName memberName = writer.CurrentMember.MemberName;
				if (memberName == MemberName.SortFilterEventInfos)
				{
					writer.WriteStringRIFObjectDictionary(m_sortFilterEventInfos);
				}
				else
				{
					Global.Tracer.Assert(condition: false);
				}
			}
		}

		public void Deserialize(IntermediateFormatReader reader)
		{
			reader.RegisterDeclaration(m_Declaration);
			while (reader.NextMember())
			{
				MemberName memberName = reader.CurrentMember.MemberName;
				if (memberName == MemberName.SortFilterEventInfos)
				{
					m_sortFilterEventInfos = reader.ReadStringRIFObjectDictionary<SortFilterEventInfo>();
				}
				else
				{
					Global.Tracer.Assert(condition: false);
				}
			}
		}

		public void ResolveReferences(Dictionary<Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
			Global.Tracer.Assert(condition: false);
		}

		public Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.SortFilterEventInfoMap;
		}
	}
}
