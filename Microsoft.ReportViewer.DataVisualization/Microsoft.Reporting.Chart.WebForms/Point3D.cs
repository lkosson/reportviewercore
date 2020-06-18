using System.ComponentModel;
using System.Drawing;

namespace Microsoft.Reporting.Chart.WebForms
{
	internal class Point3D
	{
		private PointF coordXY = new PointF(0f, 0f);

		private float coordZ;

		[Bindable(true)]
		[DefaultValue(0)]
		[SRDescription("DescriptionAttributePoint3D_X")]
		public float X
		{
			get
			{
				return coordXY.X;
			}
			set
			{
				coordXY.X = value;
			}
		}

		[Bindable(true)]
		[DefaultValue(0)]
		[SRDescription("DescriptionAttributePoint3D_Y")]
		public float Y
		{
			get
			{
				return coordXY.Y;
			}
			set
			{
				coordXY.Y = value;
			}
		}

		[Bindable(true)]
		[DefaultValue(0)]
		[SRDescription("DescriptionAttributePoint3D_Z")]
		public float Z
		{
			get
			{
				return coordZ;
			}
			set
			{
				coordZ = value;
			}
		}

		[Bindable(true)]
		[DefaultValue(0)]
		[SRDescription("DescriptionAttributePoint3D_PointF")]
		public PointF PointF
		{
			get
			{
				return coordXY;
			}
			set
			{
				coordXY = new PointF(value.X, value.Y);
			}
		}

		public Point3D(float x, float y, float z)
		{
			coordXY = new PointF(x, y);
			coordZ = z;
		}

		public Point3D()
		{
		}
	}
}
