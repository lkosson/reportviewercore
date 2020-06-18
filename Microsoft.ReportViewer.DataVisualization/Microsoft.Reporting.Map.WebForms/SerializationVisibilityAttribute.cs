using System;

namespace Microsoft.Reporting.Map.WebForms
{
	[AttributeUsage(AttributeTargets.All)]
	internal sealed class SerializationVisibilityAttribute : Attribute
	{
		private SerializationVisibility visibility = SerializationVisibility.Attribute;

		public SerializationVisibility Visibility
		{
			get
			{
				return visibility;
			}
			set
			{
				visibility = value;
			}
		}

		public SerializationVisibilityAttribute(SerializationVisibility visibility)
		{
			this.visibility = visibility;
		}
	}
}
