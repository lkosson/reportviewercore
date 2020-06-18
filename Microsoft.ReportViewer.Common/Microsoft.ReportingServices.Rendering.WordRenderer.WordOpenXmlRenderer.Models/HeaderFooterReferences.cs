using Microsoft.ReportingServices.OnDemandProcessing.Scalability;
using Microsoft.ReportingServices.ReportIntermediateFormat.Persistence;
using System;
using System.Collections.Generic;
using System.IO;

namespace Microsoft.ReportingServices.Rendering.WordRenderer.WordOpenXmlRenderer.Models
{
	internal sealed class HeaderFooterReferences : BaseInterleaver, OpenXmlSectionPropertiesModel.IHeaderFooterReferences
	{
		[NonSerialized]
		private static Declaration _declaration;

		public string Header
		{
			get;
			set;
		}

		public string Footer
		{
			get;
			set;
		}

		public string FirstPageHeader
		{
			get;
			set;
		}

		public string FirstPageFooter
		{
			get;
			set;
		}

		public override int Size => base.Size + ItemSizes.SizeOf(Header) + ItemSizes.SizeOf(Footer) + ItemSizes.SizeOf(FirstPageHeader) + ItemSizes.SizeOf(FirstPageFooter);

		public HeaderFooterReferences(int index, long location, string footer, string header, string firstPageHeader, string firstPageFooter)
			: base(index, location)
		{
			Footer = footer;
			Header = header;
			FirstPageHeader = firstPageHeader;
			FirstPageFooter = firstPageFooter;
		}

		public HeaderFooterReferences()
		{
		}

		static HeaderFooterReferences()
		{
			_declaration = new Declaration(ObjectType.WordOpenXmlHeaderFooterReferences, ObjectType.WordOpenXmlBaseInterleaver, new List<MemberInfo>
			{
				new MemberInfo(MemberName.Header, Token.String),
				new MemberInfo(MemberName.Footer, Token.String),
				new MemberInfo(MemberName.FirstPageHeader, Token.String),
				new MemberInfo(MemberName.FirstPageFooter, Token.String)
			});
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(GetDeclaration());
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.Header:
					writer.Write(Header);
					break;
				case MemberName.Footer:
					writer.Write(Footer);
					break;
				case MemberName.FirstPageHeader:
					writer.Write(FirstPageHeader);
					break;
				case MemberName.FirstPageFooter:
					writer.Write(FirstPageFooter);
					break;
				default:
					WordOpenXmlUtils.FailSerializable();
					break;
				}
			}
		}

		public override void Deserialize(IntermediateFormatReader reader)
		{
			base.Deserialize(reader);
			reader.RegisterDeclaration(GetDeclaration());
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.Header:
					Header = reader.ReadString();
					break;
				case MemberName.Footer:
					Footer = reader.ReadString();
					break;
				case MemberName.FirstPageHeader:
					FirstPageHeader = reader.ReadString();
					break;
				case MemberName.FirstPageFooter:
					FirstPageFooter = reader.ReadString();
					break;
				default:
					WordOpenXmlUtils.FailSerializable();
					break;
				}
			}
		}

		public override ObjectType GetObjectType()
		{
			return ObjectType.WordOpenXmlHeaderFooterReferences;
		}

		internal new static Declaration GetDeclaration()
		{
			return _declaration;
		}

		public override void Write(TextWriter output)
		{
		}
	}
}
