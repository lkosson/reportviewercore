using System.ComponentModel;

namespace Microsoft.ReportingServices.RdlObjectModel
{
	internal class TablixCell : DataRegionCell
	{
		internal class Definition : DefinitionStore<TablixCell, Definition.Properties>
		{
			internal enum Properties
			{
				CellContents,
				DataElementName,
				DataElementOutput
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

		[DefaultValue("")]
		public string DataElementName
		{
			get
			{
				return (string)base.PropertyStore.GetObject(1);
			}
			set
			{
				base.PropertyStore.SetObject(1, value);
			}
		}

		[DefaultValue(DataElementOutputTypes.ContentsOnly)]
		[ValidEnumValues("TablixCellDataElementOutputTypes")]
		public DataElementOutputTypes DataElementOutput
		{
			get
			{
				return (DataElementOutputTypes)base.PropertyStore.GetInteger(2);
			}
			set
			{
				base.PropertyStore.SetInteger(2, (int)value);
			}
		}

		public TablixCell()
		{
		}

		internal TablixCell(IPropertyStore propertyStore)
			: base(propertyStore)
		{
		}

		public override void Initialize()
		{
			base.Initialize();
			DataElementOutput = DataElementOutputTypes.ContentsOnly;
		}
	}
}
