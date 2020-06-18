using Microsoft.ReportingServices.OnDemandProcessing.Scalability;
using Microsoft.ReportingServices.ReportIntermediateFormat.Persistence;
using Microsoft.ReportingServices.ReportProcessing;
using System;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.ReportIntermediateFormat
{
	internal class LookupTable : IStorable, IPersistable
	{
		private ScalableDictionary<object, LookupMatches> m_table;

		[NonSerialized]
		private static readonly Declaration m_Declaration = GetDeclaration();

		public int Size => m_table.Size;

		internal LookupTable()
		{
		}

		internal LookupTable(IScalabilityCache scalabilityCache, IEqualityComparer<object> comparer, bool mustStoreDataRows)
		{
			m_table = new ScalableDictionary<object, LookupMatches>(0, scalabilityCache, 100, 10, comparer);
		}

		internal bool TryGetValue(object key, out LookupMatches matches)
		{
			return m_table.TryGetValue(key, out matches);
		}

		internal bool TryGetAndPinValue(object key, out LookupMatches matches, out IDisposable cleanupRef)
		{
			return m_table.TryGetAndPin(key, out matches, out cleanupRef);
		}

		internal IDisposable AddAndPin(object key, LookupMatches matches)
		{
			return m_table.AddAndPin(key, matches);
		}

		internal void TransferTo(IScalabilityCache scaleCache)
		{
			m_table.TransferTo(scaleCache);
		}

		internal void SetEqualityComparer(IEqualityComparer<object> comparer)
		{
			m_table.UpdateComparer(comparer);
		}

		public void Serialize(IntermediateFormatWriter writer)
		{
			writer.RegisterDeclaration(m_Declaration);
			while (writer.NextMember())
			{
				MemberName memberName = writer.CurrentMember.MemberName;
				if (memberName == MemberName.LookupTable)
				{
					writer.Write(m_table);
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
				if (memberName == MemberName.LookupTable)
				{
					m_table = reader.ReadRIFObject<ScalableDictionary<object, LookupMatches>>();
				}
				else
				{
					Global.Tracer.Assert(condition: false);
				}
			}
		}

		public void ResolveReferences(Dictionary<Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
		}

		public Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.LookupTable;
		}

		public static Declaration GetDeclaration()
		{
			if (m_Declaration == null)
			{
				List<MemberInfo> list = new List<MemberInfo>();
				list.Add(new MemberInfo(MemberName.LookupTable, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ScalableDictionary, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.LookupMatches));
				return new Declaration(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.LookupTable, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.None, list);
			}
			return m_Declaration;
		}
	}
}
