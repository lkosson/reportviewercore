using System;
using System.ComponentModel;

namespace Microsoft.Reporting.Gauge.WebForms
{
	[TypeConverter(typeof(CalculatedValueConverter))]
	internal class CalculatedValue : ValueBase, IValueConsumer
	{
		private IValueProvider provider;

		private TimerData timerData = new TimerData();

		internal bool timerRefreshCall;

		internal bool noMoreData;

		internal GaugePeriod refreshRate = new GaugePeriod(double.NaN, Microsoft.Reporting.Gauge.WebForms.PeriodType.Seconds);

		internal GaugeDuration aggregateDuration = new GaugeDuration(0.0, DurationType.Infinite);

		private string baseValueName = string.Empty;

		[SRCategory("CategoryBehavior")]
		[Bindable(true)]
		[DefaultValue(double.NaN)]
		[SRDescription("DescriptionAttributeRefreshRate3")]
		[TypeConverter(typeof(DoubleNanValueConverter))]
		public virtual double RefreshRate
		{
			get
			{
				return refreshRate.Duration;
			}
			set
			{
				refreshRate.Duration = value;
				InitTimerData();
			}
		}

		[SRCategory("CategoryBehavior")]
		[Bindable(true)]
		[DefaultValue(Microsoft.Reporting.Gauge.WebForms.PeriodType.Seconds)]
		[SRDescription("DescriptionAttributeRefreshRateType")]
		public virtual PeriodType RefreshRateType
		{
			get
			{
				return refreshRate.PeriodType;
			}
			set
			{
				refreshRate.PeriodType = value;
				InitTimerData();
			}
		}

		[SRCategory("CategoryBehavior")]
		[Bindable(true)]
		[DefaultValue(0L)]
		[SRDescription("DescriptionAttributePeriod")]
		public virtual long Period
		{
			get
			{
				return (long)aggregateDuration.Count;
			}
			set
			{
				aggregateDuration.Count = value;
			}
		}

		[SRCategory("CategoryBehavior")]
		[Bindable(true)]
		[DefaultValue(DurationType.Infinite)]
		[SRDescription("DescriptionAttributePeriodType")]
		public virtual DurationType PeriodType
		{
			get
			{
				return aggregateDuration.DurationType;
			}
			set
			{
				aggregateDuration.DurationType = value;
			}
		}

