using Microsoft.Reporting.Chart.WebForms.Design;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;

namespace Microsoft.Reporting.Chart.WebForms
{
	[SRDescription("DescriptionAttributeMargins_Margins")]
	[TypeConverter(typeof(MarginExpandableObjectConverter))]
	internal class Margins
	{
		private int top;

		private int bottom;

		private int left;

		private int right;

		internal CommonElements Common;

		[SRCategory("CategoryAttributeMisc")]
		[DefaultValue(0)]
		[SRDescription("DescriptionAttributeMargins_Top")]
		[RefreshProperties(RefreshProperties.All)]
		[NotifyParentProperty(true)]
		public int Top
		{
			get
			{
				return top;
			}
			set
			{
				if (value < 0)
				{
					throw new ArgumentException(SR.ExceptionMarginTopIsNegative, "value");
				}
				top = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryAttributeMisc")]
		[DefaultValue(0)]
		[SRDescription("DescriptionAttributeMargins_Bottom")]
		[RefreshProperties(RefreshProperties.All)]
		[NotifyParentProperty(true)]
		public int Bottom
		{
			get
			{
				return bottom;
			}
			set
			{
				if (value < 0)
				{
					throw new ArgumentException(SR.ExceptionMarginBottomIsNegative, "value");
				}
				bottom = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryAttributeMisc")]
		[DefaultValue(0)]
		[RefreshProperties(RefreshProperties.All)]
		[SRDescription("DescriptionAttributeMargins_Left")]
		[NotifyParentProperty(true)]
		public int Left
		{
			get
			{
				return left;
			}
			set
			{
				if (value < 0)
				{
					throw new ArgumentException(SR.ExceptionMarginLeftIsNegative, "value");
				}
				left = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryAttributeMisc")]
		[DefaultValue(0)]
		[SRDescription("DescriptionAttributeMargins_Right")]
		[RefreshProperties(RefreshProperties.All)]
		[NotifyParentProperty(true)]
		public int Right
		{
			get
			{
				return right;
			}
			set
			{
				if (value < 0)
				{
					throw new ArgumentException(SR.ExceptionMarginRightIsNegative, "value");
				}
				right = value;
				Invalidate();
			}
		}

		public Margins()
		{
		}

		public Margins(int top, int bottom, int left, int right)
		{
			this.top = top;
			this.bottom = bottom;
			this.left = left;
			this.right = right;
		}

		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture, "{0:D}, {1:D}, {2:D}, {3:D}", Top, Bottom, Left, Right);
		}

		public override bool Equals(object obj)
		{
			Margins margins = obj as Margins;
			if (margins != null && Top == margins.Top && Bottom == margins.Bottom && Left == margins.Left && Right == margins.Right)
			{
				return true;
			}
			return false;
		}

		public override int GetHashCode()
		{
			return Top.GetHashCode() + Bottom.GetHashCode() + Left.GetHashCode() + Right.GetHashCode();
		}

		public bool IsEmpty()
		{
			if (Top == 0 && Bottom == 0 && Left == 0)
			{
				return Right == 0;
			}
			return false;
		}

		public RectangleF ToRectangleF()
		{
			return new RectangleF(Left, Top, Right, Bottom);
		}

		private void Invalidate()
		{
		}
	}
}
