using System;
using System.Collections;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Xml;

namespace Microsoft.Reporting.Map.WebForms
{
	internal class XamlRenderer : IDisposable
	{
		private XmlDocument xaml;

		private bool allowPathGradientTransform = true;

		private Color[] layerHues;

		private XamlLayer[] layers;

		private bool disposed;

		public XmlDocument Xaml => xaml;

		public bool AllowPathGradientTransform
		{
			get
			{
				return allowPathGradientTransform;
			}
			set
			{
				allowPathGradientTransform = value;
			}
		}

		private Color[] LayerHues => layerHues;

		public XamlLayer[] Layers
		{
			get
			{
				return layers;
			}
			set
			{
				if (layers != value && layers != null)
				{
					XamlLayer[] array = layers;
					for (int i = 0; i < array.Length; i++)
					{
						array[i]?.Dispose();
					}
				}
				layers = value;
			}
		}

		public XamlRenderer(string xamlResource)
		{
			xaml = new XmlDocument();
			Stream manifestResourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(typeof(MapControl).Namespace + ".Xaml." + xamlResource);
			xaml.Load(manifestResourceStream);
			manifestResourceStream.Close();
		}

		public void ParseXaml(RectangleF viewportRect, Color[] layerHues)
		{
			this.layerHues = layerHues;
			XmlNode xmlNode = Xaml.DocumentElement;
			if (xmlNode.Name == "Viewbox")
			{
				xmlNode = FindChildNode(xmlNode, "Canvas");
			}
			RectangleF rootCanvasBoundingRectangle = GetRootCanvasBoundingRectangle(xmlNode);
			int num = CountChildNodes(xmlNode);
			Layers = new XamlLayer[num];
			int num2 = 0;
			foreach (XmlNode item in xmlNode)
			{
				if (item.NodeType != XmlNodeType.Comment)
				{
					Layers[num2] = ParseCanvas(item, num2, rootCanvasBoundingRectangle, viewportRect);
					num2++;
				}
			}
		}

		private RectangleF GetRootCanvasBoundingRectangle(XmlNode rootCanvas)
		{
			XmlAttribute xmlAttribute = rootCanvas.Attributes["Width"];
			XmlAttribute xmlAttribute2 = rootCanvas.Attributes["Height"];
			if (xmlAttribute == null || xmlAttribute2 == null)
			{
				throw new Exception(SR.ExceptionXamlInvalidCanvasSize);
			}
			float width = float.Parse(xmlAttribute.Value, CultureInfo.InvariantCulture);
			float height = float.Parse(xmlAttribute2.Value, CultureInfo.InvariantCulture);
			return new RectangleF(0f, 0f, width, height);
		}

		private RectangleF GetCanvasBoundingRectangle(XmlNode canvasNode)
		{
			RectangleF rectangleF = RectangleF.Empty;
			foreach (XmlNode childNode in canvasNode.ChildNodes)
			{
				if (childNode.NodeType == XmlNodeType.Comment)
				{
					continue;
				}
				RectangleF rectangleF2 = default(RectangleF);
				if (childNode.Name == "Canvas")
				{
					rectangleF2 = GetCanvasBoundingRectangle(childNode);
				}
				else
				{
					XmlAttribute xmlAttribute = childNode.Attributes["Canvas.Left"];
					XmlAttribute xmlAttribute2 = childNode.Attributes["Canvas.Top"];
					XmlAttribute xmlAttribute3 = childNode.Attributes["Width"];
					XmlAttribute xmlAttribute4 = childNode.Attributes["Height"];
					if (childNode.Name == "Path" && xmlAttribute == null && xmlAttribute2 == null)
					{
						string[] streamGeometryParts = GetStreamGeometryParts(childNode.Attributes["Data"].Value);
						rectangleF2 = GetStreamGeometryBounds(streamGeometryParts, includeOrigin: false);
					}
					else
					{
						rectangleF2.X = float.Parse(xmlAttribute.Value, CultureInfo.InvariantCulture);
						rectangleF2.Y = float.Parse(xmlAttribute2.Value, CultureInfo.InvariantCulture);
						rectangleF2.Width = float.Parse(xmlAttribute3.Value, CultureInfo.InvariantCulture);
						rectangleF2.Height = float.Parse(xmlAttribute4.Value, CultureInfo.InvariantCulture);
					}
				}
				rectangleF = ((!(rectangleF == RectangleF.Empty)) ? RectangleF.Union(rectangleF, rectangleF2) : rectangleF2);
			}
			return rectangleF;
		}

