using Microsoft.ReportingServices.RdlObjectModel;

namespace Microsoft.ReportingServices.RdlObjectModel2005
{
	internal class EmptyBorderColor2005 : BorderColor2005
	{
		public new ReportExpression<ReportColor> Default
		{
			get
			{
				return base.Default;
			}
			set
			{
				base.Default = value;
			}
		}

		public EmptyBorderColor2005()
		{
		}

		public EmptyBorderColor2005(IPropertyStore propertyStore)
			: base(propertyStore)
		{
		}

		public override void Initialize()
		{
			Default = Constants.DefaultEmptyColor;
		}
	}
}
