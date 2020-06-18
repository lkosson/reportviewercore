namespace Microsoft.Reporting.Map.WebForms
{
	internal class ElementEventArgs
	{
		private MapControl control;

		private NamedElement mapElement;

		internal MapControl MapControl => control;

		public NamedElement MapElement => mapElement;

		internal ElementEventArgs(MapControl control, NamedElement mapElement)
		{
			this.control = control;
			this.mapElement = mapElement;
		}
	}
}
