using System;

namespace Microsoft.Reporting.Map.WebForms
{
	internal class PredefinedSymbolCollection : NamedCollection
	{
		private PredefinedSymbol this[int index]
		{
			get
			{
				return (PredefinedSymbol)base.List[index];
			}
			set
			{
				base.List.Insert(index, value);
			}
		}

		private PredefinedSymbol this[string name]
		{
			get
			{
				return (PredefinedSymbol)GetByNameCheck(name);
			}
			set
			{
				SetByNameCheck(name, value);
			}
		}

		public PredefinedSymbol this[object obj]
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
				throw new ArgumentException(SR.ExceptionInvalidIndexerArgument);
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
				throw new ArgumentException(SR.ExceptionInvalidIndexerArgument);
			}
		}

		internal PredefinedSymbolCollection(NamedElement parent, CommonElements common)
			: base(parent, common)
		{
			elementType = typeof(PredefinedSymbol);
		}

		public PredefinedSymbol Add(string name)
		{
			PredefinedSymbol predefinedSymbol = new PredefinedSymbol();
			predefinedSymbol.Name = name;
			Add(predefinedSymbol);
			return predefinedSymbol;
		}

		public int Add(PredefinedSymbol value)
		{
			return base.List.Add(value);
		}

		public void Remove(PredefinedSymbol value)
		{
			base.List.Remove(value);
		}

		internal override string GetDefaultElementName(NamedElement el)
		{
			return "PredefinedSymbol1";
		}

		internal override string GetElementNameFormat(NamedElement el)
		{
			return "PredefinedSymbol{0}";
		}

		protected override void OnInsertComplete(int index, object value)
		{
			base.OnInsertComplete(index, value);
			_ = (PredefinedSymbol)value;
		}
	}
}
