using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Serialization;

namespace Microsoft.ReportingServices.RdlObjectModel
{
	internal class ChartAxis : ReportObject, INamedObject
	{
		internal class Definition : DefinitionStore<ChartAxis, Definition.Properties>
		{
			internal enum Properties
			{
				Name,
				Visible,
				Style,
				ChartAxisTitle,
				Margin,
				Interval,
				IntervalType,
				IntervalOffset,
				IntervalOffsetType,
				VariableAutoInterval,
				LabelInterval,
				LabelIntervalType,
				LabelIntervalOffset,
				LabelIntervalOffsetType,
				ChartMajorGridLines,
				ChartMinorGridLines,
				ChartMajorTickMarks,
				ChartMinorTickMarks,
				MarksAlwaysAtPlotEdge,
				Reverse,
				CrossAt,
				Location,
				Interlaced,
				InterlacedColor,
				ChartStripLines,
				Arrows,
				Scalar,
				Minimum,
				Maximum,
				LogScale,
				LogBase,
				HideLabels,
				Angle,
				PreventFontShrink,
				PreventFontGrow,
				PreventLabelOffset,
				PreventWordWrap,
				AllowLabelRotation,
				IncludeZero,
				LabelsAutoFitDisabled,
				MinFontSize,
				MaxFontSize,
				OffsetLabels,
				HideEndLabels,
				ChartAxisScaleBreak,
				CustomProperties,
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

		[ReportExpressionDefaultValue(typeof(ChartVisibleTypes), ChartVisibleTypes.Auto)]
		public ReportExpression<ChartVisibleTypes> Visible
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<ChartVisibleTypes>>(1);
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

		public ChartAxisTitle ChartAxisTitle
		{
			get
			{
				return (ChartAxisTitle)base.PropertyStore.GetObject(3);
			}
			set
			{
				base.PropertyStore.SetObject(3, value);
			}
		}

		[ReportExpressionDefaultValue(typeof(ChartAxisMarginVisibleTypes), ChartAxisMarginVisibleTypes.Auto)]
		public ReportExpression<ChartAxisMarginVisibleTypes> Margin
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<ChartAxisMarginVisibleTypes>>(4);
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

		[ReportExpressionDefaultValue(typeof(ChartIntervalTypes), ChartIntervalTypes.Auto)]
		public ReportExpression<ChartIntervalTypes> IntervalType
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<ChartIntervalTypes>>(6);
			}
			set
			{
				base.PropertyStore.SetObject(6, value);
			}
		}

