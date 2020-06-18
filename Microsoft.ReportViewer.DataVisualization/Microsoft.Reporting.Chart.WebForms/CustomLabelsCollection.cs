using Microsoft.Reporting.Chart.WebForms.Utilities;
using System;
using System.Collections;

namespace Microsoft.Reporting.Chart.WebForms
{
	[SRDescription("DescriptionAttributeCustomLabelsCollection_CustomLabelsCollection")]
	internal class CustomLabelsCollection : IList, ICollection, IEnumerable
	{
		private ArrayList array = new ArrayList();

		internal Axis axis;

		public CustomLabel this[int index]
		{
			get
			{
				return (CustomLabel)array[index];
			}
			set
			{
				array[index] = value;
				Invalidate();
			}
		}

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

		public bool IsFixedSize => array.IsFixedSize;

		public bool IsReadOnly => array.IsReadOnly;

		public int Count => array.Count;

		public bool IsSynchronized => array.IsSynchronized;

		public object SyncRoot => array.SyncRoot;

		private CustomLabelsCollection()
		{
		}

		public CustomLabelsCollection(Axis axis)
		{
			this.axis = axis;
		}

		public int Add(double fromPosition, double toPosition, string text)
		{
			CustomLabel customLabel = new CustomLabel(fromPosition, toPosition, text, 0, LabelMark.None);
			customLabel.axis = axis;
			Invalidate();
			return array.Add(customLabel);
		}

		internal int Add(double fromPosition, double toPosition, string text, bool customLabel)
		{
			CustomLabel customLabel2 = new CustomLabel(fromPosition, toPosition, text, 0, LabelMark.None);
			customLabel2.customLabel = customLabel;
			customLabel2.axis = axis;
			Invalidate();
			return array.Add(customLabel2);
		}

		public int Add(double fromPosition, double toPosition, string text, LabelRow row, LabelMark mark)
		{
			CustomLabel customLabel = new CustomLabel(fromPosition, toPosition, text, row, mark);
			customLabel.axis = axis;
			Invalidate();
			return array.Add(customLabel);
		}

		public int Add(double fromPosition, double toPosition, string text, int rowIndex, LabelMark mark)
		{
			CustomLabel customLabel = new CustomLabel(fromPosition, toPosition, text, rowIndex, mark);
			customLabel.axis = axis;
			Invalidate();
			return array.Add(customLabel);
		}

		public int Add(double fromPosition, double toPosition, string text, LabelRow row, LabelMark mark, GridTicks gridTick)
		{
			CustomLabel customLabel = new CustomLabel(fromPosition, toPosition, text, row, mark, gridTick);
			customLabel.axis = axis;
			Invalidate();
			return array.Add(customLabel);
		}

		public int Add(double fromPosition, double toPosition, string text, int rowIndex, LabelMark mark, GridTicks gridTick)
		{
			CustomLabel customLabel = new CustomLabel(fromPosition, toPosition, text, rowIndex, mark, gridTick);
			customLabel.axis = axis;
			Invalidate();
			return array.Add(customLabel);
		}

		public void Add(double labelsStep, DateTimeIntervalType intervalType, double min, double max, string format, LabelRow row, LabelMark mark)
		{
			Add(labelsStep, intervalType, min, max, format, (row != 0) ? 1 : 0, mark);
		}

