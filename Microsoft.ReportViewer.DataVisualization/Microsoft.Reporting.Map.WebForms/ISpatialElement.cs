using Microsoft.SqlServer.Types;
using System.Drawing;

namespace Microsoft.Reporting.Map.WebForms
{
	internal interface ISpatialElement : IImageMapProvider
	{
		string Name
		{
			get;
			set;
		}

		string Layer
		{
			get;
			set;
		}

		string Category
		{
			get;
			set;
		}

		object this[string name]
		{
			get;
			set;
		}

		MapPoint MinimumExtent
		{
			get;
		}

		MapPoint MaximumExtent
		{
			get;
		}

		MapPoint[] Points
		{
			get;
		}

		bool Visible
		{
			get;
			set;
		}

		Offset Offset
		{
			get;
			set;
		}

		string Text
		{
			get;
			set;
		}

		string ToolTip
		{
			get;
			set;
		}

		Color BorderColor
		{
			get;
			set;
		}

		int BorderWidth
		{
			get;
			set;
		}

		Color Color
		{
			get;
			set;
		}

		Color SecondaryColor
		{
			get;
			set;
		}

		GradientType GradientType
		{
			get;
			set;
		}

		MapHatchStyle HatchStyle
		{
			get;
			set;
		}

		Color TextColor
		{
			get;
			set;
		}

		Font Font
		{
			get;
			set;
		}

		int ShadowOffset
		{
			get;
			set;
		}

		bool AddGeography(SqlGeography geography);

		bool AddGeometry(SqlGeometry geometry);

		bool LoadWKT(string wkt);

		bool AddWKT(string wkt);

		bool LoadWKB(byte[] wkb);

		bool AddWKB(byte[] wkb);

		string SaveWKT();

		byte[] SaveWKB();
	}
}
