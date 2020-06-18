using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Xml;

namespace Microsoft.Reporting.Map.WebForms
{
	internal class SvgRendering : MapParameters
	{
		internal XmlTextWriter output;

		private int gradientIDNum;

		private int clipRegionIdNum;

		private bool selectionMode;

		private string gradientIDString = string.Empty;

		private float[] oldMatrix = new float[6];

		private bool antiAlias;

		internal bool antiAliasText;

		private bool transformOpen;

		private string toolTipsText = string.Empty;

		private string title = string.Empty;

		private bool toolTipsActive;

		private bool clipSet;

		private bool resizable;

		private string toolTipsScript = "\r\n\r\n\t\tfunction ToolTips( document )\r\n\t\t{\r\n\t\t\t// Tool Tip Window\r\n\t\t\twindow.svgToolTip = this;\r\n\r\n\t\t\t// Tool Tip Attributes\r\n\t\t\tToolTips.size = 10;\r\n\t\t\tToolTips.scale = document.getDocumentElement().getCurrentScale();\r\n\t\t\tToolTips.translate = document.getDocumentElement().getCurrentTranslate();\r\n\r\n\t\t\t// Init\r\n\t\t\tthis.Create( document );\r\n\t\t\tAddTitleEvents( document.getDocumentElement() );\r\n\t\t}\r\n\r\n\t\tfunction ToolTips.CreateToolTipRectangle( doc )\r\n\t\t{\r\n\t\t\tvar rectangle;\r\n\r\n\t\t\txCoordinate = -1.0 /4.0 * ToolTips.size;\r\n\t\t\tyCoordinate = -1 * ToolTips.size;\r\n\t\t\twidth = 1.0;\r\n\t\t\theight = 1.25 * ToolTips.size;\r\n\r\n\t\t\trectangle = doc.createElement( ##rect## );\r\n\r\n\t\t\trectangle.setAttribute( ##x##, xCoordinate );\r\n\t\t\trectangle.setAttribute( ##y##, yCoordinate );\r\n\t\t\trectangle.setAttribute( ##width##, width );\r\n\t\t\trectangle.setAttribute( ##height##, height );\r\n\r\n\t\t\trectangle.setAttribute( ##style##, ##stroke:black;fill:#edefc2;## );\r\n\r\n\t\t\treturn rectangle;\r\n\t\t}\r\n\r\n\t\tToolTips.prototype.Create = function( doc )\r\n\t\t{ \r\n\t\t\tthis.rectangle = ToolTips.CreateToolTipRectangle( doc );\r\n\r\n\t\t\tthis.node = doc.createTextNode( #### );\r\n\r\n\t\t\tthis.textElement = doc.createElement( ##text## )\r\n\t\t\tthis.textElement.setAttribute( ##style##, ##font-family:Arial; font-size:## + 10 + ##;fill:black;## );\r\n\t\t\tthis.textElement.appendChild( this.node );\r\n\r\n\t\t\tthis.group = doc.createElement( ##g## ),\r\n\t\t\tthis.group.appendChild( this.rectangle );\r\n\t\t\tthis.group.appendChild( this.textElement );\r\n\r\n\t\t\tdoc.getDocumentElement().appendChild( this.group );\r\n\t\t}\r\n\r\n\t\tToolTips.Start = function Title_Activate(evt)\r\n\t\t{\r\n\t\t\tif (window.svgToolTip.element == null)\r\n\t\t\t{\r\n\t\t\t\tvar  x = (evt.getClientX() - ToolTips.translate.getX())/ToolTips.scale +  0.25*ToolTips.size,\r\n\t\t\t\ty = (evt.getClientY() - ToolTips.translate.getY())/ToolTips.scale - ToolTips.size;\r\n\r\n\t\t\t\tdoc = evt.getTarget().getOwnerDocument();\r\n\t\t\t\tSVGRoot = doc.getDocumentElement();\r\n\t\t\t\tsvgW = parseInt(SVGRoot.getAttribute(##width##));\r\n\t\t\t\tsvgH = parseInt(SVGRoot.getAttribute(##height##));\r\n\t\t\t\t\t\t\t\t\r\n\t\t\t\twindow.svgToolTip.element = evt.getCurrentTarget();\r\n\t\t\t\twindow.svgToolTip.element.removeEventListener(##mouseover##, ToolTips.Start, false);\r\n\t\t\t\twindow.svgToolTip.element.addEventListener(##mouseout##, ToolTips.Stop, false);\r\n\t\t\t\twindow.svgToolTip.node.setNodeValue(TextOf(GetToolTip(window.svgToolTip.element)));\r\n\t\t\t\t\r\n\t\t\t\trectWidth = window.svgToolTip.textElement.getComputedTextLength() + 0.5 * ToolTips.size;\r\n\t\t\t\trectHeight = 1.25 * ToolTips.size;\r\n\r\n\t\t\t\tif( svgW < x + rectWidth )\r\n\t\t\t\t{\r\n\t\t\t\t\tx = svgW - rectWidth;\r\n\t\t\t\t}\r\n\r\n\t\t\t\tif( svgH < y + rectHeight )\r\n\t\t\t\t{\r\n\t\t\t\t\ty = svgH - rectHeight;\r\n\t\t\t\t}\r\n\r\n\t\t\t\tif( y < rectHeight )\r\n\t\t\t\t{\r\n\t\t\t\t\ty = 4 * rectHeight;\r\n\t\t\t\t}\r\n\r\n\t\t\t\tif( x < 0 )\r\n\t\t\t\t{\r\n\t\t\t\t\tx = 0;\r\n\t\t\t\t}\r\n\r\n\t\t\t\twindow.svgToolTip.rectangle.setAttribute(##width##, window.svgToolTip.textElement.getComputedTextLength() + 0.5*ToolTips.size);\r\n\t\t\t\twindow.svgToolTip.group.setAttribute(##transform##, ##translate(## + x + ##,## + y + ##)##);\r\n\t\t\t\twindow.svgToolTip.group.setAttribute(##visibility##, ##visible##);\r\n\t\t\t}\r\n\t\t}\r\n\r\n\t\tToolTips.Stop = function Title_Passivate(evt)\r\n\t\t{\r\n\t\t\tif (window.svgToolTip.element != null)\r\n\t\t\t{\r\n\t\t\t\twindow.svgToolTip.group.setAttribute(##visibility##, ##hidden##);\r\n\t\t\t\twindow.svgToolTip.element.removeEventListener(##mouseout##, ToolTips.Stop, false);\r\n\t\t\t\twindow.svgToolTip.element.addEventListener(##mouseover##, ToolTips.Start, false);\r\n\t\t\t\twindow.svgToolTip.element = null;\r\n\t\t\t}\r\n\t\t}\r\n\r\n\t\tToolTips.Register = function Title_Register(elem)\r\n\t\t{\r\n\t\t\tif (GetToolTip(elem) != null)\r\n\t\t\t\telem.addEventListener(##mouseover##, ToolTips.Start, false);\r\n\t\t}\r\n\t\t\r\n\t\tfunction GetToolTip( svgPrimitives )\r\n\t\t{\r\n\t\t\tvar element = svgPrimitives.getChildNodes();\r\n\r\n\t\t\tfor ( itemIndex = 0; itemIndex < element.getLength(); itemIndex++ )\r\n\t\t\t{\r\n\t\t\t\tif ( element.item( itemIndex ).getNodeType() == 1 && element.item( itemIndex ).getNodeName() == ##title## )\r\n\t\t\t\t{\r\n\t\t\t\t\treturn element.item( itemIndex );\r\n\t\t\t\t}\r\n\t\t\t}\r\n\r\n\t\t\treturn null;\r\n\r\n\t\t}\r\n\r\n\t\tfunction TextOf(elem)\r\n\t\t{\r\n\t\t\tvar childs = elem ? elem.getChildNodes() : null;\r\n\r\n\t\t\tfor (var i=0; childs && i<childs.getLength(); i++)\r\n\t\t\t\tif (childs.item(i).getNodeType() == 3)\r\n\t\t\t\t\treturn childs.item(i).getNodeValue();\r\n   \r\n\t\t\treturn ####;\r\n\t\t}\r\n\r\n\t\tfunction AddTitleEvents(elem)\r\n\t\t{\r\n\t\t\tvar childs = elem.getChildNodes();\r\n\r\n\t\t\tfor (var i=0; i<childs.getLength(); i++)\r\n\t\t\t{\r\n\t\t\t\tif (childs.item(i).getNodeType() == 1)\r\n\t\t\t\t{\r\n\t\t\t\t\tAddTitleEvents(childs.item(i));\r\n\t\t\t\t}\r\n\t\t\t}\r\n\r\n\t\t\tif ( GetToolTip(elem) != null )\r\n\t\t\t{\r\n\t\t\t\telem.addEventListener( ##mouseover##, ToolTips.Start, false );\r\n\t\t\t}\r\n\t\t}\r\n\r\n\t\tfunction LoadHandler( event ) \r\n\t\t{\r\n\t\t\tnew ToolTips( event.getTarget().getOwnerDocument() );\r\n\t\t}\r\n\t\r\n";

