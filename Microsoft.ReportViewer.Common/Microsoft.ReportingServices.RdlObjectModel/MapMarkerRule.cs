using System.Collections.Generic;
using System.Xml.Serialization;

namespace Microsoft.ReportingServices.RdlObjectModel
{
	internal class MapMarkerRule : MapAppearanceRule
	{
		internal new class Definition : DefinitionStore<MapMarkerRule, Definition.Properties>
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
				MapMarkers,
				PropertyCount
			}

			private Definition()
			{
			}
		}

		[XmlElement(typeof(RdlCollection<MapMarker>))]
		public IList<MapMarker> MapMarkers
		{
			get
			{
				return (IList<MapMarker>)base.PropertyStore.GetObject(10);
			}
			set
			{
				base.PropertyStore.SetObject(10, value);
			}
		}

		public MapMarkerRule()
		{
		}

		internal MapMarkerRule(IPropertyStore propertyStore)
			: base(propertyStore)
		{
		}

		public override void Initialize()
		{
			base.Initialize();
			MapMarkers = new RdlCollection<MapMarker>();
		}
	}
}
