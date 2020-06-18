using System;

namespace Microsoft.Reporting.Gauge.WebForms
{
	internal class CustomLabelCollection : NamedCollection
	{
		private CustomLabel this[int index]
		{
			get
			{
				return (CustomLabel)base.List[index];
			}
			set
			{
				base.List[index] = value;
			}
		}

		private CustomLabel this[string name]
		{
			get
			{
				return (CustomLabel)GetByNameCheck(name);
			}
			set
			{
				SetByNameCheck(name, value);
			}
		}

		public CustomLabel this[object obj]
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

		internal CustomLabelCollection(NamedElement parent, CommonElements common)
			: base(parent, common)
		{
			elementType = typeof(CustomLabel);
		}

		public CustomLabel Add(string name)
		{
			CustomLabel customLabel = new CustomLabel();
			customLabel.Name = name;
			Add(customLabel);
			return customLabel;
		}

		public int Add(CustomLabel value)
		{
			return base.List.Add(value);
		}

		public void Remove(CustomLabel value)
		{
			base.List.Remove(value);
		}

		public bool Contains(CustomLabel value)
		{
			return base.List.Contains(value);
		}

		public void Insert(int index, CustomLabel value)
		{
			base.List.Insert(index, value);
		}

		public int IndexOf(CustomLabel value)
		{
			return base.List.IndexOf(value);
		}

		internal override string GetDefaultElementName(NamedElement el)
		{
			return "CustomLabel1";
		}
	}
}
