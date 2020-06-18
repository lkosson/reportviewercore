using Microsoft.Reporting.Chart.WebForms.Design;
using Microsoft.Reporting.Chart.WebForms.Utilities;
using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Microsoft.Reporting.Chart.WebForms
{
	[SRDescription("DescriptionAttributeStripLine_StripLine")]
	[DefaultProperty("IntervalOffset")]
	internal class StripLine : IMapAreaAttributes
	{
		internal Axis axis;

		private double intervalOffset;

		private double interval;

		private DateTimeIntervalType intervalType;

		internal DateTimeIntervalType intervalOffsetType;

		internal bool interlaced;

		private double stripWidth;

		private DateTimeIntervalType stripWidthType;

		private Color backColor = Color.Empty;

		private ChartHatchStyle backHatchStyle;

		private string backImage = "";

		private ChartImageWrapMode backImageMode;

		private Color backImageTranspColor = Color.Empty;

		private ChartImageAlign backImageAlign;

		private GradientType backGradientType;

		private Color backGradientEndColor = Color.Empty;

		private Color borderColor = Color.Black;

		private int borderWidth = 1;

		private ChartDashStyle borderStyle = ChartDashStyle.Solid;

		private string title = "";

		private Color titleColor = Color.Black;

		private Font titleFont = new Font(ChartPicture.GetDefaultFontFamilyName(), 8f);

		private StringAlignment titleAlignment = StringAlignment.Far;

		private StringAlignment titleLineAlignment;

		private string toolTip = "";

		private string href = "";

		private string attributes = "";

		private object mapAreaTag;

		private TextOrientation textOrientation;

		private bool IsTextVertical
		{
			get
			{
				TextOrientation textOrientation = GetTextOrientation();
				if (textOrientation != TextOrientation.Rotated90)
				{
					return textOrientation == TextOrientation.Rotated270;
				}
				return true;
			}
		}

		[SRCategory("CategoryAttributeAppearance")]
		[Bindable(true)]
		[DefaultValue(TextOrientation.Auto)]
		[SRDescription("DescriptionAttribute_TextOrientation")]
		[NotifyParentProperty(true)]
		public TextOrientation TextOrientation
		{
			get
			{
				return textOrientation;
			}
			set
			{
				textOrientation = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryAttributeData")]
		[Bindable(true)]
		[DefaultValue(0.0)]
		[SRDescription("DescriptionAttributeStripLine_IntervalOffset")]
		[TypeConverter(typeof(AxisLabelDateValueConverter))]
		public double IntervalOffset
		{
			get
			{
				return intervalOffset;
			}
			set
			{
				intervalOffset = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryAttributeData")]
		[Bindable(true)]
		[DefaultValue(DateTimeIntervalType.Auto)]
		[SRDescription("DescriptionAttributeStripLine_IntervalOffsetType")]
		[RefreshProperties(RefreshProperties.All)]
		public DateTimeIntervalType IntervalOffsetType
		{
			get
			{
				return intervalOffsetType;
			}
			set
			{
				intervalOffsetType = ((value != DateTimeIntervalType.NotSet) ? value : DateTimeIntervalType.Auto);
				Invalidate();
			}
		}

		[SRCategory("CategoryAttributeData")]
		[Bindable(true)]
		[DefaultValue(0.0)]
		[RefreshProperties(RefreshProperties.All)]
		[SRDescription("DescriptionAttributeStripLine_Interval")]
		public double Interval
		{
			get
			{
				return interval;
			}
			set
			{
				interval = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryAttributeData")]
		[Bindable(true)]
		[DefaultValue(DateTimeIntervalType.Auto)]
		[SRDescription("DescriptionAttributeStripLine_IntervalType")]
		[RefreshProperties(RefreshProperties.All)]
		public DateTimeIntervalType IntervalType
		{
			get
			{
				return intervalType;
			}
			set
			{
				intervalType = ((value != DateTimeIntervalType.NotSet) ? value : DateTimeIntervalType.Auto);
				Invalidate();
			}
		}

		[SRCategory("CategoryAttributeData")]
		[Bindable(true)]
		[DefaultValue(0.0)]
		[SRDescription("DescriptionAttributeStripLine_StripWidth")]
		public double StripWidth
		{
			get
			{
				return stripWidth;
			}
			set
			{
				if (value < 0.0)
				{
					throw new ArgumentException(SR.ExceptionStripLineWidthIsNegative, "value");
				}
				stripWidth = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryAttributeData")]
		[Bindable(true)]
		[DefaultValue(DateTimeIntervalType.Auto)]
		[SRDescription("DescriptionAttributeStripLine_StripWidthType")]
		[RefreshProperties(RefreshProperties.All)]
		public DateTimeIntervalType StripWidthType
		{
			get
			{
				return stripWidthType;
			}
			set
			{
				stripWidthType = ((value != DateTimeIntervalType.NotSet) ? value : DateTimeIntervalType.Auto);
				Invalidate();
			}
		}

		[SRCategory("CategoryAttributeAppearance")]
		[Bindable(true)]
		[DefaultValue(typeof(Color), "")]
		[SRDescription("DescriptionAttributeStripLine_BackColor")]
		public Color BackColor
		{
			get
			{
				return backColor;
			}
			set
			{
				backColor = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryAttributeAppearance")]
		[Bindable(true)]
		[DefaultValue(typeof(Color), "Black")]
		[SRDescription("DescriptionAttributeStripLine_BorderColor")]
		public Color BorderColor
		{
			get
			{
				return borderColor;
			}
			set
			{
				borderColor = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryAttributeAppearance")]
		[Bindable(true)]
		[DefaultValue(ChartDashStyle.Solid)]
		[SRDescription("DescriptionAttributeStripLine_BorderStyle")]
		public ChartDashStyle BorderStyle
		{
			get
			{
				return borderStyle;
			}
			set
			{
				borderStyle = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryAttributeAppearance")]
		[Bindable(true)]
		[DefaultValue(1)]
		[SRDescription("DescriptionAttributeStripLine_BorderWidth")]
		public int BorderWidth
		{
			get
			{
				return borderWidth;
			}
			set
			{
				borderWidth = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryAttributeAppearance")]
		[Bindable(true)]
		[DefaultValue("")]
		[SRDescription("DescriptionAttributeBackImage18")]
		[NotifyParentProperty(true)]
		public string BackImage
		{
			get
			{
				return backImage;
			}
			set
			{
				backImage = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryAttributeAppearance")]
		[Bindable(true)]
		[DefaultValue(ChartImageWrapMode.Tile)]
		[NotifyParentProperty(true)]
		[SRDescription("DescriptionAttributeStripLine_BackImageMode")]
		public ChartImageWrapMode BackImageMode
		{
			get
			{
				return backImageMode;
			}
			set
			{
				backImageMode = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryAttributeAppearance")]
		[Bindable(true)]
		[DefaultValue(typeof(Color), "")]
		[NotifyParentProperty(true)]
		[SRDescription("DescriptionAttributeBackImageTransparentColor")]
		public Color BackImageTransparentColor
		{
			get
			{
				return backImageTranspColor;
			}
			set
			{
				backImageTranspColor = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryAttributeAppearance")]
		[Bindable(true)]
		[DefaultValue(ChartImageAlign.TopLeft)]
		[NotifyParentProperty(true)]
		[SRDescription("DescriptionAttributeBackImageAlign")]
		public ChartImageAlign BackImageAlign
		{
			get
			{
				return backImageAlign;
			}
			set
			{
				backImageAlign = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryAttributeAppearance")]
		[Bindable(true)]
		[DefaultValue(GradientType.None)]
		[SRDescription("DescriptionAttributeStripLine_BackGradientType")]
		public GradientType BackGradientType
		{
			get
			{
				return backGradientType;
			}
			set
			{
				backGradientType = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryAttributeAppearance")]
		[Bindable(true)]
		[DefaultValue(typeof(Color), "")]
		[SRDescription("DescriptionAttributeStripLine_BackGradientEndColor")]
		public Color BackGradientEndColor
		{
			get
			{
				return backGradientEndColor;
			}
			set
			{
				backGradientEndColor = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryAttributeAppearance")]
		[Bindable(true)]
		[DefaultValue(ChartHatchStyle.None)]
		[SRDescription("DescriptionAttributeBackHatchStyle9")]
		public ChartHatchStyle BackHatchStyle
		{
			get
			{
				return backHatchStyle;
			}
			set
			{
				backHatchStyle = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryAttributeAppearance")]
		[Bindable(false)]
		[Browsable(false)]
		[DefaultValue("StripLine")]
		[SRDescription("DescriptionAttributeStripLine_Name")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[SerializationVisibility(SerializationVisibility.Hidden)]
		public string Name => "StripLine";

		[SRCategory("CategoryAttributeTitle")]
		[Bindable(true)]
		[DefaultValue("")]
		[SRDescription("DescriptionAttributeStripLine_Title")]
		[NotifyParentProperty(true)]
		public string Title
		{
			get
			{
				return title;
			}
			set
			{
				title = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryAttributeTitle")]
		[Bindable(true)]
		[DefaultValue(typeof(Color), "Black")]
		[SRDescription("DescriptionAttributeStripLine_TitleColor")]
		[NotifyParentProperty(true)]
		public Color TitleColor
		{
			get
			{
				return titleColor;
			}
			set
			{
				titleColor = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryAttributeTitle")]
		[Bindable(true)]
		[DefaultValue(typeof(StringAlignment), "Far")]
		[SRDescription("DescriptionAttributeStripLine_TitleAlignment")]
		[NotifyParentProperty(true)]
		public StringAlignment TitleAlignment
		{
			get
			{
				return titleAlignment;
			}
			set
			{
				titleAlignment = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryAttributeTitle")]
		[Bindable(true)]
		[DefaultValue(typeof(StringAlignment), "Near")]
		[SRDescription("DescriptionAttributeStripLine_TitleLineAlignment")]
		[NotifyParentProperty(true)]
		public StringAlignment TitleLineAlignment
		{
			get
			{
				return titleLineAlignment;
			}
			set
			{
				titleLineAlignment = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryAttributeTitle")]
		[Bindable(true)]
		[DefaultValue(typeof(Font), "Microsoft Sans Serif, 8pt")]
		[SRDescription("DescriptionAttributeStripLine_TitleFont")]
		[NotifyParentProperty(true)]
		public Font TitleFont
		{
			get
			{
				return titleFont;
			}
			set
			{
				titleFont = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryAttributeMapArea")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[Browsable(false)]
		[Bindable(true)]
		[SRDescription("DescriptionAttributeStripLine_ToolTip")]
		[DefaultValue("")]
		public string ToolTip
		{
			get
			{
				return toolTip;
			}
			set
			{
				Invalidate();
				toolTip = value;
			}
		}

		[SRCategory("CategoryAttributeMapArea")]
		[Bindable(true)]
		[SRDescription("DescriptionAttributeStripLine_Href")]
		[DefaultValue("")]
		public string Href
		{
			get
			{
				return href;
			}
			set
			{
				href = value;
				Invalidate();
			}
		}

		object IMapAreaAttributes.Tag
		{
			get
			{
				return mapAreaTag;
			}
			set
			{
				mapAreaTag = value;
			}
		}

		[SRCategory("CategoryAttributeMapArea")]
		[Bindable(true)]
		[SRDescription("DescriptionAttributeStripLine_MapAreaAttributes")]
		[DefaultValue("")]
		public string MapAreaAttributes
		{
			get
			{
				return attributes;
			}
			set
			{
				attributes = value;
				Invalidate();
			}
		}

		internal Axis GetAxis()
		{
			return axis;
		}

		private TextOrientation GetTextOrientation()
		{
			if (TextOrientation == TextOrientation.Auto && axis != null)
			{
				if (axis.AxisPosition == AxisPosition.Bottom || axis.AxisPosition == AxisPosition.Top)
				{
					return TextOrientation.Rotated270;
				}
				return TextOrientation.Horizontal;
			}
			return TextOrientation;
		}

		internal void Paint(ChartGraphics graph, CommonElements common, bool drawLinesOnly)
		{
			if (axis.chartArea.chartAreaIsCurcular)
			{
				return;
			}
			RectangleF rect = axis.chartArea.PlotAreaPosition.ToRectangleF();
			bool flag = true;
			if (axis.AxisPosition == AxisPosition.Bottom || axis.AxisPosition == AxisPosition.Top)
			{
				flag = false;
			}
			Series series = null;
			if (axis.axisType == AxisName.X || axis.axisType == AxisName.X2)
			{
				ArrayList xAxesSeries = axis.chartArea.GetXAxesSeries((axis.axisType != 0) ? AxisType.Secondary : AxisType.Primary, axis.SubAxisName);
				if (xAxesSeries.Count > 0)
				{
					series = axis.Common.DataManager.Series[xAxesSeries[0]];
					if (series != null && !series.XValueIndexed)
					{
						series = null;
					}
				}
			}
			double num = axis.GetViewMinimum();
			if (!axis.chartArea.chartAreaIsCurcular || axis.axisType == AxisName.Y || axis.axisType == AxisName.Y2)
			{
				double num2 = Interval;
				if (interlaced)
				{
					num2 /= 2.0;
				}
				num = axis.AlignIntervalStart(num, num2, IntervalType, series);
			}
			if (Interval != 0.0 && (axis.GetViewMaximum() - axis.GetViewMinimum()) / axis.GetIntervalSize(num, interval, intervalType, series, 0.0, DateTimeIntervalType.Number, forceIntIndex: false) > 10000.0)
			{
				return;
			}
			DateTimeIntervalType type = (IntervalOffsetType == DateTimeIntervalType.Auto) ? IntervalType : IntervalOffsetType;
			if (Interval == 0.0)
			{
				num = IntervalOffset;
			}
			else if (IntervalOffset > 0.0)
			{
				num += axis.GetIntervalSize(num, IntervalOffset, type, series, 0.0, DateTimeIntervalType.Number, forceIntIndex: false);
			}
			else if (IntervalOffset < 0.0)
			{
				num -= axis.GetIntervalSize(num, 0.0 - IntervalOffset, type, series, 0.0, DateTimeIntervalType.Number, forceIntIndex: false);
			}
			int num3 = 0;
			while (num3++ <= 10000)
			{
				if (StripWidth > 0.0 && !drawLinesOnly)
				{
					double num4 = num + axis.GetIntervalSize(num, StripWidth, StripWidthType, series, IntervalOffset, type, forceIntIndex: false);
					if (num4 > axis.GetViewMinimum() && num < axis.GetViewMaximum())
					{
						RectangleF empty = RectangleF.Empty;
						double val = (float)axis.GetLinearPosition(num);
						double val2 = (float)axis.GetLinearPosition(num4);
						if (flag)
						{
							empty.X = rect.X;
							empty.Width = rect.Width;
							empty.Y = (float)Math.Min(val, val2);
							empty.Height = (float)Math.Max(val, val2) - empty.Y;
							empty.Intersect(rect);
						}
						else
						{
							empty.Y = rect.Y;
							empty.Height = rect.Height;
							empty.X = (float)Math.Min(val, val2);
							empty.Width = (float)Math.Max(val, val2) - empty.X;
							empty.Intersect(rect);
						}
						if (empty.Width > 0f && empty.Height > 0f)
						{
							graph.StartHotRegion(href, toolTip);
							if (!axis.chartArea.Area3DStyle.Enable3D)
							{
								graph.StartAnimation();
								graph.FillRectangleRel(empty, BackColor, BackHatchStyle, BackImage, BackImageMode, BackImageTransparentColor, BackImageAlign, BackGradientType, BackGradientEndColor, BorderColor, BorderWidth, BorderStyle, Color.Empty, 0, PenAlignment.Inset);
								graph.StopAnimation();
							}
							else
							{
								Draw3DStrip(graph, empty, flag);
							}
							graph.EndHotRegion();
							PaintTitle(graph, empty);
							if (common.ProcessModeRegions && !axis.chartArea.Area3DStyle.Enable3D)
							{
								common.HotRegionsList.AddHotRegion(graph, empty, ToolTip, Href, MapAreaAttributes, this, ChartElementType.StripLines, string.Empty);
							}
						}
					}
				}
				else if (StripWidth == 0.0 && drawLinesOnly && num > axis.GetViewMinimum() && num < axis.GetViewMaximum())
				{
					PointF empty2 = PointF.Empty;
					PointF empty3 = PointF.Empty;
					if (flag)
					{
						empty2.X = rect.X;
						empty2.Y = (float)axis.GetLinearPosition(num);
						empty3.X = rect.Right;
						empty3.Y = empty2.Y;
					}
					else
					{
						empty2.X = (float)axis.GetLinearPosition(num);
						empty2.Y = rect.Y;
						empty3.X = empty2.X;
						empty3.Y = rect.Bottom;
					}
					graph.StartHotRegion(href, toolTip);
					if (!axis.chartArea.Area3DStyle.Enable3D)
					{
						graph.DrawLineRel(BorderColor, BorderWidth, BorderStyle, empty2, empty3);
					}
					else
					{
						graph.Draw3DGridLine(axis.chartArea, borderColor, borderWidth, borderStyle, empty2, empty3, flag, axis.Common, this);
					}
					graph.EndHotRegion();
					PaintTitle(graph, empty2, empty3);
					if (common.ProcessModeRegions)
					{
						SizeF size = new SizeF(BorderWidth + 1, BorderWidth + 1);
						size = graph.GetRelativeSize(size);
						RectangleF empty4 = RectangleF.Empty;
						if (flag)
						{
							empty4.X = empty2.X;
							empty4.Y = empty2.Y - size.Height / 2f;
							empty4.Width = empty3.X - empty2.X;
							empty4.Height = size.Height;
						}
						else
						{
							empty4.X = empty2.X - size.Width / 2f;
							empty4.Y = empty2.Y;
							empty4.Width = size.Width;
							empty4.Height = empty3.Y - empty2.Y;
						}
						common.HotRegionsList.AddHotRegion(graph, empty4, ToolTip, Href, MapAreaAttributes, this, ChartElementType.StripLines, string.Empty);
					}
				}
				if (Interval > 0.0)
				{
					num += axis.GetIntervalSize(num, Interval, IntervalType, series, IntervalOffset, type, forceIntIndex: false);
				}
				if (!(Interval > 0.0) || !(num <= axis.GetViewMaximum()))
				{
					break;
				}
			}
		}

		private void Draw3DStrip(ChartGraphics graph, RectangleF rect, bool horizontal)
		{
			ChartArea chartArea = axis.chartArea;
			GraphicsPath graphicsPath = null;
			DrawingOperationTypes drawingOperationTypes = DrawingOperationTypes.DrawElement;
			if (axis.Common.ProcessModeRegions)
			{
				drawingOperationTypes |= DrawingOperationTypes.CalcElementPath;
			}
			InitAnimation3D(graph.common, rect, chartArea.IsMainSceneWallOnFront() ? chartArea.areaSceneDepth : 0f, 0f, chartArea.matrix3D, graph, axis);
			graph.StartAnimation();
			graphicsPath = graph.Fill3DRectangle(rect, chartArea.IsMainSceneWallOnFront() ? chartArea.areaSceneDepth : 0f, 0f, chartArea.matrix3D, chartArea.Area3DStyle.Light, BackColor, BackHatchStyle, BackImage, BackImageMode, BackImageTransparentColor, BackImageAlign, BackGradientType, BackGradientEndColor, BorderColor, BorderWidth, BorderStyle, PenAlignment.Outset, drawingOperationTypes);
			graph.StopAnimation();
			if (axis.Common.ProcessModeRegions)
			{
				axis.Common.HotRegionsList.AddHotRegion(graph, graphicsPath, relativePath: false, ToolTip, Href, MapAreaAttributes, this, ChartElementType.StripLines);
			}
			if (horizontal)
			{
				if (!chartArea.IsSideSceneWallOnLeft())
				{
					rect.X = rect.Right;
				}
				rect.Width = 0f;
				InitAnimation3D(graph.common, rect, 0f, chartArea.areaSceneDepth, chartArea.matrix3D, graph, axis);
				graph.StartAnimation();
				graphicsPath = graph.Fill3DRectangle(rect, 0f, chartArea.areaSceneDepth, chartArea.matrix3D, chartArea.Area3DStyle.Light, BackColor, BackHatchStyle, BackImage, BackImageMode, BackImageTransparentColor, BackImageAlign, BackGradientType, BackGradientEndColor, BorderColor, BorderWidth, BorderStyle, PenAlignment.Outset, drawingOperationTypes);
				graph.StopAnimation();
			}
			else if (chartArea.IsBottomSceneWallVisible())
			{
				rect.Y = rect.Bottom;
				rect.Height = 0f;
				InitAnimation3D(graph.common, rect, 0f, chartArea.areaSceneDepth, chartArea.matrix3D, graph, axis);
				graph.StartAnimation();
				graphicsPath = graph.Fill3DRectangle(rect, 0f, chartArea.areaSceneDepth, chartArea.matrix3D, chartArea.Area3DStyle.Light, BackColor, BackHatchStyle, BackImage, BackImageMode, BackImageTransparentColor, BackImageAlign, BackGradientType, BackGradientEndColor, BorderColor, BorderWidth, BorderStyle, PenAlignment.Outset, drawingOperationTypes);
				graph.StopAnimation();
			}
			if (axis.Common.ProcessModeRegions)
			{
				axis.Common.HotRegionsList.AddHotRegion(graph, graphicsPath, relativePath: false, ToolTip, Href, MapAreaAttributes, this, ChartElementType.StripLines);
			}
		}

		private void InitAnimation3D(CommonElements common, RectangleF position, float positionZ, float depth, Matrix3D matrix, ChartGraphics graph, Axis axis)
		{
		}

		private void PaintTitle(ChartGraphics graph, PointF point1, PointF point2)
		{
			if (Title.Length > 0)
			{
				RectangleF empty = RectangleF.Empty;
				empty.X = point1.X;
				empty.Y = point1.Y;
				empty.Height = point2.Y - empty.Y;
				empty.Width = point2.X - empty.X;
				PaintTitle(graph, empty);
			}
		}

		private void PaintTitle(ChartGraphics graph, RectangleF rect)
		{
			if (Title.Length <= 0)
			{
				return;
			}
			string text = Title;
			if (axis != null && axis.chart != null && axis.chart.LocalizeTextHandler != null)
			{
				text = axis.chart.LocalizeTextHandler(this, text, 0, ChartElementType.StripLines);
			}
			StringFormat stringFormat = new StringFormat();
			stringFormat.Alignment = TitleAlignment;
			stringFormat.LineAlignment = TitleLineAlignment;
			int num = 0;
			switch (TextOrientation)
			{
			case TextOrientation.Rotated90:
				num = 90;
				break;
			case TextOrientation.Rotated270:
				num = 270;
				break;
			case TextOrientation.Auto:
				if (axis.AxisPosition == AxisPosition.Bottom || axis.AxisPosition == AxisPosition.Top)
				{
					num = 270;
				}
				break;
			}
			switch (num)
			{
			case 90:
				stringFormat.FormatFlags = StringFormatFlags.DirectionVertical;
				num = 0;
				break;
			case 270:
				stringFormat.FormatFlags = StringFormatFlags.DirectionVertical;
				num = 180;
				break;
			}
			SizeF sizeF = graph.MeasureStringRel(text.Replace("\\n", "\n"), TitleFont, new SizeF(100f, 100f), stringFormat, GetTextOrientation());
			float z = 0f;
			if (axis.chartArea.Area3DStyle.Enable3D)
			{
				Point3D[] array = new Point3D[3];
				z = (axis.chartArea.IsMainSceneWallOnFront() ? axis.chartArea.areaSceneDepth : 0f);
				array[0] = new Point3D(0f, 0f, z);
				array[1] = new Point3D(sizeF.Width, 0f, z);
				array[2] = new Point3D(0f, sizeF.Height, z);
				axis.chartArea.matrix3D.TransformPoints(array);
				int num2 = (!axis.chartArea.IsMainSceneWallOnFront()) ? 1 : 0;
				sizeF.Width *= sizeF.Width / (array[num2].X - array[(num2 == 0) ? 1 : 0].X);
				sizeF.Height *= sizeF.Height / (array[2].Y - array[0].Y);
			}
			SizeF relativeSize = graph.GetRelativeSize(new SizeF(BorderWidth, BorderWidth));
			PointF position = PointF.Empty;
			if (stringFormat.Alignment == StringAlignment.Near)
			{
				position.X = rect.X + sizeF.Width / 2f + relativeSize.Width;
			}
			else if (stringFormat.Alignment == StringAlignment.Far)
			{
				position.X = rect.Right - sizeF.Width / 2f - relativeSize.Width;
			}
			else
			{
				position.X = (rect.Left + rect.Right) / 2f;
			}
			if (stringFormat.LineAlignment == StringAlignment.Near)
			{
				position.Y = rect.Top + sizeF.Height / 2f + relativeSize.Height;
			}
			else if (stringFormat.LineAlignment == StringAlignment.Far)
			{
				position.Y = rect.Bottom - sizeF.Height / 2f - relativeSize.Height;
			}
			else
			{
				position.Y = (rect.Bottom + rect.Top) / 2f;
			}
			stringFormat.Alignment = StringAlignment.Center;
			stringFormat.LineAlignment = StringAlignment.Center;
			if (axis.chartArea.Area3DStyle.Enable3D)
			{
				Point3D[] array2 = new Point3D[2]
				{
					new Point3D(position.X, position.Y, z),
					null
				};
				if (stringFormat.FormatFlags == StringFormatFlags.DirectionVertical)
				{
					array2[1] = new Point3D(position.X, position.Y - 20f, z);
				}
				else
				{
					array2[1] = new Point3D(position.X - 20f, position.Y, z);
				}
				axis.chartArea.matrix3D.TransformPoints(array2);
				position = array2[0].PointF;
				if (num == 0 || num == 180 || num == 90 || num == 270)
				{
					if (stringFormat.FormatFlags == StringFormatFlags.DirectionVertical)
					{
						num += 90;
					}
					array2[0].PointF = graph.GetAbsolutePoint(array2[0].PointF);
					array2[1].PointF = graph.GetAbsolutePoint(array2[1].PointF);
					float num3 = (float)Math.Atan((array2[1].Y - array2[0].Y) / (array2[1].X - array2[0].X));
					num3 = (float)Math.Round(num3 * 180f / (float)Math.PI);
					num += (int)num3;
				}
			}
			graph.DrawStringRel(text.Replace("\\n", "\n"), TitleFont, new SolidBrush(TitleColor), position, stringFormat, num, GetTextOrientation());
		}

		private void Invalidate()
		{
		}
	}
}
