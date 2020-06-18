using System;
using System.Collections;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Globalization;

namespace Microsoft.Reporting.Map.WebForms
{
	internal class MapGraphics : RenderingEngine
	{
		internal CommonElements common;

		private Pen pen;

		private SolidBrush solidBrush;

		private Matrix myMatrix;

		private int width;

		private int height;

		internal bool softShadows = true;

		private AntiAliasing antiAliasing = AntiAliasing.All;

		internal bool IsMetafile;

		internal PointF InitialOffset = new PointF(0f, 0f);

		private Stack graphicStates = new Stack();

		public new Graphics Graphics
		{
			get
			{
				return base.Graphics;
			}
			set
			{
				if (base.Graphics != value)
				{
					base.Graphics = value;
					if (base.Graphics.Transform != null)
					{
						InitialOffset.X = base.Graphics.Transform.OffsetX;
						InitialOffset.Y = base.Graphics.Transform.OffsetY;
					}
				}
			}
		}

		internal AntiAliasing AntiAliasing
		{
			get
			{
				return antiAliasing;
			}
			set
			{
				antiAliasing = value;
				if (Graphics != null)
				{
					if ((antiAliasing & AntiAliasing.Graphics) == AntiAliasing.Graphics)
					{
						base.SmoothingMode = SmoothingMode.AntiAlias;
					}
					else
					{
						base.SmoothingMode = SmoothingMode.None;
					}
				}
			}
		}

		internal float ScaleFactorX => Graphics.Transform.Elements[0];

		internal float ScaleFactorY => Graphics.Transform.Elements[3];

		internal MapGraphics(CommonElements common)
		{
			if (common != null)
			{
				this.common = common;
				common.Graph = this;
			}
			pen = new Pen(Color.Black);
			solidBrush = new SolidBrush(Color.Black);
		}

		internal void DrawLineRel(Color color, int width, MapDashStyle style, PointF firstPointF, PointF secondPointF)
		{
			DrawLineAbs(color, width, style, GetAbsolutePoint(firstPointF), GetAbsolutePoint(secondPointF));
		}

		internal void DrawLineAbs(Color color, int width, MapDashStyle style, PointF firstPoint, PointF secondPoint)
		{
			if (width != 0 && style != 0)
			{
				if (pen.Color != color)
				{
					pen.Color = color;
				}
				if (pen.Width != (float)width)
				{
					pen.Width = width;
				}
				if (pen.DashStyle != GetPenStyle(style))
				{
					pen.DashStyle = GetPenStyle(style);
				}
				SmoothingMode smoothingMode = base.SmoothingMode;
				if (width <= 1 && style != MapDashStyle.Solid && (firstPoint.X == secondPoint.X || firstPoint.Y == secondPoint.Y))
				{
					base.SmoothingMode = SmoothingMode.Default;
				}
				DrawLine(pen, (float)Math.Round(firstPoint.X), (float)Math.Round(firstPoint.Y), (float)Math.Round(secondPoint.X), (float)Math.Round(secondPoint.Y));
				base.SmoothingMode = smoothingMode;
			}
		}

		internal void DrawLineRel(Color color, int width, MapDashStyle style, PointF firstPoint, PointF secondPoint, Color shadowColor, int shadowOffset)
		{
			DrawLineAbs(color, width, style, GetAbsolutePoint(firstPoint), GetAbsolutePoint(secondPoint), shadowColor, shadowOffset);
		}

		internal void DrawLineAbs(Color color, int width, MapDashStyle style, PointF firstPoint, PointF secondPoint, Color shadowColor, int shadowOffset)
		{
			Color color2 = (shadowColor.A == byte.MaxValue) ? Color.FromArgb((int)color.A / 2, shadowColor) : shadowColor;
			PointF firstPoint2 = new PointF(firstPoint.X + (float)shadowOffset, firstPoint.Y + (float)shadowOffset);
			PointF secondPoint2 = new PointF(secondPoint.X + (float)shadowOffset, secondPoint.Y + (float)shadowOffset);
			shadowDrawingMode = true;
			DrawLineAbs(color2, width, style, firstPoint2, secondPoint2);
			shadowDrawingMode = false;
			DrawLineAbs(color, width, style, firstPoint, secondPoint);
		}

		internal static Brush GetHatchBrush(MapHatchStyle hatchStyle, Color backColor, Color foreColor)
		{
			return new HatchBrush((HatchStyle)Enum.Parse(typeof(HatchStyle), hatchStyle.ToString(CultureInfo.InvariantCulture)), foreColor, backColor);
		}

		internal Brush GetTextureBrush(string name, Color backImageTranspColor, MapImageWrapMode mode)
		{
			Image image = common.ImageLoader.LoadImage(name);
			ImageAttributes imageAttributes = new ImageAttributes();
			imageAttributes.SetWrapMode((WrapMode)((mode == MapImageWrapMode.Unscaled) ? MapImageWrapMode.Scaled : mode));
			if (backImageTranspColor != Color.Empty)
			{
				imageAttributes.SetColorKey(backImageTranspColor, backImageTranspColor, ColorAdjustType.Default);
			}
			return new TextureBrush(image, new RectangleF(0f, 0f, image.Width, image.Height), imageAttributes);
		}

		internal Brush GetShadowBrush()
		{
			return new SolidBrush(GetShadowColor());
		}

		internal Color GetShadowColor()
		{
			return Color.FromArgb((int)(255f * common.MapCore.ShadowIntensity / 100f), Color.Black);
		}

		internal Brush GetGradientBrush(RectangleF rectangle, Color firstColor, Color secondColor, GradientType type)
		{
			rectangle.Inflate(1f, 1f);
			Brush brush = null;
			float angle = 0f;
			if (rectangle.Height == 0f || rectangle.Width == 0f)
			{
				return new SolidBrush(Color.Black);
			}
			switch (type)
			{
			case GradientType.RightLeft:
			{
				type = GradientType.LeftRight;
				Color color7 = firstColor;
				firstColor = secondColor;
				secondColor = color7;
				break;
			}
			case GradientType.BottomTop:
			{
				type = GradientType.TopBottom;
				Color color6 = firstColor;
				firstColor = secondColor;
				secondColor = color6;
				break;
			}
			case GradientType.ReversedCenter:
			{
				type = GradientType.Center;
				Color color5 = firstColor;
				firstColor = secondColor;
				secondColor = color5;
				break;
			}
			case GradientType.ReversedDiagonalLeft:
			{
				type = GradientType.DiagonalLeft;
				Color color4 = firstColor;
				firstColor = secondColor;
				secondColor = color4;
				break;
			}
			case GradientType.ReversedDiagonalRight:
			{
				type = GradientType.DiagonalRight;
				Color color3 = firstColor;
				firstColor = secondColor;
				secondColor = color3;
				break;
			}
			case GradientType.ReversedHorizontalCenter:
			{
				type = GradientType.HorizontalCenter;
				Color color2 = firstColor;
				firstColor = secondColor;
				secondColor = color2;
				break;
			}
			case GradientType.ReversedVerticalCenter:
			{
				type = GradientType.VerticalCenter;
				Color color = firstColor;
				firstColor = secondColor;
				secondColor = color;
				break;
			}
			}
			switch (type)
			{
			case GradientType.LeftRight:
			case GradientType.VerticalCenter:
				angle = 0f;
				break;
			case GradientType.TopBottom:
			case GradientType.HorizontalCenter:
				angle = 90f;
				break;
			case GradientType.DiagonalLeft:
				angle = (float)(Math.Atan(rectangle.Width / rectangle.Height) * 180.0 / Math.PI);
				break;
			case GradientType.DiagonalRight:
				angle = (float)(180.0 - Math.Atan(rectangle.Width / rectangle.Height) * 180.0 / Math.PI);
				break;
			}
			if (type == GradientType.TopBottom || type == GradientType.LeftRight || type == GradientType.DiagonalLeft || type == GradientType.DiagonalRight || type == GradientType.HorizontalCenter || type == GradientType.VerticalCenter)
			{
				LinearGradientBrush linearGradientBrush = null;
				RectangleF rect = new RectangleF(0f, 0f, rectangle.Width, rectangle.Height);
				switch (type)
				{
				case GradientType.HorizontalCenter:
					rect.Height /= 2f;
					linearGradientBrush = new LinearGradientBrush(rect, firstColor, secondColor, angle);
					linearGradientBrush.WrapMode = WrapMode.TileFlipX;
					break;
				case GradientType.VerticalCenter:
					rect.Width /= 2f;
					linearGradientBrush = new LinearGradientBrush(rect, firstColor, secondColor, angle);
					linearGradientBrush.WrapMode = WrapMode.TileFlipX;
					break;
				default:
					linearGradientBrush = new LinearGradientBrush(rect, firstColor, secondColor, angle);
					break;
				}
				linearGradientBrush.TranslateTransform(rectangle.X, rectangle.Y, MatrixOrder.Append);
				return linearGradientBrush;
			}
			GraphicsPath graphicsPath = new GraphicsPath();
			graphicsPath.AddRectangle(rectangle);
			brush = new PathGradientBrush(graphicsPath);
			((PathGradientBrush)brush).CenterColor = firstColor;
			((PathGradientBrush)brush).CenterPoint = new PointF(rectangle.X + rectangle.Width / 2f, rectangle.Y + rectangle.Height / 2f);
			Color[] surroundColors = new Color[1]
			{
				secondColor
			};
			((PathGradientBrush)brush).SurroundColors = surroundColors;
			graphicsPath?.Dispose();
			return brush;
		}

		internal Brush GetPieGradientBrush(RectangleF rectangle, Color firstColor, Color secondColor)
		{
			GraphicsPath graphicsPath = new GraphicsPath();
			graphicsPath.AddEllipse(rectangle);
			PathGradientBrush pathGradientBrush = new PathGradientBrush(graphicsPath);
			pathGradientBrush.CenterColor = firstColor;
			pathGradientBrush.CenterPoint = new PointF(rectangle.X + rectangle.Width / 2f, rectangle.Y + rectangle.Height / 2f);
			Color[] array2 = pathGradientBrush.SurroundColors = new Color[1]
			{
				secondColor
			};
			graphicsPath?.Dispose();
			return pathGradientBrush;
		}

