using System.Xml.Serialization;

namespace Microsoft.ReportingServices.RdlObjectModel
{
	internal class ScaleRange : ReportObject, INamedObject
	{
		internal class Definition : DefinitionStore<ScaleRange, Definition.Properties>
		{
			internal enum Properties
			{
				Name,
				Style,
				BackgroundGradientType,
				DistanceFromScale,
				StartValue,
				EndValue,
				StartWidth,
				EndWidth,
				InRangeBarPointerColor,
				InRangeLabelColor,
				InRangeTickMarksColor,
				Placement,
				ToolTip,
				ActionInfo,
				Hidden,
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

		[ReportExpressionDefaultValue(typeof(GaugeBackgroundGradients), GaugeBackgroundGradients.StartToEnd)]
		public ReportExpression<GaugeBackgroundGradients> BackgroundGradientType
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<GaugeBackgroundGradients>>(2);
			}
			set
			{
				base.PropertyStore.SetObject(2, value);
			}
		}

		public ReportExpression<double> DistanceFromScale
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<double>>(3);
			}
			set
			{
				base.PropertyStore.SetObject(3, value);
			}
		}

		public GaugeInputValue StartValue
		{
			get
			{
				return (GaugeInputValue)base.PropertyStore.GetObject(4);
			}
			set
			{
				base.PropertyStore.SetObject(4, value);
			}
		}

		public GaugeInputValue EndValue
		{
			get
			{
				return (GaugeInputValue)base.PropertyStore.GetObject(5);
			}
			set
			{
				base.PropertyStore.SetObject(5, value);
			}
		}

		public ReportExpression<double> StartWidth
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

		public ReportExpression<double> EndWidth
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<double>>(7);
			}
			set
			{
				base.PropertyStore.SetObject(7, value);
			}
		}

		[ReportExpressionDefaultValue(typeof(ReportColor))]
		public ReportExpression<ReportColor> InRangeBarPointerColor
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<ReportColor>>(8);
			}
			set
			{
				base.PropertyStore.SetObject(8, value);
			}
		}

		[ReportExpressionDefaultValue(typeof(ReportColor))]
		public ReportExpression<ReportColor> InRangeLabelColor
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<ReportColor>>(9);
			}
			set
			{
				base.PropertyStore.SetObject(9, value);
			}
		}

		[ReportExpressionDefaultValue(typeof(ReportColor))]
		public ReportExpression<ReportColor> InRangeTickMarksColor
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<ReportColor>>(10);
			}
			set
			{
				base.PropertyStore.SetObject(10, value);
			}
		}

		public ReportExpression<Placements> Placement
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<Placements>>(11);
			}
			set
			{
				base.PropertyStore.SetObject(11, value);
			}
		}

		[ReportExpressionDefaultValue]
		public ReportExpression ToolTip
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression>(12);
			}
			set
			{
				base.PropertyStore.SetObject(12, value);
			}
		}

		public ActionInfo ActionInfo
		{
			get
			{
				return (ActionInfo)base.PropertyStore.GetObject(13);
			}
			set
			{
				base.PropertyStore.SetObject(13, value);
			}
		}

		[ReportExpressionDefaultValue(typeof(bool), false)]
		public ReportExpression<bool> Hidden
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<bool>>(14);
			}
			set
			{
				base.PropertyStore.SetObject(14, value);
			}
		}

		public ScaleRange()
		{
		}

		internal ScaleRange(IPropertyStore propertyStore)
			: base(propertyStore)
		{
		}
	}
}
