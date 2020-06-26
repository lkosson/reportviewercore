using Microsoft.ReportingServices.Rendering.ImageRenderer;
using Microsoft.ReportingServices.Rendering.RPLProcessing;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;

namespace Microsoft.Reporting.NETCore
{
	internal abstract class RenderingItem : RenderingElement
	{
		private byte m_state;

		private int m_zIndex;

		private Color m_backgroundColor;

		private Image m_backgroundImage;

		private RPLFormat.BackgroundRepeatTypes m_backgroundRepeat;

		private bool m_delayDrawBorders;

		private RenderingItemBorder m_borderLeft;

		private RenderingItemBorder m_borderRight;

		private RenderingItemBorder m_borderTop;

		private RenderingItemBorder m_borderBottom;

		internal bool HasBorders
		{
			get
			{
				if (m_borderLeft.IsVisible || m_borderRight.IsVisible || m_borderBottom.IsVisible || m_borderTop.IsVisible)
				{
					return true;
				}
				return false;
			}
		}

		internal string Bookmark
		{
			get
			{
				string bookmark = (InstanceProperties as RPLItemProps).Bookmark;
				if (string.IsNullOrEmpty(bookmark))
				{
					bookmark = (DefinitionProperties as RPLItemPropsDef).Bookmark;
				}
				return bookmark;
			}
		}

		internal string ToolTip
		{
			get
			{
				string toolTip = (InstanceProperties as RPLItemProps).ToolTip;
				if (string.IsNullOrEmpty(toolTip))
				{
					toolTip = (DefinitionProperties as RPLItemPropsDef).ToolTip;
				}
				return toolTip;
			}
		}

		internal string Label
		{
			get
			{
				string label = (InstanceProperties as RPLItemProps).Label;
				if (string.IsNullOrEmpty(label))
				{
					label = (DefinitionProperties as RPLItemPropsDef).Label;
				}
				return label;
			}
		}

		internal bool DelayDrawBorders
		{
			get
			{
				return m_delayDrawBorders;
			}
			set
			{
				m_delayDrawBorders = value;
			}
		}

		internal Color BackgroundColor => m_backgroundColor;

		internal Image BackgroundImage => m_backgroundImage;

		internal RPLFormat.BackgroundRepeatTypes BackgroundRepeat => m_backgroundRepeat;

		internal virtual void Initialize(GdiContext context, RPLItemMeasurement measurement, RectangleF bounds, string accessibleName)
		{
			Initialize(measurement.Element);
			m_accessibleName = accessibleName;
			m_zIndex = measurement.ZIndex;
			m_state = measurement.State;
			m_position = GdiContext.GetMeasurementRectangle(measurement, bounds);
			if (!string.IsNullOrEmpty(Label))
			{
				context.RenderingReport.Labels.Add(InstanceProperties.UniqueName, new LabelPoint(base.Position.Location));
			}
			if (!string.IsNullOrEmpty(Bookmark))
			{
				context.RenderingReport.Bookmarks.Add(InstanceProperties.UniqueName, new BookmarkPoint(base.Position.Location));
			}
			if (!string.IsNullOrEmpty(ToolTip))
			{
				context.RenderingReport.ToolTips.Add(new ReportToolTip(InstanceProperties.UniqueName, base.Position, ToolTip));
			}
			ProcessRenderingElementContent(measurement.Element, context, base.Position);
			ProcessBackgroundColorAndImage(context, InstanceProperties);
			if (!(this is RenderingLine))
			{
				ProcessBorders(context.GdiWriter, InstanceProperties as RPLItemProps, bounds, m_state);
			}
			if (!(InstanceProperties is RPLTextBoxProps))
			{
				InstanceProperties.NonSharedStyle = null;
			}
		}

