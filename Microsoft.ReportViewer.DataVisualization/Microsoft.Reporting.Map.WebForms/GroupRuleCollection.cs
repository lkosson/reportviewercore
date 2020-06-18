using System;

namespace Microsoft.Reporting.Map.WebForms
{
	internal class GroupRuleCollection : NamedCollection
	{
		private GroupRule this[int index]
		{
			get
			{
				return (GroupRule)base.List[index];
			}
			set
			{
				base.List.Insert(index, value);
			}
		}

		private GroupRule this[string name]
		{
			get
			{
				return (GroupRule)GetByNameCheck(name);
			}
			set
			{
				SetByNameCheck(name, value);
			}
		}

		public GroupRule this[object obj]
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

		internal GroupRuleCollection(NamedElement parent, CommonElements common)
			: base(parent, common)
		{
			elementType = typeof(GroupRule);
		}

		public GroupRule Add(string name)
		{
			GroupRule groupRule = new GroupRule();
			groupRule.Name = name;
			Add(groupRule);
			return groupRule;
		}

		public int Add(GroupRule value)
		{
			return base.List.Add(value);
		}

		public void Remove(GroupRule value)
		{
			base.List.Remove(value);
		}

		internal override string GetDefaultElementName(NamedElement el)
		{
			return "GroupRule1";
		}

		internal override string GetElementNameFormat(NamedElement el)
		{
			return "GroupRule{0}";
		}

		protected override void OnInsertComplete(int index, object value)
		{
			base.OnInsertComplete(index, value);
			_ = (GroupRule)value;
		}
	}
}
