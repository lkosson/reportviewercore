using System;
using System.ComponentModel;

namespace Microsoft.Reporting.Map.WebForms
{
	internal class CustomColorPropertyDescriptor : PropertyDescriptor
	{
		private Field field;

		public override Type ComponentType => typeof(CustomColor);

		public override bool IsBrowsable => true;

		public override bool IsReadOnly => false;

		public override Type PropertyType => field.Type;

		public CustomColorPropertyDescriptor(Field field, string name, Attribute[] attrs)
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
			CustomColor customColor = (CustomColor)component;
			if (Name == "FromValue")
			{
				return field.Parse(customColor.FromValue);
			}
			return field.Parse(customColor.ToValue);
		}

		public override void SetValue(object component, object value)
		{
			CustomColor customColor = (CustomColor)component;
			if (Name == "FromValue")
			{
				customColor.FromValue = Field.ToStringInvariant(value);
			}
			else
			{
				customColor.ToValue = Field.ToStringInvariant(value);
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
