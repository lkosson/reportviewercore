using Microsoft.Reporting.Chart.WebForms.Utilities;
using System.ComponentModel;
using System.Drawing;

namespace Microsoft.Reporting.Chart.WebForms
{
	[SRDescription("DescriptionAttributeVerticalLineAnnotation_VerticalLineAnnotation")]
	internal class VerticalLineAnnotation : LineAnnotation
	{
		[SRCategory("CategoryAttributeMisc")]
		[Bindable(true)]
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[SerializationVisibility(SerializationVisibility.Hidden)]
		[SRDescription("DescriptionAttributeAnnotationType4")]
		public override string AnnotationType => "VerticalLine";

		internal override void AdjustLineCoordinates(ref PointF point1, ref PointF point2, ref RectangleF selectionRect)
		{
			point2.X = point1.X;
			selectionRect.Width = 0f;
			base.AdjustLineCoordinates(ref point1, ref point2, ref selectionRect);
		}

		internal override RectangleF GetContentPosition()
		{
			return new RectangleF(float.NaN, float.NaN, 0f, float.NaN);
		}
	}
}