		internal Brush CreateBrush(RectangleF rect, Color backColor, MapHatchStyle backHatchStyle, string backImage, MapImageWrapMode backImageMode, Color backImageTranspColor, MapImageAlign backImageAlign, GradientType backGradientType, Color backSecondaryColor)
		{
			Brush brush = new SolidBrush(backColor);
			if (!string.IsNullOrEmpty(backImage) && backImageMode != MapImageWrapMode.Unscaled && backImageMode != MapImageWrapMode.Scaled)
			{
				return GetTextureBrush(backImage, backImageTranspColor, backImageMode);
			}
			if (backHatchStyle != 0)
			{
				return GetHatchBrush(backHatchStyle, backColor, backSecondaryColor);
			}
			if (backGradientType != 0)
			{
				return GetGradientBrush(rect, backColor, backSecondaryColor, backGradientType);
			}
			return new SolidBrush(backColor);
		}

		internal static DashStyle GetPenStyle(MapDashStyle style)
		{
			switch (style)
			{
			case MapDashStyle.Dash:
				return DashStyle.Dash;
			case MapDashStyle.DashDot:
				return DashStyle.DashDot;
			case MapDashStyle.DashDotDot:
				return DashStyle.DashDotDot;
			case MapDashStyle.Dot:
				return DashStyle.Dot;
			default:
				return DashStyle.Solid;
			}
		}

		internal GraphicsPath WidenPath(GraphicsPath path, float amount)
		{
			GraphicsPath graphicsPath = new GraphicsPath();
			GraphicsPath graphicsPath2 = (GraphicsPath)path.Clone();
			GraphicsPath graphicsPath3 = (GraphicsPath)path.Clone();
			try
			{
				using (Pen pen = new Pen(Color.Empty, amount))
				{
					pen.LineJoin = LineJoin.Round;
					graphicsPath2.Widen(pen, null, 0.3f);
				}
				graphicsPath2.Flatten(null, 0.25f);
				using (Pen pen2 = new Pen(Color.Empty, amount * 0.9f))
				{
					pen2.LineJoin = LineJoin.Round;
					graphicsPath3.Widen(pen2, null, 0.3f);
				}
				graphicsPath3.Flatten(null, 0.25f);
				ArrayList arrayList = new ArrayList();
				PointF[] pathPoints = graphicsPath2.PathPoints;
				foreach (PointF pointF in pathPoints)
				{
					if (!path.IsVisible(pointF) && !graphicsPath3.IsVisible(pointF))
					{
						arrayList.Add(pointF);
					}
				}
				graphicsPath.AddPolygon((PointF[])arrayList.ToArray(typeof(PointF)));
				return graphicsPath;
			}
			finally
			{
				graphicsPath2.Dispose();
				graphicsPath3.Dispose();
			}
		}

		internal GraphicsPath Union(GraphicsPath pathA, GraphicsPath pathB)
		{
			GraphicsPath graphicsPath = new GraphicsPath();
			pathA.Flatten();
			pathB.Flatten();
			int firstPointOf1Inside = GetFirstPointOf1Inside2(pathA, pathB);
			if (firstPointOf1Inside == -1)
			{
				return (GraphicsPath)pathA.Clone();
			}
			int firstPointOf1Inside2 = GetFirstPointOf1Inside2(pathB, pathA);
			if (firstPointOf1Inside2 == -1)
			{
				return (GraphicsPath)pathB.Clone();
			}
			GraphicsPath[] array = SplitIntoSegments(pathA, firstPointOf1Inside, pathB);
			GraphicsPath[] array2 = SplitIntoSegments(pathB, firstPointOf1Inside2, pathA);
			int num = 0;
			int num2 = 0;
			while (num < array.Length && num2 < array2.Length)
			{
				graphicsPath.AddPath(array[num], connect: true);
				graphicsPath.AddPath(array2[num2], connect: true);
			}
			return graphicsPath;
		}

		private GraphicsPath[] SplitIntoSegments(GraphicsPath pathToBeSplit, int startingPoint, GraphicsPath splitterPath)
		{
			if (startingPoint == -1)
			{
				return null;
			}
			ArrayList arrayList = new ArrayList();
			ArrayList arrayList2 = null;
			for (int i = startingPoint + 1; i != startingPoint; i++)
			{
				if (i == pathToBeSplit.PointCount)
				{
					i = 0;
				}
				if (!splitterPath.IsVisible(pathToBeSplit.PathPoints[i]))
				{
					if (arrayList2 == null)
					{
						arrayList2 = new ArrayList();
					}
					arrayList2.Add(pathToBeSplit.PathPoints[i]);
				}
				else if (arrayList2 != null)
				{
					GraphicsPath graphicsPath = new GraphicsPath();
					graphicsPath.AddPolygon((PointF[])arrayList2.ToArray(typeof(PointF)));
					arrayList.Add(graphicsPath);
					arrayList2.RemoveRange(0, arrayList2.Count);
					arrayList2 = null;
				}
			}
			return (GraphicsPath[])arrayList.ToArray(typeof(GraphicsPath));
		}

		private int GetFirstPointOf1Inside2(GraphicsPath path1, GraphicsPath path2)
		{
			for (int i = 0; i < path1.PathPoints.Length; i++)
			{
				if (path2.IsVisible(path1.PathPoints[i]))
				{
					return i;
				}
			}
			return -1;
		}

		internal Brush GetMarkerBrush(GraphicsPath path, MarkerStyle markerStyle, PointF pointOrigin, float angle, Color fillColor, GradientType fillGradientType, Color fillSecondaryColor, MapHatchStyle fillHatchStyle)
		{
			Brush brush = null;
			if (fillHatchStyle != 0)
			{
				brush = GetHatchBrush(fillHatchStyle, fillSecondaryColor, fillColor);
			}
			else if (fillGradientType != 0)
			{
				RectangleF bounds = path.GetBounds();
				if (markerStyle == MarkerStyle.Circle && fillGradientType == GradientType.DiagonalLeft)
				{
					brush = GetGradientBrush(bounds, fillColor, fillSecondaryColor, GradientType.LeftRight);
					Matrix matrix = new Matrix();
					matrix.RotateAt(45f, new PointF(bounds.X + bounds.Width / 2f, bounds.Y + bounds.Height / 2f));
					((LinearGradientBrush)brush).Transform = matrix;
				}
				else if (markerStyle == MarkerStyle.Circle && fillGradientType == GradientType.DiagonalRight)
				{
					brush = GetGradientBrush(bounds, fillColor, fillSecondaryColor, GradientType.TopBottom);
					Matrix matrix2 = new Matrix();
					matrix2.RotateAt(135f, new PointF(bounds.X + bounds.Width / 2f, bounds.Y + bounds.Height / 2f));
					((LinearGradientBrush)brush).Transform = matrix2;
				}
				else if (markerStyle == MarkerStyle.Circle && fillGradientType == GradientType.Center)
				{
					bounds.Inflate(1f, 1f);
					using (GraphicsPath graphicsPath = new GraphicsPath())
					{
						graphicsPath.AddArc(bounds, 0f, 360f);
						PathGradientBrush pathGradientBrush = new PathGradientBrush(graphicsPath);
						pathGradientBrush.CenterColor = fillColor;
						pathGradientBrush.CenterPoint = new PointF(bounds.X + bounds.Width / 2f, bounds.Y + bounds.Height / 2f);
						pathGradientBrush.SurroundColors = new Color[1]
						{
							fillSecondaryColor
						};
						brush = pathGradientBrush;
					}
				}
				else
				{
					brush = GetGradientBrush(path.GetBounds(), fillColor, fillSecondaryColor, fillGradientType);
				}
				if (brush is LinearGradientBrush)
				{
					((LinearGradientBrush)brush).RotateTransform(angle, MatrixOrder.Append);
					((LinearGradientBrush)brush).TranslateTransform(pointOrigin.X, pointOrigin.Y, MatrixOrder.Append);
				}
				else if (brush is PathGradientBrush)
				{
					((PathGradientBrush)brush).RotateTransform(angle, MatrixOrder.Append);
					((PathGradientBrush)brush).TranslateTransform(pointOrigin.X, pointOrigin.Y, MatrixOrder.Append);
				}
			}
			else
			{
				brush = new SolidBrush(fillColor);
			}
			return brush;
		}

