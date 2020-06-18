using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Serialization;

namespace Microsoft.ReportingServices.RdlObjectModel
{
	internal class StateIndicator : GaugePanelItem
	{
		internal new class Definition : DefinitionStore<StateIndicator, Definition.Properties>
		{
			internal enum Properties
			{
				Name,
				Style,
				Top,
				Left,
				Height,
				Width,
				ZIndex,
				Hidden,
				ToolTip,
				ActionInfo,
				ParentItem,
				GaugeInputValue,
				TransformationType,
				TransformationScope,
				MinimumValue,
				MaximumValue,
				IndicatorStyle,
				IndicatorImage,
				ScaleFactor,
				IndicatorStates,
				ResizeMode,
				Angle,
				StateDataElementName,
				StateDataElementOutput,
				PropertyCount
			}

			private Definition()
			{
			}
		}

		public GaugeInputValue GaugeInputValue
		{
			get
			{
				return (GaugeInputValue)base.PropertyStore.GetObject(11);
			}
			set
			{
				base.PropertyStore.SetObject(11, value);
			}
		}

		[ReportExpressionDefaultValue(typeof(GaugeTransformationType), GaugeTransformationType.Percentage)]
		public ReportExpression<GaugeTransformationType> TransformationType
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<GaugeTransformationType>>(12);
			}
			set
			{
				base.PropertyStore.SetObject(12, value);
			}
		}

		public string TransformationScope
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

		public GaugeInputValue MinimumValue
		{
			get
			{
				return (GaugeInputValue)base.PropertyStore.GetObject(14);
			}
			set
			{
				base.PropertyStore.SetObject(14, value);
			}
		}

		public GaugeInputValue MaximumValue
		{
			get
			{
				return (GaugeInputValue)base.PropertyStore.GetObject(15);
			}
			set
			{
				base.PropertyStore.SetObject(15, value);
			}
		}

		[ReportExpressionDefaultValue(typeof(GaugeStateIndicatorStyles), GaugeStateIndicatorStyles.Circle)]
		public ReportExpression<GaugeStateIndicatorStyles> IndicatorStyle
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<GaugeStateIndicatorStyles>>(16);
			}
			set
			{
				base.PropertyStore.SetObject(16, value);
			}
		}

		public IndicatorImage IndicatorImage
		{
			get
			{
				return (IndicatorImage)base.PropertyStore.GetObject(17);
			}
			set
			{
				base.PropertyStore.SetObject(17, value);
			}
		}

		public ReportExpression<double> ScaleFactor
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<double>>(18);
			}
			set
			{
				base.PropertyStore.SetObject(18, value);
			}
		}

		[XmlElement(typeof(RdlCollection<IndicatorState>))]
		public IList<IndicatorState> IndicatorStates
		{
			get
			{
				return (IList<IndicatorState>)base.PropertyStore.GetObject(19);
			}
			set
			{
				base.PropertyStore.SetObject(19, value);
			}
		}

		[ReportExpressionDefaultValue(typeof(ResizeModes), ResizeModes.AutoFit)]
		public ReportExpression<ResizeModes> ResizeMode
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<ResizeModes>>(20);
			}
			set
			{
				base.PropertyStore.SetObject(20, value);
			}
		}

		public ReportExpression<double> Angle
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<double>>(21);
			}
			set
			{
				base.PropertyStore.SetObject(21, value);
			}
		}

		public string StateDataElementName
		{
			get
			{
				return (string)base.PropertyStore.GetObject(22);
			}
			set
			{
				base.PropertyStore.SetObject(22, value);
			}
		}

		[DefaultValue(DataElementOutputTypes.Output)]
		[ValidEnumValues("GaugeInputValueDataElementOutputTypes")]
		public DataElementOutputTypes StateDataElementOutput
		{
			get
			{
				return (DataElementOutputTypes)base.PropertyStore.GetInteger(23);
			}
			set
			{
				((EnumProperty)DefinitionStore<StateIndicator, Definition.Properties>.GetProperty(23)).Validate(this, (int)value);
				base.PropertyStore.SetInteger(23, (int)value);
			}
		}

		public StateIndicator()
		{
		}

		internal StateIndicator(IPropertyStore propertyStore)
			: base(propertyStore)
		{
		}

		public override void Initialize()
		{
			base.Initialize();
			IndicatorStyle = GaugeStateIndicatorStyles.Circle;
			ScaleFactor = 1.0;
			IndicatorStates = new RdlCollection<IndicatorState>();
			ResizeMode = ResizeModes.AutoFit;
			StateDataElementOutput = DataElementOutputTypes.Output;
		}
	}
}
