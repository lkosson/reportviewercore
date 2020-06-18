using System.Collections.Generic;

namespace Microsoft.ReportingServices.RdlObjectModel
{
	internal class BaseGaugeImage : ReportObject
	{
		internal class Definition : DefinitionStore<BaseGaugeImage, Definition.Properties>
		{
			internal enum Properties
			{
				Source,
				Value,
				MIMEType,
				TransparentColor,
				PropertyCount
			}

			private Definition()
			{
			}
		}

		public ReportExpression<SourceType> Source
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<SourceType>>(0);
			}
			set
			{
				base.PropertyStore.SetObject(0, value);
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

		[ReportExpressionDefaultValue(typeof(ReportColor))]
		public ReportExpression<ReportColor> TransparentColor
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<ReportColor>>(3);
			}
			set
			{
				base.PropertyStore.SetObject(3, value);
			}
		}

		public BaseGaugeImage()
		{
		}

		internal BaseGaugeImage(IPropertyStore propertyStore)
			: base(propertyStore)
		{
		}

		protected override void GetDependenciesCore(IList<ReportObject> dependencies)
		{
			base.GetDependenciesCore(dependencies);
			Image.GetEmbeddedImgDependencies(GetAncestor<Report>(), dependencies, Source.Value, Value);
		}
	}
}
