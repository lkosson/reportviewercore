using System;
using System.Collections;
using System.ComponentModel;

namespace Microsoft.Reporting.Gauge.WebForms
{
	[Serializable]
	internal class ValueBase : NamedElement, IValueProvider, ICloneable
	{
		[NonSerialized]
		internal GaugeDuration historyDept = new GaugeDuration(0.0, DurationType.Count);

		[NonSerialized]
		internal GaugeDuration queryDept = new GaugeDuration(1.0, DurationType.Count);

		private double valueLimit = double.NaN;

		internal double inputValue = double.NaN;

		internal DateTime inputDate = DateTime.Now;

		internal double outputValue = double.NaN;

		internal DateTime outputDate = DateTime.Now;

		internal HistoryCollection history;

		internal ArrayList consumers = new ArrayList();

		internal ValueState provderState = ValueState.Interactive;

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public HistoryCollection History => history;

		[SRDescription("DescriptionAttributeName")]
		[Browsable(true)]
		[SRCategory("CategoryMisc")]
		[NotifyParentProperty(true)]
		public override string Name
		{
			get
			{
				return base.Name;
			}
			set
			{
				base.Name = value;
			}
		}

		[SRCategory("CategoryData")]
		[SRDescription("DescriptionAttributeDate")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[SerializationVisibility(SerializationVisibility.Hidden)]
		[Browsable(false)]
		public DateTime Date => outputDate;

		[SRCategory("CategoryBehavior")]
		[Bindable(true)]
		[DefaultValue(double.NaN)]
		[SRDescription("DescriptionAttributeValueLimit")]
		[TypeConverter(typeof(DoubleNanValueConverter))]
		internal virtual double ValueLimit
		{
			get
			{
				return valueLimit;
			}
			set
			{
				valueLimit = value;
			}
		}

		internal virtual GaugeDuration HistoryDeptInternal => historyDept;

		[SRCategory("CategoryMisc")]
		[Bindable(false)]
		[Browsable(false)]
		[SRDescription("DescriptionAttributeState")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[SerializationVisibility(SerializationVisibility.Hidden)]
		internal virtual ValueState State
		{
			get
			{
				if (IntState == ValueState.Suspended)
				{
					return ValueState.Playback;
				}
				return IntState;
			}
		}

		internal virtual ValueState IntState
		{
			get
			{
				return provderState;
			}
			set
			{
				provderState = value;
				foreach (object consumer in consumers)
				{
					if (consumer is ValueBase)
					{
						((ValueBase)consumer).IntState = value;
					}
				}
			}
		}

		internal bool IsPlayBackMode => provderState == ValueState.Playback;

		internal bool IsEventsEnable
		{
			get
			{
				if (provderState != ValueState.Interactive)
				{
					return provderState == ValueState.Playback;
				}
				return true;
			}
		}

		internal event ValueChangedEventHandler ValueChanged;

		internal event ValueChangedEventHandler ValueLimitOverflow;

		internal ValueBase()
		{
			history = new HistoryCollection(this);
		}

		public override string ToString()
		{
			return Name;
		}

		public virtual void Reset()
		{
			History.Clear();
			queryDept = new GaugeDuration(historyDept.Count, historyDept.DurationType);
			foreach (IValueConsumer consumer in consumers)
			{
				consumer.Reset();
			}
		}

		internal virtual void SetValueInternal(double value)
		{
			SetValueInternal(value, DateTime.Now);
		}

		internal virtual void SetValueInternal(double value, DateTime timestamp)
		{
			inputValue = value;
			inputDate = timestamp;
			Recalculate(inputValue, inputDate);
		}

		public virtual double GetValue()
		{
			return outputValue;
		}

		internal void ClearUpHistory()
		{
			if (History.Count > 0)
			{
				queryDept.Extend(historyDept, inputDate, History[0].Timestamp);
			}
			else
			{
				queryDept.Extend(historyDept, inputDate, inputDate);
			}
			History.Truncate(queryDept);
		}

		internal void AddToHistory()
		{
			if (!queryDept.IsEmpty || History.Count == 0)
			{
				AddToHistoryInt(outputValue, outputDate);
			}
		}

		internal void AddToHistoryInt(double value, DateTime timestamp)
		{
			History.Add(timestamp, value);
		}

		internal virtual void OnValueLimitOverflow(ValueChangedEventArgs e)
		{
			if (Common != null)
			{
				Common.GaugeContainer.OnValueLimitOverflow(this, e);
			}
			if (this.ValueLimitOverflow == null)
			{
				return;
			}
			if (Common != null && Common.GaugeCore.InvokeRequired)
			{
				Delegate[] invocationList = this.ValueLimitOverflow.GetInvocationList();
				for (int i = 0; i < invocationList.Length; i++)
				{
					((ValueChangedEventHandler)invocationList[i]).BeginInvoke(this, e, null, null);
				}
			}
			else
			{
				this.ValueLimitOverflow(this, e);
			}
		}

		internal virtual void CheckLimit()
		{
			try
			{
				if (!double.IsNaN(valueLimit) && outputValue > valueLimit)
				{
					OnValueLimitOverflow(new ValueChangedEventArgs(outputValue, outputDate, Name, IsPlayBackMode));
				}
			}
			catch (ApplicationException)
			{
				throw;
			}
		}

		internal virtual void OnValueChanged(ValueChangedEventArgs e)
		{
			if (Common != null)
			{
				Common.GaugeContainer.OnValueChanged(this, e);
			}
			if (this.ValueChanged == null)
			{
				return;
			}
			if (Common != null && Common.GaugeCore.InvokeRequired)
			{
				Delegate[] invocationList = this.ValueChanged.GetInvocationList();
				for (int i = 0; i < invocationList.Length; i++)
				{
					((ValueChangedEventHandler)invocationList[i]).BeginInvoke(this, e, null, null);
				}
			}
			else
			{
				this.ValueChanged(this, e);
			}
		}

		internal override void OnNameChanged()
		{
			base.OnNameChanged();
			foreach (IValueConsumer consumer in consumers)
			{
				consumer.ProviderNameChanged(this);
			}
		}

		internal override void OnRemove()
		{
			base.OnRemove();
			foreach (IValueConsumer consumer in consumers)
			{
				consumer.ProviderRemoved(this);
			}
		}

		internal override void OnAdded()
		{
			base.OnAdded();
			if (Common != null)
			{
				Common.GaugeCore.ReconnectData(exact: false);
			}
		}

		internal virtual void Recalculate(double value, DateTime timestamp)
		{
			lock (this)
			{
				double num = outputValue;
				_ = outputDate;
				CalculateValue(value, timestamp);
				if (IsEventsEnable)
				{
					CheckLimit();
				}
				AddToHistory();
				if (outputValue != num)
				{
					OnValueChanged(new ValueChangedEventArgs(outputValue, outputDate, Name, IsPlayBackMode));
				}
				ClearUpHistory();
			}
		}

		internal virtual void CalculateValue(double value, DateTime timestamp)
		{
			outputValue = value;
			outputDate = timestamp;
		}

		internal virtual void RefreshConsumers()
		{
			foreach (IValueConsumer consumer in consumers)
			{
				consumer.Refresh();
			}
		}

		internal override void Invalidate()
		{
			foreach (IValueConsumer consumer in consumers)
			{
				if (consumer is ValueBase)
				{
					((ValueBase)consumer).Invalidate();
				}
				else
				{
					consumer.InputValueChanged(this, new ValueChangedEventArgs(GetValue(), Date, Name, IsPlayBackMode));
				}
			}
		}

		protected override void OnDispose()
		{
			History.Clear();
			base.OnDispose();
		}

		void IValueProvider.AttachConsumer(IValueConsumer consumer)
		{
			if (!consumers.Contains(consumer))
			{
				consumers.Add(consumer);
				ValueChanged += consumer.InputValueChanged;
			}
		}

		void IValueProvider.DetachConsumer(IValueConsumer consumer)
		{
			if (consumers.Contains(consumer))
			{
				consumers.Remove(consumer);
				ValueChanged -= consumer.InputValueChanged;
			}
		}

		double IValueProvider.GetValue()
		{
			return outputValue;
		}

		DateTime IValueProvider.GetDate()
		{
			return outputDate;
		}

		string IValueProvider.GetValueProviderName()
		{
			return Name;
		}

		HistoryCollection IValueProvider.GetData(GaugeDuration period, DateTime currentDate)
		{
			if (History.Count > 0)
			{
				queryDept.Extend(period, currentDate, History[0].Timestamp);
			}
			return History;
		}

		bool IValueProvider.GetPlayBackMode()
		{
			return IsPlayBackMode;
		}

		ValueState IValueProvider.GetProvderState()
		{
			return provderState;
		}

		internal override object CloneInternals(object copy)
		{
			ValueBase valueBase = (ValueBase)base.CloneInternals(copy);
			valueBase.historyDept = (GaugeDuration)historyDept.Clone();
			valueBase.queryDept = (GaugeDuration)queryDept.Clone();
			valueBase.history = (HistoryCollection)history.Clone();
			valueBase.ValueChanged = null;
			valueBase.ValueLimitOverflow = null;
			valueBase.consumers = new ArrayList();
			foreach (IValueConsumer consumer in consumers)
			{
				if (!(consumer is ValueBase))
				{
					((IValueProvider)valueBase).AttachConsumer(consumer);
				}
			}
			return valueBase;
		}
	}
}
