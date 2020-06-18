namespace Microsoft.ReportingServices.RdlObjectModel
{
	internal class CellDefinition : ReportObject
	{
		internal class Definition : DefinitionStore<GridLayoutDefinition, Definition.Properties>
		{
			internal enum Properties
			{
				ColumnIndex,
				RowIndex,
				ParameterName
			}

			private Definition()
			{
			}
		}

		public int ColumnIndex
		{
			get
			{
				return base.PropertyStore.GetInteger(0);
			}
			set
			{
				base.PropertyStore.SetInteger(0, value);
			}
		}

		public int RowIndex
		{
			get
			{
				return base.PropertyStore.GetInteger(1);
			}
			set
			{
				base.PropertyStore.SetInteger(1, value);
			}
		}

		public string ParameterName
		{
			get
			{
				return (string)base.PropertyStore.GetObject(2);
			}
			set
			{
				base.PropertyStore.SetObject(2, value);
			}
		}

		public CellDefinition()
		{
			ColumnIndex = 0;
			RowIndex = 0;
		}

		internal CellDefinition(IPropertyStore propertyStore)
			: base(propertyStore)
		{
		}
	}
}
