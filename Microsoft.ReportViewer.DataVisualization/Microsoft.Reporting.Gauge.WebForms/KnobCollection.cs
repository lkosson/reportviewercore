using System;

namespace Microsoft.Reporting.Gauge.WebForms
{
	internal class KnobCollection : NamedCollection
	{
		private Knob this[int index]
		{
			get
			{
				if (index == 0 && base.List.Count == 0)
				{
					Add(new Knob());
				}
				return (Knob)base.List[index];
			}
			set
			{
				base.List.Insert(index, value);
			}
		}

		private Knob this[string name]
		{
			get
			{
				return (Knob)GetByNameCheck(name);
			}
			set
			{
				SetByNameCheck(name, value);
			}
		}

		public Knob this[object obj]
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

		internal KnobCollection(NamedElement parent, CommonElements common)
			: base(parent, common)
		{
			elementType = typeof(Knob);
		}

		public Knob Add(string name)
		{
			Knob knob = new Knob();
			knob.Name = name;
			Add(knob);
			return knob;
		}

		public int Add(Knob value)
		{
			return base.List.Add(value);
		}

		public void Remove(Knob value)
		{
			base.List.Remove(value);
		}

		public bool Contains(Knob value)
		{
			return base.List.Contains(value);
		}

		public void Insert(int index, Knob value)
		{
			base.List.Insert(index, value);
		}

		public int IndexOf(Knob value)
		{
			return base.List.IndexOf(value);
		}

		internal override string GetElementNameFormat(NamedElement el)
		{
			return "Knob{0}";
		}
	}
}
