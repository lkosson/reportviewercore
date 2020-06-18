using System;
using System.ComponentModel;

namespace Microsoft.Reporting.Map.WebForms
{
	internal class GroupFieldPropertyDescriptor : PropertyDescriptor
	{
		private Field field;

		public override Type ComponentType => typeof(Group);

		public override string DisplayName => field.Name;

		public override bool IsBrowsable => true;

		public override bool IsReadOnly => field.IsTemporary;

		public override Type PropertyType => field.Type;

		public GroupFieldPropertyDescriptor(Field field, Attribute[] attributes)
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
				return ((Group)component)[field.Name];
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
			((Group)component)[field.Name] = value;
		}
	}
}
