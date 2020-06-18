using System.ComponentModel;

namespace Microsoft.Reporting.Map.WebForms
{
	internal class LinearSpecialPosition : SpecialPosition
	{
		private LinearPinLabel pinLinearLabel;

		[SRCategory("CategoryAttribute_Appearance")]
		[SRDescription("DescriptionAttributeLinearSpecialPosition_LabelStyle")]
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