		[ReportExpressionDefaultValue(typeof(double), 0.0)]
		public ReportExpression<double> IntervalOffset
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<double>>(7);
			}
			set
			{
				base.PropertyStore.SetObject(7, value);
			}
		}

		[ReportExpressionDefaultValue(typeof(ChartIntervalOffsetTypes), ChartIntervalOffsetTypes.Auto)]
		public ReportExpression<ChartIntervalOffsetTypes> IntervalOffsetType
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<ChartIntervalOffsetTypes>>(8);
			}
			set
			{
				base.PropertyStore.SetObject(8, value);
			}
		}

		[ReportExpressionDefaultValue(typeof(bool), false)]
		public ReportExpression<bool> VariableAutoInterval
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<bool>>(9);
			}
			set
			{
				base.PropertyStore.SetObject(9, value);
			}
		}

		[ReportExpressionDefaultValue(typeof(double), 0.0)]
		public ReportExpression<double> LabelInterval
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<double>>(10);
			}
			set
			{
				base.PropertyStore.SetObject(10, value);
			}
		}

		[ReportExpressionDefaultValue(typeof(ChartIntervalTypes), ChartIntervalTypes.Default)]
		public ReportExpression<ChartIntervalTypes> LabelIntervalType
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<ChartIntervalTypes>>(11);
			}
			set
			{
				base.PropertyStore.SetObject(11, value);
			}
		}

		[ReportExpressionDefaultValue(typeof(double), 0.0)]
		public ReportExpression<double> LabelIntervalOffset
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<double>>(12);
			}
			set
			{
				base.PropertyStore.SetObject(12, value);
			}
		}

		[ReportExpressionDefaultValue(typeof(ChartIntervalOffsetTypes), ChartIntervalOffsetTypes.Default)]
		public ReportExpression<ChartIntervalOffsetTypes> LabelIntervalOffsetType
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<ChartIntervalOffsetTypes>>(13);
			}
			set
			{
				base.PropertyStore.SetObject(13, value);
			}
		}

		public ChartGridLines ChartMajorGridLines
		{
			get
			{
				return (ChartGridLines)base.PropertyStore.GetObject(14);
			}
			set
			{
				base.PropertyStore.SetObject(14, value);
			}
		}

		public ChartGridLines ChartMinorGridLines
		{
			get
			{
				return (ChartGridLines)base.PropertyStore.GetObject(15);
			}
			set
			{
				base.PropertyStore.SetObject(15, value);
			}
		}

		public ChartTickMarks ChartMajorTickMarks
		{
			get
			{
				return (ChartTickMarks)base.PropertyStore.GetObject(16);
			}
			set
			{
				base.PropertyStore.SetObject(16, value);
			}
		}

		public ChartTickMarks ChartMinorTickMarks
		{
			get
			{
				return (ChartTickMarks)base.PropertyStore.GetObject(17);
			}
			set
			{
				base.PropertyStore.SetObject(17, value);
			}
		}

		[ReportExpressionDefaultValue(typeof(bool), false)]
		public ReportExpression<bool> MarksAlwaysAtPlotEdge
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

		[ReportExpressionDefaultValue(typeof(bool), false)]
		public ReportExpression<bool> Reverse
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<bool>>(19);
			}
			set
			{
				base.PropertyStore.SetObject(19, value);
			}
		}

		[ReportExpressionDefaultValue]
		public ReportExpression CrossAt
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression>(20);
			}
			set
			{
				base.PropertyStore.SetObject(20, value);
			}
		}

		[ReportExpressionDefaultValue(typeof(ChartAxisLocations), ChartAxisLocations.Default)]
		public ReportExpression<ChartAxisLocations> Location
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<ChartAxisLocations>>(21);
			}
			set
			{
				base.PropertyStore.SetObject(21, value);
			}
		}

		[ReportExpressionDefaultValue(typeof(bool), false)]
		public ReportExpression<bool> Interlaced
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<bool>>(22);
			}
			set
			{
				base.PropertyStore.SetObject(22, value);
			}
		}

		[ReportExpressionDefaultValue(typeof(ReportColor))]
		public ReportExpression<ReportColor> InterlacedColor
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<ReportColor>>(23);
			}
			set
			{
				base.PropertyStore.SetObject(23, value);
			}
		}

		[XmlElement(typeof(RdlCollection<ChartStripLine>))]
		public IList<ChartStripLine> ChartStripLines
		{
			get
			{
				return (IList<ChartStripLine>)base.PropertyStore.GetObject(24);
			}
			set
			{
				base.PropertyStore.SetObject(24, value);
			}
		}

		[ReportExpressionDefaultValue(typeof(ChartArrowsTypes), ChartArrowsTypes.None)]
		public ReportExpression<ChartArrowsTypes> Arrows
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<ChartArrowsTypes>>(25);
			}
			set
			{
				base.PropertyStore.SetObject(25, value);
			}
		}

		[DefaultValue(false)]
		public bool Scalar
		{
			get
			{
				return base.PropertyStore.GetBoolean(26);
			}
			set
			{
				base.PropertyStore.SetBoolean(26, value);
			}
		}

		[ReportExpressionDefaultValue]
		public ReportExpression Minimum
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression>(27);
			}
			set
			{
				base.PropertyStore.SetObject(27, value);
			}
		}

		[ReportExpressionDefaultValue]
		public ReportExpression Maximum
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression>(28);
			}
			set
			{
				base.PropertyStore.SetObject(28, value);
			}
		}

		[ReportExpressionDefaultValue(typeof(bool), false)]
		public ReportExpression<bool> LogScale
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<bool>>(29);
			}
			set
			{
				base.PropertyStore.SetObject(29, value);
			}
		}

		[ReportExpressionDefaultValue(typeof(double), 10.0)]
		public ReportExpression<double> LogBase
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<double>>(30);
			}
			set
			{
				base.PropertyStore.SetObject(30, value);
			}
		}

		[ReportExpressionDefaultValue(typeof(bool), false)]
		public ReportExpression<bool> HideLabels
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<bool>>(31);
			}
			set
			{
				base.PropertyStore.SetObject(31, value);
			}
		}

		[ReportExpressionDefaultValue(typeof(double), 0.0)]
		public ReportExpression<double> Angle
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<double>>(32);
			}
			set
			{
				base.PropertyStore.SetObject(32, value);
			}
		}

		[ReportExpressionDefaultValue(typeof(bool), false)]
		public ReportExpression<bool> PreventFontShrink
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<bool>>(33);
			}
			set
			{
				base.PropertyStore.SetObject(33, value);
			}
		}

		[ReportExpressionDefaultValue(typeof(bool), false)]
		public ReportExpression<bool> PreventFontGrow
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<bool>>(34);
			}
			set
			{
				base.PropertyStore.SetObject(34, value);
			}
		}

		[ReportExpressionDefaultValue(typeof(bool), false)]
		public ReportExpression<bool> PreventLabelOffset
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<bool>>(35);
			}
			set
			{
				base.PropertyStore.SetObject(35, value);
			}
		}

		[ReportExpressionDefaultValue(typeof(bool), false)]
		public ReportExpression<bool> PreventWordWrap
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<bool>>(36);
			}
			set
			{
				base.PropertyStore.SetObject(36, value);
			}
		}

		[ReportExpressionDefaultValue(typeof(ChartLabelRotationTypes), ChartLabelRotationTypes.Rotate90)]
		public ReportExpression<ChartLabelRotationTypes> AllowLabelRotation
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<ChartLabelRotationTypes>>(37);
			}
			set
			{
				base.PropertyStore.SetObject(37, value);
			}
		}

		[ReportExpressionDefaultValue(typeof(bool), true)]
		public ReportExpression<bool> IncludeZero
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<bool>>(38);
			}
			set
			{
				base.PropertyStore.SetObject(38, value);
			}
		}

		[ReportExpressionDefaultValue(typeof(bool), false)]
		public ReportExpression<bool> LabelsAutoFitDisabled
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<bool>>(39);
			}
			set
			{
				base.PropertyStore.SetObject(39, value);
			}
		}

		[ReportExpressionDefaultValue(typeof(ReportSize), "6pt")]
		public ReportExpression<ReportSize> MinFontSize
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<ReportSize>>(40);
			}
			set
			{
				base.PropertyStore.SetObject(40, value);
			}
		}

		[ReportExpressionDefaultValue(typeof(ReportSize), "10pt")]
		public ReportExpression<ReportSize> MaxFontSize
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<ReportSize>>(41);
			}
			set
			{
				base.PropertyStore.SetObject(41, value);
			}
		}

		[ReportExpressionDefaultValue(typeof(bool), false)]
		public ReportExpression<bool> OffsetLabels
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<bool>>(42);
			}
			set
			{
				base.PropertyStore.SetObject(42, value);
			}
		}

		[ReportExpressionDefaultValue(typeof(bool), false)]
		public ReportExpression<bool> HideEndLabels
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<bool>>(43);
			}
			set
			{
				base.PropertyStore.SetObject(43, value);
			}
		}

		public ChartAxisScaleBreak ChartAxisScaleBreak
		{
			get
			{
				return (ChartAxisScaleBreak)base.PropertyStore.GetObject(44);
			}
			set
			{
				base.PropertyStore.SetObject(44, value);
			}
		}

		[XmlElement(typeof(RdlCollection<CustomProperty>))]
		public IList<CustomProperty> CustomProperties
		{
			get
			{
				return (IList<CustomProperty>)base.PropertyStore.GetObject(45);
			}
			set
			{
				base.PropertyStore.SetObject(45, value);
			}
		}

		public ChartAxis()
		{
		}

		internal ChartAxis(IPropertyStore propertyStore)
			: base(propertyStore)
		{
		}

		public override void Initialize()
		{
			base.Initialize();
			ChartStripLines = new RdlCollection<ChartStripLine>();
			CustomProperties = new RdlCollection<CustomProperty>();
			IntervalType = ChartIntervalTypes.Auto;
			IntervalOffsetType = ChartIntervalOffsetTypes.Auto;
			LabelIntervalType = ChartIntervalTypes.Default;
			LabelIntervalOffsetType = ChartIntervalOffsetTypes.Default;
			LogBase = 10.0;
			IncludeZero = true;
		}
	}
}
