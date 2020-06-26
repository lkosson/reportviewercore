using Microsoft.ReportingServices.Rendering.ImageRenderer;
using Microsoft.ReportingServices.Rendering.RichText;
using Microsoft.ReportingServices.Rendering.RPLProcessing;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Microsoft.Reporting.NETCore
{
	internal sealed class RenderingParagraph : RenderingElement, IParagraphProps
	{
		private int m_paragraphNumber;

		private bool m_isSimple = true;

		private List<RenderingTextRun> m_textRuns;

		private RectangleF m_textPosition = RectangleF.Empty;

		private Paragraph m_richParagraph;

		private List<SearchMatch> m_searchResults;

		private int m_firstSearchIndex = -1;

		private int m_lastSearchIndex = -1;

		internal Paragraph RichParagraph
		{
			get
			{
				return m_richParagraph;
			}
			set
			{
				m_richParagraph = value;
			}
		}

		internal string Text
		{
			get
			{
				StringBuilder stringBuilder = new StringBuilder();
				for (int i = 0; i < m_textRuns.Count; i++)
				{
					if (!string.IsNullOrEmpty(m_textRuns[i].Text))
					{
						stringBuilder.Append(m_textRuns[i].Text);
					}
				}
				return stringBuilder.ToString();
			}
		}

		internal RectangleF TextPosition
		{
			get
			{
				return m_textPosition;
			}
			set
			{
				m_textPosition = value;
			}
		}

		public float SpaceBefore
		{
			get
			{
				if (m_isSimple)
				{
					return 0f;
				}
				RPLParagraphProps rPLParagraphProps = InstanceProperties as RPLParagraphProps;
				if (rPLParagraphProps.SpaceBefore != null)
				{
					return (float)rPLParagraphProps.SpaceBefore.ToMillimeters();
				}
				RPLParagraphPropsDef rPLParagraphPropsDef = DefinitionProperties as RPLParagraphPropsDef;
				if (rPLParagraphPropsDef.SpaceBefore != null)
				{
					return (float)rPLParagraphPropsDef.SpaceBefore.ToMillimeters();
				}
				return 0f;
			}
		}

		public float SpaceAfter
		{
			get
			{
				if (m_isSimple)
				{
					return 0f;
				}
				RPLParagraphProps rPLParagraphProps = InstanceProperties as RPLParagraphProps;
				if (rPLParagraphProps.SpaceAfter != null)
				{
					return (float)rPLParagraphProps.SpaceAfter.ToMillimeters();
				}
				RPLParagraphPropsDef rPLParagraphPropsDef = DefinitionProperties as RPLParagraphPropsDef;
				if (rPLParagraphPropsDef.SpaceAfter != null)
				{
					return (float)rPLParagraphPropsDef.SpaceAfter.ToMillimeters();
				}
				return 0f;
			}
		}

		public float LeftIndent
		{
			get
			{
				if (m_isSimple)
				{
					return 0f;
				}
				RPLParagraphProps rPLParagraphProps = InstanceProperties as RPLParagraphProps;
				if (rPLParagraphProps.LeftIndent != null)
				{
					return (float)rPLParagraphProps.LeftIndent.ToMillimeters();
				}
				RPLParagraphPropsDef rPLParagraphPropsDef = DefinitionProperties as RPLParagraphPropsDef;
				if (rPLParagraphPropsDef.LeftIndent != null)
				{
					return (float)rPLParagraphPropsDef.LeftIndent.ToMillimeters();
				}
				return 0f;
			}
		}

		public float RightIndent
		{
			get
			{
				if (m_isSimple)
				{
					return 0f;
				}
				RPLParagraphProps rPLParagraphProps = InstanceProperties as RPLParagraphProps;
				if (rPLParagraphProps.RightIndent != null)
				{
					return (float)rPLParagraphProps.RightIndent.ToMillimeters();
				}
				RPLParagraphPropsDef rPLParagraphPropsDef = DefinitionProperties as RPLParagraphPropsDef;
				if (rPLParagraphPropsDef.RightIndent != null)
				{
					return (float)rPLParagraphPropsDef.RightIndent.ToMillimeters();
				}
				return 0f;
			}
		}

		public float HangingIndent
		{
			get
			{
				if (m_isSimple)
				{
					return 0f;
				}
				RPLParagraphProps rPLParagraphProps = InstanceProperties as RPLParagraphProps;
				if (rPLParagraphProps.HangingIndent != null)
				{
					return (float)rPLParagraphProps.HangingIndent.ToMillimeters();
				}
				RPLParagraphPropsDef rPLParagraphPropsDef = DefinitionProperties as RPLParagraphPropsDef;
				if (rPLParagraphPropsDef.HangingIndent != null)
				{
					return (float)rPLParagraphPropsDef.HangingIndent.ToMillimeters();
				}
				return 0f;
			}
		}

		public int ListLevel
		{
			get
			{
				if (m_isSimple)
				{
					return 0;
				}
				RPLParagraphProps obj = InstanceProperties as RPLParagraphProps;
				RPLParagraphPropsDef rPLParagraphPropsDef = DefinitionProperties as RPLParagraphPropsDef;
				return obj.ListLevel ?? rPLParagraphPropsDef.ListLevel;
			}
		}

		public RPLFormat.ListStyles ListStyle
		{
			get
			{
				if (m_isSimple)
				{
					return RPLFormat.ListStyles.None;
				}
				RPLParagraphProps obj = InstanceProperties as RPLParagraphProps;
				RPLParagraphPropsDef rPLParagraphPropsDef = DefinitionProperties as RPLParagraphPropsDef;
				return obj.ListStyle ?? rPLParagraphPropsDef.ListStyle;
			}
		}

		public RPLFormat.TextAlignments Alignment
		{
			get
			{
				object obj = null;
				obj = ((!m_isSimple) ? SharedRenderer.GetStylePropertyValueObject(InstanceProperties as RPLParagraphProps, 25) : SharedRenderer.GetStylePropertyValueObject(InstanceProperties as RPLTextBoxProps, 25));
				if (obj != null)
				{
					return (RPLFormat.TextAlignments)obj;
				}
				return RPLFormat.TextAlignments.General;
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

		internal void Initialize(RPLTextBox rplTextBox, RectangleF bounds)
		{
			base.Initialize(rplTextBox);
			m_paragraphNumber = 1;
			m_isSimple = true;
			m_position = GdiContext.GetMeasurementRectangle(null, bounds);
		}

		internal void Initialize(RPLParagraph rplParagraph, RectangleF bounds)
		{
			base.Initialize(rplParagraph);
			m_paragraphNumber = (InstanceProperties as RPLParagraphProps).ParagraphNumber;
			m_isSimple = false;
			m_position = GdiContext.GetMeasurementRectangle(null, bounds);
		}

		internal override void ProcessRenderingElementContent(RPLElement rplElement, GdiContext context, RectangleF bounds)
		{
			if (m_textRuns == null)
			{
				m_textRuns = new List<RenderingTextRun>();
			}
			if (m_isSimple)
			{
				RPLTextBox rplElement2 = rplElement as RPLTextBox;
				RenderingTextRun renderingTextRun = new RenderingTextRun();
				renderingTextRun.Initialize(rplElement2, bounds, m_textRuns.Count);
				renderingTextRun.ProcessRenderingElementContent(rplElement2, context, bounds);
				m_textRuns.Add(renderingTextRun);
				return;
			}
			RPLParagraph rPLParagraph = rplElement as RPLParagraph;
			for (RPLTextRun nextTextRun = rPLParagraph.GetNextTextRun(); nextTextRun != null; nextTextRun = rPLParagraph.GetNextTextRun())
			{
				RenderingTextRun renderingTextRun2 = new RenderingTextRun();
				renderingTextRun2.Initialize(nextTextRun, bounds, m_textRuns.Count);
				renderingTextRun2.ProcessRenderingElementContent(nextTextRun, context, bounds);
				m_textRuns.Add(renderingTextRun2);
			}
		}

		internal override void DrawContent(GdiContext context)
		{
			m_richParagraph = new Paragraph(this, m_textRuns.Count);
			for (int i = 0; i < m_textRuns.Count; i++)
			{
				if (i >= context.TextRunIndexHitStart && i <= context.TextRunIndexHitEnd)
				{
					context.SearchHit = true;
				}
				m_textRuns[i].DrawContent(context);
				context.SearchHit = false;
				m_richParagraph.Runs.Add(m_textRuns[i].RichTextRun);
			}
		}

		internal void Search(GdiContext context, int paragraphIndex)
		{
			if (string.IsNullOrEmpty(context.SearchText))
			{
				m_searchResults = null;
				m_firstSearchIndex = -1;
				m_lastSearchIndex = -1;
				return;
			}
			if (m_searchResults != null)
			{
				m_searchResults.Clear();
			}
			string text = Text;
			string text2 = context.SearchText;
			if (text2.IndexOf(' ') >= 0)
			{
				text2 = text2.Replace('\u00a0', ' ');
				text = text.Replace('\u00a0', ' ');
			}
			int num = text.IndexOf(text2, 0, StringComparison.OrdinalIgnoreCase);
			if (num == -1)
			{
				return;
			}
			if (context.SearchMatches == null)
			{
				context.SearchMatches = new List<SearchMatch>();
			}
			if (m_searchResults == null)
			{
				m_searchResults = new List<SearchMatch>();
			}
			m_firstSearchIndex = context.SearchMatches.Count;
			int num2 = -1;
			int num3 = 0;
			string text3 = null;
			do
			{
				num2++;
				text3 = m_textRuns[num2].Text;
			}
			while (string.IsNullOrEmpty(text3) && num2 < m_textRuns.Count);
			while (num > -1)
			{
				while (num > num3 + text3.Length && num2 + 1 < m_textRuns.Count)
				{
					num3 += text3.Length;
					num2++;
					text3 = m_textRuns[num2].Text;
				}
				TextBoxContext textBoxContext = new TextBoxContext();
				textBoxContext.ParagraphIndex = paragraphIndex;
				textBoxContext.TextRunIndex = num2;
				textBoxContext.TextRunCharacterIndex = num - num3;
				while (num3 + text3.Length < num + text2.Length && num2 + 1 < m_textRuns.Count)
				{
					num3 += text3.Length;
					num2++;
					text3 = m_textRuns[num2].Text;
				}
				TextBoxContext textBoxContext2 = new TextBoxContext();
				textBoxContext2.ParagraphIndex = paragraphIndex;
				textBoxContext2.TextRunIndex = num2;
				textBoxContext2.TextRunCharacterIndex = num + text2.Length - num3;
				SearchMatch searchMatch = new SearchMatch(new RTSelectionHighlight(textBoxContext, textBoxContext2, SystemColors.Highlight));
				searchMatch.Point = new PointF(TextPosition.X, TextPosition.Y);
				m_searchResults.Add(searchMatch);
				context.SearchMatches.Add(searchMatch);
				m_lastSearchIndex = context.SearchMatches.Count - 1;
				num = text.IndexOf(text2, num + text2.Length, StringComparison.OrdinalIgnoreCase);
			}
		}

		internal RTSelectionHighlight GetSearchHit(GdiContext context)
		{
			if (string.IsNullOrEmpty(context.SearchText))
			{
				return null;
			}
			RTSelectionHighlight result = null;
			if (context.SearchMatches != null)
			{
				int num = context.SearchMatchIndex;
				if (num >= context.SearchMatches.Count)
				{
					num -= context.SearchMatches.Count;
				}
				if (m_searchResults != null && m_firstSearchIndex <= num && num <= m_lastSearchIndex)
				{
					result = m_searchResults[num - m_firstSearchIndex].Match;
				}
			}
			return result;
		}
	}
}
