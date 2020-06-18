using Microsoft.ReportingServices.Rendering.ImageRenderer;
using Microsoft.ReportingServices.Rendering.RichText;
using Microsoft.ReportingServices.Rendering.RPLProcessing;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Security.Permissions;
using System.Text;

namespace Microsoft.Reporting.WinForms
{
	internal sealed class RenderingTextBox : RenderingItem, ITextBoxProps
	{
		private TextBox m_richTextBox;

		private List<RenderingParagraph> m_paragraphs;

		private GdiContext m_context;

		private RectangleF m_textPosition;

		private RectangleF m_toggleRectangleMM = RectangleF.Empty;

		private RectangleF m_sortRectangleMM = RectangleF.Empty;

		internal RectangleF TextPosition => m_textPosition;

		public RPLFormat.VerticalAlignments VerticalAlign
		{
			get
			{
				object stylePropertyValueObject = SharedRenderer.GetStylePropertyValueObject(InstanceProperties, 26);
				if (stylePropertyValueObject != null)
				{
					return (RPLFormat.VerticalAlignments)stylePropertyValueObject;
				}
				return RPLFormat.VerticalAlignments.Top;
			}
		}

		public RPLFormat.Directions Direction
		{
			get
			{
				object stylePropertyValueObject = SharedRenderer.GetStylePropertyValueObject(InstanceProperties, 29);
				if (stylePropertyValueObject != null)
				{
					return (RPLFormat.Directions)stylePropertyValueObject;
				}
				return RPLFormat.Directions.LTR;
			}
		}

		public RPLFormat.WritingModes WritingMode
		{
			get
			{
				object stylePropertyValueObject = SharedRenderer.GetStylePropertyValueObject(InstanceProperties, 30);
				if (stylePropertyValueObject != null)
				{
					return (RPLFormat.WritingModes)stylePropertyValueObject;
				}
				return RPLFormat.WritingModes.Horizontal;
			}
		}

		public RPLFormat.TextAlignments DefaultAlignment
		{
			get
			{
				TypeCode typeCode = (DefinitionProperties as RPLTextBoxPropsDef).SharedTypeCode;
				if (typeCode == TypeCode.Object)
				{
					typeCode = (InstanceProperties as RPLTextBoxProps).TypeCode;
				}
				return SharedRenderer.GetTextAlignForGeneral(typeCode, RPLFormat.Directions.LTR);
			}
		}

		Color ITextBoxProps.BackgroundColor => SharedRenderer.GetReportColorStyle(InstanceProperties.Style, 34);

		bool ITextBoxProps.CanGrow => ((RPLTextBoxPropsDef)DefinitionProperties).CanGrow;