		private string emptyLoadHandler = "\r\n\r\n\t\tfunction LoadHandler( event ) \r\n\t\t{\r\n\t\t}\r\n\t\r\n";

		public void Open(XmlTextWriter svgWriter, Size pictureSize)
		{
			Open(svgWriter, pictureSize, new SvgOpenParameters(toolTipsEnabled: false, resizable: false, preserveAspectRatio: false));
		}

		public void SetTitle(string title)
		{
			this.title = title;
		}

		public void Open(XmlTextWriter svgWriter, Size pictureSize, SvgOpenParameters extraParameters)
		{
			output = svgWriter;
			toolTipsActive = extraParameters.toolTipsEnabled;
			if (svgWriter == null)
			{
				throw new ArgumentException("Svg Graphics Object - Invalid Text Writer.", "svgWriter");
			}
			if (pictureSize.IsEmpty || pictureSize.Width <= 0 || pictureSize.Height <= 0)
			{
				throw new ArgumentException("Svg Graphics Object - Invalid SVG Picture Size.", "pictureSize");
			}
			base.PictureSize = pictureSize;
			output.WriteStartDocument();
			output.Formatting = Formatting.Indented;
			output.WriteComment("Map SVG converter");
			output.WriteStartElement("svg");
			if (extraParameters.resizable)
			{
				if (!extraParameters.preserveAspectRatio)
				{
					output.WriteAttributeString("preserveAspectRatio", "none");
				}
				output.WriteAttributeString("viewBox", "0 0 " + base.PictureSize.Width.ToString(CultureInfo.CurrentCulture) + " " + base.PictureSize.Height.ToString(CultureInfo.CurrentCulture));
				output.WriteAttributeString("xml:space", "preserve");
				resizable = true;
				toolTipsActive = false;
			}
			else
			{
				output.WriteAttributeString("width", base.PictureSize.Width.ToString(CultureInfo.CurrentCulture));
				output.WriteAttributeString("height", base.PictureSize.Height.ToString(CultureInfo.CurrentCulture));
			}
			output.WriteAttributeString("onload", "LoadHandler(evt)");
			if (!string.IsNullOrEmpty(title))
			{
				output.WriteStartElement("title");
				output.WriteString(title);
				output.WriteEndElement();
			}
		}

		internal void Validate()
		{
			if (output == null)
			{
				throw new InvalidOperationException("Svg Graphics Object - Svg Output Writer is null.");
			}
			if (base.PictureSize.IsEmpty || base.PictureSize.Width <= 0 || base.PictureSize.Height <= 0)
			{
				throw new ArgumentException("Svg Graphics Object - Invalid SVG Picture Size.", "pictureSize");
			}
		}

