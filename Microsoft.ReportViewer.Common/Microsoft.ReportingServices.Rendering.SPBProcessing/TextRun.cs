using Microsoft.ReportingServices.Diagnostics.Utilities;
using Microsoft.ReportingServices.OnDemandReportRendering;
using Microsoft.ReportingServices.Rendering.RichText;
using Microsoft.ReportingServices.Rendering.RPLProcessing;
using System;
using System.Collections;
using System.Drawing;
using System.IO;

namespace Microsoft.ReportingServices.Rendering.SPBProcessing
{
	internal sealed class TextRun : PageElement, ITextRunProps
	{
		private long m_offset;

		private RPLTextRun m_rplElement;

		private CompiledTextRunInstance m_compiledSource;

		private string m_fontKey;

		public override long Offset
		{
			get
			{
				return m_offset;
			}
			set
			{
				m_offset = value;
			}
		}

		internal override string SourceID => m_source.ID;

		internal override string SourceUniqueName => m_source.InstanceUniqueName;

		internal override bool HasBackground => false;

		internal RPLTextRun RPLElement => m_rplElement;

		internal CompiledTextRunInstance CompiledInstance => m_compiledSource;

		public string FontFamily
		{
			get
			{
				string text = (string)GetRichTextStyleValue(StyleAttributeNames.FontFamily, m_compiledSource);
				if (string.IsNullOrEmpty(text))
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
				double num = 12.0;
				ReportSize reportSize = (ReportSize)GetRichTextStyleValue(StyleAttributeNames.FontSize, m_compiledSource);
				if (reportSize != null)
				{
					num = reportSize.ToPoints();
				}
				return (float)num;
			}
		}

		public Color Color
		{
			get
			{
				Color result = Color.Black;
				ReportColor reportColor = (ReportColor)GetRichTextStyleValue(StyleAttributeNames.Color, m_compiledSource);
				if (reportColor != null)
				{
					result = reportColor.ToColor();
				}
				return result;
			}
		}

		public bool Bold
		{
			get
			{
				FontWeights aValue = (FontWeights)GetRichTextStyleValue(StyleAttributeNames.FontWeight, m_compiledSource);
				return IsBold((RPLFormat.FontWeights)StyleEnumConverter.Translate(aValue));
			}
		}

		public bool Italic
		{
			get
			{
				FontStyles aValue = (FontStyles)GetRichTextStyleValue(StyleAttributeNames.FontStyle, m_compiledSource);
				return IsItalic((RPLFormat.FontStyles)StyleEnumConverter.Translate(aValue));
			}
		}

		public RPLFormat.TextDecorations TextDecoration => (RPLFormat.TextDecorations)StyleEnumConverter.Translate((TextDecorations)GetRichTextStyleValue(StyleAttributeNames.TextDecoration, m_compiledSource));

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

		private TextRun()
			: base(null)
		{
		}

		internal TextRun(Microsoft.ReportingServices.OnDemandReportRendering.TextRun textRun, PageContext pageContext)
			: this(textRun, null, pageContext)
		{
		}

		internal TextRun(Microsoft.ReportingServices.OnDemandReportRendering.TextRun textRun, CompiledTextRunInstance compiledTextRun, PageContext pageContext)
			: base(textRun)
		{
			m_compiledSource = compiledTextRun;
		}

		public void AddSplitIndex(int index)
		{
		}

		internal object GetRichTextStyleValue(StyleAttributeNames styleName, ref bool isShared)
		{
			return GetRichTextStyleValue(styleName, m_compiledSource, ref isShared);
		}

		internal object GetRichTextStyleValue(StyleAttributeNames styleName)
		{
			return GetRichTextStyleValue(styleName, m_compiledSource);
		}

		internal string ComputeValue()
		{
			if (m_compiledSource == null)
			{
				Microsoft.ReportingServices.OnDemandReportRendering.TextRun textRun = m_source as Microsoft.ReportingServices.OnDemandReportRendering.TextRun;
				if (textRun.FormattedValueExpressionBased)
				{
					return textRun.Instance.Value;
				}
				return textRun.Value.Value;
			}
			return m_compiledSource.Value;
		}

