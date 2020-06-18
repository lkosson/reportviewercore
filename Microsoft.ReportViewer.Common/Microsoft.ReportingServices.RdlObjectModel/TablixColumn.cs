namespace Microsoft.ReportingServices.RdlObjectModel
{
	internal class TablixColumn : ReportObject
	{
		internal class Definition : DefinitionStore<TablixColumn, Definition.Properties>
		{
			internal enum Properties
			{
				Width
			}

			private Definition()
			{
			}
		}

		public ReportSize Width
		{
			get
			{
				return base.PropertyStore.GetSize(0);
			}
			set
			{
				base.PropertyStore.SetSize(0, value);
			}
		}

		public TablixColumn()
		{
		}

		internal TablixColumn(IPropertyStore propertyStore)
			: base(propertyStore)
		{
		}

		public override void Initialize()
		{
			base.Initialize();
			Width = Constants.DefaultZeroSize;
		}
	}
}
