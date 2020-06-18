using System.Collections.Generic;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal abstract class StyleBaseInstance : BaseInstance
	{
		internal RenderingContext m_renderingContext;

		public abstract List<StyleAttributeNames> StyleAttributes
		{
			get;
		}

		public abstract object this[StyleAttributeNames style]
		{
			get;
		}

		public abstract ReportColor BackgroundGradientEndColor
		{
			get;
			set;
		}

		public abstract ReportColor BackgroundColor
		{
			get;
			set;
		}

		public abstract ReportColor Color
		{
			get;
			set;
		}

		public abstract FontStyles FontStyle
		{
			get;
			set;
		}

		public abstract string FontFamily
		{
			get;
			set;
		}

		public abstract FontWeights FontWeight
		{
			get;
			set;
		}

		public abstract string Format
		{
			get;
			set;
		}

		public abstract TextDecorations TextDecoration
		{
			get;
			set;
		}

		public abstract TextAlignments TextAlign
		{
			get;
			set;
		}

		public abstract VerticalAlignments VerticalAlign
		{
			get;
			set;
		}

		public abstract Directions Direction
		{
			get;
			set;
		}

		public abstract WritingModes WritingMode
		{
			get;
			set;
		}

		public abstract string Language
		{
			get;
			set;
		}

		public abstract UnicodeBiDiTypes UnicodeBiDi
		{
			get;
			set;
		}

		public abstract Calendars Calendar
		{
			get;
			set;
		}

		public abstract string CurrencyLanguage
		{
			get;
			set;
		}

		public abstract string NumeralLanguage
		{
			get;
			set;
		}

		public abstract BackgroundGradients BackgroundGradientType
		{
			get;
			set;
		}

		public abstract ReportSize FontSize
		{
			get;
			set;
		}

		public abstract ReportSize PaddingLeft
		{
			get;
			set;
		}

		public abstract ReportSize PaddingRight
		{
			get;
			set;
		}

		public abstract ReportSize PaddingTop
		{
			get;
			set;
		}

		public abstract ReportSize PaddingBottom
		{
			get;
			set;
		}

		public abstract ReportSize LineHeight
		{
			get;
			set;
		}

		public abstract int NumeralVariant
		{
			get;
			set;
		}

		public abstract TextEffects TextEffect
		{
			get;
			set;
		}

		public abstract BackgroundHatchTypes BackgroundHatchType
		{
			get;
			set;
		}

		public abstract ReportColor ShadowColor
		{
			get;
			set;
		}

		public abstract ReportSize ShadowOffset
		{
			get;
			set;
		}

		internal StyleBaseInstance(RenderingContext context, IReportScope reportScope)
			: base(reportScope)
		{
			m_renderingContext = context;
		}
	}
}
