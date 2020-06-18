namespace Microsoft.ReportingServices.RdlObjectModel
{
	internal class TablixHeader : ReportObject
	{
		internal class Definition : DefinitionStore<TablixHeader, Definition.Properties>
		{
			internal enum Properties
			{
				Size,
				CellContents
			}

			private Definition()
			{
			}
		}

		public ReportSize Size
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

		public CellContents CellContents
		{
			get
			{
				return (CellContents)base.PropertyStore.GetObject(1);
			}
			set
			{
				base.PropertyStore.SetObject(1, value);
			}
		}

		public TablixHeader()
		{
		}

		internal TablixHeader(IPropertyStore propertyStore)
			: base(propertyStore)
		{
		}

		public override void Initialize()
		{
			base.Initialize();
			Size = Constants.DefaultZeroSize;
			CellContents = new CellContents();
		}
	}
}
