using System;
using System.ComponentModel;

namespace Microsoft.Reporting.Map.WebForms
{
	internal class PredefinedSymbolPropertyDescriptor : PropertyDescriptor
	{
		private Field field;

		public override Type ComponentType => typeof(PredefinedSymbol);

		public override bool IsBrowsable => true;

		public override bool IsReadOnly => false;

		public override Type PropertyType => field.Type;

		public PredefinedSymbolPropertyDescriptor(Field field, string name, Attribute[] attrs)
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
			PredefinedSymbol predefinedSymbol = (PredefinedSymbol)component;
			if (Name == "FromValue")
			{
				return field.Parse(predefinedSymbol.FromValue);
			}
			return field.Parse(predefinedSymbol.ToValue);
		}

		public override void SetValue(object component, object value)
		{
			PredefinedSymbol predefinedSymbol = (PredefinedSymbol)component;
			if (Name == "FromValue")
			{
				predefinedSymbol.FromValue = Field.ToStringInvariant(value);
			}
			else
			{
				predefinedSymbol.ToValue = Field.ToStringInvariant(value);
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
