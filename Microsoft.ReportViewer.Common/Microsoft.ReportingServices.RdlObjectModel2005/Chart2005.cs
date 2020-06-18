using Microsoft.ReportingServices.RdlObjectModel;
using Microsoft.ReportingServices.RdlObjectModel.Serialization;
using Microsoft.ReportingServices.RdlObjectModel2005.Upgrade;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Serialization;

namespace Microsoft.ReportingServices.RdlObjectModel2005
{
	internal class Chart2005 : Chart, IReportItem2005, IUpgradeable, IPageBreakLocation2005
	{
		internal new class Definition : DefinitionStore<Chart2005, Definition.Properties>
		{
			public enum Properties
			{
				Action = 38,
				PageBreakAtStart,
				PageBreakAtEnd,
				Type,
				Subtype,
				SeriesGroupings,
				CategoryGroupings,
				ChartData,
				Legend,
				CategoryAxis,
				ValueAxis,
				Title,
				PointWidth,
				Palette,
				ThreeDProperties,
				PlotArea,
				PropertyCount
			}

			private Definition()
			{
			}
		}

		[DefaultValue(ChartTypes2005.Column)]
		public ChartTypes2005 Type
		{
			get
			{
				return (ChartTypes2005)base.PropertyStore.GetInteger(41);
			}
			set
			{
				base.PropertyStore.SetInteger(41, (int)value);
			}
		}

		[DefaultValue(ChartSubtypes2005.Plain)]
		public ChartSubtypes2005 Subtype
		{
			get
			{
				return (ChartSubtypes2005)base.PropertyStore.GetInteger(42);
			}
			set
			{
				base.PropertyStore.SetInteger(42, (int)value);
			}
		}

		[XmlElement(typeof(RdlCollection<SeriesGrouping2005>))]
		[XmlArrayItem("SeriesGrouping", typeof(SeriesGrouping2005))]
		public IList<SeriesGrouping2005> SeriesGroupings
		{
			get
			{
				return (IList<SeriesGrouping2005>)base.PropertyStore.GetObject(43);
			}
			set
			{
				base.PropertyStore.SetObject(43, value);
			}
		}

		[XmlElement(typeof(RdlCollection<CategoryGrouping2005>))]
		[XmlArrayItem("CategoryGrouping", typeof(CategoryGrouping2005))]
		public IList<CategoryGrouping2005> CategoryGroupings
		{
			get
			{
				return (IList<CategoryGrouping2005>)base.PropertyStore.GetObject(44);
			}
			set
			{
				base.PropertyStore.SetObject(44, value);
			}
		}

		[XmlElement(typeof(RdlCollection<ChartSeries2005>))]
		[XmlArrayItem("ChartSeries", typeof(ChartSeries2005))]
		public new IList<ChartSeries2005> ChartData
		{
			get
			{
				return (IList<ChartSeries2005>)base.PropertyStore.GetObject(45);
			}
			set
			{
				base.PropertyStore.SetObject(45, value);
			}
		}

		public Legend2005 Legend
		{
			get
			{
				return (Legend2005)base.PropertyStore.GetObject(46);
			}
			set
			{
				base.PropertyStore.SetObject(46, value);
			}
		}

		public CategoryAxis2005 CategoryAxis
		{
			get
			{
				return (CategoryAxis2005)base.PropertyStore.GetObject(47);
			}
			set
			{
				base.PropertyStore.SetObject(47, value);
			}
		}

		public ValueAxis2005 ValueAxis
		{
			get
			{
				return (ValueAxis2005)base.PropertyStore.GetObject(48);
			}
			set
			{
				base.PropertyStore.SetObject(48, value);
			}
		}

		public Title2005 Title
		{
			get
			{
				return (Title2005)base.PropertyStore.GetObject(49);
			}
			set
			{
				base.PropertyStore.SetObject(49, value);
			}
		}

		[DefaultValue(0)]
		public int PointWidth
		{
			get
			{
				return base.PropertyStore.GetInteger(50);
			}
			set
			{
				base.PropertyStore.SetInteger(50, value);
			}
		}

		public ThreeDProperties2005 ThreeDProperties
		{
			get
			{
				return (ThreeDProperties2005)base.PropertyStore.GetObject(52);
			}
			set
			{
				base.PropertyStore.SetObject(52, value);
			}
		}

		public PlotArea2005 PlotArea
		{
			get
			{
				return (PlotArea2005)base.PropertyStore.GetObject(53);
			}
			set
			{
				base.PropertyStore.SetObject(53, value);
			}
		}

		[DefaultValue(false)]
		public bool PageBreakAtStart
		{
			get
			{
				return base.PropertyStore.GetBoolean(39);
			}
			set
			{
				base.PropertyStore.SetBoolean(39, value);
			}
		}

		[ReportExpressionDefaultValue]
		public ReportExpression NoRows
		{
			get
			{
				return base.NoRowsMessage;
			}
			set
			{
				base.NoRowsMessage = value;
			}
		}

		[DefaultValue(false)]
		public bool PageBreakAtEnd
		{
			get
			{
				return base.PropertyStore.GetBoolean(40);
			}
			set
			{
				base.PropertyStore.SetBoolean(40, value);
			}
		}

		public Action Action
		{
			get
			{
				return (Action)base.PropertyStore.GetObject(38);
			}
			set
			{
				base.PropertyStore.SetObject(38, value);
			}
		}

		[ReportExpressionDefaultValue]
		public ReportExpression Label
		{
			get
			{
				return base.DocumentMapLabel;
			}
			set
			{
				base.DocumentMapLabel = value;
			}
		}

		[XmlChildAttribute("Label", "LocID", "http://schemas.microsoft.com/SQLServer/reporting/reportdesigner")]
		public string LabelLocID
		{
			get
			{
				return (string)base.PropertyStore.GetObject(12);
			}
			set
			{
				base.PropertyStore.SetObject(12, value);
			}
		}

		public new Style2005 Style
		{
			get
			{
				return (Style2005)base.Style;
			}
			set
			{
				base.Style = value;
			}
		}

		public Chart2005()
		{
		}

		public Chart2005(IPropertyStore propertyStore)
			: base(propertyStore)
		{
		}

		public override void Initialize()
		{
			base.Initialize();
			SeriesGroupings = new RdlCollection<SeriesGrouping2005>();
			CategoryGroupings = new RdlCollection<CategoryGrouping2005>();
		}

		public void Upgrade(UpgradeImpl2005 upgrader)
		{
			upgrader.UpgradeChart(this);
		}
	}
}
