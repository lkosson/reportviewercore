using Microsoft.ReportingServices.RdlObjectModel;
using System.ComponentModel;

namespace Microsoft.ReportingServices.RdlObjectModel2005
{
	internal class GridLines2005 : ChartGridLines
	{
		internal new class Definition : DefinitionStore<GridLines2005, Definition.Properties>
		{
			public enum Properties
			{
				ShowGridLines = 6,
				Style
			}

			private Definition()
			{
			}
		}

		[DefaultValue(false)]
		public bool ShowGridLines
		{
			get
			{
				return base.PropertyStore.GetBoolean(6);
			}
			set
			{
				base.PropertyStore.SetBoolean(6, value);
			}
		}

		public new Style2005 Style
		{
			get
			{
				return (Style2005)base.PropertyStore.GetObject(7);
			}
			set
			{
				base.PropertyStore.SetObject(7, value);
			}
		}

		public GridLines2005()
		{
		}

		public GridLines2005(IPropertyStore propertyStore)
			: base(propertyStore)
		{
		}
	}
}
