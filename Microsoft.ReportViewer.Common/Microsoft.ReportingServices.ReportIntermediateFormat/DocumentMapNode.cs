using Microsoft.ReportingServices.ReportIntermediateFormat.Persistence;
using Microsoft.ReportingServices.ReportProcessing;
using System;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.ReportIntermediateFormat
{
	[Serializable]
	internal sealed class DocumentMapNode : IPersistable
	{
		internal const char IdLevelSeparator = '_';

		private string m_id;

		private string m_label;

		[NonSerialized]
		private static readonly Declaration m_Declaration = GetDeclaration();

		internal string Label
		{
			get
			{
				return m_label;
			}
			set
			{
				m_label = value;
			}
		}

		internal string Id
		{
			get
			{
				return m_id;
			}
			set
			{
				Global.Tracer.Assert(value != null, "The id of a document map node cannot be null.");
				m_id = value;
			}
		}

		internal DocumentMapNode()
		{
		}

		internal static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.ID, Token.String));
			list.Add(new MemberInfo(MemberName.Label, Token.String));
			return new Declaration(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.DocumentMapNode, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.None, list);
		}

		public void Serialize(IntermediateFormatWriter writer)
		{
			writer.RegisterDeclaration(m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.ID:
					writer.Write(m_id);
					break;
				case MemberName.Label:
					writer.Write(m_label);
					break;
				default:
					Global.Tracer.Assert(condition: false);
					break;
				}
			}
		}

		public void Deserialize(IntermediateFormatReader reader)
		{
			reader.RegisterDeclaration(m_Declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.ID:
					m_id = reader.ReadString();
					break;
				case MemberName.Label:
					m_label = reader.ReadString();
					break;
				default:
					Global.Tracer.Assert(condition: false);
					break;
				}
			}
		}

		public void ResolveReferences(Dictionary<Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
			Global.Tracer.Assert(condition: false);
		}

		public Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.DocumentMapNode;
		}
	}
}
