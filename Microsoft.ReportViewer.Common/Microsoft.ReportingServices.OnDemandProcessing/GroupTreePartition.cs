using Microsoft.ReportingServices.OnDemandProcessing.Scalability;
using Microsoft.ReportingServices.ReportIntermediateFormat;
using Microsoft.ReportingServices.ReportIntermediateFormat.Persistence;
using Microsoft.ReportingServices.ReportProcessing;
using System;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.OnDemandProcessing
{
	internal class GroupTreePartition : IPersistable
	{
		private List<IReference<ScopeInstance>> m_topLevelScopeInstances;

		[NonSerialized]
		private static readonly Declaration m_Declaration = GetDeclaration();

		internal bool IsEmpty => m_topLevelScopeInstances == null;

		internal List<IReference<ScopeInstance>> TopLevelScopeInstances => m_topLevelScopeInstances;

		internal GroupTreePartition()
		{
		}

		internal void AddTopLevelScopeInstance(IReference<ScopeInstance> instance)
		{
			if (m_topLevelScopeInstances == null)
			{
				m_topLevelScopeInstances = new List<IReference<ScopeInstance>>();
			}
			m_topLevelScopeInstances.Add(instance);
		}

		internal static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.TopLevelScopeInstances, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ScopeInstanceReference));
			return new Declaration(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.GroupTreePartition, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.None, list);
		}

		public virtual void Serialize(IntermediateFormatWriter writer)
		{
			writer.RegisterDeclaration(m_Declaration);
			while (writer.NextMember())
			{
				MemberName memberName = writer.CurrentMember.MemberName;
				if (memberName == MemberName.TopLevelScopeInstances)
				{
					writer.Write(m_topLevelScopeInstances);
				}
				else
				{
					Global.Tracer.Assert(condition: false);
				}
			}
		}

		public virtual void Deserialize(IntermediateFormatReader reader)
		{
			reader.RegisterDeclaration(m_Declaration);
			while (reader.NextMember())
			{
				MemberName memberName = reader.CurrentMember.MemberName;
				if (memberName == MemberName.TopLevelScopeInstances)
				{
					m_topLevelScopeInstances = reader.ReadGenericListOfRIFObjects<IReference<ScopeInstance>>();
				}
				else
				{
					Global.Tracer.Assert(condition: false);
				}
			}
			reader.ResolveReferences();
		}

		public virtual void ResolveReferences(Dictionary<Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
			Global.Tracer.Assert(condition: false);
		}

		public virtual Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.GroupTreePartition;
		}
	}
}
