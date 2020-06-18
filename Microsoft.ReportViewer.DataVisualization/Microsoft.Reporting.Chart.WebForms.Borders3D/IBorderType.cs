using System.Drawing;

namespace Microsoft.Reporting.Chart.WebForms.Borders3D
{
	internal interface IBorderType
	{
		string Name
		{
			get;
		}

		float Resolution
		{
			set;
		}

		void DrawBorder(ChartGraphics graph, BorderSkinAttributes borderSkin, RectangleF rect, Color backColor, ChartHatchStyle backHatchStyle, string backImage, ChartImageWrapMode backImageMode, Color backImageTranspColor, ChartImageAlign backImageAlign, GradientType backGradientType, Color backGradientEndColor, Color borderColor, int borderWidth, ChartDashStyle borderStyle);

		void AdjustAreasPosition(ChartGraphics graph, ref RectangleF areasRect);

		RectangleF GetTitlePositionInBorder();
	}
}
