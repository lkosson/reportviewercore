using System;

namespace Microsoft.Reporting.Gauge.WebForms
{
	internal class CircularScaleCollection : NamedCollection
	{
		private CircularScale this[int index]
		{
			get
			{
				if (index == 0 && base.List.Count == 0)
				{
					Add(new CircularScale());
				}
				return (CircularScale)base.List[index];
			}
			set
			{
				base.List.Insert(index, value);
			}
		}

		private CircularScale this[string name]
		{
			get
			{
				return (CircularScale)GetByNameCheck(name);
			}
			set
			{
				SetByNameCheck(name, value);
			}
		}

		public CircularScale this[object obj]
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

		internal CircularScaleCollection(NamedElement parent, CommonElements common)
			: base(parent, common)
		{
			elementType = typeof(CircularScale);
		}

		public CircularScale Add(string name)
		{
			CircularScale circularScale = new CircularScale();
			circularScale.Name = name;
			Add(circularScale);
			return circularScale;
		}

		public int Add(CircularScale value)
		{
			return base.List.Add(value);
		}

		public void Remove(CircularScale value)
		{
			base.List.Remove(value);
		}

		public bool Contains(CircularScale value)
		{
			return base.List.Contains(value);
		}

		public void Insert(int index, CircularScale value)
		{
			base.List.Insert(index, value);
		}

		public int IndexOf(CircularScale value)
		{
			return base.List.IndexOf(value);
		}

		internal override string GetElementNameFormat(NamedElement el)
		{
			return "Scale{0}";
		}
	}
}