		[SRCategory("CategoryBehavior")]
		[Browsable(true)]
		[ParenthesizePropertyName(true)]
		[SRDescription("DescriptionAttributeTypeName")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[SerializationVisibility(SerializationVisibility.Hidden)]
		public string TypeName => GetType().Name;

		[SRCategory("CategoryData")]
		[SRDescription("DescriptionAttributeBaseValueName")]
		[RefreshProperties(RefreshProperties.Repaint)]
		[TypeConverter(typeof(CalculatedValueNameConverter))]
		public string BaseValueName
		{
			get
			{
				return baseValueName;
			}
			set
			{
				if (baseValueName != value || provider == null)
				{
					if (value != string.Empty && value == Name)
					{
						throw new ArgumentException(Utils.SRGetStr("ExceptionCircularReference"));
					}
					string text = baseValueName;
					try
					{
						baseValueName = value;
						AttachToProvider();
					}
					catch
					{
						baseValueName = text;
						throw;
					}
				}
			}
		}

		[SRCategory("CategoryData")]
		[Bindable(false)]
		[Browsable(true)]
		[DefaultValue(double.NaN)]
		[NotifyParentProperty(true)]
		[TypeConverter(typeof(DoubleNanValueConverter))]
		[SRDescription("DescriptionAttributeValue8")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[SerializationVisibility(SerializationVisibility.Hidden)]
		public double Value => GetValue();

		internal override CommonElements Common
		{
			get
			{
				return base.Common;
			}
			set
			{
				base.Common = value;
				if (Common != null && !Common.GaugeCore.isInitializing)
				{
					AttachToProvider();
				}
			}
		}

		internal InputValue InputValueObj => (InputValue)Collection.parent;

		public CalculatedValue()
		{
			InitTimer();
		}

		protected override void OnDispose()
		{
			base.OnDispose();
		}

		private void InitTimer()
		{
		}

		internal void InitTimerData()
		{
			if (Common == null)
			{
				return;
			}
			TimeSpan timeSpan = refreshRate.ToTimeSpan();
			_ = timeSpan.TotalMilliseconds / (double)InputValueObj.SpeedMultiplier;
			if (timerData.ticks != timeSpan)
			{
				lock (this)
				{
					timerData.ticks = timeSpan;
				}
			}
		}

		internal void StartTimer()
		{
		}

		protected void StopTimer()
		{
		}

		private void AttachToProvider()
		{
			if (provider != null)
			{
				if (provider.GetValueProviderName() == baseValueName)
				{
					return;
				}
				provider.DetachConsumer(this);
				provider = null;
			}
			if (!(baseValueName == string.Empty) && IsConnectedInCollecton())
			{
				provider = LocateProviderByName(baseValueName);
				if (provider == null)
				{
					throw new ArgumentException(Utils.SRGetStr("ExceptioncalCulatedValueProvider", baseValueName));
				}
				provider.AttachConsumer(this);
				SetValueInternal(provider.GetValue(), provider.GetDate());
				((IValueConsumer)this).Refresh();
			}
		}

		private IValueProvider LocateProviderByName(string name)
		{
			object obj = null;
			if (Collection != null)
			{
				obj = Collection.GetByName(name);
				if (obj == null && InputValueObj.Name == name)
				{
					obj = Collection.parent;
				}
			}
			if (obj is IValueProvider)
			{
				return (IValueProvider)obj;
			}
			return null;
		}

		private bool IsConnectedInCollecton()
		{
			if (Collection != null)
			{
				return Collection.parent != null;
			}
			return false;
		}

		internal override void OnAdded()
		{
			base.OnAdded();
			if (baseValueName == string.Empty && IsConnectedInCollecton())
			{
				BaseValueName = InputValueObj.Name;
			}
			if (Common != null && !Common.GaugeCore.isInitializing)
			{
				AttachToProvider();
			}
		}

		internal override void EndInit()
		{
			base.EndInit();
			InitTimerData();
			AttachToProvider();
		}

		internal override object CloneInternals(object copy)
		{
			copy = base.CloneInternals(copy);
			((CalculatedValue)copy).provider = null;
			((CalculatedValue)copy).refreshRate = refreshRate.Clone();
			((CalculatedValue)copy).timerData = (TimerData)timerData.Clone();
			((CalculatedValue)copy).InitTimer();
			((CalculatedValue)copy).aggregateDuration = (GaugeDuration)aggregateDuration.Clone();
			return copy;
		}

		internal override void Recalculate(double value, DateTime timestamp)
		{
			noMoreData = false;
			base.Recalculate(value, timestamp);
		}

		internal override void CalculateValue(double value, DateTime timestamp)
		{
			base.CalculateValue(value, timestamp);
			if (IsConnectedInCollecton() && !noMoreData && !double.IsNaN(outputValue))
			{
				StopTimer();
				StartTimer();
			}
			else
			{
				StopTimer();
			}
		}

		void IValueConsumer.ProviderRemoved(IValueProvider provider)
		{
			if (this.provider == provider)
			{
				this.provider = null;
				baseValueName = string.Empty;
			}
		}

		void IValueConsumer.ProviderNameChanged(IValueProvider provider)
		{
			baseValueName = provider.GetValueProviderName();
			OnNameChanged();
		}

		void IValueConsumer.InputValueChanged(object sender, ValueChangedEventArgs e)
		{
			if (sender == provider)
			{
				SetValueInternal(e.Value, e.Date);
			}
		}

		IValueProvider IValueConsumer.GetProvider()
		{
			return provider;
		}

		void IValueConsumer.Reset()
		{
			Reset();
		}

		void IValueConsumer.Refresh()
		{
			RefreshConsumers();
		}
	}
}
