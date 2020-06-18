using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Serialization;

namespace Microsoft.ReportingServices.RdlObjectModel
{
	internal class TablixMember : HierarchyMember, IHierarchyMember
	{
		internal class Definition : DefinitionStore<TablixMember, Definition.Properties>
		{
			internal enum Properties
			{
				Group,
				SortExpressions,
				TablixHeader,
				TablixMembers,
				CustomProperties,
				FixedData,
				Visibility,
				HideIfNoRows,
				KeepWithGroup,
				RepeatOnNewPage,
				DataElementName,
				DataElementOutput,
				KeepTogether
			}

			private Definition()
			{
			}
		}

		public override Group Group
		{
			get
			{
				return (Group)base.PropertyStore.GetObject(0);
			}
			set
			{
				base.PropertyStore.SetObject(0, value);
			}
		}

		[XmlElement(typeof(RdlCollection<SortExpression>))]
		public IList<SortExpression> SortExpressions
		{
			get
			{
				return (IList<SortExpression>)base.PropertyStore.GetObject(1);
			}
			set
			{
				base.PropertyStore.SetObject(1, value);
			}
		}

		public TablixHeader TablixHeader
		{
			get
			{
				return (TablixHeader)base.PropertyStore.GetObject(2);
			}
			set
			{
				base.PropertyStore.SetObject(2, value);
			}
		}

		[XmlElement(typeof(RdlCollection<TablixMember>))]
		public IList<TablixMember> TablixMembers
		{
			get
			{
				return (IList<TablixMember>)base.PropertyStore.GetObject(3);
			}
			set
			{
				base.PropertyStore.SetObject(3, value);
			}
		}

		[XmlElement(typeof(RdlCollection<CustomProperty>))]
		public IList<CustomProperty> CustomProperties
		{
			get
			{
				return (IList<CustomProperty>)base.PropertyStore.GetObject(4);
			}
			set
			{
				base.PropertyStore.SetObject(4, value);
			}
		}

		[DefaultValue(false)]
		public bool FixedData
		{
			get
			{
				return base.PropertyStore.GetBoolean(5);
			}
			set
			{
				base.PropertyStore.SetBoolean(5, value);
			}
		}

		public Visibility Visibility
		{
			get
			{
				return (Visibility)base.PropertyStore.GetObject(6);
			}
			set
			{
				base.PropertyStore.SetObject(6, value);
			}
		}

		[DefaultValue(false)]
		public bool HideIfNoRows
		{
			get
			{
				return base.PropertyStore.GetBoolean(7);
			}
			set
			{
				base.PropertyStore.SetBoolean(7, value);
			}
		}

		[DefaultValue(KeepWithGroupTypes.None)]
		public KeepWithGroupTypes KeepWithGroup
		{
			get
			{
				return (KeepWithGroupTypes)base.PropertyStore.GetInteger(8);
			}
			set
			{
				base.PropertyStore.SetInteger(8, (int)value);
			}
		}

		[DefaultValue(false)]
		public bool RepeatOnNewPage
		{
			get
			{
				return base.PropertyStore.GetBoolean(9);
			}
			set
			{
				base.PropertyStore.SetBoolean(9, value);
			}
		}

		[DefaultValue("")]
		public string DataElementName
		{
			get
			{
				return (string)base.PropertyStore.GetObject(10);
			}
			set
			{
				base.PropertyStore.SetObject(10, value);
			}
		}

		[DefaultValue(DataElementOutputTypes.Auto)]
		[ValidEnumValues("TablixMemberDataElementOutputTypes")]
		public DataElementOutputTypes DataElementOutput
		{
			get
			{
				return (DataElementOutputTypes)base.PropertyStore.GetInteger(11);
			}
			set
			{
				((EnumProperty)DefinitionStore<TablixMember, Definition.Properties>.GetProperty(11)).Validate(this, (int)value);
				base.PropertyStore.SetInteger(11, (int)value);
			}
		}

		[DefaultValue(false)]
		public bool KeepTogether
		{
			get
			{
				return base.PropertyStore.GetBoolean(12);
			}
			set
			{
				base.PropertyStore.SetBoolean(12, value);
			}
		}

		IEnumerable<IHierarchyMember> IHierarchyMember.Members
		{
			get
			{
				foreach (TablixMember tablixMember in TablixMembers)
				{
					yield return tablixMember;
				}
			}
		}

		public TablixMember()
		{
		}

		internal TablixMember(IPropertyStore propertyStore)
			: base(propertyStore)
		{
		}

		public override void Initialize()
		{
			base.Initialize();
			SortExpressions = new RdlCollection<SortExpression>();
			TablixMembers = new RdlCollection<TablixMember>();
			CustomProperties = new RdlCollection<CustomProperty>();
			DataElementOutput = DataElementOutputTypes.Auto;
		}
	}
}
