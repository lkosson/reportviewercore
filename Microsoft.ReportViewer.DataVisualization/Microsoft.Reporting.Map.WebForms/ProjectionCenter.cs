using System.ComponentModel;

namespace Microsoft.Reporting.Map.WebForms
{
	internal class ProjectionCenter : MapObject
	{
		private bool xIsNaN = true;

		private double x = double.NaN;

		private bool yIsNaN = true;

		private double y = double.NaN;

		[SRCategory("CategoryAttribute_Coordinates")]
		[SRDescription("DescriptionAttributeProjectionCenter_X")]
		[RefreshProperties(RefreshProperties.All)]
		[TypeConverter(typeof(DoubleAutoValueConverter))]
		[NotifyParentProperty(true)]
		[DefaultValue(double.NaN)]
		public double X
		{
			get
			{
				return x;
			}
			set
			{
				x = value;
				xIsNaN = double.IsNaN(value);
				Invalidate();
			}
		}

		[SRCategory("CategoryAttribute_Coordinates")]
		[SRDescription("DescriptionAttributeProjectionCenter_Y")]
		[RefreshProperties(RefreshProperties.All)]
		[TypeConverter(typeof(DoubleAutoValueConverter))]
		[NotifyParentProperty(true)]
		[DefaultValue(double.NaN)]
		public double Y
		{
			get
			{
				return y;
			}
			set
			{
				y = value;
				yIsNaN = double.IsNaN(value);
				Invalidate();
			}
		}

		public ProjectionCenter()
			: this(null)
		{
		}

		internal ProjectionCenter(object parent)
			: base(parent)
		{
		}

		internal override void Invalidate()
		{
			MapCore mapCore = (MapCore)Parent;
			if (mapCore != null)
			{
				mapCore.InvalidateCachedPaths();
				mapCore.ResetCachedBoundsAfterProjection();
				mapCore.InvalidateDistanceScalePanel();
				mapCore.InvalidateViewport();
			}
		}

		internal bool IsXNaN()
		{
			return xIsNaN;
		}

		internal bool IsYNaN()
		{
			return yIsNaN;
		}
	}
}