		private XamlLayer ParseCanvas(XmlNode canvasNode, int layerIndex, RectangleF fromBounds, RectangleF toBounds)
		{
			XamlLayer xamlLayer = new XamlLayer(toBounds.Location);
			toBounds.Location = PointF.Empty;
			if (FindChildNode(canvasNode, "Canvas") != null)
			{
				RectangleF[] slicedBounds = GetSlicedBounds(canvasNode);
				RectangleF[] array = TransformSlicedBounds(slicedBounds, fromBounds, toBounds);
				int num = CountChildNodes(canvasNode);
				xamlLayer.InnerLayers = new XamlLayer[num];
				int num2 = 0;
				{
					foreach (XmlNode item in canvasNode)
					{
						if (item.NodeType != XmlNodeType.Comment)
						{
							xamlLayer.InnerLayers[num2] = ParseCanvas(item, layerIndex, slicedBounds[num2], array[num2]);
							num2++;
						}
					}
					return xamlLayer;
				}
			}
			int num3 = 0;
			int num4 = CountChildNodes(canvasNode);
			xamlLayer.Paths = new GraphicsPath[num4];
			xamlLayer.Brushes = new Brush[num4];
			xamlLayer.Pens = new Pen[num4];
			foreach (XmlNode childNode in canvasNode.ChildNodes)
			{
				if (childNode.NodeType == XmlNodeType.Comment)
				{
					continue;
				}
				RectangleF rectangleF = default(RectangleF);
				XmlAttribute xmlAttribute = childNode.Attributes["Canvas.Left"];
				XmlAttribute xmlAttribute2 = childNode.Attributes["Canvas.Top"];
				XmlAttribute xmlAttribute3 = childNode.Attributes["Width"];
				XmlAttribute xmlAttribute4 = childNode.Attributes["Height"];
				XmlAttribute xmlAttribute5 = childNode.Attributes["Stretch"];
				bool flag = false;
				if (xmlAttribute5 != null)
				{
					flag = (xmlAttribute5.Value == "Fill");
				}
				bool includeOrigin = false;
				if (childNode.Name == "Path" && xmlAttribute == null && xmlAttribute2 == null)
				{
					string[] streamGeometryParts = GetStreamGeometryParts(childNode.Attributes["Data"].Value);
					includeOrigin = true;
					rectangleF = GetStreamGeometryBounds(streamGeometryParts, includeOrigin);
				}
				else
				{
					if (xmlAttribute != null)
					{
						rectangleF.X = float.Parse(xmlAttribute.Value, CultureInfo.InvariantCulture);
					}
					if (xmlAttribute2 != null)
					{
						rectangleF.Y = float.Parse(xmlAttribute2.Value, CultureInfo.InvariantCulture);
					}
					rectangleF.Width = float.Parse(xmlAttribute3.Value, CultureInfo.InvariantCulture);
					rectangleF.Height = float.Parse(xmlAttribute4.Value, CultureInfo.InvariantCulture);
				}
				PointF location = rectangleF.Location;
				RectangleF originalShapeRect = rectangleF;
				rectangleF = TransformRectangle(rectangleF, fromBounds, toBounds);
				GraphicsPath graphicsPath = new GraphicsPath();
				if (childNode.Name == "Ellipse")
				{
					graphicsPath.AddEllipse(rectangleF);
				}
				else if (childNode.Name == "Path")
				{
					XmlAttribute xmlAttribute6 = childNode.Attributes["Data"];
					if (xmlAttribute6 != null)
					{
						string[] streamGeometryParts2 = GetStreamGeometryParts(xmlAttribute6.Value);
						float stretchFactorX = 1f;
						float stretchFactorY = 1f;
						if (flag)
						{
							RectangleF streamGeometryBounds = GetStreamGeometryBounds(streamGeometryParts2, includeOrigin: false);
							stretchFactorX = originalShapeRect.Width / streamGeometryBounds.Width;
							stretchFactorY = originalShapeRect.Height / streamGeometryBounds.Height;
						}
						IntepretStreamGeometry(streamGeometryParts2, location, stretchFactorX, stretchFactorY, includeOrigin, fromBounds, toBounds, ref graphicsPath);
					}
				}
				else
				{
					if (!(childNode.Name == "Rectangle"))
					{
						throw new Exception(SR.ExceptionXamlShapeNotSupported(childNode.Name));
					}
					graphicsPath.AddRectangle(rectangleF);
				}
				xamlLayer.Paths[num3] = graphicsPath;
				Brush brush = null;
				XmlAttribute xmlAttribute7 = childNode.Attributes["Fill"];
				XmlNode xmlNode3 = FindFillNode(childNode);
				if (xmlAttribute7 != null)
				{
					Color color = ColorTranslator.FromHtml(xmlAttribute7.Value);
					brush = new SolidBrush(TransformColor(color, layerIndex));
				}
				else if (xmlNode3 != null)
				{
					brush = CreateBrush(FindChildNode(xmlNode3, "*"), layerIndex, rectangleF, originalShapeRect, fromBounds, toBounds);
				}
				xamlLayer.Brushes[num3] = brush;
				Pen pen = null;
				XmlAttribute xmlAttribute8 = childNode.Attributes["Stroke"];
				XmlNode xmlNode4 = FindStrokeNode(childNode);
				if (xmlAttribute8 != null)
				{
					Color color2 = ColorTranslator.FromHtml(xmlAttribute8.Value);
					pen = new Pen(TransformColor(color2, layerIndex));
				}
				else if (xmlNode4 != null)
				{
					pen = new Pen(CreateBrush(FindChildNode(xmlNode4, "*"), layerIndex, rectangleF, originalShapeRect, fromBounds, toBounds));
				}
				XmlAttribute xmlAttribute9 = childNode.Attributes["StrokeThickness"];
				if (xmlAttribute9 != null)
				{
					float num5 = float.Parse(xmlAttribute9.Value, CultureInfo.InvariantCulture);
					num5 = (pen.Width = num5 * (toBounds.Width / fromBounds.Width));
				}
				XmlAttribute xmlAttribute10 = childNode.Attributes["StrokeLineJoin"];
				if (xmlAttribute10 != null)
				{
					pen.LineJoin = (LineJoin)Enum.Parse(typeof(LineJoin), xmlAttribute10.Value);
				}
				xamlLayer.Pens[num3] = pen;
				num3++;
			}
			return xamlLayer;
		}