		public void DrawTextRun(TextRun run, Paragraph paragraph, Win32DCSafeHandle hdc, float dpiX, FontCache fontCache, int x, int y, int baselineY, int lineHeight, Rectangle layoutRectangle)
		{
			RPLFormat.WritingModes writingMode = WritingMode;
			if (string.IsNullOrEmpty(run.Text))
			{
				return;
			}
			Underline underline = null;
			if (run.UnderlineHeight > 0)
			{
				underline = new Underline(run, hdc, fontCache, layoutRectangle, x, baselineY, writingMode);
			}
			int x2;
			int baselineY2;
			switch (writingMode)
			{
			case RPLFormat.WritingModes.Horizontal:
				x2 = layoutRectangle.X + x;
				baselineY2 = layoutRectangle.Y + baselineY;
				break;
			case RPLFormat.WritingModes.Vertical:
				x2 = layoutRectangle.X + (layoutRectangle.Width - baselineY);
				baselineY2 = layoutRectangle.Y + x;
				break;
			case RPLFormat.WritingModes.Rotate270:
				x2 = layoutRectangle.X + baselineY;
				baselineY2 = layoutRectangle.Y + layoutRectangle.Height - x;
				break;
			default:
				throw new NotSupportedException();
			}
			if (!m_context.TestMode)
			{
				TextBox.DrawTextRun(run, hdc, fontCache, x2, baselineY2, underline);
			}
			else
			{
				TextBox.ExtDrawTextRun(run, hdc, fontCache, x2, baselineY2, underline);
			}
			RenderingTextRun renderingTextRun = run.TextRunProperties as RenderingTextRun;
			if (renderingTextRun == null)
			{
				return;
			}
			RPLTextRunProps rPLTextRunProps = renderingTextRun.InstanceProperties as RPLTextRunProps;
			if (rPLTextRunProps == null)
			{
				return;
			}
			float num = SharedRenderer.ConvertToMillimeters(run.GetWidth(hdc, fontCache), dpiX);
			float num2 = SharedRenderer.ConvertToMillimeters(run.GetHeight(hdc, fontCache), dpiX);
			float num3 = SharedRenderer.ConvertToMillimeters(x, dpiX);
			float num4 = SharedRenderer.ConvertToMillimeters(y, dpiX);
			SizeF empty = SizeF.Empty;
			PointF location = PointF.Empty;
			if (writingMode == RPLFormat.WritingModes.Horizontal)
			{
				empty = new SizeF(num, num2);
				location = new PointF(num3 + TextPosition.X, num4 - num2 + TextPosition.Y);
			}
			else
			{
				empty = new SizeF(num2, num);
				if (writingMode == RPLFormat.WritingModes.Vertical)
				{
					float num5 = SharedRenderer.ConvertToMillimeters(layoutRectangle.Right, dpiX);
					float num6 = SharedRenderer.ConvertToMillimeters(layoutRectangle.Y, dpiX);
					location = new PointF(num5 - num4, num6 + num3);
				}
				else
				{
					float num7 = SharedRenderer.ConvertToMillimeters(layoutRectangle.X, dpiX);
					float num8 = SharedRenderer.ConvertToMillimeters(layoutRectangle.Bottom, dpiX);
					location = new PointF(num7 + num4 - num2, num8 - num3 - num);
				}
			}
			RectangleF position = new RectangleF(location, empty);
			renderingTextRun.ProcessActions(m_context, rPLTextRunProps.UniqueName, rPLTextRunProps.ActionInfo, position);
			string toolTip = renderingTextRun.ToolTip;
			if (!string.IsNullOrEmpty(toolTip))
			{
				m_context.RenderingReport.ToolTips.Add(new ReportToolTip(rPLTextRunProps.UniqueName, position, toolTip));
			}
		}

		public void DrawClippedTextRun(TextRun run, Paragraph paragraph, Win32DCSafeHandle hdc, float dpiX, FontCache fontCache, int x, int y, int baselineY, int lineHeight, Rectangle layoutRectangle, uint fontColorOverride, Rectangle clipRect)
		{
			if (!string.IsNullOrEmpty(run.Text))
			{
				Underline underline = null;
				if (run.UnderlineHeight > 0)
				{
					underline = new Underline(run, hdc, fontCache, layoutRectangle, x, baselineY, WritingMode);
				}
				int x2;
				int baselineY2;
				switch (WritingMode)
				{
				case RPLFormat.WritingModes.Horizontal:
					x2 = layoutRectangle.X + x;
					baselineY2 = layoutRectangle.Y + baselineY;
					break;
				case RPLFormat.WritingModes.Vertical:
					x2 = layoutRectangle.X + (layoutRectangle.Width - baselineY);
					baselineY2 = layoutRectangle.Y + x;
					break;
				case RPLFormat.WritingModes.Rotate270:
					x2 = layoutRectangle.X + baselineY;
					baselineY2 = layoutRectangle.Y + layoutRectangle.Height - x;
					break;
				default:
					throw new NotSupportedException();
				}
				TextBox.DrawClippedTextRun(run, hdc, fontCache, x2, baselineY2, fontColorOverride, clipRect, underline);
			}
		}

