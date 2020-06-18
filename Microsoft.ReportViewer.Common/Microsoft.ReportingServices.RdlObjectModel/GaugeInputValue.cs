using System.ComponentModel;

namespace Microsoft.ReportingServices.RdlObjectModel
{
	internal class GaugeInputValue : ReportObject
	{
		internal class Definition : DefinitionStore<GaugeInputValue, Definition.Properties>
		{
			internal enum Properties
			{
				Value,
				Formula,
				MinPercent,
				MaxPercent,
				Multiplier,
				AddConstant,
				DataElementName,
				DataElementOutput,
				PropertyCount
			}

			private Definition()
			{
			}
		}

		public ReportExpression Value
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

		[ReportExpressionDefaultValue(typeof(FormulaTypes), FormulaTypes.None)]
		public ReportExpression<FormulaTypes> Formula
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<FormulaTypes>>(1);
			}
			set
			{
				base.PropertyStore.SetObject(1, value);
			}
		}

		[ReportExpressionDefaultValue(typeof(double), 0.0)]
		public ReportExpression<double> MinPercent
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<double>>(2);
			}
			set
			{
				base.PropertyStore.SetObject(2, value);
			}
		}

		[ReportExpressionDefaultValue(typeof(double), 0.0)]
		public ReportExpression<double> MaxPercent
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

		[ReportExpressionDefaultValue(typeof(double), 0.0)]
		public ReportExpression<double> Multiplier
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

		[ReportExpressionDefaultValue(typeof(double), 0.0)]
		public ReportExpression<double> AddConstant
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<double>>(5);
			}
			set
			{
				base.PropertyStore.SetObject(5, value);
			}
		}

		[DefaultValue("")]
		public string DataElementName
		{
			get
			{
				return (string)base.PropertyStore.GetObject(6);
			}
			set
			{
				base.PropertyStore.SetObject(6, value);
			}
		}

		[DefaultValue(DataElementOutputTypes.Output)]
		[ValidEnumValues("GaugeInputValueDataElementOutputTypes")]
		public DataElementOutputTypes DataElementOutput
		{
			get
			{
				return (DataElementOutputTypes)base.PropertyStore.GetInteger(7);
			}
			set
			{
				((EnumProperty)DefinitionStore<GaugeInputValue, Definition.Properties>.GetProperty(7)).Validate(this, (int)value);
				base.PropertyStore.SetInteger(7, (int)value);
			}
		}

		public GaugeInputValue()
		{
		}

		internal GaugeInputValue(IPropertyStore propertyStore)
			: base(propertyStore)
		{
		}

		public override void Initialize()
		{
			base.Initialize();
			DataElementOutput = DataElementOutputTypes.Output;
		}
	}
}
