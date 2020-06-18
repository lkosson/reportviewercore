using System;
using System.ComponentModel;

namespace Microsoft.Reporting.Map.WebForms
{
	internal class ShapeRulePropertyDescriptor : PropertyDescriptor
	{
		private Field field;

		public override Type ComponentType => typeof(ShapeRule);

		public override bool IsBrowsable => true;

		public override bool IsReadOnly => false;

		public override Type PropertyType => field.Type;

		public ShapeRulePropertyDescriptor(Field field, string name, Attribute[] attrs)
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
			ShapeRule shapeRule = (ShapeRule)component;
			if (Name == "FromValue")
			{
				return field.Parse(shapeRule.FromValue);
			}
			return field.Parse(shapeRule.ToValue);
		}

		public override void SetValue(object component, object value)
		{
			ShapeRule shapeRule = (ShapeRule)component;
			if (Name == "FromValue")
			{
				shapeRule.FromValue = Field.ToStringInvariant(value);
			}
			else
			{
				shapeRule.ToValue = Field.ToStringInvariant(value);
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
