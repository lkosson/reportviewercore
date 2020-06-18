using System.Collections.Generic;

namespace Microsoft.ReportingServices.RdlObjectModel
{
	internal class MapMarkerImage : ReportObject
	{
		internal class Definition : DefinitionStore<MapMarkerImage, Definition.Properties>
		{
			internal enum Properties
			{
				Source,
				Value,
				MIMEType,
				TransparentColor,
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

		[ReportExpressionDefaultValue(typeof(MapResizeModes), MapResizeModes.AutoFit)]
		public ReportExpression<MapResizeModes> ResizeMode
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<MapResizeModes>>(4);
			}
			set
			{
				base.PropertyStore.SetObject(4, value);
			}
		}

		public MapMarkerImage()
		{
		}

		internal MapMarkerImage(IPropertyStore propertyStore)
			: base(propertyStore)
		{
		}

		public override void Initialize()
		{
			base.Initialize();
			ResizeMode = MapResizeModes.AutoFit;
		}

		protected override void GetDependenciesCore(IList<ReportObject> dependencies)
		{
			base.GetDependenciesCore(dependencies);
			Image.GetEmbeddedImgDependencies(GetAncestor<Report>(), dependencies, Source.Value, Value);
		}
	}
}