		public void Close()
		{
			if (toolTipsActive)
			{
				toolTipsScript = toolTipsScript.Replace("##", "\"");
				output.WriteStartElement("script");
				output.WriteCData(toolTipsScript);
				output.WriteEndElement();
			}
			else
			{
				emptyLoadHandler = emptyLoadHandler.Replace("##", "\"");
				output.WriteStartElement("script");
				output.WriteCData(emptyLoadHandler);
				output.WriteEndElement();
			}
			if (transformOpen)
			{
				output.WriteEndElement();
			}
			output.WriteEndDocument();
			output.Flush();
			output.Close();
		}

		public void DrawLine(PointF point1, PointF point2)
		{
			Validate();
			StartGraphicsParameters(fill: false, outline: true);
			output.WriteStartElement("line");
			output.WriteAttributeString("x1", GetX(point1));
			output.WriteAttributeString("y1", GetY(point1));
			output.WriteAttributeString("x2", GetX(point2));
			output.WriteAttributeString("y2", GetY(point2));
			if ((point1.X == point2.X || point1.Y == point2.Y) && NoTransformMatrix())
			{
				output.WriteAttributeString("shape-rendering", "optimizeSpeed");
			}
			SetToolTip();
			output.WriteEndElement();
			EndGraphicsParameters();
		}

		public void DrawLines(PointF[] points)
		{
			Validate();
			StartGraphicsParameters(fill: false, outline: true);
			output.WriteStartElement("polyline");
			string text = string.Empty;
			foreach (PointF point in points)
			{
				text = text + GetX(point) + "," + GetY(point) + " ";
			}
			output.WriteAttributeString("points", text);
			SetToolTip();
			output.WriteEndElement();
			EndGraphicsParameters();
		}

		public void DrawRectangle(RectangleF rect)
		{
			StartGraphicsParameters(fill: false, outline: true);
			SetRectangle(rect);
			EndGraphicsParameters();
		}

		private void SetRectangle(RectangleF rect)
		{
			Validate();
			output.WriteStartElement("rect");
			output.WriteAttributeString("x", GetX(rect));
			output.WriteAttributeString("y", GetY(rect));
			output.WriteAttributeString("width", GetWidth(rect));
			output.WriteAttributeString("height", GetHeight(rect));
			SetToolTip();
			output.WriteEndElement();
		}

		public void DrawPolygon(PointF[] points)
		{
			StartGraphicsParameters(fill: false, outline: true);
			SetPolygon(points);
			EndGraphicsParameters();
		}

		private void SetPolygon(PointF[] points)
		{
			Validate();
			output.WriteStartElement("polygon");
			string text = string.Empty;
			foreach (PointF point in points)
			{
				text = text + GetX(point) + "," + GetY(point) + " ";
			}
			output.WriteAttributeString("points", text);
			SetToolTip();
			output.WriteEndElement();
		}

		public void DrawArc(RectangleF rect, float startAngle, float sweepAngle)
		{
			Validate();
			StartGraphicsParameters(fill: false, outline: true);
			output.WriteStartElement("path");
			GraphicsPath graphicsPath = new GraphicsPath();
			graphicsPath.AddArc(rect, startAngle, sweepAngle);
			PointF point = graphicsPath.PathPoints[0];
			PointF pointF = graphicsPath.PathPoints[graphicsPath.PathPoints.Length - 1];
			int num = 0;
			if (sweepAngle > 180f)
			{
				num = 1;
			}
			string empty = string.Empty;
			empty = "M" + GetX(point) + "," + GetY(point);
			empty = empty + " a" + GetX(rect.Width / 2f) + "," + GetY(rect.Height / 2f);
			empty = empty + " 0," + num + ",1 ";
			empty = empty + GetX(pointF.X - point.X) + "," + GetY(pointF.Y - point.Y);
			output.WriteAttributeString("d", empty);
			SetToolTip();
			output.WriteEndElement();
			EndGraphicsParameters();
		}

		public void DrawPie(RectangleF rect, float startAngle, float sweepAngle)
		{
			Validate();
			StartGraphicsParameters(fill: false, outline: true);
			SetPie(rect, startAngle, sweepAngle);
			output.WriteEndElement();
		}

		private void SetPie(RectangleF rect, float startAngle, float sweepAngle)
		{
			Validate();
			output.WriteStartElement("path");
			GraphicsPath graphicsPath = new GraphicsPath();
			graphicsPath.AddArc(rect, startAngle, sweepAngle);
			PointF pointF = graphicsPath.PathPoints[0];
			PointF pointF2 = graphicsPath.PathPoints[graphicsPath.PathPoints.Length - 1];
			PointF point = new PointF(rect.X + rect.Width / 2f, rect.Y + rect.Height / 2f);
			PointF point2 = new PointF(pointF.X - point.X, pointF.Y - point.Y);
			PointF point3 = new PointF(point.X - pointF2.X, point.Y - pointF2.Y);
			int num = 0;
			if (sweepAngle > 180f)
			{
				num = 1;
			}
			string empty = string.Empty;
			empty = "M" + GetX(point) + "," + GetY(point);
			empty = empty + " l" + GetX(point2) + "," + GetY(point2);
			empty = empty + " a" + GetX(rect.Width / 2f) + "," + GetY(rect.Height / 2f);
			empty = empty + " 0," + num + ",1 ";
			empty = empty + GetX(pointF2.X - pointF.X) + "," + (pointF2.Y - pointF.Y);
			empty = empty + " l" + GetX(point3) + "," + GetY(point3);
			output.WriteAttributeString("d", empty);
			SetToolTip();
			output.WriteEndElement();
		}

		public void DrawEllipse(RectangleF rect)
		{
			Validate();
			StartGraphicsParameters(fill: false, outline: true);
			SetEllipse(rect);
			EndGraphicsParameters();
		}

		private void SetEllipse(RectangleF rect)
		{
			Validate();
			output.WriteStartElement("ellipse");
			output.WriteAttributeString("cx", GetX(rect.X + rect.Width / 2f));
			output.WriteAttributeString("cy", GetY(rect.Y + rect.Height / 2f));
			output.WriteAttributeString("rx", GetX(rect.Width / 2f));
			output.WriteAttributeString("ry", GetY(rect.Height / 2f));
			SetToolTip();
			output.WriteEndElement();
		}