		internal static RenderingItem CreateRenderingItem(GdiContext context, RPLItemMeasurement measurement, RectangleF bounds)
		{
			if (!(measurement.Element is RPLLine) && (measurement.Width <= 0f || measurement.Height <= 0f))
			{
				return null;
			}
			RenderingItem renderingItem = null;
			string text = null;
			if (measurement.Element is RPLTextBox)
			{
				renderingItem = new RenderingTextBox();
				text = ReportPreviewStrings.ReportItemAccessibleNameTextBox;
			}
			else if (measurement.Element is RPLTablix)
			{
				renderingItem = new RenderingTablix();
				text = ReportPreviewStrings.ReportItemAccessibleNameTablix;
			}
			else if (measurement.Element is RPLRectangle)
			{
				renderingItem = new RenderingRectangle();
				text = ReportPreviewStrings.ReportItemAccessibleNameRectangle;
			}
			else if (measurement.Element is RPLSubReport)
			{
				renderingItem = new RenderingSubReport();
				text = ReportPreviewStrings.ReportItemAccessibleNameSubreport;
			}
			else if (measurement.Element is RPLImage)
			{
				renderingItem = new RenderingImage();
				text = ReportPreviewStrings.ReportItemAccessibleNameImage;
			}
			else if (measurement.Element is RPLChart)
			{
				renderingItem = new RenderingDynamicImage();
				text = ReportPreviewStrings.ReportItemAccessibleNameChart;
			}
			else if (measurement.Element is RPLGaugePanel)
			{
				renderingItem = new RenderingDynamicImage();
				text = ReportPreviewStrings.ReportItemAccessibleNameGaugePanel;
			}
			else if (measurement.Element is RPLMap)
			{
				renderingItem = new RenderingDynamicImage();
				text = ReportPreviewStrings.ReportItemAccessibleNameMap;
			}
			else if (measurement.Element is RPLLine)
			{
				renderingItem = new RenderingLine();
				text = ReportPreviewStrings.ReportItemAccessibleNameLine;
			}
			else
			{
				if (!(measurement.Element is RPLBody))
				{
					return null;
				}
				renderingItem = new RenderingBody();
				text = ReportPreviewStrings.ReportItemAccessibleNameBody;
			}
			renderingItem.Initialize(context, measurement, bounds, text);
			return renderingItem;
		}

		internal void DrawToPage(GdiContext context)
		{
			if (context.IsOnScreen(base.Position) || context.FirstDraw)
			{
				if (Color.Empty != BackgroundColor)
				{
					context.Graphics.FillRectangle(new SolidBrush(BackgroundColor), base.Position);
				}
				if (BackgroundImage != null)
				{
					DrawBackgroundImage(context, BackgroundImage, BackgroundRepeat, base.Position);
				}
				DrawContent(context);
				if (!(this is RenderingLine) && !DelayDrawBorders)
				{
					DrawBorders(context);
				}
			}
		}

		internal static void DrawBackgroundImage(GdiContext context, Image backgroundImage, RPLFormat.BackgroundRepeatTypes backgroundRepeat, RectangleF position)
		{
			if (backgroundImage is Bitmap)
			{
				((Bitmap)backgroundImage).SetResolution(96f, 96f);
			}
			int width = backgroundImage.Width;
			int height = backgroundImage.Height;
			if (backgroundRepeat == RPLFormat.BackgroundRepeatTypes.Clip)
			{
				DrawImageClipped(context, backgroundImage, 0f, 0f, position);
				return;
			}
			int num = context.GdiWriter.ConvertToPixels(position.Width);
			int num2 = context.GdiWriter.ConvertToPixels(position.Height);
			float num3 = context.GdiWriter.ConvertToMillimeters(width);
			float num4 = context.GdiWriter.ConvertToMillimeters(height);
			float num5 = 0f;
			float num6 = 0f;
			switch (backgroundRepeat)
			{
			case RPLFormat.BackgroundRepeatTypes.Repeat:
			{
				for (int j = 0; j < num; j += width)
				{
					num6 = 0f;
					for (int k = 0; k < num2; k += height)
					{
						DrawImageClipped(context, backgroundImage, num5, num6, position);
						num6 += num4;
					}
					num5 += num3;
				}
				break;
			}
			case RPLFormat.BackgroundRepeatTypes.RepeatX:
			{
				for (int l = 0; l < num; l += width)
				{
					DrawImageClipped(context, backgroundImage, num5, num6, position);
					num5 += num3;
				}
				break;
			}
			case RPLFormat.BackgroundRepeatTypes.RepeatY:
			{
				for (int i = 0; i < num2; i += height)
				{
					DrawImageClipped(context, backgroundImage, num5, num6, position);
					num6 += num4;
				}
				break;
			}
			}
		}

