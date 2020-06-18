using System;

namespace Microsoft.Reporting.Map.WebForms
{
	internal class ShapeRuleCollection : NamedCollection
	{
		private ShapeRule this[int index]
		{
			get
			{
				return (ShapeRule)base.List[index];
			}
			set
			{
				base.List.Insert(index, value);
			}
		}

		private ShapeRule this[string name]
		{
			get
			{
				return (ShapeRule)GetByNameCheck(name);
			}
			set
			{
				SetByNameCheck(name, value);
			}
		}

		public ShapeRule this[object obj]
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

		internal ShapeRuleCollection(NamedElement parent, CommonElements common)
			: base(parent, common)
		{
			elementType = typeof(ShapeRule);
		}

		public ShapeRule Add(string name)
		{
			ShapeRule shapeRule = new ShapeRule();
			shapeRule.Name = name;
			Add(shapeRule);
			return shapeRule;
		}

		public int Add(ShapeRule value)
		{
			return base.List.Add(value);
		}

		public void Remove(ShapeRule value)
		{
			base.List.Remove(value);
		}

		internal override string GetDefaultElementName(NamedElement el)
		{
			return "ShapeRule1";
		}

		internal override string GetElementNameFormat(NamedElement el)
		{
			return "ShapeRule{0}";
		}

		protected override void OnInsertComplete(int index, object value)
		{
			base.OnInsertComplete(index, value);
			_ = (ShapeRule)value;
		}
	}
}
