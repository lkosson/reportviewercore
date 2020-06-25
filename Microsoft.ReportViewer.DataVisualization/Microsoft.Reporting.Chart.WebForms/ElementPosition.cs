using Microsoft.Reporting.Chart.WebForms.Design;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;

namespace Microsoft.Reporting.Chart.WebForms
{
	[SRDescription("DescriptionAttributeElementPosition_ElementPosition")]
	[TypeConverter(typeof(ElementPositionConverter))]
	[DefaultProperty("Data")]
	internal class ElementPosition
	{
		private float x;

		private float y;

		private float width;

		private float height;

		internal bool auto = true;

		internal CommonElements common;

		internal Chart chart;

		internal bool resetAreaAutoPosition;

		[SRCategory("CategoryAttributeMisc")]
		[Bindable(true)]
		[DefaultValue(0f)]
		[SRDescription("DescriptionAttributeElementPosition_X")]
		[NotifyParentProperty(true)]
		[RefreshProperties(RefreshProperties.All)]
		public float X
		{
			get
			{
				return x;
			}
			set
			{
				if ((double)value < 0.0 || (double)value > 100.0)
				{
					throw new ArgumentOutOfRangeException("value", SR.ExceptionElementPositionArgumentOutOfRange);
				}
				x = value;
				Auto = false;
				if (x + Width > 100f)
				{
					Width = 100f - x;
				}
				Invalidate();
			}
		}

		[SRCategory("CategoryAttributeMisc")]
		[Bindable(true)]
		[DefaultValue(0f)]
		[SRDescription("DescriptionAttributeElementPosition_Y")]
		[NotifyParentProperty(true)]
		[RefreshProperties(RefreshProperties.All)]
		public float Y
		{
			get
			{
				return y;
			}
			set
			{
				if ((double)value < 0.0 || (double)value > 100.0)
				{
					throw new ArgumentOutOfRangeException("value", SR.ExceptionElementPositionArgumentOutOfRange);
				}
				y = value;
				Auto = false;
				if (y + Height > 100f)
				{
					Height = 100f - y;
				}
				Invalidate();
			}
		}

		[SRCategory("CategoryAttributeMisc")]
		[Bindable(true)]
		[DefaultValue(0f)]
		[SRDescription("DescriptionAttributeElementPosition_Width")]
		[NotifyParentProperty(true)]
		[RefreshProperties(RefreshProperties.All)]
		public float Width
		{
			get
			{
				return width;
			}
			set
			{
				if ((double)value < 0.0 || (double)value > 100.0)
				{
					throw new ArgumentOutOfRangeException("value", SR.ExceptionElementPositionArgumentOutOfRange);
				}
				width = value;
				Auto = false;
				if (x + Width > 100f)
				{
					x = 100f - Width;
				}
				Invalidate();
			}
		}

		[SRCategory("CategoryAttributeMisc")]
		[Bindable(true)]
		[DefaultValue(0f)]
		[SRDescription("DescriptionAttributeElementPosition_Height")]
		[NotifyParentProperty(true)]
		[RefreshProperties(RefreshProperties.All)]
		public float Height
		{
			get
			{
				return height;
			}
			set
			{
				if ((double)value < 0.0 || (double)value > 100.0)
				{
					throw new ArgumentOutOfRangeException("value", SR.ExceptionElementPositionArgumentOutOfRange);
				}
				height = value;
				Auto = false;
				if (y + Height > 100f)
				{
					y = 100f - Height;
				}
				Invalidate();
			}
		}

		[SRCategory("CategoryAttributeMisc")]
		[Bindable(true)]
		[DefaultValue(true)]
		[SRDescription("DescriptionAttributeElementPosition_Auto")]
		[NotifyParentProperty(true)]
		[RefreshProperties(RefreshProperties.All)]
		public bool Auto
		{
			get
			{
				return auto;
			}
			set
			{
				if (value != auto)
				{
					ResetAllAreasAutoPosition(value);
					if (value)
					{
						x = 0f;
						y = 0f;
						width = 0f;
						height = 0f;
					}
					auto = value;
					Invalidate();
				}
			}
		}

		public ElementPosition()
		{
		}

		public ElementPosition(float x, float y, float width, float height)
		{
			auto = false;
			this.x = x;
			this.y = y;
			this.width = width;
			this.height = height;
		}

		private void ResetAllAreasAutoPosition(bool autoValue)
		{
			if (!resetAreaAutoPosition)
			{
				return;
			}
			if (chart == null && common != null)
			{
				chart = (Chart)common.container.GetService(typeof(Chart));
			}
			if (chart == null || !chart.IsDesignMode() || chart.serializing || chart.ChartAreas.Count <= 1)
			{
				return;
			}
			bool flag = chart.ChartAreas[0].Position.Auto;
			bool flag2 = true;
			foreach (ChartArea chartArea in chart.ChartAreas)
			{
				if (chartArea.Position.Auto != flag)
				{
					flag2 = false;
					break;
				}
			}
			if (!flag2)
			{
				return;
			}
			string messageChangingChartAreaPositionProperty = SR.MessageChangingChartAreaPositionProperty;
			messageChangingChartAreaPositionProperty = ((!autoValue) ? (messageChangingChartAreaPositionProperty + SR.MessageChangingChartAreaPositionConfirmCustom) : (messageChangingChartAreaPositionProperty + SR.MessageChangingChartAreaPositionConfirmAutomatic));
			foreach (ChartArea chartArea2 in chart.ChartAreas)
			{
				if (autoValue)
				{
					SetPositionNoAuto(0f, 0f, 0f, 0f);
				}
				chartArea2.Position.auto = autoValue;
			}
		}

		public RectangleF ToRectangleF()
		{
			return new RectangleF(x, y, width, height);
		}

		public void FromRectangleF(RectangleF rect)
		{
			x = rect.X;
			y = rect.Y;
			width = rect.Width;
			height = rect.Height;
			auto = false;
		}

		public SizeF GetSize()
		{
			return new SizeF(width, height);
		}

		public float Bottom()
		{
			return y + height;
		}

		public float Right()
		{
			return x + width;
		}

		public override bool Equals(object obj)
		{
			if (obj is ElementPosition)
			{
				ElementPosition elementPosition = (ElementPosition)obj;
				if (auto && auto == elementPosition.auto)
				{
					return true;
				}
				if (x == elementPosition.x && y == elementPosition.y && width == elementPosition.width && height == elementPosition.height)
				{
					return true;
				}
			}
			return base.Equals(obj);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public override string ToString()
		{
			string result = "Auto";
			if (!auto)
			{
				result = x.ToString(CultureInfo.CurrentCulture) + ", " + y.ToString(CultureInfo.CurrentCulture) + ", " + width.ToString(CultureInfo.CurrentCulture) + ", " + height.ToString(CultureInfo.CurrentCulture);
			}
			return result;
		}

		internal void SetPositionNoAuto(float x, float y, float width, float height)
		{
			bool flag = auto;
			this.x = x;
			this.y = y;
			this.width = width;
			this.height = height;
			auto = flag;
		}

		private void Invalidate()
		{
		}
	}
}