		internal void ProcessBackgroundColorAndImage(GdiContext context, RPLElementProps properties)
		{
			m_backgroundColor = GdiContext.GetStylePropertyValueColor(properties, 34);
			object stylePropertyValueObject = SharedRenderer.GetStylePropertyValueObject(properties, 33);
			if (stylePropertyValueObject != null)
			{
				m_backgroundImage = GetImage(context, (RPLImageData)stylePropertyValueObject);
				object stylePropertyValueObject2 = SharedRenderer.GetStylePropertyValueObject(properties, 35);
				if (stylePropertyValueObject2 == null)
				{
					m_backgroundRepeat = RPLFormat.BackgroundRepeatTypes.Repeat;
				}
				else
				{
					m_backgroundRepeat = (RPLFormat.BackgroundRepeatTypes)stylePropertyValueObject2;
				}
			}
		}

		internal static Image GetImage(GdiContext context, RPLImageData imageData)
		{
			if (string.IsNullOrEmpty(imageData.ImageName) || !context.SharedImages.TryGetValue(imageData.ImageName, out Image value))
			{
				byte[] array = imageData.ImageData;
				if (array == null)
				{
					if (imageData.ImageDataOffset == -1)
					{
						return null;
					}
					array = context.RplReport.GetImage(imageData.ImageDataOffset);
					if (array == null)
					{
						return null;
					}
				}
				try
				{
					value = Image.FromStream(new MemoryStream(array));
				}
				catch
				{
					return null;
				}
				if (!string.IsNullOrEmpty(imageData.ImageName))
				{
					context.SharedImages.Add(imageData.ImageName, value);
				}
				byte[] array3 = imageData.ImageData = null;
				array = array3;
			}
			return value;
		}

		internal void DrawBorders(GdiContext context)
		{
			if (m_borderTop.IsVisible && (m_state & 1) == 0 && m_borderTop.Operations != null)
			{
				for (int i = 0; i < m_borderTop.Operations.Count; i++)
				{
					m_borderTop.Operations[i].Perform(context.GdiWriter);
				}
			}
			if (m_borderLeft.IsVisible && (m_state & 4) == 0 && m_borderLeft.Operations != null)
			{
				for (int j = 0; j < m_borderLeft.Operations.Count; j++)
				{
					m_borderLeft.Operations[j].Perform(context.GdiWriter);
				}
			}
			if (m_borderBottom.IsVisible && (m_state & 2) == 0 && m_borderBottom.Operations != null)
			{
				for (int k = 0; k < m_borderBottom.Operations.Count; k++)
				{
					m_borderBottom.Operations[k].Perform(context.GdiWriter);
				}
			}
			if (m_borderRight.IsVisible && (m_state & 8) == 0 && m_borderRight.Operations != null)
			{
				for (int l = 0; l < m_borderRight.Operations.Count; l++)
				{
					m_borderRight.Operations[l].Perform(context.GdiWriter);
				}
			}
		}

