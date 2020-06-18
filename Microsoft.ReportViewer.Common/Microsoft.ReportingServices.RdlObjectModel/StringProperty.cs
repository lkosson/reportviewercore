namespace Microsoft.ReportingServices.RdlObjectModel
{
	internal class StringProperty : PropertyDefinition, IPropertyDefinition
	{
		private string m_default;

		public object Default => m_default;

		object IPropertyDefinition.Minimum => null;

		object IPropertyDefinition.Maximum => null;

		void IPropertyDefinition.Validate(object component, object value)
		{
		}

		public StringProperty(string name, string defaultValue)
			: base(name)
		{
			m_default = defaultValue;
		}
	}
}