		internal GraphicsPath CreateMarker(PointF point, float markerWidth, float markerHeight, MarkerStyle markerStyle)
		{
			GraphicsPath graphicsPath = new GraphicsPath();
			RectangleF empty = RectangleF.Empty;
			empty.X = point.X - markerWidth / 2f;
			empty.Y = point.Y - markerHeight / 2f;
			empty.Width = markerWidth;
			empty.Height = markerHeight;
			switch (markerStyle)
			{
			case MarkerStyle.Circle:
				graphicsPath.AddEllipse(empty);
				break;
			case MarkerStyle.Diamond:
			{
				PointF[] array = new PointF[4];
				array[0].X = empty.X;
				array[0].Y = empty.Y + empty.Height / 2f;
				array[1].X = empty.X + empty.Width / 2f;
				array[1].Y = empty.Top;
				array[2].X = empty.Right;
				array[2].Y = empty.Y + empty.Height / 2f;
				array[3].X = empty.X + empty.Width / 2f;
				array[3].Y = empty.Bottom;
				graphicsPath.AddPolygon(array);
				break;
			}
			case MarkerStyle.Star:
				graphicsPath.AddPolygon(CreateStarPolygon(empty, 5));
				break;
			case MarkerStyle.None:
			{
				PointF[] array = new PointF[4];
				array[0].X = point.X;
				array[0].Y = point.Y;
				array[1].X = point.X + 1f;
				array[1].Y = point.Y;
				array[2].X = point.X + 1f;
				array[2].Y = point.Y + 1f;
				array[3].X = point.X;
				array[3].Y = point.Y + 1f;
				graphicsPath.AddPolygon(array);
				break;
			}
			case MarkerStyle.Rectangle:
			{
				PointF[] array = new PointF[4];
				array[0].X = empty.X;
				array[0].Y = empty.Y;
				array[1].X = empty.X + empty.Width;
				array[1].Y = empty.Y;
				array[2].X = empty.X + empty.Width;
				array[2].Y = empty.Y + empty.Height;
				array[3].X = empty.X;
				array[3].Y = empty.Y + empty.Height;
				graphicsPath.AddPolygon(array);
				break;
			}
			case MarkerStyle.Trapezoid:
			{
				PointF[] array = new PointF[4];
				array[0].X = empty.X;
				array[0].Y = empty.Bottom;
				array[1].X = empty.X + empty.Width / 4f;
				array[1].Y = empty.Top;
				array[2].X = empty.X + empty.Width / 4f * 3f;
				array[2].Y = empty.Top;
				array[3].X = empty.Right;
				array[3].Y = empty.Bottom;
				graphicsPath.AddPolygon(array);
				break;
			}
			case MarkerStyle.Triangle:
			{
				PointF[] array = new PointF[3];
				array[0].X = empty.X;
				array[0].Y = empty.Bottom;
				array[1].X = empty.X + empty.Width / 2f;
				array[1].Y = empty.Top;
				array[2].X = empty.Right;
				array[2].Y = empty.Bottom;
				graphicsPath.AddPolygon(array);
				break;
			}
			case MarkerStyle.Wedge:
			{
				if (empty.Width >= empty.Height)
				{
					graphicsPath = CreateMarker(point, markerWidth, markerHeight, MarkerStyle.Triangle);
					break;
				}
				float num21 = (float)Math.Pow(Math.Pow(empty.Width, 2.0) - (double)(empty.Width / 2f * (empty.Width / 2f)), 0.5);
				PointF[] array = new PointF[5];
				array[0].X = empty.X;
				array[0].Y = empty.Y + num21;
				array[1].X = empty.X + empty.Width / 2f;
				array[1].Y = empty.Y;
				array[2].X = empty.X + empty.Width;
				array[2].Y = empty.Y + num21;
				array[3].X = empty.X + empty.Width;
				array[3].Y = empty.Y + empty.Height;
				array[4].X = empty.X;
				array[4].Y = empty.Y + empty.Height;
				graphicsPath.AddPolygon(array);
				break;
			}
			case MarkerStyle.Pentagon:
			{
				float y = (float)Math.Cos(Math.PI * 2.0 / 5.0);
				float num18 = (float)Math.Cos(Math.PI / 5.0);
				float num19 = (float)Math.Sin(Math.PI * 2.0 / 5.0);
				float num20 = (float)Math.Sin(Math.PI * 4.0 / 5.0);
				PointF[] array = new PointF[5];
				array[0].X = 0f;
				array[0].Y = 1f;
				array[1].X = num19;
				array[1].Y = y;
				array[2].X = num20;
				array[2].Y = 0f - num18;
				array[3].X = 0f - num20;
				array[3].Y = 0f - num18;
				array[4].X = 0f - num19;
				array[4].Y = y;
				using (Matrix matrix = new Matrix())
				{
					matrix.Scale(markerWidth / 2f, markerHeight / 2f);
					matrix.TransformPoints(array);
					matrix.Reset();
					matrix.Rotate(180f);
					matrix.TransformPoints(array);
					matrix.Reset();
					matrix.Translate(point.X, point.Y);
					matrix.TransformPoints(array);
				}
				graphicsPath.AddPolygon(array);
				break;
			}
			case MarkerStyle.PushPin:
			{
				PointF[] array = new PointF[11];
				float num = empty.Width / 5f;
				float num2 = empty.Height / 5f;
				float num3 = empty.X + empty.Width / 2f;
				float num4 = empty.Y + empty.Height / 2f;
				array[0].X = num3 - 3f * num;
				array[1].X = num3 + 3f * num;
				array[2].X = num3 + 1.5f * num;
				array[10].X = num3 - 1.5f * num;
				array[3].X = num3 + 2.5f * num;
				array[9].X = num3 - 2.5f * num;
				array[4].X = num3 + 4.5f * num;
				array[8].X = num3 - 4.5f * num;
				array[5].X = num3 + 1f * num;
				array[7].X = num3 - 1f * num;
				array[6].X = num3;
				float num7 = array[0].Y = (array[1].Y = num4 - 20f * num2);
				num7 = (array[2].Y = (array[10].Y = num4 - 18f * num2));
				num7 = (array[3].Y = (array[9].Y = num4 - 12f * num2));
				ref PointF reference = ref array[4];
				ref PointF reference2 = ref array[5];
				ref PointF reference3 = ref array[7];
				float num13 = array[8].Y = num4 - 10f * num2;
				float num15 = reference3.Y = num13;
				num7 = (reference.Y = (reference2.Y = num15));
				array[6].Y = num4;
				PointF[] array2 = array;
				for (int i = 0; i < array2.Length; i++)
				{
					_ = ref array2[i];
					point.X = (float)Math.Round(point.X);
					point.Y = (float)Math.Round(point.Y);
				}
				graphicsPath.AddPolygon(array);
				break;
			}
			default:
				throw new InvalidOperationException(SR.invalid_marker_type);
			}
			return graphicsPath;
		}

		internal PointF[] CreateStarPolygon(RectangleF rectReal, int numberOfCorners)
		{
			bool flag = true;
			PointF[] array = new PointF[numberOfCorners * 2];
			PointF[] array2 = new PointF[1];
			RectangleF rectangleF = new RectangleF(0f, 0f, 1f, 1f);
			using (Matrix matrix = new Matrix())
			{
				for (int i = 0; i < numberOfCorners * 2; i++)
				{
					array2[0] = new PointF(rectangleF.X + rectangleF.Width / 2f, flag ? rectangleF.Y : (rectangleF.Y + rectangleF.Height / 4f));
					matrix.Reset();
					matrix.RotateAt((float)i * (360f / ((float)numberOfCorners * 2f)), new PointF(rectangleF.X + rectangleF.Width / 2f, rectangleF.Y + rectangleF.Height / 2f));
					matrix.TransformPoints(array2);
					array[i] = array2[0];
					flag = !flag;
				}
				matrix.Reset();
				matrix.Scale(rectReal.Width, rectReal.Height);
				matrix.TransformPoints(array);
				matrix.Reset();
				matrix.Translate(rectReal.X, rectReal.Y);
				matrix.TransformPoints(array);
				return array;
			}
		}

		internal void DrawMarkerRel(PointF point, MarkerStyle markerStyle, int markerSize, Color markerColor, GradientType markerGradientType, MapHatchStyle markerHatchStyle, Color markerSecondaryColor, MapDashStyle markerBorderStyle, Color markerBorderColor, int markerBorderSize, string markerImage, Color markerImageTranspColor, int shadowSize, Color shadowColor, RectangleF imageScaleRect)
		{
			DrawMarkerAbs(GetAbsolutePoint(point), markerStyle, markerSize, markerColor, markerGradientType, markerHatchStyle, markerSecondaryColor, markerBorderStyle, markerBorderColor, markerBorderSize, markerImage, markerImageTranspColor, shadowSize, shadowColor, imageScaleRect, forceAntiAlias: false, 0f);
		}

		internal void DrawMarkerAbs(PointF point, MarkerStyle markerStyle, int markerSize, Color markerColor, GradientType markerGradientType, MapHatchStyle markerHatchStyle, Color markerSecondaryColor, MapDashStyle markerBorderStyle, Color markerBorderColor, int markerBorderSize, string markerImage, Color markerImageTranspColor, int shadowSize, Color shadowColor, RectangleF imageScaleRect, bool forceAntiAlias, float angle)
		{
			if (!string.IsNullOrEmpty(markerImage))
			{
				Image image = common.ImageLoader.LoadImage(markerImage);
				RectangleF empty = RectangleF.Empty;
				if (imageScaleRect == RectangleF.Empty)
				{
					imageScaleRect.Height = image.Height;
					imageScaleRect.Width = image.Width;
				}
				empty.X = point.X - imageScaleRect.Width / 2f;
				empty.Y = point.Y - imageScaleRect.Height / 2f;
				empty.Width = imageScaleRect.Width;
				empty.Height = imageScaleRect.Height;
				ImageAttributes imageAttributes = new ImageAttributes();
				if (markerImageTranspColor != Color.Empty)
				{
					imageAttributes.SetColorKey(markerImageTranspColor, markerImageTranspColor, ColorAdjustType.Default);
				}
				if (shadowSize != 0 && shadowColor != Color.Empty)
				{
					ImageAttributes imageAttributes2 = new ImageAttributes();
					imageAttributes2.SetColorKey(markerImageTranspColor, markerImageTranspColor, ColorAdjustType.Default);
					ColorMatrix colorMatrix = new ColorMatrix();
					colorMatrix.Matrix00 = 0.25f;
					colorMatrix.Matrix11 = 0.25f;
					colorMatrix.Matrix22 = 0.25f;
					colorMatrix.Matrix33 = 0.5f;
					colorMatrix.Matrix44 = 1f;
					imageAttributes2.SetColorMatrix(colorMatrix);
					shadowDrawingMode = true;
					DrawImage(image, new Rectangle((int)empty.X + shadowSize, (int)empty.Y + shadowSize, (int)empty.Width, (int)empty.Height), 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, imageAttributes2);
					shadowDrawingMode = false;
				}
				DrawImage(image, new Rectangle((int)empty.X, (int)empty.Y, (int)empty.Width, (int)empty.Height), 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, imageAttributes);
			}
			else
			{
				if (markerStyle == MarkerStyle.None || markerSize <= 0)
				{
					return;
				}
				SmoothingMode smoothingMode = base.SmoothingMode;
				if (forceAntiAlias)
				{
					base.SmoothingMode = SmoothingMode.AntiAlias;
				}
				RectangleF empty2 = RectangleF.Empty;
				empty2.X = point.X - (float)markerSize / 2f;
				empty2.Y = point.Y - (float)markerSize / 2f;
				empty2.Width = markerSize;
				empty2.Height = markerSize;
				Brush brush = null;
				GraphicsPath graphicsPath = null;
				Pen pen = null;
				Brush brush2 = null;
				try
				{
					brush = CreateBrush(empty2, markerColor, markerHatchStyle, string.Empty, MapImageWrapMode.Unscaled, Color.Empty, MapImageAlign.Center, markerGradientType, markerSecondaryColor);
					if (markerBorderSize > 0)
					{
						pen = new Pen(markerBorderColor, markerBorderSize);
						pen.DashStyle = GetPenStyle(markerBorderStyle);
						pen.Alignment = PenAlignment.Center;
						pen.LineJoin = LineJoin.Round;
					}
					if (shadowSize > 0)
					{
						brush2 = GetShadowBrush();
					}
					graphicsPath = CreateMarker(point, markerSize, markerSize, markerStyle);
					if (brush2 != null)
					{
						using (Matrix matrix = new Matrix())
						{
							matrix.Translate(shadowSize, shadowSize);
							graphicsPath.Transform(matrix);
							FillPath(brush2, graphicsPath);
							matrix.Reset();
							matrix.Translate(-shadowSize, -shadowSize);
							graphicsPath.Transform(matrix);
						}
					}
					FillPath(brush, graphicsPath);
					if (pen != null)
					{
						DrawPath(pen, graphicsPath);
					}
				}
				finally
				{
					brush?.Dispose();
					graphicsPath?.Dispose();
					pen?.Dispose();
					brush2?.Dispose();
				}
				if (forceAntiAlias)
				{
					base.SmoothingMode = smoothingMode;
				}
			}
		}

