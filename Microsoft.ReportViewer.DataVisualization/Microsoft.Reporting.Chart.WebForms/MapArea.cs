using Microsoft.Reporting.Chart.WebForms.Design;
using Microsoft.Reporting.Chart.WebForms.Utilities;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Text;

namespace Microsoft.Reporting.Chart.WebForms
{
	[DefaultProperty("ToolTip")]
	[SRDescription("DescriptionAttributeMapArea_MapArea")]
	internal class MapArea : IMapAreaAttributes
	{
		private string toolTip = "";

		private string href = "";

		private string attributes = "";

		private string name = "Map Area";

		private bool custom = true;

		private MapAreaShape shape;

		private float[] coordinates = new float[4];

		private object mapAreaTag;

		[Browsable(false)]
		[SRDescription("DescriptionAttributeMapArea_Custom")]
		[DefaultValue("")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[SerializationVisibility(SerializationVisibility.Hidden)]
		public bool Custom
		{
			get
			{
				return custom;
			}
			set
			{
				custom = value;
			}
		}

		[SRCategory("CategoryAttributeShape")]
		[Bindable(true)]
		[SRDescription("DescriptionAttributeMapArea_Coordinates")]
		[DefaultValue("")]
		[TypeConverter(typeof(MapAreaCoordinatesConverter))]
		public float[] Coordinates
		{
			get
			{
				return coordinates;
			}
			set
			{
				coordinates = value;
			}
		}

		[SRCategory("CategoryAttributeShape")]
		[Bindable(true)]
		[SRDescription("DescriptionAttributeMapArea_Shape")]
		[DefaultValue(typeof(MapAreaShape), "Rectangle")]
		public MapAreaShape Shape
		{
			get
			{
				return shape;
			}
			set
			{
				shape = value;
			}
		}

		[SRCategory("CategoryAttributeData")]
		[SRDescription("DescriptionAttributeMapArea_Name")]
		[DefaultValue("Map Area")]
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[SerializationVisibility(SerializationVisibility.Hidden)]
		public string Name
		{
			get
			{
				return name;
			}
			set
			{
				name = value;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		[Browsable(false)]
		[SRCategory("CategoryAttributeMapArea")]
		[Bindable(true)]
		[SRDescription("DescriptionAttributeMapArea_ToolTip")]
		[DefaultValue("")]
		public string ToolTip
		{
			get
			{
				return toolTip;
			}
			set
			{
				toolTip = value;
			}
		}

		[SRCategory("CategoryAttributeMapArea")]
		[Bindable(true)]
		[SRDescription("DescriptionAttributeMapArea_Href")]
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
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		[Browsable(false)]
		[SRCategory("CategoryAttributeMapArea")]
		[Bindable(true)]
		[SRDescription("DescriptionAttributeMapArea_MapAreaAttributes")]
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

		internal string GetTag(ChartGraphics graph)
		{
			StringBuilder stringBuilder = new StringBuilder("\r\n<area shape=\"", 120);
			if (shape == MapAreaShape.Circle)
			{
				stringBuilder.Append("circle\"");
			}
			else if (shape == MapAreaShape.Rectangle)
			{
				stringBuilder.Append("rect\"");
			}
			else if (shape == MapAreaShape.Polygon)
			{
				stringBuilder.Append("poly\"");
			}
			if (Href.Length > 0)
			{
				stringBuilder.Append(" href=\"");
				if (Href.StartsWith("www.", StringComparison.OrdinalIgnoreCase))
				{
					stringBuilder.Append("http://");
				}
				stringBuilder.Append(Href);
				stringBuilder.Append("\"");
			}
			if (ToolTip.Length > 0)
			{
				stringBuilder.Append(" title=\"");
				stringBuilder.Append(ToolTip);
				stringBuilder.Append("\"");
			}
			stringBuilder.Append(" coords=\"");
			float[] array = new float[Coordinates.Length];
			if (Shape == MapAreaShape.Circle)
			{
				PointF absolutePoint = graph.GetAbsolutePoint(new PointF(Coordinates[0], Coordinates[1]));
				array[0] = absolutePoint.X;
				array[1] = absolutePoint.Y;
				array[2] = graph.GetAbsolutePoint(new PointF(Coordinates[2], Coordinates[1])).X;
			}
			else if (Shape == MapAreaShape.Rectangle)
			{
				PointF absolutePoint2 = graph.GetAbsolutePoint(new PointF(Coordinates[0], Coordinates[1]));
				array[0] = absolutePoint2.X;
				array[1] = absolutePoint2.Y;
				absolutePoint2 = graph.GetAbsolutePoint(new PointF(Coordinates[2], Coordinates[3]));
				array[2] = absolutePoint2.X;
				array[3] = absolutePoint2.Y;
				if ((int)Math.Round(array[0]) == (int)Math.Round(array[2]))
				{
					array[2] = (float)Math.Round(array[2]) + 1f;
				}
				if ((int)Math.Round(array[1]) == (int)Math.Round(array[3]))
				{
					array[3] = (float)Math.Round(array[3]) + 1f;
				}
			}
			else
			{
				PointF pointF = Point.Empty;
				PointF relative = Point.Empty;
				for (int i = 0; i < Coordinates.Length - 1; i += 2)
				{
					relative.X = Coordinates[i];
					relative.Y = Coordinates[i + 1];
					pointF = graph.GetAbsolutePoint(relative);
					array[i] = pointF.X;
					array[i + 1] = pointF.Y;
				}
			}
			bool flag = true;
			float[] array2 = array;
			foreach (float num in array2)
			{
				if (!flag)
				{
					stringBuilder.Append(",");
				}
				flag = false;
				stringBuilder.Append((int)Math.Round(num));
			}
			stringBuilder.Append("\"");
			if (MapAreaAttributes.Length > 0)
			{
				stringBuilder.Append(" ");
				stringBuilder.Append(MapAreaAttributes);
			}
			if (stringBuilder.ToString().IndexOf("alt=", StringComparison.Ordinal) == -1)
			{
				stringBuilder.Append(" alt=\"\"");
			}
			stringBuilder.Append("/>");
			return stringBuilder.ToString();
		}
	}
}
