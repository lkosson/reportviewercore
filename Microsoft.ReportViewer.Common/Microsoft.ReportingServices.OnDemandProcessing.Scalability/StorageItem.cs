using Microsoft.ReportingServices.ReportIntermediateFormat.Persistence;
using Microsoft.ReportingServices.ReportProcessing;
using System;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.OnDemandProcessing.Scalability
{
	internal class StorageItem : ItemHolder, IStorable, IPersistable
	{
		internal int Priority;

		internal ReferenceID Id;

		[NonSerialized]
		internal long PersistedSize;

		[NonSerialized]
		private int m_size;

		[NonSerialized]
		internal int PinCount;

		[NonSerialized]
		internal bool HasBeenUnPinned;

		[NonSerialized]
		internal long Offset = -1L;

		[NonSerialized]
		private LinkedBucketedQueue<BaseReference> m_otherReferences;

		[NonSerialized]
		private static readonly Declaration m_declaration = GetDeclaration();

		public int Size => m_size;

		internal int ActiveReferenceCount
		{
			get
			{
				int num = 0;
				if (Reference != null)
				{
					num++;
				}
				if (m_otherReferences != null)
				{
					num += m_otherReferences.Count;
				}
				return num;
			}
		}

		public StorageItem()
		{
		}

		public StorageItem(ReferenceID id, int priority, IStorable item, int initialSize)
		{
			Priority = priority;
			Item = item;
			Offset = -1L;
			Id = id;
			PersistedSize = -1L;
			m_size = CalculateSize(initialSize);
		}

		public void AddReference(BaseReference newReference)
		{
			if (Reference == null)
			{
				Reference = newReference;
				return;
			}
			if (m_otherReferences == null)
			{
				m_otherReferences = new LinkedBucketedQueue<BaseReference>(5);
			}
			m_otherReferences.Enqueue(newReference);
		}

		public int UpdateSize()
		{
			int size = m_size;
			m_size = CalculateSize(ItemSizes.SizeOf(Item));
			return m_size - size;
		}

		private int CalculateSize(int itemSize)
		{
			return BaseSize() + itemSize + 8 + 8 + 4 + 4 + 1 + 8 + ItemSizes.ReferenceSize;
		}

		internal override int ComputeSizeForReference()
		{
			return 0;
		}

		public void UpdateSize(int sizeDeltaBytes)
		{
			m_size += sizeDeltaBytes;
		}

		public void Flush(IStorage storage, IIndexStrategy indexStrategy)
		{
			bool isTemporary = Id.IsTemporary;
			if (isTemporary)
			{
				Id = indexStrategy.GenerateId(Id);
			}
			UnlinkReferences(isTemporary);
			if (Offset >= 0)
			{
				long num = storage.Update(this, Offset, PersistedSize);
				if (num != Offset)
				{
					Offset = num;
					indexStrategy.Update(Id, Offset);
				}
			}
			else
			{
				Offset = storage.Allocate(this);
				indexStrategy.Update(Id, Offset);
			}
		}

		internal void UnlinkReferences(bool updateId)
		{
			if (Reference != null)
			{
				if (updateId)
				{
					Reference.Id = Id;
				}
				Reference.Item = null;
			}
			if (m_otherReferences != null)
			{
				while (m_otherReferences.Count > 0)
				{
					BaseReference baseReference = m_otherReferences.Dequeue();
					baseReference.Item = null;
					if (updateId)
					{
						baseReference.Id = Id;
					}
				}
			}
			Reference = null;
			m_otherReferences = null;
		}

		public void Serialize(IntermediateFormatWriter writer)
		{
			writer.RegisterDeclaration(m_declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.Item:
					writer.Write(Item);
					break;
				case MemberName.Priority:
					writer.Write(Priority);
					break;
				case MemberName.ID:
					writer.Write(Id.Value);
					break;
				default:
					Global.Tracer.Assert(condition: false);
					break;
				}
			}
		}

		public void Deserialize(IntermediateFormatReader reader)
		{
			reader.RegisterDeclaration(m_declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.Item:
					Item = (IStorable)reader.ReadRIFObject();
					break;
				case MemberName.Priority:
					Priority = reader.ReadInt32();
					break;
				case MemberName.ID:
					Id = new ReferenceID(reader.ReadInt64());
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
			return Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.StorageItem;
		}

		[SkipMemberStaticValidation(MemberName.Item)]
		internal static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.Priority, Token.Int32));
			list.Add(new MemberInfo(MemberName.Item, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObject));
			list.Add(new MemberInfo(MemberName.ID, Token.Int64));
			return new Declaration(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.StorageItem, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.None, list);
		}
	}
}