		internal Size MeasureStringAbs(string text, Font font)
		{
			SizeF sizeF = MeasureString(text, font);
			return new Size((int)Math.Ceiling(sizeF.Width), (int)Math.Ceiling(sizeF.Height));
		}

		internal Size MeasureStringAbs(string text, Font font, SizeF layoutArea, StringFormat stringFormat)
		{
			SizeF sizeF = MeasureString(text, font, layoutArea, stringFormat);
			return new Size((int)Math.Ceiling(sizeF.Width), (int)Math.Ceiling(sizeF.Height));
		}

		internal void DrawStringAbs(string text, Font font, Brush brush, PointF absPosition, StringFormat format, int angle)
		{
			myMatrix = base.Transform.Clone();
			myMatrix.RotateAt(angle, absPosition);
			GraphicsState gstate = Save();
			base.Transform = myMatrix;
			DrawString(text, font, brush, absPosition, format);
			Restore(gstate);
		}

		internal void DrawStringAbs(string text, Font font, Brush brush, RectangleF layoutRectangle, StringFormat format)
		{
			if (layoutRectangle.Width != 0f && layoutRectangle.Height != 0f)
			{
				DrawString(text, font, brush, layoutRectangle, format);
			}
		}

		internal SizeF MeasureStringRel(string text, Font font)
		{
			SizeF size = MeasureString(text, font);
			return GetRelativeSize(size);
		}

		internal SizeF MeasureStringRel(string text, Font font, SizeF layoutArea, StringFormat stringFormat)
		{
			SizeF absoluteSize = GetAbsoluteSize(layoutArea);
			SizeF size = MeasureString(text, font, absoluteSize, stringFormat);
			return GetRelativeSize(size);
		}

		internal void DrawStringRel(string text, Font font, Brush brush, PointF position, StringFormat format, int angle)
		{
			DrawStringAbs(text, font, brush, GetAbsolutePoint(position), format, angle);
		}

		internal void DrawStringRel(string text, Font font, Brush brush, RectangleF layoutRectangle, StringFormat format)
		{
			if (layoutRectangle.Width != 0f && layoutRectangle.Height != 0f)
			{
				RectangleF absoluteRectangle = GetAbsoluteRectangle(layoutRectangle);
				DrawString(text, font, brush, absoluteRectangle, format);
			}
		}

		internal void DrawStringRel(string text, Font font, Brush brush, RectangleF layoutRectangle, StringFormat format, int angle)
		{
			PointF empty = PointF.Empty;
			if (layoutRectangle.Width != 0f && layoutRectangle.Height != 0f)
			{
				RectangleF absoluteRectangle = GetAbsoluteRectangle(layoutRectangle);
				SizeF sizeF = MeasureString(text, font, absoluteRectangle.Size, format);
				if (format.Alignment == StringAlignment.Near)
				{
					empty.X = absoluteRectangle.X + sizeF.Width / 2f;
					empty.Y = (absoluteRectangle.Bottom + absoluteRectangle.Top) / 2f;
				}
				else if (format.Alignment == StringAlignment.Far)
				{
					empty.X = absoluteRectangle.Right - sizeF.Width / 2f;
					empty.Y = (absoluteRectangle.Bottom + absoluteRectangle.Top) / 2f;
				}
				else
				{
					empty.X = (absoluteRectangle.Left + absoluteRectangle.Right) / 2f;
					empty.Y = (absoluteRectangle.Bottom + absoluteRectangle.Top) / 2f;
				}
				myMatrix = base.Transform.Clone();
				myMatrix.RotateAt(angle, empty);
				Matrix transform = base.Transform;
				base.Transform = myMatrix;
				DrawString(text, font, brush, absoluteRectangle, format);
				base.Transform = transform;
			}
		}

		internal void FillRectangleRel(RectangleF rectF, Color backColor, MapHatchStyle backHatchStyle, string backImage, MapImageWrapMode backImageMode, Color backImageTranspColor, MapImageAlign backImageAlign, GradientType backGradientType, Color backSecondaryColor, Color borderColor, int borderWidth, MapDashStyle borderStyle, Color shadowColor, int shadowOffset, PenAlignment penAlignment)
		{
			FillRectangleRel(rectF, backColor, backHatchStyle, backImage, backImageMode, backImageTranspColor, backImageAlign, backGradientType, backSecondaryColor, borderColor, borderWidth, borderStyle, shadowColor, shadowOffset, penAlignment, circular: false, 0, circle3D: false);
		}

		internal void FillRectangleRel(RectangleF rectF, Color backColor, MapHatchStyle backHatchStyle, string backImage, MapImageWrapMode backImageMode, Color backImageTranspColor, MapImageAlign backImageAlign, GradientType backGradientType, Color backSecondaryColor, Color borderColor, int borderWidth, MapDashStyle borderStyle, Color shadowColor, int shadowOffset, PenAlignment penAlignment, bool circular, int circularSectorsCount, bool circle3D)
		{
			Brush brush = null;
			Brush brush2 = null;
			SmoothingMode smoothingMode = base.SmoothingMode;
			if (!circular)
			{
				base.SmoothingMode = SmoothingMode.Default;
			}
			if (backColor.IsEmpty)
			{
				backColor = Color.White;
			}
			if (backSecondaryColor.IsEmpty)
			{
				backSecondaryColor = Color.White;
			}
			if (borderColor.IsEmpty)
			{
				borderWidth = 0;
			}
			RectangleF absoluteRectangle = GetAbsoluteRectangle(rectF);
			if (absoluteRectangle.Width < 1f && absoluteRectangle.Width > 0f)
			{
				absoluteRectangle.Width = 1f;
			}
			if (absoluteRectangle.Height < 1f && absoluteRectangle.Height > 0f)
			{
				absoluteRectangle.Height = 1f;
			}
			absoluteRectangle = Round(absoluteRectangle);
			RectangleF rectangleF = (penAlignment != PenAlignment.Inset || borderWidth <= 0) ? absoluteRectangle : ((base.ActiveRenderingType != RenderingType.Svg && !IsMetafile) ? new RectangleF(absoluteRectangle.X + (float)borderWidth, absoluteRectangle.Y + (float)borderWidth, absoluteRectangle.Width - (float)borderWidth * 2f + 1f, absoluteRectangle.Height - (float)borderWidth * 2f + 1f) : new RectangleF(absoluteRectangle.X, absoluteRectangle.Y, absoluteRectangle.Width, absoluteRectangle.Height));
			if (string.IsNullOrEmpty(backImage) || backImageMode == MapImageWrapMode.Unscaled || backImageMode == MapImageWrapMode.Scaled)
			{
				brush = ((backHatchStyle != 0) ? GetHatchBrush(backHatchStyle, backColor, backSecondaryColor) : ((backGradientType != 0) ? GetGradientBrush(absoluteRectangle, backColor, backSecondaryColor, backGradientType) : ((!(backColor == Color.Empty) && !(backColor == Color.Transparent)) ? new SolidBrush(backColor) : null)));
			}
			else
			{
				brush2 = brush;
				brush = GetTextureBrush(backImage, backImageTranspColor, backImageMode);
			}
			FillRectangleShadowAbs(absoluteRectangle, shadowColor, shadowOffset, backColor, circular, circularSectorsCount);
			if (!string.IsNullOrEmpty(backImage) && (backImageMode == MapImageWrapMode.Unscaled || backImageMode == MapImageWrapMode.Scaled))
			{
				Image image = common.ImageLoader.LoadImage(backImage);
				ImageAttributes imageAttributes = new ImageAttributes();
				if (backImageTranspColor != Color.Empty)
				{
					imageAttributes.SetColorKey(backImageTranspColor, backImageTranspColor, ColorAdjustType.Default);
				}
				RectangleF rectangleF2 = default(RectangleF);
				rectangleF2.X = rectangleF.X;
				rectangleF2.Y = rectangleF.Y;
				rectangleF2.Width = rectangleF.Width;
				rectangleF2.Height = rectangleF.Height;
				if (backImageMode == MapImageWrapMode.Unscaled)
				{
					rectangleF2.Width = Math.Min(rectangleF.Width, image.Width);
					rectangleF2.Height = Math.Min(rectangleF.Height, image.Height);
					if (rectangleF2.Width < rectangleF.Width)
					{
						switch (backImageAlign)
						{
						case MapImageAlign.TopRight:
						case MapImageAlign.Right:
						case MapImageAlign.BottomRight:
							rectangleF2.X = rectangleF.Right - rectangleF2.Width;
							break;
						case MapImageAlign.Top:
						case MapImageAlign.Bottom:
						case MapImageAlign.Center:
							rectangleF2.X = rectangleF.X + (rectangleF.Width - rectangleF2.Width) / 2f;
							break;
						}
					}
					if (rectangleF2.Height < rectangleF.Height)
					{
						switch (backImageAlign)
						{
						case MapImageAlign.BottomRight:
						case MapImageAlign.Bottom:
						case MapImageAlign.BottomLeft:
							rectangleF2.Y = rectangleF.Bottom - rectangleF2.Height;
							break;
						case MapImageAlign.Right:
						case MapImageAlign.Left:
						case MapImageAlign.Center:
							rectangleF2.Y = rectangleF.Y + (rectangleF.Height - rectangleF2.Height) / 2f;
							break;
						}
					}
				}
				if (brush != null)
				{
					if (circular)
					{
						DrawCircleAbs(null, brush, rectangleF, circularSectorsCount, circle3D);
					}
					else
					{
						FillRectangle(brush, rectangleF);
					}
				}
				DrawImage(image, new Rectangle((int)Math.Round(rectangleF2.X), (int)Math.Round(rectangleF2.Y), (int)Math.Round(rectangleF2.Width), (int)Math.Round(rectangleF2.Height)), 0f, 0f, (backImageMode == MapImageWrapMode.Unscaled) ? rectangleF2.Width : ((float)image.Width), (backImageMode == MapImageWrapMode.Unscaled) ? rectangleF2.Height : ((float)image.Height), GraphicsUnit.Pixel, imageAttributes);
			}
			else
			{
				if (brush2 != null && backImageTranspColor != Color.Empty)
				{
					if (circular)
					{
						DrawCircleAbs(null, brush2, rectangleF, circularSectorsCount, circle3D);
					}
					else
					{
						FillRectangle(brush2, rectangleF);
					}
				}
				if (brush != null)
				{
					if (circular)
					{
						DrawCircleAbs(null, brush, rectangleF, circularSectorsCount, circle3D);
					}
					else
					{
						FillRectangle(brush, rectangleF);
					}
				}
			}
			if (borderWidth > 0 && borderStyle != 0)
			{
				if (pen.Color != borderColor)
				{
					pen.Color = borderColor;
				}
				if (pen.Width != (float)borderWidth)
				{
					pen.Width = borderWidth;
				}
				if (pen.Alignment != penAlignment)
				{
					pen.Alignment = penAlignment;
				}
				if (pen.DashStyle != GetPenStyle(borderStyle))
				{
					pen.DashStyle = GetPenStyle(borderStyle);
				}
				if (circular)
				{
					DrawCircleAbs(pen, null, absoluteRectangle, circularSectorsCount, circle3D: false);
				}
				else
				{
					if (pen.Alignment == PenAlignment.Inset && pen.Width > 1f)
					{
						absoluteRectangle.Width += 1f;
						absoluteRectangle.Height += 1f;
					}
					DrawRectangle(pen, absoluteRectangle.X, absoluteRectangle.Y, absoluteRectangle.Width, absoluteRectangle.Height);
				}
			}
			brush?.Dispose();
			base.SmoothingMode = smoothingMode;
		}

