using Microsoft.ReportingServices.RdlObjectModel;
using Microsoft.ReportingServices.RdlObjectModel.Serialization;
using Microsoft.ReportingServices.RdlObjectModel2005.Upgrade;

namespace Microsoft.ReportingServices.RdlObjectModel2005
{
	internal class Line2005 : Line, IReportItem2005, IUpgradeable
	{
		internal new class Definition : DefinitionStore<Line2005, Definition.Properties>
		{
			public enum Properties
			{
				Action = 18,
				PropertyCount
			}

			private Definition()
			{
			}
		}

		public Action Action
		{
			get
			{
				return (Action)base.PropertyStore.GetObject(18);
			}
			set
			{
				base.PropertyStore.SetObject(18, value);
			}
		}

		[ReportExpressionDefaultValue]
		public ReportExpression Label
		{
			get
			{
				return base.DocumentMapLabel;
			}
			set
			{
				base.DocumentMapLabel = value;
			}
		}

		[XmlChildAttribute("Label", "LocID", "http://schemas.microsoft.com/SQLServer/reporting/reportdesigner")]
		public string LabelLocID
		{
			get
			{
				return (string)base.PropertyStore.GetObject(12);
			}
			set
			{
				base.PropertyStore.SetObject(12, value);
			}
		}

		public new Style2005 Style
		{
			get
			{
				return (Style2005)base.Style;
			}
			set
			{
				base.Style = value;
			}
		}

		public Line2005()
		{
		}

		public Line2005(IPropertyStore propertyStore)
			: base(propertyStore)
		{
		}

		public void Upgrade(UpgradeImpl2005 upgrader)
		{
			upgrader.UpgradeReportItem(this);
		}
	}
}
