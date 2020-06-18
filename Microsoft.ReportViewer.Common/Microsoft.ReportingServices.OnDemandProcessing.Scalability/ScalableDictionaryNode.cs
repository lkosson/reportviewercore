using Microsoft.ReportingServices.ReportIntermediateFormat.Persistence;
using Microsoft.ReportingServices.ReportProcessing;
using System;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.OnDemandProcessing.Scalability
{
	internal sealed class ScalableDictionaryNode : IScalableDictionaryEntry, IStorable, IPersistable, ITransferable
	{
		internal IScalableDictionaryEntry[] Entries;

		internal int Count;

		[NonSerialized]
		private static readonly Declaration m_declaration = GetDeclaration();

		public int Size => 4 + ItemSizes.SizeOf(Entries);

		public int EmptySize => ItemSizes.NonNullIStorableOverhead + 4 + ItemSizes.SizeOfEmptyObjectArray(Entries.Length);

		internal ScalableDictionaryNode()
		{
		}

		internal ScalableDictionaryNode(int capacity)
		{
			Entries = new IScalableDictionaryEntry[capacity];
		}

		public void Serialize(IntermediateFormatWriter writer)
		{
			writer.RegisterDeclaration(m_declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.Entries:
					writer.Write(Entries);
					break;
				case MemberName.Count:
					writer.Write(Count);
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
				case MemberName.Entries:
					Entries = reader.ReadArrayOfRIFObjects<IScalableDictionaryEntry>();
					break;
				case MemberName.Count:
					Count = reader.ReadInt32();
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
			return Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ScalableDictionaryNode;
		}

		internal static Declaration GetDeclaration()
		{
			if (m_declaration == null)
			{
				List<MemberInfo> list = new List<MemberInfo>();
				list.Add(new MemberInfo(MemberName.Entries, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectArray, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.IScalableDictionaryEntry));
				list.Add(new MemberInfo(MemberName.Count, Token.Int32));
				return new Declaration(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ScalableDictionaryNode, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.None, list);
			}
			return m_declaration;
		}

		public void TransferTo(IScalabilityCache scaleCache)
		{
			for (int i = 0; i < Entries.Length; i++)
			{
				IScalableDictionaryEntry scalableDictionaryEntry = Entries[i];
				if (scalableDictionaryEntry != null)
				{
					switch (scalableDictionaryEntry.GetObjectType())
					{
					case Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ScalableDictionaryNodeReference:
					{
						ScalableDictionaryNodeReference scalableDictionaryNodeReference = scalableDictionaryEntry as ScalableDictionaryNodeReference;
						Entries[i] = (ScalableDictionaryNodeReference)scalableDictionaryNodeReference.TransferTo(scaleCache);
						break;
					}
					case Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ScalableDictionaryValues:
						(scalableDictionaryEntry as ScalableDictionaryValues).TransferTo(scaleCache);
						break;
					default:
						Global.Tracer.Assert(condition: false, "Unknown ObjectType");
						break;
					}
				}
			}
		}
	}
}
