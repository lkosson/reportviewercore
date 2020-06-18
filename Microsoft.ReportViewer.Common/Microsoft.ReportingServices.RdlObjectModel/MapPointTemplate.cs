using Microsoft.ReportingServices.RdlObjectModel.Serialization;
using System.Globalization;

namespace Microsoft.ReportingServices.RdlObjectModel
{
	[XmlElementClass("MapMarkerTemplate", typeof(MapMarkerTemplate))]
	internal abstract class MapPointTemplate : MapSpatialElementTemplate
	{
		internal new class Definition : DefinitionStore<MapPointTemplate, Definition.Properties>
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
				LabelPlacement
			}

			private Definition()
			{
			}
		}

		[ReportExpressionDefaultValue(typeof(ReportSize), "5.25pt")]
		public ReportExpression<ReportSize> Size
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

		[ReportExpressionDefaultValue(typeof(MapPointLabelPlacements), MapPointLabelPlacements.Bottom)]
		public ReportExpression<MapPointLabelPlacements> LabelPlacement
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<MapPointLabelPlacements>>(11);
			}
			set
			{
				base.PropertyStore.SetObject(11, value);
			}
		}

		public MapPointTemplate()
		{
		}

		internal MapPointTemplate(IPropertyStore propertyStore)
			: base(propertyStore)
		{
		}

		public override void Initialize()
		{
			base.Initialize();
			Size = new ReportExpression<ReportSize>("5.25pt", CultureInfo.InvariantCulture);
			LabelPlacement = MapPointLabelPlacements.Bottom;
		}
	}
}
