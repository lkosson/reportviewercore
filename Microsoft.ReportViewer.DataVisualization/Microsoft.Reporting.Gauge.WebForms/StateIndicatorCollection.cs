using System;

namespace Microsoft.Reporting.Gauge.WebForms
{
	internal class StateIndicatorCollection : NamedCollection
	{
		private StateIndicator this[int index]
		{
			get
			{
				if (index == 0 && base.List.Count == 0)
				{
					Add(new StateIndicator());
				}
				return (StateIndicator)base.List[index];
			}
			set
			{
				base.List.Insert(index, value);
			}
		}

		private StateIndicator this[string name]
		{
			get
			{
				return (StateIndicator)GetByNameCheck(name);
			}
			set
			{
				SetByNameCheck(name, value);
			}
		}

		public StateIndicator this[object obj]
		{
			get
			{
				if (obj is string)
				{
					return this[(string)obj];
				}
				if (obj is int)
				{
					return this[(int)obj];
				}
				throw new ArgumentException(Utils.SRGetStr("ExceptionInvalidIndexer_error"));
			}
			set
			{
				if (obj is string)
				{
					this[(string)obj] = value;
					return;
				}
				if (obj is int)
				{
					this[(int)obj] = value;
					return;
				}
				throw new ArgumentException(Utils.SRGetStr("ExceptionInvalidIndexer_error"));
			}
		}

		internal StateIndicatorCollection(NamedElement parent, CommonElements common)
			: base(parent, common)
		{
			elementType = typeof(StateIndicator);
		}

		public StateIndicator Add(string name)
		{
			StateIndicator stateIndicator = new StateIndicator();
			stateIndicator.Name = name;
			Add(stateIndicator);
			return stateIndicator;
		}

		public int Add(StateIndicator value)
		{
			return base.List.Add(value);
		}

		public void Remove(StateIndicator value)
		{
			base.List.Remove(value);
		}

		public bool Contains(StateIndicator value)
		{
			return base.List.Contains(value);
		}

		public void Insert(int index, StateIndicator value)
		{
			base.List.Insert(index, value);
		}

		public int IndexOf(StateIndicator value)
		{
			return base.List.IndexOf(value);
		}

		internal override string GetDefaultElementName(NamedElement el)
		{
			return "StateIndicator1";
		}

		internal override string GetElementNameFormat(NamedElement el)
		{
			return "StateIndicator{0}";
		}

		protected override void OnInsertComplete(int index, object value)
		{
			base.OnInsertComplete(index, value);
			StateIndicator stateIndicator = (StateIndicator)value;
			if (stateIndicator.Position.DefaultValues && index != 0)
			{
				StateIndicator stateIndicator2 = this[index - 1];
				stateIndicator.Location.X = stateIndicator2.Location.X + 3f;
				stateIndicator.Location.Y = stateIndicator2.Location.Y + 3f;
			}
			if (stateIndicator.DefaultParent && stateIndicator.Parent.Length == 0 && base.Common != null)
			{
				if (base.Common.GaugeContainer.CircularGauges.Count > 0)
				{
					stateIndicator.Parent = "CircularGauges." + base.Common.GaugeContainer.CircularGauges[0].Name;
				}
				else if (base.Common.GaugeContainer.LinearGauges.Count > 0)
				{
					stateIndicator.Parent = "LinearGauges." + base.Common.GaugeContainer.LinearGauges[0].Name;
				}
			}
		}
	}
}