		internal override void ProcessRenderingElementContent(RPLElement rplElement, GdiContext context, RectangleF bounds)
		{
			RPLTextBox rPLTextBox = rplElement as RPLTextBox;
			RPLTextBoxPropsDef rPLTextBoxPropsDef = DefinitionProperties as RPLTextBoxPropsDef;
			RPLTextBoxProps rPLTextBoxProps = InstanceProperties as RPLTextBoxProps;
			m_textPosition = new RectangleF(base.Position.X, base.Position.Y, base.Position.Width, base.Position.Height);
			GdiContext.CalculateUsableReportItemRectangle(InstanceProperties, ref m_textPosition);
			if (TextPosition.Width <= 0f || TextPosition.Height <= 0f)
			{
				return;
			}
			m_richTextBox = new TextBox(this);
			if (m_paragraphs == null)
			{
				m_paragraphs = new List<RenderingParagraph>();
			}
			if (rPLTextBoxPropsDef.IsSimple)
			{
				RenderingParagraph renderingParagraph = new RenderingParagraph();
				renderingParagraph.Initialize(rPLTextBox, TextPosition);
				renderingParagraph.ProcessRenderingElementContent(rPLTextBox, context, TextPosition);
				m_paragraphs.Add(renderingParagraph);
			}
			else
			{
				for (RPLParagraph nextParagraph = rPLTextBox.GetNextParagraph(); nextParagraph != null; nextParagraph = rPLTextBox.GetNextParagraph())
				{
					RenderingParagraph renderingParagraph2 = new RenderingParagraph();
					renderingParagraph2.Initialize(nextParagraph, TextPosition);
					renderingParagraph2.ProcessRenderingElementContent(nextParagraph, context, TextPosition);
					m_paragraphs.Add(renderingParagraph2);
				}
			}
			if (rPLTextBoxProps.IsToggleParent)
			{
				GetToggleImage(context.GdiWriter, out Bitmap _);
				m_textPosition.X += m_toggleRectangleMM.Width + 2.2f;
				m_textPosition.Width -= m_toggleRectangleMM.Width + 2.2f;
				Action action = new ToggleAction(InstanceProperties.UniqueName, rPLTextBoxProps.Label, m_toggleRectangleMM, rPLTextBoxProps.ToggleState);
				RegisterAction(context, action);
			}
			if (rPLTextBoxPropsDef.CanSort)
			{
				GetSortImage(context.GdiWriter, out Bitmap _, out SortOrder nextOrder);
				m_textPosition.Width -= m_sortRectangleMM.Width + 2.2f;
				if (m_sortRectangleMM.Width > 0f)
				{
					Action action2 = new SortAction(InstanceProperties.UniqueName, rPLTextBoxProps.Label, m_sortRectangleMM, nextOrder);
					RegisterAction(context, action2);
				}
			}
			ProcessActions(context, InstanceProperties.UniqueName, ((RPLTextBoxProps)InstanceProperties).ActionInfo, TextPosition);
		}

