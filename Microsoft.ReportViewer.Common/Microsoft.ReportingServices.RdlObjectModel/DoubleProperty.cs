namespace Microsoft.ReportingServices.RdlObjectModel
{
	internal class DoubleProperty : ComparablePropertyDefinition<double>
	{
		public DoubleProperty(string name, double? defaultValue)
			: base(name, defaultValue)
		{
		}

		public DoubleProperty(string name, double? defaultValue, double? minimum, double? maximum)
			: base(name, defaultValue, minimum, maximum)
		{
		}
	}
}
