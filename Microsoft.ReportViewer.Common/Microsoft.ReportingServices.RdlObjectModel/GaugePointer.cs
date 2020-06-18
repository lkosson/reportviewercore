using Microsoft.ReportingServices.RdlObjectModel.Serialization;
using System.Xml.Serialization;

namespace Microsoft.ReportingServices.RdlObjectModel
{
	[XmlElementClass("LinearPointer", typeof(LinearPointer))]
	[XmlElementClass("RadialPointer", typeof(RadialPointer))]
	internal class GaugePointer : ReportObject, INamedObject
	{
		internal class Definition : DefinitionStore<GaugePointer, Definition.Properties>
		{
			internal enum Properties
			{
				Name,
				Style,
				GaugeInputValue,
				BarStart,
				DistanceFromScale,
				PointerImage,
				MarkerLength,
				MarkerStyle,
				Placement,
				SnappingEnabled,
				SnappingInterval,
				ToolTip,
				ActionInfo,
				Hidden,
				Width,
				PropertyCount
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

		public Style Style
		{
			get
			{
				return (Style)base.PropertyStore.GetObject(1);
			}
			set
			{
				base.PropertyStore.SetObject(1, value);
			}
		}

		public GaugeInputValue GaugeInputValue
		{
			get
			{
				return (GaugeInputValue)base.PropertyStore.GetObject(2);
			}
			set
			{
				base.PropertyStore.SetObject(2, value);
			}
		}

		[ReportExpressionDefaultValue(typeof(BarStartTypes), BarStartTypes.ScaleStart)]
		public ReportExpression<BarStartTypes> BarStart
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<BarStartTypes>>(3);
			}
			set
			{
				base.PropertyStore.SetObject(3, value);
			}
		}

		[ReportExpressionDefaultValue(typeof(double), 0.0)]
		public ReportExpression<double> DistanceFromScale
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<double>>(4);
			}
			set
			{
				base.PropertyStore.SetObject(4, value);
			}
		}

		public PointerImage PointerImage
		{
			get
			{
				return (PointerImage)base.PropertyStore.GetObject(5);
			}
			set
			{
				base.PropertyStore.SetObject(5, value);
			}
		}

		public ReportExpression<double> MarkerLength
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<double>>(6);
			}
			set
			{
				base.PropertyStore.SetObject(6, value);
			}
		}

		[ReportExpressionDefaultValue(typeof(MarkerStyles), MarkerStyles.Triangle)]
		public ReportExpression<MarkerStyles> MarkerStyle
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<MarkerStyles>>(7);
			}
			set
			{
				base.PropertyStore.SetObject(7, value);
			}
		}

		public ReportExpression<Placements> Placement
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<Placements>>(8);
			}
			set
			{
				base.PropertyStore.SetObject(8, value);
			}
		}

		[ReportExpressionDefaultValue(typeof(bool), false)]
		public ReportExpression<bool> SnappingEnabled
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<bool>>(9);
			}
			set
			{
				base.PropertyStore.SetObject(9, value);
			}
		}

		[ReportExpressionDefaultValue(typeof(double), 0.0)]
		public ReportExpression<double> SnappingInterval
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<double>>(10);
			}
			set
			{
				base.PropertyStore.SetObject(10, value);
			}
		}

		[ReportExpressionDefaultValue]
		public ReportExpression ToolTip
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression>(11);
			}
			set
			{
				base.PropertyStore.SetObject(11, value);
			}
		}

		public ActionInfo ActionInfo
		{
			get
			{
				return (ActionInfo)base.PropertyStore.GetObject(12);
			}
			set
			{
				base.PropertyStore.SetObject(12, value);
			}
		}

		[ReportExpressionDefaultValue(typeof(bool), false)]
		public ReportExpression<bool> Hidden
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<bool>>(13);
			}
			set
			{
				base.PropertyStore.SetObject(13, value);
			}
		}

		public ReportExpression<double> Width
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<double>>(14);
			}
			set
			{
				base.PropertyStore.SetObject(14, value);
			}
		}

		public GaugePointer()
		{
		}

		internal GaugePointer(IPropertyStore propertyStore)
			: base(propertyStore)
		{
		}

		public override void Initialize()
		{
			base.Initialize();
			MarkerStyle = MarkerStyles.Triangle;
		}
	}
}
