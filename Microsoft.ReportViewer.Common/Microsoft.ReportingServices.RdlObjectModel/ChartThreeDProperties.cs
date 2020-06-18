namespace Microsoft.ReportingServices.RdlObjectModel
{
	internal class ChartThreeDProperties : ReportObject
	{
		internal class Definition : DefinitionStore<ChartThreeDProperties, Definition.Properties>
		{
			internal enum Properties
			{
				Enabled,
				ProjectionMode,
				Perspective,
				Rotation,
				Inclination,
				DepthRatio,
				Shading,
				GapDepth,
				WallThickness,
				Clustered,
				PropertyCount
			}

			private Definition()
			{
			}
		}

		[ReportExpressionDefaultValue(typeof(bool), false)]
		public ReportExpression<bool> Enabled
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<bool>>(0);
			}
			set
			{
				base.PropertyStore.SetObject(0, value);
			}
		}

		[ReportExpressionDefaultValue(typeof(ChartProjectionModes), ChartProjectionModes.Oblique)]
		public ReportExpression<ChartProjectionModes> ProjectionMode
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<ChartProjectionModes>>(1);
			}
			set
			{
				base.PropertyStore.SetObject(1, value);
			}
		}

		[ReportExpressionDefaultValue(typeof(int), 0)]
		public ReportExpression<int> Perspective
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<int>>(2);
			}
			set
			{
				base.PropertyStore.SetObject(2, value);
			}
		}

		[ReportExpressionDefaultValue(typeof(int), 30)]
		public ReportExpression<int> Rotation
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<int>>(3);
			}
			set
			{
				base.PropertyStore.SetObject(3, value);
			}
		}

		[ReportExpressionDefaultValue(typeof(int), 30)]
		public ReportExpression<int> Inclination
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<int>>(4);
			}
			set
			{
				base.PropertyStore.SetObject(4, value);
			}
		}

		[ReportExpressionDefaultValue(typeof(int), 100)]
		public ReportExpression<int> DepthRatio
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<int>>(5);
			}
			set
			{
				base.PropertyStore.SetObject(5, value);
			}
		}

		[ReportExpressionDefaultValue(typeof(ChartShadings), ChartShadings.Real)]
		public ReportExpression<ChartShadings> Shading
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<ChartShadings>>(6);
			}
			set
			{
				base.PropertyStore.SetObject(6, value);
			}
		}

		[ReportExpressionDefaultValue(typeof(int), 100)]
		public ReportExpression<int> GapDepth
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<int>>(7);
			}
			set
			{
				base.PropertyStore.SetObject(7, value);
			}
		}

		[ReportExpressionDefaultValue(typeof(int), 7)]
		public ReportExpression<int> WallThickness
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<int>>(8);
			}
			set
			{
				base.PropertyStore.SetObject(8, value);
			}
		}

		[ReportExpressionDefaultValue(typeof(bool), false)]
		public ReportExpression<bool> Clustered
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<bool>>(9);
			}
			set
			{
				base.PropertyStore.SetObject(9, value);
			}
		}

		public ChartThreeDProperties()
		{
		}

		internal ChartThreeDProperties(IPropertyStore propertyStore)
			: base(propertyStore)
		{
		}

		public override void Initialize()
		{
			base.Initialize();
			Rotation = 30;
			Inclination = 30;
			DepthRatio = 100;
			GapDepth = 100;
			WallThickness = 7;
		}
	}
}