		internal override void WriteSharedItemProps(BinaryWriter spbifWriter, RPLWriter rplWriter, PageContext pageContext, long offset)
		{
			Microsoft.ReportingServices.OnDemandReportRendering.TextRun textRun = m_source as Microsoft.ReportingServices.OnDemandReportRendering.TextRun;
			RSTrace.RenderingTracer.Assert(textRun != null, "The text run definition cannot be null");
			Hashtable hashtable = pageContext.ItemPropsStart;
			if (hashtable != null)
			{
				object obj = hashtable[SourceID];
				if (obj != null)
				{
					spbifWriter.Write((byte)2);
					spbifWriter.Write((long)obj);
					return;
				}
			}
			if (hashtable == null)
			{
				hashtable = (pageContext.ItemPropsStart = new Hashtable());
			}
			hashtable.Add(SourceID, offset);
			spbifWriter.Write((byte)0);
			spbifWriter.Write((byte)5);
			spbifWriter.Write(SourceID);
			if (!textRun.MarkupType.IsExpression)
			{
				spbifWriter.Write((byte)7);
				spbifWriter.Write(StyleEnumConverter.Translate(textRun.MarkupType.Value));
			}
			if (textRun.Label != null)
			{
				spbifWriter.Write((byte)8);
				spbifWriter.Write(textRun.Label);
			}
			if (textRun.ToolTip != null && !textRun.ToolTip.IsExpression && textRun.ToolTip.Value != null)
			{
				spbifWriter.Write((byte)9);
				spbifWriter.Write(textRun.ToolTip.Value);
			}
			if (textRun.Value.IsExpression)
			{
				spbifWriter.Write((byte)12);
				spbifWriter.Write(textRun.Value.ExpressionString);
			}
			if (!pageContext.HideDuplicates && !textRun.FormattedValueExpressionBased && textRun.Value.Value != null)
			{
				spbifWriter.Write((byte)10);
				if (textRun.SharedTypeCode == TypeCode.String)
				{
					spbifWriter.Write(textRun.Value.Value);
				}
				else
				{
					spbifWriter.Write(textRun.Instance.Value);
				}
			}
			WriteSharedStyle(spbifWriter, null, pageContext, 6);
			spbifWriter.Write(byte.MaxValue);
		}

		internal override void WriteSharedItemProps(RPLElementProps elemProps, RPLWriter rplWriter, PageContext pageContext)
		{
			Microsoft.ReportingServices.OnDemandReportRendering.TextRun textRun = m_source as Microsoft.ReportingServices.OnDemandReportRendering.TextRun;
			RSTrace.RenderingTracer.Assert(textRun != null, "The text run definition cannot be null");
			Hashtable hashtable = pageContext.ItemPropsStart;
			if (hashtable != null)
			{
				object obj = hashtable[SourceID];
				if (obj != null)
				{
					elemProps.Definition = (RPLTextRunPropsDef)obj;
					return;
				}
			}
			RPLTextRunPropsDef rPLTextRunPropsDef = (elemProps as RPLTextRunProps).Definition as RPLTextRunPropsDef;
			if (hashtable == null)
			{
				hashtable = (pageContext.ItemPropsStart = new Hashtable());
			}
			hashtable.Add(SourceID, rPLTextRunPropsDef);
			rPLTextRunPropsDef.ID = SourceID;
			if (!textRun.MarkupType.IsExpression)
			{
				rPLTextRunPropsDef.Markup = (RPLFormat.MarkupStyles)StyleEnumConverter.Translate(textRun.MarkupType.Value);
			}
			if (textRun.Label != null)
			{
				rPLTextRunPropsDef.Label = textRun.Label;
			}
			if (textRun.ToolTip != null && !textRun.ToolTip.IsExpression && textRun.ToolTip.Value != null)
			{
				rPLTextRunPropsDef.ToolTip = textRun.ToolTip.Value;
			}
			if (textRun.Value.IsExpression)
			{
				rPLTextRunPropsDef.Formula = textRun.Value.ExpressionString;
			}
			if (!pageContext.HideDuplicates && !textRun.FormattedValueExpressionBased && textRun.Value.Value != null)
			{
				if (textRun.SharedTypeCode == TypeCode.String)
				{
					rPLTextRunPropsDef.Value = textRun.Value.Value;
				}
				else
				{
					rPLTextRunPropsDef.Value = textRun.Instance.Value;
				}
			}
			rPLTextRunPropsDef.SharedStyle = WriteSharedStyle(null, pageContext);
		}

