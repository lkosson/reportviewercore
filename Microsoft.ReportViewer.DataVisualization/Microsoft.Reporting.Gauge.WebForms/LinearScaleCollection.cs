using System;

namespace Microsoft.Reporting.Gauge.WebForms
{
	internal class LinearScaleCollection : NamedCollection
	{
		private LinearScale this[int index]
		{
			get
			{
				if (index == 0 && base.List.Count == 0)
				{
					Add(new LinearScale());
				}
				return (LinearScale)base.List[index];
			}
			set
			{
				base.List.Insert(index, value);
			}
		}

		private LinearScale this[string name]
		{
			get
			{
				return (LinearScale)GetByNameCheck(name);
			}
			set
			{
				SetByNameCheck(name, value);
			}
		}

		public LinearScale this[object obj]
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

		internal LinearScaleCollection(NamedElement parent, CommonElements common)
			: base(parent, common)
		{
			elementType = typeof(LinearScale);
		}

		public LinearScale Add(string name)
		{
			LinearScale linearScale = new LinearScale();
			linearScale.Name = name;
			Add(linearScale);
			return linearScale;
		}

		public int Add(LinearScale value)
		{
			return base.List.Add(value);
		}

		public void Remove(LinearScale value)
		{
			base.List.Remove(value);
		}

		public bool Contains(LinearScale value)
		{
			return base.List.Contains(value);
		}

		public void Insert(int index, LinearScale value)
		{
			base.List.Insert(index, value);
		}

		public int IndexOf(LinearScale value)
		{
			return base.List.IndexOf(value);
		}

		internal override string GetElementNameFormat(NamedElement el)
		{
			return "Scale{0}";
		}
	}
}
