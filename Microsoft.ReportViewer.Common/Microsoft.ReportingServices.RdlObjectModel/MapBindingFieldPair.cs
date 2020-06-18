namespace Microsoft.ReportingServices.RdlObjectModel
{
	internal class MapBindingFieldPair : ReportObject
	{
		internal class Definition : DefinitionStore<MapBindingFieldPair, Definition.Properties>
		{
			internal enum Properties
			{
				FieldName,
				BindingExpression,
				PropertyCount
			}

			private Definition()
			{
			}
		}

		public ReportExpression FieldName
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

		public ReportExpression BindingExpression
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

		public MapBindingFieldPair()
		{
		}

		internal MapBindingFieldPair(IPropertyStore propertyStore)
			: base(propertyStore)
		{
		}

		public override void Initialize()
		{
			base.Initialize();
		}
	}
}