		public void DrawBezier(PointF pt1, PointF pt2, PointF pt3, PointF pt4)
		{
			Validate();
			StartGraphicsParameters(fill: false, outline: true);
			output.WriteStartElement("path");
			string empty = string.Empty;
			empty = "M" + GetX(pt1) + "," + GetY(pt1);
			empty = empty + " C" + GetX(pt2) + "," + GetY(pt2);
			empty = empty + " " + GetX(pt3) + "," + GetY(pt3);
			empty = empty + " " + GetX(pt4) + "," + GetY(pt4);
			output.WriteAttributeString("d", empty);
			output.WriteEndElement();
			SetToolTip();
			EndGraphicsParameters();
		}

		public void DrawBeziers(PointF[] points)
		{
			int num = points.Length / 4 + 1;
			PointF[] array = new PointF[num * 4];
			for (int i = 0; i < array.Length; i++)
			{
				if (points.Length >= i)
				{
					array[i] = new PointF(0f, 0f);
				}
				else
				{
					array[i] = points[i];
				}
			}
			for (int j = 0; j < num; j++)
			{
				DrawBezier(array[j * 4], array[j * 4 + 1], array[j * 4 + 2], array[j * 4 + 3]);
			}
		}

		public void DrawCurve(PointF[] points, float tension)
		{
			Validate();
			GraphicsPath graphicsPath = new GraphicsPath();
			graphicsPath.AddCurve(points, tension);
			graphicsPath.Flatten();
			DrawLines(graphicsPath.PathPoints);
			graphicsPath.Dispose();
		}

		public void DrawCurve(PointF[] points, int offset, int numberOfSegments, float tension)
		{
			Validate();
			GraphicsPath graphicsPath = new GraphicsPath();
			graphicsPath.AddCurve(points, offset, numberOfSegments, tension);
			graphicsPath.Flatten();
			DrawLines(graphicsPath.PathPoints);
			graphicsPath.Dispose();
		}

