using Microsoft.ReportingServices.RdlObjectModel.Serialization;

namespace Microsoft.ReportingServices.RdlObjectModel
{
	internal class Style : ReportObject, IShouldSerialize
	{
		internal class Definition : DefinitionStore<Style, Definition.Properties>
		{
			internal enum Properties
			{
				Border,
				TopBorder,
				BottomBorder,
				LeftBorder,
				RightBorder,
				BackgroundColor,
				BackgroundGradientType,
				BackgroundGradientEndColor,
				BackgroundImage,
				FontStyle,
				FontFamily,
				FontSize,
				FontWeight,
				Format,
				TextDecoration,
				TextAlign,
				VerticalAlign,
				Color,
				PaddingLeft,
				PaddingRight,
				PaddingTop,
				PaddingBottom,
				LineHeight,
				Direction,
				WritingMode,
				Language,
				UnicodeBiDi,
				Calendar,
				NumeralLanguage,
				NumeralVariant,
				TextEffect,
				BackgroundHatchType,
				ShadowColor,
				ShadowOffset,
				PropertyCount
			}

			private Definition()
			{
			}
		}

		public Border Border
		{
			get
			{
				return (Border)base.PropertyStore.GetObject(0);
			}
			set
			{
				if (value != null)
				{
					if (value.Color == ReportColor.Empty)
					{
						value.Color = Constants.DefaultBorderColor;
					}
					if (value.Width == ReportSize.Empty)
					{
						value.Width = Constants.DefaultBorderWidth;
					}
				}
				base.PropertyStore.SetObject(0, value);
			}
		}

		public Border TopBorder
		{
			get
			{
				return (Border)base.PropertyStore.GetObject(1);
			}
			set
			{
				base.PropertyStore.SetObject(1, value);
			}
		}

		public Border BottomBorder
		{
			get
			{
				return (Border)base.PropertyStore.GetObject(2);
			}
			set
			{
				base.PropertyStore.SetObject(2, value);
			}
		}

		public Border LeftBorder
		{
			get
			{
				return (Border)base.PropertyStore.GetObject(3);
			}
			set
			{
				base.PropertyStore.SetObject(3, value);
			}
		}

		public Border RightBorder
		{
			get
			{
				return (Border)base.PropertyStore.GetObject(4);
			}
			set
			{
				base.PropertyStore.SetObject(4, value);
			}
		}

