using Microsoft.ReportingServices.ReportIntermediateFormat.Persistence;
using Microsoft.ReportingServices.ReportProcessing;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Microsoft.ReportingServices.OnDemandProcessing.Scalability
{
	internal abstract class BaseReference : IReference, IStorable, IPersistable, IDisposable
	{
		private ReferenceID m_id;

		[NonSerialized]
		protected BaseScalabilityCache m_scalabilityCache;

		[NonSerialized]
		internal ItemHolder Item;

		[NonSerialized]
		private int m_pinCount;

		[NonSerialized]
		private static readonly Declaration m_declaration = GetDeclaration();

		public ReferenceID Id
		{
			get
			{
				return m_id;
			}
			set
			{
				m_id = value;
			}
		}

		internal int PinCount
		{
			get
			{
				return m_pinCount;
			}
			set
			{
				m_pinCount = value;
			}
		}

		internal BaseScalabilityCache ScalabilityCache => m_scalabilityCache;

		internal InQueueState InQueue
		{
			get
			{
				if (Item != null)
				{
					return Item.InQueue;
				}
				return InQueueState.None;
			}
			set
			{
				Item.InQueue = value;
			}
		}

		public int Size
		{
			get
			{
				int num = 16 + ItemSizes.ReferenceSize + 4 + ItemSizes.ReferenceSize;
				if (Item != null)
				{
					num += Item.ComputeSizeForReference();
				}
				return num;
			}
		}

		internal void Init(BaseScalabilityCache storageManager)
		{
			SetScalabilityCache(storageManager);
		}

		internal void Init(BaseScalabilityCache storageManager, ReferenceID id)
		{
			SetScalabilityCache(storageManager);
			m_id = id;
		}

		public IReference TransferTo(IScalabilityCache scaleCache)
		{
			return ((BaseScalabilityCache)scaleCache).TransferTo(this);
		}

		public IDisposable PinValue()
		{
			m_pinCount++;
			m_scalabilityCache.Pin(this);
			return this;
		}

		public void UnPinValue()
		{
			m_pinCount--;
			m_scalabilityCache.UnPin(this);
		}

		public void Free()
		{
			m_scalabilityCache.Free(this);
		}

		public void UpdateSize(int sizeBytesDelta)
		{
			m_scalabilityCache.UpdateTargetSize(this, sizeBytesDelta);
		}

		[DebuggerStepThrough]
		internal IStorable InternalValue()
		{
			IStorable result;
			if (Item != null)
			{
				result = Item.Item;
				m_scalabilityCache.ReferenceValueCallback(this);
			}
			else
			{
				result = m_scalabilityCache.Retrieve(this);
			}
			return result;
		}

		private void SetScalabilityCache(BaseScalabilityCache cache)
		{
			m_scalabilityCache = cache;
		}

		public void Serialize(IntermediateFormatWriter writer)
		{
			m_scalabilityCache.ReferenceSerializeCallback(this);
			long value = m_id.Value;
			if (writer.PersistenceHelper != m_scalabilityCache)
			{
				BaseScalabilityCache obj = writer.PersistenceHelper as BaseScalabilityCache;
				PairObj<ReferenceID, BaseScalabilityCache> item = new PairObj<ReferenceID, BaseScalabilityCache>(m_id, m_scalabilityCache);
				value = obj.StoreStaticReference(item);
			}
			writer.RegisterDeclaration(m_declaration);
			while (writer.NextMember())
			{
				if (writer.CurrentMember.MemberName == MemberName.ID)
				{
					writer.Write(value);
				}
				else
				{
					Global.Tracer.Assert(condition: false);
				}
			}
		}

		public void Deserialize(IntermediateFormatReader reader)
		{
			reader.RegisterDeclaration(m_declaration);
			long num = 0L;
			while (reader.NextMember())
			{
				if (reader.CurrentMember.MemberName == MemberName.ID)
				{
					num = reader.ReadInt64();
				}
				else
				{
					Global.Tracer.Assert(condition: false);
				}
			}
			BaseScalabilityCache baseScalabilityCache = reader.PersistenceHelper as BaseScalabilityCache;
			ScalabilityCacheType cacheType = baseScalabilityCache.CacheType;
			if (num < 0 && cacheType != ScalabilityCacheType.GroupTree && cacheType != ScalabilityCacheType.Lookup)
			{
				PairObj<ReferenceID, BaseScalabilityCache> pairObj = (PairObj<ReferenceID, BaseScalabilityCache>)baseScalabilityCache.FetchStaticReference((int)num);
				m_id = pairObj.First;
				baseScalabilityCache = pairObj.Second;
			}
			else
			{
				m_id = new ReferenceID(num);
			}
			SetScalabilityCache(baseScalabilityCache);
		}

		public void ResolveReferences(Dictionary<Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
			Global.Tracer.Assert(condition: false);
		}

		public abstract Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType();

		internal static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.ID, Token.Int64));
			return new Declaration(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Reference, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Reference, list);
		}

		void IDisposable.Dispose()
		{
			UnPinValue();
		}

		public static bool operator ==(BaseReference reference, object obj)
		{
			if ((object)reference == obj)
			{
				return true;
			}
			return reference?.Equals(obj) ?? false;
		}

		public static bool operator !=(BaseReference reference, object obj)
		{
			return !(reference == obj);
		}

		public override bool Equals(object obj)
		{
			if (obj == null)
			{
				return false;
			}
			BaseReference baseReference = obj as BaseReference;
			if (baseReference == null)
			{
				return false;
			}
			return m_id == baseReference.m_id;
		}

		public override int GetHashCode()
		{
			return (int)m_id.Value;
		}
	}
}
