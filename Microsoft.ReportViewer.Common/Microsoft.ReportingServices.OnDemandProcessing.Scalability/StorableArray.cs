using Microsoft.ReportingServices.ReportIntermediateFormat.Persistence;
using Microsoft.ReportingServices.ReportProcessing;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.OnDemandProcessing.Scalability
{
	internal sealed class StorableArray : IStorable, IPersistable, ITransferable
	{
		internal object[] Array;

		private static Declaration m_declaration = GetDeclaration();

		public int Size => ItemSizes.SizeOf(Array);

		public int EmptySize => ItemSizes.NonNullIStorableOverhead + ItemSizes.SizeOfEmptyObjectArray(Array.Length);

		public void Serialize(IntermediateFormatWriter writer)
		{
			writer.RegisterDeclaration(m_declaration);
			while (writer.NextMember())
			{
				MemberName memberName = writer.CurrentMember.MemberName;
				if (memberName == MemberName.Array)
				{
					writer.WriteVariantOrPersistableArray(Array);
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
			while (reader.NextMember())
			{
				MemberName memberName = reader.CurrentMember.MemberName;
				if (memberName == MemberName.Array)
				{
					Array = reader.ReadVariantArray();
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
			return Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.StorableArray;
		}

		internal static Declaration GetDeclaration()
		{
			if (m_declaration == null)
			{
				List<MemberInfo> list = new List<MemberInfo>();
				list.Add(new MemberInfo(MemberName.Array, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.PrimitiveArray, Token.Object));
				return new Declaration(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.StorableArray, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.None, list);
			}
			return m_declaration;
		}

		public void TransferTo(IScalabilityCache scaleCache)
		{
			if (Array == null || Array.Length == 0)
			{
				return;
			}
			IReference reference = Array[0] as IReference;
			if (reference != null)
			{
				Array[0] = reference.TransferTo(scaleCache);
				for (int i = 1; i < Array.Length; i++)
				{
					reference = (Array[i] as IReference);
					if (reference != null)
					{
						Array[i] = reference.TransferTo(scaleCache);
					}
				}
				return;
			}
			ITransferable transferable = Array[0] as ITransferable;
			if (transferable != null)
			{
				transferable.TransferTo(scaleCache);
				for (int j = 1; j < Array.Length; j++)
				{
					(Array[j] as ITransferable)?.TransferTo(scaleCache);
				}
			}
		}
	}
}
