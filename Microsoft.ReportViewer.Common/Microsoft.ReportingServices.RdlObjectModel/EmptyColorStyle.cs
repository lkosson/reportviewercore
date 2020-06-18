using Microsoft.ReportingServices.RdlObjectModel.Serialization;

namespace Microsoft.ReportingServices.RdlObjectModel
{
	[XmlElementClass("Style")]
	internal class EmptyColorStyle : Style
	{
		public new EmptyBorder Border
		{
			get
			{
				return (EmptyBorder)base.Border;
			}
			set
			{
				if (value != null && value.Color == ReportColor.Empty)
				{
					value.Color = Constants.DefaultEmptyColor;
				}
				base.PropertyStore.SetObject(0, value);
			}
		}

		[ReportExpressionDefaultValue(typeof(ReportColor))]
		public new ReportExpression<ReportColor> Color
		{
			get
			{
				return base.Color;
			}
			set
			{
				base.Color = value;
			}
		}

		public EmptyColorStyle()
		{
		}

		public override void Initialize()
		{
			base.Initialize();
			Color = Constants.DefaultEmptyColor;
		}

		internal EmptyColorStyle(IPropertyStore propertyStore)
			: base(propertyStore)
		{
		}
	}
}
