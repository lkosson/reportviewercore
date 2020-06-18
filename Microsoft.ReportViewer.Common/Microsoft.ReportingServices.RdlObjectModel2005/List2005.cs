using Microsoft.ReportingServices.RdlObjectModel;
using Microsoft.ReportingServices.RdlObjectModel.Serialization;
using Microsoft.ReportingServices.RdlObjectModel2005.Upgrade;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Serialization;

namespace Microsoft.ReportingServices.RdlObjectModel2005
{
	internal class List2005 : Tablix, IReportItem2005, IUpgradeable, IPageBreakLocation2005
	{
		internal new class Definition : DefinitionStore<List2005, Definition.Properties>
		{
			public enum Properties
			{
				Action = 36,
				PageBreakAtStart,
				PageBreakAtEnd,
				Grouping,
				Sorting,
				ReportItems,
				DataInstanceName,
				DataInstanceElementOutput,
				PropertyCount
			}

			private Definition()
			{
			}
		}

		public Group Grouping
		{
			get
			{
				return (Group)base.PropertyStore.GetObject(39);
			}
			set
			{
				base.PropertyStore.SetObject(39, value);
			}
		}

		[XmlElement(typeof(RdlCollection<SortExpression>))]
		public IList<SortExpression> Sorting
		{
			get
			{
				return (IList<SortExpression>)base.PropertyStore.GetObject(40);
			}
			set
			{
				base.PropertyStore.SetObject(40, value);
			}
		}

		[XmlElement(typeof(RdlCollection<ReportItem>))]
		public IList<ReportItem> ReportItems
		{
			get
			{
				return (IList<ReportItem>)base.PropertyStore.GetObject(41);
			}
			set
			{
				base.PropertyStore.SetObject(41, value);
			}
		}

		public string DataInstanceName
		{
			get
			{
				return (string)base.PropertyStore.GetObject(42);
			}
			set
			{
				base.PropertyStore.SetObject(42, value);
			}
		}

		[DefaultValue(DataElementOutputTypes.Output)]
		[ValidEnumValues(typeof(Constants2005), "List2005DataInstanceElementOutputTypes")]
		public DataElementOutputTypes DataInstanceElementOutput
		{
			get
			{
				return (DataElementOutputTypes)base.PropertyStore.GetInteger(43);
			}
			set
			{
				((EnumProperty)DefinitionStore<List2005, Definition.Properties>.GetProperty(43)).Validate(this, (int)value);
				base.PropertyStore.SetInteger(43, (int)value);
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

		public List2005()
		{
		}

		public List2005(IPropertyStore propertyStore)
			: base(propertyStore)
		{
		}

		public override void Initialize()
		{
			base.Initialize();
			DataInstanceElementOutput = DataElementOutputTypes.Output;
		}

		public void Upgrade(UpgradeImpl2005 upgrader)
		{
			upgrader.UpgradeList(this);
		}
	}
}