		[ReportExpressionDefaultValue(typeof(ReportColor))]
		public ReportExpression<ReportColor> BackgroundColor
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<ReportColor>>(5);
			}
			set
			{
				base.PropertyStore.SetObject(5, value);
			}
		}

		[ReportExpressionDefaultValue(typeof(BackgroundGradients), BackgroundGradients.Default)]
		public ReportExpression<BackgroundGradients> BackgroundGradientType
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<BackgroundGradients>>(6);
			}
			set
			{
				base.PropertyStore.SetObject(6, value);
			}
		}

		[ReportExpressionDefaultValue(typeof(ReportColor))]
		public ReportExpression<ReportColor> BackgroundGradientEndColor
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<ReportColor>>(7);
			}
			set
			{
				base.PropertyStore.SetObject(7, value);
			}
		}

		public BackgroundImage BackgroundImage
		{
			get
			{
				return (BackgroundImage)base.PropertyStore.GetObject(8);
			}
			set
			{
				base.PropertyStore.SetObject(8, value);
			}
		}

		[ReportExpressionDefaultValue(typeof(FontStyles), FontStyles.Default)]
		public ReportExpression<FontStyles> FontStyle
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

		[ReportExpressionDefaultValue("Arial")]
		public ReportExpression FontFamily
		{
			get
			{
				if (base.PropertyStore.ContainsObject(10))
				{
					return base.PropertyStore.GetObject<ReportExpression>(10);
				}
				Report ancestor = GetAncestor<Report>();
				if (ancestor != null)
				{
					return ancestor.DefaultFontFamily ?? "Arial";
				}
				return "Arial";
			}
			set
			{
				base.PropertyStore.SetObject(10, value);
			}
		}

		[ReportExpressionDefaultValueConstant(typeof(ReportSize), "DefaultFontSize")]
		public ReportExpression<ReportSize> FontSize
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<ReportSize>>(11);
			}
			set
			{
				base.PropertyStore.SetObject(11, value);
			}
		}

		[ReportExpressionDefaultValue(typeof(FontWeights), FontWeights.Default)]
		public ReportExpression<FontWeights> FontWeight
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<FontWeights>>(12);
			}
			set
			{
				base.PropertyStore.SetObject(12, value);
			}
		}

		[ReportExpressionDefaultValue]
		public ReportExpression Format
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression>(13);
			}
			set
			{
				base.PropertyStore.SetObject(13, value);
			}
		}

		[ReportExpressionDefaultValue(typeof(TextDecorations), TextDecorations.Default)]
		public ReportExpression<TextDecorations> TextDecoration
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

		[ReportExpressionDefaultValue(typeof(TextAlignments), TextAlignments.Default)]
		public ReportExpression<TextAlignments> TextAlign
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

		[ReportExpressionDefaultValue(typeof(VerticalAlignments), VerticalAlignments.Default)]
		public ReportExpression<VerticalAlignments> VerticalAlign
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

		[ReportExpressionDefaultValueConstant(typeof(ReportColor), "DefaultColor")]
		public ReportExpression<ReportColor> Color
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<ReportColor>>(17);
			}
			set
			{
				base.PropertyStore.SetObject(17, value);
			}
		}

		[ReportExpressionDefaultValueConstant(typeof(ReportSize), "DefaultZeroSize")]
		public ReportExpression<ReportSize> PaddingLeft
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<ReportSize>>(18);
			}
			set
			{
				base.PropertyStore.SetObject(18, value);
			}
		}

		[ReportExpressionDefaultValueConstant(typeof(ReportSize), "DefaultZeroSize")]
		public ReportExpression<ReportSize> PaddingRight
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<ReportSize>>(19);
			}
			set
			{
				base.PropertyStore.SetObject(19, value);
			}
		}

		[ReportExpressionDefaultValueConstant(typeof(ReportSize), "DefaultZeroSize")]
		public ReportExpression<ReportSize> PaddingTop
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<ReportSize>>(20);
			}
			set
			{
				base.PropertyStore.SetObject(20, value);
			}
		}

		[ReportExpressionDefaultValueConstant(typeof(ReportSize), "DefaultZeroSize")]
		public ReportExpression<ReportSize> PaddingBottom
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<ReportSize>>(21);
			}
			set
			{
				base.PropertyStore.SetObject(21, value);
			}
		}

		[ReportExpressionDefaultValue(typeof(ReportSize))]
		public ReportExpression<ReportSize> LineHeight
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<ReportSize>>(22);
			}
			set
			{
				base.PropertyStore.SetObject(22, value);
			}
		}

		[ReportExpressionDefaultValue(typeof(TextDirections), TextDirections.Default)]
		public ReportExpression<TextDirections> Direction
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

		[ReportExpressionDefaultValue(typeof(WritingModes), WritingModes.Default)]
		public ReportExpression<WritingModes> WritingMode
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<WritingModes>>(24);
			}
			set
			{
				base.PropertyStore.SetObject(24, value);
			}
		}

		[ReportExpressionDefaultValue]
		public ReportExpression Language
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression>(25);
			}
			set
			{
				base.PropertyStore.SetObject(25, value);
			}
		}

		[ReportExpressionDefaultValue(typeof(UnicodeBiDiTypes), UnicodeBiDiTypes.Normal)]
		public ReportExpression<UnicodeBiDiTypes> UnicodeBiDi
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<UnicodeBiDiTypes>>(26);
			}
			set
			{
				base.PropertyStore.SetObject(26, value);
			}
		}

		[ReportExpressionDefaultValue(typeof(Calendars), Calendars.Default)]
		public ReportExpression<Calendars> Calendar
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<Calendars>>(27);
			}
			set
			{
				base.PropertyStore.SetObject(27, value);
			}
		}

		[ReportExpressionDefaultValue]
		public ReportExpression NumeralLanguage
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression>(28);
			}
			set
			{
				base.PropertyStore.SetObject(28, value);
			}
		}

		[ReportExpressionDefaultValue(typeof(int), 1)]
		[ValidValues(1, 7)]
		public ReportExpression<int> NumeralVariant
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<int>>(29);
			}
			set
			{
				DefinitionStore<Style, Definition.Properties>.GetProperty(29).Validate(this, value);
				base.PropertyStore.SetObject(29, value);
			}
		}

		[ReportExpressionDefaultValue(typeof(TextEffects), TextEffects.Default)]
		public ReportExpression<TextEffects> TextEffect
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<TextEffects>>(30);
			}
			set
			{
				base.PropertyStore.SetObject(30, value);
			}
		}

		[ReportExpressionDefaultValue(typeof(BackgroundHatchTypes), BackgroundHatchTypes.Default)]
		public ReportExpression<BackgroundHatchTypes> BackgroundHatchType
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<BackgroundHatchTypes>>(31);
			}
			set
			{
				base.PropertyStore.SetObject(31, value);
			}
		}

		[ReportExpressionDefaultValue(typeof(ReportColor))]
		public ReportExpression<ReportColor> ShadowColor
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<ReportColor>>(32);
			}
			set
			{
				base.PropertyStore.SetObject(32, value);
			}
		}

		[ReportExpressionDefaultValue(typeof(ReportSize))]
		public ReportExpression<ReportSize> ShadowOffset
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<ReportSize>>(33);
			}
			set
			{
				base.PropertyStore.SetObject(33, value);
			}
		}

		public Style()
		{
		}

		public override void Initialize()
		{
			base.Initialize();
			FontSize = Constants.DefaultFontSize;
			Color = Constants.DefaultColor;
			NumeralVariant = 1;
		}

		internal Style(IPropertyStore propertyStore)
			: base(propertyStore)
		{
		}

		bool IShouldSerialize.ShouldSerializeThis()
		{
			return true;
		}

		SerializationMethod IShouldSerialize.ShouldSerializeProperty(string property)
		{
			if (property == "FontFamily" && !FontFamily.IsExpression)
			{
				Report ancestor = GetAncestor<Report>();
				string b = "Arial";
				if (ancestor != null)
				{
					b = (ancestor.DefaultFontFamily ?? "Arial");
				}
				if (FontFamily.Value == b)
				{
					return SerializationMethod.Never;
				}
				return SerializationMethod.Always;
			}
			return SerializationMethod.Auto;
		}
	}
}