		private RectangleF[] TransformSlicedBounds(RectangleF[] slicedBounds, RectangleF fromBounds, RectangleF toBounds)
		{
			RectangleF[] array = new RectangleF[slicedBounds.Length];
			float num = toBounds.Width / toBounds.Height;
			float width = slicedBounds[slicedBounds.Length - 5].Width;
			float height = slicedBounds[slicedBounds.Length - 8].Height;
			for (int num2 = slicedBounds.Length - 1; num2 >= 0; num2--)
			{
				int num3 = slicedBounds.Length - num2;
				RectangleF rect = slicedBounds[num2];
				switch (num3)
				{
				case 1:
					if (num > 1f)
					{
						rect.X /= num;
						rect.Width /= num;
					}
					else
					{
						rect.Y *= num;
						rect.Height *= num;
					}
					break;
				case 2:
					if (num > 1f)
					{
						rect.X = 100f - (100f - rect.X) / num;
						rect.Width /= num;
					}
					else
					{
						rect.Y *= num;
						rect.Height *= num;
					}
					break;
				case 3:
					if (num > 1f)
					{
						rect.X = 100f - (100f - rect.X) / num;
						rect.Width /= num;
					}
					else
					{
						rect.Y = 100f - (100f - rect.Y) * num;
						rect.Height *= num;
					}
					break;
				case 4:
					if (num > 1f)
					{
						rect.X /= num;
						rect.Width /= num;
					}
					else
					{
						rect.Y = 100f - (100f - rect.Y) * num;
						rect.Height *= num;
					}
					break;
				case 5:
					if (num > 1f)
					{
						float num8 = (fromBounds.Width - width) / 2f;
						rect.X = num8 / num;
						rect.Width = fromBounds.Width - 2f * num8 / num;
					}
					else
					{
						rect.Y *= num;
						rect.Height *= num;
					}
					break;
				case 6:
					if (num > 1f)
					{
						rect.X = 100f - (100f - rect.X) / num;
						rect.Width /= num;
					}
					else
					{
						float num6 = (fromBounds.Height - height) / 2f;
						rect.Y = num6 * num;
						rect.Height = fromBounds.Height - 2f * num6 * num;
					}
					break;
				case 7:
					if (num > 1f)
					{
						float num9 = (fromBounds.Width - width) / 2f;
						rect.X = num9 / num;
						rect.Width = fromBounds.Width - 2f * num9 / num;
					}
					else
					{
						rect.Y = 100f - (100f - rect.Y) * num;
						rect.Height *= num;
					}
					break;
				case 8:
					if (num > 1f)
					{
						rect.X /= num;
						rect.Width /= num;
					}
					else
					{
						float num7 = (fromBounds.Height - height) / 2f;
						rect.Y = num7 * num;
						rect.Height = fromBounds.Height - 2f * num7 * num;
					}
					break;
				case 9:
					if (num > 1f)
					{
						float num4 = (fromBounds.Width - width) / 2f;
						rect.X = num4 / num;
						rect.Width = fromBounds.Width - 2f * num4 / num;
					}
					else
					{
						float num5 = (fromBounds.Height - height) / 2f;
						rect.Y = num5 * num;
						rect.Height = fromBounds.Height - 2f * num5 * num;
					}
					break;
				}
				array[num2] = TransformRectangle(rect, fromBounds, toBounds);
			}
			return array;
		}

		private RectangleF[] GetSlicedBounds(XmlNode canvasNode)
		{
			RectangleF[] array = new RectangleF[CountChildNodes(canvasNode)];
			int num = 0;
			foreach (XmlNode item in canvasNode)
			{
				if (item.NodeType != XmlNodeType.Comment)
				{
					array[num] = GetCanvasBoundingRectangle(item);
					num++;
				}
			}
			return array;
		}