		internal override void DrawContent(GdiContext context)
		{
			RPLTextBoxPropsDef rPLTextBoxPropsDef = DefinitionProperties as RPLTextBoxPropsDef;
			RPLTextBoxProps rPLTextBoxProps = InstanceProperties as RPLTextBoxProps;
			if (rPLTextBoxProps.IsToggleParent)
			{
				GetToggleImage(context.GdiWriter, out Bitmap image);
				if (m_toggleRectangleMM.Width > 0f)
				{
					DrawResourceImage(imageRectanglePX: new RectangleF(0f, 0f, image.Width, image.Height), context: context, image: image, itemRectangleMM: m_toggleRectangleMM);
				}
			}
			if (rPLTextBoxPropsDef.CanSort)
			{
				GetSortImage(context.GdiWriter, out Bitmap image2, out SortOrder _);
				if (m_sortRectangleMM.Width > 0f)
				{
					DrawResourceImage(imageRectanglePX: new RectangleF(0f, 0f, image2.Width, image2.Height), context: context, image: image2, itemRectangleMM: m_sortRectangleMM);
				}
			}
			if (TextPosition.Width <= 0f || TextPosition.Height <= 0f)
			{
				return;
			}
			RPLFormat.WritingModes writingMode = WritingMode;
			float contentHeight = 0f;
			float contentHeight2 = 0f;
			if (writingMode == RPLFormat.WritingModes.Horizontal)
			{
				contentHeight = rPLTextBoxProps.ContentHeight;
			}
			else
			{
				contentHeight2 = rPLTextBoxProps.ContentHeight;
			}
			List<Paragraph> list = null;
			List<RTSelectionHighlight> list2 = null;
			FlowContext flowContext = new FlowContext(TextPosition.Width, TextPosition.Height);
			bool flag = !rPLTextBoxPropsDef.CanGrow && !rPLTextBoxPropsDef.CanShrink;
			if (flag)
			{
				m_richTextBox.Paragraphs = new List<Paragraph>(m_paragraphs.Count);
				for (int i = 0; i < m_paragraphs.Count; i++)
				{
					RTSelectionHighlight searchHit = m_paragraphs[i].GetSearchHit(context);
					if (searchHit != null)
					{
						context.TextRunIndexHitStart = searchHit.SelectionStart.TextRunIndex;
						context.TextRunIndexHitEnd = searchHit.SelectionEnd.TextRunIndex;
					}
					m_paragraphs[i].DrawContent(context);
					m_richTextBox.Paragraphs.Add(m_paragraphs[i].RichParagraph);
					context.TextRunIndexHitStart = -1;
					context.TextRunIndexHitEnd = -1;
				}
				m_richTextBox.ScriptItemize();
				if (writingMode == RPLFormat.WritingModes.Horizontal)
				{
					TextBox.MeasureFullHeight(m_richTextBox, context.Graphics, context.FontCache, flowContext, out contentHeight);
				}
				else
				{
					TextBox.MeasureFullHeight(m_richTextBox, context.Graphics, context.FontCache, flowContext, out contentHeight2);
				}
				list = m_richTextBox.Paragraphs;
				m_richTextBox.Paragraphs = null;
			}
			if (writingMode == RPLFormat.WritingModes.Horizontal)
			{
				if (contentHeight + 0.001f > TextPosition.Height)
				{
					flowContext.LineLimit = false;
				}
			}
			else if (contentHeight2 + 0.001f > TextPosition.Width)
			{
				flowContext.LineLimit = false;
			}
			m_richTextBox.Paragraphs = new List<Paragraph>(1);
			bool flag2 = true;
			float num = 0f;
			float num2 = 0f;
			for (int j = 0; j < m_paragraphs.Count; j++)
			{
				if (writingMode == RPLFormat.WritingModes.Horizontal)
				{
					if (num > 0f)
					{
						if (num >= TextPosition.Height)
						{
							break;
						}
						flowContext.Height = TextPosition.Height - num;
					}
				}
				else if (num2 > 0f)
				{
					if (num2 >= TextPosition.Width)
					{
						break;
					}
					flowContext.Width = TextPosition.Width - num2;
				}
				RTSelectionHighlight searchHit2 = m_paragraphs[j].GetSearchHit(context);
				if (!flag)
				{
					if (searchHit2 != null)
					{
						context.TextRunIndexHitStart = searchHit2.SelectionStart.TextRunIndex;
						context.TextRunIndexHitEnd = searchHit2.SelectionEnd.TextRunIndex;
					}
					m_paragraphs[j].DrawContent(context);
					m_richTextBox.Paragraphs.Add(m_paragraphs[j].RichParagraph);
					context.TextRunIndexHitStart = -1;
					context.TextRunIndexHitEnd = -1;
					m_richTextBox.ScriptItemize();
				}
				else
				{
					m_richTextBox.Paragraphs.Add(list[0]);
				}
				if (searchHit2 != null)
				{
					searchHit2.SelectionStart.ParagraphIndex = 0;
					searchHit2.SelectionEnd.ParagraphIndex = 0;
					list2 = new List<RTSelectionHighlight>(0);
					list2.Add(searchHit2);
				}
				float height = 0f;
				List<Paragraph> rTParagraphs = LineBreaker.Flow(m_richTextBox, context.Graphics, context.FontCache, flowContext, keepLines: true, out height);
				RectangleF layoutParagraph = RectangleF.Empty;
				PointF offset = PointF.Empty;
				float delta = 0f;
				if (writingMode == RPLFormat.WritingModes.Horizontal)
				{
					AdjustParagraphLayout(contentHeight, height, num, flag2, writingMode, ref delta, ref layoutParagraph, ref offset);
				}
				else
				{
					AdjustParagraphLayout(contentHeight2, height, num2, flag2, writingMode, ref delta, ref layoutParagraph, ref offset);
				}
				m_context = context;
				m_paragraphs[j].TextPosition = layoutParagraph;
				RichTextRenderer richTextRenderer = new RichTextRenderer();
				try
				{
					richTextRenderer.SetTextbox(m_richTextBox);
					richTextRenderer.RTParagraphs = rTParagraphs;
					richTextRenderer.FontCache = context.FontCache;
					if (writingMode == RPLFormat.WritingModes.Horizontal)
					{
						RenderRichText(richTextRenderer, context.Graphics, TextPosition, offset, list2);
					}
					else
					{
						RenderRichText(richTextRenderer, context.Graphics, layoutParagraph, offset, list2);
					}
					richTextRenderer.FontCache = null;
				}
				finally
				{
					if (richTextRenderer != null)
					{
						richTextRenderer.Dispose();
						richTextRenderer = null;
					}
				}
				m_context = null;
				flowContext.Reset();
				if (writingMode == RPLFormat.WritingModes.Horizontal)
				{
					if (flag2)
					{
						num += delta;
						flag2 = false;
					}
					num += height;
				}
				else
				{
					if (flag2)
					{
						num2 += delta;
						flag2 = false;
					}
					num2 += height;
				}
				if (searchHit2 != null)
				{
					searchHit2.SelectionStart.ParagraphIndex = j;
					searchHit2.SelectionEnd.ParagraphIndex = j;
				}
				m_richTextBox.Paragraphs.RemoveAt(0);
				if (!flag)
				{
					m_paragraphs[j].RichParagraph = null;
				}
				else
				{
					list.RemoveAt(0);
				}
			}
			if (list != null && list.Count > 0)
			{
				list.Clear();
			}
			list = null;
		}

