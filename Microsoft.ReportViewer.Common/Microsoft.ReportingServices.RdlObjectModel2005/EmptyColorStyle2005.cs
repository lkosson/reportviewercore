using Microsoft.ReportingServices.RdlObjectModel;
using Microsoft.ReportingServices.RdlObjectModel2005.Upgrade;

namespace Microsoft.ReportingServices.RdlObjectModel2005
{
	internal class EmptyColorStyle2005 : EmptyColorStyle, IUpgradeable
	{
		internal new class Definition : DefinitionStore<EmptyColorStyle, Definition.Properties>
		{
			public enum Properties
			{
				BorderColor = 34,
				BorderStyle,
				BorderWidth,
				FontWeight,
				WritingMode,
				UnicodeBiDi,
				Calendar,
				PropertyCount
			}

			private Definition()
			{
			}
		}

		public EmptyBorderColor2005 BorderColor
		{
			get
			{
				return (EmptyBorderColor2005)base.PropertyStore.GetObject(34);
			}
			set
			{
				base.PropertyStore.SetObject(34, value);
			}
		}

		public BorderStyle2005 BorderStyle
		{
			get
			{
				return (BorderStyle2005)base.PropertyStore.GetObject(35);
			}
			set
			{
				base.PropertyStore.SetObject(35, value);
			}
		}

		public BorderWidth2005 BorderWidth
		{
			get
			{
				return (BorderWidth2005)base.PropertyStore.GetObject(36);
			}
			set
			{
				base.PropertyStore.SetObject(36, value);
			}
		}

		public new ReportExpression<ReportEnum<FontWeight2005>> FontWeight
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<ReportEnum<FontWeight2005>>>(37);
			}
			set
			{
				base.PropertyStore.SetObject(37, value);
			}
		}

		public new ReportExpression<ReportEnum<WritingMode2005>> WritingMode
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<ReportEnum<WritingMode2005>>>(38);
			}
			set
			{
				base.PropertyStore.SetObject(38, value);
			}
		}

		public new ReportExpression<ReportEnum<UnicodeBiDi2005>> UnicodeBiDi
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<ReportEnum<UnicodeBiDi2005>>>(39);
			}
			set
			{
				base.PropertyStore.SetObject(39, value);
			}
		}

		public new ReportExpression<ReportEnum<Calendar2005>> Calendar
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<ReportEnum<Calendar2005>>>(40);
			}
			set
			{
				base.PropertyStore.SetObject(40, value);
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

		public EmptyColorStyle2005()
		{
		}

		public EmptyColorStyle2005(IPropertyStore propertyStore)
			: base(propertyStore)
		{
		}

		public override void Initialize()
		{
			base.Initialize();
			Color = Constants.DefaultEmptyColor;
		}

		public virtual void Upgrade(UpgradeImpl2005 upgrader)
		{
			upgrader.UpgradeEmptyColorStyle(this);
		}
	}
}
