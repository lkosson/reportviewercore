using Microsoft.ReportingServices.RdlObjectModel;
using Microsoft.ReportingServices.RdlObjectModel.Serialization;
using Microsoft.ReportingServices.RdlObjectModel2005.Upgrade;

namespace Microsoft.ReportingServices.RdlObjectModel2005
{
	internal class Textbox2005 : Textbox, IReportItem2005, IUpgradeable
	{
		internal new class Definition : DefinitionStore<Textbox2005, Definition.Properties>
		{
			public enum Properties
			{
				Action = 26,
				Value,
				ValueLocID,
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
				return (Action)base.PropertyStore.GetObject(26);
			}
			set
			{
				base.PropertyStore.SetObject(26, value);
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

		[ReportExpressionDefaultValue("")]
		public ReportExpression Value
		{
			get
			{
				return (ReportExpression)base.PropertyStore.GetObject(27);
			}
			set
			{
				base.PropertyStore.SetObject(27, value);
			}
		}

		[XmlChildAttribute("Value", "LocID", "http://schemas.microsoft.com/SQLServer/reporting/reportdesigner")]
		public string ValueLocID
		{
			get
			{
				return (string)base.PropertyStore.GetObject(28);
			}
			set
			{
				base.PropertyStore.SetObject(28, value);
			}
		}

		public Textbox2005()
		{
		}

		public Textbox2005(IPropertyStore propertyStore)
			: base(propertyStore)
		{
		}

		public void Upgrade(UpgradeImpl2005 upgrader)
		{
			upgrader.UpgradeTextbox(this);
		}
	}
}
