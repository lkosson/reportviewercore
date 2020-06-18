using System;
using System.Collections;
using System.ComponentModel;

namespace Microsoft.Reporting.Gauge.WebForms
{
	internal class Range : NamedElement
	{
		private Hashtable inRangeTable = new Hashtable();

		private double startValue = 70.0;

		private double endValue = 100.0;

		private double inRangeTimeout;

		private PeriodType inRangeTimeoutType = PeriodType.Seconds;

		public virtual double StartValue
		{
			get
			{
				return startValue;
			}
			set
			{
				startValue = value;
				InvalidateState();
				Invalidate();
			}
		}

		public virtual double EndValue
		{
			get
			{
				return endValue;
			}
			set
			{
				endValue = value;
				InvalidateState();
				Invalidate();
			}
		}

		[Browsable(false)]
		[DefaultValue(0.0)]
		public virtual double InRangeTimeout
		{
			get
			{
				return inRangeTimeout;
			}
			set
			{
				if (value < 0.0)
				{
					throw new ArgumentException(Utils.SRGetStr("ExceptionNegativeValue"));
				}
				inRangeTimeout = value;
			}
		}

		[Browsable(false)]
		[DefaultValue(PeriodType.Seconds)]
		public virtual PeriodType InRangeTimeoutType
		{
			get
			{
				return inRangeTimeoutType;
			}
			set
			{
				inRangeTimeoutType = value;
			}
		}

		public Range()
		{
		}

		protected Range(double start, double end)
		{
			startValue = start;
			endValue = end;
		}

		internal RangeDataState GetDataState(DataAttributes data)
		{
			if (inRangeTable.ContainsKey(data))
			{
				return (RangeDataState)inRangeTable[data];
			}
			RangeDataState rangeDataState = new RangeDataState(this, data);
			inRangeTable.Add(data, rangeDataState);
			return rangeDataState;
		}

		private void InvalidateState()
		{
			foreach (DataAttributes key in inRangeTable.Keys)
			{
				PointerValueChanged(key);
			}
		}

		internal virtual void OnValueRangeTimeOut(object sender, ValueRangeEventArgs e)
		{
			if (Common != null)
			{
				Common.GaugeContainer.OnValueRangeTimeOut(sender, e);
			}
		}

		internal virtual void OnValueRangeEnter(object sender, ValueRangeEventArgs e)
		{
			if (Common != null)
			{
				Common.GaugeContainer.OnValueRangeEnter(sender, e);
			}
		}

		internal virtual void OnValueRangeLeave(object sender, ValueRangeEventArgs e)
		{
			if (Common != null)
			{
				Common.GaugeContainer.OnValueRangeLeave(sender, e);
			}
		}

		internal virtual void PointerValueChanged(DataAttributes data)
		{
			double num = Math.Min(startValue, endValue);
			double num2 = Math.Max(startValue, endValue);
			if (Common == null)
			{
				return;
			}
			bool playbackMode = false;
			if (((IValueConsumer)data).GetProvider() != null)
			{
				playbackMode = ((IValueConsumer)data).GetProvider().GetPlayBackMode();
			}
			NamedElement pointer = (NamedElement)data.Parent;
			RangeDataState dataState = GetDataState(data);
			double num3 = data.Value;
			if (data.IsPercentBased)
			{
				num3 = data.GetValueInPercents();
			}
			if (!dataState.IsInRange && num3 >= num && num3 <= num2)
			{
				dataState.IsInRange = true;
				OnValueRangeEnter(this, new ValueRangeEventArgs(num3, data.DateValueStamp, Name, playbackMode, pointer));
				if (!(inRangeTimeout > 0.0))
				{
					dataState.IsTimerExceed = true;
				}
			}
			if (dataState.IsInRange && (double.IsNaN(num3) || num3 < num || num3 > num2))
			{
				dataState.IsInRange = false;
				dataState.IsTimerExceed = false;
				OnValueRangeLeave(this, new ValueRangeEventArgs(num3, data.DateValueStamp, Name, playbackMode, pointer));
				_ = inRangeTimeout;
				_ = 0.0;
			}
		}

		internal override void Notify(MessageType msg, NamedElement element, object param)
		{
			base.Notify(msg, element, param);
			if (msg != MessageType.NamedElementRemove)
			{
				return;
			}
			ArrayList arrayList = new ArrayList();
			foreach (DataAttributes key in inRangeTable.Keys)
			{
				if (key.Parent == element)
				{
					arrayList.Add(key);
				}
			}
			for (int i = 0; i < arrayList.Count; i++)
			{
				inRangeTable.Remove(arrayList[i]);
			}
			arrayList.Clear();
		}

		protected override void OnDispose()
		{
			inRangeTable.Clear();
			base.OnDispose();
		}
	}
}
