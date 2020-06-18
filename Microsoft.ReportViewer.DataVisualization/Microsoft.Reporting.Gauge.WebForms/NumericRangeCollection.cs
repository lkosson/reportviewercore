using System;

namespace Microsoft.Reporting.Gauge.WebForms
{
	internal class NumericRangeCollection : NamedCollection
	{
		private NumericRange this[int index]
		{
			get
			{
				if (index == 0 && base.List.Count == 0)
				{
					Add(new NumericRange());
				}
				return (NumericRange)base.List[index];
			}
			set
			{
				base.List.Insert(index, value);
			}
		}

		private NumericRange this[string name]
		{
			get
			{
				return (NumericRange)GetByNameCheck(name);
			}
			set
			{
				SetByNameCheck(name, value);
			}
		}

		public NumericRange this[object obj]
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

		internal NumericRangeCollection(NamedElement parent, CommonElements common)
			: base(parent, common)
		{
			elementType = typeof(NumericRange);
		}

		public NumericRange Add(string name)
		{
			NumericRange numericRange = new NumericRange();
			numericRange.Name = name;
			Add(numericRange);
			return numericRange;
		}

		public int Add(NumericRange value)
		{
			return base.List.Add(value);
		}

		public void Remove(NumericRange value)
		{
			base.List.Remove(value);
		}

		public bool Contains(NumericRange value)
		{
			return base.List.Contains(value);
		}

		public void Insert(int index, NumericRange value)
		{
			base.List.Insert(index, value);
		}

		public int IndexOf(NumericRange value)
		{
			return base.List.IndexOf(value);
		}

		internal override string GetElementNameFormat(NamedElement el)
		{
			return "Range{0}";
		}
	}
}
