using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Serialization;

namespace Microsoft.ReportingServices.RdlObjectModel
{
	internal class ChartArea : ReportObject, INamedObject
	{
		internal class Definition : DefinitionStore<ChartArea, Definition.Properties>
		{
			internal enum Properties
			{
				Name,
				Hidden,
				ChartCategoryAxes,
				ChartValueAxes,
				ChartThreeDProperties,
				Style,
				AlignOrientation,
				ChartAlignType,
				AlignWithChartArea,
				ChartElementPosition,
				ChartInnerPlotPosition,
				EquallySizedAxesFont,
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

		[ReportExpressionDefaultValue(typeof(bool), false)]
		public ReportExpression<bool> Hidden
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<bool>>(1);
			}
			set
			{
				base.PropertyStore.SetObject(1, value);
			}
		}

		[XmlElement(typeof(RdlCollection<ChartAxis>))]
		public IList<ChartAxis> ChartCategoryAxes
		{
			get
			{
				return (IList<ChartAxis>)base.PropertyStore.GetObject(2);
			}
			set
			{
				base.PropertyStore.SetObject(2, value);
			}
		}

		[XmlElement(typeof(RdlCollection<ChartAxis>))]
		public IList<ChartAxis> ChartValueAxes
		{
			get
			{
				return (IList<ChartAxis>)base.PropertyStore.GetObject(3);
			}
			set
			{
				base.PropertyStore.SetObject(3, value);
			}
		}

		public ChartThreeDProperties ChartThreeDProperties
		{
			get
			{
				return (ChartThreeDProperties)base.PropertyStore.GetObject(4);
			}
			set
			{
				base.PropertyStore.SetObject(4, value);
			}
		}

		public Style Style
		{
			get
			{
				return (Style)base.PropertyStore.GetObject(5);
			}
			set
			{
				base.PropertyStore.SetObject(5, value);
			}
		}

		[ReportExpressionDefaultValue(typeof(ChartAlignOrientations), ChartAlignOrientations.None)]
		public ReportExpression<ChartAlignOrientations> AlignOrientation
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<ChartAlignOrientations>>(6);
			}
			set
			{
				base.PropertyStore.SetObject(6, value);
			}
		}

		public ChartAlignType ChartAlignType
		{
			get
			{
				return (ChartAlignType)base.PropertyStore.GetObject(7);
			}
			set
			{
				base.PropertyStore.SetObject(7, value);
			}
		}

		[DefaultValue("")]
		public string AlignWithChartArea
		{
			get
			{
				return (string)base.PropertyStore.GetObject(8);
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

		public ChartElementPosition ChartInnerPlotPosition
		{
			get
			{
				return (ChartElementPosition)base.PropertyStore.GetObject(10);
			}
			set
			{
				base.PropertyStore.SetObject(10, value);
			}
		}

		[ReportExpressionDefaultValue(typeof(bool), false)]
		public ReportExpression<bool> EquallySizedAxesFont
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<bool>>(11);
			}
			set
			{
				base.PropertyStore.SetObject(11, value);
			}
		}

		public ChartArea()
		{
		}

		internal ChartArea(IPropertyStore propertyStore)
			: base(propertyStore)
		{
		}

		public override void Initialize()
		{
			base.Initialize();
			ChartCategoryAxes = new RdlCollection<ChartAxis>();
			ChartValueAxes = new RdlCollection<ChartAxis>();
		}
	}
}
