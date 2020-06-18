using Microsoft.ReportingServices.RdlObjectModel;
using Microsoft.ReportingServices.RdlObjectModel2005.Upgrade;

namespace Microsoft.ReportingServices.RdlObjectModel2005
{
	internal class Style2005 : Style, IUpgradeable
	{
		internal new class Definition : DefinitionStore<Style, Definition.Properties>
		{
			public enum Properties
			{
				BorderColor = 34,
				BorderStyle,
				BorderWidth,
				BackgroundImage,
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

		public BorderColor2005 BorderColor
		{
			get
			{
				return (BorderColor2005)base.PropertyStore.GetObject(34);
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

		public new BackgroundImage2005 BackgroundImage
		{
			get
			{
				return (BackgroundImage2005)base.PropertyStore.GetObject(37);
			}
			set
			{
				base.PropertyStore.SetObject(37, value);
			}
		}

		[ValidEnumValues(typeof(Constants2005), "Style2005FontStyles")]
		public new ReportExpression<FontStyles> FontStyle
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<FontStyles>>(9);
			}
			set
			{
				base.PropertyStore.SetObject(9, value);
			}
		}

		public new ReportExpression<ReportEnum<FontWeight2005>> FontWeight
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<ReportEnum<FontWeight2005>>>(38);
			}
			set
			{
				base.PropertyStore.SetObject(38, value);
			}
		}

		[ValidEnumValues(typeof(Constants2005), "Style2005TextDecorations")]
		public new ReportExpression<TextDecorations> TextDecoration
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<TextDecorations>>(14);
			}
			set
			{
				base.PropertyStore.SetObject(14, value);
			}
		}

		[ValidEnumValues(typeof(Constants2005), "Style2005TextAlignments")]
		public new ReportExpression<TextAlignments> TextAlign
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<TextAlignments>>(15);
			}
			set
			{
				base.PropertyStore.SetObject(15, value);
			}
		}

		[ValidEnumValues(typeof(Constants2005), "Style2005VerticalAlignments")]
		public new ReportExpression<VerticalAlignments> VerticalAlign
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<VerticalAlignments>>(16);
			}
			set
			{
				base.PropertyStore.SetObject(16, value);
			}
		}

		[ValidEnumValues(typeof(Constants2005), "Style2005TextDirections")]
		public new ReportExpression<TextDirections> Direction
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<TextDirections>>(23);
			}
			set
			{
				base.PropertyStore.SetObject(23, value);
			}
		}

		public new ReportExpression<ReportEnum<WritingMode2005>> WritingMode
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<ReportEnum<WritingMode2005>>>(39);
			}
			set
			{
				base.PropertyStore.SetObject(39, value);
			}
		}

		public new ReportExpression<ReportEnum<UnicodeBiDi2005>> UnicodeBiDi
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<ReportEnum<UnicodeBiDi2005>>>(40);
			}
			set
			{
				base.PropertyStore.SetObject(40, value);
			}
		}

		public new ReportExpression<ReportEnum<Calendar2005>> Calendar
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<ReportEnum<Calendar2005>>>(41);
			}
			set
			{
				base.PropertyStore.SetObject(41, value);
			}
		}

		public Style2005()
		{
		}

		public Style2005(IPropertyStore propertyStore)
			: base(propertyStore)
		{
		}

		public virtual void Upgrade(UpgradeImpl2005 upgrader)
		{
			upgrader.UpgradeStyle(this);
		}
	}
}