		internal override void WriteNonSharedItemProps(BinaryWriter spbifWriter, RPLWriter rplWriter, PageContext pageContext)
		{
			Microsoft.ReportingServices.OnDemandReportRendering.TextRun textRun = m_source as Microsoft.ReportingServices.OnDemandReportRendering.TextRun;
			RSTrace.RenderingTracer.Assert(textRun != null, "The text run definition cannot be null");
			TextRunInstance textRunInstance = null;
			bool flag = false;
			if (m_compiledSource != null)
			{
				textRunInstance = m_compiledSource;
				flag = true;
			}
			else
			{
				textRunInstance = textRun.Instance;
				RSTrace.RenderingTracer.Assert(textRunInstance != null, "The text run instance cannot be null");
			}
			spbifWriter.Write((byte)1);
			spbifWriter.Write((byte)4);
			spbifWriter.Write(textRunInstance.UniqueName);
			if (!flag)
			{
				if (textRunInstance.ProcessedWithError)
				{
					spbifWriter.Write((byte)13);
					spbifWriter.Write(value: true);
				}
				if (textRun.MarkupType.IsExpression)
				{
					spbifWriter.Write((byte)7);
					spbifWriter.Write(StyleEnumConverter.Translate(textRunInstance.MarkupType));
				}
				if (textRun.ToolTip != null && textRun.ToolTip.IsExpression && textRunInstance.ToolTip != null)
				{
					spbifWriter.Write((byte)9);
					spbifWriter.Write(textRunInstance.ToolTip);
				}
				WriteActionInfo(textRun.ActionInfo, spbifWriter, pageContext, 11);
			}
			else
			{
				spbifWriter.Write((byte)7);
				spbifWriter.Write(StyleEnumConverter.Translate(textRunInstance.MarkupType));
				if (textRunInstance.ToolTip != null)
				{
					spbifWriter.Write((byte)9);
					spbifWriter.Write(textRunInstance.ToolTip);
				}
				if (m_compiledSource.ActionInstance != null)
				{
					WriteActionInstance(m_compiledSource.ActionInstance, spbifWriter, pageContext);
				}
			}
			if ((pageContext.HideDuplicates || textRun.FormattedValueExpressionBased) && textRunInstance.Value != null)
			{
				spbifWriter.Write((byte)10);
				spbifWriter.Write(textRunInstance.Value);
			}
			pageContext.HideDuplicates = false;
			pageContext.TypeCodeNonString = false;
			WriteNonSharedStyle(spbifWriter, null, null, pageContext, 6, m_compiledSource);
			spbifWriter.Write(byte.MaxValue);
		}

		internal override void WriteNonSharedItemProps(RPLElementProps elemProps, RPLWriter rplWriter, PageContext pageContext)
		{
			Microsoft.ReportingServices.OnDemandReportRendering.TextRun textRun = m_source as Microsoft.ReportingServices.OnDemandReportRendering.TextRun;
			RSTrace.RenderingTracer.Assert(textRun != null, "The text run definition cannot be null");
			TextRunInstance textRunInstance = null;
			bool flag = false;
			if (m_compiledSource != null)
			{
				textRunInstance = m_compiledSource;
				flag = true;
			}
			else
			{
				textRunInstance = textRun.Instance;
				RSTrace.RenderingTracer.Assert(textRunInstance != null, "The text run instance cannot be null");
			}
			elemProps.UniqueName = textRunInstance.UniqueName;
			RPLTextRunProps rPLTextRunProps = elemProps as RPLTextRunProps;
			if (!flag)
			{
				rPLTextRunProps.ProcessedWithError = textRunInstance.ProcessedWithError;
				if (textRun.MarkupType.IsExpression)
				{
					rPLTextRunProps.Markup = (RPLFormat.MarkupStyles)StyleEnumConverter.Translate(textRunInstance.MarkupType);
				}
				if (textRun.ToolTip != null && textRun.ToolTip.IsExpression && textRunInstance.ToolTip != null)
				{
					rPLTextRunProps.ToolTip = textRunInstance.ToolTip;
				}
				rPLTextRunProps.ActionInfo = WriteActionInfo(textRun.ActionInfo, pageContext);
			}
			else
			{
				rPLTextRunProps.Markup = (RPLFormat.MarkupStyles)StyleEnumConverter.Translate(textRunInstance.MarkupType);
				if (textRunInstance.ToolTip != null)
				{
					rPLTextRunProps.ToolTip = textRunInstance.ToolTip;
				}
				if (m_compiledSource.ActionInstance != null)
				{
					rPLTextRunProps.ActionInfo = WriteActionInstance(m_compiledSource.ActionInstance, pageContext);
				}
			}
			if ((pageContext.HideDuplicates || textRun.FormattedValueExpressionBased) && textRunInstance.Value != null)
			{
				rPLTextRunProps.Value = textRunInstance.Value;
			}
			pageContext.HideDuplicates = false;
			pageContext.TypeCodeNonString = false;
			rPLTextRunProps.NonSharedStyle = WriteNonSharedStyle(null, null, pageContext, m_compiledSource);
		}

