using Microsoft.Reporting.Chart.WebForms.Design;
using Microsoft.Reporting.Chart.WebForms.Utilities;
using System.ComponentModel;
using System.Drawing;

namespace Microsoft.Reporting.Chart.WebForms
{
	[SRDescription("DescriptionAttributeAnnotationGroup_AnnotationGroup")]
	internal class AnnotationGroup : Annotation
	{
		internal AnnotationCollection annotations;

		[SRCategory("CategoryAttributeMisc")]
		[DefaultValue("NotSet")]
		[SRDescription("DescriptionAttributeAnnotationGroup_ClipToChartArea")]
		[TypeConverter(typeof(LegendAreaNameConverter))]
		public override string ClipToChartArea
		{
			get
			{
				return base.ClipToChartArea;
			}
			set
			{
				base.ClipToChartArea = value;
				foreach (Annotation annotation in annotations)
				{
					annotation.ClipToChartArea = value;
				}
			}
		}

		[SRCategory("CategoryAttributePosition")]
		[DefaultValue(true)]
		[SRDescription("DescriptionAttributeAnnotationGroup_SizeAlwaysRelative")]
		public override bool SizeAlwaysRelative
		{
			get
			{
				return base.SizeAlwaysRelative;
			}
			set
			{
				base.SizeAlwaysRelative = value;
				foreach (Annotation annotation in annotations)
				{
					annotation.SizeAlwaysRelative = value;
				}
			}
		}

		[SRCategory("CategoryAttributeAppearance")]
		[DefaultValue(false)]
		[Browsable(false)]
		[SRDescription("DescriptionAttributeAnnotationGroup_Selected")]
		public override bool Selected
		{
			get
			{
				return base.Selected;
			}
			set
			{
				base.Selected = value;
				foreach (Annotation annotation in annotations)
				{
					annotation.Selected = false;
				}
			}
		}

		[SRCategory("CategoryAttributeAppearance")]
		[DefaultValue(true)]
		[SRDescription("DescriptionAttributeAnnotationGroup_Visible")]
		[ParenthesizePropertyName(true)]
		public override bool Visible
		{
			get
			{
				return base.Visible;
			}
			set
			{
				base.Visible = value;
				foreach (Annotation annotation in annotations)
				{
					annotation.Visible = value;
				}
			}
		}

		[SRCategory("CategoryAttributeAppearance")]
		[DefaultValue(typeof(ContentAlignment), "MiddleCenter")]
		[SRDescription("DescriptionAttributeAlignment7")]
		public override ContentAlignment Alignment
		{
			get
			{
				return base.Alignment;
			}
			set
			{
				base.Alignment = value;
				foreach (Annotation annotation in annotations)
				{
					annotation.Alignment = value;
				}
			}
		}

		[SRCategory("CategoryAttributeAppearance")]
		[DefaultValue(typeof(Color), "Black")]
		[SRDescription("DescriptionAttributeTextColor6")]
		public override Color TextColor
		{
			get
			{
				return base.TextColor;
			}
			set
			{
				base.TextColor = value;
				foreach (Annotation annotation in annotations)
				{
					annotation.TextColor = value;
				}
			}
		}

		[SRCategory("CategoryAttributeAppearance")]
		[DefaultValue(typeof(Font), "Microsoft Sans Serif, 8pt")]
		[SRDescription("DescriptionAttributeTextFont")]
		public override Font TextFont
		{
			get
			{
				return base.TextFont;
			}
			set
			{
				base.TextFont = value;
				foreach (Annotation annotation in annotations)
				{
					annotation.TextFont = value;
				}
			}
		}

		[SRCategory("CategoryAttributeAppearance")]
		[DefaultValue(typeof(TextStyle), "Default")]
		[SRDescription("DescriptionAttributeTextStyle3")]
		public override TextStyle TextStyle
		{
			get
			{
				return base.TextStyle;
			}
			set
			{
				base.TextStyle = value;
				foreach (Annotation annotation in annotations)
				{
					annotation.TextStyle = value;
				}
			}
		}

		[SRCategory("CategoryAttributeAppearance")]
		[DefaultValue(typeof(Color), "Black")]
		[SRDescription("DescriptionAttributeLineColor")]
		public override Color LineColor
		{
			get
			{
				return base.LineColor;
			}
			set
			{
				base.LineColor = value;
				foreach (Annotation annotation in annotations)
				{
					annotation.LineColor = value;
				}
			}
		}

		[SRCategory("CategoryAttributeAppearance")]
		[DefaultValue(1)]
		[SRDescription("DescriptionAttributeLineWidth7")]
		public override int LineWidth
		{
			get
			{
				return base.LineWidth;
			}
			set
			{
				base.LineWidth = value;
				foreach (Annotation annotation in annotations)
				{
					annotation.LineWidth = value;
				}
			}
		}

		[SRCategory("CategoryAttributeAppearance")]
		[DefaultValue(ChartDashStyle.Solid)]
		[SRDescription("DescriptionAttributeLineStyle6")]
		public override ChartDashStyle LineStyle
		{
			get
			{
				return base.LineStyle;
			}
			set
			{
				base.LineStyle = value;
				foreach (Annotation annotation in annotations)
				{
					annotation.LineStyle = value;
				}
			}
		}

