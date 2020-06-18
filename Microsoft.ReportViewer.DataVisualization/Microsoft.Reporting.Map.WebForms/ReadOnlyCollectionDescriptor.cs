using System;
using System.ComponentModel;

namespace Microsoft.Reporting.Map.WebForms
{
	internal class ReadOnlyCollectionDescriptor : PropertyDescriptor
	{
		public override Type ComponentType => typeof(RuleBase);

		public override bool IsBrowsable => true;

		public override bool IsReadOnly => true;

		public override Type PropertyType => typeof(string);

		public ReadOnlyCollectionDescriptor(string name, Attribute[] attrs)
			: base(name, attrs)
		{
		}

		public override bool CanResetValue(object component)
		{
			return false;
		}

		public override object GetValue(object component)
		{
			return "(Collection)";
		}

		public override void SetValue(object component, object value)
		{
		}

		public override void ResetValue(object component)
		{
		}

		public override bool ShouldSerializeValue(object component)
		{
			return false;
		}
	}
}
