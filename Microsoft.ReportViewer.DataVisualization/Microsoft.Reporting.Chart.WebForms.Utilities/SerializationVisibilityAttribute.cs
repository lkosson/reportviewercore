using System;

namespace Microsoft.Reporting.Chart.WebForms.Utilities
{
	[AttributeUsage(AttributeTargets.All)]
	internal sealed class SerializationVisibilityAttribute : Attribute
	{
		private SerializationVisibility visibility = SerializationVisibility.Attribute;

		public SerializationVisibility Visibility => visibility;

		public SerializationVisibilityAttribute(SerializationVisibility visibility)
		{
			this.visibility = visibility;
		}
	}
}
