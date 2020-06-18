using Microsoft.ReportingServices.ReportIntermediateFormat.Persistence;
using Microsoft.ReportingServices.ReportProcessing;
using System.Collections;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.ReportIntermediateFormat
{
	internal abstract class BucketedAggregatesCollection<T> : IEnumerable<T>, IEnumerable, IPersistable where T : IPersistable
	{
		private List<AggregateBucket<T>> m_buckets;

		public List<AggregateBucket<T>> Buckets
		{
			get
			{
				return m_buckets;
			}
			set
			{
				m_buckets = value;
			}
		}

		public bool IsEmpty => m_buckets.Count == 0;

		public BucketedAggregatesCollection()
		{
			m_buckets = new List<AggregateBucket<T>>();
		}

		public AggregateBucket<T> GetOrCreateBucket(int level)
		{
			AggregateBucket<T> aggregateBucket = null;
			for (int i = 0; i < m_buckets.Count; i++)
			{
				if (aggregateBucket != null)
				{
					break;
				}
				AggregateBucket<T> aggregateBucket2 = m_buckets[i];
				if (aggregateBucket2.Level == level)
				{
					aggregateBucket = aggregateBucket2;
				}
				else if (aggregateBucket2.Level > level)
				{
					aggregateBucket = CreateBucket(level);
					m_buckets.Insert(i, aggregateBucket);
				}
			}
			if (aggregateBucket == null)
			{
				aggregateBucket = CreateBucket(level);
				m_buckets.Add(aggregateBucket);
			}
			return aggregateBucket;
		}

		public void MergeFrom(BucketedAggregatesCollection<T> otherCol)
		{
			if (otherCol == null)
			{
				return;
			}
			foreach (AggregateBucket<T> bucket in otherCol.Buckets)
			{
				GetOrCreateBucket(bucket.Level).Aggregates.AddRange(bucket.Aggregates);
			}
		}

		public IEnumerator<T> GetEnumerator()
		{
			foreach (AggregateBucket<T> bucket in m_buckets)
			{
				foreach (T aggregate in bucket.Aggregates)
				{
					yield return aggregate;
				}
			}
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		protected abstract AggregateBucket<T> CreateBucket(int level);

		protected abstract Declaration GetSpecificDeclaration();

		public abstract Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType();

		public void Serialize(IntermediateFormatWriter writer)
		{
			writer.RegisterDeclaration(GetSpecificDeclaration());
			while (writer.NextMember())
			{
				MemberName memberName = writer.CurrentMember.MemberName;
				if (memberName == MemberName.Buckets)
				{
					writer.Write(m_buckets);
				}
				else
				{
					Global.Tracer.Assert(condition: false);
				}
			}
		}

		public void Deserialize(IntermediateFormatReader reader)
		{
			reader.RegisterDeclaration(GetSpecificDeclaration());
			while (reader.NextMember())
			{
				MemberName memberName = reader.CurrentMember.MemberName;
				if (memberName == MemberName.Buckets)
				{
					m_buckets = reader.ReadGenericListOfRIFObjects<AggregateBucket<T>>();
				}
				else
				{
					Global.Tracer.Assert(condition: false);
				}
			}
		}

		public void ResolveReferences(Dictionary<Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
			Global.Tracer.Assert(condition: false, "No references to resolve.");
		}
	}
}