		internal override void WriteItemSharedStyleProps(BinaryWriter spbifWriter, Style style, PageContext pageContext)
		{
			WriteStyleProp(style, spbifWriter, StyleAttributeNames.FontStyle, 19);
			WriteStyleProp(style, spbifWriter, StyleAttributeNames.FontFamily, 20);
			WriteStyleProp(style, spbifWriter, StyleAttributeNames.FontSize, 21);
			WriteStyleProp(style, spbifWriter, StyleAttributeNames.FontWeight, 22);
			WriteStyleProp(style, spbifWriter, StyleAttributeNames.Format, 23);
			WriteStyleProp(style, spbifWriter, StyleAttributeNames.TextDecoration, 24);
			WriteStyleProp(style, spbifWriter, StyleAttributeNames.Color, 27);
			WriteStyleProp(style, spbifWriter, StyleAttributeNames.Language, 32);
			WriteStyleProp(style, spbifWriter, StyleAttributeNames.Calendar, 38);
			WriteStyleProp(style, spbifWriter, StyleAttributeNames.NumeralLanguage, 36);
			WriteStyleProp(style, spbifWriter, StyleAttributeNames.NumeralVariant, 37);
		}

		internal override void WriteItemSharedStyleProps(RPLStyleProps rplStyleProps, Style style, PageContext pageContext)
		{
			WriteStyleProp(style, rplStyleProps, StyleAttributeNames.FontStyle, 19);
			WriteStyleProp(style, rplStyleProps, StyleAttributeNames.FontFamily, 20);
			WriteStyleProp(style, rplStyleProps, StyleAttributeNames.FontSize, 21);
			WriteStyleProp(style, rplStyleProps, StyleAttributeNames.FontWeight, 22);
			WriteStyleProp(style, rplStyleProps, StyleAttributeNames.Format, 23);
			WriteStyleProp(style, rplStyleProps, StyleAttributeNames.TextDecoration, 24);
			WriteStyleProp(style, rplStyleProps, StyleAttributeNames.Color, 27);
			WriteStyleProp(style, rplStyleProps, StyleAttributeNames.Language, 32);
			WriteStyleProp(style, rplStyleProps, StyleAttributeNames.Calendar, 38);
			WriteStyleProp(style, rplStyleProps, StyleAttributeNames.NumeralLanguage, 36);
			WriteStyleProp(style, rplStyleProps, StyleAttributeNames.NumeralVariant, 37);
		}

