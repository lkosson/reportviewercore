using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Serialization;

namespace Microsoft.ReportingServices.RdlObjectModel
{
	internal abstract class MapAppearanceRule : ReportObject
	{
		internal class Definition : DefinitionStore<MapAppearanceRule, Definition.Properties>
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
				DataElementOutput
			}

			private Definition()
			{
			}
		}

		[ReportExpressionDefaultValue("")]
		public ReportExpression DataValue
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression>(0);
			}
			set
			{
				base.PropertyStore.SetObject(0, value);
			}
		}

		[ReportExpressionDefaultValue(typeof(MapRuleDistributionTypes), MapRuleDistributionTypes.Optimal)]
		public ReportExpression<MapRuleDistributionTypes> DistributionType
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<MapRuleDistributionTypes>>(1);
			}
			set
			{
				base.PropertyStore.SetObject(1, value);
			}
		}

		[ReportExpressionDefaultValue(typeof(int), "5")]
		public ReportExpression<int> BucketCount
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<int>>(2);
			}
			set
			{
				base.PropertyStore.SetObject(2, value);
			}
		}

		public ReportExpression StartValue
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression>(3);
			}
			set
			{
				base.PropertyStore.SetObject(3, value);
			}
		}

		public ReportExpression EndValue
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression>(4);
			}
			set
			{
				base.PropertyStore.SetObject(4, value);
			}
		}

		[XmlElement(typeof(RdlCollection<MapBucket>))]
		public IList<MapBucket> MapBuckets
		{
			get
			{
				return (IList<MapBucket>)base.PropertyStore.GetObject(5);
			}
			set
			{
				base.PropertyStore.SetObject(5, value);
			}
		}

		public string LegendName
		{
			get
			{
				return base.PropertyStore.GetObject<string>(6);
			}
			set
			{
				base.PropertyStore.SetObject(6, value);
			}
		}

		public ReportExpression LegendText
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression>(7);
			}
			set
			{
				base.PropertyStore.SetObject(7, value);
			}
		}

		[DefaultValue("")]
		public string DataElementName
		{
			get
			{
				return (string)base.PropertyStore.GetObject(8);
			}
			set
			{
				base.PropertyStore.SetObject(8, value);
			}
		}

		[DefaultValue(DataElementOutputTypes.Output)]
		[ValidEnumValues("MapDataElementOutputTypes")]
		public DataElementOutputTypes DataElementOutput
		{
			get
			{
				return (DataElementOutputTypes)base.PropertyStore.GetInteger(9);
			}
			set
			{
				((EnumProperty)DefinitionStore<MapAppearanceRule, Definition.Properties>.GetProperty(9)).Validate(this, (int)value);
				base.PropertyStore.SetInteger(9, (int)value);
			}
		}

		public MapAppearanceRule()
		{
		}

		internal MapAppearanceRule(IPropertyStore propertyStore)
			: base(propertyStore)
		{
		}

		public override void Initialize()
		{
			base.Initialize();
			DistributionType = MapRuleDistributionTypes.Optimal;
			BucketCount = 5;
			MapBuckets = new RdlCollection<MapBucket>();
			DataElementOutput = DataElementOutputTypes.Output;
		}
	}
}