		[SRCategory("CategoryAttributeAppearance")]
		[DefaultValue(typeof(Color), "")]
		[SRDescription("DescriptionAttributeBackColor8")]
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
				foreach (Annotation annotation in annotations)
				{
					annotation.BackColor = value;
				}
			}
		}

		[SRCategory("CategoryAttributeAppearance")]
		[DefaultValue(ChartHatchStyle.None)]
		[NotifyParentProperty(true)]
		[SRDescription("DescriptionAttributeBackHatchStyle")]
		public override ChartHatchStyle BackHatchStyle
		{
			get
			{
				return base.BackHatchStyle;
			}
			set
			{
				base.BackHatchStyle = value;
				foreach (Annotation annotation in annotations)
				{
					annotation.BackHatchStyle = value;
				}
			}
		}

		[SRCategory("CategoryAttributeAppearance")]
		[DefaultValue(GradientType.None)]
		[NotifyParentProperty(true)]
		[SRDescription("DescriptionAttributeBackGradientType8")]
		public override GradientType BackGradientType
		{
			get
			{
				return base.BackGradientType;
			}
			set
			{
				base.BackGradientType = value;
				foreach (Annotation annotation in annotations)
				{
					annotation.BackGradientType = value;
				}
			}
		}

		[SRCategory("CategoryAttributeAppearance")]
		[DefaultValue(typeof(Color), "")]
		[NotifyParentProperty(true)]
		[SRDescription("DescriptionAttributeAnnotationGroup_BackGradientEndColor")]
		public override Color BackGradientEndColor
		{
			get
			{
				return base.BackGradientEndColor;
			}
			set
			{
				base.BackGradientEndColor = value;
				foreach (Annotation annotation in annotations)
				{
					annotation.BackGradientEndColor = value;
				}
			}
		}

		[SRCategory("CategoryAttributeAppearance")]
		[DefaultValue(typeof(Color), "128,0,0,0")]
		[SRDescription("DescriptionAttributeAnnotationGroup_ShadowColor")]
		public override Color ShadowColor
		{
			get
			{
				return base.ShadowColor;
			}
			set
			{
				base.ShadowColor = value;
				foreach (Annotation annotation in annotations)
				{
					annotation.ShadowColor = value;
				}
			}
		}

		[SRCategory("CategoryAttributeAppearance")]
		[DefaultValue(0)]
		[SRDescription("DescriptionAttributeAnnotationGroup_ShadowOffset")]
		public override int ShadowOffset
		{
			get
			{
				return base.ShadowOffset;
			}
			set
			{
				base.ShadowOffset = value;
				foreach (Annotation annotation in annotations)
				{
					annotation.ShadowOffset = value;
				}
			}
		}

		[SRCategory("CategoryAttributeAnnotations")]
		[SRDescription("DescriptionAttributeAnnotationGroup_Annotations")]
		public AnnotationCollection Annotations => annotations;

		[SRCategory("CategoryAttributeMisc")]
		[Bindable(true)]
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[SerializationVisibility(SerializationVisibility.Hidden)]
		[SRDescription("DescriptionAttributeAnnotationType4")]
		public override string AnnotationType => "Group";

		[SRCategory("CategoryAttributeAppearance")]
		[DefaultValue(SelectionPointsStyle.Rectangle)]
		[ParenthesizePropertyName(true)]
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[SerializationVisibility(SerializationVisibility.Hidden)]
		[SRDescription("DescriptionAttributeSelectionPointsStyle3")]
		internal override SelectionPointsStyle SelectionPointsStyle => SelectionPointsStyle.Rectangle;

		public AnnotationGroup()
		{
			annotations = new AnnotationCollection();
			annotations.annotationGroup = this;
		}

		internal override void Paint(Chart chart, ChartGraphics graphics)
		{
			Chart = chart;
			foreach (Annotation annotation in annotations)
			{
				annotation.Paint(chart, graphics);
			}
			if ((!Chart.chartPicture.common.ProcessModePaint || !Selected) && !Chart.chartPicture.common.ProcessModeRegions)
			{
				return;
			}
			PointF location = PointF.Empty;
			PointF anchorLocation = PointF.Empty;
			SizeF size = SizeF.Empty;
			GetRelativePosition(out location, out size, out anchorLocation);
			PointF pointF = new PointF(location.X + size.Width, location.Y + size.Height);
			RectangleF rectangleF = new RectangleF(location, new SizeF(pointF.X - location.X, pointF.Y - location.Y));
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
			if (!rectangleF.IsEmpty && !float.IsNaN(rectangleF.X) && !float.IsNaN(rectangleF.Y) && !float.IsNaN(rectangleF.Right) && !float.IsNaN(rectangleF.Bottom))
			{
				_ = Chart.chartPicture.common.ProcessModePaint;
				if (Chart.chartPicture.common.ProcessModeRegions)
				{
					Chart.chartPicture.common.HotRegionsList.AddHotRegion(graphics, rectangleF, ReplaceKeywords(ToolTip), ReplaceKeywords(Href), ReplaceKeywords(MapAreaAttributes), this, ChartElementType.Annotation, string.Empty);
				}
				PaintSelectionHandles(graphics, rectangleF, null);
			}
		}
	}
}
