using Microsoft.ReportingServices.Rendering.RPLProcessing;
using System.Collections.Generic;
using System.Drawing;

namespace Microsoft.Reporting.WinForms
{
	internal class RenderingItemContainer : RenderingItem
	{
		internal class VHSort : IComparer<int>
		{
			private List<RenderingItem> m_collection;

			internal VHSort(List<RenderingItem> collection)
			{
				m_collection = collection;
			}

			public int Compare(int index1, int index2)
			{
				if (m_collection[index1].Position.Top == m_collection[index2].Position.Top)
				{
					if (m_collection[index1].Position.Left < m_collection[index2].Position.Left)
					{
						return -1;
					}
					if (m_collection[index1].Position.Left == m_collection[index2].Position.Left)
					{
						return 0;
					}
					return 1;
				}
				if (!(m_collection[index1].Position.Top < m_collection[index2].Position.Top))
				{
					return 1;
				}
				return -1;
			}
		}

		private List<int> m_sortedRenderingItemIndices;

		private List<RenderingItem> m_children;

		internal List<int> SortedRenderingItemIndices
		{
			get
			{
				EnsureSortedRenderingItemIndices();
				return m_sortedRenderingItemIndices;
			}
		}

		internal List<RenderingItem> Children
		{
			get
			{
				if (m_children == null)
				{
					m_children = new List<RenderingItem>();
				}
				return m_children;
			}
		}

		internal void EnsureSortedRenderingItemIndices()
		{
			if (m_sortedRenderingItemIndices == null && Children.Count != 0)
			{
				m_sortedRenderingItemIndices = new List<int>(Children.Count);
				for (int i = 0; i < Children.Count; i++)
				{
					m_sortedRenderingItemIndices.Add(i);
				}
				m_sortedRenderingItemIndices.Sort(new VHSort(Children));
			}
		}

		internal override void ProcessRenderingElementContent(RPLElement rplElement, GdiContext context, RectangleF bounds)
		{
			RPLContainer rPLContainer = (RPLContainer)rplElement;
			if (rPLContainer.Children == null)
			{
				return;
			}
			if (rPLContainer.Children.Length == 1)
			{
				RenderingItem renderingItem = RenderingItem.CreateRenderingItem(context, rPLContainer.Children[0], bounds);
				if (renderingItem != null)
				{
					Children.Add(renderingItem);
				}
				rPLContainer.Children = null;
				return;
			}
			List<RPLItemMeasurement> list = new List<RPLItemMeasurement>(rPLContainer.Children.Length);
			for (int i = 0; i < rPLContainer.Children.Length; i++)
			{
				list.Add(rPLContainer.Children[i]);
			}
			rPLContainer.Children = null;
			list.Sort(new ZIndexComparer());
			for (int j = 0; j < list.Count; j++)
			{
				RenderingItem renderingItem2 = RenderingItem.CreateRenderingItem(context, list[j], bounds);
				if (renderingItem2 != null)
				{
					Children.Add(renderingItem2);
				}
				list[j] = null;
			}
		}

		internal override void DrawContent(GdiContext context)
		{
			foreach (RenderingItem child in Children)
			{
				child.DrawToPage(context);
			}
		}

		internal void Search(GdiContext context)
		{
			EnsureSortedRenderingItemIndices();
			if (m_sortedRenderingItemIndices == null)
			{
				return;
			}
			for (int i = 0; i < m_sortedRenderingItemIndices.Count; i++)
			{
				RenderingItem renderingItem = Children[m_sortedRenderingItemIndices[i]];
				if (renderingItem is RenderingItemContainer)
				{
					((RenderingItemContainer)renderingItem).Search(context);
				}
				else if (renderingItem is RenderingTextBox)
				{
					((RenderingTextBox)renderingItem).Search(context);
				}
			}
		}
	}
}
