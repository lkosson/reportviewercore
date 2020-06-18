using Microsoft.ReportingServices.Diagnostics.Utilities;
using Microsoft.ReportingServices.OnDemandProcessing.Scalability;
using Microsoft.ReportingServices.OnDemandReportRendering;
using Microsoft.ReportingServices.Rendering.RichText;
using Microsoft.ReportingServices.Rendering.RPLProcessing;
using Microsoft.ReportingServices.ReportIntermediateFormat.Persistence;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace Microsoft.ReportingServices.Rendering.HPBProcessing
{
	internal sealed class Paragraph : IStorable, IPersistable, IParagraphProps
	{
		[StaticReference]
		public static Declaration m_declaration = GetDeclaration();

		[StaticReference]
		private Microsoft.ReportingServices.OnDemandReportRendering.Paragraph m_source;

		private Dictionary<byte, object> m_styles;

		private RPLFormat.ListStyles? m_listStyle;

		private int? m_listLevel;

		private int m_paragraphNumber;

		private ReportSize m_spaceBefore;

		private ReportSize m_spaceAfter;

		private ReportSize m_leftIndent;

		private ReportSize m_rightIndent;

		private ReportSize m_hangingIndent;

		private string m_uniqueName;

		private List<TextRun> m_textRuns = new List<TextRun>();

		private bool m_firstLine = true;

		public List<TextRun> TextRuns
		{
			get
			{
				return m_textRuns;
			}
			set
			{
				m_textRuns = value;
			}
		}

		public int ParagraphNumber
		{
			get
			{
				return m_paragraphNumber;
			}
			set
			{
				m_paragraphNumber = value;
			}
		}

		public string UniqueName => m_uniqueName;

		public bool FirstLine
		{
			get
			{
				return m_firstLine;
			}
			set
			{
				m_firstLine = value;
			}
		}

		public RPLFormat.TextAlignments Alignment
		{
			get
			{
				byte? b = null;
				if (m_source != null)
				{
					b = Utility.GetEnumProp(25, StyleAttributeNames.TextAlign, m_source.Style, m_styles);
				}
				RPLFormat.TextAlignments result = RPLFormat.TextAlignments.General;
				if (b.HasValue)
				{
					result = (RPLFormat.TextAlignments)b.Value;
				}
				return result;
			}
		}

		public float SpaceBefore
		{
			get
			{
				if (m_source == null)
				{
					return 0f;
				}
				return (float)Utility.GetSizePropertyValue(m_source.SpaceBefore, m_spaceBefore);
			}
		}

		public float SpaceAfter
		{
			get
			{
				if (m_source == null)
				{
					return 0f;
				}
				return (float)Utility.GetSizePropertyValue(m_source.SpaceAfter, m_spaceAfter);
			}
		}

		public float LeftIndent
		{
			get
			{
				if (m_source == null)
				{
					return 0f;
				}
				return (float)Utility.GetSizePropertyValue(m_source.LeftIndent, m_leftIndent);
			}
		}

		public float RightIndent
		{
			get
			{
				if (m_source == null)
				{
					return 0f;
				}
				return (float)Utility.GetSizePropertyValue(m_source.RightIndent, m_rightIndent);
			}
		}

		public float HangingIndent
		{
			get
			{
				if (m_source == null)
				{
					return 0f;
				}
				return (float)Utility.GetSizePropertyValue(m_source.HangingIndent, m_hangingIndent);
			}
		}

		public int ListLevel
		{
			get
			{
				if (m_source == null)
				{
					return 0;
				}
				return Utility.GetIntPropertyValue(m_source.ListLevel, m_listLevel);
			}
		}

		public RPLFormat.ListStyles ListStyle
		{
			get
			{
				RPLFormat.ListStyles result = RPLFormat.ListStyles.None;
				if (m_listStyle.HasValue)
				{
					result = m_listStyle.Value;
				}
				else if (m_source != null && m_source.ListStyle != null)
				{
					result = (RPLFormat.ListStyles)StyleEnumConverter.Translate(m_source.ListStyle.Value);
				}
				return result;
			}
		}

		public int Size => Microsoft.ReportingServices.OnDemandProcessing.Scalability.ItemSizes.SizeOf(m_textRuns) + Microsoft.ReportingServices.OnDemandProcessing.Scalability.ItemSizes.SizeOf(m_styles) + Microsoft.ReportingServices.OnDemandProcessing.Scalability.ItemSizes.NullableByteSize + Microsoft.ReportingServices.OnDemandProcessing.Scalability.ItemSizes.NullableInt32Size + 4 + Utility.ReportSizeItemSize(m_spaceBefore) + Utility.ReportSizeItemSize(m_spaceAfter) + Utility.ReportSizeItemSize(m_leftIndent) + Utility.ReportSizeItemSize(m_rightIndent) + Utility.ReportSizeItemSize(m_hangingIndent) + 1 + Microsoft.ReportingServices.OnDemandProcessing.Scalability.ItemSizes.SizeOf(m_uniqueName) + Microsoft.ReportingServices.OnDemandProcessing.Scalability.ItemSizes.ReferenceSize;

		internal Paragraph()
		{
		}

		internal Paragraph(string uniqueName)
		{
			m_uniqueName = uniqueName;
		}

		internal Paragraph(ParagraphInstance romParagraphInstance, string uniqueName, bool hideDuplicates)
		{
			m_source = romParagraphInstance.Definition;
			Microsoft.ReportingServices.OnDemandReportRendering.Paragraph source = m_source;
			Utility.AddInstanceStyles(romParagraphInstance.Style, ref m_styles);
			if (romParagraphInstance.IsCompiled)
			{
				m_spaceAfter = romParagraphInstance.SpaceAfter;
				m_spaceBefore = romParagraphInstance.SpaceBefore;
				m_leftIndent = romParagraphInstance.LeftIndent;
				m_rightIndent = romParagraphInstance.RightIndent;
				m_hangingIndent = romParagraphInstance.HangingIndent;
				m_listLevel = romParagraphInstance.ListLevel;
				m_listStyle = (RPLFormat.ListStyles)StyleEnumConverter.Translate(romParagraphInstance.ListStyle);
			}
			else
			{
				if (source.SpaceAfter != null && source.SpaceAfter.IsExpression)
				{
					m_spaceAfter = m_source.Instance.SpaceAfter;
				}
				if (source.SpaceBefore != null && source.SpaceBefore.IsExpression)
				{
					m_spaceBefore = m_source.Instance.SpaceBefore;
				}
				if (source.LeftIndent != null && source.LeftIndent.IsExpression)
				{
					m_leftIndent = m_source.Instance.LeftIndent;
				}
				if (source.RightIndent != null && source.RightIndent.IsExpression)
				{
					m_rightIndent = m_source.Instance.RightIndent;
				}
				if (source.HangingIndent != null && source.HangingIndent.IsExpression)
				{
					m_hangingIndent = m_source.Instance.HangingIndent;
				}
				if (source.ListLevel != null && source.ListLevel.IsExpression)
				{
					m_listLevel = m_source.Instance.ListLevel;
				}
				if (source.ListStyle != null && source.ListStyle.IsExpression)
				{
					m_listStyle = (RPLFormat.ListStyles)StyleEnumConverter.Translate(m_source.Instance.ListStyle);
				}
			}
			m_uniqueName = uniqueName;
			if (string.IsNullOrEmpty(uniqueName))
			{
				m_uniqueName = romParagraphInstance.UniqueName;
			}
			foreach (TextRunInstance textRunInstance in romParagraphInstance.TextRunInstances)
			{
				m_textRuns.Add(new TextRun(textRunInstance, hideDuplicates));
			}
		}

		internal Microsoft.ReportingServices.Rendering.RichText.Paragraph GetRichTextParagraph()
		{
			Microsoft.ReportingServices.Rendering.RichText.Paragraph paragraph = new Microsoft.ReportingServices.Rendering.RichText.Paragraph(this, m_textRuns.Count);
			foreach (TextRun textRun in m_textRuns)
			{
				Microsoft.ReportingServices.Rendering.RichText.TextRun richTextRun = textRun.GetRichTextRun();
				if (richTextRun != null)
				{
					paragraph.Runs.Add(richTextRun);
				}
			}
			return paragraph;
		}

		internal RPLParagraph GetRPLParagraph(PageContext pageContext, bool hideDuplicates, TextBox.TextBoxOffset startPosition, TextBox.TextBoxOffset endPosition, List<Microsoft.ReportingServices.Rendering.RichText.TextRun> richTextParaRuns)
		{
			RPLParagraph rPLParagraph = new RPLParagraph();
			WriteElementProps(rPLParagraph.ElementProps as RPLParagraphProps, pageContext);
			int num = 0;
			int num2 = 0;
			if (endPosition == null && startPosition == null)
			{
				foreach (TextRun textRun2 in m_textRuns)
				{
					num = num2;
					num2 += textRun2.SplitIndices.Count + 1;
					RPLTextRun rPLTextRun = textRun2.GetRPLTextRun(pageContext, hideDuplicates, null, null, num, richTextParaRuns);
					rPLParagraph.AddTextRun(rPLTextRun);
				}
				return rPLParagraph;
			}
			int count = m_textRuns.Count;
			int num3 = -1;
			if (startPosition != null)
			{
				num3 = startPosition.TextRunIndex;
			}
			int num4 = -1;
			if (endPosition != null)
			{
				num4 = endPosition.TextRunIndex;
			}
			for (int i = 0; i < count; i++)
			{
				TextRun textRun = m_textRuns[i];
				num = num2;
				num2 += textRun.SplitIndices.Count + 1;
				if (num3 >= num2)
				{
					continue;
				}
				if (endPosition == null)
				{
					rPLParagraph.AddTextRun(textRun.GetRPLTextRun(pageContext, hideDuplicates, startPosition, null, num, richTextParaRuns));
					continue;
				}
				if (num2 < num4)
				{
					rPLParagraph.AddTextRun(textRun.GetRPLTextRun(pageContext, hideDuplicates, startPosition, null, num, richTextParaRuns));
					continue;
				}
				if (num2 == num4)
				{
					rPLParagraph.AddTextRun(textRun.GetRPLTextRun(pageContext, hideDuplicates, startPosition, null, num, richTextParaRuns));
					if (endPosition.CharacterIndex <= 0)
					{
						break;
					}
					continue;
				}
				rPLParagraph.AddTextRun(textRun.GetRPLTextRun(pageContext, hideDuplicates, startPosition, endPosition, num, richTextParaRuns));
				break;
			}
			return rPLParagraph;
		}

		internal void WriteElementProps(RPLParagraphProps elemProps, PageContext pageContext)
		{
			Hashtable itemPropsStart = pageContext.ItemPropsStart;
			RPLParagraphPropsDef rPLParagraphPropsDef = pageContext.Common.GetFromCache<RPLParagraphPropsDef>(m_source.ID, out itemPropsStart);
			if (rPLParagraphPropsDef == null)
			{
				rPLParagraphPropsDef = new RPLParagraphPropsDef();
				rPLParagraphPropsDef.SharedStyle = new RPLStyleProps();
				WriteSharedStyles(new StyleWriterOM(rPLParagraphPropsDef.SharedStyle), m_source.Style);
				if (m_source.ListLevel != null && !m_source.ListLevel.IsExpression)
				{
					rPLParagraphPropsDef.ListLevel = m_source.ListLevel.Value;
				}
				if (m_source.ListStyle != null && !m_source.ListStyle.IsExpression)
				{
					rPLParagraphPropsDef.ListStyle = (RPLFormat.ListStyles)StyleEnumConverter.Translate(m_source.ListStyle.Value);
				}
				if (m_source.LeftIndent != null && !m_source.LeftIndent.IsExpression)
				{
					rPLParagraphPropsDef.LeftIndent = new RPLReportSize(m_source.LeftIndent.Value.ToString());
				}
				if (m_source.RightIndent != null && !m_source.RightIndent.IsExpression)
				{
					rPLParagraphPropsDef.RightIndent = new RPLReportSize(m_source.RightIndent.Value.ToString());
				}
				if (m_source.HangingIndent != null && !m_source.HangingIndent.IsExpression)
				{
					rPLParagraphPropsDef.HangingIndent = new RPLReportSize(m_source.HangingIndent.Value.ToString());
				}
				if (m_source.SpaceBefore != null && !m_source.SpaceBefore.IsExpression)
				{
					rPLParagraphPropsDef.SpaceBefore = new RPLReportSize(m_source.SpaceBefore.Value.ToString());
				}
				if (m_source.SpaceAfter != null && !m_source.SpaceAfter.IsExpression)
				{
					rPLParagraphPropsDef.SpaceAfter = new RPLReportSize(m_source.SpaceAfter.Value.ToString());
				}
				rPLParagraphPropsDef.ID = m_source.ID;
				itemPropsStart[m_source.ID] = rPLParagraphPropsDef;
			}
			elemProps.Definition = rPLParagraphPropsDef;
			if (m_leftIndent != null)
			{
				elemProps.LeftIndent = new RPLReportSize(m_leftIndent.ToString());
			}
			if (m_rightIndent != null)
			{
				elemProps.RightIndent = new RPLReportSize(m_rightIndent.ToString());
			}
			if (m_hangingIndent != null)
			{
				elemProps.HangingIndent = new RPLReportSize(m_hangingIndent.ToString());
			}
			if (m_listStyle.HasValue)
			{
				elemProps.ListStyle = m_listStyle.Value;
			}
			if (m_listLevel.HasValue)
			{
				elemProps.ListLevel = m_listLevel.Value;
			}
			if (m_spaceBefore != null)
			{
				elemProps.SpaceBefore = new RPLReportSize(m_spaceBefore.ToString());
			}
			if (m_spaceAfter != null)
			{
				elemProps.SpaceAfter = new RPLReportSize(m_spaceAfter.ToString());
			}
			elemProps.ParagraphNumber = m_paragraphNumber;
			elemProps.FirstLine = m_firstLine;
			elemProps.UniqueName = m_uniqueName;
			RPLStyleProps rPLStyleProps = null;
			if (m_styles != null)
			{
				rPLStyleProps = new RPLStyleProps();
				((StyleWriter)new StyleWriterOM(rPLStyleProps)).WriteAll(m_styles);
			}
			elemProps.NonSharedStyle = rPLStyleProps;
		}

		internal void WriteSharedStyles(StyleWriter writer, Style style)
		{
			Utility.WriteStyleProperty(writer, style, StyleAttributeNames.TextAlign);
			Utility.WriteStyleProperty(writer, style, StyleAttributeNames.LineHeight);
		}

		internal long WriteToStream(BinaryWriter writer, PageContext pageContext, bool hideDuplicates, TextBox.TextBoxOffset startPosition, TextBox.TextBoxOffset endPosition, List<Microsoft.ReportingServices.Rendering.RichText.TextRun> richTextParaRuns)
		{
			int num = 0;
			int num2 = 0;
			List<long> list = new List<long>();
			if (endPosition == null && startPosition == null)
			{
				foreach (TextRun textRun in m_textRuns)
				{
					num2 = num;
					num += textRun.SplitIndices.Count + 1;
					long item = textRun.WriteToStream(writer, pageContext, hideDuplicates, null, null, num2, richTextParaRuns);
					list.Add(item);
				}
			}
			else
			{
				int num3 = -1;
				if (startPosition != null)
				{
					num3 = startPosition.TextRunIndex;
				}
				int num4 = -1;
				if (endPosition != null)
				{
					num4 = endPosition.TextRunIndex;
				}
				long num5 = 0L;
				foreach (TextRun textRun2 in m_textRuns)
				{
					num2 = num;
					num += textRun2.SplitIndices.Count + 1;
					if (num3 >= num)
					{
						continue;
					}
					if (endPosition == null)
					{
						num5 = textRun2.WriteToStream(writer, pageContext, hideDuplicates, startPosition, null, num2, richTextParaRuns);
						list.Add(num5);
						continue;
					}
					if (num < num4)
					{
						num5 = textRun2.WriteToStream(writer, pageContext, hideDuplicates, startPosition, null, num2, richTextParaRuns);
						list.Add(num5);
						continue;
					}
					if (num == num4)
					{
						num5 = textRun2.WriteToStream(writer, pageContext, hideDuplicates, startPosition, null, num2, richTextParaRuns);
						list.Add(num5);
						if (endPosition.CharacterIndex > 0)
						{
							continue;
						}
						break;
					}
					num5 = textRun2.WriteToStream(writer, pageContext, hideDuplicates, startPosition, endPosition, num2, richTextParaRuns);
					list.Add(num5);
					break;
				}
			}
			long position = writer.BaseStream.Position;
			writer.Write((byte)19);
			if (m_source != null)
			{
				WriteElementProps(writer, pageContext);
			}
			else
			{
				writer.Write((byte)15);
				writer.Write((byte)0);
				writer.Write(byte.MaxValue);
				writer.Write((byte)1);
				writer.Write(byte.MaxValue);
				writer.Write(byte.MaxValue);
			}
			writer.Write(list.Count);
			foreach (long item2 in list)
			{
				writer.Write(item2);
			}
			writer.Write(byte.MaxValue);
			return position;
		}

		internal void WriteElementProps(BinaryWriter spbifWriter, PageContext pageContext)
		{
			StyleWriterStream styleWriterStream = new StyleWriterStream(spbifWriter);
			Hashtable itemPropsStart = pageContext.ItemPropsStart;
			long primitiveFromCache = pageContext.Common.GetPrimitiveFromCache<long>(m_source.ID, out itemPropsStart);
			if (primitiveFromCache <= 0)
			{
				primitiveFromCache = spbifWriter.BaseStream.Position;
				itemPropsStart[m_source.ID] = primitiveFromCache;
				spbifWriter.Write((byte)15);
				spbifWriter.Write((byte)0);
				spbifWriter.Write((byte)6);
				spbifWriter.Write((byte)0);
				WriteSharedStyles(styleWriterStream, m_source.Style);
				spbifWriter.Write(byte.MaxValue);
				styleWriterStream.WriteSharedProperty(8, m_source.ListLevel);
				if (m_source.ListStyle != null)
				{
					styleWriterStream.Write(7, StyleEnumConverter.Translate(m_source.ListStyle.Value));
				}
				styleWriterStream.WriteSharedProperty(9, m_source.LeftIndent);
				styleWriterStream.WriteSharedProperty(10, m_source.RightIndent);
				styleWriterStream.WriteSharedProperty(11, m_source.HangingIndent);
				styleWriterStream.WriteSharedProperty(12, m_source.SpaceBefore);
				styleWriterStream.WriteSharedProperty(13, m_source.SpaceAfter);
				styleWriterStream.WriteNotNull(5, m_source.ID);
				spbifWriter.Write(byte.MaxValue);
			}
			else
			{
				spbifWriter.Write((byte)15);
				spbifWriter.Write((byte)2);
				spbifWriter.Write(primitiveFromCache);
			}
			spbifWriter.Write((byte)1);
			Utility.WriteReportSize(spbifWriter, 9, m_leftIndent);
			Utility.WriteReportSize(spbifWriter, 10, m_rightIndent);
			Utility.WriteReportSize(spbifWriter, 11, m_hangingIndent);
			if (m_listStyle.HasValue)
			{
				styleWriterStream.Write(7, (byte)m_listStyle.Value);
			}
			if (m_listLevel.HasValue)
			{
				styleWriterStream.Write(8, m_listLevel.Value);
			}
			if (!m_firstLine)
			{
				styleWriterStream.Write(15, m_firstLine);
			}
			Utility.WriteReportSize(spbifWriter, 12, m_spaceBefore);
			Utility.WriteReportSize(spbifWriter, 13, m_spaceAfter);
			styleWriterStream.Write(14, m_paragraphNumber);
			spbifWriter.Write((byte)6);
			spbifWriter.Write((byte)1);
			WriteNonSharedStyles(styleWriterStream);
			spbifWriter.Write(byte.MaxValue);
			spbifWriter.Write((byte)4);
			spbifWriter.Write(m_uniqueName);
			spbifWriter.Write(byte.MaxValue);
			spbifWriter.Write(byte.MaxValue);
		}

		internal void WriteNonSharedStyles(StyleWriter writer)
		{
			if (m_styles != null)
			{
				writer.WriteAll(m_styles);
			}
		}

		public void Serialize(IntermediateFormatWriter writer)
		{
			writer.RegisterDeclaration(m_declaration);
			IScalabilityCache scalabilityCache = writer.PersistenceHelper as IScalabilityCache;
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.TextRuns:
					writer.Write(m_textRuns);
					break;
				case MemberName.Style:
					writer.WriteByteVariantHashtable(m_styles);
					break;
				case MemberName.ListStyle:
					writer.Write((byte?)m_listStyle);
					break;
				case MemberName.ListLevel:
					writer.Write(m_listLevel);
					break;
				case MemberName.ParagraphNumber:
					writer.Write(m_paragraphNumber);
					break;
				case MemberName.SpaceBefore:
					Utility.WriteReportSize(writer, m_spaceBefore);
					break;
				case MemberName.SpaceAfter:
					Utility.WriteReportSize(writer, m_spaceAfter);
					break;
				case MemberName.LeftIndent:
					Utility.WriteReportSize(writer, m_leftIndent);
					break;
				case MemberName.RightIndent:
					Utility.WriteReportSize(writer, m_rightIndent);
					break;
				case MemberName.HangingIndent:
					Utility.WriteReportSize(writer, m_hangingIndent);
					break;
				case MemberName.FirstLine:
					writer.Write(m_firstLine);
					break;
				case MemberName.UniqueName:
					writer.Write(m_uniqueName);
					break;
				case MemberName.Source:
					writer.Write(scalabilityCache.StoreStaticReference(m_source));
					break;
				default:
					RSTrace.RenderingTracer.Assert(condition: false, string.Empty);
					break;
				}
			}
		}

		public void Deserialize(IntermediateFormatReader reader)
		{
			reader.RegisterDeclaration(m_declaration);
			IScalabilityCache scalabilityCache = reader.PersistenceHelper as IScalabilityCache;
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.TextRuns:
					m_textRuns = reader.ReadGenericListOfRIFObjects<TextRun>();
					break;
				case MemberName.Style:
					m_styles = reader.ReadByteVariantHashtable<Dictionary<byte, object>>();
					break;
				case MemberName.ListStyle:
					m_listStyle = (RPLFormat.ListStyles?)reader.ReadNullable<byte>();
					break;
				case MemberName.ListLevel:
					m_listLevel = reader.ReadNullable<int>();
					break;
				case MemberName.ParagraphNumber:
					m_paragraphNumber = reader.ReadInt32();
					break;
				case MemberName.SpaceBefore:
					m_spaceBefore = Utility.ReadReportSize(reader);
					break;
				case MemberName.SpaceAfter:
					m_spaceAfter = Utility.ReadReportSize(reader);
					break;
				case MemberName.LeftIndent:
					m_leftIndent = Utility.ReadReportSize(reader);
					break;
				case MemberName.RightIndent:
					m_rightIndent = Utility.ReadReportSize(reader);
					break;
				case MemberName.HangingIndent:
					m_hangingIndent = Utility.ReadReportSize(reader);
					break;
				case MemberName.FirstLine:
					m_firstLine = reader.ReadBoolean();
					break;
				case MemberName.UniqueName:
					m_uniqueName = reader.ReadString();
					break;
				case MemberName.Source:
					m_source = (Microsoft.ReportingServices.OnDemandReportRendering.Paragraph)scalabilityCache.FetchStaticReference(reader.ReadInt32());
					break;
				default:
					RSTrace.RenderingTracer.Assert(condition: false, string.Empty);
					break;
				}
			}
		}

		public ObjectType GetObjectType()
		{
			return ObjectType.Paragraph;
		}

		public void ResolveReferences(Dictionary<ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
		}

		internal static Declaration GetDeclaration()
		{
			if (m_declaration == null)
			{
				List<MemberInfo> list = new List<MemberInfo>();
				list.Add(new MemberInfo(MemberName.TextRuns, ObjectType.RIFObjectList, ObjectType.TextRun));
				list.Add(new MemberInfo(MemberName.Style, ObjectType.ByteVariantHashtable, Token.Object));
				list.Add(new MemberInfo(MemberName.ListStyle, ObjectType.Nullable, Token.Byte));
				list.Add(new MemberInfo(MemberName.ListLevel, ObjectType.Nullable, Token.Int32));
				list.Add(new MemberInfo(MemberName.ParagraphNumber, Token.Int32));
				list.Add(new MemberInfo(MemberName.SpaceBefore, Token.String));
				list.Add(new MemberInfo(MemberName.SpaceAfter, Token.String));
				list.Add(new MemberInfo(MemberName.LeftIndent, Token.String));
				list.Add(new MemberInfo(MemberName.RightIndent, Token.String));
				list.Add(new MemberInfo(MemberName.HangingIndent, Token.String));
				list.Add(new MemberInfo(MemberName.UniqueName, Token.String));
				list.Add(new MemberInfo(MemberName.Source, Token.Int32));
				list.Add(new MemberInfo(MemberName.FirstLine, Token.Boolean));
				return new Declaration(ObjectType.Paragraph, ObjectType.None, list);
			}
			return m_declaration;
		}
	}
}
