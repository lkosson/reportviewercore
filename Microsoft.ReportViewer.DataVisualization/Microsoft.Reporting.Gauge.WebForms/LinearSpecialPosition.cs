using System.ComponentModel;

namespace Microsoft.Reporting.Gauge.WebForms
{
	internal class LinearSpecialPosition : SpecialPosition
	{
		private LinearPinLabel pinLinearLabel;

		[SRCategory("CategoryAppearance")]
		[SRDescription("DescriptionAttributeLabelStyle3")]
		[TypeConverter(typeof(NoNameExpandableObjectConverter))]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public LinearPinLabel LabelStyle
		{
			get
			{
				return pinLinearLabel;
			}
			set
			{
				pinLinearLabel = value;
				pinLinearLabel.Parent = this;
				Invalidate();
			}
		}

		public LinearSpecialPosition()
			: this(null)
		{
		}

		public LinearSpecialPosition(object parent)
			: base(parent)
		{
			pinLinearLabel = new LinearPinLabel(this);
		}
	}
}
