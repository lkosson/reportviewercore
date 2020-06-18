using System.Collections.Generic;
using System.Xml.Serialization;

namespace Microsoft.ReportingServices.RdlObjectModel
{
	internal class MapCustomColorRule : MapColorRule
	{
		internal new class Definition : DefinitionStore<MapCustomColorRule, Definition.Properties>
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
				ShowInColorScale,
				MapCustomColors,
				PropertyCount
			}

			private Definition()
			{
			}
		}

		[XmlElement(typeof(RdlCollection<ReportExpression<ReportColor>>))]
		[XmlArrayItem("MapCustomColor", typeof(ReportExpression<ReportColor>))]
		public IList<ReportExpression<ReportColor>> MapCustomColors
		{
			get
			{
				return (IList<ReportExpression<ReportColor>>)base.PropertyStore.GetObject(11);
			}
			set
			{
				base.PropertyStore.SetObject(11, value);
			}
		}

		public MapCustomColorRule()
		{
		}

		internal MapCustomColorRule(IPropertyStore propertyStore)
			: base(propertyStore)
		{
		}

		public override void Initialize()
		{
			base.Initialize();
			MapCustomColors = new RdlCollection<ReportExpression<ReportColor>>();
		}
	}
}