		private Brush CreateBrush(XmlNode brushNode, int layerIndex, RectangleF shapeRect, RectangleF originalShapeRect, RectangleF fromBounds, RectangleF toBounds)
		{
			Brush brush = null;
			if (brushNode.Name == "LinearGradientBrush")
			{
				PointF startPoint = ParsePoint(brushNode.Attributes["StartPoint"].Value);
				PointF endPoint = ParsePoint(brushNode.Attributes["EndPoint"].Value);
				bool flag = false;
				XmlAttribute xmlAttribute = brushNode.Attributes["MappingMode"];
				if (xmlAttribute != null && xmlAttribute.Value == "Absolute")
				{
					flag = true;
				}
				if (flag)
				{
					XmlNode xmlNode = FindChildNode(brushNode, "LinearGradientBrush.Transform");
					if (xmlNode != null && xmlNode.HasChildNodes)
					{
						ApplyTransform(xmlNode, shapeRect, fromBounds, toBounds, ref startPoint, ref endPoint);
					}
					startPoint.X += originalShapeRect.X;
					startPoint.Y += originalShapeRect.Y;
					startPoint = AbsoluteToRelative(startPoint, originalShapeRect);
					endPoint.X += originalShapeRect.X;
					endPoint.Y += originalShapeRect.Y;
					endPoint = AbsoluteToRelative(endPoint, originalShapeRect);
				}
				startPoint = RelativeToAbsolute(startPoint, shapeRect);
				endPoint = RelativeToAbsolute(endPoint, shapeRect);
				float stretchFactor = CalculateStretchFactor(startPoint, endPoint, shapeRect);
				StretchBrushPoints(stretchFactor, ref startPoint, ref endPoint);
				brush = new LinearGradientBrush(startPoint, endPoint, Color.Black, Color.Black);
				XmlNode xmlNode2 = FindChildNode(brushNode, "LinearGradientBrush.GradientStops");
				if (xmlNode2 == null && FindChildNode(brushNode, "GradientStop") != null)
				{
					xmlNode2 = brushNode;
				}
				if (xmlNode2 != null)
				{
					((LinearGradientBrush)brush).InterpolationColors = CreateColorBlend(xmlNode2, layerIndex, stretchFactor, radialBrush: false);
				}
				XmlNode xmlNode3 = FindChildNode(brushNode, "LinearGradientBrush.RelativeTransform");
				if (xmlNode3 != null && xmlNode3.HasChildNodes)
				{
					ApplyRelativeTransform(xmlNode3, originalShapeRect, shapeRect, fromBounds, toBounds, ref brush);
				}
			}
			else
			{
				if (!(brushNode.Name == "RadialGradientBrush"))
				{
					throw new Exception(SR.ExceptionXamlBrushNotSupported(brushNode.Name));
				}
				float num = 0.5f;
				XmlAttribute xmlAttribute2 = brushNode.Attributes["RadiusX"];
				if (xmlAttribute2 != null)
				{
					num = float.Parse(xmlAttribute2.Value, CultureInfo.InvariantCulture);
				}
				float num2 = 0.5f;
				XmlAttribute xmlAttribute3 = brushNode.Attributes["RadiusY"];
				if (xmlAttribute3 != null)
				{
					num2 = float.Parse(xmlAttribute3.Value, CultureInfo.InvariantCulture);
				}
				float num3 = (float)Math.Sqrt(num * 2f);
				float num4 = (float)Math.Sqrt(num2 * 2f);
				RectangleF rect = shapeRect;
				rect.Inflate(shapeRect.Size.Width * (num3 - 1f) / 2f, shapeRect.Size.Height * (num4 - 1f) / 2f);
				PointF relativePoint = new PointF(0.5f, 0.5f);
				XmlAttribute xmlAttribute4 = brushNode.Attributes["Center"];
				if (xmlAttribute4 != null)
				{
					relativePoint = ParsePoint(xmlAttribute4.Value);
				}
				PointF pointF = new PointF(shapeRect.X + shapeRect.Width / 2f, shapeRect.Y + shapeRect.Height / 2f);
				PointF pointF2 = RelativeToAbsolute(relativePoint, shapeRect);
				rect.Offset(pointF2.X - pointF.X, pointF2.Y - pointF.Y);
				float num5 = 8f;
				rect.Inflate(rect.Size.Width * (num5 - 1f) / 2f, rect.Size.Height * (num5 - 1f) / 2f);
				GraphicsPath graphicsPath = new GraphicsPath();
				graphicsPath.AddEllipse(rect);
				brush = new PathGradientBrush(graphicsPath);
				PointF relativePoint2 = new PointF(0.5f, 0.5f);
				XmlAttribute xmlAttribute5 = brushNode.Attributes["GradientOrigin"];
				if (xmlAttribute5 != null)
				{
					relativePoint2 = ParsePoint(xmlAttribute5.Value);
				}
				((PathGradientBrush)brush).CenterPoint = RelativeToAbsolute(relativePoint2, shapeRect);
				XmlNode xmlNode4 = FindChildNode(brushNode, "RadialGradientBrush.GradientStops");
				if (xmlNode4 == null && FindChildNode(brushNode, "GradientStop") != null)
				{
					xmlNode4 = brushNode;
				}
				if (xmlNode4 != null)
				{
					((PathGradientBrush)brush).InterpolationColors = CreateColorBlend(xmlNode4, layerIndex, num5, radialBrush: true);
				}
				XmlNode xmlNode5 = FindChildNode(brushNode, "RadialGradientBrush.RelativeTransform");
				if (xmlNode5 != null && xmlNode5.HasChildNodes)
				{
					ApplyRelativeTransform(xmlNode5, originalShapeRect, shapeRect, fromBounds, toBounds, ref brush);
				}
				XmlNode xmlNode6 = FindChildNode(brushNode, "RadialGradientBrush.Transform");
				if (xmlNode6 != null && xmlNode6.HasChildNodes)
				{
					ApplyTransform(xmlNode6, shapeRect, fromBounds, toBounds, ref brush);
				}
			}
			return brush;
		}

		private void ApplyTransform(XmlNode transformNode, RectangleF shapeRect, RectangleF fromBounds, RectangleF toBounds, ref PointF startPoint, ref PointF endPoint)
		{
			XmlNode xmlNode = FindChildNode(transformNode, "TransformGroup");
			if (xmlNode == null)
			{
				xmlNode = transformNode;
			}
			XmlNode xmlNode2 = FindChildNode(xmlNode, "MatrixTransform");
			if (xmlNode2 == null)
			{
				return;
			}
			XmlAttribute xmlAttribute = xmlNode2.Attributes["Matrix"];
			if (xmlAttribute != null)
			{
				string[] array = xmlAttribute.Value.Split(',');
				float[] array2 = new float[6];
				for (int i = 0; i < array2.Length; i++)
				{
					array2[i] = float.Parse(array[i], CultureInfo.InvariantCulture);
				}
				Matrix matrix = new Matrix(array2[0], array2[1], array2[2], array2[3], array2[4], array2[5]);
				PointF[] array3 = new PointF[2]
				{
					startPoint,
					endPoint
				};
				matrix.TransformPoints(array3);
				startPoint = array3[0];
				endPoint = array3[1];
			}
		}

