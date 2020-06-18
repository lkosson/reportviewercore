using Microsoft.ReportingServices.Diagnostics.Utilities;
using Microsoft.ReportingServices.OnDemandProcessing.Scalability;
using Microsoft.ReportingServices.OnDemandReportRendering;
using Microsoft.ReportingServices.Rendering.RichText;
using Microsoft.ReportingServices.Rendering.RPLProcessing;
using Microsoft.ReportingServices.ReportIntermediateFormat.Persistence;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace Microsoft.ReportingServices.Rendering.HPBProcessing
{
	internal sealed class TextRun : IStorable, IPersistable, ITextRunProps
	{
		[StaticReference]
		private static Declaration m_declaration = GetDeclaration();

		[StaticReference]
		private Microsoft.ReportingServices.OnDemandReportRendering.TextRun m_source;

		private Dictionary<byte, object> m_styles;

		private string m_text;

		private string m_toolTip;

		private byte? m_markup;

		private List<string> m_hyperlinks;

		private string m_uniqueName;

		private List<int> m_splitIndices = new List<int>();

		private int m_startCharacterOffset;

		private string m_fontKey;

		public string Text
		{
			get
			{
				return m_text;
			}
			set
			{
				m_text = value;
			}
		}

		public string DefinitionText
		{
			get
			{
				if (m_source != null && m_source.Value != null)
				{
					if (m_source.FormattedValueExpressionBased || m_source.SharedTypeCode == TypeCode.String)
					{
						return m_source.Value.Value;
					}
					return m_source.Instance.Value;
				}
				return null;
			}
		}

		public List<int> SplitIndices => m_splitIndices;

		public string FontFamily
		{
			get
			{
				string text = null;
				if (m_source != null)
				{
					text = Utility.GetStringProp(20, StyleAttributeNames.FontFamily, m_source.Style, m_styles);
				}
				if (text == null)
				{
					text = "Arial";
				}
				return text;
			}
		}

		public float FontSize
		{
			get
			{
				if (m_source == null)
				{
					return 10f;
				}
				return (float)Utility.GetSizeProp(21, StyleAttributeNames.FontSize, 10f, m_source.Style, m_styles);
			}
		}

		public Color Color
		{
			get
			{
				if (m_source == null)
				{
					return Color.Black;
				}
				return Utility.GetColorProp(27, StyleAttributeNames.Color, Color.Black, m_source.Style, m_styles);
			}
		}

		public bool Bold
		{
			get
			{
				bool result = false;
				if (m_source != null)
				{
					byte? enumProp = Utility.GetEnumProp(22, StyleAttributeNames.FontWeight, m_source.Style, m_styles);
					if (enumProp.HasValue)
					{
						result = Utility.IsBold((RPLFormat.FontWeights)enumProp.Value);
					}
				}
				return result;
			}
		}

		public bool Italic
		{
			get
			{
				if (m_source == null)
				{
					return false;
				}
				byte? enumProp = Utility.GetEnumProp(19, StyleAttributeNames.FontStyle, m_source.Style, m_styles);
				if (enumProp.HasValue)
				{
					return enumProp.Value == 1;
				}
				return false;
			}
		}

		public RPLFormat.TextDecorations TextDecoration
		{
			get
			{
				if (m_source == null)
				{
					return RPLFormat.TextDecorations.None;
				}
				byte? enumProp = Utility.GetEnumProp(24, StyleAttributeNames.TextDecoration, m_source.Style, m_styles);
				if (!enumProp.HasValue)
				{
					return RPLFormat.TextDecorations.None;
				}
				return (RPLFormat.TextDecorations)enumProp.Value;
			}
		}

		public int IndexInParagraph => -1;

		public string FontKey
		{
			get
			{
				return m_fontKey;
			}
			set
			{
				m_fontKey = value;
			}
		}

		public int Size => Microsoft.ReportingServices.OnDemandProcessing.Scalability.ItemSizes.SizeOf(m_text) + Microsoft.ReportingServices.OnDemandProcessing.Scalability.ItemSizes.SizeOf(m_styles) + Microsoft.ReportingServices.OnDemandProcessing.Scalability.ItemSizes.SizeOf(m_toolTip) + Microsoft.ReportingServices.OnDemandProcessing.Scalability.ItemSizes.SizeOf(m_markup) + Microsoft.ReportingServices.OnDemandProcessing.Scalability.ItemSizes.SizeOf(m_hyperlinks) + Microsoft.ReportingServices.OnDemandProcessing.Scalability.ItemSizes.SizeOf(m_uniqueName) + Microsoft.ReportingServices.OnDemandProcessing.Scalability.ItemSizes.SizeOf(m_splitIndices) + Microsoft.ReportingServices.OnDemandProcessing.Scalability.ItemSizes.SizeOf(m_startCharacterOffset) + Microsoft.ReportingServices.OnDemandProcessing.Scalability.ItemSizes.SizeOf(m_fontKey) + Microsoft.ReportingServices.OnDemandProcessing.Scalability.ItemSizes.ReferenceSize;

		internal TextRun()
		{
		}

		internal TextRun(TextRunInstance instance, bool hideDuplicates)
		{
			m_source = instance.Definition;
			Utility.AddInstanceStyles(instance.Style, ref m_styles);
			if (instance.IsCompiled)
			{
				m_text = instance.Value;
				m_toolTip = instance.ToolTip;
			}
			else
			{
				m_text = ((m_source.Value != null && (m_source.FormattedValueExpressionBased || hideDuplicates)) ? instance.Value : null);
				m_toolTip = ((m_source.ToolTip != null && m_source.ToolTip.IsExpression) ? instance.ToolTip : null);
			}
			ActionInfo actionInfo = m_source.ActionInfo;
			if (instance.IsCompiled)
			{
				ActionInstance actionInstance = (instance as CompiledTextRunInstance).ActionInstance;
				if (actionInstance != null)
				{
					m_hyperlinks = new List<string>(1);
					ReportUrl hyperlink = actionInstance.Hyperlink;
					string item = null;
					if (hyperlink != null)
					{
						item = hyperlink.ToString();
					}
					m_hyperlinks.Add(item);
				}
			}
			else if (actionInfo != null)
			{
				ActionCollection actions = actionInfo.Actions;
				m_hyperlinks = new List<string>(actions.Count);
				foreach (Microsoft.ReportingServices.OnDemandReportRendering.Action item3 in actions)
				{
					ReportUrl hyperlink2 = item3.Instance.Hyperlink;
					string item2 = null;
					if (hyperlink2 != null)
					{
						item2 = hyperlink2.ToString();
					}
					m_hyperlinks.Add(item2);
				}
			}
			m_uniqueName = instance.UniqueName;
			if (m_source.MarkupType != null && m_source.MarkupType.IsExpression)
			{
				m_markup = StyleEnumConverter.Translate(instance.MarkupType);
			}
		}

		public void AddSplitIndex(int index)
		{
			m_splitIndices.Add(index);
		}

		public void ClearTo(int splitIndex)
		{
			int startCharacterOffset = m_startCharacterOffset;
			if (splitIndex < m_splitIndices.Count)
			{
				startCharacterOffset = SplitIndices[splitIndex];
				SplitIndices.RemoveRange(0, splitIndex + 1);
			}
			m_startCharacterOffset = startCharacterOffset;
		}

		internal Microsoft.ReportingServices.Rendering.RichText.TextRun GetRichTextRun()
		{
			string text = m_text;
			if (text == null && m_source != null && m_source.Value != null)
			{
				text = m_source.Value.Value;
			}
			return new Microsoft.ReportingServices.Rendering.RichText.TextRun(text, this);
		}

		public RPLTextRun GetRPLTextRun(PageContext pageContext, bool hideDuplicates, TextBox.TextBoxOffset startPosition, TextBox.TextBoxOffset endPosition, int previousCount, List<Microsoft.ReportingServices.Rendering.RichText.TextRun> richTextRuns)
		{
			RPLTextRun rPLTextRun = new RPLTextRun();
			RPLTextRunProps props = rPLTextRun.ElementProps as RPLTextRunProps;
			WriteElementProps(props, pageContext, hideDuplicates, startPosition, endPosition, previousCount, richTextRuns);
			return rPLTextRun;
		}

		internal string GetStringValue(string fullValue, TextBox.TextBoxOffset startPosition, TextBox.TextBoxOffset endPosition, int previousRunCount, List<Microsoft.ReportingServices.Rendering.RichText.TextRun> richTextRuns, out TextRunItemizedData glyphData)
		{
			glyphData = null;
			if (string.IsNullOrEmpty(fullValue))
			{
				return null;
			}
			int num = m_startCharacterOffset;
			int num2 = fullValue.Length;
			if (endPosition == null && startPosition == null)
			{
				if (m_startCharacterOffset == 0)
				{
					glyphData = CreateGlyphData(previousRunCount, num, num2, fullValue, richTextRuns);
					return fullValue;
				}
				glyphData = CreateGlyphData(previousRunCount, num, num2, fullValue, richTextRuns);
				return fullValue.Substring(m_startCharacterOffset);
			}
			if (startPosition != null)
			{
				int num3 = startPosition.TextRunIndex - previousRunCount;
				if (num3 == 0)
				{
					num += startPosition.CharacterIndex;
				}
				else if (num3 > 0)
				{
					num = m_splitIndices[num3 - 1];
					num += startPosition.CharacterIndex;
				}
			}
			if (endPosition != null)
			{
				int num4 = endPosition.TextRunIndex - previousRunCount;
				RSTrace.RenderingTracer.Assert(num4 >= 0, string.Empty);
				if (num4 == 0)
				{
					num2 = endPosition.CharacterIndex + m_startCharacterOffset;
				}
				else if (num4 <= m_splitIndices.Count)
				{
					num2 = m_splitIndices[num4 - 1];
					num2 += endPosition.CharacterIndex;
				}
				RSTrace.RenderingTracer.Assert(num2 <= fullValue.Length, string.Empty);
			}
			glyphData = CreateGlyphData(previousRunCount, num, num2, fullValue, richTextRuns);
			int length = num2 - num;
			return fullValue.Substring(num, length);
		}

		internal TextRunItemizedData GetGlyphValue(string fullValue, int previousRunCount, List<Microsoft.ReportingServices.Rendering.RichText.TextRun> richTextRuns)
		{
			if (string.IsNullOrEmpty(fullValue))
			{
				return null;
			}
			return CreateGlyphData(previousRunCount, m_startCharacterOffset, fullValue.Length, fullValue, richTextRuns);
		}

		private TextRunItemizedData CreateGlyphData(int startIndex, int start, int end, string fullValue, List<Microsoft.ReportingServices.Rendering.RichText.TextRun> richTextRuns)
		{
			if (richTextRuns == null || richTextRuns.Count == 0)
			{
				return null;
			}
			List<int> list = new List<int>();
			List<TexRunShapeData> list2 = new List<TexRunShapeData>();
			if (start == 0 && end == fullValue.Length)
			{
				for (int i = 0; i < m_splitIndices.Count; i++)
				{
					list2.Add(new TexRunShapeData(richTextRuns[startIndex], storeGlyph: true));
					startIndex++;
				}
				list2.Add(new TexRunShapeData(richTextRuns[startIndex], storeGlyph: true));
				return new TextRunItemizedData(m_splitIndices, list2);
			}
			int startIndex2 = start - m_startCharacterOffset;
			if (m_splitIndices.Count > 0)
			{
				bool flag = true;
				bool flag2 = false;
				for (int j = 0; j < m_splitIndices.Count; j++)
				{
					if (m_splitIndices[j] < end)
					{
						if (m_splitIndices[j] == start)
						{
							flag = false;
							startIndex++;
							continue;
						}
						if (m_splitIndices[j] < start)
						{
							startIndex++;
							flag = true;
							continue;
						}
						list.Add(m_splitIndices[j] - start);
						if (flag)
						{
							if (start == 0)
							{
								list2.Add(new TexRunShapeData(richTextRuns[startIndex], storeGlyph: true));
							}
							else
							{
								list2.Add(new TexRunShapeData(richTextRuns[startIndex], storeGlyph: false, startIndex2));
							}
							flag = false;
							startIndex2 = 0;
						}
						else
						{
							list2.Add(new TexRunShapeData(richTextRuns[startIndex], storeGlyph: true));
						}
						startIndex++;
						continue;
					}
					flag2 = true;
					if (m_splitIndices[j] > end)
					{
						list2.Add(new TexRunShapeData(richTextRuns[startIndex], storeGlyph: false, startIndex2));
					}
					else
					{
						list2.Add(new TexRunShapeData(richTextRuns[startIndex], storeGlyph: true));
					}
					break;
				}
				if (!flag2)
				{
					if (end < fullValue.Length)
					{
						list2.Add(new TexRunShapeData(richTextRuns[startIndex], storeGlyph: false, startIndex2));
					}
					else
					{
						list2.Add(new TexRunShapeData(richTextRuns[startIndex], storeGlyph: true));
					}
				}
			}
			else
			{
				list2.Add(new TexRunShapeData(richTextRuns[startIndex], storeGlyph: false, startIndex2));
			}
			if (list.Count > 0 || list2.Count > 0)
			{
				return new TextRunItemizedData(list, list2);
			}
			return null;
		}

		private void WriteElementProps(RPLTextRunProps props, PageContext pageContext, bool hideDuplicates, TextBox.TextBoxOffset startPosition, TextBox.TextBoxOffset endPosition, int previousRunCount, List<Microsoft.ReportingServices.Rendering.RichText.TextRun> richTextRuns)
		{
			Hashtable itemPropsStart = pageContext.ItemPropsStart;
			string text = m_source.ID;
			bool flag = true;
			ReportStringProperty value = m_source.Value;
			if ((m_startCharacterOffset > 0 || endPosition != null || startPosition != null) && value != null && !m_source.FormattedValueExpressionBased)
			{
				text += "_NV";
				flag = false;
			}
			RPLTextRunPropsDef rPLTextRunPropsDef = pageContext.Common.GetFromCache<RPLTextRunPropsDef>(text, out itemPropsStart);
			TextRunItemizedData glyphData = null;
			if (rPLTextRunPropsDef == null)
			{
				rPLTextRunPropsDef = new RPLTextRunPropsDef();
				rPLTextRunPropsDef.SharedStyle = new RPLStyleProps();
				WriteSharedStyles(new StyleWriterOM(rPLTextRunPropsDef.SharedStyle), m_source.Style);
				if (m_source.Label != null)
				{
					rPLTextRunPropsDef.Label = m_source.Label;
				}
				if (m_source.MarkupType != null && !m_source.MarkupType.IsExpression)
				{
					rPLTextRunPropsDef.Markup = (RPLFormat.MarkupStyles)StyleEnumConverter.Translate(m_source.MarkupType.Value);
				}
				if (m_source.ToolTip != null && !m_source.ToolTip.IsExpression)
				{
					rPLTextRunPropsDef.ToolTip = m_source.ToolTip.Value;
				}
				if (flag && value != null && !m_source.FormattedValueExpressionBased && !hideDuplicates)
				{
					if (m_source.SharedTypeCode == TypeCode.String)
					{
						rPLTextRunPropsDef.Value = GetStringValue(value.Value, null, null, previousRunCount, richTextRuns, out glyphData);
					}
					else
					{
						rPLTextRunPropsDef.Value = GetStringValue(m_source.Instance.Value, null, null, previousRunCount, richTextRuns, out glyphData);
					}
				}
				rPLTextRunPropsDef.ID = text;
				itemPropsStart[text] = rPLTextRunPropsDef;
			}
			else if (richTextRuns != null && flag && !hideDuplicates && value != null && !m_source.FormattedValueExpressionBased)
			{
				glyphData = GetGlyphValue(value.Value, previousRunCount, richTextRuns);
			}
			props.Definition = rPLTextRunPropsDef;
			props.UniqueName = m_uniqueName;
			if (m_markup.HasValue)
			{
				props.Markup = (RPLFormat.MarkupStyles)m_markup.Value;
			}
			props.ToolTip = m_toolTip;
			TextRunItemizedData glyphData2 = null;
			if (!flag)
			{
				if (value != null && !hideDuplicates)
				{
					if (m_source.SharedTypeCode == TypeCode.String)
					{
						props.Value = GetStringValue(value.Value, startPosition, endPosition, previousRunCount, richTextRuns, out glyphData2);
					}
					else
					{
						props.Value = GetStringValue(m_source.Instance.Value, startPosition, endPosition, previousRunCount, richTextRuns, out glyphData2);
					}
				}
			}
			else
			{
				props.Value = GetStringValue(m_text, startPosition, endPosition, previousRunCount, richTextRuns, out glyphData2);
			}
			if (glyphData2 == null)
			{
				glyphData2 = glyphData;
			}
			pageContext.RegisterTextRunData(glyphData2);
			if (m_hyperlinks != null)
			{
				int count = m_hyperlinks.Count;
				props.ActionInfo = new RPLActionInfo(count);
				for (int i = 0; i < count; i++)
				{
					string text2 = m_hyperlinks[i];
					RPLAction rPLAction = new RPLAction();
					if (text2 != null)
					{
						rPLAction.Hyperlink = text2;
					}
					props.ActionInfo.Actions[i] = rPLAction;
				}
			}
			RPLStyleProps rPLStyleProps = null;
			if (m_styles != null)
			{
				rPLStyleProps = new RPLStyleProps();
				new StyleWriterOM(rPLStyleProps).WriteAll(m_styles);
			}
			props.NonSharedStyle = rPLStyleProps;
		}

		internal long WriteToStream(BinaryWriter writer, PageContext pageContext, bool hideDuplicates, TextBox.TextBoxOffset startPosition, TextBox.TextBoxOffset endPosition, int previousRunCount, List<Microsoft.ReportingServices.Rendering.RichText.TextRun> richTextRuns)
		{
			long position = writer.BaseStream.Position;
			writer.Write((byte)20);
			WriteElementProps(writer, pageContext, hideDuplicates, startPosition, endPosition, previousRunCount, richTextRuns);
			writer.Write(byte.MaxValue);
			return position;
		}

		private void WriteElementProps(BinaryWriter spbifWriter, PageContext pageContext, bool hideDuplicates, TextBox.TextBoxOffset startPosition, TextBox.TextBoxOffset endPosition, int previousRunCount, List<Microsoft.ReportingServices.Rendering.RichText.TextRun> richTextRuns)
		{
			StyleWriterStream styleWriterStream = new StyleWriterStream(spbifWriter);
			string text = m_source.ID;
			bool flag = true;
			ReportStringProperty value = m_source.Value;
			if ((m_startCharacterOffset > 0 || endPosition != null || startPosition != null) && value != null && !m_source.FormattedValueExpressionBased)
			{
				text += "_NV";
				flag = false;
			}
			TextRunItemizedData glyphData = null;
			string value2 = null;
			Hashtable itemPropsStart = pageContext.ItemPropsStart;
			long primitiveFromCache = pageContext.Common.GetPrimitiveFromCache<long>(text, out itemPropsStart);
			if (primitiveFromCache <= 0)
			{
				primitiveFromCache = spbifWriter.BaseStream.Position;
				itemPropsStart[text] = primitiveFromCache;
				spbifWriter.Write((byte)15);
				spbifWriter.Write((byte)0);
				spbifWriter.Write((byte)6);
				spbifWriter.Write((byte)0);
				WriteSharedStyles(styleWriterStream, m_source.Style);
				spbifWriter.Write(byte.MaxValue);
				styleWriterStream.WriteNotNull(5, text);
				styleWriterStream.WriteNotNull(8, m_source.Label);
				if (m_source.MarkupType != null)
				{
					styleWriterStream.Write(7, StyleEnumConverter.Translate(m_source.MarkupType.Value));
				}
				styleWriterStream.WriteSharedProperty(9, m_source.ToolTip);
				if (flag && !hideDuplicates && value != null && !m_source.FormattedValueExpressionBased)
				{
					if (m_source.SharedTypeCode == TypeCode.String)
					{
						styleWriterStream.WriteNotNull(10, GetStringValue(value.Value, null, null, previousRunCount, richTextRuns, out glyphData));
					}
					else
					{
						styleWriterStream.WriteNotNull(10, GetStringValue(m_source.Instance.Value, null, null, previousRunCount, richTextRuns, out glyphData));
					}
				}
				spbifWriter.Write(byte.MaxValue);
			}
			else
			{
				spbifWriter.Write((byte)15);
				spbifWriter.Write((byte)2);
				spbifWriter.Write(primitiveFromCache);
				if (richTextRuns != null && flag && !hideDuplicates && value != null && !m_source.FormattedValueExpressionBased)
				{
					glyphData = GetGlyphValue(value.Value, previousRunCount, richTextRuns);
				}
			}
			spbifWriter.Write((byte)1);
			TextRunItemizedData glyphData2 = null;
			if (!flag)
			{
				if (!hideDuplicates && value != null)
				{
					value2 = ((m_source.SharedTypeCode != TypeCode.String) ? GetStringValue(m_source.Instance.Value, startPosition, endPosition, previousRunCount, richTextRuns, out glyphData2) : GetStringValue(value.Value, startPosition, endPosition, previousRunCount, richTextRuns, out glyphData2));
				}
			}
			else
			{
				value2 = GetStringValue(m_text, startPosition, endPosition, previousRunCount, richTextRuns, out glyphData2);
			}
			if (glyphData2 == null)
			{
				glyphData2 = glyphData;
			}
			pageContext.RegisterTextRunData(glyphData2);
			styleWriterStream.WriteNotNull(10, value2);
			styleWriterStream.WriteNotNull(9, m_toolTip);
			styleWriterStream.WriteNotNull(4, m_uniqueName);
			styleWriterStream.WriteNotNull(7, m_markup);
			spbifWriter.Write((byte)6);
			spbifWriter.Write((byte)1);
			if (m_styles != null)
			{
				styleWriterStream.WriteAll(m_styles);
			}
			spbifWriter.Write(byte.MaxValue);
			WriteActions(spbifWriter);
			spbifWriter.Write(byte.MaxValue);
			spbifWriter.Write(byte.MaxValue);
		}

		public void WriteNonSharedStyles(StyleWriter writer)
		{
			if (m_styles != null)
			{
				writer.WriteAll(m_styles);
			}
		}

		internal void WriteActions(BinaryWriter spbifWriter)
		{
			if (m_hyperlinks == null)
			{
				return;
			}
			spbifWriter.Write((byte)11);
			spbifWriter.Write((byte)2);
			spbifWriter.Write(m_hyperlinks.Count);
			foreach (string hyperlink in m_hyperlinks)
			{
				spbifWriter.Write((byte)3);
				if (hyperlink != null)
				{
					spbifWriter.Write((byte)6);
					spbifWriter.Write(hyperlink);
				}
				spbifWriter.Write(byte.MaxValue);
			}
			spbifWriter.Write(byte.MaxValue);
		}

		internal void WriteSharedStyles(StyleWriter writer, Style style)
		{
			Utility.WriteStyleProperty(writer, style, StyleAttributeNames.FontFamily);
			Utility.WriteStyleProperty(writer, style, StyleAttributeNames.FontSize);
			Utility.WriteStyleProperty(writer, style, StyleAttributeNames.FontStyle);
			Utility.WriteStyleProperty(writer, style, StyleAttributeNames.FontWeight);
			Utility.WriteStyleProperty(writer, style, StyleAttributeNames.TextDecoration);
			Utility.WriteStyleProperty(writer, style, StyleAttributeNames.Color);
			Utility.WriteStyleProperty(writer, style, StyleAttributeNames.Direction);
			Utility.WriteStyleProperty(writer, style, StyleAttributeNames.Calendar);
			Utility.WriteStyleProperty(writer, style, StyleAttributeNames.NumeralLanguage);
			Utility.WriteStyleProperty(writer, style, StyleAttributeNames.NumeralVariant);
		}

		public void Serialize(IntermediateFormatWriter writer)
		{
			writer.RegisterDeclaration(m_declaration);
			IScalabilityCache scalabilityCache = writer.PersistenceHelper as IScalabilityCache;
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.Text:
					writer.Write(m_text);
					break;
				case MemberName.Style:
					writer.WriteByteVariantHashtable(m_styles);
					break;
				case MemberName.ToolTip:
					writer.Write(m_toolTip);
					break;
				case MemberName.MarkupType:
					writer.Write(m_markup);
					break;
				case MemberName.Actions:
					writer.WriteListOfPrimitives(m_hyperlinks);
					break;
				case MemberName.UniqueName:
					writer.Write(m_uniqueName);
					break;
				case MemberName.Source:
					writer.Write(scalabilityCache.StoreStaticReference(m_source));
					break;
				case MemberName.Indexes:
					writer.WriteListOfPrimitives(m_splitIndices);
					break;
				case MemberName.Offset:
					writer.Write(m_startCharacterOffset);
					break;
				case MemberName.Key:
					writer.Write(m_fontKey);
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
				case MemberName.Text:
					m_text = reader.ReadString();
					break;
				case MemberName.Style:
					m_styles = reader.ReadByteVariantHashtable<Dictionary<byte, object>>();
					break;
				case MemberName.ToolTip:
					m_toolTip = reader.ReadString();
					break;
				case MemberName.MarkupType:
					m_markup = (reader.ReadVariant() as byte?);
					break;
				case MemberName.Actions:
					m_hyperlinks = reader.ReadListOfPrimitives<string>();
					break;
				case MemberName.UniqueName:
					m_uniqueName = reader.ReadString();
					break;
				case MemberName.Source:
					m_source = (Microsoft.ReportingServices.OnDemandReportRendering.TextRun)scalabilityCache.FetchStaticReference(reader.ReadInt32());
					break;
				case MemberName.Indexes:
					m_splitIndices = reader.ReadListOfPrimitives<int>();
					break;
				case MemberName.Offset:
					m_startCharacterOffset = reader.ReadInt32();
					break;
				case MemberName.Key:
					m_fontKey = reader.ReadString();
					break;
				default:
					RSTrace.RenderingTracer.Assert(condition: false, string.Empty);
					break;
				}
			}
		}

		public ObjectType GetObjectType()
		{
			return ObjectType.TextRun;
		}

		public void ResolveReferences(Dictionary<ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
		}

		internal static Declaration GetDeclaration()
		{
			if (m_declaration == null)
			{
				List<MemberInfo> list = new List<MemberInfo>();
				list.Add(new MemberInfo(MemberName.Text, Token.String));
				list.Add(new MemberInfo(MemberName.Style, ObjectType.ByteVariantHashtable, Token.Object));
				list.Add(new MemberInfo(MemberName.ToolTip, Token.String));
				list.Add(new MemberInfo(MemberName.MarkupType, ObjectType.RIFObject));
				list.Add(new MemberInfo(MemberName.Actions, ObjectType.PrimitiveList, Token.String));
				list.Add(new MemberInfo(MemberName.UniqueName, Token.String));
				list.Add(new MemberInfo(MemberName.Source, Token.Int32));
				list.Add(new MemberInfo(MemberName.Indexes, ObjectType.PrimitiveList, Token.Int32));
				list.Add(new MemberInfo(MemberName.Offset, Token.Int32));
				list.Add(new MemberInfo(MemberName.Key, Token.String));
				return new Declaration(ObjectType.TextRun, ObjectType.None, list);
			}
			return m_declaration;
		}
	}
}