		public void Add(double labelsStep, DateTimeIntervalType intervalType, double min, double max, string format, int rowIndex, LabelMark mark)
		{
			if (min == 0.0 && max == 0.0 && axis != null && !double.IsNaN(axis.Minimum) && !double.IsNaN(axis.Maximum))
			{
				min = axis.Minimum;
				max = axis.Maximum;
			}
			double num = Math.Min(min, max);
			double num2 = Math.Max(min, max);
			double num3 = num;
			double num4 = 0.0;
			while (num3 < num2)
			{
				switch (intervalType)
				{
				case DateTimeIntervalType.Number:
					num4 = num3 + labelsStep;
					break;
				case DateTimeIntervalType.Milliseconds:
					num4 = DateTime.FromOADate(num3).AddMilliseconds(labelsStep).ToOADate();
					break;
				case DateTimeIntervalType.Seconds:
					num4 = DateTime.FromOADate(num3).AddSeconds(labelsStep).ToOADate();
					break;
				case DateTimeIntervalType.Minutes:
					num4 = DateTime.FromOADate(num3).AddMinutes(labelsStep).ToOADate();
					break;
				case DateTimeIntervalType.Hours:
					num4 = DateTime.FromOADate(num3).AddHours(labelsStep).ToOADate();
					break;
				case DateTimeIntervalType.Days:
					num4 = DateTime.FromOADate(num3).AddDays(labelsStep).ToOADate();
					break;
				case DateTimeIntervalType.Weeks:
					num4 = DateTime.FromOADate(num3).AddDays(7.0 * labelsStep).ToOADate();
					break;
				case DateTimeIntervalType.Months:
					num4 = DateTime.FromOADate(num3).AddMonths((int)labelsStep).ToOADate();
					break;
				case DateTimeIntervalType.Years:
					num4 = DateTime.FromOADate(num3).AddYears((int)labelsStep).ToOADate();
					break;
				default:
					throw new ArgumentException(SR.ExceptionAxisLabelsIntervalTypeUnsupported(intervalType.ToString()));
				}
				if (num4 > num2)
				{
					num4 = num2;
				}
				ChartValueTypes valueType = ChartValueTypes.Double;
				if (intervalType != DateTimeIntervalType.Number)
				{
					valueType = ((axis.GetAxisValuesType() != ChartValueTypes.DateTimeOffset) ? ChartValueTypes.DateTime : ChartValueTypes.DateTimeOffset);
				}
				string text = ValueConverter.FormatValue(axis.chart, axis, num3 + (num4 - num3) / 2.0, format, valueType, ChartElementType.AxisLabels);
				CustomLabel customLabel = new CustomLabel(num3, num4, text, rowIndex, mark);
				customLabel.axis = axis;
				array.Add(customLabel);
				num3 = num4;
			}
			Invalidate();
		}

		public void Add(double labelsStep, DateTimeIntervalType intervalType)
		{
			Add(labelsStep, intervalType, 0.0, 0.0, "", 0, LabelMark.None);
			Invalidate();
		}

		public void Add(double labelsStep, DateTimeIntervalType intervalType, string format)
		{
			Add(labelsStep, intervalType, 0.0, 0.0, format, 0, LabelMark.None);
			Invalidate();
		}

		public void Add(double labelsStep, DateTimeIntervalType intervalType, string format, LabelRow row, LabelMark mark)
		{
			Add(labelsStep, intervalType, 0.0, 0.0, format, row, mark);
			Invalidate();
		}

		public void Add(double labelsStep, DateTimeIntervalType intervalType, string format, int rowIndex, LabelMark mark)
		{
			Add(labelsStep, intervalType, 0.0, 0.0, format, rowIndex, mark);
			Invalidate();
		}

		public void Clear()
		{
			array.Clear();
			Invalidate();
		}

		bool IList.Contains(object value)
		{
			return array.Contains(value);
		}

		public bool Contains(CustomLabel value)
		{
			return array.Contains(value);
		}

		int IList.IndexOf(object value)
		{
			return array.IndexOf(value);
		}

		public int IndexOf(CustomLabel value)
		{
			return array.IndexOf(value);
		}

		void IList.Remove(object value)
		{
			array.Remove(value);
			Invalidate();
		}

		public void Remove(CustomLabel value)
		{
			array.Remove(value);
			Invalidate();
		}

		public void RemoveAt(int index)
		{
			array.RemoveAt(index);
			Invalidate();
		}

		public int Add(object value)
		{
			if (!(value is CustomLabel))
			{
				throw new ArgumentException(SR.ExceptionCustomLabelAddedHasWrongType);
			}
			((CustomLabel)value).axis = axis;
			Invalidate();
			return array.Add(value);
		}

		public void Insert(int index, CustomLabel value)
		{
			Insert(index, (object)value);
		}

		public void Insert(int index, object value)
		{
			if (!(value is CustomLabel))
			{
				throw new ArgumentException(SR.ExceptionCustomLabelInsertedHasWrongType);
			}
			((CustomLabel)value).axis = axis;
			array.Insert(index, value);
			Invalidate();
		}

		public void CopyTo(Array array, int index)
		{
			this.array.CopyTo(array, index);
			Invalidate();
		}

		public IEnumerator GetEnumerator()
		{
			return array.GetEnumerator();
		}

		private void Invalidate()
		{
		}
	}
}
