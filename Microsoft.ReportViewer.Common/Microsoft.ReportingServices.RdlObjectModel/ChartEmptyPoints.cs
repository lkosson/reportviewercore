using System.Collections.Generic;
using System.Xml.Serialization;

namespace Microsoft.ReportingServices.RdlObjectModel
{
	internal class ChartEmptyPoints : ReportObject
	{
		internal class Definition : DefinitionStore<ChartEmptyPoints, Definition.Properties>
		{
			internal enum Properties
			{
				Style,
				ChartMarker,
				ChartDataLabel,
				AxisLabel,
				ToolTip,
				ActionInfo,
				CustomProperties,
				PropertyCount
			}

			private Definition()
			{
			}
		}

		public EmptyColorStyle Style
		{
			get
			{
				return (EmptyColorStyle)base.PropertyStore.GetObject(0);
			}
			set
			{
				base.PropertyStore.SetObject(0, value);
			}
		}

		public ChartMarker ChartMarker
		{
			get
			{
				return (ChartMarker)base.PropertyStore.GetObject(1);
			}
			set
			{
				base.PropertyStore.SetObject(1, value);
			}
		}

		public ChartDataLabel ChartDataLabel
		{
			get
			{
				return (ChartDataLabel)base.PropertyStore.GetObject(2);
			}
			set
			{
				base.PropertyStore.SetObject(2, value);
			}
		}

		[ReportExpressionDefaultValue]
		public ReportExpression AxisLabel
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

		[ReportExpressionDefaultValue]
		public ReportExpression ToolTip
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression>(4);
			}
			set
			{
				base.PropertyStore.SetObject(4, value);
			}
		}

		public ActionInfo ActionInfo
		{
			get
			{
				return (ActionInfo)base.PropertyStore.GetObject(5);
			}
			set
			{
				base.PropertyStore.SetObject(5, value);
			}
		}

		[XmlElement(typeof(RdlCollection<CustomProperty>))]
		public IList<CustomProperty> CustomProperties
		{
			get
			{
				return (IList<CustomProperty>)base.PropertyStore.GetObject(6);
			}
			set
			{
				base.PropertyStore.SetObject(6, value);
			}
		}

		public ChartEmptyPoints()
		{
		}

		internal ChartEmptyPoints(IPropertyStore propertyStore)
			: base(propertyStore)
		{
		}

		public override void Initialize()
		{
			base.Initialize();
			CustomProperties = new RdlCollection<CustomProperty>();
		}
	}
}
