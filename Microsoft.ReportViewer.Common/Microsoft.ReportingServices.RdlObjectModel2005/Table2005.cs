using Microsoft.ReportingServices.RdlObjectModel;
using Microsoft.ReportingServices.RdlObjectModel.Serialization;
using Microsoft.ReportingServices.RdlObjectModel2005.Upgrade;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Serialization;

namespace Microsoft.ReportingServices.RdlObjectModel2005
{
	internal class Table2005 : Tablix, IReportItem2005, IUpgradeable, IPageBreakLocation2005
	{
		public new class Definition : DefinitionStore<Table2005, Definition.Properties>
		{
			public enum Properties
			{
				Action = 36,
				PageBreakAtStart,
				PageBreakAtEnd,
				TableColumns,
				Header,
				TableGroups,
				Details,
				Footer,
				DetailDataElementName,
				DetailDataCollectionName,
				DetailDataElementOutput
			}

			private Definition()
			{
			}
		}

		[XmlElement(typeof(RdlCollection<TableColumn2005>))]
		[XmlArrayItem("TableColumn", typeof(TableColumn2005))]
		public IList<TableColumn2005> TableColumns
		{
			get
			{
				return (IList<TableColumn2005>)base.PropertyStore.GetObject(39);
			}
			set
			{
				base.PropertyStore.SetObject(39, value);
			}
		}

		public Header2005 Header
		{
			get
			{
				return (Header2005)base.PropertyStore.GetObject(40);
			}
			set
			{
				base.PropertyStore.SetObject(40, value);
			}
		}

		[XmlElement(typeof(RdlCollection<TableGroup2005>))]
		[XmlArrayItem("TableGroup", typeof(TableGroup2005))]
		public IList<TableGroup2005> TableGroups
		{
			get
			{
				return (IList<TableGroup2005>)base.PropertyStore.GetObject(41);
			}
			set
			{
				base.PropertyStore.SetObject(41, value);
			}
		}

		public Details2005 Details
		{
			get
			{
				return (Details2005)base.PropertyStore.GetObject(42);
			}
			set
			{
				base.PropertyStore.SetObject(42, value);
			}
		}

		public Footer2005 Footer
		{
			get
			{
				return (Footer2005)base.PropertyStore.GetObject(43);
			}
			set
			{
				base.PropertyStore.SetObject(43, value);
			}
		}

		[DefaultValue("")]
		public string DetailDataElementName
		{
			get
			{
				return (string)base.PropertyStore.GetObject(44);
			}
			set
			{
				base.PropertyStore.SetObject(44, value);
			}
		}

		[DefaultValue("")]
		public string DetailDataCollectionName
		{
			get
			{
				return (string)base.PropertyStore.GetObject(45);
			}
			set
			{
				base.PropertyStore.SetObject(45, value);
			}
		}

		[DefaultValue(DataElementOutputTypes.Output)]
		[ValidEnumValues(typeof(Constants2005), "Table2005DetailDataElementOutputTypes")]
		public DataElementOutputTypes DetailDataElementOutput
		{
			get
			{
				return (DataElementOutputTypes)base.PropertyStore.GetInteger(46);
			}
			set
			{
				base.PropertyStore.SetInteger(46, (int)value);
			}
		}

		[DefaultValue(false)]
		public bool PageBreakAtStart
		{
			get
			{
				return base.PropertyStore.GetBoolean(37);
			}
			set
			{
				base.PropertyStore.SetBoolean(37, value);
			}
		}

		[ReportExpressionDefaultValue]
		public ReportExpression NoRows
		{
			get
			{
				return base.NoRowsMessage;
			}
			set
			{
				base.NoRowsMessage = value;
			}
		}

		[DefaultValue(false)]
		public bool PageBreakAtEnd
		{
			get
			{
				return base.PropertyStore.GetBoolean(38);
			}
			set
			{
				base.PropertyStore.SetBoolean(38, value);
			}
		}

		public Action Action
		{
			get
			{
				return (Action)base.PropertyStore.GetObject(36);
			}
			set
			{
				base.PropertyStore.SetObject(36, value);
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

		public Table2005()
		{
		}

		public Table2005(IPropertyStore propertyStore)
			: base(propertyStore)
		{
		}

		public override void Initialize()
		{
			base.Initialize();
			TableColumns = new RdlCollection<TableColumn2005>();
			TableGroups = new RdlCollection<TableGroup2005>();
			DetailDataElementOutput = DataElementOutputTypes.Output;
		}

		public void Upgrade(UpgradeImpl2005 upgrader)
		{
			upgrader.UpgradeTable(this);
		}
	}
}
