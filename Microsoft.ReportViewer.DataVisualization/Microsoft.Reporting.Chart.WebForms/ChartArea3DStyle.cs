using System;
using System.ComponentModel;

namespace Microsoft.Reporting.Chart.WebForms
{
	internal class ChartArea3DStyle
	{
		private ChartArea chartArea;

		private bool enable3D;

		private bool rightAngleAxes = true;

		private bool clustered;

		private LightStyle light = LightStyle.Simplistic;

		private int perspective;

		private int xAngle = 30;

		private int yAngle = 30;

		private int wallWidth = 7;

		private int pointDepth = 100;

		private int pointGapDepth = 100;

		[SRCategory("CategoryAttribute3D")]
		[Bindable(true)]
		[DefaultValue(false)]
		[SRDescription("DescriptionAttributeChartArea3DStyle_Enable3D")]
		[ParenthesizePropertyName(true)]
		public bool Enable3D
		{
			get
			{
				return enable3D;
			}
			set
			{
				if (enable3D != value)
				{
					enable3D = value;
					if (chartArea != null)
					{
						chartArea.Invalidate(invalidateAreaOnly: true);
					}
				}
			}
		}

		[SRCategory("CategoryAttribute3D")]
		[Bindable(true)]
		[DefaultValue(true)]
		[SRDescription("DescriptionAttributeChartArea3DStyle_RightAngleAxes")]
		[RefreshProperties(RefreshProperties.All)]
		public bool RightAngleAxes
		{
			get
			{
				return rightAngleAxes;
			}
			set
			{
				rightAngleAxes = value;
				if (rightAngleAxes)
				{
					perspective = 0;
				}
				if (chartArea != null)
				{
					chartArea.Invalidate(invalidateAreaOnly: true);
				}
			}
		}

		[SRCategory("CategoryAttribute3D")]
		[Bindable(true)]
		[DefaultValue(false)]
		[SRDescription("DescriptionAttributeChartArea3DStyle_Clustered")]
		public bool Clustered
		{
			get
			{
				return clustered;
			}
			set
			{
				clustered = value;
				if (chartArea != null)
				{
					chartArea.Invalidate(invalidateAreaOnly: true);
				}
			}
		}

		[SRCategory("CategoryAttribute3D")]
		[Bindable(true)]
		[DefaultValue(typeof(LightStyle), "Simplistic")]
		[SRDescription("DescriptionAttributeChartArea3DStyle_Light")]
		public LightStyle Light
		{
			get
			{
				return light;
			}
			set
			{
				light = value;
				if (chartArea != null)
				{
					chartArea.Invalidate(invalidateAreaOnly: true);
				}
			}
		}

		[SRCategory("CategoryAttribute3D")]
		[Bindable(true)]
		[DefaultValue(0)]
		[SRDescription("DescriptionAttributeChartArea3DStyle_Perspective")]
		[RefreshProperties(RefreshProperties.All)]
		public int Perspective
		{
			get
			{
				return perspective;
			}
			set
			{
				if (value < 0 || value > 100)
				{
					throw new ArgumentOutOfRangeException("value", SR.ExceptionChartArea3DPerspectiveInvalid);
				}
				perspective = value;
				if (perspective != 0)
				{
					rightAngleAxes = false;
				}
				if (chartArea != null)
				{
					chartArea.Invalidate(invalidateAreaOnly: true);
				}
			}
		}

		[SRCategory("CategoryAttribute3D")]
		[Bindable(true)]
		[DefaultValue(30)]
		[SRDescription("DescriptionAttributeChartArea3DStyle_XAngle")]
		[RefreshProperties(RefreshProperties.All)]
		public int XAngle
		{
			get
			{
				return xAngle;
			}
			set
			{
				if (value < -90 || value > 90)
				{
					throw new ArgumentOutOfRangeException("value", SR.ExceptionChartArea3DXAxisRotationInvalid);
				}
				xAngle = value;
				if (chartArea != null)
				{
					chartArea.Invalidate(invalidateAreaOnly: true);
				}
			}
		}

		[SRCategory("CategoryAttribute3D")]
		[Bindable(true)]
		[DefaultValue(30)]
		[SRDescription("DescriptionAttributeChartArea3DStyle_YAngle")]
		[RefreshProperties(RefreshProperties.All)]
		public int YAngle
		{
			get
			{
				return yAngle;
			}
			set
			{
				if (value < -180 || value > 180)
				{
					throw new ArgumentOutOfRangeException("value", SR.ExceptionChartArea3DYAxisRotationInvalid);
				}
				yAngle = value;
				if (chartArea != null)
				{
					chartArea.Invalidate(invalidateAreaOnly: true);
				}
			}
		}

		[SRCategory("CategoryAttribute3D")]
		[Bindable(true)]
		[DefaultValue(7)]
		[SRDescription("DescriptionAttributeChartArea3DStyle_WallWidth")]
		[RefreshProperties(RefreshProperties.All)]
		public int WallWidth
		{
			get
			{
				return wallWidth;
			}
			set
			{
				if (value < 0 || value > 30)
				{
					throw new ArgumentOutOfRangeException("value", SR.ExceptionChartArea3DWallWidthInvalid);
				}
				wallWidth = value;
				if (chartArea != null)
				{
					chartArea.Invalidate(invalidateAreaOnly: true);
				}
			}
		}

		[SRCategory("CategoryAttribute3D")]
		[Bindable(true)]
		[DefaultValue(100)]
		[SRDescription("DescriptionAttributeChartArea3DStyle_PointDepth")]
		[RefreshProperties(RefreshProperties.All)]
		public int PointDepth
		{
			get
			{
				return pointDepth;
			}
			set
			{
				if (value < 0 || value > 1000)
				{
					throw new ArgumentOutOfRangeException("value", SR.ExceptionChartArea3DPointsDepthInvalid);
				}
				pointDepth = value;
				if (chartArea != null)
				{
					chartArea.Invalidate(invalidateAreaOnly: true);
				}
			}
		}

		[SRCategory("CategoryAttribute3D")]
		[Bindable(true)]
		[DefaultValue(100)]
		[SRDescription("DescriptionAttributeChartArea3DStyle_PointGapDepth")]
		[RefreshProperties(RefreshProperties.All)]
		public int PointGapDepth
		{
			get
			{
				return pointGapDepth;
			}
			set
			{
				if (value < 0 || value > 1000)
				{
					throw new ArgumentOutOfRangeException("value", SR.ExceptionChartArea3DPointsGapInvalid);
				}
				pointGapDepth = value;
				if (chartArea != null)
				{
					chartArea.Invalidate(invalidateAreaOnly: true);
				}
			}
		}

		public ChartArea3DStyle()
		{
		}

		public ChartArea3DStyle(ChartArea chartArea)
		{
			this.chartArea = chartArea;
		}

		internal void Initialize(ChartArea chartArea)
		{
			this.chartArea = chartArea;
		}
	}
}
