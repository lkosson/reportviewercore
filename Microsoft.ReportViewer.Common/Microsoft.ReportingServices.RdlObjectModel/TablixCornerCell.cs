namespace Microsoft.ReportingServices.RdlObjectModel
{
	internal class TablixCornerCell : ReportObject
	{
		internal class Definition : DefinitionStore<TablixCornerCell, Definition.Properties>
		{
			internal enum Properties
			{
				CellContents
			}

			private Definition()
			{
			}
		}

		public CellContents CellContents
		{
			get
			{
				return (CellContents)base.PropertyStore.GetObject(0);
			}
			set
			{
				base.PropertyStore.SetObject(0, value);
			}
		}

		public TablixCornerCell()
		{
		}

		internal TablixCornerCell(IPropertyStore propertyStore)
			: base(propertyStore)
		{
		}
	}
}
