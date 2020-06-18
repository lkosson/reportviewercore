using System;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;

namespace Microsoft.Reporting.Map.WebForms
{
	internal class ViewCenter : MapObject, ICloneable
	{
		private PointF point = new PointF(0f, 0f);

		private bool defaultValues;

		[SRCategory("CategoryAttribute_Values")]
		[SRDescription("DescriptionAttributeViewCenter_X")]
		[NotifyParentProperty(true)]
		[RefreshProperties(RefreshProperties.All)]
		[DefaultValue(50f)]
		public float X
		{
			get
			{
				return point.X;
			}
			set
			{
				point.X = value;
				DefaultValues = false;
				InvalidateDistanceScalePanel();
				InvalidateViewport(invalidateGridSections: false);
			}
		}

		[SRCategory("CategoryAttribute_Values")]
		[SRDescription("DescriptionAttributeViewCenter_Y")]
		[NotifyParentProperty(true)]
		[RefreshProperties(RefreshProperties.All)]
		[DefaultValue(50f)]
		public float Y
		{
			get
			{
				return point.Y;
			}
			set
			{
				point.Y = value;
				DefaultValues = false;
				InvalidateDistanceScalePanel();
				InvalidateViewport(invalidateGridSections: false);
			}
		}

		internal bool DefaultValues
		{
			get
			{
				return defaultValues;
			}
			set
			{
				defaultValues = value;
			}
		}

		public ViewCenter()
			: this(null)
		{
		}

		internal ViewCenter(object parent)
			: base(parent)
		{
		}

		internal ViewCenter(object parent, float x, float y)
			: this(parent)
		{
			point.X = x;
			point.Y = y;
		}

		public override string ToString()
		{
			return point.X.ToString(CultureInfo.CurrentCulture) + ", " + point.Y.ToString(CultureInfo.CurrentCulture);
		}

		public PointF ToPoint()
		{
			return new PointF(point.X, point.Y);
		}

		public static implicit operator PointF(ViewCenter viewCenter)
		{
			return viewCenter.GetPointF();
		}

		public object Clone()
		{
			return new ViewCenter(Parent, X, Y);
		}

		internal PointF GetPointF()
		{
			return point;
		}
	}
}
