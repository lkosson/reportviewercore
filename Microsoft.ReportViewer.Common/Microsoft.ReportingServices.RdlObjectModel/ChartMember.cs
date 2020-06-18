using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Serialization;

namespace Microsoft.ReportingServices.RdlObjectModel
{
	internal class ChartMember : HierarchyMember, IHierarchyMember
	{
		internal class Definition : DefinitionStore<ChartMember, Definition.Properties>
		{
			internal enum Properties
			{
				Group,
				SortExpressions,
				ChartMembers,
				Label,
				LabelLocID,
				CustomProperties,
				DataElementName,
				DataElementOutput,
				PropertyCount
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

		[XmlElement(typeof(RdlCollection<ChartMember>))]
		public IList<ChartMember> ChartMembers
		{
			get
			{
				return (IList<ChartMember>)base.PropertyStore.GetObject(2);
			}
			set
			{
				base.PropertyStore.SetObject(2, value);
			}
		}

		public ReportExpression Label
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression>(3);
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
				return (IList<CustomProperty>)base.PropertyStore.GetObject(5);
			}
			set
			{
				base.PropertyStore.SetObject(5, value);
			}
		}

		[DefaultValue("")]
		public string DataElementName
		{
			get
			{
				return (string)base.PropertyStore.GetObject(6);
			}
			set
			{
				base.PropertyStore.SetObject(6, value);
			}
		}

		[DefaultValue(DataElementOutputTypes.Auto)]
		[ValidEnumValues("ChartMemberDataElementOutputTypes")]
		public DataElementOutputTypes DataElementOutput
		{
			get
			{
				return (DataElementOutputTypes)base.PropertyStore.GetInteger(7);
			}
			set
			{
				((EnumProperty)DefinitionStore<ChartMember, Definition.Properties>.GetProperty(7)).Validate(this, (int)value);
				base.PropertyStore.SetInteger(7, (int)value);
			}
		}

		IEnumerable<IHierarchyMember> IHierarchyMember.Members
		{
			get
			{
				foreach (ChartMember chartMember in ChartMembers)
				{
					yield return chartMember;
				}
			}
		}

		public ChartMember()
		{
		}

		internal ChartMember(IPropertyStore propertyStore)
			: base(propertyStore)
		{
		}

		public override void Initialize()
		{
			base.Initialize();
			SortExpressions = new RdlCollection<SortExpression>();
			ChartMembers = new RdlCollection<ChartMember>();
			CustomProperties = new RdlCollection<CustomProperty>();
		}
	}
}