		[SecurityPermission(SecurityAction.Assert, UnmanagedCode = true)]
		private void RenderRichText(RichTextRenderer richTextRenderer, System.Drawing.Graphics graphics, RectangleF rectangle, PointF offset, List<RTSelectionHighlight> matches)
		{
			richTextRenderer.Render(graphics, rectangle, offset, matches, unitsInMM: true);
		}

		internal void Search(GdiContext context)
		{
			if (m_paragraphs != null)
			{
				for (int i = 0; i < m_paragraphs.Count; i++)
				{
					m_paragraphs[i].Search(context, i);
				}
			}
		}

		internal string GetText()
		{
			StringBuilder stringBuilder = new StringBuilder();
			bool flag = true;
			for (int i = 0; i < m_paragraphs.Count; i++)
			{
				string text = m_paragraphs[i].Text;
				if (flag)
				{
					if (!string.IsNullOrEmpty(text))
					{
						stringBuilder.Append(text);
						flag = false;
					}
				}
				else if (!string.IsNullOrEmpty(text))
				{
					stringBuilder.Append(Environment.NewLine);
					stringBuilder.Append(text);
				}
			}
			return stringBuilder.ToString();
		}

		private void DrawResourceImage(GdiContext context, Bitmap image, RectangleF itemRectangleMM, RectangleF imageRectanglePX)
		{
			InterpolationMode interpolationMode = context.Graphics.InterpolationMode;
			context.Graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
			SharedRenderer.DrawImage(context.Graphics, image, itemRectangleMM, imageRectanglePX);
			context.Graphics.InterpolationMode = interpolationMode;
		}

