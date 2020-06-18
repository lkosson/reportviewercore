using Microsoft.ReportingServices.Diagnostics.Utilities;
using Microsoft.ReportingServices.OnDemandProcessing.Scalability;
using Microsoft.ReportingServices.OnDemandReportRendering;
using Microsoft.ReportingServices.Rendering.RPLProcessing;
using Microsoft.ReportingServices.ReportIntermediateFormat.Persistence;
using System.Collections.Generic;
using System.IO;

namespace Microsoft.ReportingServices.Rendering.HPBProcessing
{
	internal sealed class Line : PageItem, IStorable, IPersistable
	{
		private static Declaration m_declaration = GetDeclaration();

		internal Line()
		{
		}

		internal Line(Microsoft.ReportingServices.OnDemandReportRendering.Line source)
			: base(source)
		{
			m_itemPageSizes = new ItemSizes(source);
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(m_declaration);
			while (writer.NextMember())
			{
				_ = writer.CurrentMember.MemberName;
				RSTrace.RenderingTracer.Assert(condition: false, string.Empty);
			}
		}

		public override void Deserialize(IntermediateFormatReader reader)
		{
			base.Deserialize(reader);
			reader.RegisterDeclaration(m_declaration);
			while (reader.NextMember())
			{
				_ = reader.CurrentMember.MemberName;
				RSTrace.RenderingTracer.Assert(condition: false, string.Empty);
			}
		}

		public override ObjectType GetObjectType()
		{
			return ObjectType.Line;
		}

		internal new static Declaration GetDeclaration()
		{
			if (m_declaration == null)
			{
				List<MemberInfo> memberInfoList = new List<MemberInfo>();
				return new Declaration(ObjectType.Line, ObjectType.PageItem, memberInfoList);
			}
			return m_declaration;
		}

		internal override void WriteStartItemToStream(RPLWriter rplWriter, PageContext pageContext)
		{
			if (rplWriter != null)
			{
				BinaryWriter binaryWriter = rplWriter.BinaryWriter;
				if (binaryWriter != null)
				{
					long position = binaryWriter.BaseStream.Position;
					binaryWriter.Write((byte)8);
					WriteElementProps(binaryWriter, rplWriter, pageContext, position + 1);
					m_offset = binaryWriter.BaseStream.Position;
					binaryWriter.Write((byte)254);
					binaryWriter.Write(position);
					binaryWriter.Write(byte.MaxValue);
				}
				else if (m_rplElement == null)
				{
					m_rplElement = new RPLLine();
					WriteElementProps(m_rplElement.ElementProps, pageContext);
				}
			}
		}

		internal override void WriteCustomSharedItemProps(BinaryWriter spbifWriter, RPLWriter rplWriter, PageContext pageContext)
		{
			if (((Microsoft.ReportingServices.OnDemandReportRendering.Line)m_source).Slant)
			{
				spbifWriter.Write((byte)24);
				spbifWriter.Write(value: true);
			}
		}

		internal override void WriteCustomSharedItemProps(RPLElementPropsDef sharedProps, PageContext pageContext)
		{
			((RPLLinePropsDef)sharedProps).Slant = ((Microsoft.ReportingServices.OnDemandReportRendering.Line)m_source).Slant;
		}
	}
}