		internal void FillRectangleShadowAbs(RectangleF rect, Color shadowColor, float shadowOffset, Color backColor)
		{
			FillRectangleShadowAbs(rect, shadowColor, shadowOffset, backColor, circular: false, 0);
		}

		internal void FillRectangleShadowAbs(RectangleF rect, Color shadowColor, float shadowOffset, Color backColor, bool circular, int circularSectorsCount)
		{
			if (rect.Height == 0f || rect.Width == 0f || shadowOffset == 0f)
			{
				return;
			}
			if (!softShadows || circularSectorsCount > 2)
			{
				RectangleF empty = RectangleF.Empty;
				if (shadowOffset != 0f && !(shadowColor == Color.Empty))
				{
					RectangleF rectangleF = Round(rect);
					SolidBrush brush = new SolidBrush((shadowColor.A != byte.MaxValue) ? shadowColor : Color.FromArgb((int)backColor.A / 2, shadowColor));
					empty.X = rectangleF.X + shadowOffset;
					empty.Y = rectangleF.Y + shadowOffset;
					empty.Width = rectangleF.Width;
					empty.Height = rectangleF.Height;
					shadowDrawingMode = true;
					if (circular)
					{
						DrawCircleAbs(null, brush, empty, circularSectorsCount, circle3D: false);
					}
					else
					{
						FillRectangle(brush, empty);
					}
					shadowDrawingMode = false;
				}
				return;
			}
			RectangleF empty2 = RectangleF.Empty;
			if (shadowOffset != 0f && !(shadowColor == Color.Empty))
			{
				RectangleF rectangleF2 = Round(rect);
				empty2.X = rectangleF2.X + shadowOffset - 1f;
				empty2.Y = rectangleF2.Y + shadowOffset - 1f;
				empty2.Width = rectangleF2.Width + 2f;
				empty2.Height = rectangleF2.Height + 2f;
				float val = shadowOffset * 0.7f;
				val = Math.Max(val, 2f);
				val = Math.Min(val, empty2.Width / 4f);
				val = Math.Min(val, empty2.Height / 4f);
				val = (float)Math.Ceiling(val);
				if (circular)
				{
					val = empty2.Width / 2f;
				}
				GraphicsPath graphicsPath = new GraphicsPath();
				graphicsPath.AddLine(empty2.X + val, empty2.Y, empty2.Right - val, empty2.Y);
				graphicsPath.AddArc(empty2.Right - 2f * val, empty2.Y, 2f * val, 2f * val, 270f, 90f);
				graphicsPath.AddLine(empty2.Right, empty2.Y + val, empty2.Right, empty2.Bottom - val);
				graphicsPath.AddArc(empty2.Right - 2f * val, empty2.Bottom - 2f * val, 2f * val, 2f * val, 0f, 90f);
				graphicsPath.AddLine(empty2.Right - val, empty2.Bottom, empty2.X + val, empty2.Bottom);
				graphicsPath.AddArc(empty2.X, empty2.Bottom - 2f * val, 2f * val, 2f * val, 90f, 90f);
				graphicsPath.AddLine(empty2.X, empty2.Bottom - val, empty2.X, empty2.Y + val);
				graphicsPath.AddArc(empty2.X, empty2.Y, 2f * val, 2f * val, 180f, 90f);
				PathGradientBrush pathGradientBrush = new PathGradientBrush(graphicsPath);
				pathGradientBrush.CenterColor = shadowColor;
				Color[] array2 = pathGradientBrush.SurroundColors = new Color[1]
				{
					Color.Transparent
				};
				pathGradientBrush.CenterPoint = new PointF(empty2.X + empty2.Width / 2f, empty2.Y + empty2.Height / 2f);
				PointF focusScales = new PointF(1f - 2f * shadowOffset / empty2.Width, 1f - 2f * shadowOffset / empty2.Height);
				if (focusScales.X < 0f)
				{
					focusScales.X = 0f;
				}
				if (focusScales.Y < 0f)
				{
					focusScales.Y = 0f;
				}
				pathGradientBrush.FocusScales = focusScales;
				shadowDrawingMode = true;
				FillPath(pathGradientBrush, graphicsPath);
				shadowDrawingMode = false;
			}
		}

		internal void DrawCircleAbs(Pen pen, Brush brush, RectangleF position, int polygonSectorsNumber, bool circle3D)
		{
			bool flag = circle3D && brush != null;
			if (polygonSectorsNumber <= 2 && !flag)
			{
				if (brush != null)
				{
					FillEllipse(brush, position);
				}
				if (pen != null)
				{
					DrawEllipse(pen, position);
				}
				return;
			}
			PointF pointF = new PointF(position.X + position.Width / 2f, position.Y);
			PointF pointF2 = new PointF(position.X + position.Width / 2f, position.Y + position.Height / 2f);
			float num = 0f;
			GraphicsPath graphicsPath = new GraphicsPath();
			PointF pointF3 = PointF.Empty;
			float num2 = 0f;
			SmoothingMode smoothingMode = base.SmoothingMode;
			if (flag)
			{
				base.SmoothingMode = SmoothingMode.None;
			}
			num = ((polygonSectorsNumber > 2) ? (360f / (float)polygonSectorsNumber) : 1f);
			for (num2 = 0f; num2 < 360f; num2 += num)
			{
				Matrix matrix = new Matrix();
				matrix.RotateAt(num2, pointF2);
				PointF[] array = new PointF[1]
				{
					pointF
				};
				matrix.TransformPoints(array);
				if (!pointF3.IsEmpty)
				{
					graphicsPath.AddLine(pointF3, array[0]);
					if (flag)
					{
						graphicsPath.AddLine(array[0], pointF2);
						graphicsPath.AddLine(pointF2, pointF3);
						FillPath(GetSector3DBrush(brush, num2, num), graphicsPath);
						graphicsPath.Reset();
					}
				}
				pointF3 = array[0];
			}
			graphicsPath.CloseAllFigures();
			if (!pointF3.IsEmpty && flag)
			{
				graphicsPath.AddLine(pointF3, pointF);
				graphicsPath.AddLine(pointF, pointF2);
				graphicsPath.AddLine(pointF2, pointF3);
				FillPath(GetSector3DBrush(brush, num2, num), graphicsPath);
				graphicsPath.Reset();
			}
			if (flag)
			{
				base.SmoothingMode = smoothingMode;
			}
			if (brush != null && !circle3D)
			{
				FillPath(brush, graphicsPath);
			}
			if (pen != null)
			{
				DrawPath(pen, graphicsPath);
			}
		}

		internal void DrawRectangleRel(Pen pen, RectangleF rect)
		{
			RectangleF absoluteRectangle = GetAbsoluteRectangle(rect);
			DrawRectangle(pen, absoluteRectangle.X, absoluteRectangle.Y, absoluteRectangle.Width, absoluteRectangle.Height);
		}

		internal Brush GetSector3DBrush(Brush brush, float curentSector, float sectorSize)
		{
			Color beginColor = Color.Gray;
			if (brush is HatchBrush)
			{
				beginColor = ((HatchBrush)brush).BackgroundColor;
			}
			else if (brush is LinearGradientBrush)
			{
				beginColor = ((LinearGradientBrush)brush).LinearColors[0];
			}
			else if (brush is PathGradientBrush)
			{
				beginColor = ((PathGradientBrush)brush).CenterColor;
			}
			else if (brush is SolidBrush)
			{
				beginColor = ((SolidBrush)brush).Color;
			}
			curentSector -= sectorSize / 2f;
			if (sectorSize == 72f && curentSector == 180f)
			{
				curentSector *= 0.8f;
			}
			if (curentSector > 180f)
			{
				curentSector = 360f - curentSector;
			}
			curentSector /= 180f;
			beginColor = GetBrightGradientColor(beginColor, curentSector);
			return new SolidBrush(beginColor);
		}

		internal Color GetBrightGradientColor(Color beginColor, double position)
		{
			double num = 0.5;
			if (position < num)
			{
				return GetGradientColor(Color.FromArgb(beginColor.A, 255, 255, 255), beginColor, 1.0 - num + position);
			}
			if (0.0 - num + position < 1.0)
			{
				return GetGradientColor(beginColor, Color.Black, 0.0 - num + position);
			}
			return Color.FromArgb(beginColor.A, 0, 0, 0);
		}

