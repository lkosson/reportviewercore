using System;

namespace Microsoft.Reporting.Gauge.WebForms
{
	internal class StateCollection : NamedCollection
	{
		private State this[int index]
		{
			get
			{
				if (index == 0 && base.List.Count == 0)
				{
					Add(new State());
				}
				return (State)base.List[index];
			}
			set
			{
				base.List.Insert(index, value);
			}
		}

		private State this[string name]
		{
			get
			{
				return (State)GetByNameCheck(name);
			}
			set
			{
				SetByNameCheck(name, value);
			}
		}

		public State this[object obj]
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

		internal StateCollection(NamedElement parent, CommonElements common)
			: base(parent, common)
		{
			elementType = typeof(State);
		}

		public State Add(string name)
		{
			State state = new State();
			state.Name = name;
			Add(state);
			return state;
		}

		public int Add(State value)
		{
			return base.List.Add(value);
		}

		public void Remove(State value)
		{
			base.List.Remove(value);
		}

		public bool Contains(State value)
		{
			return base.List.Contains(value);
		}

		public void Insert(int index, State value)
		{
			base.List.Insert(index, value);
		}

		public int IndexOf(State value)
		{
			return base.List.IndexOf(value);
		}

		internal override string GetDefaultElementName(NamedElement el)
		{
			return "State1";
		}

		internal override string GetElementNameFormat(NamedElement el)
		{
			return "State{0}";
		}
	}
}