		internal override void WriteNonSharedStyleProp(BinaryWriter spbifWriter, Style styleDef, StyleInstance style, StyleAttributeNames styleAttribute, PageContext pageContext)
		{
			switch (styleAttribute)
			{
			case StyleAttributeNames.TextAlign:
			case StyleAttributeNames.VerticalAlign:
			case StyleAttributeNames.PaddingLeft:
			case StyleAttributeNames.PaddingRight:
			case StyleAttributeNames.PaddingTop:
			case StyleAttributeNames.PaddingBottom:
			case StyleAttributeNames.LineHeight:
			case StyleAttributeNames.Direction:
			case StyleAttributeNames.WritingMode:
			case StyleAttributeNames.UnicodeBiDi:
				break;
			case StyleAttributeNames.FontStyle:
				WriteStyleProp(styleDef, style, spbifWriter, StyleAttributeNames.FontStyle, 19);
				break;
			case StyleAttributeNames.FontFamily:
				WriteStyleProp(styleDef, style, spbifWriter, StyleAttributeNames.FontFamily, 20);
				break;
			case StyleAttributeNames.FontSize:
				WriteStyleProp(styleDef, style, spbifWriter, StyleAttributeNames.FontSize, 21);
				break;
			case StyleAttributeNames.FontWeight:
				WriteStyleProp(styleDef, style, spbifWriter, StyleAttributeNames.FontWeight, 22);
				break;
			case StyleAttributeNames.Format:
				WriteStyleProp(styleDef, style, spbifWriter, StyleAttributeNames.Format, 23);
				break;
			case StyleAttributeNames.TextDecoration:
				WriteStyleProp(styleDef, style, spbifWriter, StyleAttributeNames.TextDecoration, 24);
				break;
			case StyleAttributeNames.Color:
				WriteStyleProp(styleDef, style, spbifWriter, StyleAttributeNames.Color, 27);
				break;
			case StyleAttributeNames.Language:
				WriteStyleProp(styleDef, style, spbifWriter, StyleAttributeNames.Language, 32);
				break;
			case StyleAttributeNames.Calendar:
				WriteStyleProp(styleDef, style, spbifWriter, StyleAttributeNames.Calendar, 38);
				break;
			case StyleAttributeNames.NumeralLanguage:
				WriteStyleProp(styleDef, style, spbifWriter, StyleAttributeNames.NumeralLanguage, 36);
				break;
			case StyleAttributeNames.NumeralVariant:
				WriteStyleProp(styleDef, style, spbifWriter, StyleAttributeNames.NumeralVariant, 37);
				break;
			}
		}

		internal override void WriteNonSharedStyleProp(RPLStyleProps rplStyleProps, Style styleDef, StyleInstance style, StyleAttributeNames styleAttribute, PageContext pageContext)
		{
			switch (styleAttribute)
			{
			case StyleAttributeNames.TextAlign:
			case StyleAttributeNames.VerticalAlign:
			case StyleAttributeNames.PaddingLeft:
			case StyleAttributeNames.PaddingRight:
			case StyleAttributeNames.PaddingTop:
			case StyleAttributeNames.PaddingBottom:
			case StyleAttributeNames.LineHeight:
			case StyleAttributeNames.Direction:
			case StyleAttributeNames.WritingMode:
			case StyleAttributeNames.UnicodeBiDi:
				break;
			case StyleAttributeNames.FontStyle:
				WriteStyleProp(styleDef, style, rplStyleProps, StyleAttributeNames.FontStyle, 19);
				break;
			case StyleAttributeNames.FontFamily:
				WriteStyleProp(styleDef, style, rplStyleProps, StyleAttributeNames.FontFamily, 20);
				break;
			case StyleAttributeNames.FontSize:
				WriteStyleProp(styleDef, style, rplStyleProps, StyleAttributeNames.FontSize, 21);
				break;
			case StyleAttributeNames.FontWeight:
				WriteStyleProp(styleDef, style, rplStyleProps, StyleAttributeNames.FontWeight, 22);
				break;
			case StyleAttributeNames.Format:
				WriteStyleProp(styleDef, style, rplStyleProps, StyleAttributeNames.Format, 23);
				break;
			case StyleAttributeNames.TextDecoration:
				WriteStyleProp(styleDef, style, rplStyleProps, StyleAttributeNames.TextDecoration, 24);
				break;
			case StyleAttributeNames.Color:
				WriteStyleProp(styleDef, style, rplStyleProps, StyleAttributeNames.Color, 27);
				break;
			case StyleAttributeNames.Language:
				WriteStyleProp(styleDef, style, rplStyleProps, StyleAttributeNames.Language, 32);
				break;
			case StyleAttributeNames.Calendar:
				WriteStyleProp(styleDef, style, rplStyleProps, StyleAttributeNames.Calendar, 38);
				break;
			case StyleAttributeNames.NumeralLanguage:
				WriteStyleProp(styleDef, style, rplStyleProps, StyleAttributeNames.NumeralLanguage, 36);
				break;
			case StyleAttributeNames.NumeralVariant:
				WriteStyleProp(styleDef, style, rplStyleProps, StyleAttributeNames.NumeralVariant, 37);
				break;
			}
		}

		internal override void WriteBackgroundImage(BinaryWriter spbifWriter, Style style, bool writeShared, PageContext pageContext)
		{
		}

		internal override void WriteBackgroundImage(RPLStyleProps rplStyleProps, Style style, bool writeShared, PageContext pageContext)
		{
		}

