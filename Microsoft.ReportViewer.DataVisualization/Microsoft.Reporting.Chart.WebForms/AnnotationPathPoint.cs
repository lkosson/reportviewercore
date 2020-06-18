using Microsoft.Reporting.Chart.WebForms.Utilities;
using System.ComponentModel;

namespace Microsoft.Reporting.Chart.WebForms
{
	[SRDescription("DescriptionAttributeAnnotationPathPoint_AnnotationPathPoint")]
	internal class AnnotationPathPoint
	{
		private float x;

		private float y;

		private byte pointType = 1;

		[SRCategory("CategoryAttributePosition")]
		[DefaultValue(0f)]
		[Browsable(true)]
		[SRDescription("DescriptionAttributeAnnotationPathPoint_X")]
		public float X
		{
			get
			{
				return x;
			}
			set
			{
				x = value;
			}
		}

		[SRCategory("CategoryAttributePosition")]
		[DefaultValue(0f)]
		[Browsable(true)]
		[SRDescription("DescriptionAttributeAnnotationPathPoint_Y")]
		public float Y
		{
			get
			{
				return y;
			}
			set
			{
				y = value;
			}
		}

		[SRCategory("CategoryAttributePosition")]
		[DefaultValue(typeof(byte), "1")]
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[SRDescription("DescriptionAttributeAnnotationPathPoint_Name")]
		public byte PointType
		{
			get
			{
				return pointType;
			}
			set
			{
				pointType = value;
			}
		}

		[SRCategory("CategoryAttributeMisc")]
		[DefaultValue("PathPoint")]
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[SRDescription("DescriptionAttributeAnnotationPathPoint_Name")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[SerializationVisibility(SerializationVisibility.Hidden)]
		public string Name => "PathPoint";

		public AnnotationPathPoint()
		{
		}

		public AnnotationPathPoint(float x, float y)
		{
			this.x = x;
			this.y = y;
		}

		public AnnotationPathPoint(float x, float y, byte type)
		{
			this.x = x;
			this.y = y;
			pointType = type;
		}
	}
}
