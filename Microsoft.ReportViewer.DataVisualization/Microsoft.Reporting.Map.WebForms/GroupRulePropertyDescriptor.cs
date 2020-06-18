using System;
using System.ComponentModel;

namespace Microsoft.Reporting.Map.WebForms
{
	internal class GroupRulePropertyDescriptor : PropertyDescriptor
	{
		private Field field;

		public override Type ComponentType => typeof(GroupRule);

		public override bool IsBrowsable => true;

		public override bool IsReadOnly => false;

		public override Type PropertyType => field.Type;

		public GroupRulePropertyDescriptor(Field field, string name, Attribute[] attrs)
			: base(name, attrs)
		{
			this.field = field;
		}

		public override bool CanResetValue(object component)
		{
			return false;
		}

		public override object GetValue(object component)
		{
			GroupRule groupRule = (GroupRule)component;
			if (Name == "FromValue")
			{
				return field.Parse(groupRule.FromValue);
			}
			return field.Parse(groupRule.ToValue);
		}

		public override void SetValue(object component, object value)
		{
			GroupRule groupRule = (GroupRule)component;
			if (Name == "FromValue")
			{
				groupRule.FromValue = Field.ToStringInvariant(value);
			}
			else
			{
				groupRule.ToValue = Field.ToStringInvariant(value);
			}
		}

		public override void ResetValue(object component)
		{
		}

		public override bool ShouldSerializeValue(object component)
		{
			return true;
		}
	}
}
