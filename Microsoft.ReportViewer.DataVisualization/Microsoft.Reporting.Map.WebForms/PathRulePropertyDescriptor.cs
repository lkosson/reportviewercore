using System;
using System.ComponentModel;

namespace Microsoft.Reporting.Map.WebForms
{
	internal class PathRulePropertyDescriptor : PropertyDescriptor
	{
		private Field field;

		public override Type ComponentType => typeof(PathRule);

		public override bool IsBrowsable => true;

		public override bool IsReadOnly => false;

		public override Type PropertyType => field.Type;

		public PathRulePropertyDescriptor(Field field, string name, Attribute[] attrs)
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
			PathRule pathRule = (PathRule)component;
			if (Name == "FromValue")
			{
				return field.Parse(pathRule.FromValue);
			}
			return field.Parse(pathRule.ToValue);
		}

		public override void SetValue(object component, object value)
		{
			PathRule pathRule = (PathRule)component;
			if (Name == "FromValue")
			{
				pathRule.FromValue = Field.ToStringInvariant(value);
			}
			else
			{
				pathRule.ToValue = Field.ToStringInvariant(value);
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
