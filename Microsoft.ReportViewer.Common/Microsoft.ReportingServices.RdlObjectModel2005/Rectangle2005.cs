using Microsoft.ReportingServices.RdlObjectModel;
using Microsoft.ReportingServices.RdlObjectModel.Serialization;
using Microsoft.ReportingServices.RdlObjectModel2005.Upgrade;
using System.ComponentModel;

namespace Microsoft.ReportingServices.RdlObjectModel2005
{
	internal class Rectangle2005 : Rectangle, IReportItem2005, IUpgradeable, IPageBreakLocation2005
	{
		internal new class Definition : DefinitionStore<Rectangle2005, Definition.Properties>
		{
			public enum Properties
			{
				Action = 24,
				PageBreakAtStart,
				PageBreakAtEnd,
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
				return (Action)base.PropertyStore.GetObject(24);
			}
			set
			{
				base.PropertyStore.SetObject(24, value);
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

		[DefaultValue(false)]
		public bool PageBreakAtStart
		{
			get
			{
				return base.PropertyStore.GetBoolean(25);
			}
			set
			{
				base.PropertyStore.SetBoolean(25, value);
			}
		}

		[DefaultValue(false)]
		public bool PageBreakAtEnd
		{
			get
			{
				return base.PropertyStore.GetBoolean(26);
			}
			set
			{
				base.PropertyStore.SetBoolean(26, value);
			}
		}

		public Rectangle2005()
		{
		}

		public Rectangle2005(IPropertyStore propertyStore)
			: base(propertyStore)
		{
		}

		public void Upgrade(UpgradeImpl2005 upgrader)
		{
			upgrader.UpgradeRectangle(this);
		}
	}
}