		internal static void ProcessBorders(GdiWriter writer, ref RenderingItemBorder top, ref RenderingItemBorder left, ref RenderingItemBorder bottom, ref RenderingItemBorder right, RectangleF position, RectangleF bounds, byte state)
		{
			if (!top.IsVisible && !left.IsVisible && !bottom.IsVisible && !right.IsVisible)
			{
				return;
			}
			left.Point = position.Left;
			left.Edge = position.Left;
			float borderLeftEdgeUnclipped = 0f;
			right.Point = position.Right;
			right.Edge = position.Right;
			float borderRightEdgeUnclipped = 0f;
			top.Point = position.Top;
			top.Edge = position.Top;
			float borderTopEdgeUnclipped = 0f;
			bottom.Point = position.Bottom;
			bottom.Edge = position.Bottom;
			float borderBottomEdgeUnclipped = 0f;
			float width = left.Width;
			float width2 = right.Width;
			float width3 = top.Width;
			float width4 = bottom.Width;
			if (left.Style == RPLFormat.BorderStyles.Double && left.Width < 0.5292f)
			{
				left.Style = RPLFormat.BorderStyles.Solid;
			}
			if (right.Style == RPLFormat.BorderStyles.Double && right.Width < 0.5292f)
			{
				right.Style = RPLFormat.BorderStyles.Solid;
			}
			if (top.Style == RPLFormat.BorderStyles.Double && top.Width < 0.5292f)
			{
				top.Style = RPLFormat.BorderStyles.Solid;
			}
			if (bottom.Style == RPLFormat.BorderStyles.Double && bottom.Width < 0.5292f)
			{
				bottom.Style = RPLFormat.BorderStyles.Solid;
			}
			if (left.IsVisible)
			{
				left.Edge -= left.Width / 2f;
				borderLeftEdgeUnclipped = left.Edge;
				if (left.Edge < bounds.Left)
				{
					float num = bounds.Left - left.Edge;
					left.Width -= num;
					left.Point += num / 2f;
					left.Edge = bounds.Left;
				}
			}
			else
			{
				left.Width = 0f;
			}
			if (right.IsVisible)
			{
				right.Edge += right.Width / 2f;
				borderRightEdgeUnclipped = right.Edge;
				if (right.Edge > bounds.Right)
				{
					float num2 = right.Edge - bounds.Right;
					right.Width -= num2;
					right.Point -= num2 / 2f;
					right.Edge = bounds.Right;
				}
			}
			else
			{
				right.Width = 0f;
			}
			if (top.IsVisible)
			{
				top.Edge -= top.Width / 2f;
				borderTopEdgeUnclipped = top.Edge;
				if (top.Edge < bounds.Top)
				{
					float num3 = bounds.Top - top.Edge;
					top.Width -= num3;
					top.Point += num3 / 2f;
					top.Edge = bounds.Top;
				}
			}
			else
			{
				top.Width = 0f;
			}
			if (bottom.IsVisible)
			{
				bottom.Edge += bottom.Width / 2f;
				borderBottomEdgeUnclipped = bottom.Edge;
				if (bottom.Edge > bounds.Bottom)
				{
					float num4 = bottom.Edge - bounds.Bottom;
					bottom.Width -= num4;
					bottom.Point -= num4 / 2f;
					bottom.Edge = bounds.Bottom;
				}
			}
			else
			{
				bottom.Width = 0f;
			}
			left.Edge = Math.Max(bounds.Left, left.Edge);
			right.Edge = Math.Min(bounds.Right, right.Edge);
			top.Edge = Math.Max(bounds.Top, top.Edge);
			bottom.Edge = Math.Min(bounds.Bottom, bottom.Edge);
			if (top.Style != RPLFormat.BorderStyles.Double && state == 0 && top.Style == left.Style && top.Style == bottom.Style && top.Style == right.Style && top.Width == left.Width && top.Width == bottom.Width && top.Width == right.Width && top.Color == left.Color && top.Color == bottom.Color && top.Color == right.Color)
			{
				RectangleF rectangle = new RectangleF(left.Point, top.Point, right.Point - left.Point, bottom.Point - top.Point);
				top.Operations = new List<Operation>(1);
				top.Operations.Add(new DrawRectangleOp(top.Color, top.Width, top.Style, rectangle));
				return;
			}
			float halfPixelWidthX = writer.HalfPixelWidthX;
			float halfPixelWidthY = writer.HalfPixelWidthY;
			float num5 = Math.Min(halfPixelWidthY, top.Width / 2f);
			float num6 = Math.Min(halfPixelWidthX, left.Width / 2f);
			float num7 = Math.Min(halfPixelWidthY, bottom.Width / 2f);
			float num8 = Math.Min(halfPixelWidthX, right.Width / 2f);
			top.Operations = new List<Operation>();
			SharedRenderer.ProcessTopBorder(writer, top.Operations, top.Width, top.Style, top.Color, left.Color, right.Color, top.Point, top.Edge, left.Edge + num6, right.Edge - num8, borderTopEdgeUnclipped, borderLeftEdgeUnclipped, borderRightEdgeUnclipped, left.Width, right.Width, width3, width, width2);
			right.Operations = new List<Operation>();
			SharedRenderer.ProcessRightBorder(writer, right.Operations, right.Width, right.Style, right.Color, top.Color, bottom.Color, right.Point, right.Edge, top.Edge + num5, bottom.Edge - num7, borderRightEdgeUnclipped, borderTopEdgeUnclipped, borderBottomEdgeUnclipped, top.Width, bottom.Width, width2, width3, width4);
			left.Operations = new List<Operation>();
			SharedRenderer.ProcessLeftBorder(writer, left.Operations, left.Width, left.Style, left.Color, top.Color, bottom.Color, left.Point, left.Edge, top.Edge + num5, bottom.Edge - num7, borderLeftEdgeUnclipped, borderTopEdgeUnclipped, borderBottomEdgeUnclipped, top.Width, bottom.Width, width, width3, width4);
			bottom.Operations = new List<Operation>();
			SharedRenderer.ProcessBottomBorder(writer, bottom.Operations, bottom.Width, bottom.Style, bottom.Color, left.Color, right.Color, bottom.Point, bottom.Edge, left.Edge + num6, right.Edge - num8, borderBottomEdgeUnclipped, borderLeftEdgeUnclipped, borderRightEdgeUnclipped, left.Width, right.Width, width4, width, width2);
		}

