using Microsoft.ReportingServices.ReportIntermediateFormat.Persistence;
using Microsoft.ReportingServices.ReportProcessing;
using System;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.ReportIntermediateFormat
{
	internal class FieldInfo : IPersistable
	{
		private List<int> m_propertyReaderIndices;

		private List<string> m_propertyNames;

		[NonSerialized]
		internal bool ErrorRegistered;

		[NonSerialized]
		internal bool Missing;

		[NonSerialized]
		private readonly bool[] m_propertyErrorRegistered;

		[NonSerialized]
		private static readonly Declaration m_declaration = GetDeclaration();

		internal int PropertyCount
		{
			get
			{
				if (PropertyReaderIndices == null)
				{
					return 0;
				}
				return PropertyReaderIndices.Count;
			}
		}

		internal List<int> PropertyReaderIndices => m_propertyReaderIndices;

		internal List<string> PropertyNames => m_propertyNames;

		internal FieldInfo()
		{
			m_propertyErrorRegistered = new bool[0];
		}

		internal FieldInfo(List<int> aPropIndices, List<string> aPropNames)
		{
			m_propertyReaderIndices = aPropIndices;
			m_propertyNames = aPropNames;
			m_propertyErrorRegistered = new bool[aPropIndices.Count];
		}

		internal bool IsPropertyErrorRegistered(int aIndex)
		{
			if (m_propertyErrorRegistered == null)
			{
				return false;
			}
			return m_propertyErrorRegistered[aIndex];
		}

		internal void SetPropertyErrorRegistered(int aIndex)
		{
			m_propertyErrorRegistered[aIndex] = true;
		}

		internal static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.FieldPropertyNames, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.PrimitiveList, Token.String));
			list.Add(new MemberInfo(MemberName.FieldPropertyReaderIndices, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.PrimitiveList, Token.Int32));
			return new Declaration(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.FieldInfo, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.None, list);
		}

		public virtual void Serialize(IntermediateFormatWriter writer)
		{
			writer.RegisterDeclaration(m_declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.FieldPropertyNames:
					writer.WriteListOfPrimitives(m_propertyNames);
					break;
				case MemberName.FieldPropertyReaderIndices:
					writer.WriteListOfPrimitives(m_propertyReaderIndices);
					break;
				default:
					Global.Tracer.Assert(condition: false);
					break;
				}
			}
		}

		public virtual void Deserialize(IntermediateFormatReader reader)
		{
			reader.RegisterDeclaration(m_declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.FieldPropertyNames:
					m_propertyNames = reader.ReadListOfPrimitives<string>();
					break;
				case MemberName.FieldPropertyReaderIndices:
					m_propertyReaderIndices = reader.ReadListOfPrimitives<int>();
					break;
				default:
					Global.Tracer.Assert(condition: false);
					break;
				}
			}
		}

		public virtual void ResolveReferences(Dictionary<Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
			Global.Tracer.Assert(condition: false);
		}

		public virtual Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.FieldInfo;
		}
	}
}