		internal void FillRectangleAbs(RectangleF rect, Color backColor, MapHatchStyle backHatchStyle, string backImage, MapImageWrapMode backImageMode, Color backImageTranspColor, MapImageAlign backImageAlign, GradientType backGradientType, Color backSecondaryColor, Color borderColor, int borderWidth, MapDashStyle borderStyle, PenAlignment penAlignment)
		{
			Brush brush = null;
			Brush brush2 = null;
			SmoothingMode smoothingMode = base.SmoothingMode;
			base.SmoothingMode = SmoothingMode.None;
			if (backColor.IsEmpty)
			{
				backColor = Color.White;
			}
			if (backSecondaryColor.IsEmpty)
			{
				backSecondaryColor = Color.White;
			}
			if (borderColor.IsEmpty)
			{
				borderColor = Color.White;
				borderWidth = 0;
			}
			pen.Color = borderColor;
			pen.Width = borderWidth;
			pen.Alignment = penAlignment;
			pen.DashStyle = GetPenStyle(borderStyle);
			if (backGradientType == GradientType.None)
			{
				solidBrush.Color = backColor;
				brush = solidBrush;
			}
			else
			{
				brush = GetGradientBrush(rect, backColor, backSecondaryColor, backGradientType);
			}
			if (backHatchStyle != 0)
			{
				brush = GetHatchBrush(backHatchStyle, backColor, backSecondaryColor);
			}
			if (!string.IsNullOrEmpty(backImage) && backImageMode != MapImageWrapMode.Unscaled && backImageMode != MapImageWrapMode.Scaled)
			{
				brush2 = brush;
				brush = GetTextureBrush(backImage, backImageTranspColor, backImageMode);
			}
			RectangleF rectangleF = new RectangleF(rect.X + (float)borderWidth, rect.Y + (float)borderWidth, rect.Width - (float)(borderWidth * 2), rect.Height - (float)(borderWidth * 2));
			rectangleF.Width += 1f;
			rectangleF.Height += 1f;
			if (!string.IsNullOrEmpty(backImage) && (backImageMode == MapImageWrapMode.Unscaled || backImageMode == MapImageWrapMode.Scaled))
			{
				Image image = common.ImageLoader.LoadImage(backImage);
				ImageAttributes imageAttributes = new ImageAttributes();
				if (backImageTranspColor != Color.Empty)
				{
					imageAttributes.SetColorKey(backImageTranspColor, backImageTranspColor, ColorAdjustType.Default);
				}
				RectangleF rectangleF2 = default(RectangleF);
				rectangleF2.X = rectangleF.X;
				rectangleF2.Y = rectangleF.Y;
				rectangleF2.Width = rectangleF.Width;
				rectangleF2.Height = rectangleF.Height;
				if (backImageMode == MapImageWrapMode.Unscaled)
				{
					rectangleF2.Width = image.Width;
					rectangleF2.Height = image.Height;
					if (rectangleF2.Width < rectangleF.Width)
					{
						switch (backImageAlign)
						{
						case MapImageAlign.TopRight:
						case MapImageAlign.Right:
						case MapImageAlign.BottomRight:
							rectangleF2.X = rectangleF.Right - rectangleF2.Width;
							break;
						case MapImageAlign.Top:
						case MapImageAlign.Bottom:
						case MapImageAlign.Center:
							rectangleF2.X = rectangleF.X + (rectangleF.Width - rectangleF2.Width) / 2f;
							break;
						}
					}
					if (rectangleF2.Height < rectangleF.Height)
					{
						switch (backImageAlign)
						{
						case MapImageAlign.BottomRight:
						case MapImageAlign.Bottom:
						case MapImageAlign.BottomLeft:
							rectangleF2.Y = rectangleF.Bottom - rectangleF2.Height;
							break;
						case MapImageAlign.Right:
						case MapImageAlign.Left:
						case MapImageAlign.Center:
							rectangleF2.Y = rectangleF.Y + (rectangleF.Height - rectangleF2.Height) / 2f;
							break;
						}
					}
				}
				FillRectangle(brush, rect.X, rect.Y, rect.Width + 1f, rect.Height + 1f);
				DrawImage(image, new Rectangle((int)Math.Round(rectangleF2.X), (int)Math.Round(rectangleF2.Y), (int)Math.Round(rectangleF2.Width), (int)Math.Round(rectangleF2.Height)), 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, imageAttributes);
			}
			else
			{
				if (brush2 != null && backImageTranspColor != Color.Empty)
				{
					FillRectangle(brush2, rect.X, rect.Y, rect.Width + 1f, rect.Height + 1f);
				}
				FillRectangle(brush, rect.X, rect.Y, rect.Width + 1f, rect.Height + 1f);
			}
			if (borderStyle != 0)
			{
				if (borderWidth > 1)
				{
					DrawRectangle(pen, rect.X, rect.Y, rect.Width + 1f, rect.Height + 1f);
				}
				else if (borderWidth == 1)
				{
					DrawRectangle(pen, rect.X, rect.Y, rect.Width, rect.Height);
				}
			}
			if (backGradientType != 0)
			{
				brush.Dispose();
			}
			if (!string.IsNullOrEmpty(backImage) && backImageMode != MapImageWrapMode.Unscaled && backImageMode != MapImageWrapMode.Scaled)
			{
				brush.Dispose();
			}
			if (backHatchStyle != 0)
			{
				brush.Dispose();
			}
			base.SmoothingMode = smoothingMode;
		}

		internal void DrawPathShadowAbs(GraphicsPath path, Color shadowColor, float shadowWidth)
		{
			if (shadowWidth != 0f)
			{
				if (shadowColor == Color.Empty)
				{
					shadowColor = Color.FromArgb(128, 128, 128, 128);
				}
				if (shadowColor.A == byte.MaxValue)
				{
					shadowColor = Color.FromArgb((int)shadowColor.A / 2, shadowColor);
				}
				Matrix matrix = new Matrix();
				matrix.Translate(shadowWidth, shadowWidth);
				path.Transform(matrix);
				using (Brush brush = new SolidBrush(shadowColor))
				{
					FillPath(brush, path);
				}
				matrix.Reset();
				matrix.Translate(0f - shadowWidth, 0f - shadowWidth);
				path.Transform(matrix);
			}
		}

		internal void DrawPathAbs(GraphicsPath path, Color backColor, MapHatchStyle backHatchStyle, string backImage, MapImageWrapMode backImageMode, Color backImageTranspColor, MapImageAlign backImageAlign, GradientType backGradientType, Color backSecondaryColor, Color borderColor, int borderWidth, MapDashStyle borderStyle, PenAlignment penAlignment)
		{
			Brush brush = null;
			Brush brush2 = null;
			if (backColor.IsEmpty)
			{
				backColor = Color.White;
			}
			if (backSecondaryColor.IsEmpty)
			{
				backSecondaryColor = Color.White;
			}
			if (borderColor.IsEmpty)
			{
				borderColor = Color.White;
				borderWidth = 0;
			}
			pen.Color = borderColor;
			pen.Width = borderWidth;
			pen.Alignment = penAlignment;
			pen.DashStyle = GetPenStyle(borderStyle);
			if (backGradientType == GradientType.None)
			{
				solidBrush.Color = backColor;
				brush = solidBrush;
			}
			else
			{
				RectangleF bounds = path.GetBounds();
				bounds.Inflate(new SizeF(2f, 2f));
				brush = GetGradientBrush(bounds, backColor, backSecondaryColor, backGradientType);
			}
			if (backHatchStyle != 0)
			{
				brush = GetHatchBrush(backHatchStyle, backColor, backSecondaryColor);
			}
			if (!string.IsNullOrEmpty(backImage) && backImageMode != MapImageWrapMode.Unscaled && backImageMode != MapImageWrapMode.Scaled)
			{
				brush2 = brush;
				brush = GetTextureBrush(backImage, backImageTranspColor, backImageMode);
			}
			RectangleF bounds2 = path.GetBounds();
			if (!string.IsNullOrEmpty(backImage) && (backImageMode == MapImageWrapMode.Unscaled || backImageMode == MapImageWrapMode.Scaled))
			{
				Image image = common.ImageLoader.LoadImage(backImage);
				ImageAttributes imageAttributes = new ImageAttributes();
				if (backImageTranspColor != Color.Empty)
				{
					imageAttributes.SetColorKey(backImageTranspColor, backImageTranspColor, ColorAdjustType.Default);
				}
				RectangleF rectangleF = default(RectangleF);
				rectangleF.X = bounds2.X;
				rectangleF.Y = bounds2.Y;
				rectangleF.Width = bounds2.Width;
				rectangleF.Height = bounds2.Height;
				if (backImageMode == MapImageWrapMode.Unscaled)
				{
					rectangleF.Width = image.Width;
					rectangleF.Height = image.Height;
					if (rectangleF.Width < bounds2.Width)
					{
						switch (backImageAlign)
						{
						case MapImageAlign.TopRight:
						case MapImageAlign.Right:
						case MapImageAlign.BottomRight:
							rectangleF.X = bounds2.Right - rectangleF.Width;
							break;
						case MapImageAlign.Top:
						case MapImageAlign.Bottom:
						case MapImageAlign.Center:
							rectangleF.X = bounds2.X + (bounds2.Width - rectangleF.Width) / 2f;
							break;
						}
					}
					if (rectangleF.Height < bounds2.Height)
					{
						switch (backImageAlign)
						{
						case MapImageAlign.BottomRight:
						case MapImageAlign.Bottom:
						case MapImageAlign.BottomLeft:
							rectangleF.Y = bounds2.Bottom - rectangleF.Height;
							break;
						case MapImageAlign.Right:
						case MapImageAlign.Left:
						case MapImageAlign.Center:
							rectangleF.Y = bounds2.Y + (bounds2.Height - rectangleF.Height) / 2f;
							break;
						}
					}
				}
				FillPath(brush, path);
				Region clip = base.Clip;
				base.Clip = new Region(path);
				DrawImage(image, new Rectangle((int)Math.Round(rectangleF.X), (int)Math.Round(rectangleF.Y), (int)Math.Round(rectangleF.Width), (int)Math.Round(rectangleF.Height)), 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, imageAttributes);
				base.Clip = clip;
			}
			else
			{
				if (brush2 != null && backImageTranspColor != Color.Empty)
				{
					FillPath(brush2, path);
				}
				FillPath(brush, path);
			}
			if (borderColor != Color.Empty && borderWidth > 0 && borderStyle != 0)
			{
				DrawPath(pen, path);
			}
		}

		public PointF PixelsToPercents(PointF pointInPixels)
		{
			return GetRelativePoint(pointInPixels);
		}

		public PointF PercentsToPixels(PointF pointInPercents)
		{
			return GetAbsolutePoint(pointInPercents);
		}

		public SizeF PixelsToPercents(SizeF sizeInPixels)
		{
			return GetRelativeSize(sizeInPixels);
		}

		public SizeF PercentsToPixels(SizeF sizeInPercents)
		{
			return GetAbsoluteSize(sizeInPercents);
		}

		public MapPoint PixelsToGeographic(PointF pointInPixels)
		{
			PointF relativePoint = GetRelativePoint(pointInPixels);
			return common.MapCore.PercentsToGeographic(relativePoint.X, relativePoint.Y);
		}

		public PointF GeographicToPixels(MapPoint pointOnMap)
		{
			PointF relative = common.MapCore.GeographicToPercents(pointOnMap).ToPointF();
			return GetAbsolutePoint(relative);
		}

		internal float GetRelativeX(float absoluteX)
		{
			return absoluteX * 100f / (float)(width - 1);
		}

		internal float GetRelativeY(float absoluteY)
		{
			return absoluteY * 100f / (float)(height - 1);
		}

		internal float GetRelativeWidth(float absoluteWidth)
		{
			return absoluteWidth * 100f / (float)width;
		}

