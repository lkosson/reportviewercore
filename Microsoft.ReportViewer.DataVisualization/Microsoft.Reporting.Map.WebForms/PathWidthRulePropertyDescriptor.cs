using System;
using System.ComponentModel;

namespace Microsoft.Reporting.Map.WebForms
{
	internal class PathWidthRulePropertyDescriptor : PropertyDescriptor
	{
		private Field field;

		public override Type ComponentType => typeof(PathWidthRule);

		public override bool IsBrowsable => true;

		public override bool IsReadOnly => false;

		public override Type PropertyType => field.Type;

		public PathWidthRulePropertyDescriptor(Field field, string name, Attribute[] attrs)
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
			PathWidthRule pathWidthRule = (PathWidthRule)component;
			if (Name == "FromValue")
			{
				return field.Parse(pathWidthRule.FromValue);
			}
			return field.Parse(pathWidthRule.ToValue);
		}

		public override void SetValue(object component, object value)
		{
			PathWidthRule pathWidthRule = (PathWidthRule)component;
			if (Name == "FromValue")
			{
				pathWidthRule.FromValue = Field.ToStringInvariant(value);
			}
			else
			{
				pathWidthRule.ToValue = Field.ToStringInvariant(value);
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
