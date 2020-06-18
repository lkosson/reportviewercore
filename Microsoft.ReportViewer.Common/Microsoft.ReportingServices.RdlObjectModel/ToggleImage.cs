namespace Microsoft.ReportingServices.RdlObjectModel
{
	internal class ToggleImage : ReportObject
	{
		internal class Definition : DefinitionStore<ToggleImage, Definition.Properties>
		{
			internal enum Properties
			{
				InitialState
			}

			private Definition()
			{
			}
		}

		public ReportExpression<bool> InitialState
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<bool>>(0);
			}
			set
			{
				base.PropertyStore.SetObject(0, value);
			}
		}

		public ToggleImage()
		{
		}

		internal ToggleImage(IPropertyStore propertyStore)
			: base(propertyStore)
		{
		}
	}
}
