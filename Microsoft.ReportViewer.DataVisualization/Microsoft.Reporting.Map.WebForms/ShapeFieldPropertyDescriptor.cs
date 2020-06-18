using System;
using System.ComponentModel;

namespace Microsoft.Reporting.Map.WebForms
{
	internal class ShapeFieldPropertyDescriptor : PropertyDescriptor
	{
		private Field field;

		public override Type ComponentType => typeof(Shape);

		public override string DisplayName => field.Name;

		public override bool IsBrowsable => true;

		public override bool IsReadOnly => field.IsTemporary;

		public override Type PropertyType => field.Type;

		public ShapeFieldPropertyDescriptor(Field field, Attribute[] attributes)
			: base(field.Name, attributes)
		{
			this.field = field;
		}

		public override bool CanResetValue(object component)
		{
			return false;
		}

		public override object GetValue(object component)
		{
			try
			{
				return ((Shape)component)[field.Name];
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}

		public override void ResetValue(object component)
		{
		}

		public override bool ShouldSerializeValue(object component)
		{
			return true;
		}

		public override void SetValue(object component, object value)
		{
			((Shape)component)[field.Name] = value;
		}
	}
}