		private void ApplyTransform(XmlNode transformNode, RectangleF shapeRect, RectangleF fromBounds, RectangleF toBounds, ref Brush brush)
		{
			XmlNode xmlNode = FindChildNode(transformNode, "TransformGroup");
			if (xmlNode == null)
			{
				xmlNode = transformNode;
			}
			XmlNode xmlNode2 = FindChildNode(xmlNode, "MatrixTransform");
			if (xmlNode2 == null)
			{
				return;
			}
			Matrix matrix = null;
			XmlAttribute xmlAttribute = xmlNode2.Attributes["Matrix"];
			if (xmlAttribute != null)
			{
				string[] array = xmlAttribute.Value.Split(',');
				float[] array2 = new float[6];
				for (int i = 0; i < array2.Length; i++)
				{
					array2[i] = float.Parse(array[i], CultureInfo.InvariantCulture);
				}
				PointF pointF = new PointF(array2[4], array2[5]);
				pointF.X /= fromBounds.Width;
				pointF.Y /= fromBounds.Height;
				pointF.X *= toBounds.Width;
				pointF.Y *= toBounds.Height;
				Matrix matrix2 = new Matrix(array2[0], array2[1], array2[2], array2[3], 0f, 0f);
				PointF[] array3 = new PointF[1]
				{
					new PointF(shapeRect.X, shapeRect.Y)
				};
				matrix2.TransformPoints(array3);
				pointF.X += shapeRect.X - array3[0].X;
				pointF.Y += shapeRect.Y - array3[0].Y;
				array2[4] = pointF.X;
				array2[5] = pointF.Y;
				matrix = new Matrix(array2[0], array2[1], array2[2], array2[3], array2[4], array2[5]);
			}
			if (matrix != null && brush is LinearGradientBrush)
			{
				Matrix matrix3 = ((LinearGradientBrush)brush).Transform.Clone();
				matrix3.Multiply(matrix, MatrixOrder.Append);
				((LinearGradientBrush)brush).Transform = matrix3;
			}
			else if (matrix != null && brush is PathGradientBrush && AllowPathGradientTransform)
			{
				Matrix matrix4 = ((PathGradientBrush)brush).Transform.Clone();
				matrix4.Multiply(matrix, MatrixOrder.Append);
				((PathGradientBrush)brush).Transform = matrix4;
			}
		}

		private void ApplyRelativeTransform(XmlNode transformNode, RectangleF originalShapeRect, RectangleF shapeRect, RectangleF fromBounds, RectangleF toBounds, ref Brush brush)
		{
			XmlNode xmlNode = FindChildNode(transformNode, "TransformGroup");
			if (xmlNode == null)
			{
				xmlNode = transformNode;
			}
			XmlNode xmlNode2 = FindChildNode(xmlNode, "ScaleTransform");
			if (xmlNode2 != null)
			{
				PointF relativePoint = new PointF(0f, 0f);
				XmlAttribute xmlAttribute = xmlNode2.Attributes["CenterX"];
				if (xmlAttribute != null)
				{
					relativePoint.X = float.Parse(xmlAttribute.Value, CultureInfo.InvariantCulture);
				}
				XmlAttribute xmlAttribute2 = xmlNode2.Attributes["CenterY"];
				if (xmlAttribute2 != null)
				{
					relativePoint.Y = float.Parse(xmlAttribute2.Value, CultureInfo.InvariantCulture);
				}
				relativePoint = RelativeToAbsolute(relativePoint, shapeRect);
				float scaleX = 1f;
				XmlAttribute xmlAttribute3 = xmlNode2.Attributes["ScaleX"];
				if (xmlAttribute3 != null)
				{
					scaleX = float.Parse(xmlAttribute3.Value, CultureInfo.InvariantCulture);
				}
				float scaleY = 1f;
				XmlAttribute xmlAttribute4 = xmlNode2.Attributes["ScaleY"];
				if (xmlAttribute4 != null)
				{
					scaleY = float.Parse(xmlAttribute4.Value, CultureInfo.InvariantCulture);
				}
				if (brush is LinearGradientBrush)
				{
					Matrix matrix = ((LinearGradientBrush)brush).Transform.Clone();
					matrix.Translate(0f - relativePoint.X, 0f - relativePoint.Y, MatrixOrder.Append);
					matrix.Scale(scaleX, scaleY, MatrixOrder.Append);
					matrix.Translate(relativePoint.X, relativePoint.Y, MatrixOrder.Append);
					((LinearGradientBrush)brush).Transform = matrix;
				}
				else if (brush is PathGradientBrush && AllowPathGradientTransform)
				{
					Matrix matrix2 = ((PathGradientBrush)brush).Transform.Clone();
					matrix2.Translate(0f - relativePoint.X, 0f - relativePoint.Y, MatrixOrder.Append);
					matrix2.Scale(scaleX, scaleY, MatrixOrder.Append);
					matrix2.Translate(relativePoint.X, relativePoint.Y, MatrixOrder.Append);
					((PathGradientBrush)brush).Transform = matrix2;
				}
			}
			XmlNode xmlNode3 = FindChildNode(xmlNode, "SkewTransform");
			if (xmlNode3 != null)
			{
				float num = 0f;
				XmlAttribute xmlAttribute5 = xmlNode3.Attributes["AngleX"];
				if (xmlAttribute5 != null)
				{
					num = float.Parse(xmlAttribute5.Value, CultureInfo.InvariantCulture);
				}
				num = (float)Math.Tan((double)num * Math.PI / 180.0) * shapeRect.Width / shapeRect.Height;
				float num2 = 0f;
				XmlAttribute xmlAttribute6 = xmlNode3.Attributes["AngleY"];
				if (xmlAttribute6 != null)
				{
					num2 = float.Parse(xmlAttribute6.Value, CultureInfo.InvariantCulture);
				}
				num2 = (float)Math.Tan((double)num2 * Math.PI / 180.0) * shapeRect.Height / shapeRect.Width;
				if (brush is LinearGradientBrush)
				{
					Matrix matrix3 = ((LinearGradientBrush)brush).Transform.Clone();
					matrix3.Translate(0f - shapeRect.X, 0f - shapeRect.Y, MatrixOrder.Append);
					matrix3.Shear(num, num2, MatrixOrder.Append);
					matrix3.Translate(shapeRect.X, shapeRect.Y, MatrixOrder.Append);
					((LinearGradientBrush)brush).Transform = matrix3;
				}
				else if (brush is PathGradientBrush && AllowPathGradientTransform)
				{
					Matrix matrix4 = ((PathGradientBrush)brush).Transform.Clone();
					matrix4.Translate(0f - shapeRect.X, 0f - shapeRect.Y, MatrixOrder.Append);
					matrix4.Shear(num, num2, MatrixOrder.Append);
					matrix4.Translate(shapeRect.X, shapeRect.Y, MatrixOrder.Append);
					((PathGradientBrush)brush).Transform = matrix4;
				}
			}
			XmlNode xmlNode4 = FindChildNode(xmlNode, "RotateTransform");
			if (xmlNode4 != null)
			{
				PointF relativePoint2 = new PointF(0.5f, 0.5f);
				XmlAttribute xmlAttribute7 = xmlNode4.Attributes["CenterX"];
				if (xmlAttribute7 != null)
				{
					relativePoint2.X = float.Parse(xmlAttribute7.Value, CultureInfo.InvariantCulture);
				}
				XmlAttribute xmlAttribute8 = xmlNode4.Attributes["CenterY"];
				if (xmlAttribute8 != null)
				{
					relativePoint2.Y = float.Parse(xmlAttribute8.Value, CultureInfo.InvariantCulture);
				}
				relativePoint2 = RelativeToAbsolute(relativePoint2, shapeRect);
				float angle = 0f;
				XmlAttribute xmlAttribute9 = xmlNode4.Attributes["Angle"];
				if (xmlAttribute9 != null)
				{
					angle = float.Parse(xmlAttribute9.Value, CultureInfo.InvariantCulture);
				}
				if (brush is LinearGradientBrush)
				{
					Matrix matrix5 = ((LinearGradientBrush)brush).Transform.Clone();
					matrix5.RotateAt(angle, relativePoint2, MatrixOrder.Append);
					((LinearGradientBrush)brush).Transform = matrix5;
				}
				else if (brush is PathGradientBrush && AllowPathGradientTransform)
				{
					Matrix matrix6 = ((PathGradientBrush)brush).Transform.Clone();
					matrix6.RotateAt(angle, relativePoint2, MatrixOrder.Append);
					((PathGradientBrush)brush).Transform = matrix6;
				}
			}
			XmlNode xmlNode5 = FindChildNode(xmlNode, "TranslateTransform");
			if (xmlNode5 != null)
			{
				PointF relativePoint3 = new PointF(0f, 0f);
				XmlAttribute xmlAttribute10 = xmlNode5.Attributes["X"];
				if (xmlAttribute10 != null)
				{
					relativePoint3.X = float.Parse(xmlAttribute10.Value, CultureInfo.InvariantCulture);
				}
				XmlAttribute xmlAttribute11 = xmlNode5.Attributes["Y"];
				if (xmlAttribute11 != null)
				{
					relativePoint3.Y = float.Parse(xmlAttribute11.Value, CultureInfo.InvariantCulture);
				}
				relativePoint3 = RelativeToAbsolute(relativePoint3, shapeRect);
				relativePoint3.X -= shapeRect.X;
				relativePoint3.Y -= shapeRect.Y;
				if (brush is LinearGradientBrush)
				{
					Matrix matrix7 = ((LinearGradientBrush)brush).Transform.Clone();
					matrix7.Translate(relativePoint3.X, relativePoint3.Y, MatrixOrder.Append);
					((LinearGradientBrush)brush).Transform = matrix7;
				}
				else if (brush is PathGradientBrush && AllowPathGradientTransform)
				{
					Matrix matrix8 = ((PathGradientBrush)brush).Transform.Clone();
					matrix8.Translate(relativePoint3.X, relativePoint3.Y, MatrixOrder.Append);
					((PathGradientBrush)brush).Transform = matrix8;
				}
			}
		}

