using System.Globalization;

namespace Microsoft.ReportingServices.RdlObjectModel
{
	internal class MapLineTemplate : MapSpatialElementTemplate
	{
		internal new class Definition : DefinitionStore<MapLineTemplate, Definition.Properties>
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
				Width,
				LineLabelPlacement,
				PropertyCount
			}

			private Definition()
			{
			}
		}

		[ReportExpressionDefaultValue(typeof(ReportSize), "3.75pt")]
		public ReportExpression<ReportSize> Width
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<ReportSize>>(10);
			}
			set
			{
				base.PropertyStore.SetObject(10, value);
			}
		}

		[ReportExpressionDefaultValue(typeof(MapLineLabelPlacements), MapLineLabelPlacements.Above)]
		public ReportExpression<MapLineLabelPlacements> LabelPlacement
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<MapLineLabelPlacements>>(11);
			}
			set
			{
				base.PropertyStore.SetObject(11, value);
			}
		}

		public MapLineTemplate()
		{
		}

		internal MapLineTemplate(IPropertyStore propertyStore)
			: base(propertyStore)
		{
		}

		public override void Initialize()
		{
			base.Initialize();
			Width = new ReportExpression<ReportSize>("3.75pt", CultureInfo.InvariantCulture);
			LabelPlacement = MapLineLabelPlacements.Above;
		}
	}
}
