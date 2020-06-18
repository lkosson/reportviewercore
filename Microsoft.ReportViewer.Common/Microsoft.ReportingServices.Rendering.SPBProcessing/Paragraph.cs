using Microsoft.ReportingServices.Diagnostics.Utilities;
using Microsoft.ReportingServices.OnDemandReportRendering;
using Microsoft.ReportingServices.Rendering.RichText;
using Microsoft.ReportingServices.Rendering.RPLProcessing;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace Microsoft.ReportingServices.Rendering.SPBProcessing
{
	internal sealed class Paragraph : PageElement, IParagraphProps
	{
		private long m_offset;

		private List<TextRun> m_textRuns;

		private RPLParagraph m_rplElement;

		private CompiledParagraphInstance m_compiledSource;

		private CompiledParagraphInstanceCollection m_compiledParagraphsCollection;

		private int m_paragraphNumber;

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

		internal List<TextRun> TextRuns => m_textRuns;

		internal RPLParagraph RPLElement => m_rplElement;

		internal CompiledParagraphInstance CompiledInstance => m_compiledSource;

		internal CompiledParagraphInstanceCollection CompiledParagraphsCollection => m_compiledParagraphsCollection;

		public RPLFormat.TextAlignments Alignment => (RPLFormat.TextAlignments)StyleEnumConverter.Translate((TextAlignments)GetRichTextStyleValue(StyleAttributeNames.TextAlign, m_compiledSource));

		public float SpaceBefore
		{
			get
			{
				Microsoft.ReportingServices.OnDemandReportRendering.Paragraph paragraph = m_source as Microsoft.ReportingServices.OnDemandReportRendering.Paragraph;
				ParagraphInstance compiledSource = m_compiledSource;
				ReportSize reportSize;
				if (compiledSource == null)
				{
					compiledSource = paragraph.Instance;
					reportSize = compiledSource.SpaceBefore;
				}
				else
				{
					reportSize = compiledSource.SpaceBefore;
					if (reportSize == null && paragraph.SpaceBefore != null)
					{
						reportSize = paragraph.SpaceBefore.Value;
					}
				}
				if (reportSize != null)
				{
					return (float)reportSize.ToMillimeters();
				}
				return 0f;
			}
		}

		public float SpaceAfter
		{
			get
			{
				Microsoft.ReportingServices.OnDemandReportRendering.Paragraph paragraph = m_source as Microsoft.ReportingServices.OnDemandReportRendering.Paragraph;
				ParagraphInstance compiledSource = m_compiledSource;
				ReportSize reportSize;
				if (compiledSource == null)
				{
					compiledSource = paragraph.Instance;
					reportSize = compiledSource.SpaceAfter;
				}
				else
				{
					reportSize = compiledSource.SpaceAfter;
					if (reportSize == null && paragraph.SpaceAfter != null)
					{
						reportSize = paragraph.SpaceAfter.Value;
					}
				}
				if (reportSize != null)
				{
					return (float)reportSize.ToMillimeters();
				}
				return 0f;
			}
		}

		public float LeftIndent
		{
			get
			{
				Microsoft.ReportingServices.OnDemandReportRendering.Paragraph paragraph = m_source as Microsoft.ReportingServices.OnDemandReportRendering.Paragraph;
				ParagraphInstance compiledSource = m_compiledSource;
				ReportSize reportSize;
				if (compiledSource == null)
				{
					compiledSource = paragraph.Instance;
					reportSize = compiledSource.LeftIndent;
				}
				else
				{
					reportSize = compiledSource.LeftIndent;
					if (reportSize == null && paragraph.LeftIndent != null)
					{
						reportSize = paragraph.LeftIndent.Value;
					}
				}
				if (reportSize != null)
				{
					return (float)reportSize.ToMillimeters();
				}
				return 0f;
			}
		}

		public float RightIndent
		{
			get
			{
				Microsoft.ReportingServices.OnDemandReportRendering.Paragraph paragraph = m_source as Microsoft.ReportingServices.OnDemandReportRendering.Paragraph;
				ParagraphInstance compiledSource = m_compiledSource;
				ReportSize reportSize;
				if (compiledSource == null)
				{
					compiledSource = paragraph.Instance;
					reportSize = compiledSource.RightIndent;
				}
				else
				{
					reportSize = compiledSource.RightIndent;
					if (reportSize == null && paragraph.RightIndent != null)
					{
						reportSize = paragraph.RightIndent.Value;
					}
				}
				if (reportSize != null)
				{
					return (float)reportSize.ToMillimeters();
				}
				return 0f;
			}
		}

		public float HangingIndent
		{
			get
			{
				Microsoft.ReportingServices.OnDemandReportRendering.Paragraph paragraph = m_source as Microsoft.ReportingServices.OnDemandReportRendering.Paragraph;
				ParagraphInstance compiledSource = m_compiledSource;
				ReportSize reportSize;
				if (compiledSource == null)
				{
					compiledSource = paragraph.Instance;
					reportSize = compiledSource.HangingIndent;
				}
				else
				{
					reportSize = compiledSource.HangingIndent;
					if (reportSize == null && paragraph.HangingIndent != null)
					{
						reportSize = paragraph.HangingIndent.Value;
					}
				}
				if (reportSize != null)
				{
					return (float)reportSize.ToMillimeters();
				}
				return 0f;
			}
		}

		public int ListLevel
		{
			get
			{
				Microsoft.ReportingServices.OnDemandReportRendering.Paragraph paragraph = m_source as Microsoft.ReportingServices.OnDemandReportRendering.Paragraph;
				ParagraphInstance paragraphInstance = m_compiledSource;
				if (paragraphInstance == null)
				{
					paragraphInstance = paragraph.Instance;
				}
				return paragraphInstance.ListLevel;
			}
		}

		public RPLFormat.ListStyles ListStyle
		{
			get
			{
				Microsoft.ReportingServices.OnDemandReportRendering.Paragraph paragraph = m_source as Microsoft.ReportingServices.OnDemandReportRendering.Paragraph;
				ParagraphInstance paragraphInstance = m_compiledSource;
				if (paragraphInstance == null)
				{
					paragraphInstance = paragraph.Instance;
				}
				return (RPLFormat.ListStyles)StyleEnumConverter.Translate(paragraphInstance.ListStyle);
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

		private Paragraph()
			: base(null)
		{
		}

		internal Paragraph(Microsoft.ReportingServices.OnDemandReportRendering.Paragraph paragraph, PageContext pageContext)
			: this(paragraph, null, pageContext)
		{
		}

		internal Paragraph(Microsoft.ReportingServices.OnDemandReportRendering.Paragraph paragraph, CompiledParagraphInstance compiledParagraph, PageContext pageContext)
			: base(paragraph)
		{
			m_compiledSource = compiledParagraph;
			BuildTextRunCollection(pageContext);
		}

		internal object GetRichTextStyleValue(StyleAttributeNames styleName, ref bool isShared)
		{
			return GetRichTextStyleValue(styleName, m_compiledSource, ref isShared);
		}

		internal object GetRichTextStyleValue(StyleAttributeNames styleName)
		{
			return GetRichTextStyleValue(styleName, m_compiledSource);
		}

		internal override void WriteSharedItemProps(BinaryWriter spbifWriter, RPLWriter rplWriter, PageContext pageContext, long offset)
		{
			Microsoft.ReportingServices.OnDemandReportRendering.Paragraph paragraph = m_source as Microsoft.ReportingServices.OnDemandReportRendering.Paragraph;
			RSTrace.RenderingTracer.Assert(paragraph != null, "The paragraph definition cannot be null");
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
			if (IsNotExpressionValue(paragraph.LeftIndent))
			{
				spbifWriter.Write((byte)9);
				spbifWriter.Write(paragraph.LeftIndent.Value.ToString());
			}
			if (IsNotExpressionValue(paragraph.RightIndent))
			{
				spbifWriter.Write((byte)10);
				spbifWriter.Write(paragraph.RightIndent.Value.ToString());
			}
			if (IsNotExpressionValue(paragraph.HangingIndent))
			{
				spbifWriter.Write((byte)11);
				spbifWriter.Write(paragraph.HangingIndent.Value.ToString());
			}
			if (!paragraph.ListStyle.IsExpression)
			{
				spbifWriter.Write((byte)7);
				spbifWriter.Write(StyleEnumConverter.Translate(paragraph.ListStyle.Value));
			}
			if (!paragraph.ListLevel.IsExpression)
			{
				spbifWriter.Write((byte)8);
				spbifWriter.Write(paragraph.ListLevel.Value);
			}
			if (IsNotExpressionValue(paragraph.SpaceBefore))
			{
				spbifWriter.Write((byte)12);
				spbifWriter.Write(paragraph.SpaceBefore.Value.ToString());
			}
			if (IsNotExpressionValue(paragraph.SpaceAfter))
			{
				spbifWriter.Write((byte)13);
				spbifWriter.Write(paragraph.SpaceAfter.Value.ToString());
			}
			WriteSharedStyle(spbifWriter, null, pageContext, 6);
			spbifWriter.Write(byte.MaxValue);
		}

		internal override void WriteSharedItemProps(RPLElementProps elemProps, RPLWriter rplWriter, PageContext pageContext)
		{
			Microsoft.ReportingServices.OnDemandReportRendering.Paragraph paragraph = m_source as Microsoft.ReportingServices.OnDemandReportRendering.Paragraph;
			RSTrace.RenderingTracer.Assert(paragraph != null, "The paragraph definition cannot be null");
			Hashtable hashtable = pageContext.ItemPropsStart;
			if (hashtable != null)
			{
				object obj = hashtable[SourceID];
				if (obj != null)
				{
					elemProps.Definition = (RPLParagraphPropsDef)obj;
					return;
				}
			}
			RPLParagraphPropsDef rPLParagraphPropsDef = (elemProps as RPLParagraphProps).Definition as RPLParagraphPropsDef;
			if (hashtable == null)
			{
				hashtable = (pageContext.ItemPropsStart = new Hashtable());
			}
			hashtable.Add(SourceID, rPLParagraphPropsDef);
			rPLParagraphPropsDef.ID = SourceID;
			if (IsNotExpressionValue(paragraph.LeftIndent))
			{
				rPLParagraphPropsDef.LeftIndent = new RPLReportSize(paragraph.LeftIndent.Value.ToString());
			}
			if (IsNotExpressionValue(paragraph.RightIndent))
			{
				rPLParagraphPropsDef.RightIndent = new RPLReportSize(paragraph.RightIndent.Value.ToString());
			}
			if (IsNotExpressionValue(paragraph.HangingIndent))
			{
				rPLParagraphPropsDef.HangingIndent = new RPLReportSize(paragraph.HangingIndent.Value.ToString());
			}
			if (!paragraph.ListStyle.IsExpression)
			{
				rPLParagraphPropsDef.ListStyle = (RPLFormat.ListStyles)StyleEnumConverter.Translate(paragraph.ListStyle.Value);
			}
			if (!paragraph.ListLevel.IsExpression)
			{
				rPLParagraphPropsDef.ListLevel = paragraph.ListLevel.Value;
			}
			if (IsNotExpressionValue(paragraph.SpaceBefore))
			{
				rPLParagraphPropsDef.SpaceBefore = new RPLReportSize(paragraph.SpaceBefore.Value.ToString());
			}
			if (IsNotExpressionValue(paragraph.SpaceAfter))
			{
				rPLParagraphPropsDef.SpaceAfter = new RPLReportSize(paragraph.SpaceAfter.Value.ToString());
			}
			rPLParagraphPropsDef.SharedStyle = WriteSharedStyle(null, pageContext);
		}

		internal override void WriteNonSharedItemProps(BinaryWriter spbifWriter, RPLWriter rplWriter, PageContext pageContext)
		{
			Microsoft.ReportingServices.OnDemandReportRendering.Paragraph paragraph = m_source as Microsoft.ReportingServices.OnDemandReportRendering.Paragraph;
			RSTrace.RenderingTracer.Assert(paragraph != null, "The paragraph definition cannot be null");
			ParagraphInstance paragraphInstance = null;
			bool flag = false;
			if (m_compiledSource != null)
			{
				paragraphInstance = m_compiledSource;
				flag = true;
			}
			else
			{
				paragraphInstance = paragraph.Instance;
				RSTrace.RenderingTracer.Assert(paragraphInstance != null, "The paragraph instance cannot be null");
			}
			spbifWriter.Write((byte)1);
			spbifWriter.Write((byte)4);
			spbifWriter.Write(paragraphInstance.UniqueName);
			if (!flag)
			{
				if (IsExpressionValue(paragraph.LeftIndent, paragraphInstance.LeftIndent))
				{
					spbifWriter.Write((byte)9);
					spbifWriter.Write(paragraphInstance.LeftIndent.ToString());
				}
				if (IsExpressionValue(paragraph.RightIndent, paragraphInstance.RightIndent))
				{
					spbifWriter.Write((byte)10);
					spbifWriter.Write(paragraphInstance.RightIndent.ToString());
				}
				if (IsExpressionValue(paragraph.HangingIndent, paragraphInstance.HangingIndent))
				{
					spbifWriter.Write((byte)11);
					spbifWriter.Write(paragraphInstance.HangingIndent.ToString());
				}
				if (paragraph.ListStyle.IsExpression)
				{
					spbifWriter.Write((byte)7);
					spbifWriter.Write(StyleEnumConverter.Translate(paragraphInstance.ListStyle));
				}
				if (paragraph.ListLevel.IsExpression)
				{
					spbifWriter.Write((byte)8);
					spbifWriter.Write(paragraphInstance.ListLevel);
				}
				if (IsExpressionValue(paragraph.SpaceBefore, paragraphInstance.SpaceBefore))
				{
					spbifWriter.Write((byte)12);
					spbifWriter.Write(paragraphInstance.SpaceBefore.ToString());
				}
				if (IsExpressionValue(paragraph.SpaceAfter, paragraphInstance.SpaceAfter))
				{
					spbifWriter.Write((byte)13);
					spbifWriter.Write(paragraphInstance.SpaceAfter.ToString());
				}
			}
			else
			{
				if (paragraphInstance.LeftIndent != null)
				{
					spbifWriter.Write((byte)9);
					spbifWriter.Write(paragraphInstance.LeftIndent.ToString());
				}
				if (paragraphInstance.RightIndent != null)
				{
					spbifWriter.Write((byte)10);
					spbifWriter.Write(paragraphInstance.RightIndent.ToString());
				}
				if (paragraphInstance.HangingIndent != null)
				{
					spbifWriter.Write((byte)11);
					spbifWriter.Write(paragraphInstance.HangingIndent.ToString());
				}
				spbifWriter.Write((byte)7);
				spbifWriter.Write(StyleEnumConverter.Translate(paragraphInstance.ListStyle));
				spbifWriter.Write((byte)8);
				spbifWriter.Write(paragraphInstance.ListLevel);
				if (paragraphInstance.SpaceBefore != null)
				{
					spbifWriter.Write((byte)12);
					spbifWriter.Write(paragraphInstance.SpaceBefore.ToString());
				}
				if (paragraphInstance.SpaceAfter != null)
				{
					spbifWriter.Write((byte)13);
					spbifWriter.Write(paragraphInstance.SpaceAfter.ToString());
				}
			}
			if (m_paragraphNumber > 0)
			{
				spbifWriter.Write((byte)14);
				spbifWriter.Write(m_paragraphNumber);
			}
			WriteNonSharedStyle(spbifWriter, null, null, pageContext, 6, m_compiledSource);
			spbifWriter.Write(byte.MaxValue);
		}

		internal override void WriteNonSharedItemProps(RPLElementProps elemProps, RPLWriter rplWriter, PageContext pageContext)
		{
			Microsoft.ReportingServices.OnDemandReportRendering.Paragraph paragraph = m_source as Microsoft.ReportingServices.OnDemandReportRendering.Paragraph;
			RSTrace.RenderingTracer.Assert(paragraph != null, "The paragraph definition cannot be null");
			ParagraphInstance paragraphInstance = null;
			bool flag = false;
			if (m_compiledSource != null)
			{
				paragraphInstance = m_compiledSource;
				flag = true;
			}
			else
			{
				paragraphInstance = paragraph.Instance;
				RSTrace.RenderingTracer.Assert(paragraphInstance != null, "The paragraph instance cannot be null");
			}
			elemProps.UniqueName = paragraphInstance.UniqueName;
			RPLParagraphProps rPLParagraphProps = elemProps as RPLParagraphProps;
			if (!flag)
			{
				if (IsExpressionValue(paragraph.LeftIndent, paragraphInstance.LeftIndent))
				{
					rPLParagraphProps.LeftIndent = new RPLReportSize(paragraphInstance.LeftIndent.ToString());
				}
				if (IsExpressionValue(paragraph.RightIndent, paragraphInstance.RightIndent))
				{
					rPLParagraphProps.RightIndent = new RPLReportSize(paragraphInstance.RightIndent.ToString());
				}
				if (IsExpressionValue(paragraph.HangingIndent, paragraphInstance.HangingIndent))
				{
					rPLParagraphProps.HangingIndent = new RPLReportSize(paragraphInstance.HangingIndent.ToString());
				}
				if (paragraph.ListStyle.IsExpression)
				{
					rPLParagraphProps.ListStyle = (RPLFormat.ListStyles)StyleEnumConverter.Translate(paragraphInstance.ListStyle);
				}
				else
				{
					rPLParagraphProps.ListStyle = null;
				}
				if (paragraph.ListLevel.IsExpression)
				{
					rPLParagraphProps.ListLevel = paragraphInstance.ListLevel;
				}
				else
				{
					rPLParagraphProps.ListLevel = null;
				}
				if (IsExpressionValue(paragraph.SpaceBefore, paragraphInstance.SpaceBefore))
				{
					rPLParagraphProps.SpaceBefore = new RPLReportSize(paragraphInstance.SpaceBefore.ToString());
				}
				if (IsExpressionValue(paragraph.SpaceAfter, paragraphInstance.SpaceAfter))
				{
					rPLParagraphProps.SpaceAfter = new RPLReportSize(paragraphInstance.SpaceAfter.ToString());
				}
			}
			else
			{
				if (paragraphInstance.LeftIndent != null)
				{
					rPLParagraphProps.LeftIndent = new RPLReportSize(paragraphInstance.LeftIndent.ToString());
				}
				if (paragraphInstance.RightIndent != null)
				{
					rPLParagraphProps.RightIndent = new RPLReportSize(paragraphInstance.RightIndent.ToString());
				}
				if (paragraphInstance.HangingIndent != null)
				{
					rPLParagraphProps.HangingIndent = new RPLReportSize(paragraphInstance.HangingIndent.ToString());
				}
				rPLParagraphProps.ListStyle = (RPLFormat.ListStyles)StyleEnumConverter.Translate(paragraphInstance.ListStyle);
				rPLParagraphProps.ListLevel = paragraphInstance.ListLevel;
				if (paragraphInstance.SpaceBefore != null)
				{
					rPLParagraphProps.SpaceBefore = new RPLReportSize(paragraphInstance.SpaceBefore.ToString());
				}
				if (paragraphInstance.SpaceAfter != null)
				{
					rPLParagraphProps.SpaceAfter = new RPLReportSize(paragraphInstance.SpaceAfter.ToString());
				}
			}
			rPLParagraphProps.ParagraphNumber = m_paragraphNumber;
			rPLParagraphProps.NonSharedStyle = WriteNonSharedStyle(null, null, pageContext, m_compiledSource);
		}

		internal override void WriteItemSharedStyleProps(BinaryWriter spbifWriter, Style style, PageContext pageContext)
		{
			WriteStyleProp(style, spbifWriter, StyleAttributeNames.TextAlign, 25);
			WriteStyleProp(style, spbifWriter, StyleAttributeNames.LineHeight, 28);
		}

		internal override void WriteItemSharedStyleProps(RPLStyleProps rplStyleProps, Style style, PageContext pageContext)
		{
			WriteStyleProp(style, rplStyleProps, StyleAttributeNames.TextAlign, 25);
			WriteStyleProp(style, rplStyleProps, StyleAttributeNames.LineHeight, 28);
		}

		internal override void WriteNonSharedStyleProp(BinaryWriter spbifWriter, Style styleDef, StyleInstance style, StyleAttributeNames styleAttribute, PageContext pageContext)
		{
			switch (styleAttribute)
			{
			case StyleAttributeNames.TextAlign:
				WriteStyleProp(styleDef, style, spbifWriter, styleAttribute, 25);
				break;
			case StyleAttributeNames.LineHeight:
				WriteStyleProp(styleDef, style, spbifWriter, styleAttribute, 28);
				break;
			}
		}

		internal override void WriteNonSharedStyleProp(RPLStyleProps rplStyleProps, Style styleDef, StyleInstance style, StyleAttributeNames styleAttribute, PageContext pageContext)
		{
			switch (styleAttribute)
			{
			case StyleAttributeNames.TextAlign:
				WriteStyleProp(styleDef, style, rplStyleProps, styleAttribute, 25);
				break;
			case StyleAttributeNames.LineHeight:
				WriteStyleProp(styleDef, style, rplStyleProps, styleAttribute, 28);
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
			List<long> list = null;
			BinaryWriter binaryWriter = rplWriter.BinaryWriter;
			if (binaryWriter != null)
			{
				if (m_textRuns != null)
				{
					list = new List<long>();
					foreach (TextRun textRun in m_textRuns)
					{
						textRun.WriteItemToStream(rplWriter, pageContext);
						list.Add(textRun.Offset);
					}
				}
				Stream baseStream = binaryWriter.BaseStream;
				m_offset = baseStream.Position;
				binaryWriter.Write((byte)19);
				WriteElementProps(binaryWriter, rplWriter, pageContext, m_offset + 1);
				if (list != null)
				{
					binaryWriter.Write(list.Count);
					for (int i = 0; i < list.Count; i++)
					{
						binaryWriter.Write(list[i]);
					}
				}
				else
				{
					binaryWriter.Write(0);
				}
				binaryWriter.Write(byte.MaxValue);
				return;
			}
			m_rplElement = new RPLParagraph();
			if (m_textRuns != null)
			{
				foreach (TextRun textRun2 in m_textRuns)
				{
					textRun2.WriteItemToStream(rplWriter, pageContext);
					m_rplElement.AddTextRun(textRun2.RPLElement);
				}
			}
			WriteElementProps(m_rplElement.ElementProps, rplWriter, pageContext);
		}

		internal Microsoft.ReportingServices.Rendering.RichText.Paragraph GetRichTextParagraph()
		{
			Microsoft.ReportingServices.Rendering.RichText.Paragraph paragraph = new Microsoft.ReportingServices.Rendering.RichText.Paragraph(this, m_textRuns.Count);
			for (int i = 0; i < m_textRuns.Count; i++)
			{
				Microsoft.ReportingServices.Rendering.RichText.TextRun richTextRun = m_textRuns[i].GetRichTextRun();
				if (richTextRun != null)
				{
					paragraph.Runs.Add(richTextRun);
				}
			}
			return paragraph;
		}

		private void BuildTextRunCollection(PageContext pageContext)
		{
			Microsoft.ReportingServices.OnDemandReportRendering.Paragraph paragraph = m_source as Microsoft.ReportingServices.OnDemandReportRendering.Paragraph;
			RSTrace.RenderingTracer.Assert(paragraph != null, "The paragraph definition cannot be null");
			ParagraphInstance paragraphInstance = null;
			if (m_compiledSource != null)
			{
				paragraphInstance = m_compiledSource;
			}
			else
			{
				paragraphInstance = paragraph.Instance;
				RSTrace.RenderingTracer.Assert(paragraphInstance != null, "The paragraph instance cannot be null");
			}
			IEnumerator<TextRunInstance> enumerator = paragraphInstance.TextRunInstances.GetEnumerator();
			while (enumerator.MoveNext())
			{
				TextRunInstance current = enumerator.Current;
				if (m_textRuns == null)
				{
					m_textRuns = new List<TextRun>();
				}
				TextRun textRun = null;
				textRun = ((!current.IsCompiled) ? new TextRun(current.Definition, pageContext) : new TextRun(current.Definition, current as CompiledTextRunInstance, pageContext));
				m_textRuns.Add(textRun);
			}
		}

		private bool IsExpressionValue(ReportSizeProperty property, ReportSize instance)
		{
			if (property != null && property.IsExpression && instance != null)
			{
				return true;
			}
			return false;
		}

		private bool IsNotExpressionValue(ReportSizeProperty property)
		{
			if (property != null && property.Value != null && !property.IsExpression)
			{
				return true;
			}
			return false;
		}
	}
}
