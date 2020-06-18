using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Serialization;

namespace Microsoft.ReportingServices.RdlObjectModel
{
	internal class ChartSeries : ReportObject, INamedObject
	{
		internal class Definition : DefinitionStore<ChartSeries, Definition.Properties>
		{
			internal enum Properties
			{
				Name,
				Hidden,
				ChartDataPoints,
				Type,
				Subtype,
				Style,
				ChartMarker,
				ChartDataLabel,
				ChartEmptyPoints,
				CustomProperties,
				LegendName,
				ChartItemInLegend,
				ChartAreaName,
				ValueAxisName,
				CategoryAxisName,
				PropertyCount,
				ChartSmartLabel
			}

			private Definition()
			{
			}
		}

		[XmlAttribute(typeof(string))]
		public string Name
		{
			get
			{
				return (string)base.PropertyStore.GetObject(0);
			}
			set
			{
				base.PropertyStore.SetObject(0, value);
			}
		}

		[ReportExpressionDefaultValue(typeof(bool), false)]
		public ReportExpression<bool> Hidden
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<bool>>(1);
			}
			set
			{
				base.PropertyStore.SetObject(1, value);
			}
		}

		[XmlElement(typeof(RdlCollection<ChartDataPoint>))]
		public IList<ChartDataPoint> ChartDataPoints
		{
			get
			{
				return (IList<ChartDataPoint>)base.PropertyStore.GetObject(2);
			}
			set
			{
				base.PropertyStore.SetObject(2, value);
			}
		}

		[ReportExpressionDefaultValue(typeof(ChartTypes), ChartTypes.Column)]
		public ReportExpression<ChartTypes> Type
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<ChartTypes>>(3);
			}
			set
			{
				base.PropertyStore.SetObject(3, value);
			}
		}

		[ReportExpressionDefaultValue(typeof(ChartSubtypes), ChartSubtypes.Plain)]
		public ReportExpression<ChartSubtypes> Subtype
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<ChartSubtypes>>(4);
			}
			set
			{
				base.PropertyStore.SetObject(4, value);
			}
		}

		public EmptyColorStyle Style
		{
			get
			{
				return (EmptyColorStyle)base.PropertyStore.GetObject(5);
			}
			set
			{
				base.PropertyStore.SetObject(5, value);
			}
		}

		public ChartMarker ChartMarker
		{
			get
			{
				return (ChartMarker)base.PropertyStore.GetObject(6);
			}
			set
			{
				base.PropertyStore.SetObject(6, value);
			}
		}

		public ChartDataLabel ChartDataLabel
		{
			get
			{
				return (ChartDataLabel)base.PropertyStore.GetObject(7);
			}
			set
			{
				base.PropertyStore.SetObject(7, value);
			}
		}

		public ChartEmptyPoints ChartEmptyPoints
		{
			get
			{
				return (ChartEmptyPoints)base.PropertyStore.GetObject(8);
			}
			set
			{
				base.PropertyStore.SetObject(8, value);
			}
		}

		[XmlElement(typeof(RdlCollection<CustomProperty>))]
		public IList<CustomProperty> CustomProperties
		{
			get
			{
				return (IList<CustomProperty>)base.PropertyStore.GetObject(9);
			}
			set
			{
				base.PropertyStore.SetObject(9, value);
			}
		}

		[DefaultValue("")]
		public string LegendName
		{
			get
			{
				return (string)base.PropertyStore.GetObject(10);
			}
			set
			{
				base.PropertyStore.SetObject(10, value);
			}
		}

		public ChartItemInLegend ChartItemInLegend
		{
			get
			{
				return (ChartItemInLegend)base.PropertyStore.GetObject(11);
			}
			set
			{
				base.PropertyStore.SetObject(11, value);
			}
		}

		[DefaultValue("")]
		public string ChartAreaName
		{
			get
			{
				return (string)base.PropertyStore.GetObject(12);
			}
			set
			{
				base.PropertyStore.SetObject(12, value);
			}
		}

		[DefaultValue("")]
		public string ValueAxisName
		{
			get
			{
				return (string)base.PropertyStore.GetObject(13);
			}
			set
			{
				base.PropertyStore.SetObject(13, value);
			}
		}

		[DefaultValue("")]
		public string CategoryAxisName
		{
			get
			{
				return (string)base.PropertyStore.GetObject(14);
			}
			set
			{
				base.PropertyStore.SetObject(14, value);
			}
		}

		public ChartSmartLabel ChartSmartLabel
		{
			get
			{
				return (ChartSmartLabel)base.PropertyStore.GetObject(16);
			}
			set
			{
				base.PropertyStore.SetObject(16, value);
			}
		}

		public ChartSeries()
		{
		}

		internal ChartSeries(IPropertyStore propertyStore)
			: base(propertyStore)
		{
		}

		public override void Initialize()
		{
			base.Initialize();
			ChartDataPoints = new RdlCollection<ChartDataPoint>();
			CustomProperties = new RdlCollection<CustomProperty>();
			Style = new EmptyColorStyle();
		}
	}
}
