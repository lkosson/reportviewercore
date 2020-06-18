using System;
using System.Collections;
using System.Globalization;

namespace Microsoft.Reporting.Gauge.WebForms
{
	internal class DataAttributes : GaugeObject, IValueConsumer
	{
		private double dValue = double.NaN;

		private bool isPercentBased;

		private double minimum;

		private double maximum = 100.0;

		private string valueSource = "";

		private double oldValue = double.NaN;

		private DateTime dateValueStamp;

		private IValueProvider provider;

		internal double Value
		{
			get
			{
				return dValue;
			}
			set
			{
				if (dValue != value)
				{
					SetValue(value, initialize: false);
				}
			}
		}

		public bool IsPercentBased
		{
			get
			{
				return isPercentBased;
			}
			set
			{
				isPercentBased = value;
				Invalidate();
			}
		}

		public double Minimum
		{
			get
			{
				return minimum;
			}
			set
			{
				if (Common != null)
				{
					if (value > Maximum || (value == Maximum && value != 0.0))
					{
						throw new ArgumentException(Utils.SRGetStr("ExceptionMinMax"));
					}
					if (double.IsNaN(value) || double.IsInfinity(value))
					{
						throw new ArgumentException(Utils.SRGetStr("ExceptionInvalidValue"));
					}
				}
				minimum = value;
				Invalidate();
			}
		}

		public double Maximum
		{
			get
			{
				return maximum;
			}
			set
			{
				if (Common != null)
				{
					if (value < Minimum || (value == Minimum && value != 0.0))
					{
						throw new ArgumentException(Utils.SRGetStr("ExceptionMaxMin"));
					}
					if (double.IsNaN(value) || double.IsInfinity(value))
					{
						throw new ArgumentException(Utils.SRGetStr("ExceptionInvalidValue"));
					}
				}
				maximum = value;
				Invalidate();
			}
		}

		internal string ValueSource
		{
			get
			{
				return valueSource;
			}
			set
			{
				if (value == "(none)")
				{
					value = string.Empty;
				}
				if (valueSource != value)
				{
					valueSource = value;
					AttachToProvider(exact: false);
					Invalidate();
				}
			}
		}

		internal double OldValue
		{
			get
			{
				return oldValue;
			}
			set
			{
				oldValue = value;
			}
		}

		internal DateTime DateValueStamp => dateValueStamp;

		public DataAttributes()
			: this(null)
		{
		}

		internal DataAttributes(object parent)
			: base(parent)
		{
		}

		internal double GetValueInPercents()
		{
			double result = double.NaN;
			if (IsPercentBased)
			{
				if (Maximum - Minimum != 0.0)
				{
					result = (Value - Minimum) / (Maximum - Minimum) * 100.0;
				}
				else if (Minimum == Maximum && Value == Minimum)
				{
					result = 100.0;
				}
			}
			return result;
		}

		internal void SetValue(double value, bool initialize)
		{
			OldValue = dValue;
			dValue = value;
			if (Parent != null)
			{
				if (initialize)
				{
					StopDampening();
				}
				((IPointerProvider)Parent).DataValueChanged(initialize);
			}
		}

		private Hashtable CollectValues()
		{
			Hashtable hashtable = new Hashtable();
			if (Common != null)
			{
				foreach (InputValue value in Common.GaugeCore.Values)
				{
					hashtable.Add(value.Name, value);
					foreach (CalculatedValue calculatedValue in value.CalculatedValues)
					{
						if (!hashtable.ContainsKey(calculatedValue.Name))
						{
							hashtable.Add(calculatedValue.Name, calculatedValue);
						}
						else if (!(hashtable[calculatedValue.Name] is InputValue))
						{
							hashtable.Remove(calculatedValue.Name);
						}
						hashtable.Add(string.Format(CultureInfo.InvariantCulture, "{0}.{1}", calculatedValue.InputValueObj.Name, calculatedValue.Name), calculatedValue);
					}
				}
				return hashtable;
			}
			return hashtable;
		}

		private void AttachToProvider(bool exact)
		{
			Hashtable hashtable = CollectValues();
			if (provider != null)
			{
				if (hashtable.ContainsKey(valueSource) && hashtable[valueSource] == provider)
				{
					return;
				}
				provider.DetachConsumer(this);
				provider = null;
			}
			if (Common == null || Common.GaugeCore.isInitializing)
			{
				return;
			}
			if (valueSource == string.Empty)
			{
				SetValue(Value, initialize: true);
				return;
			}
			if (hashtable.ContainsKey(ValueSource))
			{
				provider = (IValueProvider)hashtable[ValueSource];
				provider.AttachConsumer(this);
			}
			else if (exact)
			{
				throw new ArgumentException(Utils.SRGetStr("ExceptionLocateCProviderFailed", valueSource));
			}
			if (provider != null)
			{
				SetValue(provider.GetValue(), initialize: true);
			}
		}

		internal override void EndInit()
		{
			base.EndInit();
			AttachToProvider(exact: true);
		}

		internal override void ReconnectData(bool exact)
		{
			AttachToProvider(exact);
		}

		internal override void Notify(MessageType msg, NamedElement element, object param)
		{
			base.Notify(msg, element, param);
			if (msg == MessageType.DataInvalidated)
			{
				((IValueConsumer)this).Refresh();
			}
		}

		internal bool StartDampening(double targetValue, double minimum, double maximum, double dampeningSweepTime, double refreshRate)
		{
			if (Common == null)
			{
				return false;
			}
			if (dampeningSweepTime <= 0.0)
			{
				return false;
			}
			IPointerProvider pointerProvider = (IPointerProvider)Parent;
			double num = (maximum - minimum) / (dampeningSweepTime * refreshRate);
			if (num > 0.0 && Math.Abs(pointerProvider.Position - targetValue) > num)
			{
				return true;
			}
			return false;
		}

		internal void StopDampening()
		{
		}

		void IValueConsumer.ProviderRemoved(IValueProvider provider)
		{
			if (this.provider == provider)
			{
				valueSource = string.Empty;
				this.provider = null;
			}
		}

		void IValueConsumer.ProviderNameChanged(IValueProvider provider)
		{
			if (provider is CalculatedValue)
			{
				valueSource = string.Format(CultureInfo.InvariantCulture, "{0}.{1}", ((CalculatedValue)provider).InputValueObj.Name, provider.GetValueProviderName());
			}
			else
			{
				valueSource = provider.GetValueProviderName();
			}
		}

		void IValueConsumer.InputValueChanged(object sender, ValueChangedEventArgs e)
		{
			IValueProvider valueProvider = (IValueProvider)sender;
			if (valueProvider.GetProvderState() == ValueState.Interactive || valueProvider.GetProvderState() == ValueState.Playback)
			{
				dateValueStamp = e.Date;
				Value = e.Value;
			}
		}

		IValueProvider IValueConsumer.GetProvider()
		{
			return provider;
		}

		void IValueConsumer.Reset()
		{
			SetValue(double.NaN, initialize: true);
		}

		void IValueConsumer.Refresh()
		{
			if (provider != null)
			{
				dateValueStamp = provider.GetDate();
				SetValue(provider.GetValue(), initialize: true);
			}
			else
			{
				SetValue(Value, initialize: true);
			}
		}

		protected override void OnDispose()
		{
			base.OnDispose();
		}
	}
}
