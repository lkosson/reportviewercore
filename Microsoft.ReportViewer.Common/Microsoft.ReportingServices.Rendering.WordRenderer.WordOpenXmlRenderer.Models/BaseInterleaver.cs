using Microsoft.ReportingServices.OnDemandProcessing.Scalability;
using Microsoft.ReportingServices.ReportIntermediateFormat.Persistence;
using System;
using System.Collections.Generic;
using System.IO;

namespace Microsoft.ReportingServices.Rendering.WordRenderer.WordOpenXmlRenderer.Models
{
	internal abstract class BaseInterleaver : IInterleave, IStorable, IPersistable
	{
		private int _index;

		private long _location;

		[NonSerialized]
		private static Declaration _declaration;

		public int Index => _index;

		public long Location => _location;

		public virtual int Size => 12;

		protected BaseInterleaver(int index, long location)
		{
			_index = index;
			_location = location;
		}

		protected BaseInterleaver()
		{
		}

		static BaseInterleaver()
		{
			_declaration = new Declaration(ObjectType.WordOpenXmlBaseInterleaver, ObjectType.None, new List<MemberInfo>
			{
				new MemberInfo(MemberName.Index, Token.Int32),
				new MemberInfo(MemberName.Location, Token.Int64)
			});
		}

		public abstract void Write(TextWriter output);

		public virtual void Serialize(IntermediateFormatWriter writer)
		{
			writer.RegisterDeclaration(GetDeclaration());
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.Index:
					writer.Write(_index);
					break;
				case MemberName.Location:
					writer.Write(_location);
					break;
				default:
					WordOpenXmlUtils.FailSerializable();
					break;
				}
			}
		}

		public virtual void Deserialize(IntermediateFormatReader reader)
		{
			reader.RegisterDeclaration(GetDeclaration());
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.Index:
					_index = reader.ReadInt32();
					break;
				case MemberName.Location:
					_location = reader.ReadInt64();
					break;
				default:
					WordOpenXmlUtils.FailSerializable();
					break;
				}
			}
		}

		public virtual void ResolveReferences(Dictionary<ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
		}

		public virtual ObjectType GetObjectType()
		{
			return ObjectType.WordOpenXmlBaseInterleaver;
		}

		internal static Declaration GetDeclaration()
		{
			return _declaration;
		}
	}
}
