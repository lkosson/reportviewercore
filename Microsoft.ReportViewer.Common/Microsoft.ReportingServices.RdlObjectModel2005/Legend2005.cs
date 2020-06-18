using Microsoft.ReportingServices.RdlObjectModel;
using System.ComponentModel;

namespace Microsoft.ReportingServices.RdlObjectModel2005
{
	internal class Legend2005 : ChartLegend
	{
		internal new class Definition : DefinitionStore<Legend2005, Definition.Properties>
		{
			public enum Properties
			{
				Visible = 24,
				Style,
				Position,
				Layout,
				InsidePlotArea
			}

			private Definition()
			{
			}
		}

		[DefaultValue(false)]
		public bool Visible
		{
			get
			{
				return base.PropertyStore.GetBoolean(24);
			}
			set
			{
				base.PropertyStore.SetBoolean(24, value);
			}
		}

		public new Style2005 Style
		{
			get
			{
				return (Style2005)base.PropertyStore.GetObject(25);
			}
			set
			{
				base.PropertyStore.SetObject(25, value);
			}
		}

		[DefaultValue(LegendLayouts2005.Column)]
		public new LegendLayouts2005 Layout
		{
			get
			{
				return (LegendLayouts2005)base.PropertyStore.GetInteger(27);
			}
			set
			{
				base.PropertyStore.SetInteger(27, (int)value);
			}
		}

		[DefaultValue(false)]
		public bool InsidePlotArea
		{
			get
			{
				return base.PropertyStore.GetBoolean(28);
			}
			set
			{
				base.PropertyStore.SetBoolean(28, value);
			}
		}

		public Legend2005()
		{
		}

		public Legend2005(IPropertyStore propertyStore)
			: base(propertyStore)
		{
		}
	}
}
