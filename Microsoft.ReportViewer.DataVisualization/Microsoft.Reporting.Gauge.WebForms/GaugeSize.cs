using System;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;

namespace Microsoft.Reporting.Gauge.WebForms
{
	internal sealed class GaugeSize : GaugeObject, ICloneable
	{
		private SizeF size = new SizeF(100f, 100f);

		private bool defaultValues;

		[SRCategory("CategoryValues")]
		[SRDescription("DescriptionAttributeGaugeSize_Width")]
		[NotifyParentProperty(true)]
		[RefreshProperties(RefreshProperties.All)]
		[ValidateBound(0.0, 100.0)]
		public float Width
		{
			get
			{
				return size.Width;
			}
			set
			{
				if ((double)value > 100000000.0 || (double)value < -100000000.0)
				{
					throw new ArgumentException(Utils.SRGetStr("ExceptionOutOfrange", -100000000.0, 100000000.0));
				}
				RemoveAutoLayout();
				size.Width = Math.Max(value, 0f);
				DefaultValues = false;
				Invalidate();
			}
		}

		[SRCategory("CategoryValues")]
		[SRDescription("DescriptionAttributeGaugeSize_Height")]
		[NotifyParentProperty(true)]
		[RefreshProperties(RefreshProperties.All)]
		[ValidateBound(0.0, 100.0)]
		public float Height
		{
			get
			{
				return size.Height;
			}
			set
			{
				if ((double)value > 100000000.0 || (double)value < -100000000.0)
				{
					throw new ArgumentException(Utils.SRGetStr("ExceptionOutOfrange", -100000000.0, 100000000.0));
				}
				RemoveAutoLayout();
				size.Height = Math.Max(value, 0f);
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

		public GaugeSize()
			: this(null)
		{
		}

		internal GaugeSize(object parent)
			: base(parent)
		{
		}

		internal GaugeSize(object parent, float width, float height)
			: this(parent)
		{
			Width = Math.Max(width, 0f);
			Height = Math.Max(height, 0f);
		}

		internal GaugeSize(object parent, SizeF size)
			: this(parent)
		{
			Width = size.Width;
			Height = size.Height;
		}

		public override string ToString()
		{
			if (Parent != null && Parent is GaugeBase)
			{
				GaugeBase gaugeBase = (GaugeBase)Parent;
				if (gaugeBase.Size == this && gaugeBase.Common.GaugeContainer.AutoLayout && gaugeBase.Parent.Length == 0)
				{
					return "(AutoLayout)";
				}
			}
			return size.Width.ToString(CultureInfo.CurrentCulture) + ", " + size.Height.ToString(CultureInfo.CurrentCulture);
		}

		public SizeF ToSize()
		{
			return new SizeF(size);
		}

		public static implicit operator SizeF(GaugeSize size)
		{
			return size.ToSize();
		}

		public object Clone()
		{
			return new GaugeSize(Parent, size);
		}

		private void RemoveAutoLayout()
		{
			if (Parent != null && Parent is GaugeBase)
			{
				GaugeBase gaugeBase = (GaugeBase)Parent;
				if (gaugeBase.Size == this && gaugeBase.Parent == string.Empty && gaugeBase.Common != null && gaugeBase.Common.GaugeContainer != null && gaugeBase.Common.GaugeContainer.AutoLayout && gaugeBase.Common.GaugeCore != null && !gaugeBase.Common.GaugeCore.layoutFlag)
				{
					gaugeBase.Common.GaugeContainer.AutoLayout = false;
				}
			}
		}
	}
}
