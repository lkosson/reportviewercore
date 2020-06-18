namespace Microsoft.Reporting.Map.WebForms
{
	internal class RaisedBorder : SunkenBorder
	{
		public override string Name => "Raised";

		public RaisedBorder()
		{
			sunken = false;
		}
	}
}
