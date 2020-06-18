using System;

namespace Microsoft.Reporting.Map.WebForms
{
	internal class PathRuleCollection : NamedCollection
	{
		private PathRuleBase this[int index]
		{
			get
			{
				return (PathRuleBase)base.List[index];
			}
			set
			{
				base.List.Insert(index, value);
			}
		}

		private PathRuleBase this[string name]
		{
			get
			{
				return (PathRuleBase)GetByNameCheck(name);
			}
			set
			{
				SetByNameCheck(name, value);
			}
		}

		public PathRuleBase this[object obj]
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

		internal PathRuleCollection(NamedElement parent, CommonElements common)
			: base(parent, common)
		{
			elementType = typeof(PathRuleBase);
		}

		public PathRule Add(string name)
		{
			PathRule pathRule = new PathRule();
			pathRule.Name = name;
			Add(pathRule);
			return pathRule;
		}

		public int Add(PathRuleBase value)
		{
			return base.List.Add(value);
		}

		public void Remove(PathRuleBase value)
		{
			base.List.Remove(value);
		}

		internal override string GetDefaultElementName(NamedElement el)
		{
			if (el is PathWidthRule)
			{
				return "PathWidthRule1";
			}
			return "PathRule1";
		}

		internal override string GetElementNameFormat(NamedElement el)
		{
			if (el is PathWidthRule)
			{
				return "PathWidthRule{0}";
			}
			return "PathRule{0}";
		}
	}
}
