using System;
using System.ComponentModel;
using System.Text;

namespace Microsoft.Reporting.Gauge.WebForms
{
	[DefaultProperty("ToolTip")]
	[SRDescription("DescriptionAttributeMapArea_MapArea")]
	internal class MapArea : IMapAreaAttributes
	{
		private string toolTip = "";

		private string href = "";

		private string attributes = "";

		private float[] coordinates = new float[4];

		private string name = "Map Area";

		private bool custom = true;

		private MapAreaShape shape;

		private object imagMapProviderTag;

		[Browsable(false)]
		[SRDescription("DescriptionAttributeMapArea_Custom")]
		[DefaultValue("")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[SerializationVisibility(SerializationVisibility.Hidden)]
		internal bool Custom
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

		[Category("Shape")]
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

		[Category("Shape")]
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

		[Category("Data")]
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

		[Category("MapArea")]
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

		[Category("MapArea")]
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

		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[Category("MapArea")]
		[Bindable(true)]
		[SRDescription("DescriptionAttributeMapAreaAttributes4")]
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

		internal object Tag
		{
			get
			{
				return imagMapProviderTag;
			}
			set
			{
				imagMapProviderTag = value;
			}
		}

		internal string GetTag()
		{
			StringBuilder stringBuilder = new StringBuilder("\r\n<AREA SHAPE=\"", 120);
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
				stringBuilder.Append(" HREF=\"");
				if (Href.StartsWith("WWW.", StringComparison.OrdinalIgnoreCase))
				{
					stringBuilder.Append("http://");
				}
				stringBuilder.Append(Href);
				stringBuilder.Append("\"");
			}
			if (ToolTip.Length > 0)
			{
				stringBuilder.Append(" Title=\"");
				stringBuilder.Append(ToolTip);
				stringBuilder.Append("\"");
			}
			stringBuilder.Append(" COORDS=\"");
			float[] array = new float[Coordinates.Length];
			Coordinates.CopyTo(array, 0);
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
			stringBuilder.Append(">");
			return stringBuilder.ToString();
		}
	}
}
