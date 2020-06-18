using System;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;

namespace Microsoft.Reporting.Gauge.WebForms
{
	internal class GaugeLocation : GaugeObject, ICloneable
	{
		private PointF point = new PointF(0f, 0f);

		private bool defaultValues;

		[SRCategory("CategoryValues")]
		[SRDescription("DescriptionAttributeGaugeLocation_X")]
		[NotifyParentProperty(true)]
		[RefreshProperties(RefreshProperties.All)]
		[ValidateBound(0.0, 100.0, false)]
		public float X
		{
			get
			{
				return point.X;
			}
			set
			{
				if ((double)value > 100000000.0 || (double)value < -100000000.0)
				{
					throw new ArgumentException(Utils.SRGetStr("ExceptionOutOfrange", -100000000.0, 100000000.0));
				}
				RemoveAutoLayout();
				point.X = value;
				DefaultValues = false;
				Invalidate();
			}
		}

		[SRCategory("CategoryValues")]
		[SRDescription("DescriptionAttributeY")]
		[NotifyParentProperty(true)]
		[RefreshProperties(RefreshProperties.All)]
		[ValidateBound(0.0, 100.0, false)]
		public float Y
		{
			get
			{
				return point.Y;
			}
			set
			{
				if ((double)value > 100000000.0 || (double)value < -100000000.0)
				{
					throw new ArgumentException(Utils.SRGetStr("ExceptionOutOfrange", -100000000.0, 100000000.0));
				}
				RemoveAutoLayout();
				point.Y = value;
				DefaultValues = false;
				Invalidate();
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

		public GaugeLocation()
			: this(null)
		{
		}

		internal GaugeLocation(object parent)
			: base(parent)
		{
		}

		internal GaugeLocation(object parent, float x, float y)
			: this(parent)
		{
			point.X = x;
			point.Y = y;
		}

		public override string ToString()
		{
			if (Parent != null && Parent is GaugeBase)
			{
				GaugeBase gaugeBase = (GaugeBase)Parent;
				if (gaugeBase.Location == this && gaugeBase.Common.GaugeContainer.AutoLayout && gaugeBase.Parent.Length == 0)
				{
					return "(AutoLayout)";
				}
			}
			return point.X.ToString(CultureInfo.CurrentCulture) + ", " + point.Y.ToString(CultureInfo.CurrentCulture);
		}

		public PointF ToPoint()
		{
			return new PointF(point.X, point.Y);
		}

		public static implicit operator PointF(GaugeLocation location)
		{
			return location.GetPointF();
		}

		public object Clone()
		{
			return new GaugeLocation(Parent, X, Y);
		}

		internal PointF GetPointF()
		{
			return point;
		}

		private void RemoveAutoLayout()
		{
			if (Parent != null && Parent is GaugeBase)
			{
				GaugeBase gaugeBase = (GaugeBase)Parent;
				if (gaugeBase.Location == this && gaugeBase.Parent == string.Empty && gaugeBase.Common != null && gaugeBase.Common.GaugeContainer != null && gaugeBase.Common.GaugeContainer.AutoLayout && gaugeBase.Common.GaugeCore != null && !gaugeBase.Common.GaugeCore.layoutFlag)
				{
					gaugeBase.Common.GaugeContainer.AutoLayout = false;
				}
			}
		}
	}
}