		internal override void WriteBorderProps(BinaryWriter spbifWriter, Style style)
		{
		}

		internal override void WriteBorderProps(RPLStyleProps rplStyleProps, Style style)
		{
		}

		internal void WriteItemToStream(RPLWriter rplWriter, PageContext pageContext)
		{
			BinaryWriter binaryWriter = rplWriter.BinaryWriter;
			if (binaryWriter != null)
			{
				Stream baseStream = binaryWriter.BaseStream;
				m_offset = baseStream.Position;
				binaryWriter.Write((byte)20);
				WriteElementProps(binaryWriter, rplWriter, pageContext, m_offset + 1);
				binaryWriter.Write(byte.MaxValue);
			}
			else
			{
				m_rplElement = new RPLTextRun();
				WriteElementProps(m_rplElement.ElementProps, rplWriter, pageContext);
			}
		}

		internal Microsoft.ReportingServices.Rendering.RichText.TextRun GetRichTextRun()
		{
			string text = null;
			Microsoft.ReportingServices.OnDemandReportRendering.TextRun textRun = m_source as Microsoft.ReportingServices.OnDemandReportRendering.TextRun;
			RSTrace.RenderingTracer.Assert(textRun != null, "The text run definition cannot be null");
			TextRunInstance textRunInstance = null;
			textRunInstance = ((m_compiledSource != null) ? m_compiledSource : textRun.Instance);
			text = textRunInstance.Value;
			if (string.IsNullOrEmpty(text) && textRun.Value != null)
			{
				text = textRun.Value.Value;
			}
			return new Microsoft.ReportingServices.Rendering.RichText.TextRun(text, this);
		}

		private void WriteActionInstance(ActionInstance actionInst, BinaryWriter spbifWriter, PageContext pageContext)
		{
			if (actionInst == null)
			{
				return;
			}
			spbifWriter.Write((byte)11);
			spbifWriter.Write((byte)2);
			spbifWriter.Write(1);
			spbifWriter.Write((byte)3);
			if (actionInst.Label != null)
			{
				spbifWriter.Write((byte)4);
				spbifWriter.Write(actionInst.Label);
			}
			if (actionInst.Hyperlink != null)
			{
				ReportUrl hyperlink = actionInst.Hyperlink;
				if (hyperlink != null)
				{
					Uri uri = hyperlink.ToUri();
					if (null != uri)
					{
						spbifWriter.Write((byte)6);
						spbifWriter.Write(uri.AbsoluteUri);
					}
				}
			}
			else if (actionInst.BookmarkLink != null)
			{
				spbifWriter.Write((byte)7);
				spbifWriter.Write(actionInst.BookmarkLink);
			}
			spbifWriter.Write(byte.MaxValue);
			spbifWriter.Write(byte.MaxValue);
		}

		private RPLActionInfo WriteActionInstance(ActionInstance actionInst, PageContext pageContext)
		{
			if (actionInst == null)
			{
				return null;
			}
			RPLActionInfo rPLActionInfo = new RPLActionInfo(1);
			RPLAction rPLAction = null;
			rPLAction = ((actionInst.Label == null) ? new RPLAction() : new RPLAction(actionInst.Label));
			if (actionInst.Hyperlink != null)
			{
				ReportUrl hyperlink = actionInst.Hyperlink;
				if (hyperlink != null)
				{
					Uri uri = hyperlink.ToUri();
					if (null != uri)
					{
						rPLAction.Hyperlink = uri.AbsoluteUri;
					}
				}
			}
			else if (actionInst.BookmarkLink != null)
			{
				rPLAction.BookmarkLink = actionInst.BookmarkLink;
			}
			rPLActionInfo.Actions[0] = rPLAction;
			return rPLActionInfo;
		}

		private bool IsBold(RPLFormat.FontWeights fontWeight)
		{
			if (fontWeight == RPLFormat.FontWeights.SemiBold || fontWeight == RPLFormat.FontWeights.Bold || fontWeight == RPLFormat.FontWeights.ExtraBold || fontWeight == RPLFormat.FontWeights.Heavy)
			{
				return true;
			}
			return false;
		}

		private bool IsItalic(RPLFormat.FontStyles fontStyle)
		{
			if (fontStyle == RPLFormat.FontStyles.Italic)
			{
				return true;
			}
			return false;
		}
	}
}
