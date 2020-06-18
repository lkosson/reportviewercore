using System.ComponentModel;

namespace Microsoft.Reporting.Map.WebForms
{
	internal class MapLimits : MapObject
	{
		private bool minimumXIsNaN = true;

		private double minimumX = double.NaN;

		private bool minimumYIsNaN = true;

		private double minimumY = double.NaN;

		private bool maximumXIsNaN = true;

		private double maximumX = double.NaN;

		private bool maximumYIsNaN = true;

		private double maximumY = double.NaN;

		[SRCategory("CategoryAttribute_Coordinates")]
		[SRDescription("DescriptionAttributeMapLimits_MinimumX")]
		[RefreshProperties(RefreshProperties.All)]
		[TypeConverter(typeof(DoubleAutoValueConverter))]
		[NotifyParentProperty(true)]
		[DefaultValue(double.NaN)]
		public double MinimumX
		{
			get
			{
				return minimumX;
			}
			set
			{
				minimumX = value;
				minimumXIsNaN = double.IsNaN(value);
				Invalidate();
			}
		}

		[SRCategory("CategoryAttribute_Coordinates")]
		[SRDescription("DescriptionAttributeMapLimits_MinimumY")]
		[RefreshProperties(RefreshProperties.All)]
		[TypeConverter(typeof(DoubleAutoValueConverter))]
		[NotifyParentProperty(true)]
		[DefaultValue(double.NaN)]
		public double MinimumY
		{
			get
			{
				return minimumY;
			}
			set
			{
				minimumY = value;
				minimumYIsNaN = double.IsNaN(value);
				Invalidate();
			}
		}

		[SRCategory("CategoryAttribute_Coordinates")]
		[SRDescription("DescriptionAttributeMapLimits_MaximumX")]
		[RefreshProperties(RefreshProperties.All)]
		[TypeConverter(typeof(DoubleAutoValueConverter))]
		[NotifyParentProperty(true)]
		[DefaultValue(double.NaN)]
		public double MaximumX
		{
			get
			{
				return maximumX;
			}
			set
			{
				maximumX = value;
				maximumXIsNaN = double.IsNaN(value);
				Invalidate();
			}
		}

		[SRCategory("CategoryAttribute_Coordinates")]
		[SRDescription("DescriptionAttributeMapLimits_MaximumY")]
		[RefreshProperties(RefreshProperties.All)]
		[TypeConverter(typeof(DoubleAutoValueConverter))]
		[NotifyParentProperty(true)]
		[DefaultValue(double.NaN)]
		public double MaximumY
		{
			get
			{
				return maximumY;
			}
			set
			{
				maximumY = value;
				maximumYIsNaN = double.IsNaN(value);
				Invalidate();
			}
		}

		public MapLimits()
			: this(null)
		{
		}

		internal MapLimits(object parent)
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

		internal bool IsMinimumXNaN()
		{
			return minimumXIsNaN;
		}

		internal bool IsMinimumYNaN()
		{
			return minimumYIsNaN;
		}

		internal bool IsMaximumXNaN()
		{
			return maximumXIsNaN;
		}

		internal bool IsMaximumYNaN()
		{
			return maximumYIsNaN;
		}
	}
}
