using Microsoft.ReportingServices.RdlObjectModel.Serialization;

namespace Microsoft.ReportingServices.RdlObjectModel
{
	internal class Border : ReportObject, IShouldSerialize
	{
		internal class Definition : DefinitionStore<Border, Definition.Properties>
		{
			internal enum Properties
			{
				Color,
				Style,
				Width
			}

			private Definition()
			{
			}
		}

		[ReportExpressionDefaultValueConstant(typeof(ReportColor), "DefaultBorderColor")]
		public ReportExpression<ReportColor> Color
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<ReportColor>>(0);
			}
			set
			{
				base.PropertyStore.SetObject(0, value);
			}
		}

		[ReportExpressionDefaultValue(typeof(BorderStyles), BorderStyles.Default)]
		public ReportExpression<BorderStyles> Style
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<BorderStyles>>(1);
			}
			set
			{
				base.PropertyStore.SetObject(1, value);
			}
		}

		[ReportExpressionDefaultValueConstant(typeof(ReportSize), "DefaultBorderWidth")]
		public ReportExpression<ReportSize> Width
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<ReportSize>>(2);
			}
			set
			{
				base.PropertyStore.SetObject(2, value);
			}
		}

		public Border()
		{
		}

		internal Border(IPropertyStore propertyStore)
			: base(propertyStore)
		{
		}

		bool IShouldSerialize.ShouldSerializeThis()
		{
			return true;
		}

		SerializationMethod IShouldSerialize.ShouldSerializeProperty(string property)
		{
			if (property == "Style" && !Style.IsExpression && Style.Value == BorderStyles.Default)
			{
				return SerializationMethod.Never;
			}
			if (base.Parent is Style && ((Style)base.Parent).Border != this)
			{
				return SerializationMethod.Always;
			}
			return SerializationMethod.Auto;
		}
	}
}
