using System.ComponentModel;

namespace Microsoft.ReportingServices.RdlObjectModel
{
	internal class ParameterValue : ReportObject
	{
		internal class Definition : DefinitionStore<ParameterValue, Definition.Properties>
		{
			internal enum Properties
			{
				Value,
				Label,
				LabelLocID
			}

			private Definition()
			{
			}
		}

		[DefaultValue(null)]
		public ReportExpression? Value
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression?>(0);
			}
			set
			{
				base.PropertyStore.SetObject(0, value);
			}
		}

		[ReportExpressionDefaultValue]
		public ReportExpression Label
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression>(1);
			}
			set
			{
				base.PropertyStore.SetObject(1, value);
			}
		}

		public ParameterValue()
		{
		}

		internal ParameterValue(IPropertyStore propertyStore)
			: base(propertyStore)
		{
		}
	}
}
