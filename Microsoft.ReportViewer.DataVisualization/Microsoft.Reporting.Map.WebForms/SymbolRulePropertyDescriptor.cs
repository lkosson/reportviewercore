using System;
using System.ComponentModel;

namespace Microsoft.Reporting.Map.WebForms
{
	internal class SymbolRulePropertyDescriptor : PropertyDescriptor
	{
		private Field field;

		public override Type ComponentType => typeof(SymbolRule);

		public override bool IsBrowsable => true;

		public override bool IsReadOnly => false;

		public override Type PropertyType => field.Type;

		public SymbolRulePropertyDescriptor(Field field, string name, Attribute[] attrs)
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
			SymbolRule symbolRule = (SymbolRule)component;
			if (Name == "FromValue")
			{
				return field.Parse(symbolRule.FromValue);
			}
			return field.Parse(symbolRule.ToValue);
		}

		public override void SetValue(object component, object value)
		{
			SymbolRule symbolRule = (SymbolRule)component;
			if (Name == "FromValue")
			{
				symbolRule.FromValue = Field.ToStringInvariant(value);
			}
			else
			{
				symbolRule.ToValue = Field.ToStringInvariant(value);
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
