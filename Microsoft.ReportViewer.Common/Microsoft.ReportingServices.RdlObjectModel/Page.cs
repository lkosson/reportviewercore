using System.ComponentModel;

namespace Microsoft.ReportingServices.RdlObjectModel
{
	internal class Page : ReportObject
	{
		internal class Definition : DefinitionStore<Page, Definition.Properties>
		{
			internal enum Properties
			{
				PageHeader,
				PageFooter,
				PageHeight,
				PageWidth,
				InteractiveHeight,
				InteractiveWidth,
				LeftMargin,
				RightMargin,
				TopMargin,
				BottomMargin,
				Columns,
				ColumnSpacing,
				Style
			}

			private Definition()
			{
			}
		}

		public PageSection PageHeader
		{
			get
			{
				return (PageSection)base.PropertyStore.GetObject(0);
			}
			set
			{
				base.PropertyStore.SetObject(0, value);
			}
		}

		public PageSection PageFooter
		{
			get
			{
				return (PageSection)base.PropertyStore.GetObject(1);
			}
			set
			{
				base.PropertyStore.SetObject(1, value);
			}
		}

		[DefaultValueConstant("DefaultPageHeight")]
		public ReportSize PageHeight
		{
			get
			{
				return base.PropertyStore.GetSize(2);
			}
			set
			{
				base.PropertyStore.SetSize(2, value);
			}
		}

		[DefaultValueConstant("DefaultPageWidth")]
		public ReportSize PageWidth
		{
			get
			{
				return base.PropertyStore.GetSize(3);
			}
			set
			{
				base.PropertyStore.SetSize(3, value);
			}
		}

		public ReportSize InteractiveHeight
		{
			get
			{
				return base.PropertyStore.GetSize(4);
			}
			set
			{
				base.PropertyStore.SetSize(4, value);
			}
		}

		public ReportSize InteractiveWidth
		{
			get
			{
				return base.PropertyStore.GetSize(5);
			}
			set
			{
				base.PropertyStore.SetSize(5, value);
			}
		}

		[DefaultValueConstant("DefaultZeroSize")]
		public ReportSize LeftMargin
		{
			get
			{
				return base.PropertyStore.GetSize(6);
			}
			set
			{
				base.PropertyStore.SetSize(6, value);
			}
		}

		[DefaultValueConstant("DefaultZeroSize")]
		public ReportSize RightMargin
		{
			get
			{
				return base.PropertyStore.GetSize(7);
			}
			set
			{
				base.PropertyStore.SetSize(7, value);
			}
		}

		[DefaultValueConstant("DefaultZeroSize")]
		public ReportSize TopMargin
		{
			get
			{
				return base.PropertyStore.GetSize(8);
			}
			set
			{
				base.PropertyStore.SetSize(8, value);
			}
		}

		[DefaultValueConstant("DefaultZeroSize")]
		public ReportSize BottomMargin
		{
			get
			{
				return base.PropertyStore.GetSize(9);
			}
			set
			{
				base.PropertyStore.SetSize(9, value);
			}
		}

		[DefaultValue(1)]
		[ValidValues(1, 100)]
		public int Columns
		{
			get
			{
				return base.PropertyStore.GetInteger(10);
			}
			set
			{
				((IntProperty)DefinitionStore<Page, Definition.Properties>.GetProperty(10)).Validate(this, value);
				base.PropertyStore.SetInteger(10, value);
			}
		}

		[DefaultValueConstant("DefaultColumnSpacing")]
		public ReportSize ColumnSpacing
		{
			get
			{
				return base.PropertyStore.GetSize(11);
			}
			set
			{
				base.PropertyStore.SetSize(11, value);
			}
		}

		public Style Style
		{
			get
			{
				return (Style)base.PropertyStore.GetObject(12);
			}
			set
			{
				base.PropertyStore.SetObject(12, value);
			}
		}

		public Page()
		{
		}

		internal Page(IPropertyStore propertyStore)
			: base(propertyStore)
		{
		}

		public override void Initialize()
		{
			base.Initialize();
			Columns = 1;
			PageHeight = Constants.DefaultPageHeight;
			PageWidth = Constants.DefaultPageWidth;
			ColumnSpacing = Constants.DefaultColumnSpacing;
		}
	}
}
