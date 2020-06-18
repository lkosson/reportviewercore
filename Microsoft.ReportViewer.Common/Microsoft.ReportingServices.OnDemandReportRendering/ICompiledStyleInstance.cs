namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal interface ICompiledStyleInstance
	{
		ReportColor BackgroundGradientEndColor
		{
			get;
			set;
		}

		ReportColor BackgroundColor
		{
			get;
			set;
		}

		ReportColor Color
		{
			get;
			set;
		}

		FontStyles FontStyle
		{
			get;
			set;
		}

		string FontFamily
		{
			get;
			set;
		}

		FontWeights FontWeight
		{
			get;
			set;
		}

		string Format
		{
			get;
			set;
		}

		TextDecorations TextDecoration
		{
			get;
			set;
		}

		TextAlignments TextAlign
		{
			get;
			set;
		}

		VerticalAlignments VerticalAlign
		{
			get;
			set;
		}

		Directions Direction
		{
			get;
			set;
		}

		WritingModes WritingMode
		{
			get;
			set;
		}

		string Language
		{
			get;
			set;
		}

		UnicodeBiDiTypes UnicodeBiDi
		{
			get;
			set;
		}

		Calendars Calendar
		{
			get;
			set;
		}

		string CurrencyLanguage
		{
			get;
			set;
		}

		string NumeralLanguage
		{
			get;
			set;
		}

		BackgroundGradients BackgroundGradientType
		{
			get;
			set;
		}

		ReportSize FontSize
		{
			get;
			set;
		}

		ReportSize PaddingLeft
		{
			get;
			set;
		}

		ReportSize PaddingRight
		{
			get;
			set;
		}

		ReportSize PaddingTop
		{
			get;
			set;
		}

		ReportSize PaddingBottom
		{
			get;
			set;
		}

		ReportSize LineHeight
		{
			get;
			set;
		}

		int NumeralVariant
		{
			get;
			set;
		}

		TextEffects TextEffect
		{
			get;
			set;
		}

		BackgroundHatchTypes BackgroundHatchType
		{
			get;
			set;
		}

		ReportColor ShadowColor
		{
			get;
			set;
		}

		ReportSize ShadowOffset
		{
			get;
			set;
		}
	}
}
