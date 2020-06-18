using System.ComponentModel;

namespace Microsoft.ReportingServices.RdlObjectModel
{
	internal class CellContents : ReportObject
	{
		internal class Definition : DefinitionStore<CellContents, Definition.Properties>
		{
			internal enum Properties
			{
				ReportItem,
				ColSpan,
				RowSpan
			}

			private Definition()
			{
			}
		}

		public ReportItem ReportItem
		{
			get
			{
				return (ReportItem)base.PropertyStore.GetObject(0);
			}
			set
			{
				base.PropertyStore.SetObject(0, value);
			}
		}

		[DefaultValue(1)]
		[ValidValues(1, int.MaxValue)]
		public int ColSpan
		{
			get
			{
				return base.PropertyStore.GetInteger(1);
			}
			set
			{
				((IntProperty)DefinitionStore<CellContents, Definition.Properties>.GetProperty(1)).Validate(this, value);
				base.PropertyStore.SetInteger(1, value);
			}
		}

		[DefaultValue(1)]
		[ValidValues(1, int.MaxValue)]
		public int RowSpan
		{
			get
			{
				return base.PropertyStore.GetInteger(2);
			}
			set
			{
				((IntProperty)DefinitionStore<CellContents, Definition.Properties>.GetProperty(2)).Validate(this, value);
				base.PropertyStore.SetInteger(2, value);
			}
		}

		public CellContents()
		{
		}

		internal CellContents(IPropertyStore propertyStore)
			: base(propertyStore)
		{
		}

		public override void Initialize()
		{
			base.Initialize();
			ColSpan = 1;
			RowSpan = 1;
		}
	}
}
