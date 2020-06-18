using System.ComponentModel;

namespace Microsoft.Reporting.Gauge.WebForms
{
	internal class CircularSpecialPosition : SpecialPosition
	{
		private CircularPinLabel pinCircularLabel;

		[SRCategory("CategoryAppearance")]
		[SRDescription("DescriptionAttributeLabelStyle3")]
		[TypeConverter(typeof(NoNameExpandableObjectConverter))]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public CircularPinLabel LabelStyle
		{
			get
			{
				return pinCircularLabel;
			}
			set
			{
				pinCircularLabel = value;
				pinCircularLabel.Parent = this;
				Invalidate();
			}
		}

		public CircularSpecialPosition()
			: this(null)
		{
		}

		public CircularSpecialPosition(object parent)
			: base(parent)
		{
			pinCircularLabel = new CircularPinLabel(this);
		}
	}
}
