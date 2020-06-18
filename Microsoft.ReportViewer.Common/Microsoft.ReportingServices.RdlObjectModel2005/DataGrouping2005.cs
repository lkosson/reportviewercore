using Microsoft.ReportingServices.RdlObjectModel;
using Microsoft.ReportingServices.RdlObjectModel2005.Upgrade;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Serialization;

namespace Microsoft.ReportingServices.RdlObjectModel2005
{
	internal class DataGrouping2005 : DataMember, IUpgradeable
	{
		public new class Definition : DefinitionStore<DataGrouping2005, Definition.Properties>
		{
			public enum Properties
			{
				Static = 4,
				PropertyCount
			}

			private Definition()
			{
			}
		}

		[DefaultValue(false)]
		public bool Static
		{
			get
			{
				return base.PropertyStore.GetBoolean(4);
			}
			set
			{
				base.PropertyStore.SetBoolean(4, value);
			}
		}

		public Grouping2005 Grouping
		{
			get
			{
				return (Grouping2005)base.Group;
			}
			set
			{
				base.Group = value;
			}
		}

		[XmlElement(typeof(RdlCollection<SortExpression>))]
		public IList<SortExpression> Sorting
		{
			get
			{
				return base.SortExpressions;
			}
			set
			{
				base.SortExpressions = value;
			}
		}

		[XmlElement(typeof(RdlCollection<DataMember>))]
		[XmlArrayItem("DataGrouping", typeof(DataGrouping2005))]
		public IList<DataMember> DataGroupings
		{
			get
			{
				return base.DataMembers;
			}
			set
			{
				base.DataMembers = value;
			}
		}

		public DataGrouping2005()
		{
		}

		public DataGrouping2005(IPropertyStore propertyStore)
			: base(propertyStore)
		{
		}

		public override void Initialize()
		{
			base.Initialize();
			base.CustomProperties = new RdlCollection<CustomProperty>();
			DataGroupings = new RdlCollection<DataMember>();
		}

		public void Upgrade(UpgradeImpl2005 upgrader)
		{
			upgrader.UpgradeDataGrouping(this);
		}
	}
}
