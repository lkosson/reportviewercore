using Microsoft.ReportingServices.RdlObjectModel.Serialization;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Microsoft.ReportingServices.RdlObjectModel
{
	[XmlElementClass("LinearScale", typeof(LinearScale))]
	[XmlElementClass("RadialScale", typeof(RadialScale))]
	internal class GaugeScale : ReportObject, INamedObject
	{
		internal class Definition : DefinitionStore<GaugeScale, Definition.Properties>
		{
			internal enum Properties
			{
				Name,
				GaugePointers,
				ScaleRanges,
				Style,
				CustomLabels,
				Interval,
				IntervalOffset,
				Logarithmic,
				LogarithmicBase,
				MaximumValue,
				MinimumValue,
				Multiplier,
				Reversed,
				GaugeMajorTickMarks,
				GaugeMinorTickMarks,
				MaximumPin,
				MinimumPin,
				ScaleLabels,
				TickMarksOnTop,
				ToolTip,
				ActionInfo,
				Hidden,
				Width,
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

		[XmlElement(typeof(RdlCollection<GaugePointer>))]
		public IList<GaugePointer> GaugePointers
		{
			get
			{
				return (IList<GaugePointer>)base.PropertyStore.GetObject(1);
			}
			set
			{
				base.PropertyStore.SetObject(1, value);
			}
		}

		[XmlElement(typeof(RdlCollection<ScaleRange>))]
		public IList<ScaleRange> ScaleRanges
		{
			get
			{
				return (IList<ScaleRange>)base.PropertyStore.GetObject(2);
			}
			set
			{
				base.PropertyStore.SetObject(2, value);
			}
		}

		public Style Style
		{
			get
			{
				return (Style)base.PropertyStore.GetObject(3);
			}
			set
			{
				base.PropertyStore.SetObject(3, value);
			}
		}

		[XmlElement(typeof(RdlCollection<CustomLabel>))]
		public IList<CustomLabel> CustomLabels
		{
			get
			{
				return (IList<CustomLabel>)base.PropertyStore.GetObject(4);
			}
			set
			{
				base.PropertyStore.SetObject(4, value);
			}
		}

		[ReportExpressionDefaultValue(typeof(double), 0.0)]
		public ReportExpression<double> Interval
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<double>>(5);
			}
			set
			{
				base.PropertyStore.SetObject(5, value);
			}
		}

		[ReportExpressionDefaultValue(typeof(double), 0.0)]
		public ReportExpression<double> IntervalOffset
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<double>>(6);
			}
			set
			{
				base.PropertyStore.SetObject(6, value);
			}
		}

		[ReportExpressionDefaultValue(typeof(bool), false)]
		public ReportExpression<bool> Logarithmic
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

		[ReportExpressionDefaultValue(typeof(double), 10.0)]
		public ReportExpression<double> LogarithmicBase
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<double>>(8);
			}
			set
			{
				base.PropertyStore.SetObject(8, value);
			}
		}

		public GaugeInputValue MaximumValue
		{
			get
			{
				return (GaugeInputValue)base.PropertyStore.GetObject(9);
			}
			set
			{
				base.PropertyStore.SetObject(9, value);
			}
		}

		public GaugeInputValue MinimumValue
		{
			get
			{
				return (GaugeInputValue)base.PropertyStore.GetObject(10);
			}
			set
			{
				base.PropertyStore.SetObject(10, value);
			}
		}

		[ReportExpressionDefaultValue(typeof(double), 1.0)]
		public ReportExpression<double> Multiplier
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<double>>(11);
			}
			set
			{
				base.PropertyStore.SetObject(11, value);
			}
		}

		[ReportExpressionDefaultValue(typeof(bool), false)]
		public ReportExpression<bool> Reversed
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<bool>>(12);
			}
			set
			{
				base.PropertyStore.SetObject(12, value);
			}
		}

		public GaugeTickMarks GaugeMajorTickMarks
		{
			get
			{
				return (GaugeTickMarks)base.PropertyStore.GetObject(13);
			}
			set
			{
				base.PropertyStore.SetObject(13, value);
			}
		}

		public GaugeTickMarks GaugeMinorTickMarks
		{
			get
			{
				return (GaugeTickMarks)base.PropertyStore.GetObject(14);
			}
			set
			{
				base.PropertyStore.SetObject(14, value);
			}
		}

		public ScalePin MaximumPin
		{
			get
			{
				return (ScalePin)base.PropertyStore.GetObject(15);
			}
			set
			{
				base.PropertyStore.SetObject(15, value);
			}
		}

		public ScalePin MinimumPin
		{
			get
			{
				return (ScalePin)base.PropertyStore.GetObject(16);
			}
			set
			{
				base.PropertyStore.SetObject(16, value);
			}
		}

		public ScaleLabels ScaleLabels
		{
			get
			{
				return (ScaleLabels)base.PropertyStore.GetObject(17);
			}
			set
			{
				base.PropertyStore.SetObject(17, value);
			}
		}

		[ReportExpressionDefaultValue(typeof(bool), false)]
		public ReportExpression<bool> TickMarksOnTop
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<bool>>(18);
			}
			set
			{
				base.PropertyStore.SetObject(18, value);
			}
		}

		[ReportExpressionDefaultValue]
		public ReportExpression ToolTip
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression>(19);
			}
			set
			{
				base.PropertyStore.SetObject(19, value);
			}
		}

		public ActionInfo ActionInfo
		{
			get
			{
				return (ActionInfo)base.PropertyStore.GetObject(20);
			}
			set
			{
				base.PropertyStore.SetObject(20, value);
			}
		}

		[ReportExpressionDefaultValue(typeof(bool), false)]
		public ReportExpression<bool> Hidden
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<bool>>(21);
			}
			set
			{
				base.PropertyStore.SetObject(21, value);
			}
		}

		[ReportExpressionDefaultValue(typeof(double), 5.0)]
		public ReportExpression<double> Width
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<double>>(22);
			}
			set
			{
				base.PropertyStore.SetObject(22, value);
			}
		}

		public GaugeScale()
		{
		}

		internal GaugeScale(IPropertyStore propertyStore)
			: base(propertyStore)
		{
		}

		public override void Initialize()
		{
			base.Initialize();
			GaugePointers = new RdlCollection<GaugePointer>();
			ScaleRanges = new RdlCollection<ScaleRange>();
			CustomLabels = new RdlCollection<CustomLabel>();
			LogarithmicBase = 10.0;
			Multiplier = 1.0;
			Width = 5.0;
		}
	}
}
