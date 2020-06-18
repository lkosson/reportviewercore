using System;
using System.ComponentModel;

namespace Microsoft.Reporting.Chart.WebForms.Design
{
	internal class DynamicPropertyDescriptor : PropertyDescriptor
	{
		private PropertyDescriptor basePropertyDescriptor;

		private string displayName = string.Empty;

		public override Type ComponentType => basePropertyDescriptor.ComponentType;

		public override string DisplayName
		{
			get
			{
				if (displayName.Length > 0)
				{
					return displayName;
				}
				return basePropertyDescriptor.DisplayName;
			}
		}

		public override bool IsBrowsable => basePropertyDescriptor.IsBrowsable;

		public override bool IsReadOnly => basePropertyDescriptor.IsReadOnly;

		public override Type PropertyType => basePropertyDescriptor.PropertyType;

		public DynamicPropertyDescriptor(PropertyDescriptor basePropertyDescriptor, string displayName)
			: base(basePropertyDescriptor)
		{
			this.displayName = displayName;
			this.basePropertyDescriptor = basePropertyDescriptor;
		}

		public override bool CanResetValue(object component)
		{
			return basePropertyDescriptor.CanResetValue(component);
		}

		public override object GetValue(object component)
		{
			return basePropertyDescriptor.GetValue(component);
		}

		public override void ResetValue(object component)
		{
			basePropertyDescriptor.ResetValue(component);
		}

		public override bool ShouldSerializeValue(object component)
		{
			return basePropertyDescriptor.ShouldSerializeValue(component);
		}

		public override void SetValue(object component, object value)
		{
			basePropertyDescriptor.SetValue(component, value);
		}
	}
}