		private void StretchBrushPoints(float stretchFactor, ref PointF startPoint, ref PointF endPoint)
		{
			PointF pointF = new PointF(startPoint.X + (endPoint.X - startPoint.X) / 2f, startPoint.Y + (endPoint.Y - startPoint.Y) / 2f);
			startPoint.X -= pointF.X;
			startPoint.Y -= pointF.Y;
			endPoint.X -= pointF.X;
			endPoint.Y -= pointF.Y;
			startPoint.X *= stretchFactor;
			startPoint.Y *= stretchFactor;
			endPoint.X *= stretchFactor;
			endPoint.Y *= stretchFactor;
			startPoint.X += pointF.X;
			startPoint.Y += pointF.Y;
			endPoint.X += pointF.X;
			endPoint.Y += pointF.Y;
		}

		private float CalculateStretchFactor(PointF startPoint, PointF endPoint, RectangleF shapeRect)
		{
			float num = (float)Math.Sqrt((endPoint.X - startPoint.X) * (endPoint.X - startPoint.X) + (endPoint.Y - startPoint.Y) * (endPoint.Y - startPoint.Y));
			return (float)Math.Sqrt(shapeRect.Width * shapeRect.Width + shapeRect.Height * shapeRect.Height) / num * 10f;
		}

		private ColorBlend CreateColorBlend(XmlNode gradientStopsNode, int layerIndex, float stretchFactor, bool radialBrush)
		{
			SortedList sortedList = new SortedList();
			foreach (XmlNode childNode in gradientStopsNode.ChildNodes)
			{
				if (!(childNode.Name != "GradientStop"))
				{
					Color color = ColorTranslator.FromHtml(childNode.Attributes["Color"].Value);
					color = TransformColor(color, layerIndex);
					float num = float.Parse(childNode.Attributes["Offset"].Value, CultureInfo.InvariantCulture);
					if (radialBrush)
					{
						num /= stretchFactor;
						num = 1f - num;
					}
					else
					{
						num -= 0.5f;
						num /= stretchFactor;
						num += 0.5f;
					}
					sortedList.Add(num, color);
				}
			}
			ColorBlend colorBlend = new ColorBlend(sortedList.Count + 2);
			int num2 = 1;
			foreach (DictionaryEntry item in sortedList)
			{
				colorBlend.Positions[num2] = (float)item.Key;
				colorBlend.Colors[num2] = (Color)item.Value;
				num2++;
			}
			colorBlend.Positions[0] = 0f;
			colorBlend.Colors[0] = (Color)sortedList.GetValueList()[0];
			colorBlend.Positions[num2] = 1f;
			colorBlend.Colors[num2] = (Color)sortedList.GetValueList()[sortedList.Count - 1];
			return colorBlend;
		}

