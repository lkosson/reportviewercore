using System;
using System.Drawing;

namespace Microsoft.Reporting.Chart.WebForms
{
	internal interface IChart
	{
		AnnotationCollection Annotations
		{
			get;
		}

		AntiAlias AntiAlias
		{
			get;
			set;
		}

		AntiAliasingTypes AntiAliasing
		{
			get;
			set;
		}

		Color BackColor
		{
			get;
			set;
		}

		Color BackGradientEndColor
		{
			get;
			set;
		}

		GradientType BackGradientType
		{
			get;
			set;
		}

		ChartHatchStyle BackHatchStyle
		{
			get;
			set;
		}

		string BackImage
		{
			get;
			set;
		}

		ChartImageAlign BackImageAlign
		{
			get;
			set;
		}

		ChartImageWrapMode BackImageMode
		{
			get;
			set;
		}

		Color BackImageTransparentColor
		{
			get;
			set;
		}

		Color BorderColor
		{
			get;
			set;
		}

		Color BorderlineColor
		{
			get;
			set;
		}

		ChartDashStyle BorderlineStyle
		{
			get;
			set;
		}

		int BorderlineWidth
		{
			get;
			set;
		}

		BorderSkinAttributes BorderSkin
		{
			get;
			set;
		}

		ChartDashStyle BorderStyle
		{
			get;
			set;
		}

		int BorderWidth
		{
			get;
			set;
		}

		string BuildNumber
		{
			get;
			set;
		}

		ChartAreaCollection ChartAreas
		{
			get;
		}

		string CodeException
		{
			get;
			set;
		}

		DataManipulator DataManipulator
		{
			get;
		}

		ChartEdition Edition
		{
			get;
		}

		Color ForeColor
		{
			get;
			set;
		}

		int Width
		{
			get;
			set;
		}

		int Height
		{
			get;
			set;
		}

		float ImageResolution
		{
			get;
			set;
		}

		NamedImagesCollection Images
		{
			get;
		}

		ChartImageType ImageType
		{
			get;
			set;
		}

		string ImageUrl
		{
			get;
			set;
		}

		Legend Legend
		{
			get;
			set;
		}

		LegendCollection Legends
		{
			get;
		}

		MapAreasCollection MapAreas
		{
			get;
		}

		string MultiValueSeparator
		{
			get;
			set;
		}

		Title NoDataMessage
		{
			get;
			set;
		}

		ChartColorPalette Palette
		{
			get;
			set;
		}

		Color[] PaletteCustomColors
		{
			get;
			set;
		}

		double RenderingDpiX
		{
			get;
			set;
		}

		double RenderingDpiY
		{
			get;
			set;
		}

		bool ReverseSeriesOrder
		{
			get;
			set;
		}

		SeriesCollection Series
		{
			get;
		}

		bool SoftShadows
		{
			get;
			set;
		}

		bool SuppressCodeExceptions
		{
			get;
			set;
		}

		bool SuppressExceptions
		{
			get;
			set;
		}

		TextAntiAliasingQuality TextAntiAliasingQuality
		{
			get;
			set;
		}

		string Title
		{
			get;
			set;
		}

		Font TitleFont
		{
			get;
			set;
		}

		Color TitleFontColor
		{
			get;
			set;
		}

		TitleCollection Titles
		{
			get;
		}

		void AlignDataPointsByAxisLabel();

		void AlignDataPointsByAxisLabel(PointsSortOrder sortingOrder);

		void AlignDataPointsByAxisLabel(string series);

		void AlignDataPointsByAxisLabel(string series, PointsSortOrder sortingOrder);

		void ApplyPaletteColors();

		object GetService(Type serviceType);

		HitTestResult HitTest(int x, int y);

		HitTestResult HitTest(int x, int y, ChartElementType requestedElement);

		HitTestResult HitTest(int x, int y, bool ignoreTransparent);

		void RaisePostBackEvent(string eventArgument);

		void ResetAutoValues();

		void ResetPaletteCustomColors();

		void SaveXml(string name);

		void Select(int x, int y, out string series, out int point);

		bool ShouldSerializePaletteCustomColors();
	}
}
