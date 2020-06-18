using System.Collections.Generic;

namespace Microsoft.ReportingServices.RdlObjectModel
{
	internal class BackgroundImage : ReportObject
	{
		internal class Definition : DefinitionStore<BackgroundImage, Definition.Properties>
		{
			internal enum Properties
			{
				Source,
				Value,
				MIMEType,
				BackgroundRepeat,
				TransparentColor,
				Position,
				PropertyCount
			}

			private Definition()
			{
			}
		}

		public SourceType Source
		{
			get
			{
				return (SourceType)base.PropertyStore.GetInteger(0);
			}
			set
			{
				base.PropertyStore.SetInteger(0, (int)value);
			}
		}

		public ReportExpression Value
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression>(1);
			}
			set
			{
				base.PropertyStore.SetObject(1, value);
			}
		}

		[ReportExpressionDefaultValue]
		public ReportExpression MIMEType
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression>(2);
			}
			set
			{
				base.PropertyStore.SetObject(2, value);
			}
		}

		[ReportExpressionDefaultValue(typeof(BackgroundRepeatTypes), BackgroundRepeatTypes.Default)]
		public ReportExpression<BackgroundRepeatTypes> BackgroundRepeat
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<BackgroundRepeatTypes>>(3);
			}
			set
			{
				base.PropertyStore.SetObject(3, value);
			}
		}

		[ReportExpressionDefaultValue(typeof(ReportColor))]
		public ReportExpression<ReportColor> TransparentColor
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<ReportColor>>(4);
			}
			set
			{
				base.PropertyStore.SetObject(4, value);
			}
		}

		[ReportExpressionDefaultValue(typeof(BackgroundPositions), BackgroundPositions.Default)]
		public ReportExpression<BackgroundPositions> Position
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<BackgroundPositions>>(5);
			}
			set
			{
				base.PropertyStore.SetObject(5, value);
			}
		}

		public BackgroundImage()
		{
		}

		internal BackgroundImage(IPropertyStore propertyStore)
			: base(propertyStore)
		{
		}

		protected override void GetDependenciesCore(IList<ReportObject> dependencies)
		{
			base.GetDependenciesCore(dependencies);
			if (Source != SourceType.Embedded)
			{
				return;
			}
			Report ancestor = GetAncestor<Report>();
			if (ancestor != null && !Value.IsExpression)
			{
				EmbeddedImage embeddedImageByName = ancestor.GetEmbeddedImageByName(Value.Expression);
				if (embeddedImageByName != null && !dependencies.Contains(embeddedImageByName))
				{
					dependencies.Add(embeddedImageByName);
				}
			}
		}
	}
}
