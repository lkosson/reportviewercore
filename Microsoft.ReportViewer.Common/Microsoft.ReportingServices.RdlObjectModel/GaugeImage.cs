using System.Collections.Generic;

namespace Microsoft.ReportingServices.RdlObjectModel
{
	internal class GaugeImage : GaugePanelItem
	{
		internal new class Definition : DefinitionStore<GaugeImage, Definition.Properties>
		{
			internal enum Properties
			{
				Name,
				Style,
				Top,
				Left,
				Height,
				Width,
				ZIndex,
				Hidden,
				ToolTip,
				ActionInfo,
				ParentItem,
				Source,
				Value,
				MIMEType,
				TransparentColor,
				Angle,
				Transparency,
				ResizeMode,
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
				return base.PropertyStore.GetObject<ReportExpression<SourceType>>(11);
			}
			set
			{
				base.PropertyStore.SetObject(11, value);
			}
		}

		public ReportExpression Value
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression>(12);
			}
			set
			{
				base.PropertyStore.SetObject(12, value);
			}
		}

		[ReportExpressionDefaultValue]
		public ReportExpression MIMEType
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

		[ReportExpressionDefaultValue(typeof(ReportColor))]
		public ReportExpression<ReportColor> TransparentColor
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<ReportColor>>(14);
			}
			set
			{
				base.PropertyStore.SetObject(14, value);
			}
		}

		[ReportExpressionDefaultValue(typeof(double), 0.0)]
		public ReportExpression<double> Angle
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<double>>(15);
			}
			set
			{
				base.PropertyStore.SetObject(15, value);
			}
		}

		[ReportExpressionDefaultValue(typeof(double), 0.0)]
		public ReportExpression<double> Transparency
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<double>>(16);
			}
			set
			{
				base.PropertyStore.SetObject(16, value);
			}
		}

		[ReportExpressionDefaultValue(typeof(ResizeModes), ResizeModes.AutoFit)]
		public ReportExpression<ResizeModes> ResizeMode
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<ResizeModes>>(17);
			}
			set
			{
				base.PropertyStore.SetObject(17, value);
			}
		}

		public GaugeImage()
		{
		}

		internal GaugeImage(IPropertyStore propertyStore)
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
