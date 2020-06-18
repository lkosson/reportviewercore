using System;
using System.Collections;
using System.Collections.Generic;

namespace Microsoft.Reporting.Chart.WebForms
{
	internal sealed class CategoryNodeCollection : IEnumerable<CategoryNode>, IEnumerable
	{
		private readonly List<CategoryNode> _nodes;

		public CategoryNode Parent
		{
			get;
			private set;
		}

		public bool AllNodesEmpty
		{
			get;
			private set;
		}

		public CategoryNodeCollection(CategoryNode parent)
		{
			Parent = parent;
			_nodes = new List<CategoryNode>();
			AllNodesEmpty = true;
		}

		public IEnumerator<CategoryNode> GetEnumerator()
		{
			return _nodes.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public void Add(CategoryNode node)
		{
			AllNodesEmpty &= node.Empty;
			_nodes.Add(node);
		}

		public int GetDepth()
		{
			int num = 0;
			foreach (CategoryNode node in _nodes)
			{
				num = Math.Max(num, node.GetDepth());
			}
			return num;
		}

		public void Calculate(List<Series> seriesCollection)
		{
			int dataPointIndex = -1;
			CalculateIndices(ref dataPointIndex);
			CalculateValues(seriesCollection);
		}

		private void CalculateValues(List<Series> seriesCollection)
		{
			foreach (CategoryNode node in _nodes)
			{
				node.CalculateValues(seriesCollection);
			}
		}

		public void CalculateIndices(ref int dataPointIndex)
		{
			foreach (CategoryNode node in _nodes)
			{
				node.CalculateIndices(ref dataPointIndex);
			}
		}

		public double GetTotalAbsoluetValue()
		{
			double num = 0.0;
			foreach (CategoryNode node in _nodes)
			{
				num += node.GetTotalAbsoluteValue();
			}
			return num;
		}

		public void SortByAbsoluteValue(Series series)
		{
			_nodes.Sort((CategoryNode node1, CategoryNode node2) => node2.GetValues(series).AbsoluteValue.CompareTo(node1.GetValues(series).AbsoluteValue));
		}

		public double GetTotalAbsoluteValue(Series series)
		{
			double num = 0.0;
			foreach (CategoryNode node in _nodes)
			{
				num += node.GetValues(series).AbsoluteValue;
			}
			return num;
		}

		public bool AreAllNodesEmpty(Series series)
		{
			if (AllNodesEmpty)
			{
				return true;
			}
			foreach (CategoryNode node in _nodes)
			{
				if (!node.Empty && node.GetValues(series).AbsoluteValue != 0.0)
				{
					return false;
				}
			}
			return true;
		}

		public CategoryNode GetEmptyNode()
		{
			foreach (CategoryNode node in _nodes)
			{
				if (node.Empty)
				{
					return node;
				}
			}
			return null;
		}
	}
}