		public void DrawPath(GraphicsPath path)
		{
			byte[] pathTypes = path.PathTypes;
			int num = 1;
			byte[] array = pathTypes;
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i] == 0)
				{
					num++;
				}
			}
			if (num == 2)
			{
				Validate();
				path.Flatten();
				DrawPolygon(path.PathPoints);
				return;
			}
			Validate();
			path.Flatten();
			PointF[] array2 = new PointF[path.PathPoints.Length];
			int num2 = 0;
			int num3 = 0;
			PointF[] pathPoints = path.PathPoints;
			for (int i = 0; i < pathPoints.Length; i++)
			{
				PointF pointF = array2[num2] = pathPoints[i];
				if (path.PathTypes[num3] == 129)
				{
					PointF[] array3 = new PointF[num2 + 2];
					for (int j = 0; j <= num2; j++)
					{
						array3[j] = array2[j];
					}
					array3[num2 + 1] = array2[0];
					num2 = 0;
					DrawLines(array3);
					array2 = new PointF[path.PathPoints.Length];
				}
				else
				{
					num2++;
				}
				num3++;
			}
		}

		public void DrawClosedCurve(PointF[] points, float tension)
		{
			Validate();
			GraphicsPath graphicsPath = new GraphicsPath();
			graphicsPath.AddClosedCurve(points, tension);
			graphicsPath.Flatten();
			DrawLines(graphicsPath.PathPoints);
			graphicsPath.Dispose();
		}

		public void DrawImage(Image image, Rectangle destRect, int srcX, int srcY, int srcWidth, int srcHeight, GraphicsUnit srcUnit, ImageAttributes imageAttr)
		{
			Validate();
			Image image2 = new Bitmap(image, destRect.Width, destRect.Height);
			Graphics.FromImage(image2).DrawImage(image, new Rectangle(0, 0, destRect.Width, destRect.Height), srcX, srcY, srcWidth, srcHeight, srcUnit, imageAttr);
			output.WriteStartElement("image");
			string str = ImageToString(image2);
			output.WriteAttributeString("xlink:href", "data:image/jpeg;base64," + str);
			output.WriteAttributeString("x", GetX(destRect));
			output.WriteAttributeString("y", GetY(destRect));
			output.WriteAttributeString("width", GetWidth(destRect));
			output.WriteAttributeString("height", GetHeight(destRect));
			output.WriteEndElement();
			image2.Dispose();
		}

		public void DrawImage(Image image, RectangleF destRect)
		{
			Validate();
			Image image2 = new Bitmap(image, (int)destRect.Width, (int)destRect.Height);
			Graphics.FromImage(image2).DrawImage(image, new RectangleF(0f, 0f, destRect.Width, destRect.Height));
			output.WriteStartElement("image");
			string str = ImageToString(image2);
			output.WriteAttributeString("xlink:href", "data:image/jpeg;base64," + str);
			output.WriteAttributeString("x", GetX(destRect));
			output.WriteAttributeString("y", GetY(destRect));
			output.WriteAttributeString("width", GetWidth(destRect));
			output.WriteAttributeString("height", GetHeight(destRect));
			output.WriteEndElement();
			image2.Dispose();
		}

		public void DrawImage(Image image, Rectangle destRect, float srcX, float srcY, float srcWidth, float srcHeight, GraphicsUnit srcUnit, ImageAttributes imageAttrs)
		{
			Validate();
			Image image2 = new Bitmap(image, destRect.Width, destRect.Height);
			Graphics.FromImage(image2).DrawImage(image, new Rectangle(0, 0, destRect.Width, destRect.Height), srcX, srcY, srcWidth, srcHeight, srcUnit, imageAttrs);
			output.WriteStartElement("image");
			string str = ImageToString(image2);
			output.WriteAttributeString("xlink:href", "data:image/jpeg;base64," + str);
			output.WriteAttributeString("x", GetX(destRect));
			output.WriteAttributeString("y", GetY(destRect));
			output.WriteAttributeString("width", GetWidth(destRect));
			output.WriteAttributeString("height", GetHeight(destRect));
			output.WriteEndElement();
			image2.Dispose();
		}

		public void FillTexturedRectangle(TextureBrush textureBrush, RectangleF destRect)
		{
			Validate();
			Image image = new Bitmap(textureBrush.Image, (int)destRect.Width, (int)destRect.Height);
			Graphics.FromImage(image).FillRectangle(textureBrush, destRect);
			output.WriteStartElement("image");
			string str = ImageToString(image);
			output.WriteAttributeString("xlink:href", "data:image/jpeg;base64," + str);
			output.WriteAttributeString("x", GetX(destRect));
			output.WriteAttributeString("y", GetY(destRect));
			output.WriteAttributeString("width", GetWidth(destRect));
			output.WriteAttributeString("height", GetHeight(destRect));
			output.WriteEndElement();
			image.Dispose();
		}

		public void DrawString(string text, RectangleF layoutRect)
		{
			Transformations();
			string[] stringArray;
			try
			{
				WrapString(text, layoutRect, out stringArray);
			}
			catch
			{
				throw new InvalidOperationException("Svg Rendering - Wrap String Error.");
			}
			int num = 0;
			string[] array = stringArray;
			foreach (string text2 in array)
			{
				if (text2 == null)
				{
					num++;
					continue;
				}
				float number;
				float y;
				if (base.StringFormat.Alignment == StringAlignment.Near)
				{
					number = layoutRect.X;
					y = layoutRect.Y;
				}
				else if (base.StringFormat.Alignment == StringAlignment.Far)
				{
					number = layoutRect.X + layoutRect.Width;
					y = layoutRect.Y;
				}
				else
				{
					number = layoutRect.X + layoutRect.Width / 2f;
					y = layoutRect.Y;
				}
				int num2 = (int)Math.Round((double)Font.SizeInPoints * 1.3333333333333333);
				float num3 = (float)num * (float)num2;
				num3 += 1f;
				float num4 = 0f;
				if (base.StringFormat.LineAlignment != StringAlignment.Center)
				{
					num4 = ((base.StringFormat.LineAlignment != 0) ? (num4 + layoutRect.Height) : (num4 + (float)num2));
				}
				else
				{
					num4 += (float)((num + 1) * num2 / 2);
					num4 += layoutRect.Height / 2f;
				}
				output.WriteStartElement("text");
				output.WriteAttributeString("x", ToUSString(number));
				output.WriteAttributeString("y", ToUSString(y + num4 + num3));
				SetStringAlignment(base.StringFormat);
				output.WriteAttributeString("font-family", Font.FontFamily.Name);
				output.WriteAttributeString("font-size", ToUSString(Font.SizeInPoints) + "pt");
				output.WriteAttributeString("fill-opacity", GetAlpha(TextColor));
				if (Font.Italic)
				{
					output.WriteAttributeString("font-style", "italic");
				}
				if (Font.Bold)
				{
					output.WriteAttributeString("font-weight", "bold");
				}
				if (Font.Underline)
				{
					output.WriteAttributeString("text-decoration", "underline");
				}
				else if (Font.Strikeout)
				{
					output.WriteAttributeString("text-decoration", "line-through");
				}
				output.WriteAttributeString("fill", ColorToString(TextColor));
				output.WriteAttributeString("stroke", "none");
				if (!antiAliasText)
				{
					output.WriteAttributeString("text-rendering", "optimizeSpeed");
				}
				output.WriteString(text2);
				SetToolTip();
				output.WriteEndElement();
				num++;
			}
		}

		public void DrawString(string text, PointF point)
		{
			Transformations();
			output.WriteStartElement("text");
			output.WriteAttributeString("x", ToUSString(point.X));
			output.WriteAttributeString("y", ToUSString(point.Y));
			SetStringAlignment(base.StringFormat);
			output.WriteAttributeString("font-family", Font.FontFamily.Name);
			output.WriteAttributeString("font-size", ToUSString(Font.SizeInPoints) + "pt");
			if (Font.Italic)
			{
				output.WriteAttributeString("font-style", "italic");
			}
			if (Font.Bold)
			{
				output.WriteAttributeString("font-weight", "bold");
			}
			if (Font.Underline)
			{
				output.WriteAttributeString("text-decoration", "underline");
			}
			else if (Font.Strikeout)
			{
				output.WriteAttributeString("text-decoration", "line-through");
			}
			output.WriteAttributeString("fill-opacity", GetAlpha(TextColor));
			output.WriteAttributeString("fill", ColorToString(TextColor));
			output.WriteAttributeString("stroke", "none");
			if (!antiAliasText)
			{
				output.WriteAttributeString("text-rendering", "optimizeSpeed");
			}
			output.WriteString(text);
			SetToolTip();
			output.WriteEndElement();
		}

		public void FillRectangle(RectangleF rect)
		{
			Validate();
			StartGraphicsParameters(fill: true, outline: false);
			SetRectangle(rect);
			EndGraphicsParameters();
		}

		public void FillPolygon(PointF[] points)
		{
			Validate();
			StartGraphicsParameters(fill: true, outline: false);
			SetPolygon(points);
			EndGraphicsParameters();
		}

		public void FillPie(RectangleF rect, float startAngle, float sweepAngle)
		{
			Validate();
			StartGraphicsParameters(fill: true, outline: false);
			SetPie(rect, startAngle, sweepAngle);
			EndGraphicsParameters();
		}

		public void FillEllipse(RectangleF rect)
		{
			Validate();
			StartGraphicsParameters(fill: true, outline: false);
			SetEllipse(rect);
			EndGraphicsParameters();
		}

		public void FillClosedCurve(PointF[] points, float tension)
		{
			Validate();
			GraphicsPath graphicsPath = new GraphicsPath();
			graphicsPath.AddClosedCurve(points, tension);
			graphicsPath.Flatten();
			FillPolygon(graphicsPath.PathPoints);
			graphicsPath.Dispose();
		}

		public void FillPath(GraphicsPath path)
		{
			Validate();
			path.Flatten();
			FillPolygon(path.PathPoints);
		}

		public void FillBezier(PointF pt1, PointF pt2, PointF pt3, PointF pt4)
		{
			Validate();
			StartGraphicsParameters(fill: true, outline: false);
			output.WriteStartElement("path");
			string empty = string.Empty;
			empty = "M" + GetX(pt1) + "," + GetY(pt1);
			empty = empty + " C" + GetX(pt2) + "," + GetY(pt2);
			empty = empty + " " + GetX(pt3) + "," + GetY(pt3);
			empty = empty + " " + GetX(pt4) + "," + GetY(pt4);
			if (!pt1.Equals(pt2))
			{
				empty = empty + "  L" + GetX(pt4) + "," + GetY(pt4);
			}
			output.WriteAttributeString("d", empty);
			SetToolTip();
			output.WriteEndElement();
			EndGraphicsParameters();
		}

		public void FillCurve(PointF[] points, float tension)
		{
			Validate();
			GraphicsPath graphicsPath = new GraphicsPath();
			graphicsPath.AddCurve(points, tension);
			graphicsPath.Flatten();
			FillPolygon(graphicsPath.PathPoints);
			graphicsPath.Dispose();
		}

		public void FillArc(RectangleF rect, float startAngle, float sweepAngle)
		{
			Validate();
			StartGraphicsParameters(fill: true, outline: false);
			output.WriteStartElement("path");
			GraphicsPath graphicsPath = new GraphicsPath();
			graphicsPath.AddArc(rect, startAngle, sweepAngle);
			PointF point = graphicsPath.PathPoints[0];
			PointF pointF = graphicsPath.PathPoints[graphicsPath.PathPoints.Length - 1];
			int num = 0;
			if (sweepAngle > 180f)
			{
				num = 1;
			}
			string empty = string.Empty;
			empty = "M" + GetX(point) + "," + GetY(point);
			empty = empty + " a" + GetX(rect.Width / 2f) + "," + GetY(rect.Height / 2f);
			empty = empty + " 0," + num + ",1 ";
			empty = empty + GetX(pointF.X - point.X) + "," + GetY(pointF.Y - point.Y);
			if (!point.Equals(pointF))
			{
				empty = empty + "  L" + GetX(point) + "," + GetY(point);
			}
			output.WriteAttributeString("d", empty);
			SetToolTip();
			output.WriteEndElement();
			EndGraphicsParameters();
		}

		private bool MatrixChanged()
		{
			if (Transform == null)
			{
				return false;
			}
			for (int i = 0; i < 6; i++)
			{
				if (oldMatrix[i] != Transform.Elements[i])
				{
					return true;
				}
			}
			return false;
		}

		internal bool NoTransformMatrix()
		{
			if (Transform == null)
			{
				return true;
			}
			for (int i = 0; i < 6; i++)
			{
				if (i == 0 || i == 3)
				{
					if (Transform.Elements[i] != 1f)
					{
						return false;
					}
				}
				else if (Transform.Elements[i] != 0f)
				{
					return false;
				}
			}
			return true;
		}

		private void ResetTransformMatrix()
		{
			if (oldMatrix == null)
			{
				return;
			}
			for (int i = 0; i < 6; i++)
			{
				if (i == 0 || i == 3)
				{
					oldMatrix[i] = 1f;
				}
				else
				{
					oldMatrix[i] = 0f;
				}
			}
		}

		public float WrapString(string text, RectangleF destRectangle, out string[] stringArray)
		{
			Graphics graphics = Graphics.FromImage(new Bitmap(base.PictureSize.Width, base.PictureSize.Height));
			SizeF sizeF = graphics.MeasureString(text, Font, base.PictureSize.Width, StringFormat);
			destRectangle.Height = base.PictureSize.Height;
			int num = (int)(destRectangle.Height / sizeF.Height);
			if (num == 0)
			{
				num = 1;
			}
			stringArray = new string[num];
			string[] array = SplitText(text);
			int num2 = 0;
			string empty = string.Empty;
			string[] array2 = array;
			foreach (string text2 in array2)
			{
				if (text2.Length == 0)
				{
					continue;
				}
				if (text2[0] == '\n')
				{
					num2++;
					empty = string.Empty;
				}
				if (num2 >= stringArray.Length)
				{
					break;
				}
				empty = stringArray[num2] + text2 + " ";
				sizeF = graphics.MeasureString(empty, Font, base.PictureSize.Width * 10, StringFormat);
				if (sizeF.Width < destRectangle.Width)
				{
					ref string reference = ref stringArray[num2];
					reference = reference + text2 + " ";
					continue;
				}
				num2++;
				if (num2 >= stringArray.Length)
				{
					break;
				}
				empty = string.Empty;
				ref string reference2 = ref stringArray[num2];
				reference2 = reference2 + text2 + " ";
			}
			return sizeF.Height;
		}

		private string[] SplitText(string text)
		{
			text = text.Replace("\n", " \n");
			return text.Split(' ');
		}

		protected string ImageToString(Image image)
		{
			MemoryStream memoryStream = new MemoryStream();
			image.Save(memoryStream, ImageFormat.Jpeg);
			memoryStream.Seek(0L, SeekOrigin.Begin);
			byte[] inArray = memoryStream.ToArray();
			memoryStream.Close();
			return Convert.ToBase64String(inArray);
		}

		private string GetDashStyle(SvgDashStyle dashStyle)
		{
			switch (dashStyle)
			{
			case SvgDashStyle.Dash:
				return "8,3";
			case SvgDashStyle.DashDashDot:
				return "8,3,8,3,2,3";
			case SvgDashStyle.DashDot:
				return "8,3,2,3";
			case SvgDashStyle.DashDotDot:
				return "8,3,2,3,2,3";
			case SvgDashStyle.Dot:
				return "2,3";
			case SvgDashStyle.Doubledash:
				return "16,3";
			case SvgDashStyle.DoubledashDoubledashHalfdash:
				return "16,3,16,3,4,3";
			case SvgDashStyle.DoubledashHalfdash:
				return "16,3,4,3";
			case SvgDashStyle.DoubledashHalfdashHalfdash:
				return "16,3,4,3,4,3";
			case SvgDashStyle.Halfdash:
				return "4,3";
			case SvgDashStyle.HalfdashDot:
				return "4,3,2,3";
			case SvgDashStyle.HalfdashDotDot:
				return "4,3,2,3,2,3";
			case SvgDashStyle.HalfdashHalfdashDot:
				return "4,3,4,3,2,3";
			case SvgDashStyle.Solid:
				return "";
			default:
				return "4,5";
			}
		}

		internal string GetAlpha(Color color)
		{
			double number = (double)(int)color.A / 255.0;
			return ToUSString(number);
		}

		internal string ColorToString(Color color)
		{
			string str = color.R.ToString("x2", CultureInfo.InvariantCulture);
			string str2 = color.G.ToString("x2", CultureInfo.InvariantCulture);
			string str3 = color.B.ToString("x2", CultureInfo.InvariantCulture);
			return "#" + str + str2 + str3;
		}

		internal void SetStringAlignment(StringFormat stringFormat)
		{
			bool flag = false;
			if ((stringFormat.FormatFlags & StringFormatFlags.DirectionVertical) == StringFormatFlags.DirectionVertical)
			{
				flag = true;
			}
			if (flag)
			{
				output.WriteAttributeString("writing-mode", "tb");
			}
			if (stringFormat.Alignment == StringAlignment.Center)
			{
				output.WriteAttributeString("text-anchor", "middle");
			}
			else if (stringFormat.Alignment == StringAlignment.Near)
			{
				output.WriteAttributeString("text-anchor", "start");
			}
			else
			{
				output.WriteAttributeString("text-anchor", "end");
			}
		}

		internal void Transformations()
		{
			if (MatrixChanged())
			{
				if (transformOpen)
				{
					output.WriteEndElement();
				}
				string matrix = GetMatrix(Transform, setOldMatrix: true);
				output.WriteStartElement("g");
				transformOpen = true;
				output.WriteAttributeString("transform", "matrix(" + matrix + ")");
			}
		}

		public void SetSmoothingMode(bool antiAlias, bool shape)
		{
			if (shape)
			{
				this.antiAlias = antiAlias;
			}
			else
			{
				antiAliasText = antiAlias;
			}
		}

		private string GetMatrix(Matrix matrix, bool setOldMatrix)
		{
			string text = string.Empty;
			for (int i = 0; i < 6; i++)
			{
				if (setOldMatrix)
				{
					oldMatrix[i] = Transform.Elements[i];
				}
				text = text + ToUSString(matrix.Elements[i]) + " ";
			}
			return text;
		}

		private void SVGDefine(bool fill, bool outline)
		{
			if (fill && FillType == SvgFillType.Gradient)
			{
				gradientIDString = SetGradient(BrushColor, BrushSecondColor, GradientType);
			}
		}

		public void BeginSvgSelection(string hRef, string title)
		{
			if (!string.IsNullOrEmpty(hRef))
			{
				if (transformOpen)
				{
					output.WriteEndElement();
					ResetTransformMatrix();
					transformOpen = false;
				}
				if (!hRef.ToUpperInvariant().StartsWith("HTTP", StringComparison.Ordinal) && !hRef.ToUpperInvariant().StartsWith("JAVASCRIPT", StringComparison.Ordinal))
				{
					hRef = "http://" + hRef;
				}
				output.WriteStartElement("a");
				output.WriteAttributeString("xlink:href", hRef);
				output.WriteAttributeString("xlink:show", "new");
				output.WriteAttributeString("xlink:title", title);
				selectionMode = true;
			}
			if (!string.IsNullOrEmpty(title) && !resizable)
			{
				toolTipsText = title;
				toolTipsActive = true;
			}
		}

		public void EndSvgSelection()
		{
			if (selectionMode)
			{
				output.WriteEndElement();
				selectionMode = false;
			}
			toolTipsText = string.Empty;
		}

		internal void SetToolTip()
		{
			if (!string.IsNullOrEmpty(toolTipsText))
			{
				output.WriteStartElement("title");
				output.WriteString(toolTipsText);
				output.WriteEndElement();
			}
		}

		internal void StartGraphicsParameters(bool fill, bool outline)
		{
			SVGDefine(fill, outline);
			Transformations();
			output.WriteStartElement("g");
			if (!antiAlias)
			{
				output.WriteAttributeString("shape-rendering", "optimizeSpeed");
			}
			if (clipSet)
			{
				output.WriteAttributeString("clip-path", "url(#MapClipID" + clipRegionIdNum + ")");
			}
			if (fill)
			{
				if (FillMode == FillMode.Winding)
				{
					output.WriteAttributeString("fill-rule", "evenodd");
				}
				if (FillType == SvgFillType.Gradient)
				{
					output.WriteAttributeString("fill", "url(#" + gradientIDString + ")");
				}
				else if (FillType == SvgFillType.Solid)
				{
					output.WriteAttributeString("fill-opacity", GetAlpha(BrushColor));
					output.WriteAttributeString("fill", ColorToString(BrushColor));
				}
			}
			else
			{
				output.WriteAttributeString("fill", "none");
			}
			if (outline)
			{
				output.WriteAttributeString("stroke-opacity", GetAlpha(PenColor));
				output.WriteAttributeString("stroke", ColorToString(PenColor));
				output.WriteAttributeString("stroke-width", ToUSString(PenWidth));
				if (!string.IsNullOrEmpty(GetDashStyle(DashStyle)))
				{
					output.WriteAttributeString("stroke-dasharray", GetDashStyle(DashStyle));
				}
				if (base.SvgLineCap == SvgLineCapStyle.Square)
				{
					output.WriteAttributeString("stroke-linecap", "square");
				}
				else if (base.SvgLineCap == SvgLineCapStyle.Round)
				{
					output.WriteAttributeString("stroke-linecap", "round");
				}
			}
			else
			{
				output.WriteAttributeString("stroke", "none");
			}
		}

		internal void EndGraphicsParameters()
		{
			output.WriteEndElement();
		}

		private string SetGradient(Color firstColor, Color secondColor, SvgGradientType type)
		{
			if (gradientIDNum == int.MaxValue)
			{
				throw new InvalidOperationException(" SVG Graphics object - The maximum number for gradients is:" + int.MaxValue);
			}
			switch (type)
			{
			case SvgGradientType.None:
				return "";
			case SvgGradientType.Center:
			{
				gradientIDNum++;
				string text2 = "GradientIDNumber" + gradientIDNum;
				SetRadialGradient(text2, firstColor, secondColor);
				return text2;
			}
			default:
			{
				gradientIDNum++;
				string text = "GradientIDNumber" + gradientIDNum;
				SetLinearGradient(text, firstColor, secondColor, type);
				return text;
			}
			}
		}

		private void SetRadialGradient(string gradientID, Color firstColor, Color secondColor)
		{
			output.WriteStartElement("defs");
			output.WriteStartElement("radialGradient");
			output.WriteAttributeString("id", gradientID);
			output.WriteAttributeString("gradientUnits", "objectBoundingBox");
			output.WriteStartElement("stop");
			output.WriteAttributeString("offset", "0%");
			output.WriteAttributeString("stop-color", ColorToString(firstColor));
			output.WriteEndElement();
			output.WriteStartElement("stop");
			output.WriteAttributeString("offset", "100%");
			output.WriteAttributeString("stop-color", ColorToString(secondColor));
			output.WriteEndElement();
			output.WriteEndElement();
			output.WriteEndElement();
		}

		private void SetLinearGradient(string gradientID, Color firstColor, Color secondColor, SvgGradientType type)
		{
			output.WriteStartElement("defs");
			output.WriteStartElement("linearGradient");
			output.WriteAttributeString("id", gradientID);
			output.WriteAttributeString("gradientUnits", "objectBoundingBox");
			output.WriteAttributeString("spreadMethod", "reflect");
			switch (type)
			{
			case SvgGradientType.LeftRight:
				output.WriteAttributeString("x1", "0%");
				output.WriteAttributeString("y1", "0%");
				output.WriteAttributeString("x2", "100%");
				output.WriteAttributeString("y2", "0%");
				break;
			case SvgGradientType.DiagonalLeft:
				output.WriteAttributeString("x1", "0%");
				output.WriteAttributeString("y1", "0%");
				output.WriteAttributeString("x2", "100%");
				output.WriteAttributeString("y2", "100%");
				break;
			case SvgGradientType.DiagonalRight:
				output.WriteAttributeString("x1", "100%");
				output.WriteAttributeString("y1", "0%");
				output.WriteAttributeString("x2", "0%");
				output.WriteAttributeString("y2", "100%");
				break;
			case SvgGradientType.TopBottom:
				output.WriteAttributeString("x1", "0%");
				output.WriteAttributeString("y1", "0%");
				output.WriteAttributeString("x2", "0%");
				output.WriteAttributeString("y2", "100%");
				break;
			case SvgGradientType.HorizontalCenter:
				output.WriteAttributeString("x1", "0%");
				output.WriteAttributeString("y1", "100%");
				output.WriteAttributeString("x2", "0%");
				output.WriteAttributeString("y2", "50%");
				break;
			case SvgGradientType.VerticalCenter:
				output.WriteAttributeString("x1", "100%");
				output.WriteAttributeString("y1", "0%");
				output.WriteAttributeString("x2", "50%");
				output.WriteAttributeString("y2", "0%");
				break;
			default:
				throw new InvalidOperationException("SVG Graphics object - Gradient type is not defined.");
			}
			output.WriteStartElement("stop");
			output.WriteAttributeString("offset", "0%");
			output.WriteAttributeString("stop-color", ColorToString(firstColor));
			output.WriteEndElement();
			output.WriteStartElement("stop");
			output.WriteAttributeString("offset", "100%");
			output.WriteAttributeString("stop-color", ColorToString(secondColor));
			output.WriteEndElement();
			output.WriteEndElement();
			output.WriteEndElement();
		}

		private void SetLinearGradient(string gradientID, LinearGradientBrush brush)
		{
			output.WriteStartElement("defs");
			output.WriteStartElement("linearGradient");
			output.WriteAttributeString("id", gradientID);
			output.WriteAttributeString("gradientUnits", "userSpaceOnUse");
			output.WriteAttributeString("spreadMethod", "reflect");
			output.WriteAttributeString("x1", GetX(brush.Rectangle.X));
			output.WriteAttributeString("y1", GetY(brush.Rectangle.Y));
			output.WriteAttributeString("x2", GetX(brush.Rectangle.X + brush.Rectangle.Width));
			output.WriteAttributeString("y2", GetY(brush.Rectangle.Y + brush.Rectangle.Height));
			output.WriteAttributeString("gradientTransform", "rotate(90)");
			output.WriteStartElement("stop");
			output.WriteAttributeString("offset", "0%");
			output.WriteAttributeString("stop-color", ColorToString(brush.LinearColors[0]));
			output.WriteEndElement();
			output.WriteStartElement("stop");
			output.WriteAttributeString("offset", "100%");
			output.WriteAttributeString("stop-color", ColorToString(brush.LinearColors[1]));
			output.WriteEndElement();
			output.WriteEndElement();
			output.WriteEndElement();
		}

		public void SetClip(RectangleF rect)
		{
			clipRegionIdNum++;
			clipSet = true;
			output.WriteStartElement("clipPath");
			output.WriteAttributeString("id", "MapClipID" + clipRegionIdNum);
			output.WriteAttributeString("clipPathUnits", "userSpaceOnUse");
			output.WriteStartElement("rect");
			output.WriteAttributeString("x", GetX(rect));
			output.WriteAttributeString("y", GetY(rect));
			output.WriteAttributeString("width", GetWidth(rect));
			output.WriteAttributeString("height", GetHeight(rect));
			output.WriteEndElement();
			output.WriteEndElement();
		}

		public void ResetClip()
		{
			clipSet = false;
		}
	}
}
