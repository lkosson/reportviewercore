using System.Xml.Serialization;

namespace Microsoft.ReportingServices.RdlObjectModel
{
	internal class IndicatorState : ReportObject, INamedObject
	{
		internal class Definition : DefinitionStore<IndicatorState, Definition.Properties>
		{
			internal enum Properties
			{
				Name,
				StartValue,
				EndValue,
				Color,
				ScaleFactor,
				IndicatorStyle,
				IndicatorImage,
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

		public GaugeInputValue StartValue
		{
			get
			{
				return (GaugeInputValue)base.PropertyStore.GetObject(1);
			}
			set
			{
				base.PropertyStore.SetObject(1, value);
			}
		}

		public GaugeInputValue EndValue
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

		public ReportExpression<ReportColor> Color
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<ReportColor>>(3);
			}
			set
			{
				base.PropertyStore.SetObject(3, value);
			}
		}

		public ReportExpression<double> ScaleFactor
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

		public ReportExpression<GaugeStateIndicatorStyles> IndicatorStyle
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<GaugeStateIndicatorStyles>>(5);
			}
			set
			{
				base.PropertyStore.SetObject(5, value);
			}
		}

		public IndicatorImage IndicatorImage
		{
			get
			{
				return (IndicatorImage)base.PropertyStore.GetObject(6);
			}
			set
			{
				base.PropertyStore.SetObject(6, value);
			}
		}

		public IndicatorState()
		{
		}

		internal IndicatorState(IPropertyStore propertyStore)
			: base(propertyStore)
		{
		}

		public override void Initialize()
		{
			base.Initialize();
			ScaleFactor = 1.0;
		}
	}
}
