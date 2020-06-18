using System;
using System.ComponentModel;

namespace Microsoft.Reporting.Map.WebForms
{
	internal class PathFieldPropertyDescriptor : PropertyDescriptor
	{
		private Field field;

		public override Type ComponentType => typeof(Path);

		public override string DisplayName => field.Name;

		public override bool IsBrowsable => true;

		public override bool IsReadOnly => field.IsTemporary;

		public override Type PropertyType => field.Type;

		public PathFieldPropertyDescriptor(Field field, Attribute[] attributes)
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
				return ((Path)component)[field.Name];
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
			((Path)component)[field.Name] = value;
		}
	}
}
