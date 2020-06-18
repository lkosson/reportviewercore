using Microsoft.ReportingServices.RdlObjectModel.Serialization;

namespace Microsoft.ReportingServices.RdlObjectModel
{
	[XmlElementClass("MapColorRangeRule", typeof(MapColorRangeRule))]
	[XmlElementClass("MapColorPaletteRule", typeof(MapColorPaletteRule))]
	[XmlElementClass("MapCustomColorRule", typeof(MapCustomColorRule))]
	internal abstract class MapColorRule : MapAppearanceRule
	{
		internal new class Definition : DefinitionStore<MapColorRule, Definition.Properties>
		{
			internal enum Properties
			{
				DataValue,
				DistributionType,
				BucketCount,
				StartValue,
				EndValue,
				MapBuckets,
				LegendName,
				LegendText,
				DataElementName,
				DataElementOutput,
				ShowInColorScale
			}

			private Definition()
			{
			}
		}

		public ReportExpression<bool> ShowInColorScale
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<bool>>(10);
			}
			set
			{
				base.PropertyStore.SetObject(10, value);
			}
		}

		public MapColorRule()
		{
		}

		internal MapColorRule(IPropertyStore propertyStore)
			: base(propertyStore)
		{
		}

		public override void Initialize()
		{
			base.Initialize();
		}
	}
}
