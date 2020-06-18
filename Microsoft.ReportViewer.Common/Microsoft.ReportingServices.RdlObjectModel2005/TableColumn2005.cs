using Microsoft.ReportingServices.RdlObjectModel;
using System.ComponentModel;

namespace Microsoft.ReportingServices.RdlObjectModel2005
{
	internal class TableColumn2005 : ReportObject
	{
		internal class Definition : DefinitionStore<TableColumn2005, Definition.Properties>
		{
			public enum Properties
			{
				Width,
				Visibility,
				FixedHeader
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

		public Visibility Visibility
		{
			get
			{
				return (Visibility)base.PropertyStore.GetObject(1);
			}
			set
			{
				base.PropertyStore.SetObject(1, value);
			}
		}

		[DefaultValue(false)]
		public bool FixedHeader
		{
			get
			{
				return base.PropertyStore.GetBoolean(2);
			}
			set
			{
				base.PropertyStore.SetBoolean(2, value);
			}
		}

		public TableColumn2005()
		{
		}

		public TableColumn2005(IPropertyStore propertyStore)
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
