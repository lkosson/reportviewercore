namespace Microsoft.ReportingServices.RdlObjectModel
{
	internal class CustomProperty : ReportObject
	{
		internal class Definition : DefinitionStore<CustomProperty, Definition.Properties>
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

		public CustomProperty()
		{
		}

		internal CustomProperty(IPropertyStore propertyStore)
			: base(propertyStore)
		{
		}
	}
}
