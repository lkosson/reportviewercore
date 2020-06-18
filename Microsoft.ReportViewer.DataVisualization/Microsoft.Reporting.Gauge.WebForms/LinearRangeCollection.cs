using System;

namespace Microsoft.Reporting.Gauge.WebForms
{
	internal class LinearRangeCollection : NamedCollection
	{
		private LinearRange this[int index]
		{
			get
			{
				if (index == 0 && base.List.Count == 0)
				{
					Add(new LinearRange());
				}
				return (LinearRange)base.List[index];
			}
			set
			{
				base.List.Insert(index, value);
			}
		}

		private LinearRange this[string name]
		{
			get
			{
				return (LinearRange)GetByNameCheck(name);
			}
			set
			{
				SetByNameCheck(name, value);
			}
		}

		public LinearRange this[object obj]
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

		internal LinearRangeCollection(NamedElement parent, CommonElements common)
			: base(parent, common)
		{
			elementType = typeof(LinearRange);
		}

		public LinearRange Add(string name)
		{
			LinearRange linearRange = new LinearRange();
			linearRange.Name = name;
			Add(linearRange);
			return linearRange;
		}

		public int Add(LinearRange value)
		{
			return base.List.Add(value);
		}

		public void Remove(LinearRange value)
		{
			base.List.Remove(value);
		}

		public bool Contains(LinearRange value)
		{
			return base.List.Contains(value);
		}

		public void Insert(int index, LinearRange value)
		{
			base.List.Insert(index, value);
		}

		public int IndexOf(LinearRange value)
		{
			return base.List.IndexOf(value);
		}

		internal override string GetElementNameFormat(NamedElement el)
		{
			return "Range{0}";
		}
	}
}