		internal float GetRelativeHeight(float absoluteHeight)
		{
			return absoluteHeight * 100f / (float)height;
		}

		internal float GetAbsoluteX(float relativeX)
		{
			return relativeX * (float)(width - 1) / 100f;
		}

		internal float GetAbsoluteY(float relativeY)
		{
			return relativeY * (float)(height - 1) / 100f;
		}

		internal float GetAbsoluteWidth(float relativeWidth)
		{
			return relativeWidth * (float)width / 100f;
		}

		internal float GetAbsoluteHeight(float relativeHeight)
		{
			return relativeHeight * (float)height / 100f;
		}

		public RectangleF GetRelativeRectangle(RectangleF absolute)
		{
			RectangleF empty = RectangleF.Empty;
			empty.X = GetRelativeX(absolute.X);
			empty.Y = GetRelativeY(absolute.Y);
			empty.Width = GetRelativeWidth(absolute.Width);
			empty.Height = GetRelativeHeight(absolute.Height);
			return empty;
		}

		public PointF GetRelativePoint(PointF absolute)
		{
			PointF empty = PointF.Empty;
			empty.X = GetRelativeX(absolute.X);
			empty.Y = GetRelativeY(absolute.Y);
			return empty;
		}

		public SizeF GetRelativeSize(SizeF size)
		{
			SizeF empty = SizeF.Empty;
			empty.Width = GetRelativeWidth(size.Width);
			empty.Height = GetRelativeHeight(size.Height);
			return empty;
		}

		internal float GetAbsoluteDimension(float relative)
		{
			if (width < height)
			{
				return GetAbsoluteWidth(relative);
			}
			return GetAbsoluteHeight(relative);
		}

		public PointF GetAbsolutePoint(PointF relative)
		{
			PointF empty = PointF.Empty;
			empty.X = GetAbsoluteX(relative.X);
			empty.Y = GetAbsoluteY(relative.Y);
			return empty;
		}

		public RectangleF GetAbsoluteRectangle(RectangleF relative)
		{
			RectangleF empty = RectangleF.Empty;
			empty.X = GetAbsoluteX(relative.X);
			empty.Y = GetAbsoluteY(relative.Y);
			empty.Width = GetAbsoluteWidth(relative.Width);
			empty.Height = GetAbsoluteHeight(relative.Height);
			return empty;
		}

		public SizeF GetAbsoluteSize(SizeF relative)
		{
			SizeF empty = SizeF.Empty;
			empty.Width = GetAbsoluteWidth(relative.Width);
			empty.Height = GetAbsoluteHeight(relative.Height);
			return empty;
		}

		internal RectangleF GetBorder3DAdjustedRect(Frame frameAttr)
		{
			RectangleF areasRect = new RectangleF(0f, 0f, 100f, 100f);
			if (frameAttr.FrameStyle != 0)
			{
				common.BorderTypeRegistry.GetBorderType(frameAttr.FrameStyle.ToString(CultureInfo.InvariantCulture))?.AdjustAreasPosition(this, ref areasRect);
			}
			RectangleF absoluteRectangle = GetAbsoluteRectangle(areasRect);
			absoluteRectangle.Inflate(-5f, -5f);
			absoluteRectangle.Height -= 3f;
			absoluteRectangle.Width -= 2f;
			return absoluteRectangle;
		}

		internal GraphicsPath CreateRoundedRectPath(RectangleF rect, float[] cornerRadius)
		{
			GraphicsPath graphicsPath = new GraphicsPath();
			graphicsPath.AddLine(rect.X + cornerRadius[0], rect.Y, rect.Right - cornerRadius[1], rect.Y);
			graphicsPath.AddArc(rect.Right - 2f * cornerRadius[1], rect.Y, 2f * cornerRadius[1], 2f * cornerRadius[2], 270f, 90f);
			graphicsPath.AddLine(rect.Right, rect.Y + cornerRadius[2], rect.Right, rect.Bottom - cornerRadius[3]);
			graphicsPath.AddArc(rect.Right - 2f * cornerRadius[4], rect.Bottom - 2f * cornerRadius[3], 2f * cornerRadius[4], 2f * cornerRadius[3], 0f, 90f);
			graphicsPath.AddLine(rect.Right - cornerRadius[4], rect.Bottom, rect.X + cornerRadius[5], rect.Bottom);
			graphicsPath.AddArc(rect.X, rect.Bottom - 2f * cornerRadius[6], 2f * cornerRadius[5], 2f * cornerRadius[6], 90f, 90f);
			graphicsPath.AddLine(rect.X, rect.Bottom - cornerRadius[6], rect.X, rect.Y + cornerRadius[7]);
			graphicsPath.AddArc(rect.X, rect.Y, 2f * cornerRadius[0], 2f * cornerRadius[7], 180f, 90f);
			return graphicsPath;
		}

		internal void DrawRoundedRectShadowAbs(RectangleF rect, float[] cornerRadius, float radius, Color centerColor, Color surroundColor, float shadowScale)
		{
			GraphicsPath graphicsPath = CreateRoundedRectPath(rect, cornerRadius);
			PathGradientBrush pathGradientBrush = new PathGradientBrush(graphicsPath);
			pathGradientBrush.CenterColor = centerColor;
			Color[] array2 = pathGradientBrush.SurroundColors = new Color[1]
			{
				surroundColor
			};
			pathGradientBrush.CenterPoint = new PointF(rect.X + rect.Width / 2f, rect.Y + rect.Height / 2f);
			PointF pointF2 = pathGradientBrush.FocusScales = new PointF(1f - shadowScale * radius / rect.Width, 1f - shadowScale * radius / rect.Height);
			FillPath(pathGradientBrush, graphicsPath);
			graphicsPath?.Dispose();
		}

		internal void Draw3DBorderRel(Frame borderSkin, RectangleF rect, Color borderColor, Color backColor)
		{
			Draw3DBorderAbs(borderSkin, GetAbsoluteRectangle(rect), borderColor, backColor);
		}

		internal void Draw3DBorderAbs(Frame borderSkin, RectangleF absRect, Color borderColor, Color backColor)
		{
			Draw3DBorderAbs(borderSkin, absRect, backColor, MapHatchStyle.None, "", MapImageWrapMode.Tile, Color.Empty, MapImageAlign.Center, GradientType.None, Color.Empty, borderColor, 1, MapDashStyle.Dot);
		}

		internal void Draw3DBorderRel(Frame borderSkin, RectangleF rect, Color backColor, MapHatchStyle backHatchStyle, string backImage, MapImageWrapMode backImageMode, Color backImageTranspColor, MapImageAlign backImageAlign, GradientType backGradientType, Color backSecondaryColor, Color borderColor, int borderWidth, MapDashStyle borderStyle)
		{
			Draw3DBorderAbs(borderSkin, GetAbsoluteRectangle(rect), backColor, backHatchStyle, backImage, backImageMode, backImageTranspColor, backImageAlign, backGradientType, backSecondaryColor, borderColor, borderWidth, borderStyle);
		}

		internal void Draw3DBorderAbs(Frame borderSkin, RectangleF absRect, Color backColor, MapHatchStyle backHatchStyle, string backImage, MapImageWrapMode backImageMode, Color backImageTranspColor, MapImageAlign backImageAlign, GradientType backGradientType, Color backSecondaryColor, Color borderColor, int borderWidth, MapDashStyle borderStyle)
		{
			if (common != null && borderSkin.FrameStyle != 0 && absRect.Width != 0f && absRect.Height != 0f)
			{
				IBorderType borderType = common.BorderTypeRegistry.GetBorderType(borderSkin.FrameStyle.ToString(CultureInfo.InvariantCulture));
				if (borderType != null && borderType.IsVisible(this))
				{
					borderType.DrawBorder(this, borderSkin, absRect, backColor, backHatchStyle, backImage, backImageMode, backImageTranspColor, backImageAlign, backGradientType, backSecondaryColor, borderColor, borderWidth, borderStyle);
				}
				else
				{
					FillRectangleAbs(absRect, backColor, backHatchStyle, "", MapImageWrapMode.Unscaled, Color.Empty, MapImageAlign.Center, backGradientType, backSecondaryColor, Color.Empty, 0, MapDashStyle.None, PenAlignment.Inset);
				}
			}
		}

		internal void DrawPieRel(RectangleF rect, float startAngle, float sweepAngle, Color backColor, MapHatchStyle backHatchStyle, string backImage, MapImageWrapMode backImageMode, Color backImageTranspColor, MapImageAlign backImageAlign, GradientType backGradientType, Color backSecondaryColor, Color borderColor, int borderWidth, MapDashStyle borderStyle, PenAlignment penAlignment, bool shadow, double shadowOffset, bool doughnut, float doughnutRadius, bool explodedShadow)
		{
			Pen pen = null;
			RectangleF absoluteRectangle = GetAbsoluteRectangle(rect);
			if ((double)doughnutRadius == 100.0)
			{
				doughnut = false;
			}
			if ((double)doughnutRadius == 0.0)
			{
				return;
			}
			Brush brush;
			if (backHatchStyle != 0)
			{
				brush = GetHatchBrush(backHatchStyle, backColor, backSecondaryColor);
			}
			else if (backSecondaryColor.IsEmpty || backGradientType == GradientType.None)
			{
				brush = ((string.IsNullOrEmpty(backImage) || backImageMode == MapImageWrapMode.Unscaled || backImageMode == MapImageWrapMode.Scaled) ? new SolidBrush(backColor) : GetTextureBrush(backImage, backImageTranspColor, backImageMode));
			}
			else if (backGradientType == GradientType.Center)
			{
				brush = GetPieGradientBrush(absoluteRectangle, backColor, backSecondaryColor);
			}
			else
			{
				GraphicsPath graphicsPath = new GraphicsPath();
				graphicsPath.AddPie(absoluteRectangle.X, absoluteRectangle.Y, absoluteRectangle.Width, absoluteRectangle.Height, startAngle, sweepAngle);
				brush = GetGradientBrush(graphicsPath.GetBounds(), backColor, backSecondaryColor, backGradientType);
				graphicsPath?.Dispose();
			}
			pen = new Pen(borderColor, borderWidth);
			pen.DashStyle = GetPenStyle(borderStyle);
			if (doughnut)
			{
				GraphicsPath graphicsPath2 = new GraphicsPath();
				graphicsPath2.AddArc(absoluteRectangle.X + absoluteRectangle.Width * doughnutRadius / 200f - 1f, absoluteRectangle.Y + absoluteRectangle.Height * doughnutRadius / 200f - 1f, absoluteRectangle.Width - absoluteRectangle.Width * doughnutRadius / 100f + 2f, absoluteRectangle.Height - absoluteRectangle.Height * doughnutRadius / 100f + 2f, startAngle, sweepAngle);
				graphicsPath2.AddArc(absoluteRectangle.X, absoluteRectangle.Y, absoluteRectangle.Width, absoluteRectangle.Height, startAngle + sweepAngle, 0f - sweepAngle);
				graphicsPath2.CloseFigure();
				FillPath(brush, graphicsPath2);
				if (!shadow)
				{
					DrawPath(pen, graphicsPath2);
				}
				graphicsPath2?.Dispose();
			}
			else
			{
				if (shadow && softShadows)
				{
					DrawPieSoftShadow(shadowOffset, startAngle, sweepAngle, explodedShadow, absoluteRectangle, backColor);
				}
				else
				{
					shadowDrawingMode = shadow;
					FillPie(brush, absoluteRectangle.X, absoluteRectangle.Y, absoluteRectangle.Width, absoluteRectangle.Height, startAngle, sweepAngle);
					shadowDrawingMode = false;
				}
				if (!shadow)
				{
					DrawPie(pen, absoluteRectangle.X, absoluteRectangle.Y, absoluteRectangle.Width, absoluteRectangle.Height, startAngle, sweepAngle);
				}
			}
			pen?.Dispose();
			brush?.Dispose();
		}

