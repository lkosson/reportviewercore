using Microsoft.ReportingServices.Diagnostics.Utilities;
using Microsoft.ReportingServices.OnDemandProcessing.Scalability;
using Microsoft.ReportingServices.ReportIntermediateFormat.Persistence;
using System;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.Rendering.ExcelRenderer.SPBIFReader.Callbacks
{
	internal class ToggleParent : IStorable, IPersistable
	{
		internal int m_top;

		internal int m_left;

		internal int m_width;

		internal int m_height;

		[NonSerialized]
		private static Declaration m_declaration = GetDeclaration();

		internal int Top => m_top;

		internal int Left => m_left;

		internal int Width => m_width;

		internal int Height => m_height;

		public int Size => 16;

		internal ToggleParent()
		{
		}

		internal ToggleParent(int top, int left, int width, int height)
		{
			m_top = top;
			m_left = left;
			m_width = width;
			m_height = height;
		}

		public void Serialize(IntermediateFormatWriter writer)
		{
			writer.RegisterDeclaration(m_declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.Top:
					writer.Write(m_top);
					break;
				case MemberName.Left:
					writer.Write(m_left);
					break;
				case MemberName.Width:
					writer.Write(m_width);
					break;
				case MemberName.Height:
					writer.Write(m_height);
					break;
				default:
					RSTrace.ExcelRendererTracer.Assert(condition: false);
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
				case MemberName.Top:
					m_top = reader.ReadInt32();
					break;
				case MemberName.Left:
					m_left = reader.ReadInt32();
					break;
				case MemberName.Width:
					m_width = reader.ReadInt32();
					break;
				case MemberName.Height:
					m_height = reader.ReadInt32();
					break;
				default:
					RSTrace.ExcelRendererTracer.Assert(condition: false);
					break;
				}
			}
		}

		public void ResolveReferences(Dictionary<ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
		}

		public ObjectType GetObjectType()
		{
			return ObjectType.ToggleParent;
		}

		internal static Declaration GetDeclaration()
		{
			if (m_declaration == null)
			{
				List<MemberInfo> list = new List<MemberInfo>();
				list.Add(new MemberInfo(MemberName.Top, Token.Int32));
				list.Add(new MemberInfo(MemberName.Left, Token.Int32));
				list.Add(new MemberInfo(MemberName.Width, Token.Int32));
				list.Add(new MemberInfo(MemberName.Height, Token.Int32));
				return new Declaration(ObjectType.ToggleParent, ObjectType.None, list);
			}
			return m_declaration;
		}
	}
}
