namespace Microsoft.Reporting.Map.WebForms
{
	internal class MapPaintEventArgs
	{
		private MapControl control;

		private NamedElement mapElement;

		private MapGraphics graphics;

		internal MapControl MapControl => control;

		public NamedElement MapElement => mapElement;

		public MapGraphics Graphics => graphics;

		internal MapPaintEventArgs(MapControl control, NamedElement mapElement, MapGraphics graphics)
		{
			this.control = control;
			this.mapElement = mapElement;
			this.graphics = graphics;
		}
	}
}
