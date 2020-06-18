namespace Microsoft.ReportingServices.RdlObjectModel
{
	internal class ColorProperty : PropertyDefinition<ReportColor>
	{
		public ColorProperty(string name, ReportColor? defaultValue)
			: base(name, defaultValue)
		{
		}
	}
}
