namespace Microsoft.ReportingServices.RdlObjectModel
{
	internal class ChartNoMoveDirections : ReportObject
	{
		internal class Definition : DefinitionStore<ChartNoMoveDirections, Definition.Properties>
		{
			internal enum Properties
			{
				Up,
				Left,
				Right,
				Down,
				UpLeft,
				UpRight,
				DownLeft,
				DownRight,
				PropertyCount
			}

			private Definition()
			{
			}
		}

		[ReportExpressionDefaultValue(typeof(bool), false)]
		public ReportExpression<bool> Up
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

		[ReportExpressionDefaultValue(typeof(bool), false)]
		public ReportExpression<bool> Left
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<bool>>(1);
			}
			set
			{
				base.PropertyStore.SetObject(1, value);
			}
		}

		[ReportExpressionDefaultValue(typeof(bool), false)]
		public ReportExpression<bool> Right
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<bool>>(2);
			}
			set
			{
				base.PropertyStore.SetObject(2, value);
			}
		}

		[ReportExpressionDefaultValue(typeof(bool), false)]
		public ReportExpression<bool> Down
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<bool>>(3);
			}
			set
			{
				base.PropertyStore.SetObject(3, value);
			}
		}

		[ReportExpressionDefaultValue(typeof(bool), false)]
		public ReportExpression<bool> UpLeft
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<bool>>(4);
			}
			set
			{
				base.PropertyStore.SetObject(4, value);
			}
		}

		[ReportExpressionDefaultValue(typeof(bool), false)]
		public ReportExpression<bool> UpRight
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<bool>>(5);
			}
			set
			{
				base.PropertyStore.SetObject(5, value);
			}
		}

		[ReportExpressionDefaultValue(typeof(bool), false)]
		public ReportExpression<bool> DownLeft
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<bool>>(6);
			}
			set
			{
				base.PropertyStore.SetObject(6, value);
			}
		}

		[ReportExpressionDefaultValue(typeof(bool), false)]
		public ReportExpression<bool> DownRight
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<bool>>(7);
			}
			set
			{
				base.PropertyStore.SetObject(7, value);
			}
		}

		public ChartNoMoveDirections()
		{
		}

		internal ChartNoMoveDirections(IPropertyStore propertyStore)
			: base(propertyStore)
		{
		}
	}
}
