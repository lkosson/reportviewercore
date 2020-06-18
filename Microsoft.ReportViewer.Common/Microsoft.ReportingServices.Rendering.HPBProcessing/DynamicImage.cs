using Microsoft.ReportingServices.Diagnostics.Utilities;
using Microsoft.ReportingServices.OnDemandProcessing.Scalability;
using Microsoft.ReportingServices.OnDemandReportRendering;
using Microsoft.ReportingServices.Rendering.RPLProcessing;
using Microsoft.ReportingServices.ReportIntermediateFormat.Persistence;
using System.Collections.Generic;
using System.IO;

namespace Microsoft.ReportingServices.Rendering.HPBProcessing
{
	internal abstract class DynamicImage : PageItem, IStorable, IPersistable
	{
		private static Declaration m_declaration = GetDeclaration();

		protected abstract byte ElementToken
		{
			get;
		}

		protected abstract bool SpecialBorderHandling
		{
			get;
		}

		internal DynamicImage()
		{
		}

		internal DynamicImage(ReportItem source)
			: base(source)
		{
			m_itemPageSizes = new ItemSizes(source);
			base.UnresolvedPBS = (base.UnresolvedPBE = true);
		}

		protected abstract RPLItem CreateRPLItem();

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
			return ObjectType.DynamicImage;
		}

		internal new static Declaration GetDeclaration()
		{
			if (m_declaration == null)
			{
				List<MemberInfo> memberInfoList = new List<MemberInfo>();
				return new Declaration(ObjectType.DynamicImage, ObjectType.PageItem, memberInfoList);
			}
			return m_declaration;
		}

		private Stream LoadDynamicImage(out ActionInfoWithDynamicImageMapCollection actionImageMaps, PageContext pageContext)
		{
			IDynamicImageInstance dynamicImageInstance = (IDynamicImageInstance)m_source.Instance;
			dynamicImageInstance.SetDpi(pageContext.DynamicImageDpiX, pageContext.DynamicImageDpiY);
			if (pageContext.IsInSelectiveRendering)
			{
				dynamicImageInstance.SetSize(pageContext.Common.Pagination.PhysicalPageWidth, pageContext.Common.Pagination.PhysicalPageHeight);
			}
			return dynamicImageInstance.GetImage(pageContext.EMFDynamicImages ? DynamicImageInstance.ImageType.EMF : DynamicImageInstance.ImageType.PNG, out actionImageMaps);
		}

		internal override void WriteStartItemToStream(RPLWriter rplWriter, PageContext pageContext)
		{
			if (rplWriter != null)
			{
				BinaryWriter binaryWriter = rplWriter.BinaryWriter;
				if (binaryWriter != null)
				{
					Stream baseStream = binaryWriter.BaseStream;
					long position = baseStream.Position;
					binaryWriter.Write(ElementToken);
					WriteElementProps(binaryWriter, rplWriter, pageContext, position + 1);
					m_offset = baseStream.Position;
					binaryWriter.Write((byte)254);
					binaryWriter.Write(position);
					binaryWriter.Write(byte.MaxValue);
				}
				else if (m_rplElement == null)
				{
					m_rplElement = CreateRPLItem();
					WriteElementProps(m_rplElement.ElementProps, pageContext);
				}
			}
		}

		internal override void WriteCustomNonSharedItemProps(BinaryWriter spbifWriter, PageContext pageContext)
		{
			ActionInfoWithDynamicImageMapCollection actionImageMaps = null;
			Stream stream = LoadDynamicImage(out actionImageMaps, pageContext);
			if (stream != null)
			{
				spbifWriter.Write((byte)39);
				spbifWriter.Write((int)stream.Length);
				byte[] array = new byte[4096];
				stream.Position = 0L;
				for (int num = stream.Read(array, 0, array.Length); num != 0; num = stream.Read(array, 0, array.Length))
				{
					spbifWriter.Write(array, 0, num);
				}
			}
		}

		internal override void WriteCustomNonSharedItemProps(RPLElementProps nonSharedProps, PageContext pageContext)
		{
			ActionInfoWithDynamicImageMapCollection actionImageMaps = null;
			((RPLDynamicImageProps)nonSharedProps).DynamicImageContent = LoadDynamicImage(out actionImageMaps, pageContext);
		}

		internal override void WriteItemSharedStyleProps(BinaryWriter spbifWriter, Style style, PageContext pageContext)
		{
			WriteStyleProp(style, spbifWriter, StyleAttributeNames.BackgroundColor, 34);
		}

		internal override void WriteItemSharedStyleProps(RPLStyleProps styleProps, Style style, PageContext pageContext)
		{
			PageItem.WriteStyleProp(style, styleProps, StyleAttributeNames.BackgroundColor, 34);
		}

		internal override void WriteItemNonSharedStyleProps(BinaryWriter spbifWriter, Style styleDef, StyleInstance style, StyleAttributeNames styleAtt, PageContext pageContext)
		{
			if (styleAtt == StyleAttributeNames.BackgroundColor)
			{
				WriteStyleProp(styleDef, style, spbifWriter, StyleAttributeNames.BackgroundColor, 34);
			}
		}

		internal override void WriteItemNonSharedStyleProps(RPLStyleProps styleProps, Style styleDef, StyleInstance style, StyleAttributeNames styleAtt, PageContext pageContext)
		{
			if (styleAtt == StyleAttributeNames.BackgroundColor)
			{
				WriteStyleProp(styleDef, style, styleProps, StyleAttributeNames.BackgroundColor, 34);
			}
		}

		internal override void WriteBorderProps(BinaryWriter spbifWriter, Style style)
		{
			if (!SpecialBorderHandling)
			{
				base.WriteBorderProps(spbifWriter, style);
			}
		}

		internal override void WriteBorderProps(RPLStyleProps rplStyleProps, Style style)
		{
			if (!SpecialBorderHandling)
			{
				base.WriteBorderProps(rplStyleProps, style);
			}
		}

		internal override void WriteNonSharedStyleProp(BinaryWriter spbifWriter, Style styleDef, StyleInstance style, StyleAttributeNames styleAttribute, PageContext pageContext)
		{
			if (!SpecialBorderHandling)
			{
				base.WriteNonSharedStyleProp(spbifWriter, styleDef, style, styleAttribute, pageContext);
			}
			else
			{
				WriteItemNonSharedStyleProps(spbifWriter, styleDef, style, styleAttribute, pageContext);
			}
		}

		internal override void WriteNonSharedStyleProp(RPLStyleProps rplStyleProps, Style styleDef, StyleInstance style, StyleAttributeNames styleAttribute, PageContext pageContext)
		{
			if (!SpecialBorderHandling)
			{
				base.WriteNonSharedStyleProp(rplStyleProps, styleDef, style, styleAttribute, pageContext);
			}
			else
			{
				WriteItemNonSharedStyleProps(rplStyleProps, styleDef, style, styleAttribute, pageContext);
			}
		}
	}
}
