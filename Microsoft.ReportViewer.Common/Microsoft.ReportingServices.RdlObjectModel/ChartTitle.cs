using System.ComponentModel;
using System.Xml.Serialization;

namespace Microsoft.ReportingServices.RdlObjectModel
{
	internal class ChartTitle : ReportObject, INamedObject
	{
		internal class Definition : DefinitionStore<ChartTitle, Definition.Properties>
		{
			internal enum Properties
			{
				Name,
				Caption,
				CaptionLocID,
				Hidden,
				Style,
				Position,
				DockToChartArea,
				DockOutsideChartArea,
				DockOffset,
				ChartElementPosition,
				ToolTip,
				ToolTipLocID,
				ActionInfo,
				TextOrientation,
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

		[ReportExpressionDefaultValue(typeof(bool), false)]
		public ReportExpression<bool> Hidden
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<bool>>(3);
			}
			set
			{
				base.PropertyStore.SetObject(3, value);
			}
		}

		public Style Style
		{
			get
			{
				return (Style)base.PropertyStore.GetObject(4);
			}
			set
			{
				base.PropertyStore.SetObject(4, value);
			}
		}

		[ReportExpressionDefaultValue(typeof(ChartPositions), ChartPositions.TopCenter)]
		public ReportExpression<ChartPositions> Position
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<ChartPositions>>(5);
			}
			set
			{
				base.PropertyStore.SetObject(5, value);
			}
		}

		[DefaultValue("")]
		public string DockToChartArea
		{
			get
			{
				return (string)base.PropertyStore.GetObject(6);
			}
			set
			{
				base.PropertyStore.SetObject(6, value);
			}
		}

		[ReportExpressionDefaultValue(typeof(bool), false)]
		public ReportExpression<bool> DockOutsideChartArea
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<bool>>(7);
			}
			set
			{
				base.PropertyStore.SetObject(7, value);
			}
		}

		[ReportExpressionDefaultValue(typeof(int), 0)]
		public ReportExpression<int> DockOffset
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<int>>(8);
			}
			set
			{
				base.PropertyStore.SetObject(8, value);
			}
		}

		public ChartElementPosition ChartElementPosition
		{
			get
			{
				return (ChartElementPosition)base.PropertyStore.GetObject(9);
			}
			set
			{
				base.PropertyStore.SetObject(9, value);
			}
		}

		[ReportExpressionDefaultValue]
		public ReportExpression ToolTip
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression>(10);
			}
			set
			{
				base.PropertyStore.SetObject(10, value);
			}
		}

		public ActionInfo ActionInfo
		{
			get
			{
				return (ActionInfo)base.PropertyStore.GetObject(12);
			}
			set
			{
				base.PropertyStore.SetObject(12, value);
			}
		}

		[ReportExpressionDefaultValue(typeof(TextOrientations), TextOrientations.Auto)]
		public ReportExpression<TextOrientations> TextOrientation
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<TextOrientations>>(13);
			}
			set
			{
				base.PropertyStore.SetObject(13, value);
			}
		}

		public ChartTitle()
		{
		}

		internal ChartTitle(IPropertyStore propertyStore)
			: base(propertyStore)
		{
		}

		public override void Initialize()
		{
			base.Initialize();
			Position = ChartPositions.TopCenter;
		}
	}
}
