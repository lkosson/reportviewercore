namespace Microsoft.ReportingServices.RdlObjectModel
{
	internal class LinearPointer : GaugePointer
	{
		internal new class Definition : DefinitionStore<LinearPointer, Definition.Properties>
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
				Thermometer,
				PropertyCount
			}

			private Definition()
			{
			}
		}

		[ReportExpressionDefaultValue(typeof(LinearPointerTypes), LinearPointerTypes.Marker)]
		public ReportExpression<LinearPointerTypes> Type
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<LinearPointerTypes>>(15);
			}
			set
			{
				base.PropertyStore.SetObject(15, value);
			}
		}

		public Thermometer Thermometer
		{
			get
			{
				return (Thermometer)base.PropertyStore.GetObject(16);
			}
			set
			{
				base.PropertyStore.SetObject(16, value);
			}
		}

		public LinearPointer()
		{
		}

		internal LinearPointer(IPropertyStore propertyStore)
			: base(propertyStore)
		{
		}
	}
}
