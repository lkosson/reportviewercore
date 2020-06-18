using System;
using System.Globalization;

namespace Microsoft.Reporting.Gauge.WebForms
{
	internal class CalculatedValueCollection : NamedCollection
	{
		public CalculatedValue this[object obj]
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
				throw new ArgumentException(Utils.SRGetStr("ExceptionInvalidIndexer"));
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
				throw new ArgumentException(Utils.SRGetStr("ExceptionInvalidIndexer"));
			}
		}

		private CalculatedValue this[string name]
		{
			get
			{
				return (CalculatedValue)GetByNameCheck(name);
			}
			set
			{
				SetByNameCheck(name, value);
			}
		}

		private CalculatedValue this[int index]
		{
			get
			{
				return (CalculatedValue)base.List[index];
			}
			set
			{
				base.List[index] = value;
			}
		}

		internal CalculatedValueCollection(NamedElement parent, CommonElements common)
			: base(parent, common)
		{
			elementType = typeof(CalculatedValue);
		}

		public CalculatedValue Add(string name)
		{
			CalculatedValue calculatedValue = new CalculatedValue();
			calculatedValue.Name = name;
			Add(calculatedValue);
			return calculatedValue;
		}

		public int Add(CalculatedValue value)
		{
			return base.List.Add(value);
		}

		public void Insert(int index, CalculatedValue value)
		{
			base.List.Insert(index, value);
		}

		public void Remove(CalculatedValue value)
		{
			base.List.Remove(value);
		}

		public bool Contains(CalculatedValue value)
		{
			return base.List.Contains(value);
		}

		public int IndexOf(CalculatedValue value)
		{
			return base.List.IndexOf(value);
		}

		internal override string GetElementNameFormat(NamedElement el)
		{
			return base.GetElementNameFormat(el).Replace("CalculatedValue", "");
		}

		internal override string GetDefaultElementName(NamedElement el)
		{
			return string.Format(CultureInfo.InvariantCulture, GetElementNameFormat(el), 1);
		}

		internal override bool IsCorrectType(object value)
		{
			return value is CalculatedValue;
		}

		internal override void IsValidNameCheck(string name, NamedElement element)
		{
			base.IsValidNameCheck(name, element);
			if (parent is ValueBase && ((ValueBase)parent).Name.Equals(name, StringComparison.Ordinal))
			{
				throw new NotSupportedException(Utils.SRGetStr("ExceptionListUniqueName", GetType().Name));
			}
		}

		internal override bool IsUniqueName(string name)
		{
			bool flag = true;
			if (parent is ValueBase)
			{
				flag = ((ValueBase)parent).Name.Equals(name, StringComparison.Ordinal);
			}
			if (flag)
			{
				return base.IsUniqueName(name);
			}
			return false;
		}
	}
}
