namespace Microsoft.ReportingServices.RdlObjectModel
{
	internal class DataValue : ReportObject
	{
		internal class Definition : DefinitionStore<DataValue, Definition.Properties>
		{
			internal enum Properties
			{
				Name,
				Value
			}

			private Definition()
			{
			}
		}

		[ReportExpressionDefaultValue]
		public ReportExpression Name
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

		public ReportExpression Value
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

		public DataValue()
		{
		}

		internal DataValue(IPropertyStore propertyStore)
			: base(propertyStore)
		{
		}
	}
}
