using Microsoft.Reporting.Chart.WebForms.Formulas;
using System;
using System.Collections;
using System.Drawing;

namespace Microsoft.Reporting.Chart.WebForms
{
	internal class FinancialMarkersCollection : IList, ICollection, IEnumerable
	{
		private ArrayList array = new ArrayList();

		internal Series series;

		private FinancialMarkers markers;

		public bool IsReadOnly => array.IsReadOnly;

		public bool IsFixedSize => array.IsFixedSize;

		public bool IsSynchronized => array.IsSynchronized;

		public int Count => array.Count;

		public object SyncRoot => array.SyncRoot;

		object IList.this[int index]
		{
			get
			{
				return array[index];
			}
			set
			{
				array[index] = value;
			}
		}

		[SRDescription("DescriptionAttributeFinancialMarkersCollection_Item")]
		public FinancialMarker this[object parameter]
		{
			get
			{
				if (parameter is int)
				{
					return (FinancialMarker)array[(int)parameter];
				}
				throw new ArgumentException(SR.ExceptionInvalidIndexerArgumentType);
			}
			set
			{
				if (parameter is int)
				{
					array[(int)parameter] = value;
					Invalidate();
					return;
				}
				throw new ArgumentException(SR.ExceptionInvalidIndexerArgumentType);
			}
		}

		public int Add(FinancialMarkerType markerName, int firstPointIndex, int secondPointIndex, int firstYIndex, int secondYIndex, Color lineColor, int lineWidth, Color textColor, Font textFont)
		{
			FinancialMarker value = new FinancialMarker(markerName, firstPointIndex, secondPointIndex, firstYIndex, secondYIndex, lineColor, lineWidth, textColor, textFont);
			int result = array.Add(value);
			Invalidate();
			return result;
		}

		public int Add(FinancialMarkerType markerName, int firstPointIndex, int secondPointIndex, int firstYIndex, int secondYIndex, Color lineColor, int lineWidth)
		{
			return Add(markerName, firstPointIndex, secondPointIndex, firstYIndex, secondYIndex, lineColor, lineWidth, Color.Black, new Font(ChartPicture.GetDefaultFontFamilyName(), 8f));
		}

		public int Add(FinancialMarkerType markerName, int firstPointIndex, int secondPointIndex, int firstYIndex, int secondYIndex)
		{
			return Add(markerName, firstPointIndex, secondPointIndex, firstYIndex, secondYIndex, Color.Gray, 1, Color.Black, new Font(ChartPicture.GetDefaultFontFamilyName(), 8f));
		}

		public int Add(FinancialMarkerType markerName, int firstPointIndex, int secondPointIndex)
		{
			return Add(markerName, firstPointIndex, secondPointIndex, 0, 0, Color.Gray, 1, Color.Black, new Font(ChartPicture.GetDefaultFontFamilyName(), 8f));
		}

		public int Add(object value)
		{
			if (!(value is FinancialMarker))
			{
				throw new ArgumentException(SR.ExceptionFinancialMarkerObjectRequired);
			}
			Invalidate();
			return array.Add(value);
		}

		public void Insert(int index, FinancialMarkerType markerName, int firstPointIndex, int secondPointIndex, int firstYIndex, int secondYIndex, Color lineColor, int lineWidth, Color textColor, Font textFont)
		{
			FinancialMarker value = new FinancialMarker(markerName, firstPointIndex, secondPointIndex, firstYIndex, secondYIndex, lineColor, lineWidth, textColor, textFont);
			array.Insert(index, value);
		}

		public void Insert(int index, FinancialMarkerType markerName, int firstPointIndex, int secondPointIndex, int firstYIndex, int secondYIndex, Color lineColor, int lineWidth)
		{
			Insert(index, markerName, firstPointIndex, secondPointIndex, firstYIndex, secondYIndex, lineColor, lineWidth, Color.Black, new Font(ChartPicture.GetDefaultFontFamilyName(), 8f));
		}

		public void Insert(int index, FinancialMarkerType markerName, int firstPointIndex, int secondPointIndex, int firstYIndex, int secondYIndex)
		{
			Insert(index, markerName, firstPointIndex, secondPointIndex, firstYIndex, secondYIndex, Color.Gray, 1, Color.Black, new Font(ChartPicture.GetDefaultFontFamilyName(), 8f));
		}

		public void Insert(int index, FinancialMarkerType markerName, int firstPointIndex, int secondPointIndex)
		{
			Insert(index, markerName, firstPointIndex, secondPointIndex, 0, 0, Color.Gray, 1, Color.Black, new Font(ChartPicture.GetDefaultFontFamilyName(), 8f));
		}

		public void Insert(int index, FinancialMarker value)
		{
			Insert(index, (object)value);
		}

		public void Insert(int index, object value)
		{
			if (!(value is FinancialMarker))
			{
				throw new ArgumentException(SR.ExceptionFinancialMarkerObjectRequired);
			}
			Invalidate();
			array.Insert(index, value);
		}

		int IList.IndexOf(object value)
		{
			return array.IndexOf(value);
		}

		bool IList.Contains(object value)
		{
			return array.Contains(value);
		}

		void IList.Remove(object value)
		{
			array.Remove(value);
			Invalidate();
		}

		public int IndexOf(FinancialMarker value)
		{
			return array.IndexOf(value);
		}

		public bool Contains(FinancialMarker value)
		{
			return array.Contains(value);
		}

		public void Remove(FinancialMarker value)
		{
			array.Remove(value);
			Invalidate();
		}

		public void RemoveAt(int index)
		{
			array.RemoveAt(index);
			Invalidate();
		}

		public void Clear()
		{
			array.Clear();
			Invalidate();
		}

		public IEnumerator GetEnumerator()
		{
			return array.GetEnumerator();
		}

		public void CopyTo(Array array, int index)
		{
			this.array.CopyTo(array, index);
			Invalidate();
		}

		public FinancialMarkersCollection()
		{
			markers = new FinancialMarkers();
		}

		internal void DrawMarkers(ChartGraphics graph, ChartPicture chart)
		{
			if (array == null || array.Count == 0)
			{
				return;
			}
			foreach (FinancialMarker item in array)
			{
				markers.DrawMarkers(graph, chart, item.MarkerType, series, item.FirstPointIndex, item.FirstYIndex, item.SecondPointIndex, item.SecondYIndex, item.LineColor, item.LineWidth, item.LineStyle, item.TextColor, item.Font);
			}
		}

		private void Invalidate()
		{
		}
	}
}
