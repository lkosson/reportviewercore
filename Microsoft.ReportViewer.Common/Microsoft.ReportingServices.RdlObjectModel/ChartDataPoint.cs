using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Serialization;

namespace Microsoft.ReportingServices.RdlObjectModel
{
	internal class ChartDataPoint : DataRegionCell
	{
		internal class Definition : DefinitionStore<ChartDataPoint, Definition.Properties>
		{
			internal enum Properties
			{
				ChartDataPointValues,
				ChartDataLabel,
				AxisLabel,
				ToolTip,
				ActionInfo,
				Style,
				ChartMarker,
				DataElementName,
				DataElementOutput,
				ChartItemInLegend,
				CustomProperties,
				PropertyCount
			}

			private Definition()
			{
			}
		}

		public ChartDataPointValues ChartDataPointValues
		{
			get
			{
				return (ChartDataPointValues)base.PropertyStore.GetObject(0);
			}
			set
			{
				base.PropertyStore.SetObject(0, value);
			}
		}

		public ChartDataLabel ChartDataLabel
		{
			get
			{
				return (ChartDataLabel)base.PropertyStore.GetObject(1);
			}
			set
			{
				base.PropertyStore.SetObject(1, value);
			}
		}

		[ReportExpressionDefaultValue]
		public ReportExpression AxisLabel
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression>(2);
			}
			set
			{
				base.PropertyStore.SetObject(2, value);
			}
		}

		[ReportExpressionDefaultValue]
		public ReportExpression ToolTip
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression>(3);
			}
			set
			{
				base.PropertyStore.SetObject(3, value);
			}
		}

		public ActionInfo ActionInfo
		{
			get
			{
				return (ActionInfo)base.PropertyStore.GetObject(4);
			}
			set
			{
				base.PropertyStore.SetObject(4, value);
			}
		}

		public EmptyColorStyle Style
		{
			get
			{
				return (EmptyColorStyle)base.PropertyStore.GetObject(5);
			}
			set
			{
				base.PropertyStore.SetObject(5, value);
			}
		}

		public ChartMarker ChartMarker
		{
			get
			{
				return (ChartMarker)base.PropertyStore.GetObject(6);
			}
			set
			{
				base.PropertyStore.SetObject(6, value);
			}
		}

		[DefaultValue("")]
		public string DataElementName
		{
			get
			{
				return (string)base.PropertyStore.GetObject(7);
			}
			set
			{
				base.PropertyStore.SetObject(7, value);
			}
		}

		[DefaultValue(DataElementOutputTypes.ContentsOnly)]
		[ValidEnumValues("DataPointDataElementOutputTypes")]
		public DataElementOutputTypes DataElementOutput
		{
			get
			{
				return (DataElementOutputTypes)base.PropertyStore.GetInteger(8);
			}
			set
			{
				((EnumProperty)DefinitionStore<ChartDataPoint, Definition.Properties>.GetProperty(8)).Validate(this, (int)value);
				base.PropertyStore.SetInteger(8, (int)value);
			}
		}

		public ChartItemInLegend ChartItemInLegend
		{
			get
			{
				return (ChartItemInLegend)base.PropertyStore.GetObject(9);
			}
			set
			{
				base.PropertyStore.SetObject(9, value);
			}
		}

		[XmlElement(typeof(RdlCollection<CustomProperty>))]
		public IList<CustomProperty> CustomProperties
		{
			get
			{
				return (IList<CustomProperty>)base.PropertyStore.GetObject(10);
			}
			set
			{
				base.PropertyStore.SetObject(10, value);
			}
		}

		public ChartDataPoint()
		{
		}

		internal ChartDataPoint(IPropertyStore propertyStore)
			: base(propertyStore)
		{
		}

		public override void Initialize()
		{
			base.Initialize();
			ChartDataPointValues = new ChartDataPointValues();
			CustomProperties = new RdlCollection<CustomProperty>();
			DataElementOutput = DataElementOutputTypes.ContentsOnly;
		}
	}
}
