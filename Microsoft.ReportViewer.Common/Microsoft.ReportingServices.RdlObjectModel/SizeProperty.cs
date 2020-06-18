namespace Microsoft.ReportingServices.RdlObjectModel
{
	internal class SizeProperty : ComparablePropertyDefinition<ReportSize>
	{
		public SizeProperty(string name, ReportSize? defaultValue)
			: base(name, defaultValue)
		{
		}

		public SizeProperty(string name, ReportSize? defaultValue, ReportSize? minimum, ReportSize? maximum)
			: base(name, defaultValue, minimum, maximum)
		{
		}
	}
}