		private void DrawPieSoftShadow(double shadowOffset, float startAngle, float sweepAngle, bool explodedShadow, RectangleF absRect, Color backColor)
		{
			GraphicsPath graphicsPath = new GraphicsPath();
			graphicsPath.AddEllipse(absRect.X, absRect.Y, absRect.Width, absRect.Height);
			PathGradientBrush pathGradientBrush = new PathGradientBrush(graphicsPath);
			Color[] colors = new Color[3]
			{
				Color.FromArgb(0, backColor),
				Color.FromArgb(backColor.A, backColor),
				Color.FromArgb(backColor.A, backColor)
			};
			float[] positions = new float[3]
			{
				0f,
				0.05f,
				1f
			};
			ColorBlend colorBlend = new ColorBlend();
			colorBlend.Colors = colors;
			colorBlend.Positions = positions;
			pathGradientBrush.InterpolationColors = colorBlend;
			shadowDrawingMode = true;
			FillPie(pathGradientBrush, absRect.X, absRect.Y, absRect.Width, absRect.Height, startAngle, sweepAngle);
			shadowDrawingMode = false;
		}

		internal void DrawImageRel(string name, RectangleF position)
		{
			RectangleF absoluteRectangle = GetAbsoluteRectangle(position);
			Image image = common.ImageLoader.LoadImage(name);
			DrawImage(image, absoluteRectangle);
		}

		internal static RectangleF Round(RectangleF rect)
		{
			return new RectangleF((float)Math.Round(rect.X), (float)Math.Round(rect.Y), (float)Math.Round(rect.Width), (float)Math.Round(rect.Height));
		}

		internal void SetPictureSize(int width, int height)
		{
			this.width = width;
			this.height = height;
		}

		internal void CreateDrawRegion(RectangleF rect)
		{
			graphicStates.Push(new MapGraphState(Save(), width, height));
			RectangleF absoluteRectangle = GetAbsoluteRectangle(rect);
			if (base.Transform == null)
			{
				base.Transform = new Matrix();
			}
			TranslateTransform((float)Math.Round(absoluteRectangle.Location.X), (float)Math.Round(absoluteRectangle.Location.Y));
			SetPictureSize((int)Math.Round(absoluteRectangle.Size.Width), (int)Math.Round(absoluteRectangle.Size.Height));
		}

		internal void CreateContentDrawRegion(Viewport viewport, PointF gridSectionOffset)
		{
			graphicStates.Push(new MapGraphState(Save(), width, height));
			if (base.Transform == null)
			{
				base.Transform = new Matrix();
			}
			PointF contentOffsetInPixels = viewport.GetContentOffsetInPixels();
			contentOffsetInPixels.X += viewport.Margins.Left;
			contentOffsetInPixels.Y += viewport.Margins.Top;
			SizeF contentSizeInPixels = viewport.GetContentSizeInPixels();
			contentOffsetInPixels.X -= gridSectionOffset.X;
			contentOffsetInPixels.Y -= gridSectionOffset.Y;
			TranslateTransform(contentOffsetInPixels.X, contentOffsetInPixels.Y);
			SetPictureSize((int)(contentSizeInPixels.Width * viewport.Zoom / 100f), (int)(contentSizeInPixels.Height * viewport.Zoom / 100f));
		}

		internal void RestoreDrawRegion()
		{
			MapGraphState mapGraphState = (MapGraphState)graphicStates.Pop();
			Restore(mapGraphState.state);
			SetPictureSize(mapGraphState.width, mapGraphState.height);
		}

		public override void Close()
		{
			common.Graph = null;
			base.Close();
		}

		internal void Dispose()
		{
			if (pen != null)
			{
				pen.Dispose();
			}
			if (solidBrush != null)
			{
				solidBrush.Dispose();
			}
		}

		internal new void SetClip(RectangleF region)
		{
			base.SetClip(GetAbsoluteRectangle(region));
		}

		internal static Color GetGradientColor(Color beginColor, Color endColor, double dPosition)
		{
			if (dPosition < 0.0 || dPosition > 1.0 || double.IsNaN(dPosition))
			{
				return beginColor;
			}
			int r = beginColor.R;
			int g = beginColor.G;
			int b = beginColor.B;
			int r2 = endColor.R;
			int g2 = endColor.G;
			int b2 = endColor.B;
			double num = (double)r + (double)(r2 - r) * dPosition;
			double num2 = (double)g + (double)(g2 - g) * dPosition;
			double num3 = (double)b + (double)(b2 - b) * dPosition;
			if (num > 255.0)
			{
				num = 255.0;
			}
			if (num < 0.0)
			{
				num = 0.0;
			}
			if (num2 > 255.0)
			{
				num2 = 255.0;
			}
			if (num2 < 0.0)
			{
				num2 = 0.0;
			}
			if (num3 > 255.0)
			{
				num3 = 255.0;
			}
			if (num3 < 0.0)
			{
				num3 = 0.0;
			}
			return Color.FromArgb(beginColor.A, (int)num, (int)num2, (int)num3);
		}

		internal Pen GetSelectionPen(bool designTimeSelection, Color borderColor)
		{
			Pen pen = null;
			if (designTimeSelection)
			{
				pen = new Pen(Color.Black, 1f);
				pen.DashStyle = DashStyle.Dot;
				pen.DashPattern = new float[2]
				{
					2f,
					2f
				};
				pen.Width = 1f / ScaleFactorX;
			}
			else
			{
				pen = new Pen(borderColor, 1f);
				pen.DashStyle = DashStyle.Dot;
				pen.DashPattern = new float[2]
				{
					2f,
					2f
				};
			}
			return pen;
		}

		internal Brush GetDesignTimeSelectionFillBrush()
		{
			return new SolidBrush(Color.White);
		}

		internal Pen GetDesignTimeSelectionBorderPen()
		{
			return new Pen(Color.Black)
			{
				Width = 1f / ScaleFactorX
			};
		}

		internal void DrawSelection(RectangleF rect, bool designTimeSelection, Color borderColor, Color markerColor)
		{
			DrawSelection(rect, 3f / ScaleFactorX, designTimeSelection, borderColor, markerColor);
		}

		internal void DrawSelection(RectangleF rect, float inflateBy, bool designTimeSelection, Color borderColor, Color markerColor)
		{
			rect.Inflate(inflateBy, inflateBy);
			RectangleF visibleClipBounds = Graphics.VisibleClipBounds;
			visibleClipBounds.Inflate(-3f / ScaleFactorX, -3f / ScaleFactorY);
			visibleClipBounds.Width -= 1f;
			visibleClipBounds.Height -= 1f;
			rect = RectangleF.Intersect(rect, visibleClipBounds);
			PointF pointF = new PointF(rect.X + rect.Width / 2f, rect.Y + rect.Height / 2f);
			using (Pen pen = GetSelectionPen(designTimeSelection, borderColor))
			{
				DrawLine(pen, new PointF(rect.Left, rect.Top), new PointF(rect.Right, rect.Top));
				DrawLine(pen, new PointF(rect.Left, rect.Bottom), new PointF(rect.Right, rect.Bottom));
				DrawLine(pen, new PointF(rect.Left, rect.Top), new PointF(rect.Left, rect.Bottom));
				DrawLine(pen, new PointF(rect.Right, rect.Top), new PointF(rect.Right, rect.Bottom));
			}
			ArrayList arrayList = new ArrayList();
			arrayList.Add(new PointF(rect.X, rect.Y));
			if (rect.Width >= 20f)
			{
				arrayList.Add(new PointF(pointF.X, rect.Y));
			}
			arrayList.Add(new PointF(rect.X + rect.Width, rect.Y));
			if (rect.Height >= 20f)
			{
				arrayList.Add(new PointF(rect.X, pointF.Y));
			}
			if (rect.Height >= 20f)
			{
				arrayList.Add(new PointF(rect.X + rect.Width, pointF.Y));
			}
			arrayList.Add(new PointF(rect.X, rect.Y + rect.Height));
			if (rect.Width >= 20f)
			{
				arrayList.Add(new PointF(pointF.X, rect.Y + rect.Height));
			}
			arrayList.Add(new PointF(rect.X + rect.Width, rect.Y + rect.Height));
			DrawSelectionMarkers((PointF[])arrayList.ToArray(typeof(PointF)), designTimeSelection, borderColor, markerColor);
		}

		internal void DrawSelectionMarkers(PointF[] markerPositions, bool designTimeSelection, Color borderColor, Color markerColor)
		{
			float num = 6f / ScaleFactorX;
			float num2 = 6f / ScaleFactorY;
			Brush brush;
			Pen pen;
			if (designTimeSelection)
			{
				brush = GetDesignTimeSelectionFillBrush();
				pen = GetDesignTimeSelectionBorderPen();
			}
			else
			{
				brush = new SolidBrush(markerColor);
				pen = new Pen(borderColor, 1f);
			}
			for (int i = 0; i < markerPositions.Length; i++)
			{
				FillEllipse(brush, new RectangleF(markerPositions[i].X - num / 2f, markerPositions[i].Y - num2 / 2f, num, num2));
				DrawEllipse(pen, new RectangleF(markerPositions[i].X - num / 2f, markerPositions[i].Y - num2 / 2f, num, num2));
			}
			brush?.Dispose();
			pen?.Dispose();
		}
	}
}
