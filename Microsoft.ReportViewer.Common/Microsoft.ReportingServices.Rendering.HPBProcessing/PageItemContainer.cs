using Microsoft.ReportingServices.Diagnostics.Utilities;
using Microsoft.ReportingServices.OnDemandProcessing.Scalability;
using Microsoft.ReportingServices.OnDemandReportRendering;
using Microsoft.ReportingServices.Rendering.RPLProcessing;
using Microsoft.ReportingServices.ReportIntermediateFormat.Persistence;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace Microsoft.ReportingServices.Rendering.HPBProcessing
{
	internal abstract class PageItemContainer : PageItem, IComparer, IStorable, IPersistable
	{
		protected PageItem[] m_children;

		protected int[] m_indexesLeftToRight;

		protected double m_rightPadding;

		protected double m_definitionRightPadding;

		protected double m_bottomPadding;

		private static Declaration m_declaration = GetDeclaration();

		internal abstract byte RPLFormatType
		{
			get;
		}

		public override int Size => base.Size + Microsoft.ReportingServices.OnDemandProcessing.Scalability.ItemSizes.SizeOf(m_children) + 24 + Microsoft.ReportingServices.OnDemandProcessing.Scalability.ItemSizes.SizeOf(m_indexesLeftToRight);

		internal PageItemContainer()
		{
		}

		internal PageItemContainer(ReportItem source)
			: base(source)
		{
			base.FullyCreated = false;
		}

		internal abstract RPLElement CreateRPLElement();

		internal abstract RPLElement CreateRPLElement(RPLElementProps props, PageContext pageContext);

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(m_declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.Children:
					writer.Write(m_children);
					break;
				case MemberName.Indexes:
					writer.Write(m_indexesLeftToRight);
					break;
				case MemberName.HorizontalPadding:
					writer.Write(m_rightPadding);
					break;
				case MemberName.DefPadding:
					writer.Write(m_definitionRightPadding);
					break;
				case MemberName.VerticalPadding:
					writer.Write(m_bottomPadding);
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
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.Children:
					m_children = reader.ReadArrayOfRIFObjects<PageItem>();
					break;
				case MemberName.Indexes:
					m_indexesLeftToRight = reader.ReadInt32Array();
					break;
				case MemberName.HorizontalPadding:
					m_rightPadding = reader.ReadDouble();
					break;
				case MemberName.DefPadding:
					m_definitionRightPadding = reader.ReadDouble();
					break;
				case MemberName.VerticalPadding:
					m_bottomPadding = reader.ReadDouble();
					break;
				default:
					RSTrace.RenderingTracer.Assert(condition: false, string.Empty);
					break;
				}
			}
		}

		public override ObjectType GetObjectType()
		{
			return ObjectType.PageItemContainer;
		}

		internal new static Declaration GetDeclaration()
		{
			if (m_declaration == null)
			{
				List<MemberInfo> list = new List<MemberInfo>();
				list.Add(new MemberInfo(MemberName.Children, ObjectType.RIFObjectArray, ObjectType.PageItem));
				list.Add(new MemberInfo(MemberName.Indexes, ObjectType.PrimitiveTypedArray, Token.Int32));
				list.Add(new MemberInfo(MemberName.HorizontalPadding, Token.Double));
				list.Add(new MemberInfo(MemberName.DefPadding, Token.Double));
				list.Add(new MemberInfo(MemberName.VerticalPadding, Token.Double));
				return new Declaration(ObjectType.PageItemContainer, ObjectType.PageItem, list);
			}
			return m_declaration;
		}

		int IComparer.Compare(object o1, object o2)
		{
			int num = (int)o1;
			int num2 = (int)o2;
			if (m_children[num].ItemPageSizes.Left == m_children[num2].ItemPageSizes.Left)
			{
				return 0;
			}
			if (m_children[num].ItemPageSizes.Left < m_children[num2].ItemPageSizes.Left)
			{
				return -1;
			}
			return 1;
		}

		private void VerticalDependency()
		{
			ItemSizes itemSizes = null;
			PageItem pageItem = null;
			ItemSizes itemSizes2 = null;
			RoundedDouble roundedDouble = new RoundedDouble(-1.0);
			RoundedDouble roundedDouble2 = new RoundedDouble(0.0);
			for (int i = 0; i < m_children.Length; i++)
			{
				itemSizes = m_children[i].ItemPageSizes;
				roundedDouble2.Value = itemSizes.Bottom;
				roundedDouble.Value = -1.0;
				for (int j = i + 1; j < m_children.Length; j++)
				{
					pageItem = m_children[j];
					itemSizes2 = pageItem.ItemPageSizes;
					if (roundedDouble >= 0.0 && roundedDouble <= itemSizes2.Top)
					{
						break;
					}
					if (roundedDouble2 <= itemSizes2.Top)
					{
						if (pageItem.PageItemsAbove == null)
						{
							pageItem.PageItemsAbove = new List<int>();
						}
						pageItem.PageItemsAbove.Add(i);
						if (roundedDouble < 0.0 || roundedDouble > itemSizes2.Bottom)
						{
							roundedDouble.Value = itemSizes2.Bottom;
						}
					}
				}
			}
		}

		private void HorizontalDependecy()
		{
			ItemSizes itemSizes = null;
			PageItem pageItem = null;
			ItemSizes itemSizes2 = null;
			RoundedDouble roundedDouble = new RoundedDouble(-1.0);
			RoundedDouble roundedDouble2 = new RoundedDouble(0.0);
			for (int i = 0; i < m_children.Length; i++)
			{
				itemSizes = m_children[m_indexesLeftToRight[i]].ItemPageSizes;
				roundedDouble.Value = -1.0;
				roundedDouble2.Value = itemSizes.Right;
				for (int j = i + 1; j < m_children.Length; j++)
				{
					pageItem = m_children[m_indexesLeftToRight[j]];
					itemSizes2 = pageItem.ItemPageSizes;
					if (roundedDouble >= 0.0 && roundedDouble <= itemSizes2.Left)
					{
						break;
					}
					if (roundedDouble2 <= itemSizes2.Left)
					{
						if (pageItem.PageItemsLeft == null)
						{
							pageItem.PageItemsLeft = new List<int>();
						}
						pageItem.PageItemsLeft.Add(m_indexesLeftToRight[i]);
						if (roundedDouble < 0.0 || roundedDouble > itemSizes2.Right)
						{
							roundedDouble.Value = itemSizes2.Right;
						}
					}
				}
			}
		}

		protected abstract void CreateChildren(PageContext pageContext);

		protected void CreateChildren(ReportItemCollection childrenDef, PageContext pageContext)
		{
			if (childrenDef != null && childrenDef.Count != 0 && m_children == null)
			{
				double num = 0.0;
				double num2 = 0.0;
				m_children = new PageItem[childrenDef.Count + 1];
				m_indexesLeftToRight = new int[childrenDef.Count + 1];
				for (int i = 0; i < childrenDef.Count; i++)
				{
					ReportItem source = childrenDef[i];
					m_children[i] = PageItem.Create(source, tablixCellParent: false, pageContext);
					m_indexesLeftToRight[i] = i;
					num = Math.Max(num, m_children[i].ItemPageSizes.Right);
					num2 = Math.Max(num2, m_children[i].ItemPageSizes.Bottom);
				}
				double num3 = 0.0;
				num3 = ((!pageContext.ConsumeWhitespace) ? Math.Max(num2, base.ItemPageSizes.Height) : num2);
				m_children[m_children.Length - 1] = new HiddenPageItem(num3, 0.0);
				m_indexesLeftToRight[m_children.Length - 1] = m_children.Length - 1;
				m_rightPadding = Math.Max(0.0, m_itemPageSizes.Width - num);
				m_definitionRightPadding = m_rightPadding;
				m_bottomPadding = Math.Max(0.0, m_itemPageSizes.Height - num2);
				Array.Sort(m_indexesLeftToRight, this);
				VerticalDependency();
				HorizontalDependecy();
			}
		}

		private void ConsumeWhitespaceVertical(ItemSizes itemSizes, double adjustHeightTo)
		{
			double num = adjustHeightTo - itemSizes.Height;
			itemSizes.AdjustHeightTo(adjustHeightTo);
			if (!(num <= 0.0) && !(m_bottomPadding <= 0.0))
			{
				double num2 = num - m_bottomPadding;
				if (num2 > 0.0)
				{
					itemSizes.AdjustHeightTo(itemSizes.Height - m_bottomPadding);
					m_bottomPadding = 0.0;
				}
				else
				{
					itemSizes.AdjustHeightTo(itemSizes.Height - num);
					m_bottomPadding = 0.0 - num2;
				}
			}
		}

		protected override void DetermineVerticalSize(PageContext pageContext, double topInParentSystem, double bottomInParentSystem, List<PageItem> ancestors, ref bool anyAncestorHasKT, bool hasUnpinnedAncestors)
		{
			if (m_children == null)
			{
				CreateChildren(pageContext);
			}
			if (m_children == null)
			{
				base.FullyCreated = true;
				return;
			}
			ancestors.Add(this);
			bool keepTogetherVertical = base.KeepTogetherVertical;
			bool anyAncestorHasKT2 = keepTogetherVertical || anyAncestorHasKT;
			DetermineContentVerticalSize(pageContext, topInParentSystem, bottomInParentSystem, ancestors, ref anyAncestorHasKT2, hasUnpinnedAncestors, resolveState: false, resolveItem: false);
			if (keepTogetherVertical && !anyAncestorHasKT2)
			{
				anyAncestorHasKT = false;
			}
			ancestors.RemoveAt(ancestors.Count - 1);
		}

		internal override bool ResolveDuplicates(PageContext pageContext, double topInParentSystem, PageItem[] siblings, bool recalculate)
		{
			base.ResolveDuplicates(pageContext, topInParentSystem, siblings, recalculate);
			if (m_children == null)
			{
				return false;
			}
			double topInParentSystem2 = Math.Max(0.0, topInParentSystem - base.ItemPageSizes.Top);
			double num = 0.0;
			bool flag = false;
			for (int i = 0; i < m_children.Length; i++)
			{
				PageItem pageItem = m_children[i];
				if (pageItem != null)
				{
					flag |= pageItem.ResolveDuplicates(pageContext, topInParentSystem2, m_children, flag);
					num = Math.Max(num, pageItem.ItemPageSizes.Bottom);
				}
			}
			if (flag)
			{
				if (pageContext.ConsumeWhitespace)
				{
					ConsumeWhitespaceVertical(base.ItemPageSizes, num + m_bottomPadding);
				}
				else
				{
					base.ItemPageSizes.AdjustHeightTo(num);
				}
				PageItem[] children = m_children;
				foreach (PageItem pageItem2 in children)
				{
					if (pageItem2 != null)
					{
						pageItem2.ItemPageSizes.DeltaY = 0.0;
					}
				}
			}
			return flag;
		}

		private void DetermineContentVerticalSize(PageContext pageContext, double topInParentSystem, double bottomInParentSystem, List<PageItem> ancestors, ref bool anyAncestorHasKT, bool hasUnpinnedAncestors, bool resolveState, bool resolveItem)
		{
			double num = 0.0;
			double topInParentSystem2 = Math.Max(0.0, topInParentSystem - base.ItemPageSizes.Top);
			double bottomInParentSystem2 = bottomInParentSystem - base.ItemPageSizes.Top;
			PageContext pageContext2 = pageContext;
			if (!pageContext.IgnorePageBreaks && base.IgnorePageBreaks)
			{
				pageContext2 = new PageContext(pageContext, pageContext.CacheNonSharedProps);
				pageContext2.IgnorePageBreaks = true;
				pageContext2.IgnorePageBreaksReason = PageContext.IgnorePageBreakReason.ToggleableItem;
			}
			base.FullyCreated = true;
			for (int i = 0; i < m_children.Length; i++)
			{
				PageItem pageItem = m_children[i];
				if (pageItem != null)
				{
					if (resolveState)
					{
						pageItem.ResolveVertical(pageContext2, topInParentSystem2, bottomInParentSystem2, m_children, resolveItem, pageContext.Common.CanOverwritePageBreak, pageContext.Common.CanSetPageName);
					}
					else
					{
						pageItem.CalculateVertical(pageContext2, topInParentSystem2, bottomInParentSystem2, m_children, ancestors, ref anyAncestorHasKT, hasUnpinnedAncestors);
					}
					if (pageItem.KTVIsUnresolved)
					{
						base.UnresolvedCKTV = true;
					}
					if (pageItem.PBAreUnresolved)
					{
						base.UnresolvedCPB = true;
					}
					if (pageItem.NeedResolve)
					{
						base.NeedResolve = true;
					}
					if (!pageItem.FullyCreated)
					{
						base.FullyCreated = false;
					}
					num = Math.Max(num, pageItem.ItemPageSizes.Bottom);
				}
			}
			if (pageContext.ConsumeWhitespace)
			{
				ConsumeWhitespaceVertical(base.ItemPageSizes, num + m_bottomPadding);
			}
			else
			{
				base.ItemPageSizes.AdjustHeightTo(num);
			}
			PageItem[] children = m_children;
			foreach (PageItem pageItem2 in children)
			{
				if (pageItem2 != null)
				{
					pageItem2.ItemPageSizes.DeltaY = 0.0;
				}
			}
		}

		protected override bool ResolveKeepTogetherVertical(PageContext pageContext, double topInParentSystem, double bottomInParentSystem, bool resolveItem, bool canOverwritePageBreak, bool canSetPageName)
		{
			bool flag = base.ResolveKeepTogetherVertical(pageContext, topInParentSystem, bottomInParentSystem, resolveItem, canOverwritePageBreak, canSetPageName);
			if (m_children == null)
			{
				return flag;
			}
			bool anyAncestorHasKT = false;
			if ((resolveItem && base.NeedResolve) || (!flag && base.UnresolvedCKTV) || base.UnresolvedCPB)
			{
				base.UnresolvedCKTV = false;
				base.UnresolvedCPB = false;
				base.NeedResolve = false;
				DetermineContentVerticalSize(pageContext, topInParentSystem, bottomInParentSystem, null, ref anyAncestorHasKT, hasUnpinnedAncestors: false, resolveState: true, resolveItem);
			}
			return flag;
		}

		protected override void DetermineHorizontalSize(PageContext pageContext, double leftInParentSystem, double rightInParentSystem, List<PageItem> ancestors, bool anyAncestorHasKT, bool hasUnpinnedAncestors)
		{
			if (m_children != null)
			{
				ancestors.Add(this);
				if (base.KeepTogetherHorizontal)
				{
					anyAncestorHasKT = true;
				}
				DetermineContentHorizontalSize(pageContext, leftInParentSystem, rightInParentSystem, ancestors, anyAncestorHasKT, hasUnpinnedAncestors, resolveState: false, resolveItem: false);
				ancestors.RemoveAt(ancestors.Count - 1);
			}
		}

		protected virtual void DetermineContentHorizontalSize(PageContext pageContext, double leftInParentSystem, double rightInParentSystem, List<PageItem> ancestors, bool anyAncestorHasKT, bool hasUnpinnedAncestors, bool resolveState, bool resolveItem)
		{
			double num = 0.0;
			double leftInParentSystem2 = Math.Max(0.0, leftInParentSystem - base.ItemPageSizes.Left);
			double rightInParentSystem2 = rightInParentSystem - base.ItemPageSizes.Left;
			bool flag = true;
			for (int i = 0; i < m_children.Length; i++)
			{
				PageItem pageItem = m_children[m_indexesLeftToRight[i]];
				if (pageItem != null)
				{
					if (resolveState)
					{
						pageItem.ResolveHorizontal(pageContext, leftInParentSystem2, rightInParentSystem2, m_children, resolveItem);
					}
					else
					{
						pageItem.CalculateHorizontal(pageContext, leftInParentSystem2, rightInParentSystem2, m_children, ancestors, ref anyAncestorHasKT, hasUnpinnedAncestors);
					}
					if (pageItem.KTHIsUnresolved)
					{
						base.UnresolvedCKTH = true;
					}
					if (pageItem.NeedResolve)
					{
						base.NeedResolve = true;
					}
					num = Math.Max(num, pageItem.ItemPageSizes.Right);
					if (!(pageItem is HiddenPageItem))
					{
						flag = false;
					}
				}
			}
			if (num > 0.0)
			{
				StretchHorizontal(num, pageContext);
			}
			else if (!flag && base.ItemPageSizes.Left >= leftInParentSystem)
			{
				StretchHorizontal(num, pageContext);
			}
			PageItem[] children = m_children;
			foreach (PageItem pageItem2 in children)
			{
				if (pageItem2 != null)
				{
					pageItem2.ItemPageSizes.DeltaX = 0.0;
				}
			}
		}

		private void StretchHorizontal(double maxRightInThisSystem, PageContext pageContext)
		{
			if (maxRightInThisSystem + m_rightPadding > OriginalWidth)
			{
				double num = 0.0;
				if (!pageContext.ConsumeWhitespace)
				{
					num = m_definitionRightPadding;
				}
				maxRightInThisSystem = ((!(maxRightInThisSystem > OriginalWidth - num)) ? OriginalWidth : (maxRightInThisSystem + num));
			}
			else
			{
				maxRightInThisSystem += m_rightPadding;
			}
			base.ItemPageSizes.AdjustWidthTo(maxRightInThisSystem);
		}

		protected override bool ResolveKeepTogetherHorizontal(PageContext pageContext, double leftInParentSystem, double rightInParentSystem, bool resolveItem)
		{
			bool flag = base.ResolveKeepTogetherHorizontal(pageContext, leftInParentSystem, rightInParentSystem, resolveItem);
			if (m_children == null)
			{
				return flag;
			}
			if ((resolveItem && base.NeedResolve) || (!flag && base.UnresolvedCKTH))
			{
				base.UnresolvedCKTH = false;
				base.NeedResolve = false;
				DetermineContentHorizontalSize(pageContext, leftInParentSystem, rightInParentSystem, null, anyAncestorHasKT: false, hasUnpinnedAncestors: false, resolveState: true, resolveItem);
			}
			return flag;
		}

		internal override bool AddToPage(RPLWriter rplWriter, PageContext pageContext, double pageLeft, double pageTop, double pageRight, double pageBottom, RepeatState repeatState)
		{
			if (HitsCurrentPage(pageLeft, pageTop, pageRight, pageBottom))
			{
				WriteStartItemToStream(rplWriter, pageContext);
				OmitBorderOnPageBreak(rplWriter, pageLeft, pageTop, pageRight, pageBottom);
				List<PageItem> list = null;
				if (m_children != null)
				{
					double pageLeft2 = Math.Max(0.0, pageLeft - base.ItemPageSizes.Left);
					double pageTop2 = Math.Max(0.0, pageTop - base.ItemPageSizes.Top);
					double pageRight2 = pageRight - base.ItemPageSizes.Left;
					double pageBottom2 = pageBottom - base.ItemPageSizes.Top;
					double num = 0.0;
					list = new List<PageItem>();
					for (int i = 0; i < m_children.Length; i++)
					{
						PageItem pageItem = m_children[i];
						if (pageItem == null)
						{
							continue;
						}
						if (pageItem.AddToPage(rplWriter, pageContext, pageLeft2, pageTop2, pageRight2, pageBottom2, repeatState))
						{
							if (pageItem.ContentOnPage)
							{
								list.Add(pageItem);
							}
							if (pageItem.Release(pageBottom2, pageRight2))
							{
								if (repeatState == RepeatState.None && (pageItem.ContentOnPage || !pageContext.ConsumeWhitespace || m_bottomPadding <= 0.0))
								{
									m_children[i] = null;
								}
							}
							else
							{
								num = Math.Max(num, pageItem.OriginalRight);
							}
						}
						else
						{
							num = Math.Max(num, pageItem.OriginalRight);
						}
					}
					m_rightPadding = Math.Max(0.0, OriginalRight - num);
				}
				WriteEndItemToStream(rplWriter, list);
				return true;
			}
			return false;
		}

		internal override void ResetHorizontal(bool spanPages, double? width)
		{
			base.ResetHorizontal(spanPages, width);
			m_rightPadding = m_definitionRightPadding;
		}

		internal override void WriteStartItemToStream(RPLWriter rplWriter, PageContext pageContext)
		{
			if (rplWriter != null)
			{
				BinaryWriter binaryWriter = rplWriter.BinaryWriter;
				if (binaryWriter != null)
				{
					Stream baseStream = binaryWriter.BaseStream;
					m_offset = baseStream.Position;
					binaryWriter.Write(RPLFormatType);
					WriteElementProps(binaryWriter, rplWriter, pageContext, m_offset + 1);
				}
				else if (m_rplElement == null)
				{
					m_rplElement = CreateRPLElement();
					WriteElementProps(m_rplElement.ElementProps, pageContext);
				}
				else
				{
					m_rplElement = CreateRPLElement(m_rplElement.ElementProps, pageContext);
				}
			}
		}

		internal virtual void WriteEndItemToStream(RPLWriter rplWriter, List<PageItem> itemsOnPage)
		{
			if (rplWriter == null)
			{
				return;
			}
			BinaryWriter binaryWriter = rplWriter.BinaryWriter;
			int num = itemsOnPage?.Count ?? 0;
			long value = 0L;
			RPLItemMeasurement[] array = null;
			if (binaryWriter != null)
			{
				value = binaryWriter.BaseStream.Position;
				binaryWriter.Write((byte)16);
				binaryWriter.Write(m_offset);
				binaryWriter.Write(num);
			}
			else
			{
				array = new RPLItemMeasurement[num];
				((RPLContainer)m_rplElement).Children = array;
			}
			if (itemsOnPage != null)
			{
				for (int i = 0; i < num; i++)
				{
					PageItem pageItem = itemsOnPage[i];
					if (pageItem != null)
					{
						if (binaryWriter != null)
						{
							pageItem.WritePageItemSizes(binaryWriter);
						}
						else
						{
							array[i] = pageItem.WritePageItemSizes();
						}
					}
				}
			}
			if (binaryWriter != null)
			{
				m_offset = binaryWriter.BaseStream.Position;
				binaryWriter.Write((byte)254);
				binaryWriter.Write(value);
				binaryWriter.Write(byte.MaxValue);
			}
		}

		internal override void WriteItemSharedStyleProps(BinaryWriter spbifWriter, Style style, PageContext pageContext)
		{
			WriteStyleProp(style, spbifWriter, StyleAttributeNames.BackgroundColor, 34);
			WriteBackgroundImage(style, writeShared: true, spbifWriter, pageContext);
		}

		internal override void WriteItemSharedStyleProps(RPLStyleProps rplStyleProps, Style style, PageContext pageContext)
		{
			PageItem.WriteStyleProp(style, rplStyleProps, StyleAttributeNames.BackgroundColor, 34);
			WriteBackgroundImage(style, writeShared: true, rplStyleProps, pageContext);
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
			}
		}
	}
}