		internal void ProcessBorders(GdiWriter writer, RPLItemProps properties, RectangleF bounds, byte state)
		{
			RPLFormat.BorderStyles stylePropertyValueBorderStyle = SharedRenderer.GetStylePropertyValueBorderStyle(properties, 5, RPLFormat.BorderStyles.None);
			m_borderLeft.Style = SharedRenderer.GetStylePropertyValueBorderStyle(properties, 6, stylePropertyValueBorderStyle);
			m_borderRight.Style = SharedRenderer.GetStylePropertyValueBorderStyle(properties, 7, stylePropertyValueBorderStyle);
			m_borderTop.Style = SharedRenderer.GetStylePropertyValueBorderStyle(properties, 8, stylePropertyValueBorderStyle);
			m_borderBottom.Style = SharedRenderer.GetStylePropertyValueBorderStyle(properties, 9, stylePropertyValueBorderStyle);
			if (m_borderLeft.Style == RPLFormat.BorderStyles.None && m_borderRight.Style == RPLFormat.BorderStyles.None && m_borderTop.Style == RPLFormat.BorderStyles.None && m_borderBottom.Style == RPLFormat.BorderStyles.None)
			{
				return;
			}
			float stylePropertyValueSizeMM = GdiContext.GetStylePropertyValueSizeMM(properties, 10);
			m_borderLeft.Width = GdiContext.GetStylePropertyValueSizeMM(properties, 11);
			if (float.IsNaN(m_borderLeft.Width) && !float.IsNaN(stylePropertyValueSizeMM))
			{
				m_borderLeft.Width = stylePropertyValueSizeMM;
			}
			m_borderRight.Width = GdiContext.GetStylePropertyValueSizeMM(properties, 12);
			if (float.IsNaN(m_borderRight.Width) && !float.IsNaN(stylePropertyValueSizeMM))
			{
				m_borderRight.Width = stylePropertyValueSizeMM;
			}
			m_borderTop.Width = GdiContext.GetStylePropertyValueSizeMM(properties, 13);
			if (float.IsNaN(m_borderTop.Width) && !float.IsNaN(stylePropertyValueSizeMM))
			{
				m_borderTop.Width = stylePropertyValueSizeMM;
			}
			m_borderBottom.Width = GdiContext.GetStylePropertyValueSizeMM(properties, 14);
			if (float.IsNaN(m_borderBottom.Width) && !float.IsNaN(stylePropertyValueSizeMM))
			{
				m_borderBottom.Width = stylePropertyValueSizeMM;
			}
			if (!float.IsNaN(m_borderLeft.Width) || !float.IsNaN(m_borderRight.Width) || !float.IsNaN(m_borderTop.Width) || !float.IsNaN(m_borderBottom.Width))
			{
				Color stylePropertyValueColor = GdiContext.GetStylePropertyValueColor(properties, 0);
				m_borderLeft.Color = GdiContext.GetStylePropertyValueColor(properties, 1);
				if (m_borderLeft.Color == Color.Empty && stylePropertyValueColor != Color.Empty)
				{
					m_borderLeft.Color = stylePropertyValueColor;
				}
				m_borderRight.Color = GdiContext.GetStylePropertyValueColor(properties, 2);
				if (m_borderRight.Color == Color.Empty && stylePropertyValueColor != Color.Empty)
				{
					m_borderRight.Color = stylePropertyValueColor;
				}
				m_borderTop.Color = GdiContext.GetStylePropertyValueColor(properties, 3);
				if (m_borderTop.Color == Color.Empty && stylePropertyValueColor != Color.Empty)
				{
					m_borderTop.Color = stylePropertyValueColor;
				}
				m_borderBottom.Color = GdiContext.GetStylePropertyValueColor(properties, 4);
				if (m_borderBottom.Color == Color.Empty && stylePropertyValueColor != Color.Empty)
				{
					m_borderBottom.Color = stylePropertyValueColor;
				}
				if (!(m_borderLeft.Color == Color.Empty) || !(m_borderRight.Color == Color.Empty) || !(m_borderTop.Color == Color.Empty) || !(m_borderBottom.Color == Color.Empty))
				{
					ProcessBorders(writer, ref m_borderTop, ref m_borderLeft, ref m_borderBottom, ref m_borderRight, base.Position, bounds, state);
				}
			}
		}

