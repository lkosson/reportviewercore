using Microsoft.ReportingServices.ReportIntermediateFormat.Persistence;
using Microsoft.ReportingServices.ReportProcessing;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.ReportIntermediateFormat
{
	internal abstract class AggregateBucket<T> : IPersistable where T : IPersistable
	{
		private List<T> m_aggregates;

		private int m_level;

		public List<T> Aggregates
		{
			get
			{
				return m_aggregates;
			}
			set
			{
				m_aggregates = value;
			}
		}

		public int Level
		{
			get
			{
				return m_level;
			}
			set
			{
				m_level = value;
			}
		}

		internal AggregateBucket()
		{
		}

		internal AggregateBucket(int level)
		{
			m_aggregates = new List<T>();
			m_level = level;
		}

		protected abstract Declaration GetSpecificDeclaration();

		public abstract Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType();

		public void Serialize(IntermediateFormatWriter writer)
		{
			writer.RegisterDeclaration(GetSpecificDeclaration());
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.Aggregates:
					writer.Write(m_aggregates);
					break;
				case MemberName.Level:
					writer.Write(m_level);
					break;
				default:
					Global.Tracer.Assert(condition: false);
					break;
				}
			}
		}

		public void Deserialize(IntermediateFormatReader reader)
		{
			reader.RegisterDeclaration(GetSpecificDeclaration());
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.Aggregates:
					m_aggregates = reader.ReadGenericListOfRIFObjects<T>();
					break;
				case MemberName.Level:
					m_level = reader.ReadInt32();
					break;
				default:
					Global.Tracer.Assert(condition: false);
					break;
				}
			}
		}

		public void ResolveReferences(Dictionary<Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
			Global.Tracer.Assert(condition: false, "No references to resolve.");
		}
	}
}
