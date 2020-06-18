using System.Collections.Generic;
using System.Xml.Serialization;

namespace Microsoft.ReportingServices.RdlObjectModel
{
	internal class GaugePanel : DataRegion
	{
		internal new class Definition : DefinitionStore<GaugePanel, Definition.Properties>
		{
			internal enum Properties
			{
				Style,
				Name,
				ActionInfo,
				Top,
				Left,
				Height,
				Width,
				ZIndex,
				Visibility,
				ToolTip,
				ToolTipLocID,
				DocumentMapLabel,
				DocumentMapLabelLocID,
				Bookmark,
				RepeatWith,
				CustomProperties,
				DataElementName,
				DataElementOutput,
				KeepTogether,
				NoRowsMessage,
				DataSetName,
				PageBreak,
				PageName,
				Filters,
				SortExpressions,
				LinearGauges,
				RadialGauges,
				NumericIndicators,
				StateIndicators,
				GaugeImages,
				GaugeLabels,
				GaugeMember,
				AntiAliasing,
				AutoLayout,
				BackFrame,
				ShadowIntensity,
				TextAntiAliasingQuality,
				TopImage,
				PropertyCount
			}

			private Definition()
			{
			}
		}

		[XmlElement(typeof(RdlCollection<LinearGauge>))]
		public IList<LinearGauge> LinearGauges
		{
			get
			{
				return (IList<LinearGauge>)base.PropertyStore.GetObject(25);
			}
			set
			{
				base.PropertyStore.SetObject(25, value);
			}
		}

		[XmlElement(typeof(RdlCollection<RadialGauge>))]
		public IList<RadialGauge> RadialGauges
		{
			get
			{
				return (IList<RadialGauge>)base.PropertyStore.GetObject(26);
			}
			set
			{
				base.PropertyStore.SetObject(26, value);
			}
		}

		[XmlElement(typeof(RdlCollection<NumericIndicator>))]
		public IList<NumericIndicator> NumericIndicators
		{
			get
			{
				return (IList<NumericIndicator>)base.PropertyStore.GetObject(27);
			}
			set
			{
				base.PropertyStore.SetObject(27, value);
			}
		}

		[XmlElement(typeof(RdlCollection<StateIndicator>))]
		public IList<StateIndicator> StateIndicators
		{
			get
			{
				return (IList<StateIndicator>)base.PropertyStore.GetObject(28);
			}
			set
			{
				base.PropertyStore.SetObject(28, value);
			}
		}

		[XmlElement(typeof(RdlCollection<GaugeImage>))]
		public IList<GaugeImage> GaugeImages
		{
			get
			{
				return (IList<GaugeImage>)base.PropertyStore.GetObject(29);
			}
			set
			{
				base.PropertyStore.SetObject(29, value);
			}
		}

		[XmlElement(typeof(RdlCollection<GaugeLabel>))]
		public IList<GaugeLabel> GaugeLabels
		{
			get
			{
				return (IList<GaugeLabel>)base.PropertyStore.GetObject(30);
			}
			set
			{
				base.PropertyStore.SetObject(30, value);
			}
		}

		public GaugeMember GaugeMember
		{
			get
			{
				return (GaugeMember)base.PropertyStore.GetObject(31);
			}
			set
			{
				base.PropertyStore.SetObject(31, value);
			}
		}

		[ReportExpressionDefaultValue(typeof(AntiAliasingTypes), AntiAliasingTypes.All)]
		public ReportExpression<AntiAliasingTypes> AntiAliasing
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<AntiAliasingTypes>>(32);
			}
			set
			{
				base.PropertyStore.SetObject(32, value);
			}
		}

		[ReportExpressionDefaultValue(typeof(bool), false)]
		public ReportExpression<bool> AutoLayout
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

		public BackFrame BackFrame
		{
			get
			{
				return (BackFrame)base.PropertyStore.GetObject(34);
			}
			set
			{
				base.PropertyStore.SetObject(34, value);
			}
		}

		[ValidValues(0.0, 100.0)]
		[ReportExpressionDefaultValue(typeof(double), 25.0)]
		public ReportExpression<double> ShadowIntensity
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<double>>(35);
			}
			set
			{
				base.PropertyStore.SetObject(35, value);
			}
		}

		[ReportExpressionDefaultValue(typeof(TextAntiAliasingQualityTypes), TextAntiAliasingQualityTypes.High)]
		public ReportExpression<TextAntiAliasingQualityTypes> TextAntiAliasingQuality
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<TextAntiAliasingQualityTypes>>(36);
			}
			set
			{
				base.PropertyStore.SetObject(36, value);
			}
		}

		public TopImage TopImage
		{
			get
			{
				return (TopImage)base.PropertyStore.GetObject(37);
			}
			set
			{
				base.PropertyStore.SetObject(37, value);
			}
		}

		public GaugePanel()
		{
		}

		internal GaugePanel(IPropertyStore propertyStore)
			: base(propertyStore)
		{
		}

		public override void Initialize()
		{
			base.Initialize();
			LinearGauges = new RdlCollection<LinearGauge>();
			RadialGauges = new RdlCollection<RadialGauge>();
			NumericIndicators = new RdlCollection<NumericIndicator>();
			StateIndicators = new RdlCollection<StateIndicator>();
			GaugeImages = new RdlCollection<GaugeImage>();
			GaugeLabels = new RdlCollection<GaugeLabel>();
			ShadowIntensity = 25.0;
		}
	}
}