		internal static DashStyle TranslateBorderStyle(object style)
		{
			if (style == null)
			{
				return DashStyle.Custom;
			}
			switch ((RPLFormat.BorderStyles)style)
			{
			case RPLFormat.BorderStyles.Dashed:
				return DashStyle.Dash;
			case RPLFormat.BorderStyles.Dotted:
				return DashStyle.Dot;
			case RPLFormat.BorderStyles.Solid:
			case RPLFormat.BorderStyles.Double:
				return DashStyle.Solid;
			default:
				return DashStyle.Custom;
			}
		}

		private static void DrawImageClipped(GdiContext context, Image image, float xOffsetMM, float yOffsetMM, RectangleF position)
		{
			int width = image.Width;
			int height = image.Height;
			RectangleF rectangleF = default(RectangleF);
			rectangleF.X = position.Left + xOffsetMM;
			rectangleF.Width = position.Width - xOffsetMM;
			rectangleF.Y = position.Top + yOffsetMM;
			rectangleF.Height = position.Height - yOffsetMM;
			RectangleF srcRect = new RectangleF(0f, 0f, width, height);
			srcRect.Width = Math.Min(width, context.GdiWriter.ConvertToPixels(rectangleF.Width));
			srcRect.Height = Math.Min(height, context.GdiWriter.ConvertToPixels(rectangleF.Height));
			rectangleF.Width = Math.Min(rectangleF.Width, context.GdiWriter.ConvertToMillimeters(width));
			rectangleF.Height = Math.Min(rectangleF.Height, context.GdiWriter.ConvertToMillimeters(height));
			ImageAttributes imageAttributes = null;
			try
			{
				imageAttributes = new ImageAttributes();
				imageAttributes.SetWrapMode(WrapMode.Tile);
				PointF[] destPoints = new PointF[3]
				{
					rectangleF.Location,
					new PointF(rectangleF.Location.X + rectangleF.Width, rectangleF.Location.Y),
					new PointF(rectangleF.Location.X, rectangleF.Location.Y + rectangleF.Height)
				};
				context.Graphics.DrawImage(image, destPoints, srcRect, GraphicsUnit.Pixel, imageAttributes);
			}
			finally
			{
				if (imageAttributes != null)
				{
					imageAttributes.Dispose();
					imageAttributes = null;
				}
			}
		}
	}
}
