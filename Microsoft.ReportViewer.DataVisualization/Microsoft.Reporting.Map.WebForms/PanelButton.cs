using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Microsoft.Reporting.Map.WebForms
{
	internal class PanelButton : MapObject, IToolTipProvider
	{
		private const int ButtonPadding = 4;

		private EventHandler clickEventHandler;

		private PanelButtonStyle style;

		private PanelButtonType type;

		private RectangleF bounds = RectangleF.Empty;

		private Color borderColor = Color.DarkGray;

		private Color backColor = Color.Gray;

		private Color symbolBorderColor = Color.Black;

		private Color symbolColor = Color.White;

		private double intitalAutoRepeatDelay = 1000.0;

		private double autoRepeatDelay = 500.0;

		public PanelButtonStyle Style
		{
			get
			{
				return style;
			}
			set
			{
				style = value;
			}
		}

		public PanelButtonType Type
		{
			get
			{
				return type;
			}
			set
			{
				type = value;
			}
		}

		public RectangleF Bounds
		{
			get
			{
				return bounds;
			}
			set
			{
				bounds = value;
			}
		}

		public PointF Location
		{
			get
			{
				return bounds.Location;
			}
			set
			{
				bounds.Location = value;
			}
		}

		public SizeF Size
		{
			get
			{
				return bounds.Size;
			}
			set
			{
				bounds.Size = value;
			}
		}

		public Color BorderColor
		{
			get
			{
				return borderColor;
			}
			set
			{
				borderColor = value;
			}
		}

		public Color BackColor
		{
			get
			{
				return backColor;
			}
			set
			{
				backColor = value;
			}
		}

		public Color SymbolBorderColor
		{
			get
			{
				return symbolBorderColor;
			}
			set
			{
				symbolBorderColor = value;
			}
		}

		public Color SymbolColor
		{
			get
			{
				return symbolColor;
			}
			set
			{
				symbolColor = value;
			}
		}

		public double InititalAutoRepeatDelay
		{
			get
			{
				return intitalAutoRepeatDelay;
			}
			set
			{
				intitalAutoRepeatDelay = value;
			}
		}

		public double AutoRepeatDelay
		{
			get
			{
				return autoRepeatDelay;
			}
			set
			{
				autoRepeatDelay = value;
			}
		}

		public PanelButton(object parent)
			: this(parent, PanelButtonType.Unknown, PanelButtonStyle.Rectangle, null)
		{
		}

		public PanelButton(object parent, PanelButtonType type, EventHandler clickHandler)
			: this(parent, type, PanelButtonStyle.Rectangle, clickHandler)
		{
		}

		public PanelButton(object parent, PanelButtonType buttonType, PanelButtonStyle buttonStyle, EventHandler clickHandler)
			: base(parent)
		{
			Parent = parent;
			Type = buttonType;
			Style = buttonStyle;
			clickEventHandler = clickHandler;
		}

		public void Render(MapGraphics g)
		{
			using (GraphicsPath graphicsPath = GetButtonPath(g))
			{
				g.DrawPathAbs(graphicsPath, BackColor, MapHatchStyle.None, string.Empty, MapImageWrapMode.Unscaled, Color.Black, MapImageAlign.Center, GradientType.None, Color.White, BorderColor, 1, MapDashStyle.Solid, PenAlignment.Center);
				if (Common != null)
				{
					Common.MapCore.HotRegionList.SetHotRegion(g, this, graphicsPath);
				}
			}
			using (GraphicsPath path = GetButtonFacePath(g))
			{
				g.DrawPathAbs(path, SymbolColor, MapHatchStyle.None, string.Empty, MapImageWrapMode.Unscaled, Color.Black, MapImageAlign.Center, GradientType.None, Color.White, SymbolBorderColor, 1, MapDashStyle.Solid, PenAlignment.Center);
			}
		}

		private GraphicsPath GetButtonPath(MapGraphics g)
		{
			GraphicsPath graphicsPath = new GraphicsPath();
			switch (Style)
			{
			case PanelButtonStyle.Rectangle:
				graphicsPath.AddRectangle(g.GetAbsoluteRectangle(Bounds));
				break;
			case PanelButtonStyle.Circle:
				graphicsPath.AddEllipse(g.GetAbsoluteRectangle(Bounds));
				break;
			case PanelButtonStyle.RoundedRectangle:
			{
				RectangleF absoluteRectangle = g.GetAbsoluteRectangle(Bounds);
				if (absoluteRectangle.Width < 1f || absoluteRectangle.Height < 1f)
				{
					return graphicsPath;
				}
				float num = absoluteRectangle.Width / 8f;
				float[] cornerRadius = new float[8]
				{
					num,
					num,
					num,
					num,
					num,
					num,
					num,
					num
				};
				graphicsPath.AddPath(g.CreateRoundedRectPath(absoluteRectangle, cornerRadius), connect: false);
				break;
			}
			case PanelButtonStyle.Triangle:
				switch (Type)
				{
				case PanelButtonType.NavigationButton:
					graphicsPath.AddLines(new PointF[3]
					{
						g.GetAbsolutePoint(new PointF(Bounds.Left, Bounds.Bottom)),
						g.GetAbsolutePoint(new PointF((Bounds.Left + Bounds.Right) / 2f, Bounds.Top)),
						g.GetAbsolutePoint(new PointF(Bounds.Right, Bounds.Bottom))
					});
					graphicsPath.CloseAllFigures();
					break;
				case PanelButtonType.NaviagateSouth:
					graphicsPath.AddLines(new PointF[3]
					{
						g.GetAbsolutePoint(new PointF(Bounds.Left, Bounds.Top)),
						g.GetAbsolutePoint(new PointF((Bounds.Left + Bounds.Right) / 2f, Bounds.Bottom)),
						g.GetAbsolutePoint(new PointF(Bounds.Right, Bounds.Top))
					});
					graphicsPath.CloseAllFigures();
					break;
				case PanelButtonType.NaviagateEast:
					graphicsPath.AddLines(new PointF[3]
					{
						g.GetAbsolutePoint(new PointF(Bounds.Left, Bounds.Top)),
						g.GetAbsolutePoint(new PointF(Bounds.Right, (Bounds.Top + Bounds.Bottom) / 2f)),
						g.GetAbsolutePoint(new PointF(Bounds.Left, Bounds.Bottom))
					});
					graphicsPath.CloseAllFigures();
					break;
				case PanelButtonType.NaviagateWest:
					graphicsPath.AddLines(new PointF[3]
					{
						g.GetAbsolutePoint(new PointF(Bounds.Right, Bounds.Top)),
						g.GetAbsolutePoint(new PointF(Bounds.Left, (Bounds.Top + Bounds.Bottom) / 2f)),
						g.GetAbsolutePoint(new PointF(Bounds.Right, Bounds.Bottom))
					});
					graphicsPath.CloseAllFigures();
					break;
				}
				break;
			}
			return graphicsPath;
		}

		private GraphicsPath GetButtonFacePath(MapGraphics g)
		{
			GraphicsPath graphicsPath = new GraphicsPath();
			RectangleF rectangleF = RectangleF.Inflate(g.GetAbsoluteRectangle(Bounds), -4f, -4f);
			if (rectangleF.Width < 1f || rectangleF.Height < 1f)
			{
				return graphicsPath;
			}
			float num = rectangleF.Width / 8f;
			PointF pointF = new PointF((rectangleF.Left + rectangleF.Right) / 2f, rectangleF.Top);
			PointF pointF2 = new PointF((rectangleF.Left + rectangleF.Right) / 2f, rectangleF.Bottom);
			PointF pointF3 = new PointF(rectangleF.Left, (rectangleF.Top + rectangleF.Bottom) / 2f);
			PointF pointF4 = new PointF(rectangleF.Right, (rectangleF.Top + rectangleF.Bottom) / 2f);
			PointF pointF5 = new PointF((rectangleF.Left + rectangleF.Right) / 2f, (rectangleF.Top + rectangleF.Bottom) / 2f);
			if (Type == PanelButtonType.ZoomButton)
			{
				graphicsPath.AddLines(new PointF[12]
				{
					new PointF(pointF3.X, pointF3.Y - num),
					new PointF(pointF5.X - num, pointF5.Y - num),
					new PointF(pointF.X - num, pointF.Y),
					new PointF(pointF.X + num, pointF.Y),
					new PointF(pointF5.X + num, pointF5.Y - num),
					new PointF(pointF4.X, pointF4.Y - num),
					new PointF(pointF4.X, pointF4.Y + num),
					new PointF(pointF5.X + num, pointF5.Y + num),
					new PointF(pointF2.X + num, pointF2.Y),
					new PointF(pointF2.X - num, pointF2.Y),
					new PointF(pointF5.X - num, pointF5.Y + num),
					new PointF(pointF3.X, pointF3.Y + num)
				});
				graphicsPath.CloseAllFigures();
			}
			else if (Type == PanelButtonType.ZoomOut)
			{
				graphicsPath.AddLines(new PointF[4]
				{
					new PointF(pointF3.X, pointF3.Y - num),
					new PointF(pointF4.X, pointF3.Y - num),
					new PointF(pointF4.X, pointF3.Y + num),
					new PointF(pointF3.X, pointF3.Y + num)
				});
				graphicsPath.CloseAllFigures();
			}
			else if ((Type & PanelButtonType.NavigationButton) != 0)
			{
				CreateArrowPath(g, graphicsPath);
			}
			return graphicsPath;
		}

		private void CreateArrowPath(MapGraphics g, GraphicsPath path)
		{
			if (Type != PanelButtonType.NaviagateCenter && (Style == PanelButtonStyle.Rectangle || Style == PanelButtonStyle.RoundedRectangle))
			{
				RectangleF rectangleF = RectangleF.Inflate(g.GetAbsoluteRectangle(Bounds), -4f, -4f);
				float left = rectangleF.Left;
				float right = rectangleF.Right;
				float x = rectangleF.X + rectangleF.Width / 2f;
				float x2 = rectangleF.X + rectangleF.Width * 0.3f;
				float x3 = rectangleF.X + rectangleF.Width * 0.7f;
				float top = rectangleF.Top;
				float bottom = rectangleF.Bottom;
				float y = rectangleF.Y + rectangleF.Height * 0.7f;
				float y2 = rectangleF.Y + rectangleF.Height / 2f;
				path.StartFigure();
				path.AddLines(new PointF[7]
				{
					new PointF(x, top),
					new PointF(right, y),
					new PointF(x3, y),
					new PointF(x3, bottom),
					new PointF(x2, bottom),
					new PointF(x2, y),
					new PointF(left, y)
				});
				path.CloseAllFigures();
				Matrix matrix = new Matrix();
				switch (Type)
				{
				case PanelButtonType.NaviagateSouth:
					matrix.RotateAt(180f, new PointF(x, y2));
					break;
				case PanelButtonType.NaviagateEast:
					matrix.RotateAt(90f, new PointF(x, y2));
					break;
				case PanelButtonType.NaviagateWest:
					matrix.RotateAt(270f, new PointF(x, y2));
					break;
				}
				path.Transform(matrix);
			}
		}

		internal void DoClick()
		{
			if (clickEventHandler != null)
			{
				clickEventHandler(this, EventArgs.Empty);
			}
		}

		public string GetToolTip()
		{
			IToolTipProvider toolTipProvider = Parent as IToolTipProvider;
			if (toolTipProvider != null)
			{
				return toolTipProvider.GetToolTip();
			}
			return string.Empty;
		}
	}
}
