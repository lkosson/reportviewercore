using Microsoft.ReportingServices.Rendering.RPLProcessing;
using System.Drawing;

namespace Microsoft.Reporting.NETCore
{
	internal class Action
	{
		internal string Id;

		internal float DpiX;

		internal float DpiY;

		internal string Label;

		internal Rectangle Position = Rectangle.Empty;

		internal Point[] Points;

		internal ActionShape Shape;

		internal ActionType Type;

		internal int XOffset;

		internal int YOffset;

		private RectangleF m_position;

		private float[] m_path;

		public string LocalizedTypeName
		{
			get
			{
				switch (Type)
				{
				case ActionType.BookmarkLink:
					return ReportPreviewStrings.ReportActionAccessibleNameBookmark;
				case ActionType.DocumentMap:
					return ReportPreviewStrings.ReportActionAccessibleNameDocumentMap;
				case ActionType.DrillThrough:
					return ReportPreviewStrings.ReportActionAccessibleNameDrillThrough;
				case ActionType.HyperLink:
					return ReportPreviewStrings.ReportActionAccessibleNameHyperLink;
				case ActionType.Sort:
					return ReportPreviewStrings.ReportActionAccessibleNameSort;
				case ActionType.Toggle:
					return ReportPreviewStrings.ReportActionAccessibleNameToggle;
				default:
					return Type.ToString();
				}
			}
		}

		public bool IsChildAction
		{
			get
			{
				if (Type != ActionType.Toggle)
				{
					return Type == ActionType.Sort;
				}
				return true;
			}
		}

		internal Action(string id, string label, ActionType type, RPLFormat.ShapeType shape, RectangleF position, float[] path)
		{
			Id = id;
			Label = label;
			Shape = ConvertRPLShape(shape);
			Type = type;
			m_position = position;
			m_path = path;
		}

		internal void SetDpi(float dpiX, float dpiY)
		{
			if (Position != Rectangle.Empty)
			{
				return;
			}
			DpiX = dpiX;
			DpiY = dpiY;
			Position.X = Global.ToPixels(m_position.X, dpiX);
			Position.Y = Global.ToPixels(m_position.Y, dpiY);
			Position.Width = Global.ToPixels(m_position.Width, dpiX);
			Position.Height = Global.ToPixels(m_position.Height, dpiY);
			if (Shape == ActionShape.Rectangle)
			{
				if (m_path != null)
				{
					Position.X += (int)((float)Position.Width * (m_path[0] / 100f));
					Position.Y += (int)((float)Position.Height * (m_path[1] / 100f));
					Position.Width = (int)((float)Position.Width * ((m_path[2] - m_path[0]) / 100f));
					Position.Height = (int)((float)Position.Height * ((m_path[3] - m_path[1]) / 100f));
				}
			}
			else if (Shape == ActionShape.Circle)
			{
				Position.X += (int)((float)Position.Width * ((m_path[0] - m_path[2]) / 100f));
				Position.Y += (int)((float)Position.Height * ((m_path[1] - m_path[2]) / 100f));
				int num3 = Position.Height = (Position.Width = (int)((float)Position.Width * (m_path[2] * 2f / 100f)));
			}
			else if (Shape == ActionShape.Polygon)
			{
				Points = new Point[m_path.Length / 2];
				int num4 = 0;
				int num5 = 0;
				while (num4 < Points.Length)
				{
					Points[num4].X = Global.ToPixels(m_position.Width * (m_path[num5] / 100f), dpiX);
					Points[num4].Y = Global.ToPixels(m_position.Height * (m_path[num5 + 1] / 100f), dpiY);
					num4++;
					num5 += 2;
				}
			}
			m_position = RectangleF.Empty;
			m_path = null;
		}

		private ActionShape ConvertRPLShape(RPLFormat.ShapeType shape)
		{
			switch (shape)
			{
			case RPLFormat.ShapeType.Rectangle:
				return ActionShape.Rectangle;
			case RPLFormat.ShapeType.Circle:
				return ActionShape.Circle;
			case RPLFormat.ShapeType.Polygon:
				return ActionShape.Polygon;
			default:
				return ActionShape.None;
			}
		}
	}
}