		private RectangleF GetStreamGeometryBounds(string[] parts, bool includeOrigin)
		{
			RectangleF rectangleF = RectangleF.Empty;
			using (GraphicsPath graphicsPath = new GraphicsPath())
			{
				PointF pointF = PointF.Empty;
				for (int i = 0; i < parts.Length; i++)
				{
					if (parts[i] == "F0" || parts[i] == "f0")
					{
						graphicsPath.FillMode = FillMode.Alternate;
					}
					else if (parts[i] == "F1" || parts[i] == "f1")
					{
						graphicsPath.FillMode = FillMode.Winding;
					}
					else if (parts[i] == "M")
					{
						graphicsPath.StartFigure();
						pointF = ParsePoint(parts[++i]);
					}
					else if (parts[i] == "C")
					{
						ArrayList arrayList = new ArrayList();
						arrayList.Add(pointF);
						while (++i < parts.Length && parts[i].IndexOf(',') != -1)
						{
							PointF pointF2 = ParsePoint(parts[i]);
							arrayList.Add(pointF2);
						}
						i--;
						graphicsPath.AddBeziers((PointF[])arrayList.ToArray(typeof(PointF)));
						pointF = (PointF)arrayList[arrayList.Count - 1];
					}
					else if (parts[i] == "L")
					{
						ArrayList arrayList2 = new ArrayList();
						arrayList2.Add(pointF);
						while (++i < parts.Length && parts[i].IndexOf(',') != -1)
						{
							PointF pointF3 = ParsePoint(parts[i]);
							arrayList2.Add(pointF3);
						}
						i--;
						graphicsPath.AddLines((PointF[])arrayList2.ToArray(typeof(PointF)));
						pointF = (PointF)arrayList2[arrayList2.Count - 1];
					}
					else if (parts[i] == "Z" || parts[i] == "z")
					{
						graphicsPath.CloseFigure();
					}
					else if (!(parts[i] == ""))
					{
						throw new Exception(SR.ExceptionXamlGeometryNotSupported(parts[i]));
					}
				}
				graphicsPath.Flatten();
				rectangleF = graphicsPath.GetBounds();
			}
			if (includeOrigin)
			{
				rectangleF = RectangleF.Union(rectangleF, RectangleF.Empty);
			}
			return rectangleF;
		}

		private string[] GetStreamGeometryParts(string streamGeometry)
		{
			streamGeometry = streamGeometry.Replace("M", " M ");
			streamGeometry = streamGeometry.Replace("C", " C ");
			streamGeometry = streamGeometry.Replace("L", " L ");
			streamGeometry = streamGeometry.Replace("  ", " ");
			return streamGeometry.Split(' ');
		}

		private void IntepretStreamGeometry(string[] parts, PointF shapeOffset, float stretchFactorX, float stretchFactorY, bool includeOrigin, RectangleF fromBounds, RectangleF toBounds, ref GraphicsPath graphicsPath)
		{
			shapeOffset.X /= stretchFactorX;
			shapeOffset.Y /= stretchFactorY;
			toBounds.Width *= stretchFactorX;
			toBounds.Height *= stretchFactorY;
			PointF location = GetStreamGeometryBounds(parts, includeOrigin).Location;
			location.X -= shapeOffset.X;
			location.Y -= shapeOffset.Y;
			PointF pointF = PointF.Empty;
			int num = 0;
			while (true)
			{
				if (num >= parts.Length)
				{
					return;
				}
				if (parts[num] == "F0" || parts[num] == "f0")
				{
					graphicsPath.FillMode = FillMode.Alternate;
				}
				else if (parts[num] == "F1" || parts[num] == "f1")
				{
					graphicsPath.FillMode = FillMode.Winding;
				}
				else if (parts[num] == "M")
				{
					graphicsPath.StartFigure();
					pointF = OffsetPoint(ParsePoint(parts[++num]), location);
					pointF = TransformPoint(pointF, fromBounds, toBounds);
				}
				else if (parts[num] == "C")
				{
					ArrayList arrayList = new ArrayList();
					arrayList.Add(pointF);
					while (++num < parts.Length && parts[num].IndexOf(',') != -1)
					{
						PointF point = OffsetPoint(ParsePoint(parts[num]), location);
						point = TransformPoint(point, fromBounds, toBounds);
						arrayList.Add(point);
					}
					num--;
					graphicsPath.AddBeziers((PointF[])arrayList.ToArray(typeof(PointF)));
					pointF = (PointF)arrayList[arrayList.Count - 1];
				}
				else if (parts[num] == "L")
				{
					ArrayList arrayList2 = new ArrayList();
					arrayList2.Add(pointF);
					while (++num < parts.Length && parts[num].IndexOf(',') != -1)
					{
						PointF point2 = OffsetPoint(ParsePoint(parts[num]), location);
						point2 = TransformPoint(point2, fromBounds, toBounds);
						arrayList2.Add(point2);
					}
					num--;
					graphicsPath.AddLines((PointF[])arrayList2.ToArray(typeof(PointF)));
					pointF = (PointF)arrayList2[arrayList2.Count - 1];
				}
				else if (parts[num] == "Z" || parts[num] == "z")
				{
					graphicsPath.CloseFigure();
				}
				else if (!(parts[num] == ""))
				{
					break;
				}
				num++;
			}
			throw new Exception(SR.ExceptionXamlGeometryNotSupported(parts[num]));
		}

