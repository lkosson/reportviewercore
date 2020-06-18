namespace Microsoft.ReportingServices.RdlObjectModel
{
	internal class MapMarkerTemplate : MapPointTemplate
	{
		internal new class Definition : DefinitionStore<MapMarkerTemplate, Definition.Properties>
		{
			internal enum Properties
			{
				Style,
				ActionInfo,
				Hidden,
				OffsetX,
				OffsetY,
				Label,
				ToolTip,
				DataElementName,
				DataElementOutput,
				DataElementLabel,
				Size,
				LabelPlacement,
				MapMarker,
				PropertyCount
			}

			private Definition()
			{
			}
		}

		public MapMarker MapMarker
		{
			get
			{
				return (MapMarker)base.PropertyStore.GetObject(12);
			}
			set
			{
				base.PropertyStore.SetObject(12, value);
			}
		}

		public MapMarkerTemplate()
		{
		}

		internal MapMarkerTemplate(IPropertyStore propertyStore)
			: base(propertyStore)
		{
		}

		public override void Initialize()
		{
			base.Initialize();
		}
	}
}
