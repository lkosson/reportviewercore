namespace Microsoft.Reporting.Chart.WebForms.Borders3D
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
