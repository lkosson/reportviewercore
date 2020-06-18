using Microsoft.Reporting.Chart.WebForms.Utilities;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Microsoft.Reporting.Chart.WebForms
{
	[SRDescription("DescriptionAttributeTextAnnotation_TextAnnotation")]
	internal class TextAnnotation : Annotation
	{
		private string text = "";

		private bool multiline;

		internal SizeF contentSize = SizeF.Empty;

		internal bool isEllipse;

		internal bool restrictedPermissions;

		[SRCategory("CategoryAttributeAppearance")]
		[DefaultValue("")]
		[SRDescription("DescriptionAttributeText4")]
		public virtual string Text
		{
			get
			{
				return text;
			}
			set
			{
				text = value;
				Invalidate();
				contentSize = SizeF.Empty;
			}
		}

		[SRCategory("CategoryAttributeAppearance")]
		[DefaultValue(false)]
		[SRDescription("DescriptionAttributeMultiline")]
		public virtual bool Multiline
		{
			get
			{
				return multiline;
			}
			set
			{
				multiline = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryAttributeAppearance")]
		[DefaultValue(typeof(Font), "Microsoft Sans Serif, 8pt")]
		[SRDescription("DescriptionAttributeTextFont4")]
		public override Font TextFont
		{
			get
			{
				return base.TextFont;
			}
			set
			{
				base.TextFont = value;
				contentSize = SizeF.Empty;
			}
		}

		[SRCategory("CategoryAttributeAppearance")]
		[Browsable(false)]
		[DefaultValue(typeof(Color), "Black")]
		[SRDescription("DescriptionAttributeTextAnnotation_LineWidth")]
		public override Color LineColor
		{
			get
			{
				return base.LineColor;
			}
			set
			{
				base.LineColor = value;
			}
		}

		[SRCategory("CategoryAttributeAppearance")]
		[Browsable(false)]
		[DefaultValue(1)]
		[SRDescription("DescriptionAttributeTextAnnotation_LineWidth")]
		public override int LineWidth
		{
			get
			{
				return base.LineWidth;
			}
			set
			{
				base.LineWidth = value;
			}
		}

		[SRCategory("CategoryAttributeAppearance")]
		[Browsable(false)]
		[DefaultValue(ChartDashStyle.Solid)]
		[SRDescription("DescriptionAttributeTextAnnotation_LineWidth")]
		public override ChartDashStyle LineStyle
		{
			get
			{
				return base.LineStyle;
			}
			set
			{
				base.LineStyle = value;
			}
		}

		[SRCategory("CategoryAttributeAppearance")]
		[Browsable(false)]
		[DefaultValue(typeof(Color), "")]
		[SRDescription("DescriptionAttributeTextAnnotation_LineWidth")]
		[NotifyParentProperty(true)]
		public override Color BackColor
		{
			get
			{
				return base.BackColor;
			}
			set
			{
				base.BackColor = value;
			}
		}

		[SRCategory("CategoryAttributeAppearance")]
		[Browsable(false)]
		[DefaultValue(ChartHatchStyle.None)]
		[NotifyParentProperty(true)]
		[SRDescription("DescriptionAttributeTextAnnotation_LineWidth")]
		public override ChartHatchStyle BackHatchStyle
		{
			get
			{
				return base.BackHatchStyle;
			}
			set
			{
				base.BackHatchStyle = value;
			}
		}

		[SRCategory("CategoryAttributeAppearance")]
		[Browsable(false)]
		[DefaultValue(GradientType.None)]
		[NotifyParentProperty(true)]
		[SRDescription("DescriptionAttributeTextAnnotation_LineWidth")]
		public override GradientType BackGradientType
		{
			get
			{
				return base.BackGradientType;
			}
			set
			{
				base.BackGradientType = value;
			}
		}

		[SRCategory("CategoryAttributeAppearance")]
		[Browsable(false)]
		[DefaultValue(typeof(Color), "")]
		[NotifyParentProperty(true)]
		[SRDescription("DescriptionAttributeTextAnnotation_LineWidth")]
		public override Color BackGradientEndColor
		{
			get
			{
				return base.BackGradientEndColor;
			}
			set
			{
				base.BackGradientEndColor = value;
			}
		}

		[SRCategory("CategoryAttributeMisc")]
		[Bindable(true)]
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[SerializationVisibility(SerializationVisibility.Hidden)]
		[SRDescription("DescriptionAttributeTextAnnotation_AnnotationType")]
		public override string AnnotationType => "Text";

		[SRCategory("CategoryAttributeAppearance")]
		[DefaultValue(SelectionPointsStyle.Rectangle)]
		[ParenthesizePropertyName(true)]
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[SerializationVisibility(SerializationVisibility.Hidden)]
		[SRDescription("DescriptionAttributeSelectionPointsStyle3")]
		internal override SelectionPointsStyle SelectionPointsStyle => SelectionPointsStyle.Rectangle;

		internal override void Paint(Chart chart, ChartGraphics graphics)
		{
			Chart = chart;
			PointF location = PointF.Empty;
			PointF anchorLocation = PointF.Empty;
			SizeF size = SizeF.Empty;
			GetRelativePosition(out location, out size, out anchorLocation);
			PointF pointF = new PointF(location.X + size.Width, location.Y + size.Height);
			RectangleF rect = new RectangleF(location, new SizeF(pointF.X - location.X, pointF.Y - location.Y));
			RectangleF rectangleF = new RectangleF(rect.Location, rect.Size);
			if (rectangleF.Width < 0f)
			{
				rectangleF.X = rectangleF.Right;
				rectangleF.Width = 0f - rectangleF.Width;
			}
			if (rectangleF.Height < 0f)
			{
				rectangleF.Y = rectangleF.Bottom;
				rectangleF.Height = 0f - rectangleF.Height;
			}
			if (rectangleF.IsEmpty || float.IsNaN(rectangleF.X) || float.IsNaN(rectangleF.Y) || float.IsNaN(rectangleF.Right) || float.IsNaN(rectangleF.Bottom))
			{
				return;
			}
			if (Chart.chartPicture.common.ProcessModePaint)
			{
				DrawText(graphics, rectangleF, noSpacingForCenteredText: false, getTextPosition: false);
			}
			if (Chart.chartPicture.common.ProcessModeRegions)
			{
				if (isEllipse)
				{
					GraphicsPath graphicsPath = new GraphicsPath();
					graphicsPath.AddEllipse(rectangleF);
					Chart.chartPicture.common.HotRegionsList.AddHotRegion(graphics, graphicsPath, relativePath: true, ReplaceKeywords(ToolTip), ReplaceKeywords(Href), ReplaceKeywords(MapAreaAttributes), this, ChartElementType.Annotation);
				}
				else
				{
					Chart.chartPicture.common.HotRegionsList.AddHotRegion(graphics, rectangleF, ReplaceKeywords(ToolTip), ReplaceKeywords(Href), ReplaceKeywords(MapAreaAttributes), this, ChartElementType.Annotation, string.Empty);
				}
			}
			PaintSelectionHandles(graphics, rect, null);
		}

		internal RectangleF DrawText(ChartGraphics graphics, RectangleF textPosition, bool noSpacingForCenteredText, bool getTextPosition)
		{
			RectangleF result = RectangleF.Empty;
			bool annotationRelative = false;
			RectangleF textSpacing = GetTextSpacing(out annotationRelative);
			float num = 1f;
			float num2 = 1f;
			if (annotationRelative)
			{
				if (textPosition.Width > 25f)
				{
					num = textPosition.Width / 50f;
					num = Math.Max(1f, num);
				}
				if (textPosition.Height > 25f)
				{
					num2 = textPosition.Height / 50f;
					num2 = Math.Max(1f, num2);
				}
			}
			RectangleF rectangleF = new RectangleF(textPosition.Location, textPosition.Size);
			rectangleF.Width -= (textSpacing.Width + textSpacing.X) * num;
			rectangleF.X += textSpacing.X * num;
			rectangleF.Height -= (textSpacing.Height + textSpacing.Y) * num2;
			rectangleF.Y += textSpacing.Y * num2;
			string text = ReplaceKeywords(Text.Replace("\\n", "\n"));
			if (noSpacingForCenteredText && text.IndexOf('\n') == -1)
			{
				if (Alignment == ContentAlignment.MiddleCenter || Alignment == ContentAlignment.MiddleLeft || Alignment == ContentAlignment.MiddleRight)
				{
					rectangleF.Y = textPosition.Y;
					rectangleF.Height = textPosition.Height;
					rectangleF.Height -= textSpacing.Height / 2f + textSpacing.Y / 2f;
					rectangleF.Y += textSpacing.Y / 2f;
				}
				if (Alignment == ContentAlignment.BottomCenter || Alignment == ContentAlignment.MiddleCenter || Alignment == ContentAlignment.TopCenter)
				{
					rectangleF.X = textPosition.X;
					rectangleF.Width = textPosition.Width;
					rectangleF.Width -= textSpacing.Width / 2f + textSpacing.X / 2f;
					rectangleF.X += textSpacing.X / 2f;
				}
			}
			using (Brush brush = new SolidBrush(TextColor))
			{
				StringFormat stringFormat = new StringFormat(StringFormat.GenericTypographic);
				stringFormat.FormatFlags ^= StringFormatFlags.LineLimit;
				stringFormat.Trimming = StringTrimming.EllipsisCharacter;
				if (Alignment == ContentAlignment.BottomRight || Alignment == ContentAlignment.MiddleRight || Alignment == ContentAlignment.TopRight)
				{
					stringFormat.Alignment = StringAlignment.Far;
				}
				if (Alignment == ContentAlignment.BottomCenter || Alignment == ContentAlignment.MiddleCenter || Alignment == ContentAlignment.TopCenter)
				{
					stringFormat.Alignment = StringAlignment.Center;
				}
				if (Alignment == ContentAlignment.BottomCenter || Alignment == ContentAlignment.BottomLeft || Alignment == ContentAlignment.BottomRight)
				{
					stringFormat.LineAlignment = StringAlignment.Far;
				}
				if (Alignment == ContentAlignment.MiddleCenter || Alignment == ContentAlignment.MiddleLeft || Alignment == ContentAlignment.MiddleRight)
				{
					stringFormat.LineAlignment = StringAlignment.Center;
				}
				Color color = ChartGraphics.GetGradientColor(TextColor, Color.Black, 0.8);
				int num3 = 1;
				TextStyle textStyle = TextStyle;
				if (textStyle == TextStyle.Shadow && ShadowOffset != 0)
				{
					color = ShadowColor;
					num3 = ShadowOffset;
				}
				if (getTextPosition)
				{
					SizeF size = graphics.MeasureStringRel(ReplaceKeywords(this.text.Replace("\\n", "\n")), TextFont, rectangleF.Size, stringFormat);
					result = new RectangleF(rectangleF.Location, size);
					if (Alignment == ContentAlignment.BottomRight || Alignment == ContentAlignment.MiddleRight || Alignment == ContentAlignment.TopRight)
					{
						result.X += rectangleF.Width - size.Width;
					}
					if (Alignment == ContentAlignment.BottomCenter || Alignment == ContentAlignment.MiddleCenter || Alignment == ContentAlignment.TopCenter)
					{
						result.X += (rectangleF.Width - size.Width) / 2f;
					}
					if (Alignment == ContentAlignment.BottomCenter || Alignment == ContentAlignment.BottomLeft || Alignment == ContentAlignment.BottomRight)
					{
						result.Y += rectangleF.Height - size.Height;
					}
					if (Alignment == ContentAlignment.MiddleCenter || Alignment == ContentAlignment.MiddleLeft || Alignment == ContentAlignment.MiddleRight)
					{
						result.Y += (rectangleF.Height - size.Height) / 2f;
					}
					result.Intersect(rectangleF);
				}
				RectangleF absoluteRectangle = graphics.GetAbsoluteRectangle(rectangleF);
				switch (textStyle)
				{
				case TextStyle.Default:
					graphics.DrawStringRel(text, TextFont, brush, rectangleF, stringFormat);
					return result;
				case TextStyle.Frame:
				{
					using (GraphicsPath graphicsPath = new GraphicsPath())
					{
						graphicsPath.AddString(text, TextFont.FontFamily, (int)TextFont.Style, TextFont.Size * 1.3f, absoluteRectangle, stringFormat);
						graphicsPath.CloseAllFigures();
						graphics.DrawPath(new Pen(TextColor, 1f), graphicsPath);
						return result;
					}
				}
				case TextStyle.Embed:
				{
					RectangleF layoutRectangle3 = new RectangleF(absoluteRectangle.Location, absoluteRectangle.Size);
					layoutRectangle3.X -= 1f;
					layoutRectangle3.Y -= 1f;
					graphics.DrawString(text, TextFont, brush, layoutRectangle3, stringFormat);
					layoutRectangle3.X += 2f;
					layoutRectangle3.Y += 2f;
					Color gradientColor2 = ChartGraphics.GetGradientColor(Color.White, TextColor, 0.3);
					graphics.DrawString(text, TextFont, new SolidBrush(gradientColor2), layoutRectangle3, stringFormat);
					graphics.DrawString(text, TextFont, brush, absoluteRectangle, stringFormat);
					return result;
				}
				case TextStyle.Emboss:
				{
					RectangleF layoutRectangle2 = new RectangleF(absoluteRectangle.Location, absoluteRectangle.Size);
					layoutRectangle2.X += 1f;
					layoutRectangle2.Y += 1f;
					graphics.DrawString(text, TextFont, new SolidBrush(color), layoutRectangle2, stringFormat);
					layoutRectangle2.X -= 2f;
					layoutRectangle2.Y -= 2f;
					Color gradientColor = ChartGraphics.GetGradientColor(Color.White, TextColor, 0.3);
					graphics.DrawString(text, TextFont, new SolidBrush(gradientColor), layoutRectangle2, stringFormat);
					graphics.DrawString(text, TextFont, brush, absoluteRectangle, stringFormat);
					return result;
				}
				case TextStyle.Shadow:
				{
					RectangleF layoutRectangle = new RectangleF(absoluteRectangle.Location, absoluteRectangle.Size);
					layoutRectangle.X += num3;
					layoutRectangle.Y += num3;
					graphics.DrawString(text, TextFont, new SolidBrush(color), layoutRectangle, stringFormat);
					graphics.DrawString(text, TextFont, brush, absoluteRectangle, stringFormat);
					return result;
				}
				default:
					throw new InvalidOperationException(SR.ExceptionAnnotationTextDrawingStyleUnknown);
				}
			}
		}

		internal override RectangleF GetContentPosition()
		{
			if (!contentSize.IsEmpty)
			{
				return new RectangleF(float.NaN, float.NaN, contentSize.Width, contentSize.Height);
			}
			Graphics graphics = null;
			Image image = null;
			ChartGraphics chartGraphics = null;
			if (GetGraphics() == null && chart != null && chart.chartPicture != null && chart.chartPicture.common != null)
			{
				image = new Bitmap(chart.chartPicture.Width, chart.chartPicture.Height);
				graphics = Graphics.FromImage(image);
				chartGraphics = new ChartGraphics(chart.chartPicture.common);
				chartGraphics.Graphics = graphics;
				chartGraphics.SetPictureSize(chart.chartPicture.Width, chart.chartPicture.Height);
				chart.chartPicture.common.graph = chartGraphics;
			}
			RectangleF result = RectangleF.Empty;
			if (GetGraphics() != null && Text.Trim().Length > 0)
			{
				StringFormat stringFormat = new StringFormat(StringFormat.GenericTypographic);
				contentSize = GetGraphics().MeasureString("W" + ReplaceKeywords(Text.Replace("\\n", "\n")), TextFont, new SizeF(2000f, 2000f), stringFormat);
				contentSize.Height *= 1.04f;
				contentSize = GetGraphics().GetRelativeSize(contentSize);
				bool annotationRelative = false;
				RectangleF textSpacing = GetTextSpacing(out annotationRelative);
				float num = 1f;
				float num2 = 1f;
				if (annotationRelative)
				{
					if (contentSize.Width > 25f)
					{
						num = contentSize.Width / 25f;
						num = Math.Max(1f, num);
					}
					if (contentSize.Height > 25f)
					{
						num2 = contentSize.Height / 25f;
						num2 = Math.Max(1f, num2);
					}
				}
				contentSize.Width += (textSpacing.X + textSpacing.Width) * num;
				contentSize.Height += (textSpacing.Y + textSpacing.Height) * num2;
				result = new RectangleF(float.NaN, float.NaN, contentSize.Width, contentSize.Height);
			}
			if (chartGraphics != null)
			{
				chartGraphics.Dispose();
				graphics.Dispose();
				image.Dispose();
				chart.chartPicture.common.graph = null;
			}
			return result;
		}

		internal virtual RectangleF GetTextSpacing(out bool annotationRelative)
		{
			annotationRelative = false;
			RectangleF rectangleF = new RectangleF(3f, 3f, 3f, 3f);
			if (GetGraphics() != null)
			{
				return GetGraphics().GetRelativeRectangle(rectangleF);
			}
			return rectangleF;
		}
	}
}
