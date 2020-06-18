using System;

namespace Microsoft.Reporting.Gauge.WebForms
{
	internal class CircularPointerCollection : NamedCollection
	{
		private CircularPointer this[int index]
		{
			get
			{
				if (index == 0 && base.List.Count == 0)
				{
					Add(new CircularPointer());
				}
				return (CircularPointer)base.List[index];
			}
			set
			{
				base.List.Insert(index, value);
			}
		}

		private CircularPointer this[string name]
		{
			get
			{
				return (CircularPointer)GetByNameCheck(name);
			}
			set
			{
				SetByNameCheck(name, value);
			}
		}

		public CircularPointer this[object obj]
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

		internal CircularPointerCollection(NamedElement parent, CommonElements common)
			: base(parent, common)
		{
			elementType = typeof(CircularPointer);
		}

		public CircularPointer Add(string name)
		{
			CircularPointer circularPointer = new CircularPointer();
			circularPointer.Name = name;
			Add(circularPointer);
			return circularPointer;
		}

		public int Add(CircularPointer value)
		{
			return base.List.Add(value);
		}

		public void Remove(CircularPointer value)
		{
			base.List.Remove(value);
		}

		public bool Contains(CircularPointer value)
		{
			return base.List.Contains(value);
		}

		public void Insert(int index, CircularPointer value)
		{
			base.List.Insert(index, value);
		}

		public int IndexOf(CircularPointer value)
		{
			return base.List.IndexOf(value);
		}

		internal override string GetElementNameFormat(NamedElement el)
		{
			return "Pointer{0}";
		}
	}
}
