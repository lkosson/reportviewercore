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
	internal sealed class TextBox : PageItem, ITextBoxProps
	{
		internal enum CalcSize : byte
		{
			None,
			Done,
			Delay,
			LateCalc
		}

		internal sealed class TextBoxOffset : IStorable, IPersistable
		{
			private int m_paragraphIndex;

			private int m_textRunIndex;

			private int m_characterIndex;

			[StaticReference]
			internal static Declaration m_declaration = GetDeclaration();

			public int ParagraphIndex
			{
				get
				{
					return m_paragraphIndex;
				}
				set
				{
					m_paragraphIndex = value;
				}
			}

			public int TextRunIndex
			{
				get
				{
					return m_textRunIndex;
				}
				set
				{
					m_textRunIndex = value;
				}
			}

			public int CharacterIndex
			{
				get
				{
					return m_characterIndex;
				}
				set
				{
					m_characterIndex = value;
				}
			}

			public int Size => 12;

			public TextBoxOffset()
			{
			}

			public TextBoxOffset(int defaultValue)
			{
				Reset(defaultValue);
			}

			public void Reset(int defaultValue)
			{
				ParagraphIndex = defaultValue;
				TextRunIndex = defaultValue;
				CharacterIndex = defaultValue;
			}

			public void Set(TextBoxContext context)
			{
				ParagraphIndex = context.ParagraphIndex;
				TextRunIndex = context.TextRunIndex;
				CharacterIndex = context.TextRunCharacterIndex;
			}

			public void Serialize(IntermediateFormatWriter writer)
			{
				writer.RegisterDeclaration(m_declaration);
				while (writer.NextMember())
				{
					switch (writer.CurrentMember.MemberName)
					{
					case MemberName.ParagraphIndex:
						writer.Write(m_paragraphIndex);
						break;
					case MemberName.TextRunIndex:
						writer.Write(m_textRunIndex);
						break;
					case MemberName.CharacterIndex:
						writer.Write(m_characterIndex);
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
				while (reader.NextMember())
				{
					switch (reader.CurrentMember.MemberName)
					{
					case MemberName.ParagraphIndex:
						m_paragraphIndex = reader.ReadInt32();
						break;
					case MemberName.TextRunIndex:
						m_textRunIndex = reader.ReadInt32();
						break;
					case MemberName.CharacterIndex:
						m_characterIndex = reader.ReadInt32();
						break;
					default:
						RSTrace.RenderingTracer.Assert(condition: false, string.Empty);
						break;
					}
				}
			}

			public ObjectType GetObjectType()
			{
				return ObjectType.TextBoxOffset;
			}

			public void ResolveReferences(Dictionary<ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
			{
			}

			internal static Declaration GetDeclaration()
			{
				if (m_declaration == null)
				{
					List<MemberInfo> list = new List<MemberInfo>();
					list.Add(new MemberInfo(MemberName.ParagraphIndex, Token.Int32));
					list.Add(new MemberInfo(MemberName.TextRunIndex, Token.Int32));
					list.Add(new MemberInfo(MemberName.CharacterIndex, Token.Int32));
					return new Declaration(ObjectType.TextBoxOffset, ObjectType.None, list);
				}
				return m_declaration;
			}
		}

		private CalcSize m_calcSizeState;

		private double m_padHorizontal;

		private double m_padVertical;

		private double m_padTop;

		private List<Paragraph> m_paragraphs = new List<Paragraph>();

		[StaticReference]
		private Microsoft.ReportingServices.Rendering.RichText.TextBox m_richTextBox;

		private TextBoxState m_textBoxState = new TextBoxState();

		private TextBoxOffset m_pageStartOffset = new TextBoxOffset(0);

		private TextBoxOffset m_pageEndOffset = new TextBoxOffset(-1);

		private TextBoxOffset m_nextPageStartOffset = new TextBoxOffset(-1);

		private double m_contentOffset;

		private double m_contentBottom;

		private float m_contentHeight;

		[StaticReference]
		private static Declaration m_declaration = GetDeclaration();

		internal CalcSize CalcSizeState
		{
			get
			{
				return m_calcSizeState;
			}
			set
			{
				m_calcSizeState = value;
			}
		}

		internal override string SourceID
		{
			get
			{
				Microsoft.ReportingServices.OnDemandReportRendering.TextBox textBox = m_source as Microsoft.ReportingServices.OnDemandReportRendering.TextBox;
				if (textBox != null)
				{
					if (m_textBoxState.SpanPages && textBox.IsSimple && !textBox.FormattedValueExpressionBased)
					{
						return m_source.ID + "_NV";
					}
					return m_source.ID;
				}
				return m_source.ID + "_NR";
			}
		}

		public override int Size => base.Size + 1 + 8 + 8 + 8 + 1 + Microsoft.ReportingServices.OnDemandProcessing.Scalability.ItemSizes.SizeOf(m_paragraphs) + 8 + 8 + Microsoft.ReportingServices.OnDemandProcessing.Scalability.ItemSizes.SizeOf(m_pageStartOffset) + Microsoft.ReportingServices.OnDemandProcessing.Scalability.ItemSizes.SizeOf(m_pageEndOffset) + Microsoft.ReportingServices.OnDemandProcessing.Scalability.ItemSizes.SizeOf(m_nextPageStartOffset) + Microsoft.ReportingServices.OnDemandProcessing.Scalability.ItemSizes.ReferenceSize;

		public RPLFormat.TextAlignments DefaultAlignment => m_textBoxState.DefaultTextAlign;

		public RPLFormat.Directions Direction => m_textBoxState.Direction;

		public RPLFormat.WritingModes WritingMode => m_textBoxState.WritingMode;

		public bool VerticalText => m_textBoxState.VerticalText;

		public bool HorizontalText => m_textBoxState.HorizontalText;

		public Color BackgroundColor => Color.Empty;

		public bool CanGrow
		{
			get
			{
				Microsoft.ReportingServices.OnDemandReportRendering.TextBox textBox = m_source as Microsoft.ReportingServices.OnDemandReportRendering.TextBox;
				if (textBox != null)
				{
					if (m_textBoxState.SpanPages)
					{
						return false;
					}
					return textBox.CanGrow;
				}
				return false;
			}
		}

		internal TextBox()
		{
		}

		private TextBox(ReportItem source, PageContext pageContext)
			: base(source)
		{
			m_itemPageSizes = new ItemSizes(source);
			InitParagraphs();
		}

		internal TextBox(Microsoft.ReportingServices.OnDemandReportRendering.TextBox source, PageContext pageContext)
			: this((ReportItem)source, pageContext)
		{
			base.KeepTogetherVertical = source.KeepTogether;
			base.KeepTogetherHorizontal = source.KeepTogether;
			base.UnresolvedKTH = (base.UnresolvedKTV = source.KeepTogether);
			if (source.HideDuplicates)
			{
				base.Duplicate = ((TextBoxInstance)source.Instance).Duplicate;
				if (pageContext.TextBoxDuplicates == null)
				{
					pageContext.TextBoxDuplicates = new Hashtable();
					pageContext.TextBoxDuplicates.Add(source.ID, null);
					base.Duplicate = false;
				}
				else if (!pageContext.TextBoxDuplicates.ContainsKey(source.ID))
				{
					pageContext.TextBoxDuplicates.Add(source.ID, null);
					base.Duplicate = false;
				}
			}
			CreateFullStyle(pageContext);
		}

		internal TextBox(DataRegion source, PageContext pageContext)
			: this((ReportItem)source, pageContext)
		{
			if (source is Microsoft.ReportingServices.OnDemandReportRendering.Tablix)
			{
				Microsoft.ReportingServices.OnDemandReportRendering.Tablix tablix = (Microsoft.ReportingServices.OnDemandReportRendering.Tablix)source;
				base.KeepTogetherVertical = tablix.KeepTogether;
				base.KeepTogetherHorizontal = tablix.KeepTogether;
				base.UnresolvedKTH = (base.UnresolvedKTV = tablix.KeepTogether);
			}
			else
			{
				base.KeepTogetherVertical = true;
				base.KeepTogetherHorizontal = true;
				base.UnresolvedKTH = (base.UnresolvedKTV = true);
			}
			CreateFullStyle(pageContext);
		}

		internal TextBox(Microsoft.ReportingServices.OnDemandReportRendering.SubReport source, PageContext pageContext)
			: this((ReportItem)source, pageContext)
		{
			base.KeepTogetherVertical = source.KeepTogether;
			base.KeepTogetherHorizontal = source.KeepTogether;
			base.UnresolvedKTH = (base.UnresolvedKTV = source.KeepTogether);
			CreateFullStyle(pageContext);
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(m_declaration);
			IScalabilityCache scalabilityCache = writer.PersistenceHelper as IScalabilityCache;
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.CalcSizeState:
					writer.Write((byte)m_calcSizeState);
					break;
				case MemberName.HorizontalPadding:
					writer.Write(m_padHorizontal);
					break;
				case MemberName.VerticalPadding:
					writer.Write(m_padVertical);
					break;
				case MemberName.TopPadding:
					writer.Write(m_padTop);
					break;
				case MemberName.Paragraphs:
					writer.Write(m_paragraphs);
					break;
				case MemberName.TextBox:
					writer.Write(scalabilityCache.StoreStaticReference(m_richTextBox));
					break;
				case MemberName.State:
					writer.Write(m_textBoxState.State);
					break;
				case MemberName.ContentOffset:
					writer.Write(m_contentOffset);
					break;
				case MemberName.ContentBottom:
					writer.Write(m_contentBottom);
					break;
				case MemberName.ContentHeight:
					writer.Write(m_contentHeight);
					break;
				case MemberName.PageStartOffset:
					writer.Write(m_pageStartOffset);
					break;
				case MemberName.PageEndOffset:
					writer.Write(m_pageEndOffset);
					break;
				case MemberName.NextPageStartOffset:
					writer.Write(m_nextPageStartOffset);
					break;
				default:
					RSTrace.RenderingTracer.Assert(condition: false, string.Empty);
					break;
				}
			}
		}

		public override void Deserialize(IntermediateFormatReader reader)
		{
			base.Deserialize(reader);
			reader.RegisterDeclaration(m_declaration);
			IScalabilityCache scalabilityCache = reader.PersistenceHelper as IScalabilityCache;
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.CalcSizeState:
					m_calcSizeState = (CalcSize)reader.ReadByte();
					break;
				case MemberName.HorizontalPadding:
					m_padHorizontal = reader.ReadDouble();
					break;
				case MemberName.VerticalPadding:
					m_padVertical = reader.ReadDouble();
					break;
				case MemberName.TopPadding:
					m_padTop = reader.ReadDouble();
					break;
				case MemberName.Paragraphs:
					m_paragraphs = reader.ReadGenericListOfRIFObjects<Paragraph>();
					break;
				case MemberName.TextBox:
					m_richTextBox = (Microsoft.ReportingServices.Rendering.RichText.TextBox)scalabilityCache.FetchStaticReference(reader.ReadInt32());
					break;
				case MemberName.State:
					m_textBoxState.State = reader.ReadByte();
					break;
				case MemberName.ContentBottom:
					m_contentBottom = reader.ReadDouble();
					break;
				case MemberName.ContentOffset:
					m_contentOffset = reader.ReadDouble();
					break;
				case MemberName.ContentHeight:
					m_contentHeight = reader.ReadSingle();
					break;
				case MemberName.PageStartOffset:
					m_pageStartOffset = reader.ReadRIFObject<TextBoxOffset>();
					break;
				case MemberName.PageEndOffset:
					m_pageEndOffset = reader.ReadRIFObject<TextBoxOffset>();
					break;
				case MemberName.NextPageStartOffset:
					m_nextPageStartOffset = reader.ReadRIFObject<TextBoxOffset>();
					break;
				default:
					RSTrace.RenderingTracer.Assert(condition: false, string.Empty);
					break;
				}
			}
		}

		public override ObjectType GetObjectType()
		{
			return ObjectType.TextBox;
		}

		internal new static Declaration GetDeclaration()
		{
			if (m_declaration == null)
			{
				List<MemberInfo> list = new List<MemberInfo>();
				list.Add(new MemberInfo(MemberName.CalcSizeState, Token.Byte));
				list.Add(new MemberInfo(MemberName.HorizontalPadding, Token.Double));
				list.Add(new MemberInfo(MemberName.VerticalPadding, Token.Double));
				list.Add(new MemberInfo(MemberName.TopPadding, Token.Double));
				list.Add(new MemberInfo(MemberName.Paragraphs, ObjectType.RIFObjectList, ObjectType.Paragraph));
				list.Add(new MemberInfo(MemberName.TextBox, Token.Int32));
				list.Add(new MemberInfo(MemberName.State, Token.Byte));
				list.Add(new MemberInfo(MemberName.ContentOffset, Token.Double));
				list.Add(new MemberInfo(MemberName.ContentBottom, Token.Double));
				list.Add(new MemberInfo(MemberName.ContentHeight, Token.Single));
				list.Add(new MemberInfo(MemberName.PageStartOffset, ObjectType.TextBoxOffset));
				list.Add(new MemberInfo(MemberName.PageEndOffset, ObjectType.TextBoxOffset));
				list.Add(new MemberInfo(MemberName.NextPageStartOffset, ObjectType.TextBoxOffset));
				return new Declaration(ObjectType.TextBox, ObjectType.PageItem, list);
			}
			return m_declaration;
		}

		private bool GetAlignmentRight(Style style, StyleInstance styleInstance)
		{
			Microsoft.ReportingServices.OnDemandReportRendering.TextBox textBox = m_source as Microsoft.ReportingServices.OnDemandReportRendering.TextBox;
			if (textBox == null || !textBox.IsSimple)
			{
				return false;
			}
			RPLFormat.TextAlignments textAlignments = RPLFormat.TextAlignments.General;
			_ = textBox.Paragraphs[0];
			byte? nonCompiledEnumProp = Utility.GetNonCompiledEnumProp(25, StyleAttributeNames.TextAlign, style, styleInstance);
			if (nonCompiledEnumProp.HasValue)
			{
				textAlignments = (RPLFormat.TextAlignments)nonCompiledEnumProp.Value;
			}
			switch (textAlignments)
			{
			case RPLFormat.TextAlignments.General:
			{
				TypeCode typeCode = textBox.SharedTypeCode;
				if (typeCode == TypeCode.Object)
				{
					typeCode = ((TextBoxInstance)textBox.Instance).TypeCode;
				}
				bool result = false;
				if ((uint)(typeCode - 5) <= 11u)
				{
					result = true;
				}
				return result;
			}
			case RPLFormat.TextAlignments.Right:
				return true;
			default:
				return false;
			}
		}

		private void CreateFullStyle(PageContext pageContext)
		{
			PaddingsStyle paddingsStyle = null;
			if (pageContext.ItemPaddingsStyle != null)
			{
				paddingsStyle = (PaddingsStyle)pageContext.ItemPaddingsStyle[m_source.ID];
			}
			if (paddingsStyle != null)
			{
				paddingsStyle.GetPaddingValues(m_source, out m_padVertical, out m_padHorizontal, out m_padTop);
			}
			else
			{
				PaddingsStyle.CreatePaddingsStyle(pageContext, m_source, out m_padVertical, out m_padHorizontal, out m_padTop);
			}
		}

		private void CalculateVerticalSize(PageContext pageContext)
		{
			Microsoft.ReportingServices.OnDemandReportRendering.TextBox textBox = m_source as Microsoft.ReportingServices.OnDemandReportRendering.TextBox;
			if (textBox == null)
			{
				return;
			}
			bool flag = !textBox.CanShrink;
			float contentHeight = 0f;
			if (textBox.CanGrow || textBox.CanShrink)
			{
				if (m_richTextBox == null)
				{
					m_richTextBox = GetRichTextBox();
				}
				if (m_richTextBox != null)
				{
					double num = m_itemPageSizes.Width - m_padHorizontal;
					if (num > 0.0)
					{
						double num2 = m_itemPageSizes.Height - m_padVertical;
						if (textBox.CanGrow || num2 > 0.0)
						{
							FlowContext flowContext = new FlowContext((float)num, float.MaxValue);
							flowContext.Updatable = false;
							if (!textBox.CanShrink)
							{
								flowContext.Height = (float)num2;
							}
							double num3 = (double)pageContext.Common.MeasureFullTextBoxHeight(m_richTextBox, flowContext, out contentHeight) - (m_itemPageSizes.Height - m_padVertical);
							flag = (HorizontalText ? ((num3 < 0.0 && !textBox.CanShrink) ? true : false) : (((double)contentHeight - num < 0.0) ? true : false));
							if ((num3 > 0.0 && textBox.CanGrow) || (num3 < 0.0 && textBox.CanShrink))
							{
								m_itemPageSizes.AdjustHeightTo(m_itemPageSizes.Height + num3 + 0.0001);
							}
						}
					}
				}
				else if (textBox.CanShrink)
				{
					m_itemPageSizes.AdjustHeightTo(0.0);
				}
			}
			if (flag && m_textBoxState.VerticalAlignment != 0)
			{
				if (m_richTextBox == null)
				{
					m_richTextBox = GetRichTextBox();
				}
				if (m_richTextBox != null)
				{
					double num4 = m_itemPageSizes.Width - m_padHorizontal;
					if (contentHeight == 0f)
					{
						FlowContext flowContext2 = new FlowContext((float)num4, (float)(m_itemPageSizes.Height - m_padVertical));
						flowContext2.Updatable = false;
						pageContext.Common.MeasureFullTextBoxHeight(m_richTextBox, flowContext2, out contentHeight);
					}
				}
			}
			m_contentHeight = contentHeight;
		}

		protected override void DetermineVerticalSize(PageContext pageContext, double topInParentSystem, double bottomInParentSystem, List<PageItem> ancestors, ref bool anyAncestorHasKT, bool hasUnpinnedAncestors)
		{
			if (m_calcSizeState != CalcSize.Done && m_calcSizeState != CalcSize.Delay)
			{
				CalculateVerticalSize(pageContext);
				m_calcSizeState = CalcSize.Done;
			}
		}

		protected override void DetermineHorizontalSize(PageContext pageContext, double leftInParentSystem, double rightInParentSystem, List<PageItem> ancestors, bool anyAncestorHasKT, bool hasUnpinnedAncestors)
		{
			if (m_textBoxState.SpanPages && m_textBoxState.ResetHorizontalState && new RoundedDouble(base.ItemPageSizes.Left) >= leftInParentSystem)
			{
				UpdateState();
				m_textBoxState.ResetHorizontalState = false;
			}
		}

		private void CalculateOffsetFromHeight()
		{
			if (!(m_contentHeight > 0f) || m_textBoxState.SpanPages)
			{
				return;
			}
			RPLFormat.VerticalAlignments verticalAlignment = m_textBoxState.VerticalAlignment;
			if (verticalAlignment == RPLFormat.VerticalAlignments.Top)
			{
				return;
			}
			bool horizontalText = HorizontalText;
			if (!((double)m_contentHeight < m_itemPageSizes.Height && horizontalText) && (!((double)m_contentHeight < m_itemPageSizes.Width) || horizontalText))
			{
				return;
			}
			float contentHeight = m_contentHeight;
			double num = m_itemPageSizes.Width - m_padHorizontal;
			double num2 = 0.0;
			num2 = ((!horizontalText) ? num : (m_itemPageSizes.Height - m_padVertical));
			if (num2 > (double)contentHeight)
			{
				switch (verticalAlignment)
				{
				case RPLFormat.VerticalAlignments.Bottom:
					m_contentOffset = num2 - (double)contentHeight;
					break;
				case RPLFormat.VerticalAlignments.Middle:
					m_contentOffset = (num2 - (double)contentHeight) / 2.0;
					break;
				}
			}
		}

		internal override bool AddToPage(RPLWriter rplWriter, PageContext pageContext, double pageLeft, double pageTop, double pageRight, double pageBottom, RepeatState repeatState)
		{
			if (HitsCurrentPage(pageLeft, pageTop, pageRight, pageBottom))
			{
				bool flag = false;
				FlowContext flowContext = null;
				base.SplitsVerticalPage = false;
				CalculateOffsetFromHeight();
				RoundedDouble roundedDouble = new RoundedDouble(base.ItemPageSizes.Left);
				if (roundedDouble >= pageLeft)
				{
					double num = base.ItemPageSizes.Bottom - (m_padVertical - m_padTop);
					double num2 = 0.0;
					bool flag2 = true;
					bool flag3 = false;
					roundedDouble.Value = base.ItemPageSizes.Top;
					if (roundedDouble < pageTop)
					{
						flag3 = true;
						base.SplitsVerticalPage = true;
						roundedDouble.Value = num;
						if (roundedDouble > pageBottom)
						{
							flag2 = false;
							num2 = num - pageBottom;
							m_textBoxState.ResetHorizontalState = true;
						}
					}
					else
					{
						roundedDouble.Value = num;
						if (roundedDouble > pageBottom)
						{
							flag3 = true;
							m_textBoxState.SpanPages = true;
							m_textBoxState.ResetHorizontalState = true;
							flag2 = false;
							num2 = num - pageBottom;
						}
					}
					if (flag3)
					{
						m_rplItemState |= 64;
						double num3 = Math.Min(pageBottom, num);
						double num4 = m_itemPageSizes.Width - m_padHorizontal;
						double num5 = 0.0;
						num5 = ((!VerticalText) ? (num3 - (base.ItemPageSizes.Top + m_padTop)) : (num3 - Math.Max(pageTop, base.ItemPageSizes.Top + m_padTop)));
						if (num5 <= 0.0 || num4 <= 0.0)
						{
							m_pageEndOffset.ParagraphIndex = 0;
							m_contentBottom = 0.0;
						}
						else if (HorizontalText && num5 <= m_contentOffset)
						{
							m_contentBottom = m_contentOffset;
							m_pageEndOffset.ParagraphIndex = 0;
						}
						else if (VerticalText && num4 <= m_contentOffset)
						{
							m_contentBottom = m_contentOffset - num4;
							m_pageEndOffset.ParagraphIndex = 0;
						}
						else
						{
							Microsoft.ReportingServices.OnDemandReportRendering.TextBox textBox = m_source as Microsoft.ReportingServices.OnDemandReportRendering.TextBox;
							if (m_richTextBox == null)
							{
								m_richTextBox = GetRichTextBox();
							}
							flowContext = new FlowContext((float)num4, (float)num5, 0, m_pageStartOffset.TextRunIndex, m_pageStartOffset.CharacterIndex);
							flowContext.Updatable = true;
							flowContext.ContentOffset = (float)m_contentOffset;
							if (flag2)
							{
								flowContext.LineLimit = false;
							}
							double num6 = 0.0;
							if (m_richTextBox != null)
							{
								num6 = pageContext.Common.MeasureTextBoxHeight(m_richTextBox, flowContext);
							}
							if (flowContext.OmittedLineHeight > 0f)
							{
								if (VerticalText)
								{
									if ((double)flowContext.OmittedLineHeight > num4)
									{
										flowContext = new FlowContext((float)num4, (float)num5, 0, m_pageStartOffset.TextRunIndex, m_pageStartOffset.CharacterIndex);
										flowContext.Updatable = true;
										flowContext.ContentOffset = (float)m_contentOffset;
										flowContext.LineLimit = false;
										num6 = pageContext.Common.MeasureTextBoxHeight(m_richTextBox, flowContext);
									}
								}
								else if ((double)flowContext.OmittedLineHeight > pageContext.ColumnHeight || ((textBox == null || !textBox.CanGrow) && (double)flowContext.OmittedLineHeight > num2) || pageContext.Common.InHeaderFooter)
								{
									flowContext = new FlowContext((float)num4, (float)num5, 0, m_pageStartOffset.TextRunIndex, m_pageStartOffset.CharacterIndex);
									flowContext.Updatable = true;
									flowContext.ContentOffset = (float)m_contentOffset;
									flowContext.LineLimit = false;
									num6 = pageContext.Common.MeasureTextBoxHeight(m_richTextBox, flowContext);
								}
							}
							if (HorizontalText)
							{
								m_contentBottom = flowContext.ContentOffset;
								if (flowContext.OmittedLineHeight > 0f)
								{
									double num7 = num5 - (num6 - (double)flowContext.OmittedLineHeight);
									m_contentBottom += num7;
									if (!flowContext.AtEndOfTextBox && textBox != null && textBox.CanGrow && num7 > 0.0)
									{
										base.ItemPageSizes.AdjustHeightTo(base.ItemPageSizes.Height + num7);
									}
								}
							}
							else
							{
								m_contentBottom = (double)flowContext.ContentOffset - num4;
								if (flowContext.OmittedLineHeight > 0f)
								{
									m_contentBottom += num4 - (num6 - (double)flowContext.OmittedLineHeight);
								}
							}
							m_pageEndOffset.Set(flowContext.Context);
							TextBoxContext textBoxContext = flowContext.ClipContext;
							if (textBoxContext == null)
							{
								textBoxContext = flowContext.Context;
							}
							m_nextPageStartOffset.Set(textBoxContext);
						}
					}
					if (repeatState == RepeatState.None)
					{
						roundedDouble.Value = base.ItemPageSizes.Bottom;
						if (roundedDouble <= pageBottom)
						{
							roundedDouble.Value = base.ItemPageSizes.Right;
							if (roundedDouble <= pageRight)
							{
								flag = true;
							}
						}
					}
				}
				WriteStartItemToStream(rplWriter, pageContext);
				if (flag)
				{
					m_richTextBox = null;
				}
				RegisterTextBoxes(rplWriter, pageContext);
				return true;
			}
			return false;
		}

		internal override void ResetHorizontal(bool spanPages, double? width)
		{
			base.ResetHorizontal(spanPages, width);
			if (spanPages)
			{
				UpdateState();
			}
			else
			{
				m_pageStartOffset.Reset(0);
				m_nextPageStartOffset.Reset(-1);
				m_pageEndOffset.Reset(-1);
			}
			m_textBoxState.ResetHorizontalState = false;
		}

		private void UpdateState()
		{
			int paragraphIndex = m_nextPageStartOffset.ParagraphIndex;
			if (paragraphIndex > 0)
			{
				m_richTextBox.Paragraphs.RemoveRange(0, paragraphIndex);
				m_paragraphs.RemoveRange(0, paragraphIndex);
			}
			int textRunIndex = m_nextPageStartOffset.TextRunIndex;
			if (textRunIndex > -1)
			{
				m_pageStartOffset.TextRunIndex = textRunIndex;
			}
			int characterIndex = m_nextPageStartOffset.CharacterIndex;
			if (characterIndex > -1)
			{
				m_pageStartOffset.CharacterIndex = characterIndex;
			}
			if (m_paragraphs.Count > 0 && paragraphIndex > -1)
			{
				Paragraph paragraph = m_paragraphs[0];
				if (textRunIndex > 0 || characterIndex > 0)
				{
					paragraph.FirstLine = false;
					m_richTextBox.Paragraphs[0].Updated = true;
				}
				if (textRunIndex > 0)
				{
					int textRunIndex2 = m_nextPageStartOffset.TextRunIndex;
					int num;
					for (num = 0; num < textRunIndex2; num++)
					{
						TextRun textRun = paragraph.TextRuns[0];
						int num2 = num;
						num += textRun.SplitIndices.Count;
						if (num < textRunIndex2)
						{
							paragraph.TextRuns.RemoveAt(0);
							m_richTextBox.Paragraphs[0].Runs.RemoveRange(0, textRun.SplitIndices.Count + 1);
						}
						else if (textRunIndex2 > num2)
						{
							m_richTextBox.Paragraphs[0].Runs.RemoveRange(0, textRunIndex2 - num2);
							textRun.ClearTo(textRunIndex2 - num2 - 1);
						}
						m_pageStartOffset.TextRunIndex = 0;
					}
				}
			}
			m_nextPageStartOffset.Reset(-1);
			m_pageEndOffset.Reset(-1);
			m_contentOffset = m_contentBottom;
		}

		internal override bool ResolveDuplicates(PageContext pageContext, double topInParentSystem, PageItem[] siblings, bool recalculate)
		{
			if (base.SplitsVerticalPage || AboveCurrentVerticalPage(topInParentSystem))
			{
				return false;
			}
			base.ResolveDuplicates(pageContext, topInParentSystem, siblings, recalculate);
			Microsoft.ReportingServices.OnDemandReportRendering.TextBox textBox = m_source as Microsoft.ReportingServices.OnDemandReportRendering.TextBox;
			if (textBox != null && textBox.HideDuplicates)
			{
				bool flag = false;
				if (pageContext.TextBoxDuplicates == null)
				{
					flag = true;
					pageContext.TextBoxDuplicates = new Hashtable();
				}
				else if (!pageContext.TextBoxDuplicates.ContainsKey(textBox.ID))
				{
					flag = true;
				}
				if (flag)
				{
					pageContext.TextBoxDuplicates.Add(textBox.ID, null);
					base.Duplicate = false;
					if ((textBox.CanGrow || textBox.CanShrink) && CalcSizeState == CalcSize.Done)
					{
						CalculateVerticalSize(pageContext);
						return true;
					}
				}
			}
			return false;
		}

		private object GetOriginalValue(PageContext pageContext)
		{
			object originalValue = null;
			if (m_nonSharedOffset > -1)
			{
				ReadNonSharedValuesFromCache(pageContext, out string value, out TypeCode _, out originalValue);
				if (originalValue == null)
				{
					originalValue = value;
				}
			}
			else
			{
				originalValue = ((TextBoxInstance)m_source.Instance).OriginalValue;
			}
			return originalValue;
		}

		internal override void RegisterTextBoxes(RPLWriter rplWriter, PageContext pageContext)
		{
			if (rplWriter == null || pageContext.Common.InSubReport || !pageContext.EvaluatePageHeaderFooter)
			{
				return;
			}
			Microsoft.ReportingServices.OnDemandReportRendering.TextBox textBox = m_source as Microsoft.ReportingServices.OnDemandReportRendering.TextBox;
			if (textBox == null)
			{
				return;
			}
			ReportStringProperty reportStringProperty = null;
			if (textBox.IsSimple)
			{
				reportStringProperty = textBox.Paragraphs[0].TextRuns[0].Value;
			}
			if (reportStringProperty == null || reportStringProperty.IsExpression)
			{
				if (rplWriter.DelayedTBLevels == 0)
				{
					pageContext.AddTextBox(m_source.Name, GetOriginalValue(pageContext));
				}
				else
				{
					rplWriter.AddTextBox(m_source.Name, GetOriginalValue(pageContext));
				}
			}
			else if (rplWriter.DelayedTBLevels == 0)
			{
				if (textBox.SharedTypeCode == TypeCode.String)
				{
					pageContext.AddTextBox(m_source.Name, reportStringProperty.Value);
				}
				else
				{
					pageContext.AddTextBox(m_source.Name, ((TextBoxInstance)m_source.Instance).OriginalValue);
				}
			}
			else if (textBox.SharedTypeCode == TypeCode.String)
			{
				rplWriter.AddTextBox(m_source.Name, reportStringProperty.Value);
			}
			else
			{
				rplWriter.AddTextBox(m_source.Name, ((TextBoxInstance)m_source.Instance).OriginalValue);
			}
		}

		internal override void CacheNonSharedProperties(PageContext pageContext)
		{
			if (!pageContext.CacheNonSharedProps)
			{
				return;
			}
			base.CacheNonSharedProperties(pageContext);
			BinaryWriter propertyCacheWriter = pageContext.PropertyCacheWriter;
			if (pageContext.PropertyCacheState != 0)
			{
				m_nonSharedOffset = propertyCacheWriter.BaseStream.Position;
			}
			Microsoft.ReportingServices.OnDemandReportRendering.TextBox textBox = m_source as Microsoft.ReportingServices.OnDemandReportRendering.TextBox;
			if (textBox != null && (!textBox.IsSimple || textBox.FormattedValueExpressionBased) && pageContext.EvaluatePageHeaderFooter)
			{
				TextBoxInstance textBoxInstance = (TextBoxInstance)m_source.Instance;
				object originalValue = textBoxInstance.OriginalValue;
				if (originalValue != null)
				{
					bool changed = false;
					TypeCode typeCode = GetTypeCode(textBox.SharedTypeCode, textBoxInstance, ref changed);
					propertyCacheWriter.Write((byte)33);
					propertyCacheWriter.Write((byte)typeCode);
					WriteOriginalValue(propertyCacheWriter, typeCode, originalValue);
				}
			}
			propertyCacheWriter.Write(byte.MaxValue);
		}

		internal void ReadNonSharedValuesFromCache(PageContext pageContext, out string value, out TypeCode typeCode, out object originalValue)
		{
			if (pageContext.PropertyCacheState == PageContext.CacheState.RPLStream)
			{
				pageContext.PropertyCacheReader.BaseStream.Seek(m_nonSharedOffset + 16, SeekOrigin.Begin);
			}
			else
			{
				pageContext.PropertyCacheReader.BaseStream.Seek(m_nonSharedOffset, SeekOrigin.Begin);
			}
			ReadNonSharedValuesFromCache(pageContext.PropertyCacheReader, out value, out typeCode, out originalValue);
		}

		internal void ReadNonSharedValuesFromCache(BinaryReader reader, out string value, out TypeCode typeCode, out object originalValue)
		{
			value = null;
			originalValue = null;
			typeCode = TypeCode.String;
			Microsoft.ReportingServices.OnDemandReportRendering.TextBox textBox = m_source as Microsoft.ReportingServices.OnDemandReportRendering.TextBox;
			if (textBox != null)
			{
				typeCode = textBox.SharedTypeCode;
			}
			byte b = reader.ReadByte();
			while (true)
			{
				switch (b)
				{
				case byte.MaxValue:
					return;
				case 27:
					value = reader.ReadString();
					break;
				case 33:
					typeCode = (TypeCode)reader.ReadByte();
					break;
				case 34:
					switch (typeCode)
					{
					case TypeCode.Char:
						originalValue = reader.ReadChar();
						break;
					case TypeCode.String:
						originalValue = reader.ReadString();
						break;
					case TypeCode.Boolean:
						originalValue = reader.ReadBoolean();
						break;
					case TypeCode.Byte:
						originalValue = reader.ReadByte();
						break;
					case TypeCode.Int16:
						originalValue = reader.ReadInt16();
						break;
					case TypeCode.Int32:
						originalValue = reader.ReadInt32();
						break;
					case TypeCode.Int64:
						originalValue = reader.ReadInt64();
						break;
					case TypeCode.Single:
						originalValue = reader.ReadSingle();
						break;
					case TypeCode.Decimal:
						originalValue = reader.ReadDecimal();
						break;
					case TypeCode.Double:
						originalValue = reader.ReadDouble();
						break;
					case TypeCode.DateTime:
						originalValue = DateTime.FromBinary(reader.ReadInt64());
						break;
					default:
						typeCode = TypeCode.String;
						originalValue = reader.ReadString();
						break;
					}
					break;
				}
				b = reader.ReadByte();
			}
		}

		internal override void CopyCachedData(RPLWriter rplWriter, PageContext pageContext)
		{
			BinaryReader propertyCacheReader = pageContext.PropertyCacheReader;
			propertyCacheReader.BaseStream.Seek(m_nonSharedOffset, SeekOrigin.Begin);
			long num = propertyCacheReader.ReadInt64();
			long num2 = propertyCacheReader.ReadInt64();
			if (num2 == m_nonSharedOffset)
			{
				propertyCacheReader.BaseStream.Seek(num, SeekOrigin.Begin);
				CopyData(propertyCacheReader, rplWriter, m_nonSharedOffset - num - 1);
			}
			else
			{
				CopyDataAndResolve(rplWriter, pageContext, num, num2, ignoreDelimiter: true);
			}
			Microsoft.ReportingServices.OnDemandReportRendering.TextBox textBox = m_source as Microsoft.ReportingServices.OnDemandReportRendering.TextBox;
			bool flag = false;
			bool flag2 = false;
			if (textBox != null)
			{
				if (textBox.IsSimple && !base.Duplicate)
				{
					if (textBox.FormattedValueExpressionBased)
					{
						flag = true;
					}
					else if (textBox.HideDuplicates || m_textBoxState.SpanPages)
					{
						flag2 = true;
						flag = true;
					}
				}
			}
			else
			{
				flag = true;
			}
			StyleWriterStream styleWriterStream = new StyleWriterStream(rplWriter.BinaryWriter);
			if (flag)
			{
				string value = CalculateSimpleValue(!flag2, pageContext);
				styleWriterStream.WriteNotNull(27, value);
			}
			if (m_contentOffset > 0.0)
			{
				styleWriterStream.Write(37, (float)m_contentOffset);
			}
			rplWriter.BinaryWriter.Write(byte.MaxValue);
		}

		internal override void WriteStartItemToStream(RPLWriter rplWriter, PageContext pageContext)
		{
			if (rplWriter == null)
			{
				return;
			}
			BinaryWriter binaryWriter = rplWriter.BinaryWriter;
			rplWriter.RegisterCacheRichData(m_richTextBox != null);
			Microsoft.ReportingServices.OnDemandReportRendering.TextBox textBox = m_source as Microsoft.ReportingServices.OnDemandReportRendering.TextBox;
			if ((textBox == null || textBox.IsSimple) && rplWriter.PageParagraphsItemizedData != null)
			{
				pageContext.ParagraphItemizedData = new List<TextRunItemizedData>();
			}
			if (binaryWriter != null)
			{
				Stream baseStream = binaryWriter.BaseStream;
				long position = baseStream.Position;
				binaryWriter.Write((byte)7);
				WriteElementProps(binaryWriter, rplWriter, pageContext, position + 1);
				List<long> list = WriteParagraphs(binaryWriter, pageContext, rplWriter.PageParagraphsItemizedData);
				long position2 = baseStream.Position;
				binaryWriter.Write((byte)18);
				binaryWriter.Write(position);
				binaryWriter.Write(list.Count);
				foreach (long item in list)
				{
					binaryWriter.Write(item);
				}
				binaryWriter.Write(byte.MaxValue);
				binaryWriter.Flush();
				m_offset = baseStream.Position;
				binaryWriter.Write((byte)254);
				binaryWriter.Write(position2);
				binaryWriter.Write(byte.MaxValue);
			}
			else if (m_rplElement == null)
			{
				m_rplElement = new RPLTextBox();
				WriteElementProps(m_rplElement.ElementProps, pageContext);
				if (m_nonSharedOffset < 0)
				{
					WriteParagraphs(pageContext, rplWriter.PageParagraphsItemizedData);
				}
			}
			else
			{
				if (base.SplitsVerticalPage)
				{
					RPLTextBoxProps rplElementProps = (RPLTextBoxProps)((RPLTextBoxProps)(m_rplElement.ElementProps as RPLItemProps)).Clone();
					m_rplElement = new RPLTextBox(rplElementProps);
				}
				RPLTextBoxProps rPLTextBoxProps = m_rplElement.ElementProps as RPLTextBoxProps;
				bool flag = textBox?.IsSimple ?? false;
				bool flag2 = true;
				if (m_textBoxState.SpanPages && flag && !textBox.FormattedValueExpressionBased)
				{
					WriteSharedItemProps(rPLTextBoxProps, pageContext);
					flag2 = false;
				}
				rPLTextBoxProps.ContentOffset = (float)m_contentOffset;
				if (flag || textBox == null)
				{
					if (!WriteSimpleValue(rPLTextBoxProps, pageContext) && flag2)
					{
						CalculateSimpleGlyph(pageContext);
					}
				}
				else
				{
					WriteParagraphs(pageContext, rplWriter.PageParagraphsItemizedData);
				}
			}
			if (pageContext.ParagraphItemizedData != null && pageContext.ParagraphItemizedData.Count > 0)
			{
				rplWriter.PageParagraphsItemizedData.Add(m_paragraphs[0].UniqueName, pageContext.ParagraphItemizedData);
			}
			pageContext.ParagraphItemizedData = null;
		}

		internal override void WriteCustomSharedItemProps(BinaryWriter spbifWriter, RPLWriter rplWriter, PageContext pageContext)
		{
			Microsoft.ReportingServices.OnDemandReportRendering.TextBox textBox = m_source as Microsoft.ReportingServices.OnDemandReportRendering.TextBox;
			if (textBox == null)
			{
				return;
			}
			StyleWriterStream styleWriterStream = new StyleWriterStream(spbifWriter);
			if (textBox.SharedTypeCode != TypeCode.String)
			{
				styleWriterStream.Write(33, (byte)textBox.SharedTypeCode);
			}
			if (textBox.CanGrow)
			{
				styleWriterStream.Write(25, value: true);
			}
			if (!textBox.IsSimple)
			{
				styleWriterStream.Write(35, textBox.IsSimple);
				return;
			}
			if (textBox.FormattedValueExpressionBased)
			{
				styleWriterStream.Write(45, textBox.FormattedValueExpressionBased);
			}
			if (!textBox.HideDuplicates && SimpleValuesIsSharedNonNull(textBox) && !m_textBoxState.SpanPages)
			{
				styleWriterStream.WriteNotNull(27, CalculateSimpleValue(isNonShared: false, pageContext));
			}
		}

		private bool SimpleValuesIsSharedNonNull(Microsoft.ReportingServices.OnDemandReportRendering.TextBox textBoxDef)
		{
			Microsoft.ReportingServices.OnDemandReportRendering.TextRun textRun = textBoxDef.Paragraphs[0].TextRuns[0];
			ReportStringProperty value = textRun.Value;
			if (!textRun.FormattedValueExpressionBased)
			{
				return value.Value != null;
			}
			return false;
		}

		private string CalculateSimpleValue(bool isNonShared, PageContext pageContext)
		{
			if (m_paragraphs.Count > 0 && (m_pageEndOffset.ParagraphIndex != 0 || m_pageEndOffset.TextRunIndex > 0 || m_pageEndOffset.CharacterIndex > 0))
			{
				Paragraph paragraph = m_paragraphs[0];
				if (paragraph.TextRuns.Count > 0)
				{
					TextRun textRun = paragraph.TextRuns[0];
					TextBoxOffset endPosition = m_pageEndOffset;
					if (m_pageEndOffset.ParagraphIndex != 0)
					{
						endPosition = null;
					}
					string text = null;
					text = ((!isNonShared) ? textRun.DefinitionText : textRun.Text);
					List<Microsoft.ReportingServices.Rendering.RichText.TextRun> richTextRuns = null;
					if (pageContext.ParagraphItemizedData != null)
					{
						richTextRuns = m_richTextBox.Paragraphs[0].Runs;
					}
					TextRunItemizedData glyphData = null;
					string stringValue = textRun.GetStringValue(text, m_pageStartOffset, endPosition, 0, richTextRuns, out glyphData);
					if (glyphData != null)
					{
						pageContext.RegisterTextRunData(glyphData);
					}
					return stringValue;
				}
			}
			return null;
		}

		private void CalculateSimpleGlyph(PageContext pageContext)
		{
			if (pageContext.ParagraphItemizedData == null || m_paragraphs.Count <= 0 || (m_pageEndOffset.ParagraphIndex == 0 && m_pageEndOffset.TextRunIndex <= 0 && m_pageEndOffset.CharacterIndex <= 0))
			{
				return;
			}
			Paragraph paragraph = m_paragraphs[0];
			if (paragraph.TextRuns.Count > 0)
			{
				TextRun textRun = paragraph.TextRuns[0];
				_ = m_pageEndOffset;
				_ = m_pageEndOffset.ParagraphIndex;
				TextRunItemizedData textRunItemizedData = textRun.GetGlyphValue(richTextRuns: m_richTextBox.Paragraphs[0].Runs, fullValue: textRun.DefinitionText, previousRunCount: 0);
				if (textRunItemizedData != null)
				{
					pageContext.RegisterTextRunData(textRunItemizedData);
				}
			}
		}

		internal override void WriteCustomSharedItemProps(RPLElementPropsDef sharedProps, PageContext pageContext)
		{
			Microsoft.ReportingServices.OnDemandReportRendering.TextBox textBox = m_source as Microsoft.ReportingServices.OnDemandReportRendering.TextBox;
			if (textBox != null)
			{
				RPLTextBoxPropsDef rPLTextBoxPropsDef = (RPLTextBoxPropsDef)sharedProps;
				rPLTextBoxPropsDef.SharedTypeCode = textBox.SharedTypeCode;
				rPLTextBoxPropsDef.CanGrow = textBox.CanGrow;
				rPLTextBoxPropsDef.FormattedValueExpressionBased = textBox.FormattedValueExpressionBased;
				if (!textBox.IsSimple)
				{
					rPLTextBoxPropsDef.IsSimple = false;
				}
				else if (!textBox.HideDuplicates && SimpleValuesIsSharedNonNull(textBox) && !m_textBoxState.SpanPages)
				{
					rPLTextBoxPropsDef.Value = CalculateSimpleValue(isNonShared: false, pageContext);
				}
			}
		}

		internal override void WriteCustomNonSharedItemProps(BinaryWriter spbifWriter, PageContext pageContext)
		{
			StyleWriterStream styleWriterStream = new StyleWriterStream(spbifWriter);
			Microsoft.ReportingServices.OnDemandReportRendering.TextBox textBox = m_source as Microsoft.ReportingServices.OnDemandReportRendering.TextBox;
			if (textBox == null)
			{
				if (m_nonSharedOffset == -1)
				{
					styleWriterStream.WriteNotNull(27, CalculateSimpleValue(isNonShared: true, pageContext));
				}
			}
			else
			{
				if (textBox.IsSimple)
				{
					bool changed = false;
					TypeCode typeCode = GetTypeCode(textBox.SharedTypeCode, m_source.Instance as TextBoxInstance, ref changed);
					if (changed)
					{
						styleWriterStream.Write(33, (byte)typeCode);
					}
					if (m_nonSharedOffset == -1 && !base.Duplicate)
					{
						bool formattedValueExpressionBased = textBox.Paragraphs[0].TextRuns[0].FormattedValueExpressionBased;
						if (formattedValueExpressionBased || textBox.HideDuplicates || m_textBoxState.SpanPages)
						{
							styleWriterStream.WriteNotNull(27, CalculateSimpleValue(formattedValueExpressionBased, pageContext));
						}
					}
				}
				WriteActionInfo(textBox.ActionInfo, spbifWriter);
			}
			if (m_nonSharedOffset == -1)
			{
				styleWriterStream.Write(37, (float)m_contentOffset);
			}
		}

		internal override void WriteCustomNonSharedItemProps(RPLElementProps nonSharedProps, PageContext pageContext)
		{
			RPLTextBoxProps rPLTextBoxProps = (RPLTextBoxProps)nonSharedProps;
			if (m_nonSharedOffset < 0)
			{
				WriteSimpleValue(rPLTextBoxProps, pageContext);
				rPLTextBoxProps.ContentOffset = (float)m_contentOffset;
			}
			Microsoft.ReportingServices.OnDemandReportRendering.TextBox textBox = m_source as Microsoft.ReportingServices.OnDemandReportRendering.TextBox;
			if (textBox != null)
			{
				if (textBox.IsSimple)
				{
					bool changed = false;
					TextBoxInstance textBox2 = textBox.Instance as TextBoxInstance;
					rPLTextBoxProps.TypeCode = GetTypeCode(textBox.SharedTypeCode, textBox2, ref changed);
				}
				rPLTextBoxProps.ActionInfo = PageItem.WriteActionInfo(textBox.ActionInfo);
			}
		}

		private bool WriteSimpleValue(RPLTextBoxProps textBoxProps, PageContext pageContext)
		{
			Microsoft.ReportingServices.OnDemandReportRendering.TextBox textBox = m_source as Microsoft.ReportingServices.OnDemandReportRendering.TextBox;
			if (textBox == null)
			{
				textBoxProps.Value = CalculateSimpleValue(isNonShared: true, pageContext);
				return true;
			}
			if (textBox.IsSimple && !base.Duplicate)
			{
				bool formattedValueExpressionBased = textBox.Paragraphs[0].TextRuns[0].FormattedValueExpressionBased;
				if (formattedValueExpressionBased || textBox.HideDuplicates || m_textBoxState.SpanPages)
				{
					textBoxProps.Value = CalculateSimpleValue(formattedValueExpressionBased, pageContext);
					return true;
				}
			}
			return false;
		}

		private TypeCode GetTypeCode(TypeCode sharedTypeCode, TextBoxInstance textBox, ref bool changed)
		{
			TypeCode typeCode = sharedTypeCode;
			if (typeCode == TypeCode.Object)
			{
				changed = true;
				typeCode = textBox.TypeCode;
			}
			return typeCode;
		}

		private void WriteParagraphs(PageContext pageContext, Dictionary<string, List<TextRunItemizedData>> textBoxItemizedData)
		{
			RPLTextBox tb = m_rplElement as RPLTextBox;
			Microsoft.ReportingServices.OnDemandReportRendering.TextBox textBox = m_source as Microsoft.ReportingServices.OnDemandReportRendering.TextBox;
			if (textBox == null || textBox.IsSimple || base.Duplicate)
			{
				return;
			}
			bool hideDuplicates = textBox.HideDuplicates;
			bool cacheRichData = (textBoxItemizedData != null) ? true : false;
			int num = m_pageEndOffset.ParagraphIndex;
			if (num < 0)
			{
				num = m_paragraphs.Count;
			}
			TextBoxOffset textBoxOffset = m_pageStartOffset;
			if (textBoxOffset.TextRunIndex == 0 && textBoxOffset.CharacterIndex == 0)
			{
				textBoxOffset = null;
			}
			if (num > 0)
			{
				WriteParagraph(tb, 0, cacheRichData, hideDuplicates, textBoxOffset, null, pageContext, textBoxItemizedData);
				for (int i = 1; i < num; i++)
				{
					WriteParagraph(tb, i, cacheRichData, hideDuplicates, null, null, pageContext, textBoxItemizedData);
				}
			}
			if (num < m_paragraphs.Count && (m_pageEndOffset.TextRunIndex > 0 || m_pageEndOffset.CharacterIndex > 0))
			{
				if (num > 0)
				{
					textBoxOffset = null;
				}
				WriteParagraph(tb, num, cacheRichData, hideDuplicates, textBoxOffset, m_pageEndOffset, pageContext, textBoxItemizedData);
			}
		}

		private void WriteParagraph(RPLTextBox tb, int index, bool cacheRichData, bool hideDuplicates, TextBoxOffset startPosition, TextBoxOffset endPosition, PageContext pageContext, Dictionary<string, List<TextRunItemizedData>> textBoxItemizedData)
		{
			Paragraph paragraph = m_paragraphs[index];
			List<Microsoft.ReportingServices.Rendering.RichText.TextRun> richTextParaRuns = null;
			if (cacheRichData)
			{
				richTextParaRuns = m_richTextBox.Paragraphs[index].Runs;
				pageContext.ParagraphItemizedData = new List<TextRunItemizedData>();
			}
			tb.AddParagraph(paragraph.GetRPLParagraph(pageContext, hideDuplicates, startPosition, endPosition, richTextParaRuns));
			if (cacheRichData && pageContext.ParagraphItemizedData.Count > 0)
			{
				textBoxItemizedData.Add(paragraph.UniqueName, pageContext.ParagraphItemizedData);
			}
			pageContext.ParagraphItemizedData = null;
		}

		private List<long> WriteParagraphs(BinaryWriter spbifWriter, PageContext pageContext, Dictionary<string, List<TextRunItemizedData>> textBoxItemizedData)
		{
			Microsoft.ReportingServices.OnDemandReportRendering.TextBox textBox = m_source as Microsoft.ReportingServices.OnDemandReportRendering.TextBox;
			if (textBox != null && !textBox.IsSimple && !base.Duplicate)
			{
				bool cacheRichData = (textBoxItemizedData != null) ? true : false;
				bool hideDuplicates = textBox?.HideDuplicates ?? false;
				TextBoxOffset textBoxOffset = m_pageStartOffset;
				if (textBoxOffset.TextRunIndex == 0 && textBoxOffset.CharacterIndex == 0)
				{
					textBoxOffset = null;
				}
				List<long> list = new List<long>();
				int num = m_pageEndOffset.ParagraphIndex;
				if (num < 0)
				{
					num = m_paragraphs.Count;
				}
				if (num > 0)
				{
					WriteParagraph(spbifWriter, 0, cacheRichData, hideDuplicates, textBoxOffset, null, list, pageContext, textBoxItemizedData);
					for (int i = 1; i < num; i++)
					{
						WriteParagraph(spbifWriter, i, cacheRichData, hideDuplicates, null, null, list, pageContext, textBoxItemizedData);
					}
				}
				if (num < m_paragraphs.Count && (m_pageEndOffset.TextRunIndex > 0 || m_pageEndOffset.CharacterIndex > 0))
				{
					if (num > 0)
					{
						textBoxOffset = null;
					}
					WriteParagraph(spbifWriter, num, cacheRichData, hideDuplicates, textBoxOffset, m_pageEndOffset, list, pageContext, textBoxItemizedData);
				}
				return list;
			}
			return new List<long>();
		}

		private void WriteParagraph(BinaryWriter spbifWriter, int index, bool cacheRichData, bool hideDuplicates, TextBoxOffset startPosition, TextBoxOffset endPosition, List<long> offsets, PageContext pageContext, Dictionary<string, List<TextRunItemizedData>> textBoxItemizedData)
		{
			Paragraph paragraph = m_paragraphs[index];
			List<Microsoft.ReportingServices.Rendering.RichText.TextRun> richTextParaRuns = null;
			if (cacheRichData)
			{
				richTextParaRuns = m_richTextBox.Paragraphs[index].Runs;
				pageContext.ParagraphItemizedData = new List<TextRunItemizedData>();
			}
			offsets.Add(paragraph.WriteToStream(spbifWriter, pageContext, hideDuplicates, startPosition, endPosition, richTextParaRuns));
			if (cacheRichData && pageContext.ParagraphItemizedData.Count > 0)
			{
				textBoxItemizedData.Add(paragraph.UniqueName, pageContext.ParagraphItemizedData);
			}
			pageContext.ParagraphItemizedData = null;
		}

		private bool WriteOriginalValue(BinaryWriter spbifWriter, TypeCode typeCode, object value)
		{
			if (value == null)
			{
				return false;
			}
			return WriteObjectValue(spbifWriter, 34, typeCode, value);
		}

		internal override void WriteItemSharedStyleProps(BinaryWriter spbifWriter, Style style, PageContext pageContext)
		{
			WriteStyleProp(style, spbifWriter, StyleAttributeNames.BackgroundColor, 34);
			WriteBackgroundImage(style, writeShared: true, spbifWriter, pageContext);
			WriteStyleProp(style, spbifWriter, StyleAttributeNames.VerticalAlign, 26);
			WriteStyleProp(style, spbifWriter, StyleAttributeNames.WritingMode, 30);
			WriteStyleProp(style, spbifWriter, StyleAttributeNames.PaddingBottom, 18);
			WriteStyleProp(style, spbifWriter, StyleAttributeNames.PaddingLeft, 15);
			WriteStyleProp(style, spbifWriter, StyleAttributeNames.PaddingRight, 16);
			WriteStyleProp(style, spbifWriter, StyleAttributeNames.PaddingTop, 17);
			WriteStyleProp(style, spbifWriter, StyleAttributeNames.Direction, 29);
			Microsoft.ReportingServices.OnDemandReportRendering.TextBox textBox = m_source as Microsoft.ReportingServices.OnDemandReportRendering.TextBox;
			if (textBox != null && textBox.IsSimple)
			{
				Microsoft.ReportingServices.OnDemandReportRendering.Paragraph paragraph = textBox.Paragraphs[0];
				Microsoft.ReportingServices.OnDemandReportRendering.TextRun textRun = paragraph.TextRuns[0];
				WriteStyleProp(textRun.Style, spbifWriter, StyleAttributeNames.Color, 27);
				WriteStyleProp(textRun.Style, spbifWriter, StyleAttributeNames.FontFamily, 20);
				WriteStyleProp(textRun.Style, spbifWriter, StyleAttributeNames.FontSize, 21);
				WriteStyleProp(textRun.Style, spbifWriter, StyleAttributeNames.FontStyle, 19);
				WriteStyleProp(textRun.Style, spbifWriter, StyleAttributeNames.FontWeight, 22);
				WriteStyleProp(textRun.Style, spbifWriter, StyleAttributeNames.Language, 32);
				WriteStyleProp(paragraph.Style, spbifWriter, StyleAttributeNames.TextAlign, 25);
				WriteStyleProp(textRun.Style, spbifWriter, StyleAttributeNames.TextDecoration, 24);
			}
			else if (m_source is Microsoft.ReportingServices.OnDemandReportRendering.Tablix)
			{
				WriteStyleProp(style, spbifWriter, StyleAttributeNames.Color, 27);
				WriteStyleProp(style, spbifWriter, StyleAttributeNames.FontFamily, 20);
				WriteStyleProp(style, spbifWriter, StyleAttributeNames.FontSize, 21);
				WriteStyleProp(style, spbifWriter, StyleAttributeNames.FontStyle, 19);
				WriteStyleProp(style, spbifWriter, StyleAttributeNames.FontWeight, 22);
				WriteStyleProp(style, spbifWriter, StyleAttributeNames.Language, 32);
				WriteStyleProp(style, spbifWriter, StyleAttributeNames.TextAlign, 25);
				WriteStyleProp(style, spbifWriter, StyleAttributeNames.TextDecoration, 24);
			}
		}

		internal override void WriteItemSharedStyleProps(RPLStyleProps rplStyleProps, Style style, PageContext pageContext)
		{
			PageItem.WriteStyleProp(style, rplStyleProps, StyleAttributeNames.BackgroundColor, 34);
			WriteBackgroundImage(style, writeShared: true, rplStyleProps, pageContext);
			PageItem.WriteStyleProp(style, rplStyleProps, StyleAttributeNames.VerticalAlign, 26);
			PageItem.WriteStyleProp(style, rplStyleProps, StyleAttributeNames.WritingMode, 30);
			PageItem.WriteStyleProp(style, rplStyleProps, StyleAttributeNames.PaddingBottom, 18);
			PageItem.WriteStyleProp(style, rplStyleProps, StyleAttributeNames.PaddingLeft, 15);
			PageItem.WriteStyleProp(style, rplStyleProps, StyleAttributeNames.PaddingRight, 16);
			PageItem.WriteStyleProp(style, rplStyleProps, StyleAttributeNames.PaddingTop, 17);
			PageItem.WriteStyleProp(style, rplStyleProps, StyleAttributeNames.Direction, 29);
			Microsoft.ReportingServices.OnDemandReportRendering.TextBox textBox = m_source as Microsoft.ReportingServices.OnDemandReportRendering.TextBox;
			if (textBox != null && textBox.IsSimple)
			{
				Microsoft.ReportingServices.OnDemandReportRendering.Paragraph paragraph = textBox.Paragraphs[0];
				Microsoft.ReportingServices.OnDemandReportRendering.TextRun textRun = paragraph.TextRuns[0];
				PageItem.WriteStyleProp(textRun.Style, rplStyleProps, StyleAttributeNames.Color, 27);
				PageItem.WriteStyleProp(textRun.Style, rplStyleProps, StyleAttributeNames.FontFamily, 20);
				PageItem.WriteStyleProp(textRun.Style, rplStyleProps, StyleAttributeNames.FontSize, 21);
				PageItem.WriteStyleProp(textRun.Style, rplStyleProps, StyleAttributeNames.FontStyle, 19);
				PageItem.WriteStyleProp(textRun.Style, rplStyleProps, StyleAttributeNames.FontWeight, 22);
				PageItem.WriteStyleProp(textRun.Style, rplStyleProps, StyleAttributeNames.Language, 32);
				PageItem.WriteStyleProp(paragraph.Style, rplStyleProps, StyleAttributeNames.TextAlign, 25);
				PageItem.WriteStyleProp(textRun.Style, rplStyleProps, StyleAttributeNames.TextDecoration, 24);
			}
			else if (m_source is Microsoft.ReportingServices.OnDemandReportRendering.Tablix)
			{
				PageItem.WriteStyleProp(style, rplStyleProps, StyleAttributeNames.Color, 27);
				PageItem.WriteStyleProp(style, rplStyleProps, StyleAttributeNames.FontFamily, 20);
				PageItem.WriteStyleProp(style, rplStyleProps, StyleAttributeNames.FontSize, 21);
				PageItem.WriteStyleProp(style, rplStyleProps, StyleAttributeNames.FontStyle, 19);
				PageItem.WriteStyleProp(style, rplStyleProps, StyleAttributeNames.FontWeight, 22);
				PageItem.WriteStyleProp(style, rplStyleProps, StyleAttributeNames.Language, 32);
				PageItem.WriteStyleProp(style, rplStyleProps, StyleAttributeNames.TextAlign, 25);
				PageItem.WriteStyleProp(style, rplStyleProps, StyleAttributeNames.TextDecoration, 24);
			}
		}

		internal override void WriteNonSharedStyle(BinaryWriter spbifWriter, Style styleDef, StyleInstance style, PageContext pageContext)
		{
			if (styleDef == null)
			{
				styleDef = m_source.Style;
			}
			Microsoft.ReportingServices.OnDemandReportRendering.TextBox textBox = m_source as Microsoft.ReportingServices.OnDemandReportRendering.TextBox;
			List<StyleAttributeNames> nonSharedStyleAttributes = styleDef.NonSharedStyleAttributes;
			if (Utility.IsNullOrEmpty(nonSharedStyleAttributes) && (textBox == null || !textBox.IsSimple || (Utility.IsNullOrEmpty(textBox.Paragraphs[0].Style.NonSharedStyleAttributes) && Utility.IsNullOrEmpty(textBox.Paragraphs[0].TextRuns[0].Style.NonSharedStyleAttributes))))
			{
				return;
			}
			if (style == null)
			{
				style = m_source.Instance.Style;
			}
			bool flag = false;
			spbifWriter.Write((byte)6);
			spbifWriter.Write((byte)1);
			if (!Utility.IsNullOrEmpty(nonSharedStyleAttributes))
			{
				int count = nonSharedStyleAttributes.Count;
				for (int i = 0; i < count; i++)
				{
					StyleAttributeNames styleAttributeNames = nonSharedStyleAttributes[i];
					if ((uint)(styleAttributeNames - 45) <= 1u)
					{
						if (!flag)
						{
							flag = true;
							WriteItemNonSharedStyleProps(spbifWriter, styleDef, style, StyleAttributeNames.BackgroundImage, pageContext);
						}
					}
					else
					{
						WriteNonSharedStyleProp(spbifWriter, styleDef, style, nonSharedStyleAttributes[i], pageContext);
					}
				}
			}
			if (textBox != null && textBox.IsSimple)
			{
				StyleWriterStream writer = new StyleWriterStream(spbifWriter);
				Paragraph paragraph = m_paragraphs[0];
				paragraph.WriteNonSharedStyles(writer);
				paragraph.TextRuns[0].WriteNonSharedStyles(writer);
			}
			spbifWriter.Write(byte.MaxValue);
		}

		internal override RPLStyleProps WriteNonSharedStyle(Style styleDef, StyleInstance style, PageContext pageContext)
		{
			RPLStyleProps rPLStyleProps = base.WriteNonSharedStyle(styleDef, style, pageContext);
			Microsoft.ReportingServices.OnDemandReportRendering.TextBox textBox = m_source as Microsoft.ReportingServices.OnDemandReportRendering.TextBox;
			if (textBox != null && textBox.IsSimple && m_paragraphs.Count > 0)
			{
				if (rPLStyleProps == null)
				{
					rPLStyleProps = new RPLStyleProps();
				}
				StyleWriterOM writer = new StyleWriterOM(rPLStyleProps);
				Paragraph paragraph = m_paragraphs[0];
				paragraph.WriteNonSharedStyles(writer);
				paragraph.TextRuns[0].WriteNonSharedStyles(writer);
				if (rPLStyleProps.Count == 0)
				{
					rPLStyleProps = null;
				}
			}
			return rPLStyleProps;
		}

		internal override void WriteItemNonSharedStyleProps(BinaryWriter spbifWriter, Style styleDef, StyleInstance style, StyleAttributeNames styleAtt, PageContext pageContext)
		{
			switch (styleAtt)
			{
			case StyleAttributeNames.BackgroundColor:
				WriteStyleProp(styleDef, style, spbifWriter, StyleAttributeNames.BackgroundColor, 34);
				break;
			case StyleAttributeNames.BackgroundImage:
				WriteBackgroundImage(styleDef, writeShared: false, spbifWriter, pageContext);
				break;
			case StyleAttributeNames.Direction:
				WriteStyleProp(styleDef, style, spbifWriter, StyleAttributeNames.Direction, 29);
				break;
			case StyleAttributeNames.VerticalAlign:
				WriteStyleProp(styleDef, style, spbifWriter, StyleAttributeNames.VerticalAlign, 26);
				break;
			case StyleAttributeNames.WritingMode:
				WriteStyleProp(styleDef, style, spbifWriter, StyleAttributeNames.WritingMode, 30);
				break;
			case StyleAttributeNames.PaddingBottom:
				WriteStyleProp(styleDef, style, spbifWriter, StyleAttributeNames.PaddingBottom, 18);
				break;
			case StyleAttributeNames.PaddingLeft:
				WriteStyleProp(styleDef, style, spbifWriter, StyleAttributeNames.PaddingLeft, 15);
				break;
			case StyleAttributeNames.PaddingRight:
				WriteStyleProp(styleDef, style, spbifWriter, StyleAttributeNames.PaddingRight, 16);
				break;
			case StyleAttributeNames.PaddingTop:
				WriteStyleProp(styleDef, style, spbifWriter, StyleAttributeNames.PaddingTop, 17);
				break;
			}
		}

		internal override void WriteItemNonSharedStyleProps(RPLStyleProps rplStyleProps, Style styleDef, StyleInstance style, StyleAttributeNames styleAtt, PageContext pageContext)
		{
			switch (styleAtt)
			{
			case StyleAttributeNames.BackgroundColor:
				WriteStyleProp(styleDef, style, rplStyleProps, StyleAttributeNames.BackgroundColor, 34);
				break;
			case StyleAttributeNames.BackgroundImage:
				WriteBackgroundImage(styleDef, writeShared: false, rplStyleProps, pageContext);
				break;
			case StyleAttributeNames.Direction:
				WriteStyleProp(styleDef, style, rplStyleProps, StyleAttributeNames.Direction, 29);
				break;
			case StyleAttributeNames.VerticalAlign:
				WriteStyleProp(styleDef, style, rplStyleProps, StyleAttributeNames.VerticalAlign, 26);
				break;
			case StyleAttributeNames.WritingMode:
				WriteStyleProp(styleDef, style, rplStyleProps, StyleAttributeNames.WritingMode, 30);
				break;
			case StyleAttributeNames.PaddingBottom:
				WriteStyleProp(styleDef, style, rplStyleProps, StyleAttributeNames.PaddingBottom, 18);
				break;
			case StyleAttributeNames.PaddingLeft:
				WriteStyleProp(styleDef, style, rplStyleProps, StyleAttributeNames.PaddingLeft, 15);
				break;
			case StyleAttributeNames.PaddingRight:
				WriteStyleProp(styleDef, style, rplStyleProps, StyleAttributeNames.PaddingRight, 16);
				break;
			case StyleAttributeNames.PaddingTop:
				WriteStyleProp(styleDef, style, rplStyleProps, StyleAttributeNames.PaddingTop, 17);
				break;
			}
		}

		private Microsoft.ReportingServices.Rendering.RichText.TextBox GetRichTextBox()
		{
			if (base.Duplicate)
			{
				return null;
			}
			if (m_paragraphs.Count == 0)
			{
				return null;
			}
			Microsoft.ReportingServices.OnDemandReportRendering.TextBox textBox = m_source as Microsoft.ReportingServices.OnDemandReportRendering.TextBox;
			if (textBox == null || textBox.IsSimple)
			{
				string text = m_paragraphs[0].TextRuns[0].Text;
				if (text == null && textBox != null)
				{
					text = textBox.Paragraphs[0].TextRuns[0].Value.Value;
				}
				if (string.IsNullOrEmpty(text))
				{
					return null;
				}
			}
			Microsoft.ReportingServices.Rendering.RichText.TextBox textBox2 = new Microsoft.ReportingServices.Rendering.RichText.TextBox(this);
			textBox2.Paragraphs = new List<Microsoft.ReportingServices.Rendering.RichText.Paragraph>(m_paragraphs.Count);
			foreach (Paragraph paragraph in m_paragraphs)
			{
				Microsoft.ReportingServices.Rendering.RichText.Paragraph richTextParagraph = paragraph.GetRichTextParagraph();
				textBox2.Paragraphs.Add(richTextParagraph);
			}
			textBox2.ScriptItemize();
			return textBox2;
		}

		private void InitParagraphs()
		{
			Microsoft.ReportingServices.OnDemandReportRendering.TextBox textBox = m_source as Microsoft.ReportingServices.OnDemandReportRendering.TextBox;
			if (textBox != null)
			{
				Style style = m_source.Style;
				StyleInstance style2 = m_source.Instance.Style;
				if (GetAlignmentRight(style, style2))
				{
					m_textBoxState.DefaultTextAlign = RPLFormat.TextAlignments.Right;
				}
				byte? nonCompiledEnumProp = Utility.GetNonCompiledEnumProp(29, StyleAttributeNames.Direction, style, style2);
				if (nonCompiledEnumProp.HasValue)
				{
					m_textBoxState.Direction = (RPLFormat.Directions)nonCompiledEnumProp.Value;
				}
				nonCompiledEnumProp = Utility.GetNonCompiledEnumProp(30, StyleAttributeNames.WritingMode, style, style2);
				if (nonCompiledEnumProp.HasValue)
				{
					m_textBoxState.WritingMode = (RPLFormat.WritingModes)nonCompiledEnumProp.Value;
				}
				nonCompiledEnumProp = Utility.GetNonCompiledEnumProp(26, StyleAttributeNames.VerticalAlign, style, style2);
				if (nonCompiledEnumProp.HasValue)
				{
					m_textBoxState.VerticalAlignment = (RPLFormat.VerticalAlignments)nonCompiledEnumProp.Value;
				}
				string uniqueName = null;
				if (textBox.IsSimple)
				{
					uniqueName = m_source.Instance.UniqueName;
				}
				ParagraphInstanceCollection paragraphInstances = (textBox.Instance as TextBoxInstance).ParagraphInstances;
				ParagraphNumberCalculator paragraphNumberCalculator = new ParagraphNumberCalculator();
				foreach (ParagraphInstance item in paragraphInstances)
				{
					Paragraph paragraph = new Paragraph(item, uniqueName, textBox.HideDuplicates);
					paragraphNumberCalculator.UpdateParagraph(paragraph);
					m_paragraphs.Add(paragraph);
				}
				return;
			}
			string text = null;
			string uniqueName2 = null;
			DataRegion dataRegion = m_source as DataRegion;
			if (dataRegion != null)
			{
				text = ((DataRegionInstance)dataRegion.Instance).NoRowsMessage;
				uniqueName2 = dataRegion.Instance.UniqueName;
			}
			else
			{
				Microsoft.ReportingServices.OnDemandReportRendering.SubReport subReport = m_source as Microsoft.ReportingServices.OnDemandReportRendering.SubReport;
				if (subReport != null)
				{
					SubReportInstance subReportInstance = subReport.Instance as SubReportInstance;
					text = ((!subReportInstance.ProcessedWithError) ? subReportInstance.NoRowsMessage : subReportInstance.ErrorMessage);
					uniqueName2 = subReportInstance.UniqueName;
				}
			}
			Paragraph paragraph2 = new Paragraph(uniqueName2);
			TextRun textRun = new TextRun();
			textRun.Text = text;
			paragraph2.TextRuns.Add(textRun);
			m_paragraphs.Add(paragraph2);
		}

		public void DrawTextRun(Microsoft.ReportingServices.Rendering.RichText.TextRun run, Microsoft.ReportingServices.Rendering.RichText.Paragraph paragraph, Win32DCSafeHandle hdc, float dpiX, FontCache fontCache, int x, int y, int baselineY, int lineHeight, System.Drawing.Rectangle layoutRectangle)
		{
		}

		public void DrawClippedTextRun(Microsoft.ReportingServices.Rendering.RichText.TextRun run, Microsoft.ReportingServices.Rendering.RichText.Paragraph paragraph, Win32DCSafeHandle hdc, float dpiX, FontCache fontCache, int x, int y, int baselineY, int lineHeight, System.Drawing.Rectangle layoutRectangle, uint fontColorOverride, System.Drawing.Rectangle clipRect)
		{
		}
	}
}