		private void GetSortImage(GdiWriter writer, out Bitmap image, out SortOrder nextOrder)
		{
			string key = null;
			nextOrder = SortOrder.Descending;
			switch (((RPLTextBoxProps)InstanceProperties).SortState)
			{
			case RPLFormat.SortOptions.None:
				nextOrder = SortOrder.Ascending;
				key = "unsorted";
				break;
			case RPLFormat.SortOptions.Ascending:
				key = "sortAsc";
				break;
			case RPLFormat.SortOptions.Descending:
				nextOrder = SortOrder.Ascending;
				key = "sortDesc";
				break;
			}
			image = GdiContext.ImageResources[key];
			float num = writer.ConvertToMillimeters(image.Width);
			float num2 = writer.ConvertToMillimeters(image.Height);
			if (m_sortRectangleMM == RectangleF.Empty)
			{
				float num3 = Math.Max(TextPosition.Right - num, TextPosition.X) - 0.34f;
				float num4 = TextPosition.Y + 0.34f;
				if (TextPosition.Height < num2 + 0.34f)
				{
					num2 = TextPosition.Height;
					num4 -= 0.34f;
				}
				if (TextPosition.Width < num + 0.34f)
				{
					num = TextPosition.Width;
					num3 += 0.34f;
				}
				m_sortRectangleMM = new RectangleF(num3, num4, num, num2);
			}
		}

		private void GetToggleImage(GdiWriter writer, out Bitmap image)
		{
			image = GdiContext.ImageResources[((RPLTextBoxProps)InstanceProperties).ToggleState ? "toggleMinus" : "togglePlus"];
			float num = writer.ConvertToMillimeters(image.Width);
			float num2 = writer.ConvertToMillimeters(image.Height);
			if (m_toggleRectangleMM == RectangleF.Empty)
			{
				float num3 = TextPosition.X + 1f;
				float num4 = TextPosition.Y + 1f;
				if (TextPosition.Height < num2 + 1f)
				{
					num2 = TextPosition.Height;
					num4 -= 1f;
				}
				if (TextPosition.Width < num + 1f)
				{
					num = TextPosition.Width;
					num3 -= 1f;
				}
				m_toggleRectangleMM = new RectangleF(num3, num4, num, num2);
			}
		}

		private void AdjustParagraphLayout(float totalContent, float newContent, float usedContent, bool firstParagraph, RPLFormat.WritingModes writingMode, ref float delta, ref RectangleF layoutParagraph, ref PointF offset)
		{
			_ = DefinitionProperties;
			RPLFormat.VerticalAlignments verticalAlign = VerticalAlign;
			if (firstParagraph)
			{
				bool flag = false;
				if ((writingMode != 0) ? (totalContent < TextPosition.Width) : (totalContent < TextPosition.Height))
				{
					switch (verticalAlign)
					{
					case RPLFormat.VerticalAlignments.Top:
						delta = 0f;
						break;
					case RPLFormat.VerticalAlignments.Bottom:
						delta = 0f;
						if (writingMode == RPLFormat.WritingModes.Horizontal)
						{
							delta = TextPosition.Height - totalContent;
						}
						else
						{
							delta = TextPosition.Width - totalContent;
						}
						if (delta < 0f)
						{
							delta = 0f;
						}
						break;
					default:
						delta = 0f;
						if (writingMode == RPLFormat.WritingModes.Horizontal)
						{
							delta = (TextPosition.Height - totalContent) / 2f;
						}
						else
						{
							delta = (TextPosition.Width - totalContent) / 2f;
						}
						if (delta < 0f)
						{
							delta = 0f;
						}
						break;
					}
				}
			}
			if (writingMode == RPLFormat.WritingModes.Horizontal)
			{
				layoutParagraph = new RectangleF(TextPosition.X, TextPosition.Y + usedContent + delta, TextPosition.Width, newContent);
				offset = new PointF(0f, usedContent + delta);
				return;
			}
			float num = newContent;
			float num2;
			if (writingMode == RPLFormat.WritingModes.Vertical)
			{
				num2 = TextPosition.Width - (usedContent + delta + newContent);
				if (num2 < 0f)
				{
					num += num2;
					num2 = 0f;
				}
			}
			else
			{
				num2 = usedContent + delta;
				if (num > TextPosition.Width)
				{
					num = TextPosition.Width;
				}
			}
			layoutParagraph = new RectangleF(TextPosition.X + num2, TextPosition.Y, num, TextPosition.Height);
			offset = new PointF(newContent, 0f);
		}
	}
}
