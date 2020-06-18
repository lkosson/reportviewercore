using Microsoft.ReportingServices.RdlObjectModel;
using Microsoft.ReportingServices.RdlObjectModel.Serialization;
using Microsoft.ReportingServices.RdlObjectModel2005.Upgrade;
using Microsoft.ReportingServices.RdlObjectModel2008;

namespace Microsoft.ReportingServices.RdlObjectModel2005
{
	[XmlElementClass("Report", Namespace = "http://schemas.microsoft.com/sqlserver/reporting/2005/01/reportdefinition")]
	internal class Report2005 : Report2008, IUpgradeable
	{
		internal new class Definition : DefinitionStore<Report2005, Definition.Properties>
		{
			public enum Properties
			{
				PropertyCount = 28
			}

			private Definition()
			{
			}
		}

		public new const string DesignerNamespace = "http://schemas.microsoft.com/SQLServer/reporting/reportdesigner";

		public PageSection PageHeader
		{
			get
			{
				return Page.PageHeader;
			}
			set
			{
				Page.PageHeader = value;
			}
		}

		public PageSection PageFooter
		{
			get
			{
				return Page.PageFooter;
			}
			set
			{
				Page.PageFooter = value;
			}
		}

		public ReportSize PageHeight
		{
			get
			{
				return Page.PageHeight;
			}
			set
			{
				Page.PageHeight = value;
			}
		}

		public ReportSize PageWidth
		{
			get
			{
				return Page.PageWidth;
			}
			set
			{
				Page.PageWidth = value;
			}
		}

		public ReportSize InteractiveHeight
		{
			get
			{
				return Page.InteractiveHeight;
			}
			set
			{
				Page.InteractiveHeight = value;
			}
		}

		public ReportSize InteractiveWidth
		{
			get
			{
				return Page.InteractiveWidth;
			}
			set
			{
				Page.InteractiveWidth = value;
			}
		}

		public ReportSize LeftMargin
		{
			get
			{
				return Page.LeftMargin;
			}
			set
			{
				Page.LeftMargin = value;
			}
		}

		public ReportSize RightMargin
		{
			get
			{
				return Page.RightMargin;
			}
			set
			{
				Page.RightMargin = value;
			}
		}

		public ReportSize TopMargin
		{
			get
			{
				return Page.TopMargin;
			}
			set
			{
				Page.TopMargin = value;
			}
		}

		public ReportSize BottomMargin
		{
			get
			{
				return Page.BottomMargin;
			}
			set
			{
				Page.BottomMargin = value;
			}
		}

		public new DataElementStyles2005 DataElementStyle
		{
			get
			{
				return (DataElementStyles2005)base.DataElementStyle;
			}
			set
			{
				base.DataElementStyle = (DataElementStyles)value;
			}
		}

		public Report2005()
		{
		}

		public Report2005(IPropertyStore propertyStore)
			: base(propertyStore)
		{
		}

		public void Upgrade(UpgradeImpl2005 upgrader)
		{
			upgrader.UpgradeReport(this);
		}
	}
}
