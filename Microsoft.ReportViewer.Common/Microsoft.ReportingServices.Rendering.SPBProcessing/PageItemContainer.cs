using Microsoft.ReportingServices.Diagnostics.Utilities;
using Microsoft.ReportingServices.OnDemandReportRendering;
using Microsoft.ReportingServices.Rendering.RPLProcessing;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace Microsoft.ReportingServices.Rendering.SPBProcessing
{
	internal class PageItemContainer : PageItem, IComparer
	{
		protected PageItem[] m_children;

		protected int[] m_indexesTopToBottom;

		protected RepeatWithItem[] m_repeatWithItems;

		protected int[] m_indexesLeftToRight;

		protected bool m_itemsCreated;

		protected double m_prevPageEnd;

		private PageItem m_rightEdgeItem;

		internal PageItemContainer(ReportItem source, bool createForRepeat)
			: base(source)
		{
		}

		int IComparer.Compare(object o1, object o2)
		{
			int num = (int)o1;
			int num2 = (int)o2;
			if (m_children[num] == null || m_children[num2] == null)
			{
				return 0;
			}
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

		private void VerticalDependency(PageItem[] children)
		{
			PageItem pageItem = null;
			ItemSizes itemSizes = null;
			PageItem pageItem2 = null;
			ItemSizes itemSizes2 = null;
			RoundedDouble roundedDouble = new RoundedDouble(-1.0);
			RoundedDouble roundedDouble2 = new RoundedDouble(0.0);
			for (int i = 0; i < children.Length; i++)
			{
				pageItem = children[i];
				if (pageItem == null)
				{
					continue;
				}
				itemSizes = pageItem.ItemPageSizes;
				roundedDouble2.Value = itemSizes.Bottom;
				roundedDouble.Value = -1.0;
				for (int j = i + 1; j < children.Length; j++)
				{
					pageItem2 = children[j];
					if (pageItem2 == null)
					{
						continue;
					}
					itemSizes2 = pageItem2.ItemPageSizes;
					if (roundedDouble >= 0.0 && roundedDouble <= itemSizes2.Top)
					{
						break;
					}
					if (roundedDouble2 <= itemSizes2.Top)
					{
						if (pageItem2.PageItemsAbove == null)
						{
							pageItem2.PageItemsAbove = new List<int>();
						}
						pageItem2.PageItemsAbove.Add(i);
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
			PageItem pageItem = null;
			ItemSizes itemSizes = null;
			PageItem pageItem2 = null;
			ItemSizes itemSizes2 = null;
			int num = 0;
			RoundedDouble roundedDouble = new RoundedDouble(-1.0);
			RoundedDouble roundedDouble2 = new RoundedDouble(0.0);
			m_rightEdgeItem.PageItemsLeft = null;
			for (int i = 0; i < m_children.Length; i++)
			{
				pageItem = m_children[m_indexesLeftToRight[i]];
				if (pageItem == null)
				{
					continue;
				}
				itemSizes = pageItem.ItemPageSizes;
				roundedDouble.Value = -1.0;
				roundedDouble2.Value = itemSizes.Right;
				for (num = i + 1; num < m_children.Length; num++)
				{
					pageItem2 = m_children[m_indexesLeftToRight[num]];
					if (pageItem2 == null)
					{
						continue;
					}
					itemSizes2 = pageItem2.ItemPageSizes;
					if (roundedDouble >= 0.0 && roundedDouble <= itemSizes2.Left)
					{
						break;
					}
					if (roundedDouble2 <= itemSizes2.Left)
					{
						if (pageItem2.PageItemsLeft == null)
						{
							pageItem2.PageItemsLeft = new List<int>();
						}
						pageItem2.PageItemsLeft.Add(m_indexesLeftToRight[i]);
						if (roundedDouble < 0.0 || roundedDouble > itemSizes2.Right)
						{
							roundedDouble.Value = itemSizes2.Right;
						}
					}
				}
				if (num != m_children.Length)
				{
					continue;
				}
				pageItem2 = m_rightEdgeItem;
				itemSizes2 = pageItem2.ItemPageSizes;
				if ((roundedDouble < 0.0 || roundedDouble > itemSizes2.Left) && roundedDouble2 <= itemSizes2.Left)
				{
					if (pageItem2.PageItemsLeft == null)
					{
						pageItem2.PageItemsLeft = new List<int>();
					}
					pageItem2.PageItemsLeft.Add(m_indexesLeftToRight[i]);
					if (roundedDouble < 0.0 || roundedDouble > itemSizes2.Right)
					{
						roundedDouble.Value = itemSizes2.Right;
					}
				}
			}
		}

		private bool IsStaticTablix(Microsoft.ReportingServices.OnDemandReportRendering.Tablix tablix, TablixMember colMemberParent)
		{
			if (tablix == null && colMemberParent == null)
			{
				return true;
			}
			TablixMember tablixMember = null;
			TablixMemberCollection tablixMemberCollection = null;
			tablixMemberCollection = ((colMemberParent != null) ? colMemberParent.Children : tablix.ColumnHierarchy.MemberCollection);
			if (tablixMemberCollection == null)
			{
				return true;
			}
			for (int i = 0; i < tablixMemberCollection.Count; i++)
			{
				tablixMember = tablixMemberCollection[i];
				if (tablixMember.IsStatic)
				{
					if (tablixMember.Visibility != null && tablixMember.Visibility.HiddenState == SharedHiddenState.Sometimes)
					{
						return false;
					}
					if (!IsStaticTablix(null, tablixMember))
					{
						return false;
					}
					continue;
				}
				return false;
			}
			return true;
		}

		internal bool CreateChildren(ReportItemCollection childrenDef, PageContext pageContext, double parentWidth, double parentHeight)
		{
			return CreateChildren(childrenDef, pageContext, parentWidth, parentHeight, isSimple: false);
		}

		internal bool CreateChildren(ReportItemCollection childrenDef, PageContext pageContext, double parentWidth, double parentHeight, bool isSimple)
		{
			if (childrenDef == null || childrenDef.Count == 0)
			{
				return isSimple;
			}
			double num = 0.0;
			double num2 = 0.0;
			bool flag = isSimple;
			ReportItem reportItem = null;
			m_children = new PageItem[childrenDef.Count];
			m_indexesLeftToRight = new int[childrenDef.Count];
			for (int i = 0; i < childrenDef.Count; i++)
			{
				m_indexesLeftToRight[i] = i;
				reportItem = childrenDef[i];
				m_children[i] = PageItem.Create(reportItem, pageContext, tablixCellParent: false, createForRepeat: false);
				if (flag)
				{
					if (reportItem.Visibility != null && reportItem.Visibility.HiddenState == SharedHiddenState.Sometimes)
					{
						flag = false;
					}
					else
					{
						Microsoft.ReportingServices.OnDemandReportRendering.Tablix tablix = reportItem as Microsoft.ReportingServices.OnDemandReportRendering.Tablix;
						flag = IsStaticTablix(tablix, null);
						if (flag)
						{
							bool noRows = false;
							VerifyNoRows(reportItem, ref noRows);
							if (noRows)
							{
								flag = false;
							}
						}
					}
				}
				if (reportItem.RepeatedSibling)
				{
					if (m_repeatWithItems == null)
					{
						m_repeatWithItems = new RepeatWithItem[childrenDef.Count];
					}
					m_repeatWithItems[i] = new RepeatWithItem(PageItem.Create(reportItem, pageContext, tablixCellParent: false, createForRepeat: true), pageContext);
				}
				num = Math.Max(num, m_children[i].ItemPageSizes.Right);
				num2 = Math.Max(num2, m_children[i].ItemPageSizes.Bottom);
			}
			m_rightEdgeItem = new EdgePageItem(0.0, Math.Max(parentWidth, num), SourceID, pageContext);
			num = Math.Max(0.0, parentWidth - num);
			num2 = Math.Max(0.0, parentHeight - num2);
			m_itemPageSizes.SetPaddings(num, num2);
			Array.Sort(m_indexesLeftToRight, this);
			VerticalDependency(m_children);
			HorizontalDependecy();
			ResolveOverlappingItems(pageContext);
			return flag;
		}

		internal bool CreateChildrenFromPaginationState(ReportItemCollection childrenDef, PageContext pageContext, PageItemContainerHelper itemHelper, bool isSimple)
		{
			if (itemHelper == null || itemHelper.Children == null || itemHelper.Children.Length == 0)
			{
				return isSimple;
			}
			if (childrenDef == null || childrenDef.Count == 0)
			{
				return isSimple;
			}
			bool flag = isSimple;
			ReportItem reportItem = null;
			int num = 0;
			PageItemHelper[] children = itemHelper.Children;
			int[] indexesTopToBottom = itemHelper.IndexesTopToBottom;
			m_children = new PageItem[childrenDef.Count];
			RSTrace.RenderingTracer.Assert(m_children.Length == children.Length, "Mismatch between the number of children on page and the saved number of children for the page.");
			for (int i = 0; i < childrenDef.Count; i++)
			{
				num = i;
				if (indexesTopToBottom != null)
				{
					num = indexesTopToBottom[i];
				}
				reportItem = childrenDef[num];
				if (children[i] != null)
				{
					m_children[i] = PageItem.Create(reportItem, pageContext, tablixCellParent: false, createForRepeat: false);
					m_children[i].UpdateItem(children[i]);
				}
				if (flag)
				{
					if (reportItem.Visibility != null && reportItem.Visibility.HiddenState == SharedHiddenState.Sometimes)
					{
						flag = false;
					}
					else
					{
						Microsoft.ReportingServices.OnDemandReportRendering.Tablix tablix = reportItem as Microsoft.ReportingServices.OnDemandReportRendering.Tablix;
						flag = IsStaticTablix(tablix, null);
						if (flag)
						{
							bool noRows = false;
							VerifyNoRows(reportItem, ref noRows);
							if (noRows)
							{
								flag = false;
							}
						}
					}
				}
				if (reportItem.RepeatedSibling)
				{
					if (m_repeatWithItems == null)
					{
						m_repeatWithItems = new RepeatWithItem[childrenDef.Count];
					}
					m_repeatWithItems[i] = new RepeatWithItem(PageItem.Create(reportItem, pageContext, tablixCellParent: false, createForRepeat: true), pageContext);
				}
			}
			return flag;
		}

		internal void ResolveRepeatWithFromPaginationState(PageItemContainerHelper itemHelper, RPLWriter rplWriter, PageContext pageContext)
		{
			if (itemHelper == null || m_repeatWithItems == null || m_repeatWithItems.Length == 0)
			{
				return;
			}
			for (int i = 0; i < m_repeatWithItems.Length; i++)
			{
				if (m_repeatWithItems[i] != null)
				{
					m_repeatWithItems[i].UpdateItem(itemHelper.RepeatWithItems[i], rplWriter, pageContext);
				}
			}
		}

		internal void ResolveRepeatWith(ReportItemCollection childrenDef, PageContext pageContext)
		{
			if (childrenDef == null || m_repeatWithItems == null)
			{
				return;
			}
			DataRegion dataRegion = null;
			int[] array = null;
			PageItem[] array2 = null;
			int num = 0;
			int num2 = -1;
			RPLWriter rplWriter = new RPLWriter();
			PageContext pageContext2 = new PageContext(pageContext, PageContext.PageContextFlags.FullOnPage, PageContext.IgnorePBReasonFlag.Repeated);
			for (int i = 0; i < childrenDef.Count; i++)
			{
				dataRegion = (childrenDef[i] as DataRegion);
				if (dataRegion == null)
				{
					continue;
				}
				num2 = -1;
				array = dataRegion.GetRepeatSiblings();
				if (array == null || array.Length == 0)
				{
					continue;
				}
				array2 = new PageItem[array.Length + 1];
				for (int j = 0; j < array.Length; j++)
				{
					num = array[j];
					if (num < i)
					{
						if (m_repeatWithItems[num] != null)
						{
							array2[j] = m_repeatWithItems[num].SourcePageItem;
						}
						continue;
					}
					if (num2 < 0)
					{
						num2 = j;
						array2[j] = PageItem.Create(dataRegion, pageContext2, tablixCellParent: false, createForRepeat: true);
					}
					if (m_repeatWithItems[num] != null)
					{
						array2[j + 1] = m_repeatWithItems[num].SourcePageItem;
					}
				}
				if (num2 < 0)
				{
					num2 = array2.Length - 1;
					array2[num2] = PageItem.Create(dataRegion, pageContext2, tablixCellParent: false, createForRepeat: true);
				}
				VerticalDependency(array2);
				for (int k = 0; k < array2.Length; k++)
				{
					if (num2 == k)
					{
						array2[k].AdjustOriginFromItemsAbove(array2, null);
					}
					else if (array2[k] != null)
					{
						array2[k].CalculateRepeatWithPage(rplWriter, pageContext2, array2);
					}
				}
				for (int l = 0; l < array.Length; l++)
				{
					num = array[l];
					if (m_children[num] != null)
					{
						m_repeatWithItems[num].UpdateCreateState(array2[num2], i, m_children[num].PageItemsAbove, pageContext2);
					}
				}
			}
		}

		private void ResolveOverlappingItems(PageContext pageContext)
		{
			PageItem pageItem = null;
			bool verticalOverlap = false;
			bool itemsOverlap = false;
			double num = 0.0;
			PageItem[] defChildren = null;
			bool flag = false;
			DetectOverlappingItems(pageContext, out itemsOverlap, out verticalOverlap);
			while (itemsOverlap)
			{
				num = 0.0;
				if (verticalOverlap)
				{
					for (int i = 0; i < m_children.Length; i++)
					{
						pageItem = m_children[i];
						if (pageItem != null)
						{
							pageItem.AdjustOriginFromItemsAbove(m_children, null, adjustForRender: false);
							num = Math.Max(num, pageItem.ItemPageSizes.Bottom + base.ItemPageSizes.PaddingBottom);
						}
					}
					base.ItemPageSizes.AdjustHeightTo(num);
				}
				else
				{
					for (int j = 0; j < m_children.Length; j++)
					{
						pageItem = m_children[m_indexesLeftToRight[j]];
						if (pageItem != null)
						{
							pageItem.AdjustOriginFromItemsAtLeft(m_children, adjustForRender: false);
							num = Math.Max(num, pageItem.ItemPageSizes.Right + base.ItemPageSizes.PaddingRight);
						}
					}
					base.ItemPageSizes.AdjustWidthTo(num);
					m_rightEdgeItem.ItemPageSizes.Left = Math.Max(m_rightEdgeItem.ItemPageSizes.Left, num);
				}
				for (int k = 0; k < m_children.Length; k++)
				{
					pageItem = m_children[k];
					if (pageItem != null)
					{
						if (verticalOverlap)
						{
							pageItem.ItemPageSizes.DeltaY = 0.0;
							pageItem.PageItemsAbove = null;
						}
						else
						{
							pageItem.ItemPageSizes.DeltaX = 0.0;
							pageItem.PageItemsLeft = null;
						}
					}
				}
				if (verticalOverlap)
				{
					UpdateChildren(ref defChildren);
					VerticalDependency(m_children);
				}
				else
				{
					if (m_indexesTopToBottom != null)
					{
						for (int l = 0; l < m_indexesLeftToRight.Length; l++)
						{
							m_indexesLeftToRight[l] = l;
						}
					}
					Array.Sort(m_indexesLeftToRight, this);
					HorizontalDependecy();
					flag = true;
				}
				DetectOverlappingItems(pageContext, out itemsOverlap, out verticalOverlap);
			}
			if (!flag)
			{
				return;
			}
			for (int m = 0; m < m_children.Length; m++)
			{
				if (m_children[m] != null)
				{
					m_children[m].DefLeftValue = m_children[m].ItemPageSizes.Left;
				}
			}
		}

		private void UpdateChildren(ref PageItem[] defChildren)
		{
			if (m_indexesTopToBottom == null)
			{
				m_indexesTopToBottom = new int[m_children.Length];
				defChildren = new PageItem[m_children.Length];
				for (int i = 0; i < m_children.Length; i++)
				{
					m_indexesTopToBottom[i] = i;
					defChildren[i] = m_children[i];
				}
			}
			Array.Sort(m_indexesTopToBottom, new SortPageItemIndexesOnTop(defChildren));
			for (int j = 0; j < m_children.Length; j++)
			{
				m_children[j] = defChildren[m_indexesTopToBottom[j]];
			}
		}

		private static bool IsStraightLine(Line line)
		{
			if (line == null)
			{
				return false;
			}
			if (line.ItemPageSizes.Width != 0.0)
			{
				return line.ItemPageSizes.Height == 0.0;
			}
			return true;
		}

		private void RemoveItem(PageItem item, int index)
		{
			m_children[index] = null;
			if (item.RepeatedSibling)
			{
				m_repeatWithItems[index] = null;
			}
		}

		private void DetectOverlappingItems(PageContext pageContext, out bool itemsOverlap, out bool verticalOverlap)
		{
			itemsOverlap = false;
			verticalOverlap = false;
			PageItem pageItem = null;
			PageItem pageItem2 = null;
			Line line = null;
			Line line2 = null;
			RoundedDouble roundedDouble = new RoundedDouble(0.0, forOverlapDetection: true);
			RoundedDouble roundedDouble2 = new RoundedDouble(0.0, forOverlapDetection: true);
			RoundedDouble roundedDouble3 = new RoundedDouble(0.0, forOverlapDetection: true);
			RoundedDouble roundedDouble4 = new RoundedDouble(0.0, forOverlapDetection: true);
			for (int i = 0; i < m_children.Length; i++)
			{
				if (itemsOverlap)
				{
					break;
				}
				pageItem = m_children[i];
				if (pageItem == null || pageItem.HiddenForOverlap(pageContext))
				{
					continue;
				}
				line = (pageItem as Line);
				if (IsStraightLine(line))
				{
					continue;
				}
				roundedDouble.Value = pageItem.ItemPageSizes.Bottom;
				roundedDouble2.Value = pageItem.ItemPageSizes.Right;
				for (int j = i + 1; j < m_children.Length; j++)
				{
					if (itemsOverlap)
					{
						break;
					}
					pageItem2 = m_children[j];
					if (pageItem2 == null || pageItem.HiddenForOverlap(pageContext))
					{
						continue;
					}
					line2 = (pageItem2 as Line);
					if (IsStraightLine(line2))
					{
						continue;
					}
					roundedDouble3.Value = pageItem2.ItemPageSizes.Bottom;
					roundedDouble4.Value = pageItem2.ItemPageSizes.Right;
					if (roundedDouble <= pageItem2.ItemPageSizes.Top)
					{
						break;
					}
					if (roundedDouble2 <= pageItem2.ItemPageSizes.Left || roundedDouble4 <= pageItem.ItemPageSizes.Left)
					{
						continue;
					}
					if (line != null)
					{
						RemoveItem(line, i);
						continue;
					}
					if (line2 != null)
					{
						RemoveItem(line2, j);
						continue;
					}
					itemsOverlap = true;
					double num = roundedDouble.Value - pageItem2.ItemPageSizes.Top;
					double overlapY = Math.Min(num, pageItem2.ItemPageSizes.Height);
					if ((RoundedDouble)pageItem.ItemPageSizes.Left <= pageItem2.ItemPageSizes.Left)
					{
						MoveItems(roundedDouble2, pageItem2, pageItem2, overlapY, num, ref verticalOverlap);
					}
					else
					{
						MoveItems(roundedDouble4, pageItem, pageItem2, overlapY, num, ref verticalOverlap);
					}
				}
			}
		}

		private void MoveItems(RoundedDouble pinItemRight, PageItem moveItemRight, PageItem moveItemDown, double overlapY, double deltaY, ref bool verticalOverlap)
		{
			double num = 0.0;
			double num2 = 0.0;
			if (pinItemRight <= moveItemRight.ItemPageSizes.Right)
			{
				num = pinItemRight.Value - moveItemRight.ItemPageSizes.Left;
				num2 = num;
			}
			else
			{
				num = moveItemRight.ItemPageSizes.Width;
				num2 = pinItemRight.Value - moveItemRight.ItemPageSizes.Left;
			}
			if (num <= overlapY)
			{
				moveItemRight.ItemPageSizes.MoveHorizontal(num2);
				return;
			}
			verticalOverlap = true;
			moveItemDown.ItemPageSizes.MoveVertical(deltaY);
		}

		internal override bool CalculatePage(RPLWriter rplWriter, PageItemHelper lastPageInfo, PageContext pageContext, PageItem[] siblings, RepeatWithItem[] repeatWithItems, double parentTopInPage, ref double parentPageHeight, Interactivity interactivity)
		{
			return true;
		}

		internal override void CreateItemRenderSizes(ItemSizes contentSize, PageContext pageContext, bool createForRepeat)
		{
			if (contentSize == null)
			{
				if (pageContext != null)
				{
					if (createForRepeat)
					{
						m_itemRenderSizes = pageContext.GetSharedRenderFromRepeatItemSizesElement(m_itemPageSizes, isPadded: true, returnPaddings: true);
					}
					else
					{
						m_itemRenderSizes = pageContext.GetSharedRenderItemSizesElement(m_itemPageSizes, isPadded: true, returnPaddings: true);
					}
				}
				if (m_itemRenderSizes == null)
				{
					m_itemRenderSizes = new PaddItemSizes((PaddItemSizes)m_itemPageSizes);
				}
			}
			else
			{
				m_itemRenderSizes = contentSize;
			}
		}

		internal PageItem[] BringRepeatedWithOnPage(RPLWriter rplWriter, List<int> repeatedSiblings, PageContext pageContext)
		{
			if (m_children == null)
			{
				return null;
			}
			if (repeatedSiblings == null)
			{
				return m_children;
			}
			PageItem[] array = new PageItem[m_children.Length];
			PageItem pageItem = null;
			for (int i = 0; i < m_children.Length; i++)
			{
				pageItem = m_children[i];
				if (pageItem != null && ((pageItem.ItemState != State.Below && pageItem.ItemState != State.TopNextPage) || m_repeatWithItems[i] == null || repeatedSiblings.BinarySearch(i) < 0))
				{
					array[i] = m_children[i];
				}
			}
			int num = 0;
			PageItem pageItem2 = null;
			RepeatWithItem repeatWithItem = null;
			List<int> parentOverlappedItems = null;
			double header = 0.0;
			for (int num2 = repeatedSiblings.Count - 1; num2 >= 0; num2--)
			{
				num = repeatedSiblings[num2];
				if (array[num] == null)
				{
					repeatWithItem = m_repeatWithItems[num];
					if (repeatWithItem != null)
					{
						pageItem2 = m_children[repeatWithItem.DataRegionIndex];
						if (!repeatWithItem.AddOnPage(pageItem2.ItemRenderSizes, array, num, ref parentOverlappedItems, ref header))
						{
							repeatedSiblings.RemoveAt(num2);
						}
						else
						{
							array[num] = repeatWithItem.SourcePageItem;
						}
					}
				}
				else
				{
					repeatedSiblings.RemoveAt(num2);
				}
			}
			if (header < 0.0)
			{
				for (int j = 0; j < array.Length; j++)
				{
					pageItem = array[j];
					if (pageItem != null && pageItem.ItemState != State.Below && pageItem.ItemState != State.TopNextPage)
					{
						pageItem.ItemRenderSizes.Top -= header;
					}
				}
			}
			for (int k = 0; k < repeatedSiblings.Count; k++)
			{
				num = repeatedSiblings[k];
				m_repeatWithItems[num]?.WriteRepeatWithToPage(rplWriter, pageContext);
			}
			if (parentOverlappedItems != null)
			{
				for (int l = 0; l < parentOverlappedItems.Count; l++)
				{
					array[parentOverlappedItems[l]].AdjustOriginFromRepeatItems(array);
				}
			}
			return array;
		}

		protected void ConsumeWhitespaceHorizontal(ItemSizes itemSizes, double adjustWidthTo, PageContext pageContext)
		{
			if (!pageContext.ConsumeContainerWhitespace)
			{
				itemSizes.AdjustWidthTo(adjustWidthTo);
				return;
			}
			double num = adjustWidthTo - itemSizes.Width;
			itemSizes.AdjustWidthTo(adjustWidthTo);
			if (!(num <= 0.0) && !(itemSizes.PaddingRight <= 0.0))
			{
				double num2 = num - itemSizes.PaddingRight;
				if (num2 > 0.0)
				{
					itemSizes.AdjustWidthTo(itemSizes.PadWidth);
					itemSizes.PaddingRight = 0.0;
				}
				else
				{
					itemSizes.AdjustWidthTo(itemSizes.Width - num);
					itemSizes.PaddingRight = 0.0 - num2;
				}
			}
		}

		protected void ConsumeWhitespaceVertical(ItemSizes itemSizes, double adjustHeightTo, PageContext pageContext)
		{
			if (!pageContext.ConsumeContainerWhitespace)
			{
				itemSizes.AdjustHeightTo(adjustHeightTo);
				return;
			}
			double num = adjustHeightTo - itemSizes.Height;
			itemSizes.AdjustHeightTo(adjustHeightTo);
			if (!(num <= 0.0) && !(itemSizes.PaddingBottom <= 0.0))
			{
				double num2 = num - itemSizes.PaddingBottom;
				if (num2 > 0.0)
				{
					itemSizes.AdjustHeightTo(itemSizes.PadHeight);
					itemSizes.PaddingBottom = 0.0;
				}
				else
				{
					itemSizes.AdjustHeightTo(itemSizes.Height - num);
					itemSizes.PaddingBottom = 0.0 - num2;
				}
			}
		}

		internal void CalculateRepeatWithRenderSizes(PageContext pageContext)
		{
			if (m_children == null)
			{
				return;
			}
			PageItem pageItem = null;
			double num = m_itemRenderSizes.PaddingBottom;
			for (int i = 0; i < m_children.Length; i++)
			{
				pageItem = m_children[i];
				if (pageItem != null)
				{
					pageItem.AdjustOriginFromItemsAbove(m_children, null, adjustForRender: true);
					num = Math.Max(num, pageItem.ItemRenderSizes.Bottom + m_itemRenderSizes.PaddingBottom);
				}
			}
			ConsumeWhitespaceVertical(m_itemRenderSizes, num, pageContext);
			num = m_itemRenderSizes.PaddingRight;
			for (int j = 0; j < m_children.Length; j++)
			{
				pageItem = m_children[m_indexesLeftToRight[j]];
				if (pageItem != null)
				{
					pageItem.AdjustOriginFromItemsAtLeft(m_children, adjustForRender: true);
					num = Math.Max(num, pageItem.ItemRenderSizes.Right + m_itemRenderSizes.PaddingRight);
				}
			}
			if (m_rightEdgeItem != null)
			{
				if (pageContext != null)
				{
					m_rightEdgeItem.ItemRenderSizes = pageContext.GetSharedRenderEdgeItemSizesElement(m_rightEdgeItem.ItemPageSizes);
				}
				else
				{
					m_rightEdgeItem.ItemRenderSizes = new ItemSizes(m_rightEdgeItem.ItemPageSizes);
				}
				m_rightEdgeItem.AdjustOriginFromItemsAtLeft(m_children, adjustForRender: true);
				ConsumeWhitespaceHorizontal(m_itemRenderSizes, m_rightEdgeItem.ItemRenderSizes.Right, pageContext);
			}
			else
			{
				ConsumeWhitespaceHorizontal(m_itemRenderSizes, num, pageContext);
			}
		}

		internal int CalculateRenderSizes(RPLWriter rplWriter, PageContext pageContext, Interactivity interactivity, List<int> repeatedSiblings, out PageItem[] childrenOnPage)
		{
			childrenOnPage = BringRepeatedWithOnPage(rplWriter, repeatedSiblings, pageContext);
			if (childrenOnPage == null)
			{
				return 0;
			}
			PageItem pageItem = null;
			double num = m_itemRenderSizes.PaddingBottom;
			int num2 = 0;
			for (int i = 0; i < childrenOnPage.Length; i++)
			{
				pageItem = childrenOnPage[i];
				if (pageItem != null && pageItem.ItemState != State.Below && pageItem.ItemState != State.TopNextPage)
				{
					if (pageItem.ItemState != State.OnPageHidden && !(pageItem is NoRowsItem))
					{
						num2++;
					}
					RegisterItem.RegisterPageItem(pageItem, pageContext, pageContext.EvaluatePageHeaderFooter, interactivity);
					pageItem.AdjustOriginFromItemsAbove(childrenOnPage, m_repeatWithItems, adjustForRender: true);
					num = Math.Max(num, pageItem.ItemRenderSizes.Bottom + m_itemRenderSizes.PaddingBottom);
				}
			}
			ConsumeWhitespaceVertical(m_itemRenderSizes, num, pageContext);
			num = m_itemRenderSizes.PaddingRight;
			for (int j = 0; j < childrenOnPage.Length; j++)
			{
				pageItem = childrenOnPage[m_indexesLeftToRight[j]];
				if (pageItem != null && pageItem.ItemState != State.Below && pageItem.ItemState != State.TopNextPage)
				{
					pageItem.AdjustOriginFromItemsAtLeft(childrenOnPage, adjustForRender: true);
					num = Math.Max(num, pageItem.ItemRenderSizes.Right + m_itemRenderSizes.PaddingRight);
				}
			}
			if (m_rightEdgeItem != null)
			{
				if (pageContext != null)
				{
					m_rightEdgeItem.ItemRenderSizes = pageContext.GetSharedRenderEdgeItemSizesElement(m_rightEdgeItem.ItemPageSizes);
				}
				else
				{
					m_rightEdgeItem.ItemRenderSizes = new ItemSizes(m_rightEdgeItem.ItemPageSizes);
				}
				m_rightEdgeItem.AdjustOriginFromItemsAtLeft(childrenOnPage, adjustForRender: true);
				ConsumeWhitespaceHorizontal(m_itemRenderSizes, m_rightEdgeItem.ItemRenderSizes.Right, pageContext);
			}
			else
			{
				ConsumeWhitespaceHorizontal(m_itemRenderSizes, num, pageContext);
			}
			return num2;
		}

		internal void UpdateItemPageState(PageContext pageContext, bool omitBorderOnPageBreak)
		{
			if (m_itemState == State.SpanPages)
			{
				if (omitBorderOnPageBreak)
				{
					m_rplItemState |= 1;
				}
				m_itemState = State.OnPage;
				PageItem pageItem = null;
				for (int i = 0; i < m_children.Length; i++)
				{
					pageItem = m_children[i];
					if (pageItem != null)
					{
						pageItem.UpdateSizes(m_prevPageEnd, m_children, m_repeatWithItems);
						if (pageItem.ItemState == State.Below)
						{
							pageItem.ItemState = State.Unknow;
						}
						else if (pageItem.ItemState == State.TopNextPage)
						{
							pageItem.ItemState = State.OnPage;
						}
					}
				}
			}
			double num = m_prevPageEnd - base.ItemPageSizes.PadHeight;
			m_itemPageSizes.AdjustHeightTo(base.ItemPageSizes.Height - m_prevPageEnd);
			if (num > 0.0)
			{
				m_itemPageSizes.PaddingBottom = m_itemPageSizes.Height;
			}
			m_prevPageEnd = 0.0;
			if (m_repeatWithItems == null)
			{
				return;
			}
			for (int j = 0; j < m_repeatWithItems.Length; j++)
			{
				if (m_repeatWithItems[j] != null)
				{
					m_repeatWithItems[j].UpdateSizes(pageContext);
				}
			}
		}

		internal override void UpdateItem(PageItemHelper itemHelper)
		{
			if (itemHelper != null)
			{
				base.UpdateItem(itemHelper);
				PageItemContainerHelper pageItemContainerHelper = itemHelper as PageItemContainerHelper;
				RSTrace.RenderingTracer.Assert(pageItemContainerHelper != null, "This should be a container object");
				m_itemsCreated = pageItemContainerHelper.ItemsCreated;
				m_prevPageEnd = pageItemContainerHelper.PrevPageEnd;
				if (pageItemContainerHelper.RightEdgeItem != null)
				{
					m_rightEdgeItem = new EdgePageItem(pageItemContainerHelper.RightEdgeItem.ItemPageSizes.Top, pageItemContainerHelper.RightEdgeItem.ItemPageSizes.Left, SourceID, null);
					m_rightEdgeItem.UpdateItem(pageItemContainerHelper.RightEdgeItem);
				}
				if (pageItemContainerHelper.IndexesLeftToRight != null)
				{
					m_indexesLeftToRight = PageItem.GetNewArray(pageItemContainerHelper.IndexesLeftToRight);
				}
				if (pageItemContainerHelper.IndexesTopToBottom != null)
				{
					m_indexesTopToBottom = PageItem.GetNewArray(pageItemContainerHelper.IndexesTopToBottom);
				}
			}
		}

		internal void ResolveVerticalDependencyList(List<int> pageItemsAbove, int index)
		{
			if (pageItemsAbove == null || m_repeatWithItems == null)
			{
				return;
			}
			List<int> list = null;
			for (int i = 0; i < pageItemsAbove.Count; i++)
			{
				if (m_repeatWithItems[pageItemsAbove[i]] != null && list == null)
				{
					list = new List<int>();
					list.Add(pageItemsAbove[i]);
				}
			}
			if (list != null)
			{
				for (int j = index + 1; j < m_children.Length; j++)
				{
					FixItemsAbove(m_children[j], index, list);
				}
			}
		}

		private void FixItemsAbove(PageItem item, int index, List<int> repeatedItems)
		{
			if (item == null)
			{
				return;
			}
			List<int> pageItemsAbove = item.PageItemsAbove;
			if (pageItemsAbove == null)
			{
				return;
			}
			int num = pageItemsAbove.BinarySearch(index);
			if (num < 0)
			{
				return;
			}
			pageItemsAbove.RemoveAt(num);
			Hashtable hashtable = new Hashtable();
			CollectAllItems(pageItemsAbove, hashtable);
			if (hashtable.Count > 0)
			{
				for (int i = 0; i < repeatedItems.Count; i++)
				{
					if (hashtable.ContainsKey(repeatedItems[i]))
					{
						repeatedItems.RemoveAt(i);
						i--;
					}
				}
			}
			int j = 0;
			for (int k = 0; k < repeatedItems.Count; k++)
			{
				for (; j < pageItemsAbove.Count; j++)
				{
					if (pageItemsAbove[j] >= repeatedItems[k])
					{
						pageItemsAbove.Insert(j, repeatedItems[k]);
						j++;
						break;
					}
				}
				if (j == pageItemsAbove.Count)
				{
					pageItemsAbove.Add(repeatedItems[k]);
					j++;
				}
			}
		}

		private void CollectAllItems(List<int> srcList, Hashtable destList)
		{
			if (srcList == null)
			{
				return;
			}
			PageItem pageItem = null;
			for (int i = 0; i < srcList.Count; i++)
			{
				if (!destList.ContainsKey(srcList[i]))
				{
					destList.Add(srcList[i], srcList[i]);
				}
				pageItem = m_children[srcList[i]];
				if (pageItem != null)
				{
					CollectAllItems(pageItem.PageItemsAbove, destList);
				}
			}
		}

		internal void ReleaseChildrenOnPage()
		{
			if (m_children == null)
			{
				return;
			}
			PageItem pageItem = null;
			for (int i = 0; i < m_children.Length; i++)
			{
				pageItem = m_children[i];
				if (pageItem != null && (pageItem.ItemState == State.OnPage || pageItem.ItemState == State.OnPagePBEnd || pageItem.ItemState == State.OnPageHidden))
				{
					if (!pageItem.RepeatedSibling)
					{
						ResolveVerticalDependencyList(pageItem.PageItemsAbove, i);
					}
					m_children[i] = null;
				}
			}
		}

		private void VerifyNoRows(ReportItem reportItem, ref bool noRows)
		{
			if (reportItem == null)
			{
				return;
			}
			DataRegion dataRegion = reportItem as DataRegion;
			if (dataRegion != null)
			{
				if (((DataRegionInstance)dataRegion.Instance).NoRows)
				{
					noRows = true;
				}
				return;
			}
			Microsoft.ReportingServices.OnDemandReportRendering.SubReport subReport = reportItem as Microsoft.ReportingServices.OnDemandReportRendering.SubReport;
			if (subReport != null)
			{
				SubReportInstance subReportInstance = (SubReportInstance)subReport.Instance;
				if (subReportInstance.NoRows || subReportInstance.ProcessedWithError)
				{
					noRows = true;
				}
			}
		}

		internal virtual void WriteEndItemToStream(RPLWriter rplWriter, int itemsOnPage, PageItem[] childrenOnPage)
		{
			if (rplWriter == null)
			{
				return;
			}
			BinaryWriter binaryWriter = rplWriter.BinaryWriter;
			int num = (childrenOnPage != null) ? childrenOnPage.Length : 0;
			long value = 0L;
			RPLItemMeasurement[] array = null;
			if (binaryWriter != null)
			{
				value = binaryWriter.BaseStream.Position;
				binaryWriter.Write((byte)16);
				binaryWriter.Write(m_offset);
				binaryWriter.Write(itemsOnPage);
			}
			else if (itemsOnPage > 0)
			{
				array = new RPLItemMeasurement[itemsOnPage];
				((RPLContainer)m_rplElement).Children = array;
			}
			PageItem pageItem = null;
			int num2 = 0;
			for (int i = 0; i < num; i++)
			{
				pageItem = childrenOnPage[i];
				if (pageItem == null || pageItem.ItemState == State.Below || pageItem.ItemState == State.TopNextPage)
				{
					continue;
				}
				if (pageItem.ItemState == State.OnPageHidden || pageItem is NoRowsItem)
				{
					if (!pageItem.RepeatedSibling)
					{
						ResolveVerticalDependencyList(pageItem.PageItemsAbove, i);
					}
					childrenOnPage[i] = null;
					m_children[i] = null;
					continue;
				}
				if (binaryWriter != null)
				{
					pageItem.WritePageItemRenderSizes(binaryWriter);
				}
				else
				{
					array[num2] = pageItem.WritePageItemRenderSizes();
					num2++;
				}
				if (pageItem.ItemState == State.OnPage || pageItem.ItemState == State.OnPagePBEnd)
				{
					if (!pageItem.RepeatedSibling)
					{
						ResolveVerticalDependencyList(pageItem.PageItemsAbove, i);
					}
					childrenOnPage[i] = null;
					m_children[i] = null;
				}
				else
				{
					pageItem.ItemRenderSizes = null;
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

		internal void WriteRepeatWithEndItemToStream(RPLWriter rplWriter, int itemsOnPage)
		{
			if (rplWriter == null)
			{
				return;
			}
			BinaryWriter binaryWriter = rplWriter.BinaryWriter;
			int num = (m_children != null) ? m_children.Length : 0;
			long value = 0L;
			RPLItemMeasurement[] array = null;
			if (binaryWriter != null)
			{
				value = binaryWriter.BaseStream.Position;
				binaryWriter.Write((byte)16);
				binaryWriter.Write(m_offset);
				binaryWriter.Write(itemsOnPage);
			}
			else if (itemsOnPage > 0)
			{
				array = new RPLItemMeasurement[itemsOnPage];
				((RPLContainer)m_rplElement).Children = array;
			}
			PageItem pageItem = null;
			int num2 = 0;
			for (int i = 0; i < num; i++)
			{
				pageItem = m_children[i];
				if (pageItem != null && pageItem.ItemState != State.OnPageHidden)
				{
					if (binaryWriter != null)
					{
						pageItem.WritePageItemRenderSizes(binaryWriter);
						continue;
					}
					array[num2] = pageItem.WritePageItemRenderSizes();
					num2++;
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
			WriteBackgroundImage(spbifWriter, style, writeShared: true, pageContext);
		}

		internal override void WriteItemSharedStyleProps(RPLStyleProps rplStyleProps, Style style, PageContext pageContext)
		{
			WriteStyleProp(style, rplStyleProps, StyleAttributeNames.BackgroundColor, 34);
			WriteBackgroundImage(rplStyleProps, style, writeShared: true, pageContext);
		}

		internal override void WriteItemNonSharedStyleProp(BinaryWriter spbifWriter, Style styleDef, StyleInstance style, StyleAttributeNames styleAtt, PageContext pageContext)
		{
			switch (styleAtt)
			{
			case StyleAttributeNames.BackgroundColor:
				WriteStyleProp(styleDef, style, spbifWriter, StyleAttributeNames.BackgroundColor, 34);
				break;
			case StyleAttributeNames.BackgroundImage:
				WriteBackgroundImage(spbifWriter, styleDef, writeShared: false, pageContext);
				break;
			}
		}

		internal override void WriteItemNonSharedStyleProp(RPLStyleProps rplStyleProps, Style styleDef, StyleInstance style, StyleAttributeNames styleAtt, PageContext pageContext)
		{
			switch (styleAtt)
			{
			case StyleAttributeNames.BackgroundColor:
				WriteStyleProp(styleDef, style, rplStyleProps, StyleAttributeNames.BackgroundColor, 34);
				break;
			case StyleAttributeNames.BackgroundImage:
				WriteBackgroundImage(rplStyleProps, styleDef, writeShared: false, pageContext);
				break;
			}
		}

		internal override void WritePaginationInfo(BinaryWriter reportPageInfo)
		{
			if (reportPageInfo != null)
			{
				reportPageInfo.Write((byte)5);
				WritePaginationInfoProperties(reportPageInfo);
				reportPageInfo.Write(byte.MaxValue);
			}
		}

		internal override PageItemHelper WritePaginationInfo()
		{
			PageItemHelper pageItemHelper = new PageItemContainerHelper(5);
			WritePaginationInfoProperties(pageItemHelper);
			return pageItemHelper;
		}

		internal override void WritePaginationInfoProperties(PageItemHelper itemHelper)
		{
			if (itemHelper == null)
			{
				return;
			}
			PageItemContainerHelper pageItemContainerHelper = itemHelper as PageItemContainerHelper;
			RSTrace.RenderingTracer.Assert(pageItemContainerHelper != null, "This should be a container object");
			base.WritePaginationInfoProperties(pageItemContainerHelper);
			pageItemContainerHelper.ItemsCreated = m_itemsCreated;
			pageItemContainerHelper.PrevPageEnd = m_prevPageEnd;
			if (m_rightEdgeItem != null)
			{
				pageItemContainerHelper.RightEdgeItem = m_rightEdgeItem.WritePaginationInfo();
			}
			pageItemContainerHelper.IndexesLeftToRight = PageItem.GetNewArray(m_indexesLeftToRight);
			pageItemContainerHelper.IndexesTopToBottom = PageItem.GetNewArray(m_indexesTopToBottom);
			if (m_children != null && m_children.Length != 0)
			{
				pageItemContainerHelper.Children = new PageItemHelper[m_children.Length];
				for (int i = 0; i < m_children.Length; i++)
				{
					if (m_children[i] != null)
					{
						pageItemContainerHelper.Children[i] = m_children[i].WritePaginationInfo();
					}
				}
			}
			if (m_repeatWithItems == null || m_repeatWithItems.Length == 0)
			{
				return;
			}
			pageItemContainerHelper.RepeatWithItems = new PageItemRepeatWithHelper[m_repeatWithItems.Length];
			for (int j = 0; j < m_repeatWithItems.Length; j++)
			{
				if (m_repeatWithItems[j] != null)
				{
					pageItemContainerHelper.RepeatWithItems[j] = m_repeatWithItems[j].WritePaginationInfo();
				}
			}
		}

		internal override void WritePaginationInfoProperties(BinaryWriter reportPageInfo)
		{
			if (reportPageInfo == null)
			{
				return;
			}
			base.WritePaginationInfoProperties(reportPageInfo);
			reportPageInfo.Write((byte)6);
			reportPageInfo.Write(m_itemsCreated);
			reportPageInfo.Write((byte)11);
			reportPageInfo.Write(m_prevPageEnd);
			if (m_rightEdgeItem != null)
			{
				reportPageInfo.Write((byte)9);
				m_rightEdgeItem.WritePaginationInfo(reportPageInfo);
			}
			if (m_indexesLeftToRight != null && m_indexesLeftToRight.Length != 0)
			{
				reportPageInfo.Write((byte)7);
				reportPageInfo.Write(m_indexesLeftToRight.Length);
				for (int i = 0; i < m_indexesLeftToRight.Length; i++)
				{
					reportPageInfo.Write(m_indexesLeftToRight[i]);
				}
			}
			if (m_indexesTopToBottom != null && m_indexesTopToBottom.Length != 0)
			{
				reportPageInfo.Write((byte)20);
				reportPageInfo.Write(m_indexesTopToBottom.Length);
				for (int j = 0; j < m_indexesTopToBottom.Length; j++)
				{
					reportPageInfo.Write(m_indexesTopToBottom[j]);
				}
			}
			if (m_children != null && m_children.Length != 0)
			{
				reportPageInfo.Write((byte)10);
				reportPageInfo.Write(m_children.Length);
				for (int k = 0; k < m_children.Length; k++)
				{
					if (m_children[k] != null)
					{
						m_children[k].WritePaginationInfo(reportPageInfo);
						continue;
					}
					reportPageInfo.Write((byte)14);
					reportPageInfo.Write(byte.MaxValue);
				}
			}
			if (m_repeatWithItems == null || m_repeatWithItems.Length == 0)
			{
				return;
			}
			reportPageInfo.Write((byte)8);
			reportPageInfo.Write(m_repeatWithItems.Length);
			for (int l = 0; l < m_repeatWithItems.Length; l++)
			{
				if (m_repeatWithItems[l] != null)
				{
					m_repeatWithItems[l].WritePaginationInfo(reportPageInfo);
					continue;
				}
				reportPageInfo.Write((byte)14);
				reportPageInfo.Write(byte.MaxValue);
			}
		}
	}
}