		private PointF RelativeToAbsolute(PointF relativePoint, RectangleF relativeToRect)
		{
			PointF result = default(PointF);
			result.X = relativeToRect.X + relativeToRect.Width * relativePoint.X;
			result.Y = relativeToRect.Y + relativeToRect.Height * relativePoint.Y;
			return result;
		}

		private PointF AbsoluteToRelative(PointF alsolutePoint, RectangleF relativeToRect)
		{
			PointF result = default(PointF);
			result.X = (alsolutePoint.X - relativeToRect.X) / relativeToRect.Width;
			result.Y = (alsolutePoint.Y - relativeToRect.Y) / relativeToRect.Height;
			return result;
		}

		private XmlNode FindFillNode(XmlNode shapeNode)
		{
			foreach (XmlNode childNode in shapeNode.ChildNodes)
			{
				if (childNode.Name == "Shape.Fill" || childNode.Name == shapeNode.Name + ".Fill")
				{
					return childNode;
				}
			}
			return null;
		}

		private XmlNode FindStrokeNode(XmlNode shapeNode)
		{
			foreach (XmlNode childNode in shapeNode.ChildNodes)
			{
				if (childNode.Name == "Shape.Stroke" || childNode.Name == shapeNode.Name + ".Stroke")
				{
					return childNode;
				}
			}
			return null;
		}

		private XmlNode FindChildNode(XmlNode parent, string childName)
		{
			foreach (XmlNode childNode in parent.ChildNodes)
			{
				if (childName == "*" && childNode.NodeType != XmlNodeType.Comment)
				{
					return childNode;
				}
				if (childNode.Name == childName)
				{
					return childNode;
				}
			}
			return null;
		}

		private int CountChildNodes(XmlNode parentNode)
		{
			int num = 0;
			foreach (XmlNode childNode in parentNode.ChildNodes)
			{
				if (childNode.NodeType != XmlNodeType.Comment)
				{
					num++;
				}
			}
			return num;
		}

		private PointF OffsetPoint(PointF point, PointF offset)
		{
			point.X -= offset.X;
			point.Y -= offset.Y;
			return point;
		}

		private PointF TransformPoint(PointF point, RectangleF fromBounds, RectangleF toBounds)
		{
			point.X -= fromBounds.X;
			point.Y -= fromBounds.Y;
			point.X /= fromBounds.Width;
			point.Y /= fromBounds.Height;
			point.X *= toBounds.Width;
			point.Y *= toBounds.Height;
			point.X += toBounds.X;
			point.Y += toBounds.Y;
			return point;
		}

		private RectangleF TransformRectangle(RectangleF rect, RectangleF fromBounds, RectangleF toBounds)
		{
			rect.Location = TransformPoint(rect.Location, fromBounds, toBounds);
			rect.Width /= fromBounds.Width;
			rect.Height /= fromBounds.Height;
			rect.Width *= toBounds.Width;
			rect.Height *= toBounds.Height;
			return rect;
		}

		private Color TransformColor(Color color, int layerIndex)
		{
			if (LayerHues == null || layerIndex >= LayerHues.Length)
			{
				return color;
			}
			ColorMatrix colorMatrix = new ColorMatrix();
			colorMatrix.Matrix00 = (float)(int)LayerHues[layerIndex].R / 255f;
			colorMatrix.Matrix11 = (float)(int)LayerHues[layerIndex].G / 255f;
			colorMatrix.Matrix22 = (float)(int)LayerHues[layerIndex].B / 255f;
			colorMatrix.Matrix33 = (float)(int)LayerHues[layerIndex].A / 255f;
			float[] array = new float[5]
			{
				(float)(int)color.R / 255f,
				(float)(int)color.G / 255f,
				(float)(int)color.B / 255f,
				(float)(int)color.A / 255f,
				1f
			};
			int val = (int)((array[0] * colorMatrix.Matrix00 + array[1] * colorMatrix.Matrix10 + array[2] * colorMatrix.Matrix20 + array[3] * colorMatrix.Matrix30 + array[4] * colorMatrix.Matrix40) * 255f);
			int val2 = (int)((array[0] * colorMatrix.Matrix01 + array[1] * colorMatrix.Matrix11 + array[2] * colorMatrix.Matrix21 + array[3] * colorMatrix.Matrix31 + array[4] * colorMatrix.Matrix41) * 255f);
			int val3 = (int)((array[0] * colorMatrix.Matrix02 + array[1] * colorMatrix.Matrix12 + array[2] * colorMatrix.Matrix22 + array[3] * colorMatrix.Matrix32 + array[4] * colorMatrix.Matrix42) * 255f);
			int val4 = (int)((array[0] * colorMatrix.Matrix03 + array[1] * colorMatrix.Matrix13 + array[2] * colorMatrix.Matrix23 + array[3] * colorMatrix.Matrix33 + array[4] * colorMatrix.Matrix43) * 255f);
			val = Math.Max(val, 0);
			val = Math.Min(val, 255);
			val2 = Math.Max(val2, 0);
			val2 = Math.Min(val2, 255);
			val3 = Math.Max(val3, 0);
			val3 = Math.Min(val3, 255);
			int alpha = Math.Min(Math.Max(val4, 0), 255);
			HLSColor hLSColor = new HLSColor(val, val2, val3);
			float brightness = LayerHues[layerIndex].GetBrightness();
			return Color.FromArgb(alpha, hLSColor.Lighten(brightness));
		}

		private PointF ParsePoint(string point)
		{
			string[] array = point.Split(',');
			return new PointF(float.Parse(array[0], CultureInfo.InvariantCulture), float.Parse(array[1], CultureInfo.InvariantCulture));
		}

		public void Dispose()
		{
			Dispose(disposing: true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!disposed && disposing)
			{
				Layers = null;
			}
			disposed = true;
		}

		~XamlRenderer()
		{
			Dispose(disposing: false);
		}
	}
}
