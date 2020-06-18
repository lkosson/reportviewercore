using Microsoft.ReportingServices.ReportIntermediateFormat.Persistence;
using Microsoft.ReportingServices.ReportProcessing;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.OnDemandProcessing.Scalability
{
	internal sealed class ScalableDictionaryValues : IScalableDictionaryEntry, IStorable, IPersistable, ITransferable
	{
		private object[] m_keys;

		private object[] m_values;

		private int m_count;

		private static readonly Declaration m_declaration = GetDeclaration();

		public object[] Keys => m_keys;

		public object[] Values => m_values;

		public int Count
		{
			get
			{
				return m_count;
			}
			set
			{
				m_count = value;
			}
		}

		public int Capacity => m_keys.Length;

		public int Size => 4 + ItemSizes.SizeOf(m_keys) + ItemSizes.SizeOf(m_values);

		public int EmptySize => ItemSizes.NonNullIStorableOverhead + 4 + ItemSizes.SizeOfEmptyObjectArray(m_keys.Length) * 2;

		internal ScalableDictionaryValues()
		{
		}

		public ScalableDictionaryValues(int capacity)
		{
			m_count = 0;
			m_keys = new object[capacity];
			m_values = new object[capacity];
		}

		public void Serialize(IntermediateFormatWriter writer)
		{
			writer.RegisterDeclaration(m_declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.Keys:
					writer.WriteVariantOrPersistableArray(m_keys);
					break;
				case MemberName.Values:
					writer.WriteVariantOrPersistableArray(m_values);
					break;
				case MemberName.Count:
					writer.Write(m_count);
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
				case MemberName.Keys:
					m_keys = reader.ReadVariantArray();
					break;
				case MemberName.Values:
					m_values = reader.ReadVariantArray();
					break;
				case MemberName.Count:
					m_count = reader.ReadInt32();
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
			return Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ScalableDictionaryValues;
		}

		internal static Declaration GetDeclaration()
		{
			if (m_declaration == null)
			{
				List<MemberInfo> list = new List<MemberInfo>();
				list.Add(new MemberInfo(MemberName.Keys, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.PrimitiveArray, Token.Object));
				list.Add(new MemberInfo(MemberName.Values, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.PrimitiveArray, Token.Object));
				list.Add(new MemberInfo(MemberName.Count, Token.Int32));
				return new Declaration(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ScalableDictionaryValues, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.None, list);
			}
			return m_declaration;
		}

		public void TransferTo(IScalabilityCache scaleCache)
		{
			for (int i = 0; i < m_count; i++)
			{
				(m_values[i] as ITransferable)?.TransferTo(scaleCache);
			}
		}
	}
}
