using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;

namespace Microsoft.Reporting.Gauge.WebForms
{
	[TypeConverter(typeof(CircularPointerConverter))]
	internal class CircularPointer : PointerBase, ISelectable
	{
		private XamlRenderer xamlRenderer;

		private CircularPointerType type;

		private NeedleStyle needleStyle;

		private bool capVisible = true;

		private bool capOnTop = true;

		private bool capReflection;

		private float capWidth = 26f;

		private string capImage = "";

		private Color capImageTransColor = Color.Empty;

		private Color capImageHueColor = Color.Empty;

		private Point capImageOrigin = Point.Empty;

		private Placement placement = Placement.Cross;

		private CapStyle capStyle;

		private Color capFillColor = Color.Gainsboro;

		private GradientType capFillGradientType = GradientType.DiagonalLeft;

		private Color capFillGradientEndColor = Color.DimGray;

		private GaugeHatchStyle capFillHatchStyle;

		private bool selected;

		private GraphicsPath[] hotRegions = new GraphicsPath[2];

		[SRCategory("CategoryTypeSpecific")]
		[SRDescription("DescriptionAttributeCircularPointer_Type")]
		[ParenthesizePropertyName(true)]
		[RefreshProperties(RefreshProperties.All)]
		[DefaultValue(CircularPointerType.Needle)]
		public CircularPointerType Type
		{
			get
			{
				return type;
			}
			set
			{
				type = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryTypeSpecific")]
		[SRDescription("DescriptionAttributeCircularPointer_NeedleStyle")]
		[DefaultValue(NeedleStyle.Style1)]
		public NeedleStyle NeedleStyle
		{
			get
			{
				return needleStyle;
			}
			set
			{
				needleStyle = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryPointerCap")]
		[SRDescription("DescriptionAttributeCircularPointer_CapVisible")]
		[ParenthesizePropertyName(true)]
		[DefaultValue(true)]
		public bool CapVisible
		{
			get
			{
				return capVisible;
			}
			set
			{
				capVisible = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryPointerCap")]
		[SRDescription("DescriptionAttributeCircularPointer_CapOnTop")]
		[DefaultValue(true)]
		public bool CapOnTop
		{
			get
			{
				return capOnTop;
			}
			set
			{
				capOnTop = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryPointerCap")]
		[SRDescription("DescriptionAttributeCircularPointer_CapReflection")]
		[DefaultValue(false)]
		public bool CapReflection
		{
			get
			{
				return capReflection;
			}
			set
			{
				capReflection = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryPointerCap")]
		[SRDescription("DescriptionAttributeCircularPointer_CapWidth")]
		[ValidateBound(0.0, 100.0)]
		[DefaultValue(26f)]
		public float CapWidth
		{
			get
			{
				return capWidth;
			}
			set
			{
				if (value < 0f || value > 1000f)
				{
					throw new ArgumentException(Utils.SRGetStr("ExceptionMustInRange", 0, 1000));
				}
				capWidth = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryImage")]
		[SRDescription("DescriptionAttributeCircularPointer_CapImage")]
		[DefaultValue("")]
		public string CapImage
		{
			get
			{
				return capImage;
			}
			set
			{
				capImage = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryImage")]
		[SRDescription("DescriptionAttributeCircularPointer_CapImageTransColor")]
		[DefaultValue(typeof(Color), "")]
		public Color CapImageTransColor
		{
			get
			{
				return capImageTransColor;
			}
			set
			{
				capImageTransColor = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryImage")]
		[SRDescription("DescriptionAttributeCapImageHueColor")]
		[DefaultValue(typeof(Color), "")]
		public Color CapImageHueColor
		{
			get
			{
				return capImageHueColor;
			}
			set
			{
				capImageHueColor = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryImage")]
		[SRDescription("DescriptionAttributeCircularPointer_CapImageOrigin")]
		[TypeConverter(typeof(EmptyPointConverter))]
		[DefaultValue(typeof(Point), "0, 0")]
		public Point CapImageOrigin
		{
			get
			{
				return capImageOrigin;
			}
			set
			{
				capImageOrigin = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryLayout")]
		[SRDescription("DescriptionAttributePlacement")]
		[DefaultValue(Placement.Cross)]
		public Placement Placement
		{
			get
			{
				return placement;
			}
			set
			{
				placement = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryLayout")]
		[SRDescription("DescriptionAttributeWidth")]
		[ValidateBound(0.0, 30.0)]
		[DefaultValue(15f)]
		public override float Width
		{
			get
			{
				return base.Width;
			}
			set
			{
				if (value < 0f || value > 1000f)
				{
					throw new ArgumentException(Utils.SRGetStr("ExceptionMustInRange", 0, 1000));
				}
				base.Width = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryTypeSpecific")]
		[SRDescription("DescriptionAttributeMarkerStyle4")]
		[DefaultValue(MarkerStyle.Diamond)]
		public override MarkerStyle MarkerStyle
		{
			get
			{
				return base.MarkerStyle;
			}
			set
			{
				base.MarkerStyle = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryTypeSpecific")]
		[SRDescription("DescriptionAttributeMarkerLength4")]
		[ValidateBound(0.0, 50.0)]
		[DefaultValue(10f)]
		public override float MarkerLength
		{
			get
			{
				return base.MarkerLength;
			}
			set
			{
				if (value < 0f || value > 1000f)
				{
					throw new ArgumentException(Utils.SRGetStr("ExceptionMustInRange", 0, 1000));
				}
				base.MarkerLength = value;
			}
		}

		[SRCategory("CategoryAppearance")]
		[SRDescription("DescriptionAttributeFillGradientType6")]
		[DefaultValue(GradientType.LeftRight)]
		public override GradientType FillGradientType
		{
			get
			{
				return base.FillGradientType;
			}
			set
			{
				base.FillGradientType = value;
			}
		}

		[SRCategory("CategoryPointerCap")]
		[SRDescription("DescriptionAttributeCircularPointer_CapStyle")]
		[ParenthesizePropertyName(true)]
		[RefreshProperties(RefreshProperties.All)]
		[DefaultValue(CapStyle.Simple)]
		public CapStyle CapStyle
		{
			get
			{
				return capStyle;
			}
			set
			{
				capStyle = value;
				ResetCachedXamlRenderer();
				Invalidate();
			}
		}

		[SRCategory("CategoryPointerCap")]
		[SRDescription("DescriptionAttributeCircularPointer_CapFillColor")]
		[DefaultValue(typeof(Color), "Gainsboro")]
		public Color CapFillColor
		{
			get
			{
				return capFillColor;
			}
			set
			{
				capFillColor = value;
				ResetCachedXamlRenderer();
				Invalidate();
			}
		}

		[SRCategory("CategoryPointerCap")]
		[SRDescription("DescriptionAttributeCircularPointer_CapFillGradientType")]
		[DefaultValue(GradientType.DiagonalLeft)]
		public GradientType CapFillGradientType
		{
			get
			{
				return capFillGradientType;
			}
			set
			{
				capFillGradientType = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryPointerCap")]
		[SRDescription("DescriptionAttributeCircularPointer_CapFillGradientEndColor")]
		[DefaultValue(typeof(Color), "DimGray")]
		public Color CapFillGradientEndColor
		{
			get
			{
				return capFillGradientEndColor;
			}
			set
			{
				capFillGradientEndColor = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryPointerCap")]
		[SRDescription("DescriptionAttributeCircularPointer_CapFillHatchStyle")]
		[DefaultValue(GaugeHatchStyle.None)]
		public GaugeHatchStyle CapFillHatchStyle
		{
			get
			{
				return capFillHatchStyle;
			}
			set
			{
				capFillHatchStyle = value;
				Invalidate();
			}
		}

		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[SRCategory("CategoryBehavior")]
		[SRDescription("DescriptionAttributeCursor")]
		[DefaultValue(GaugeCursor.Default)]
		public override GaugeCursor Cursor
		{
			get
			{
				return base.Cursor;
			}
			set
			{
				base.Cursor = value;
			}
		}

		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[SRCategory("CategoryBehavior")]
		[SRDescription("DescriptionAttributeDampeningEnabled3")]
		[DefaultValue(false)]
		public override bool DampeningEnabled
		{
			get
			{
				return base.DampeningEnabled;
			}
			set
			{
				base.DampeningEnabled = value;
			}
		}

		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[SRCategory("CategoryAppearance")]
		[SRDescription("DescriptionAttributeSelected")]
		[DefaultValue(false)]
		public bool Selected
		{
			get
			{
				return selected;
			}
			set
			{
				selected = value;
				Invalidate();
			}
		}

		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[SerializationVisibility(SerializationVisibility.Hidden)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool NeedleCapVisible
		{
			get
			{
				return CapVisible;
			}
			set
			{
				CapVisible = value;
			}
		}

		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[SerializationVisibility(SerializationVisibility.Hidden)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool NeedleCapOnTop
		{
			get
			{
				return CapOnTop;
			}
			set
			{
				CapOnTop = value;
			}
		}

		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[SerializationVisibility(SerializationVisibility.Hidden)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public float NeedleCapWidth
		{
			get
			{
				return CapWidth;
			}
			set
			{
				CapWidth = value;
			}
		}

		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[SerializationVisibility(SerializationVisibility.Hidden)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public string NeedleCapImage
		{
			get
			{
				return CapImage;
			}
			set
			{
				CapImage = value;
			}
		}

		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[SerializationVisibility(SerializationVisibility.Hidden)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public Color NeedleCapImageTransColor
		{
			get
			{
				return CapImageTransColor;
			}
			set
			{
				CapImageTransColor = value;
			}
		}

		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[SerializationVisibility(SerializationVisibility.Hidden)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public Point NeedleCapImageOrigin
		{
			get
			{
				return CapImageOrigin;
			}
			set
			{
				CapImageOrigin = value;
			}
		}

		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[SerializationVisibility(SerializationVisibility.Hidden)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public Color NeedleCapFillColor
		{
			get
			{
				return CapFillColor;
			}
			set
			{
				CapFillColor = value;
			}
		}

		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[SerializationVisibility(SerializationVisibility.Hidden)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public GradientType NeedleCapFillGradientType
		{
			get
			{
				return CapFillGradientType;
			}
			set
			{
				CapFillGradientType = value;
			}
		}

		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[SerializationVisibility(SerializationVisibility.Hidden)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public Color NeedleCapFillGradientEndColor
		{
			get
			{
				return CapFillGradientEndColor;
			}
			set
			{
				CapFillGradientEndColor = value;
			}
		}

		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[SerializationVisibility(SerializationVisibility.Hidden)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public GaugeHatchStyle NeedleCapFillHatchStyle
		{
			get
			{
				return CapFillHatchStyle;
			}
			set
			{
				CapFillHatchStyle = value;
			}
		}

		public CircularPointer()
			: base(MarkerStyle.Diamond, 10f, 15f, GradientType.LeftRight)
		{
		}

		internal Brush GetNeedleFillBrush(GaugeGraphics g, bool primary, GraphicsPath path, PointF pointOrigin, float angle)
		{
			Brush brush = null;
			if (primary)
			{
				if (FillHatchStyle != 0)
				{
					brush = GaugeGraphics.GetHatchBrush(FillHatchStyle, FillColor, FillGradientEndColor);
				}
				else if (FillGradientType != 0)
				{
					brush = g.GetGradientBrush(path.GetBounds(), FillColor, FillGradientEndColor, FillGradientType);
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
					brush = new SolidBrush(FillColor);
				}
			}
			else if (CapFillHatchStyle != 0)
			{
				brush = GaugeGraphics.GetHatchBrush(CapFillHatchStyle, CapFillColor, CapFillGradientEndColor);
			}
			else if (CapFillGradientType != 0)
			{
				RectangleF bounds = path.GetBounds();
				if (CapFillGradientType == GradientType.DiagonalLeft)
				{
					brush = g.GetGradientBrush(bounds, CapFillColor, CapFillGradientEndColor, GradientType.LeftRight);
					using (Matrix matrix = new Matrix())
					{
						matrix.RotateAt(45f, new PointF(bounds.X + bounds.Width / 2f, bounds.Y + bounds.Height / 2f));
						((LinearGradientBrush)brush).Transform = matrix;
					}
				}
				else if (CapFillGradientType == GradientType.DiagonalRight)
				{
					brush = g.GetGradientBrush(bounds, CapFillColor, CapFillGradientEndColor, GradientType.TopBottom);
					using (Matrix matrix2 = new Matrix())
					{
						matrix2.RotateAt(135f, new PointF(bounds.X + bounds.Width / 2f, bounds.Y + bounds.Height / 2f));
						((LinearGradientBrush)brush).Transform = matrix2;
					}
				}
				else if (CapFillGradientType == GradientType.Center)
				{
					bounds.Inflate(1f, 1f);
					using (GraphicsPath graphicsPath = new GraphicsPath())
					{
						graphicsPath.AddArc(bounds, 0f, 360f);
						PathGradientBrush pathGradientBrush = new PathGradientBrush(graphicsPath);
						pathGradientBrush.CenterColor = CapFillColor;
						pathGradientBrush.CenterPoint = new PointF(bounds.X + bounds.Width / 2f, bounds.Y + bounds.Height / 2f);
						pathGradientBrush.SurroundColors = new Color[1]
						{
							CapFillGradientEndColor
						};
						brush = pathGradientBrush;
					}
				}
				else
				{
					brush = g.GetGradientBrush(path.GetBounds(), CapFillColor, CapFillGradientEndColor, CapFillGradientType);
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
				brush = new SolidBrush(CapFillColor);
			}
			return brush;
		}

		internal NeedleStyleAttrib GetNeedleStyleAttrib(GaugeGraphics g, PointF pointOrigin, float angle)
		{
			NeedleStyleAttrib needleStyleAttrib = new NeedleStyleAttrib();
			needleStyleAttrib.primaryPath = new GraphicsPath();
			if (CapVisible && CapWidth > 0f)
			{
				needleStyleAttrib.secondaryPath = new GraphicsPath();
			}
			if (needleStyleAttrib.primaryPath == null && needleStyleAttrib.secondaryPath == null)
			{
				return needleStyleAttrib;
			}
			float relative = (Placement == Placement.Cross) ? (GetScale().GetRadius() - DistanceFromScale) : ((Placement != 0) ? (GetScale().GetRadius() + GetScale().Width / 2f + DistanceFromScale) : (GetScale().GetRadius() - GetScale().Width / 2f - DistanceFromScale));
			relative = g.GetAbsoluteDimension(relative);
			float width = Width;
			width = g.GetAbsoluteDimension(width);
			float relative2 = CapWidth;
			relative2 = g.GetAbsoluteDimension(relative2);
			if (needleStyleAttrib.primaryPath != null)
			{
				PointF[] points = new PointF[0];
				switch (NeedleStyle)
				{
				case NeedleStyle.Style1:
					points = new PointF[3]
					{
						new PointF((0f - width) / 2f, (0f - width) / 2f),
						new PointF(width / 2f, (0f - width) / 2f),
						new PointF(0f, relative)
					};
					break;
				case NeedleStyle.Style2:
				{
					float num14 = relative / 1.618034f;
					num14 = num14 * 1.618034f - num14;
					points = new PointF[4]
					{
						new PointF((0f - width) / 2f, 0f - num14),
						new PointF(width / 2f, 0f - num14),
						new PointF(width / 2f, relative),
						new PointF((0f - width) / 2f, relative)
					};
					break;
				}
				case NeedleStyle.Style3:
				{
					float num13 = relative / 1.618034f;
					num13 = num13 * 1.618034f - num13;
					points = new PointF[5]
					{
						new PointF((0f - width) / 2f, 0f - num13),
						new PointF(width / 2f, 0f - num13),
						new PointF(width / 4f, relative - width),
						new PointF(0f, relative),
						new PointF((0f - width) / 4f, relative - width)
					};
					break;
				}
				case NeedleStyle.Style4:
					points = new PointF[5]
					{
						new PointF((0f - width) / 2f, 0f),
						new PointF(width / 2f, 0f),
						new PointF(width / 3f, relative - width),
						new PointF(0f, relative),
						new PointF((0f - width) / 3f, relative - width)
					};
					break;
				case NeedleStyle.Style5:
				{
					float num11 = relative / 1.618034f;
					num11 = num11 * 1.618034f - num11;
					float num12 = width / 1.618034f;
					points = new PointF[7]
					{
						new PointF((0f - num12) / 2f, 0f - num11),
						new PointF(num12 / 2f, 0f - num11),
						new PointF(num12 / 2f, relative - width),
						new PointF(width / 2f, relative - width),
						new PointF(0f, relative),
						new PointF((0f - width) / 2f, relative - width),
						new PointF((0f - num12) / 2f, relative - width)
					};
					break;
				}
				case NeedleStyle.Style6:
				{
					float num9 = relative / 1.618034f;
					num9 = num9 * 1.618034f - num9;
					float num10 = width / 1.618034f;
					points = new PointF[7]
					{
						new PointF((0f - num10) / 2f, 0f),
						new PointF(num10 / 2f, 0f),
						new PointF(num10 / 2f, relative - width),
						new PointF(width / 2f, relative - width),
						new PointF(0f, relative),
						new PointF((0f - width) / 2f, relative - width),
						new PointF((0f - num10) / 2f, relative - width)
					};
					break;
				}
				case NeedleStyle.Style7:
				{
					float num7 = relative / 1.618034f;
					num7 = num7 * 1.618034f - num7;
					float num8 = width / 1.618034f;
					points = new PointF[7]
					{
						new PointF((0f - num8) / 2f, 0f - num7),
						new PointF(num8 / 2f, 0f - num7),
						new PointF(num8 / 2f, relative - width),
						new PointF(width / 2f, relative - width - width / 8f),
						new PointF(0f, relative),
						new PointF((0f - width) / 2f, relative - width - width / 8f),
						new PointF((0f - num8) / 2f, relative - width)
					};
					break;
				}
				case NeedleStyle.Style8:
				{
					float num6 = width / 1.618034f;
					points = new PointF[7]
					{
						new PointF((0f - num6) / 2f, 0f),
						new PointF(num6 / 2f, 0f),
						new PointF(num6 / 2f, relative - width),
						new PointF(width / 2f, relative - width - width / 8f),
						new PointF(0f, relative),
						new PointF((0f - width) / 2f, relative - width - width / 8f),
						new PointF((0f - num6) / 2f, relative - width)
					};
					break;
				}
				case NeedleStyle.Style9:
				{
					float num4 = relative / 1.618034f;
					num4 = num4 * 1.618034f - num4;
					float num5 = width / 1.618034f;
					points = new PointF[8]
					{
						new PointF((0f - width) / 2f, 0f - num4),
						new PointF(0f, 0f - num4 + num5 / 2f),
						new PointF(width / 2f, 0f - num4),
						new PointF(num5 / 2f, relative - width),
						new PointF(width / 2f, relative - width - width / 8f),
						new PointF(0f, relative),
						new PointF((0f - width) / 2f, relative - width - width / 8f),
						new PointF((0f - num5) / 2f, relative - width)
					};
					break;
				}
				case NeedleStyle.Style10:
				{
					float num2 = relative / 1.618034f;
					num2 = num2 * 1.618034f - num2;
					float num3 = width / 1.618034f;
					points = new PointF[12]
					{
						new PointF((0f - width) / 2f, 0f - num2),
						new PointF(0f, 0f - num2 + num3),
						new PointF(width / 2f, 0f - num2),
						new PointF(width / 2f, 0f - num2 + num3 * 1.618034f),
						new PointF(num3 / 2f, 0f - num2 + num3 * 2.5f),
						new PointF(num3 / 2f, relative - width),
						new PointF(width / 2f, relative - width - width / 8f),
						new PointF(0f, relative),
						new PointF((0f - width) / 2f, relative - width - width / 8f),
						new PointF((0f - num3) / 2f, relative - width),
						new PointF((0f - num3) / 2f, 0f - num2 + num3 * 2.5f),
						new PointF((0f - width) / 2f, 0f - num2 + num3 * 1.618034f)
					};
					break;
				}
				case NeedleStyle.Style11:
				{
					float num = width / 4f;
					needleStyleAttrib.primaryPath.AddLine((0f - width) / 2f, (0f - width) / 2f, width / 2f, (0f - width) / 2f);
					needleStyleAttrib.primaryPath.AddLine(width / 2f, (0f - width) / 2f, num, relative - num);
					needleStyleAttrib.primaryPath.AddArc(0f - num, relative - num * 2f, num * 2f, num * 2f, 0f, 180f);
					needleStyleAttrib.primaryPath.AddLine(0f - num, relative - num, (0f - width) / 2f, (0f - width) / 2f);
					break;
				}
				default:
					throw new ArgumentException(SR.NotImplemented(NeedleStyle.ToString()));
				}
				if (NeedleStyle != NeedleStyle.Style11)
				{
					needleStyleAttrib.primaryPath.AddLines(points);
				}
				needleStyleAttrib.primaryPath.CloseFigure();
				needleStyleAttrib.primaryBrush = GetNeedleFillBrush(g, primary: true, needleStyleAttrib.primaryPath, pointOrigin, angle);
			}
			if (needleStyleAttrib.secondaryPath != null)
			{
				needleStyleAttrib.secondaryPath.AddEllipse((0f - relative2) / 2f, (0f - relative2) / 2f, relative2, relative2);
				needleStyleAttrib.secondaryBrush = GetNeedleFillBrush(g, primary: false, needleStyleAttrib.secondaryPath, pointOrigin, 0f);
				if (CapReflection)
				{
					needleStyleAttrib.reflectionPaths = new GraphicsPath[2];
					needleStyleAttrib.reflectionBrushes = new Brush[2];
					g.GetCircularEdgeReflection(needleStyleAttrib.secondaryPath.GetBounds(), 135f, 200, pointOrigin, out needleStyleAttrib.reflectionPaths[0], out needleStyleAttrib.reflectionBrushes[0]);
					g.GetCircularEdgeReflection(needleStyleAttrib.secondaryPath.GetBounds(), 315f, 128, pointOrigin, out needleStyleAttrib.reflectionPaths[1], out needleStyleAttrib.reflectionBrushes[1]);
				}
			}
			using (Matrix matrix = new Matrix())
			{
				matrix.Rotate(angle, MatrixOrder.Append);
				matrix.Translate(pointOrigin.X, pointOrigin.Y, MatrixOrder.Append);
				if (needleStyleAttrib.primaryPath != null)
				{
					needleStyleAttrib.primaryPath.Transform(matrix);
				}
				if (needleStyleAttrib.secondaryPath != null)
				{
					needleStyleAttrib.secondaryPath.Transform(matrix);
					return needleStyleAttrib;
				}
				return needleStyleAttrib;
			}
		}

		internal MarkerStyleAttrib GetMarkerStyleAttrib(GaugeGraphics g)
		{
			MarkerStyleAttrib markerStyleAttrib = new MarkerStyleAttrib();
			if (Image != "")
			{
				return markerStyleAttrib;
			}
			float positionFromValue = GetScale().GetPositionFromValue(base.Position);
			PointF absolutePoint = g.GetAbsolutePoint(GetScale().GetPivotPoint());
			float absoluteDimension = g.GetAbsoluteDimension(MarkerLength);
			float absoluteDimension2 = g.GetAbsoluteDimension(Width);
			float relative = CalculateMarkerDistance();
			relative = g.GetAbsoluteDimension(relative);
			PointF point = new PointF(0f, relative);
			markerStyleAttrib.path = g.CreateMarker(point, absoluteDimension2, absoluteDimension, MarkerStyle);
			markerStyleAttrib.brush = g.GetMarkerBrush(markerStyleAttrib.path, MarkerStyle, absolutePoint, positionFromValue, FillColor, FillGradientType, FillGradientEndColor, FillHatchStyle);
			using (Matrix matrix = new Matrix())
			{
				if (Placement == Placement.Inside)
				{
					matrix.RotateAt(180f, point, MatrixOrder.Append);
				}
				matrix.Rotate(positionFromValue, MatrixOrder.Append);
				matrix.Translate(absolutePoint.X, absolutePoint.Y, MatrixOrder.Append);
				markerStyleAttrib.path.Transform(matrix);
				return markerStyleAttrib;
			}
		}

		internal float CalculateMarkerDistance()
		{
			if (Placement == Placement.Cross)
			{
				return GetScale().GetRadius() - DistanceFromScale;
			}
			if (Placement == Placement.Inside)
			{
				return GetScale().GetRadius() - GetScale().Width / 2f - DistanceFromScale - MarkerLength / 2f;
			}
			return GetScale().GetRadius() + GetScale().Width / 2f + DistanceFromScale + MarkerLength / 2f;
		}

		internal BarStyleAttrib GetBarStyleAttrib(GaugeGraphics g)
		{
			BarStyleAttrib barStyleAttrib = new BarStyleAttrib();
			if (Image != "")
			{
				return barStyleAttrib;
			}
			CircularScale scale = GetScale();
			double valueLimit = scale.GetValueLimit(GetBarStartValue());
			double valueLimit2 = scale.GetValueLimit(base.Position);
			float positionFromValue = scale.GetPositionFromValue(valueLimit);
			float num = scale.GetPositionFromValue(valueLimit2) - positionFromValue;
			if (Math.Round(num, 4) == 0.0)
			{
				return barStyleAttrib;
			}
			if (base.BarStyle == BarStyle.Style1)
			{
				RectangleF rectangleF = CalculateBarRectangle();
				barStyleAttrib.primaryPath = g.GetCircularRangePath(rectangleF, positionFromValue + 90f, num, Width, Width, Placement);
				if (barStyleAttrib.primaryPath == null)
				{
					return barStyleAttrib;
				}
				RectangleF rect = rectangleF;
				if (Placement != 0)
				{
					if (Placement == Placement.Outside)
					{
						rect.Inflate(Width, Width);
					}
					else
					{
						rect.Inflate(Width / 2f, Width / 2f);
					}
				}
				barStyleAttrib.primaryBrush = g.GetCircularRangeBrush(rect, positionFromValue + 90f, num, FillColor, FillHatchStyle, "", GaugeImageWrapMode.Unscaled, Color.Empty, GaugeImageAlign.TopLeft, (RangeGradientType)Enum.Parse(typeof(RangeGradientType), FillGradientType.ToString()), FillGradientEndColor);
				CircularRange[] colorRanges = GetColorRanges();
				if (colorRanges != null)
				{
					barStyleAttrib.secondaryPaths = new GraphicsPath[colorRanges.Length];
					barStyleAttrib.secondaryBrushes = new Brush[colorRanges.Length];
					int num2 = 0;
					CircularRange[] array = colorRanges;
					foreach (CircularRange circularRange in array)
					{
						double num3 = scale.GetValueLimit(circularRange.StartValue);
						if (num3 < valueLimit)
						{
							num3 = valueLimit;
						}
						if (num3 > valueLimit2)
						{
							num3 = valueLimit2;
						}
						double num4 = scale.GetValueLimit(circularRange.EndValue);
						if (num4 < valueLimit)
						{
							num4 = valueLimit;
						}
						if (num4 > valueLimit2)
						{
							num4 = valueLimit2;
						}
						float positionFromValue2 = scale.GetPositionFromValue(num3);
						float num5 = scale.GetPositionFromValue(num4) - positionFromValue2;
						if (Math.Round(num5, 4) == 0.0)
						{
							barStyleAttrib.secondaryPaths[num2] = null;
							barStyleAttrib.secondaryBrushes[num2] = null;
						}
						else
						{
							barStyleAttrib.secondaryPaths[num2] = g.GetCircularRangePath(rectangleF, positionFromValue2 + 90f, num5, Width, Width, Placement);
							barStyleAttrib.secondaryBrushes[num2] = g.GetCircularRangeBrush(rectangleF, positionFromValue2 + 90f, num5, circularRange.InRangeBarPointerColor, FillHatchStyle, "", GaugeImageWrapMode.Unscaled, Color.Empty, GaugeImageAlign.TopLeft, (RangeGradientType)Enum.Parse(typeof(RangeGradientType), FillGradientType.ToString()), FillGradientEndColor);
						}
						num2++;
					}
				}
			}
			return barStyleAttrib;
		}

		private CircularRange[] GetColorRanges()
		{
			CircularGauge gauge = GetGauge();
			CircularScale scale = GetScale();
			if (gauge == null || scale == null)
			{
				return null;
			}
			double barStartValue = GetBarStartValue();
			double position = base.Position;
			ArrayList arrayList = null;
			foreach (CircularRange range in gauge.Ranges)
			{
				if (range.GetScale() != scale || !(range.InRangeBarPointerColor != Color.Empty))
				{
					continue;
				}
				double valueLimit = scale.GetValueLimit(range.StartValue);
				double valueLimit2 = scale.GetValueLimit(range.EndValue);
				if ((barStartValue >= valueLimit && barStartValue <= valueLimit2) || (position >= valueLimit && position <= valueLimit2) || (valueLimit >= barStartValue && valueLimit <= position) || (valueLimit2 >= barStartValue && valueLimit2 <= position))
				{
					if (arrayList == null)
					{
						arrayList = new ArrayList();
					}
					arrayList.Add(range);
				}
			}
			if (arrayList == null)
			{
				return null;
			}
			return (CircularRange[])arrayList.ToArray(typeof(CircularRange));
		}

		private double GetBarStartValue()
		{
			CircularScale scale = GetScale();
			if (BarStart == BarStart.ScaleStart)
			{
				return scale.MinimumLog;
			}
			if (scale.Logarithmic)
			{
				return 1.0;
			}
			return 0.0;
		}

		internal RectangleF CalculateBarRectangle()
		{
			CircularScale scale = GetScale();
			PointF pivotPoint = GetScale().GetPivotPoint();
			float radius = scale.GetRadius();
			RectangleF result = new RectangleF(pivotPoint.X - radius, pivotPoint.Y - radius, radius * 2f, radius * 2f);
			if (Placement == Placement.Inside)
			{
				result.Inflate(0f - DistanceFromScale, 0f - DistanceFromScale);
				result.Inflate((0f - scale.Width) / 2f, (0f - scale.Width) / 2f);
			}
			else if (Placement == Placement.Outside)
			{
				result.Inflate(DistanceFromScale, DistanceFromScale);
				result.Inflate(scale.Width / 2f, scale.Width / 2f);
			}
			else
			{
				result.Inflate(0f - DistanceFromScale, 0f - DistanceFromScale);
			}
			return result;
		}

		internal RectangleF GetNeedleCapBounds(GaugeGraphics g, PointF pointOrigin)
		{
			float absoluteDimension = g.GetAbsoluteDimension(CapWidth);
			RectangleF result = new RectangleF((0f - absoluteDimension) / 2f, (0f - absoluteDimension) / 2f, absoluteDimension, absoluteDimension);
			result.Offset(pointOrigin);
			return result;
		}

		internal override void Render(GaugeGraphics g)
		{
			if (Common == null || !Visible || GetScale() == null)
			{
				return;
			}
			Common.GaugeCore.TraceWrite("GaugePaint", SR.TraceStartRendering(Name));
			g.StartHotRegion(this);
			GetScale().SetDrawRegion(g);
			if (Image != "" && CapImage != "")
			{
				if (CapOnTop)
				{
					DrawImage(g, primary: true, drawShadow: false);
					if (Type == CircularPointerType.Needle && CapVisible)
					{
						DrawImage(g, primary: false, drawShadow: false);
					}
				}
				else
				{
					if (Type == CircularPointerType.Needle && CapVisible)
					{
						DrawImage(g, primary: false, drawShadow: false);
					}
					DrawImage(g, primary: true, drawShadow: false);
				}
				SetAllHotRegions(g);
				g.RestoreDrawRegion();
				g.EndHotRegion();
				Common.GaugeCore.TraceWrite("GaugePaint", SR.TraceRenderingComplete(Name));
				return;
			}
			if (Image != "" && CapOnTop)
			{
				DrawImage(g, primary: true, drawShadow: false);
			}
			if (CapImage != "" && !CapOnTop && Type == CircularPointerType.Needle && CapVisible)
			{
				DrawImage(g, primary: false, drawShadow: false);
			}
			float positionFromValue = GetScale().GetPositionFromValue(base.Position);
			PointF absolutePoint = g.GetAbsolutePoint(GetScale().GetPivotPoint());
			Pen pen = new Pen(base.BorderColor, base.BorderWidth);
			pen.DashStyle = g.GetPenStyle(base.BorderStyle);
			if (pen.DashStyle != 0)
			{
				pen.Alignment = PenAlignment.Center;
			}
			if (Type == CircularPointerType.Needle)
			{
				NeedleStyleAttrib needleStyleAttrib = GetNeedleStyleAttrib(g, absolutePoint, positionFromValue);
				try
				{
					if (CapOnTop)
					{
						if (needleStyleAttrib.primaryPath != null && Image == string.Empty)
						{
							g.FillPath(needleStyleAttrib.primaryBrush, needleStyleAttrib.primaryPath, positionFromValue, useBrushOffset: true, circularFill: false);
							if (base.BorderWidth > 0 && base.BorderStyle != 0)
							{
								g.DrawPath(pen, needleStyleAttrib.primaryPath);
							}
						}
						if (needleStyleAttrib.secondaryPath != null && CapImage == string.Empty)
						{
							if (CapStyle == CapStyle.Simple)
							{
								g.FillPath(needleStyleAttrib.secondaryBrush, needleStyleAttrib.secondaryPath, 0f, useBrushOffset: true, circularFill: true);
							}
							else
							{
								XamlLayer[] layers = GetCachedXamlRenderer(GetNeedleCapBounds(g, absolutePoint)).Layers;
								for (int i = 0; i < layers.Length; i++)
								{
									layers[i].Render(g);
								}
							}
							if (needleStyleAttrib.reflectionPaths != null)
							{
								for (int j = 0; j < needleStyleAttrib.reflectionPaths.Length; j++)
								{
									if (needleStyleAttrib.reflectionPaths[j] != null)
									{
										g.FillPath(needleStyleAttrib.reflectionBrushes[j], needleStyleAttrib.reflectionPaths[j]);
									}
								}
							}
							if (base.BorderWidth > 0 && base.BorderStyle != 0)
							{
								g.DrawPath(pen, needleStyleAttrib.secondaryPath);
							}
						}
					}
					else
					{
						if (needleStyleAttrib.secondaryPath != null && CapImage == string.Empty)
						{
							if (CapStyle == CapStyle.Simple)
							{
								g.FillPath(needleStyleAttrib.secondaryBrush, needleStyleAttrib.secondaryPath, 0f, useBrushOffset: true, circularFill: true);
							}
							else
							{
								XamlLayer[] layers = GetCachedXamlRenderer(GetNeedleCapBounds(g, absolutePoint)).Layers;
								for (int i = 0; i < layers.Length; i++)
								{
									layers[i].Render(g);
								}
							}
							if (base.BorderWidth > 0 && base.BorderStyle != 0)
							{
								g.DrawPath(pen, needleStyleAttrib.secondaryPath);
							}
						}
						if (needleStyleAttrib.primaryPath != null && Image == string.Empty)
						{
							g.FillPath(needleStyleAttrib.primaryBrush, needleStyleAttrib.primaryPath, positionFromValue, useBrushOffset: true, circularFill: false);
							if (base.BorderWidth > 0 && base.BorderStyle != 0)
							{
								g.DrawPath(pen, needleStyleAttrib.primaryPath);
							}
						}
					}
					if (needleStyleAttrib.primaryPath != null && Image == string.Empty)
					{
						AddHotRegion((GraphicsPath)needleStyleAttrib.primaryPath.Clone(), primary: true);
					}
					if (needleStyleAttrib.secondaryPath != null && CapImage == string.Empty)
					{
						AddHotRegion((GraphicsPath)needleStyleAttrib.secondaryPath.Clone(), primary: false);
					}
				}
				finally
				{
					needleStyleAttrib.Dispose();
				}
			}
			else if (Type == CircularPointerType.Bar)
			{
				BarStyleAttrib barStyleAttrib = GetBarStyleAttrib(g);
				try
				{
					if (barStyleAttrib.primaryPath != null)
					{
						g.FillPath(barStyleAttrib.primaryBrush, barStyleAttrib.primaryPath);
					}
					if (barStyleAttrib.secondaryPaths != null)
					{
						int num = 0;
						GraphicsPath[] secondaryPaths = barStyleAttrib.secondaryPaths;
						foreach (GraphicsPath graphicsPath in secondaryPaths)
						{
							if (graphicsPath != null && barStyleAttrib.secondaryBrushes[num] != null)
							{
								g.FillPath(barStyleAttrib.secondaryBrushes[num], graphicsPath);
							}
							num++;
						}
					}
					if (base.BorderWidth > 0 && barStyleAttrib.primaryPath != null && base.BorderStyle != 0)
					{
						g.DrawPath(pen, barStyleAttrib.primaryPath);
					}
					if (barStyleAttrib.primaryPath != null)
					{
						AddHotRegion((GraphicsPath)barStyleAttrib.primaryPath.Clone(), primary: true);
					}
				}
				finally
				{
					barStyleAttrib.Dispose();
				}
			}
			else
			{
				MarkerStyleAttrib markerStyleAttrib = GetMarkerStyleAttrib(g);
				try
				{
					if (markerStyleAttrib.path != null)
					{
						bool circularFill = (MarkerStyle == MarkerStyle.Circle) ? true : false;
						g.FillPath(markerStyleAttrib.brush, markerStyleAttrib.path, positionFromValue, useBrushOffset: true, circularFill);
					}
					if (base.BorderWidth > 0 && markerStyleAttrib.path != null && base.BorderStyle != 0)
					{
						g.DrawPath(pen, markerStyleAttrib.path);
					}
					if (markerStyleAttrib.path != null)
					{
						AddHotRegion((GraphicsPath)markerStyleAttrib.path.Clone(), primary: true);
					}
				}
				finally
				{
					markerStyleAttrib.Dispose();
				}
			}
			if (Image != "" && !CapOnTop)
			{
				DrawImage(g, primary: true, drawShadow: false);
			}
			if (CapImage != "" && CapOnTop && Type == CircularPointerType.Needle && CapVisible)
			{
				DrawImage(g, primary: false, drawShadow: false);
			}
			SetAllHotRegions(g);
			g.RestoreDrawRegion();
			g.EndHotRegion();
			Common.GaugeCore.TraceWrite("GaugePaint", SR.TraceRenderingComplete(Name));
		}

		internal void DrawImage(GaugeGraphics g, bool primary, bool drawShadow)
		{
			if (!Visible || (drawShadow && base.ShadowOffset == 0f))
			{
				return;
			}
			float relative = (Placement == Placement.Cross) ? (GetScale().GetRadius() - DistanceFromScale) : ((Placement != 0) ? (GetScale().GetRadius() + GetScale().Width / 2f + DistanceFromScale) : (GetScale().GetRadius() - GetScale().Width / 2f - DistanceFromScale));
			relative = g.GetAbsoluteDimension(relative);
			Image image = null;
			image = ((!primary) ? Common.ImageLoader.LoadImage(CapImage) : Common.ImageLoader.LoadImage(Image));
			if (image.Width == 0 || image.Height == 0)
			{
				return;
			}
			Point empty = Point.Empty;
			if (primary)
			{
				empty = ImageOrigin;
				if (empty.X == 0)
				{
					empty.X = image.Width / 2;
				}
			}
			else
			{
				empty = CapImageOrigin;
				if (empty.IsEmpty)
				{
					empty.X = image.Width / 2;
					empty.Y = image.Height / 2;
				}
			}
			int num = primary ? (image.Height - empty.Y) : ((image.Height <= image.Width) ? image.Width : image.Height);
			if (num == 0)
			{
				return;
			}
			float num2 = (!primary) ? (g.GetAbsoluteDimension(CapWidth) / (float)num) : (relative / (float)num);
			Rectangle rectangle = new Rectangle(0, 0, (int)((float)image.Width * num2), (int)((float)image.Height * num2));
			ImageAttributes imageAttributes = new ImageAttributes();
			if (primary && ImageTransColor != Color.Empty)
			{
				imageAttributes.SetColorKey(ImageTransColor, ImageTransColor, ColorAdjustType.Default);
			}
			if (!primary && CapImageTransColor != Color.Empty)
			{
				imageAttributes.SetColorKey(CapImageTransColor, CapImageTransColor, ColorAdjustType.Default);
			}
			Matrix transform = g.Transform;
			Matrix matrix = g.Transform.Clone();
			float positionFromValue = GetScale().GetPositionFromValue(base.Position);
			PointF absolutePoint = g.GetAbsolutePoint(GetScale().GetPivotPoint());
			PointF pointF = new PointF((float)empty.X * num2, (float)empty.Y * num2);
			float offsetX = matrix.OffsetX;
			float offsetY = matrix.OffsetY;
			matrix.Translate(absolutePoint.X - pointF.X, absolutePoint.Y - pointF.Y, MatrixOrder.Append);
			absolutePoint.X += offsetX;
			absolutePoint.Y += offsetY;
			if (primary)
			{
				matrix.RotateAt(positionFromValue, absolutePoint, MatrixOrder.Append);
			}
			if (drawShadow)
			{
				ColorMatrix colorMatrix = new ColorMatrix();
				colorMatrix.Matrix00 = 0f;
				colorMatrix.Matrix11 = 0f;
				colorMatrix.Matrix22 = 0f;
				colorMatrix.Matrix33 = Common.GaugeCore.ShadowIntensity / 100f;
				imageAttributes.SetColorMatrix(colorMatrix);
				matrix.Translate(base.ShadowOffset, base.ShadowOffset, MatrixOrder.Append);
			}
			else
			{
				ColorMatrix colorMatrix2 = new ColorMatrix();
				if (primary && !ImageHueColor.IsEmpty)
				{
					Color color = g.TransformHueColor(ImageHueColor);
					colorMatrix2.Matrix00 = (float)(int)color.R / 255f;
					colorMatrix2.Matrix11 = (float)(int)color.G / 255f;
					colorMatrix2.Matrix22 = (float)(int)color.B / 255f;
				}
				else if (!primary && !CapImageHueColor.IsEmpty)
				{
					Color color2 = g.TransformHueColor(CapImageHueColor);
					colorMatrix2.Matrix00 = (float)(int)color2.R / 255f;
					colorMatrix2.Matrix11 = (float)(int)color2.G / 255f;
					colorMatrix2.Matrix22 = (float)(int)color2.B / 255f;
				}
				if (primary)
				{
					colorMatrix2.Matrix33 = 1f - ImageTransparency / 100f;
				}
				imageAttributes.SetColorMatrix(colorMatrix2);
			}
			g.Transform = matrix;
			g.DrawImage(image, rectangle, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, imageAttributes);
			g.Transform = transform;
			if (!drawShadow)
			{
				matrix.Translate(0f - offsetX, 0f - offsetY, MatrixOrder.Append);
				GraphicsPath graphicsPath = new GraphicsPath();
				graphicsPath.AddRectangle(rectangle);
				graphicsPath.Transform(matrix);
				AddHotRegion(graphicsPath, primary);
			}
		}

		internal GraphicsPath GetPointerPath(GaugeGraphics g, bool shadowPath)
		{
			if (!Visible)
			{
				return null;
			}
			GraphicsPath graphicsPath = new GraphicsPath();
			float positionFromValue = GetScale().GetPositionFromValue(base.Position);
			PointF absolutePoint = g.GetAbsolutePoint(GetScale().GetPivotPoint());
			if (Type == CircularPointerType.Needle || Image != string.Empty)
			{
				NeedleStyleAttrib needleStyleAttrib = GetNeedleStyleAttrib(g, absolutePoint, positionFromValue);
				if (needleStyleAttrib.primaryPath != null && (Image == string.Empty || !shadowPath))
				{
					graphicsPath.AddPath(needleStyleAttrib.primaryPath, connect: false);
				}
				if (needleStyleAttrib.secondaryPath != null && CapVisible && Type == CircularPointerType.Needle && (CapImage == string.Empty || !shadowPath))
				{
					graphicsPath.AddPath(needleStyleAttrib.secondaryPath, connect: false);
				}
			}
			else if (Type == CircularPointerType.Bar)
			{
				BarStyleAttrib barStyleAttrib = GetBarStyleAttrib(g);
				if (barStyleAttrib.primaryPath == null)
				{
					graphicsPath.Dispose();
					return null;
				}
				if (barStyleAttrib.primaryPath != null)
				{
					graphicsPath.AddPath(barStyleAttrib.primaryPath, connect: false);
				}
			}
			else if (Type == CircularPointerType.Marker)
			{
				MarkerStyleAttrib markerStyleAttrib = GetMarkerStyleAttrib(g);
				if (markerStyleAttrib.path != null)
				{
					graphicsPath.AddPath(markerStyleAttrib.path, connect: false);
				}
			}
			return graphicsPath;
		}

		internal GraphicsPath GetShadowPath(GaugeGraphics g)
		{
			if (base.ShadowOffset == 0f || GetScale() == null)
			{
				return null;
			}
			GetScale().SetDrawRegion(g);
			if (CapOnTop)
			{
				if (Image != "")
				{
					DrawImage(g, primary: true, drawShadow: true);
				}
				if (CapImage != "" && Type == CircularPointerType.Needle && CapVisible)
				{
					DrawImage(g, primary: false, drawShadow: true);
				}
			}
			else
			{
				if (CapImage != "" && Type == CircularPointerType.Needle && CapVisible)
				{
					DrawImage(g, primary: false, drawShadow: true);
				}
				if (Image != "")
				{
					DrawImage(g, primary: true, drawShadow: true);
				}
			}
			GraphicsPath pointerPath = GetPointerPath(g, shadowPath: true);
			if (pointerPath == null || pointerPath.PointCount == 0)
			{
				g.RestoreDrawRegion();
				return null;
			}
			using (Matrix matrix = new Matrix())
			{
				matrix.Translate(base.ShadowOffset, base.ShadowOffset);
				pointerPath.Transform(matrix);
			}
			PointF pointF = new PointF(g.Graphics.Transform.OffsetX, g.Graphics.Transform.OffsetY);
			g.RestoreDrawRegion();
			PointF pointF2 = new PointF(g.Graphics.Transform.OffsetX, g.Graphics.Transform.OffsetY);
			using (Matrix matrix2 = new Matrix())
			{
				matrix2.Translate(pointF.X - pointF2.X, pointF.Y - pointF2.Y);
				pointerPath.Transform(matrix2);
				return pointerPath;
			}
		}

		internal void AddHotRegion(GraphicsPath path, bool primary)
		{
			if (primary)
			{
				hotRegions[0] = path;
			}
			else
			{
				hotRegions[1] = path;
			}
		}

		internal void SetAllHotRegions(GaugeGraphics g)
		{
			Common.GaugeCore.HotRegionList.SetHotRegion(this, g.GetAbsolutePoint(GetScale().GetPivotPoint()), hotRegions[0], hotRegions[1]);
			hotRegions[0] = null;
			hotRegions[1] = null;
		}

		public override string ToString()
		{
			return Name;
		}

		public CircularGauge GetGauge()
		{
			if (Collection == null)
			{
				return null;
			}
			return (CircularGauge)Collection.parent;
		}

		public CircularScale GetScale()
		{
			if (GetGauge() == null)
			{
				return null;
			}
			if (ScaleName == string.Empty)
			{
				return null;
			}
			if (ScaleName == "Default" && GetGauge().Scales.Count == 0)
			{
				return null;
			}
			return GetGauge().Scales[ScaleName];
		}

		internal float GetNeedleTailLength()
		{
			float num = (Placement == Placement.Cross) ? (GetScale().GetRadius() - DistanceFromScale) : ((Placement != 0) ? (GetScale().GetRadius() + GetScale().Width / 2f + DistanceFromScale) : (GetScale().GetRadius() - GetScale().Width / 2f - DistanceFromScale));
			if (NeedleStyle == NeedleStyle.Style2 || NeedleStyle == NeedleStyle.Style3 || NeedleStyle == NeedleStyle.Style5 || NeedleStyle == NeedleStyle.Style7 || NeedleStyle == NeedleStyle.Style9 || NeedleStyle == NeedleStyle.Style10)
			{
				float num2 = num / 1.618034f;
				return num2 * 1.618034f - num2;
			}
			return Width / 2f;
		}

		internal XamlRenderer GetCachedXamlRenderer(RectangleF bounds)
		{
			if (xamlRenderer != null)
			{
				return xamlRenderer;
			}
			xamlRenderer = new XamlRenderer(CapStyle.ToString() + ".xaml");
			xamlRenderer.AllowPathGradientTransform = false;
			Color[] layerHues = new Color[1]
			{
				CapFillColor
			};
			xamlRenderer.ParseXaml(bounds, layerHues);
			return xamlRenderer;
		}

		internal void ResetCachedXamlRenderer()
		{
			if (xamlRenderer != null)
			{
				xamlRenderer.Dispose();
				xamlRenderer = null;
			}
		}

		void ISelectable.DrawSelection(GaugeGraphics g, bool designTimeSelection)
		{
			if (GetGauge() == null || GetScale() == null)
			{
				return;
			}
			Stack stack = new Stack();
			for (NamedElement namedElement = GetGauge().ParentObject; namedElement != null; namedElement = (NamedElement)((IRenderable)namedElement).GetParentRenderable())
			{
				stack.Push(namedElement);
			}
			foreach (IRenderable item in stack)
			{
				g.CreateDrawRegion(item.GetBoundRect(g));
			}
			g.CreateDrawRegion(((IRenderable)GetGauge()).GetBoundRect(g));
			GetScale().SetDrawRegion(g);
			using (GraphicsPath graphicsPath = GetPointerPath(g, shadowPath: false))
			{
				if (graphicsPath != null)
				{
					RectangleF bounds = graphicsPath.GetBounds();
					g.DrawSelection(bounds, designTimeSelection, Common.GaugeCore.SelectionBorderColor, Common.GaugeCore.SelectionMarkerColor);
				}
			}
			g.RestoreDrawRegion();
			g.RestoreDrawRegion();
			foreach (IRenderable item2 in stack)
			{
				_ = item2;
				g.RestoreDrawRegion();
			}
		}

		public override object Clone()
		{
			MemoryStream stream = new MemoryStream();
			BinaryFormatSerializer binaryFormatSerializer = new BinaryFormatSerializer();
			binaryFormatSerializer.Serialize(this, stream);
			CircularPointer circularPointer = new CircularPointer();
			binaryFormatSerializer.Deserialize(circularPointer, stream);
			return circularPointer;
		}
	}
}
