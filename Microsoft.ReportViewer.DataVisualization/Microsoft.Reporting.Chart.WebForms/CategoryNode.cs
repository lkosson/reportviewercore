using System;
using System.Collections.Generic;

namespace Microsoft.Reporting.Chart.WebForms
{
	internal sealed class CategoryNode
	{
		internal sealed class Values
		{
			public double Value
			{
				get;
				private set;
			}

			public double AbsoluteValue
			{
				get;
				private set;
			}

			public Values()
			{
				Value = 0.0;
				AbsoluteValue = 0.0;
			}

			public void AddValues(double value, double absoluteValue)
			{
				Value += value;
				AbsoluteValue += absoluteValue;
			}
		}

		private Dictionary<Series, DataPoint> _dataPoints;

		private Dictionary<Series, Values> _values;

		private CategoryNodeCollection _parentCollection;

		public int Index
		{
			get;
			private set;
		}

		public bool Empty
		{
			get;
			private set;
		}

		public string Label
		{
			get;
			private set;
		}

		public string ToolTip
		{
			get;
			set;
		}

		public string Href
		{
			get;
			set;
		}

		public string LabelToolTip
		{
			get;
			set;
		}

		public string LabelHref
		{
			get;
			set;
		}

		public CategoryNode Parent => _parentCollection.Parent;

		public CategoryNodeCollection Children
		{
			get;
			set;
		}

		public CategoryNode(CategoryNodeCollection parentCollection, bool empty, string label)
		{
			_parentCollection = parentCollection;
			Empty = empty;
			Label = label;
			ToolTip = label;
			Href = string.Empty;
			LabelToolTip = string.Empty;
			LabelHref = string.Empty;
		}

		public void AddDataPoint(DataPoint dataPoint)
		{
			if (_dataPoints == null)
			{
				_dataPoints = new Dictionary<Series, DataPoint>();
			}
			_dataPoints[dataPoint.series] = dataPoint;
		}

		public DataPoint GetDataPoint(Series series)
		{
			if (_dataPoints == null)
			{
				return null;
			}
			_dataPoints.TryGetValue(series, out DataPoint value);
			return value;
		}

		public CategoryNode GetDataPointNode(Series series)
		{
			if (_dataPoints != null)
			{
				return this;
			}
			if (Children == null || !Children.AreAllNodesEmpty(series))
			{
				return null;
			}
			return Children.GetEmptyNode()?.GetDataPointNode(series);
		}

		public int GetDepth()
		{
			if (Empty)
			{
				return 0;
			}
			if (Children == null)
			{
				return 1;
			}
			return Children.GetDepth() + 1;
		}

		public void CalculateIndices(ref int dataPointIndex)
		{
			if (Children == null)
			{
				dataPointIndex++;
				Index = dataPointIndex;
			}
			else
			{
				Index = -1;
				Children.CalculateIndices(ref dataPointIndex);
			}
		}

		public void CalculateValues(List<Series> seriesCollection)
		{
			foreach (Series item in seriesCollection)
			{
				GetValues(item);
			}
		}

		public double GetTotalAbsoluteValue()
		{
			double num = 0.0;
			foreach (Series key in _values.Keys)
			{
				num += GetValues(key).AbsoluteValue;
			}
			return num;
		}

		public Values GetValues(Series series)
		{
			if (_values == null)
			{
				_values = new Dictionary<Series, Values>();
			}
			if (!_values.TryGetValue(series, out Values value))
			{
				value = new Values();
				if (Children == null)
				{
					double value2;
					double absoluteValue;
					DataPoint value3;
					if (_dataPoints == null)
					{
						value2 = (absoluteValue = 0.0);
					}
					else if (!_dataPoints.TryGetValue(series, out value3) || value3.Empty)
					{
						value2 = (absoluteValue = 0.0);
					}
					else
					{
						value2 = value3.YValues[0];
						absoluteValue = Math.Abs(value2);
					}
					value.AddValues(value2, absoluteValue);
				}
				else
				{
					foreach (CategoryNode child in Children)
					{
						Values values = child.GetValues(series);
						value.AddValues(values.Value, values.AbsoluteValue);
					}
				}
				_values.Add(series, value);
			}
			return value;
		}
	}
}
