using Microsoft.ReportingServices.ReportIntermediateFormat.Persistence;
using Microsoft.ReportingServices.ReportProcessing;
using System;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.ReportIntermediateFormat
{
	[Serializable]
	internal sealed class SortFilterEventInfo : IPersistable
	{
		[Reference]
		private IInScopeEventSource m_eventSource;

		private List<object>[] m_eventSourceScopeInfo;

		[NonSerialized]
		private static readonly Declaration m_Declaration = GetDeclaration();

		internal IInScopeEventSource EventSource
		{
			get
			{
				return m_eventSource;
			}
			set
			{
				m_eventSource = value;
			}
		}

		internal List<object>[] EventSourceScopeInfo
		{
			get
			{
				return m_eventSourceScopeInfo;
			}
			set
			{
				m_eventSourceScopeInfo = value;
			}
		}

		internal SortFilterEventInfo()
		{
		}

		internal SortFilterEventInfo(IInScopeEventSource eventSource)
		{
			m_eventSource = eventSource;
		}

		internal static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.EventSource, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.IInScopeEventSource, Token.GlobalReference));
			list.Add(new MemberInfo(MemberName.EventSourceScopeInfo, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.PrimitiveArray, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.PrimitiveList));
			return new Declaration(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.SortFilterEventInfo, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.None, list);
		}

		public void Serialize(IntermediateFormatWriter writer)
		{
			writer.RegisterDeclaration(m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.EventSource:
					writer.WriteGlobalReference(m_eventSource);
					break;
				case MemberName.EventSourceScopeInfo:
					writer.WriteArrayOfListsOfPrimitives(m_eventSourceScopeInfo);
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
				case MemberName.EventSource:
					m_eventSource = reader.ReadGlobalReference<IInScopeEventSource>();
					break;
				case MemberName.EventSourceScopeInfo:
					m_eventSourceScopeInfo = reader.ReadArrayOfListsOfPrimitives<object>();
					break;
				default:
					Global.Tracer.Assert(condition: false);
					break;
				}
			}
		}

		public void ResolveReferences(Dictionary<Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
			Global.Tracer.Assert(condition: false);
		}

		public Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.SortFilterEventInfo;
		}
	}
}
