using System.Globalization;

namespace Microsoft.ReportingServices.RdlObjectModel
{
	internal class MapLegendTitle : ReportObject
	{
		internal class Definition : DefinitionStore<MapLegendTitle, Definition.Properties>
		{
			internal enum Properties
			{
				Style,
				Caption,
				TitleSeparator,
				TitleSeparatorColor,
				PropertyCount
			}

			private Definition()
			{
			}
		}

		public Style Style
		{
			get
			{
				return (Style)base.PropertyStore.GetObject(0);
			}
			set
			{
				base.PropertyStore.SetObject(0, value);
			}
		}

		public ReportExpression Caption
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression>(1);
			}
			set
			{
				base.PropertyStore.SetObject(1, value);
			}
		}

		[ReportExpressionDefaultValue(typeof(MapLegendTitleSeparators), MapLegendTitleSeparators.None)]
		public ReportExpression<MapLegendTitleSeparators> TitleSeparator
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<MapLegendTitleSeparators>>(2);
			}
			set
			{
				base.PropertyStore.SetObject(2, value);
			}
		}

		[ReportExpressionDefaultValue(typeof(ReportColor), "Gray")]
		public ReportExpression<ReportColor> TitleSeparatorColor
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<ReportColor>>(3);
			}
			set
			{
				base.PropertyStore.SetObject(3, value);
			}
		}

		public MapLegendTitle()
		{
		}

		internal MapLegendTitle(IPropertyStore propertyStore)
			: base(propertyStore)
		{
		}

		public override void Initialize()
		{
			base.Initialize();
			TitleSeparator = MapLegendTitleSeparators.None;
			TitleSeparatorColor = new ReportExpression<ReportColor>("Gray", CultureInfo.InvariantCulture);
		}
	}
}
