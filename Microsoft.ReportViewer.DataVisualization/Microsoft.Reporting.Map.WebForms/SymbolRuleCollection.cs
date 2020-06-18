using System;

namespace Microsoft.Reporting.Map.WebForms
{
	internal class SymbolRuleCollection : NamedCollection
	{
		private SymbolRule this[int index]
		{
			get
			{
				return (SymbolRule)base.List[index];
			}
			set
			{
				base.List.Insert(index, value);
			}
		}

		private SymbolRule this[string name]
		{
			get
			{
				return (SymbolRule)GetByNameCheck(name);
			}
			set
			{
				SetByNameCheck(name, value);
			}
		}

		public SymbolRule this[object obj]
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

		internal SymbolRuleCollection(NamedElement parent, CommonElements common)
			: base(parent, common)
		{
			elementType = typeof(SymbolRule);
		}

		public SymbolRule Add(string name)
		{
			SymbolRule symbolRule = new SymbolRule();
			symbolRule.Name = name;
			Add(symbolRule);
			return symbolRule;
		}

		public int Add(SymbolRule value)
		{
			return base.List.Add(value);
		}

		public void Remove(SymbolRule value)
		{
			base.List.Remove(value);
		}

		internal override string GetDefaultElementName(NamedElement el)
		{
			return "SymbolRule1";
		}

		internal override string GetElementNameFormat(NamedElement el)
		{
			return "SymbolRule{0}";
		}

		protected override void OnInsertComplete(int index, object value)
		{
			base.OnInsertComplete(index, value);
			_ = (SymbolRule)value;
		}
	}
}
