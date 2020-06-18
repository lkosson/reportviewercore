using Microsoft.ReportingServices.RdlObjectModel;
using System.ComponentModel;

namespace Microsoft.ReportingServices.RdlObjectModel2005
{
	internal class Title2005 : ChartTitle
	{
		internal new class Definition : DefinitionStore<Title2005, Definition.Properties>
		{
			public enum Properties
			{
				Caption = 14,
				Style,
				Position
			}

			private Definition()
			{
			}
		}

		[ReportExpressionDefaultValue]
		public new ReportExpression Caption
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression>(14);
			}
			set
			{
				base.PropertyStore.SetObject(14, value);
			}
		}

		public new Style2005 Style
		{
			get
			{
				return (Style2005)base.PropertyStore.GetObject(15);
			}
			set
			{
				base.PropertyStore.SetObject(15, value);
			}
		}

		[DefaultValue(TitlePositions2005.Center)]
		public new TitlePositions2005 Position
		{
			get
			{
				return (TitlePositions2005)base.PropertyStore.GetInteger(16);
			}
			set
			{
				base.PropertyStore.SetInteger(16, (int)value);
			}
		}

		public Title2005()
		{
		}

		public Title2005(IPropertyStore propertyStore)
			: base(propertyStore)
		{
		}
	}
}
