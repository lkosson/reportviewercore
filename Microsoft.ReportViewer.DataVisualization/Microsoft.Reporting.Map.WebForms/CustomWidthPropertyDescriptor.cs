using System;
using System.ComponentModel;

namespace Microsoft.Reporting.Map.WebForms
{
	internal class CustomWidthPropertyDescriptor : PropertyDescriptor
	{
		private Field field;

		public override Type ComponentType => typeof(CustomWidth);

		public override bool IsBrowsable => true;

		public override bool IsReadOnly => false;

		public override Type PropertyType => field.Type;

		public CustomWidthPropertyDescriptor(Field field, string name, Attribute[] attrs)
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
			CustomWidth customWidth = (CustomWidth)component;
			if (Name == "FromValue")
			{
				return field.Parse(customWidth.FromValue);
			}
			return field.Parse(customWidth.ToValue);
		}

		public override void SetValue(object component, object value)
		{
			CustomWidth customWidth = (CustomWidth)component;
			if (Name == "FromValue")
			{
				customWidth.FromValue = Field.ToStringInvariant(value);
			}
			else
			{
				customWidth.ToValue = Field.ToStringInvariant(value);
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
