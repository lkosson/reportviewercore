using Microsoft.Reporting.Chart.WebForms.Utilities;
using System.ComponentModel;

namespace Microsoft.Reporting.Chart.WebForms
{
	[SRDescription("DescriptionAttributeEllipseAnnotation_EllipseAnnotation")]
	internal class EllipseAnnotation : RectangleAnnotation
	{
		[SRCategory("CategoryAttributeMisc")]
		[Bindable(true)]
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[SerializationVisibility(SerializationVisibility.Hidden)]
		[SRDescription("DescriptionAttributeAnnotationType4")]
		public override string AnnotationType => "Ellipse";

		public EllipseAnnotation()
		{
			isEllipse = true;
		}
	}
}
