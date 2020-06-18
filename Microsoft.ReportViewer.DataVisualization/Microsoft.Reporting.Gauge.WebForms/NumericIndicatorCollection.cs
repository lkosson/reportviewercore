using System;

namespace Microsoft.Reporting.Gauge.WebForms
{
	internal class NumericIndicatorCollection : NamedCollection
	{
		private NumericIndicator this[int index]
		{
			get
			{
				if (index == 0 && base.List.Count == 0)
				{
					Add(new NumericIndicator());
				}
				return (NumericIndicator)base.List[index];
			}
			set
			{
				base.List.Insert(index, value);
			}
		}

		private NumericIndicator this[string name]
		{
			get
			{
				return (NumericIndicator)GetByNameCheck(name);
			}
			set
			{
				SetByNameCheck(name, value);
			}
		}

		public NumericIndicator this[object obj]
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

		internal NumericIndicatorCollection(NamedElement parent, CommonElements common)
			: base(parent, common)
		{
			elementType = typeof(NumericIndicator);
		}

		public NumericIndicator Add(string name)
		{
			NumericIndicator numericIndicator = new NumericIndicator();
			numericIndicator.Name = name;
			Add(numericIndicator);
			return numericIndicator;
		}

		public int Add(NumericIndicator value)
		{
			return base.List.Add(value);
		}

		public void Remove(NumericIndicator value)
		{
			base.List.Remove(value);
		}

		public bool Contains(NumericIndicator value)
		{
			return base.List.Contains(value);
		}

		public void Insert(int index, NumericIndicator value)
		{
			base.List.Insert(index, value);
		}

		public int IndexOf(NumericIndicator value)
		{
			return base.List.IndexOf(value);
		}

		internal override string GetDefaultElementName(NamedElement el)
		{
			return "NumericIndicator1";
		}

		internal override string GetElementNameFormat(NamedElement el)
		{
			return "NumericIndicator{0}";
		}

		protected override void OnInsertComplete(int index, object value)
		{
			base.OnInsertComplete(index, value);
			NumericIndicator numericIndicator = (NumericIndicator)value;
			if (numericIndicator.Position.DefaultValues && index != 0)
			{
				NumericIndicator numericIndicator2 = this[index - 1];
				numericIndicator.Location.X = numericIndicator2.Location.X + 3f;
				numericIndicator.Location.Y = numericIndicator2.Location.Y + 3f;
			}
			if (numericIndicator.DefaultParent && numericIndicator.Parent.Length == 0 && base.Common != null)
			{
				if (base.Common.GaugeContainer.CircularGauges.Count > 0)
				{
					numericIndicator.Parent = "CircularGauges." + base.Common.GaugeContainer.CircularGauges[0].Name;
				}
				else if (base.Common.GaugeContainer.LinearGauges.Count > 0)
				{
					numericIndicator.Parent = "LinearGauges." + base.Common.GaugeContainer.LinearGauges[0].Name;
				}
			}
		}
	}
}
