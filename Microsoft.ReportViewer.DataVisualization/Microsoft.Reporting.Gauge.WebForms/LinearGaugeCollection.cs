using System;

namespace Microsoft.Reporting.Gauge.WebForms
{
	internal class LinearGaugeCollection : NamedCollection
	{
		private LinearGauge this[int index]
		{
			get
			{
				if (index == 0 && base.List.Count == 0)
				{
					Add(new LinearGauge());
				}
				return (LinearGauge)base.List[index];
			}
			set
			{
				base.List.Insert(index, value);
			}
		}

		private LinearGauge this[string name]
		{
			get
			{
				return (LinearGauge)GetByNameCheck(name);
			}
			set
			{
				SetByNameCheck(name, value);
			}
		}

		public LinearGauge this[object obj]
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

		internal LinearGaugeCollection(NamedElement parent, CommonElements common)
			: base(parent, common)
		{
			elementType = typeof(LinearGauge);
		}

		public LinearGauge Add(string name)
		{
			LinearGauge linearGauge = new LinearGauge();
			linearGauge.Name = name;
			linearGauge.Scales.Add(new LinearScale());
			linearGauge.Pointers.Add(new LinearPointer());
			linearGauge.Ranges.Add(new LinearRange());
			Add(linearGauge);
			return linearGauge;
		}

		public int Add(LinearGauge value)
		{
			return base.List.Add(value);
		}

		public void Remove(LinearGauge value)
		{
			base.List.Remove(value);
		}

		public bool Contains(LinearGauge value)
		{
			return base.List.Contains(value);
		}

		public void Insert(int index, LinearGauge value)
		{
			base.List.Insert(index, value);
		}

		public int IndexOf(LinearGauge value)
		{
			return base.List.IndexOf(value);
		}

		internal override string GetElementNameFormat(NamedElement el)
		{
			return "Gauge{0}";
		}
	}
}
