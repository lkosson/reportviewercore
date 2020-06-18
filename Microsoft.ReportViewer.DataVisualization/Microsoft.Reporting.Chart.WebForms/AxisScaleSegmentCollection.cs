using System;
using System.Collections;

namespace Microsoft.Reporting.Chart.WebForms
{
	[SRDescription("DescriptionAttributeAxisScaleSegmentCollection_AxisScaleSegmentCollection")]
	internal class AxisScaleSegmentCollection : CollectionBase
	{
		private Axis axis;

		private AxisScaleSegment enforcedSegment;

		internal bool AllowOutOfScaleValues;

		[SRDescription("DescriptionAttributeAxisScaleSegmentCollection_Item")]
		public AxisScaleSegment this[int index]
		{
			get
			{
				return (AxisScaleSegment)base.List[index];
			}
			set
			{
				base.List[index] = value;
			}
		}

		public AxisScaleSegmentCollection()
		{
		}

		internal AxisScaleSegmentCollection(Axis axis)
		{
			this.axis = axis;
		}

		public void Remove(AxisScaleSegment segment)
		{
			if (segment != null)
			{
				base.List.Remove(segment);
			}
		}

		public int Add(AxisScaleSegment segment)
		{
			return base.List.Add(segment);
		}

		public bool Contains(AxisScaleSegment value)
		{
			return base.List.Contains(value);
		}

		public int IndexOf(AxisScaleSegment value)
		{
			return base.List.IndexOf(value);
		}

		public void Insert(int index, AxisScaleSegment value)
		{
			base.List.Insert(index, value);
		}

		protected override void OnInsertComplete(int index, object value)
		{
			((AxisScaleSegment)value).axis = axis;
		}

		protected override void OnSetComplete(int index, object oldValue, object newValue)
		{
			((AxisScaleSegment)newValue).axis = axis;
		}

		internal void EnforceSegment(AxisScaleSegment segment)
		{
			enforcedSegment = segment;
		}

		public AxisScaleSegment FindScaleSegmentForAxisValue(double axisValue)
		{
			if (base.List.Count == 0)
			{
				return null;
			}
			if (enforcedSegment != null)
			{
				return enforcedSegment;
			}
			for (int i = 0; i < base.Count; i++)
			{
				if (axisValue < this[i].ScaleMinimum)
				{
					if (i == 0)
					{
						return this[i];
					}
					if (Math.Abs(this[i].ScaleMinimum - axisValue) < Math.Abs(axisValue - this[i - 1].ScaleMaximum))
					{
						return this[i];
					}
					return this[i - 1];
				}
				if (axisValue <= this[i].ScaleMaximum)
				{
					return this[i];
				}
				if (i == base.Count - 1)
				{
					return this[i];
				}
			}
			return null;
		}
	}
}
