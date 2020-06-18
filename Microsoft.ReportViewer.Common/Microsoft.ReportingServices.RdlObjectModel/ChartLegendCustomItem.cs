using System.Collections.Generic;
using System.Xml.Serialization;

namespace Microsoft.ReportingServices.RdlObjectModel
{
	internal class ChartLegendCustomItem : ReportObject, INamedObject
	{
		internal class Definition : DefinitionStore<ChartLegendCustomItem, Definition.Properties>
		{
			internal enum Properties
			{
				Name,
				ChartLegendCustomItemCells,
				Style,
				ChartMarker,
				Separator,
				SeparatorColor,
				ToolTip,
				ToolTipLocID,
				ActionInfo,
				PropertyCount
			}

			private Definition()
			{
			}
		}

		[XmlAttribute(typeof(string))]
		public string Name
		{
			get
			{
				return (string)base.PropertyStore.GetObject(0);
			}
			set
			{
				base.PropertyStore.SetObject(0, value);
			}
		}

		[XmlElement(typeof(RdlCollection<ChartLegendCustomItemCell>))]
		public IList<ChartLegendCustomItemCell> ChartLegendCustomItemCells
		{
			get
			{
				return (IList<ChartLegendCustomItemCell>)base.PropertyStore.GetObject(1);
			}
			set
			{
				base.PropertyStore.SetObject(1, value);
			}
		}

		public Style Style
		{
			get
			{
				return (Style)base.PropertyStore.GetObject(2);
			}
			set
			{
				base.PropertyStore.SetObject(2, value);
			}
		}

		public ChartMarker ChartMarker
		{
			get
			{
				return (ChartMarker)base.PropertyStore.GetObject(3);
			}
			set
			{
				base.PropertyStore.SetObject(3, value);
			}
		}

		[ReportExpressionDefaultValue(typeof(ChartLegendItemSeparatorTypes), ChartLegendItemSeparatorTypes.None)]
		public ReportExpression<ChartLegendItemSeparatorTypes> Separator
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<ChartLegendItemSeparatorTypes>>(4);
			}
			set
			{
				base.PropertyStore.SetObject(4, value);
			}
		}

		[ReportExpressionDefaultValue(typeof(ReportColor))]
		public ReportExpression<ReportColor> SeparatorColor
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<ReportColor>>(5);
			}
			set
			{
				base.PropertyStore.SetObject(5, value);
			}
		}

		[ReportExpressionDefaultValue]
		public ReportExpression ToolTip
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression>(6);
			}
			set
			{
				base.PropertyStore.SetObject(6, value);
			}
		}

		public ActionInfo ActionInfo
		{
			get
			{
				return (ActionInfo)base.PropertyStore.GetObject(8);
			}
			set
			{
				base.PropertyStore.SetObject(8, value);
			}
		}

		public ChartLegendCustomItem()
		{
		}

		internal ChartLegendCustomItem(IPropertyStore propertyStore)
			: base(propertyStore)
		{
		}

		public override void Initialize()
		{
			base.Initialize();
			ChartLegendCustomItemCells = new RdlCollection<ChartLegendCustomItemCell>();
		}
	}
}
