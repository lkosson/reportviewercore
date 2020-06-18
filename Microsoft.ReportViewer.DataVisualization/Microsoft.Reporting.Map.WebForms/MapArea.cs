using System;
using System.ComponentModel;
using System.Globalization;
using System.Text;

namespace Microsoft.Reporting.Map.WebForms
{
	[DefaultProperty("ToolTip")]
	[Description("Represent a custom map area element, that has a user-defined shape and dimensions.")]
	internal class MapArea : IMapAreaAttributes
	{
		private string toolTip = "";

		private string href = "";

		private string attributes = "";

		private int[] coordinates = new int[4];

		private string name = "Map Area";

		private bool custom = true;

		private MapAreaShape shape;

		private object mapAreaTag;

		[Browsable(false)]
		[Description("Indicates that the map area is custom.")]
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

		[SRCategory("CategoryAttribute_Shape")]
		[Bindable(true)]
		[SRDescription("DescriptionAttributeMapArea_Coordinates")]
		[DefaultValue("")]
		[TypeConverter(typeof(MapAreaCoordinatesConverter))]
		public int[] Coordinates
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

		[SRCategory("CategoryAttribute_Shape")]
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

		[SRCategory("CategoryAttribute_Data")]
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

		[SRCategory("CategoryAttribute_MapArea")]
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

		[SRCategory("CategoryAttribute_MapArea")]
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
		[SRCategory("CategoryAttribute_MapArea")]
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
				if (Href.ToUpper(CultureInfo.InvariantCulture).StartsWith("WWW.", StringComparison.Ordinal))
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
			bool flag = true;
			int[] array = Coordinates;
			foreach (int value in array)
			{
				if (!flag)
				{
					stringBuilder.Append(",");
				}
				flag = false;
				stringBuilder.Append(value);
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
