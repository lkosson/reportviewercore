namespace Microsoft.ReportingServices.RdlObjectModel
{
	internal class RadialPointer : GaugePointer
	{
		internal new class Definition : DefinitionStore<RadialPointer, Definition.Properties>
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
				Type,
				PointerCap,
				NeedleStyle,
				PropertyCount
			}

			private Definition()
			{
			}
		}

		[ReportExpressionDefaultValue(typeof(RadialPointerTypes), RadialPointerTypes.Needle)]
		public ReportExpression<RadialPointerTypes> Type
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<RadialPointerTypes>>(15);
			}
			set
			{
				base.PropertyStore.SetObject(15, value);
			}
		}

		public PointerCap PointerCap
		{
			get
			{
				return (PointerCap)base.PropertyStore.GetObject(16);
			}
			set
			{
				base.PropertyStore.SetObject(16, value);
			}
		}

		[ReportExpressionDefaultValue(typeof(NeedleStyles), NeedleStyles.Triangular)]
		public ReportExpression<NeedleStyles> NeedleStyle
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<NeedleStyles>>(17);
			}
			set
			{
				base.PropertyStore.SetObject(17, value);
			}
		}

		public RadialPointer()
		{
		}

		internal RadialPointer(IPropertyStore propertyStore)
			: base(propertyStore)
		{
		}
	}
}
