namespace Microsoft.ReportingServices.RdlObjectModel
{
	internal class IntProperty : ComparablePropertyDefinition<int>
	{
		public IntProperty(string name, int? defaultValue)
			: base(name, defaultValue)
		{
		}

		public IntProperty(string name, int? defaultValue, int? minimum, int? maximum)
			: base(name, defaultValue, minimum, maximum)
		{
		}
	}
}
