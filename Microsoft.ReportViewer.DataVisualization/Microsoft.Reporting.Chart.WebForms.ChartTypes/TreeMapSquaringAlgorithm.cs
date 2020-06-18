using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Microsoft.Reporting.Chart.WebForms.ChartTypes
{
	internal sealed class TreeMapSquaringAlgorithm
	{
		private sealed class TreeMapDataPointInfo
		{
			public RectangleF Rect
			{
				get;
				set;
			}

			public TreeMapNode Node
			{
				get;
				set;
			}

			public TreeMapDataPointInfo(RectangleF rect, TreeMapNode dataPoint)
			{
				Rect = rect;
				Node = dataPoint;
			}
		}

		private IList<TreeMapNode> dataPoint;

		private RectangleF currentRectangle;

		private int currentStart;

		private double factor;

		private IEnumerable<TreeMapDataPointInfo> Split(RectangleF parentRectangle, double totalValue, IEnumerable<TreeMapNode> children)
		{
			if (children == null || !children.Any() || totalValue == 0.0)
			{
				return Enumerable.Empty<TreeMapDataPointInfo>();
			}
			if (parentRectangle.Width <= 0f || parentRectangle.Height <= 0f)
			{
				return children.Select((TreeMapNode child) => new TreeMapDataPointInfo(new RectangleF(0f, 0f, 0f, 0f), child));
			}
			currentRectangle = new RectangleF(parentRectangle.X, parentRectangle.Y, parentRectangle.Width, parentRectangle.Height);
			dataPoint = (from child in children
				where child.Value != 0.0
				orderby child.Value descending
				select child).ToArray();
			factor = (double)(currentRectangle.Width * currentRectangle.Height) / totalValue;
			return BuildTreeMap().ToArray();
		}

		private IEnumerable<TreeMapDataPointInfo> BuildTreeMap()
		{
			currentStart = 0;
			while (currentStart < dataPoint.Count)
			{
				foreach (TreeMapDataPointInfo item in BuildTreeMapStep())
				{
					yield return item;
				}
			}
		}

		private IEnumerable<TreeMapDataPointInfo> BuildTreeMapStep()
		{
			int last = currentStart;
			double num = 0.0;
			double num2 = double.PositiveInfinity;
			double widthOrHeight = 0.0;
			bool horizontal = currentRectangle.Width > currentRectangle.Height;
			for (; last < dataPoint.Count; last++)
			{
				num += GetArea(last);
				widthOrHeight = num / (double)(horizontal ? currentRectangle.Height : currentRectangle.Width);
				double num3 = Math.Max(GetAspect(currentStart, widthOrHeight), GetAspect(last, widthOrHeight));
				if (num3 > num2)
				{
					num -= GetArea(last);
					widthOrHeight = num / (double)(horizontal ? currentRectangle.Height : currentRectangle.Width);
					last--;
					break;
				}
				num2 = num3;
			}
			if (last == dataPoint.Count)
			{
				last--;
			}
			double x = currentRectangle.Left;
			double y = currentRectangle.Top;
			for (int i = currentStart; i <= last; i++)
			{
				if (horizontal)
				{
					double h = GetArea(i) / widthOrHeight;
					RectangleF rect = new RectangleF((float)x, (float)y, (float)widthOrHeight, (float)h);
					yield return new TreeMapDataPointInfo(rect, dataPoint[i]);
					y += h;
				}
				else
				{
					double w = GetArea(i) / widthOrHeight;
					RectangleF rect2 = new RectangleF((float)x, (float)y, (float)w, (float)widthOrHeight);
					yield return new TreeMapDataPointInfo(rect2, dataPoint[i]);
					x += w;
				}
			}
			currentStart = last + 1;
			if (horizontal)
			{
				currentRectangle = new RectangleF((float)((double)currentRectangle.Left + widthOrHeight), currentRectangle.Top, (float)Math.Max(0.0, (double)currentRectangle.Width - widthOrHeight), currentRectangle.Height);
			}
			else
			{
				currentRectangle = new RectangleF(currentRectangle.Left, (float)((double)currentRectangle.Top + widthOrHeight), currentRectangle.Width, (float)Math.Max(0.0, (double)currentRectangle.Height - widthOrHeight));
			}
		}

		private double GetArea(int i)
		{
			return dataPoint[i].Value * factor;
		}

		private double GetAspect(int i, double width)
		{
			double num = GetArea(i) / (width * width);
			if (num < 1.0)
			{
				num = 1.0 / num;
			}
			return num;
		}

		public static void CalculateRectangles(RectangleF containerRect, IEnumerable<TreeMapNode> treeMapNodes, double value)
		{
			foreach (TreeMapDataPointInfo item in new TreeMapSquaringAlgorithm().Split(containerRect, value, treeMapNodes))
			{
				RectangleF rect = item.Rect;
				TreeMapNode node = item.Node;
				node.Rectangle = rect;
				if (node.Children != null)
				{
					CalculateRectangles(node.Rectangle, node.Children, node.Value);
				}
			}
		}
	}
}
