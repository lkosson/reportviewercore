using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Serialization;

namespace Microsoft.ReportingServices.RdlObjectModel
{
	internal class Group : ReportObject, IGlobalNamedObject, INamedObject
	{
		internal class Definition : DefinitionStore<Group, Definition.Properties>
		{
			internal enum Properties
			{
				Name,
				DocumentMapLabel,
				DocumentMapLabelLocID,
				GroupExpressions,
				ReGroupExpressions,
				PageBreak,
				Filters,
				Parent,
				DataElementName,
				DataElementOutput,
				Variables,
				PageName,
				DomainScope,
				PropertyCount
			}

			private Definition()
			{
			}
		}

		[XmlAttribute(typeof(string))]
		public string Name
		{
			get
			{
				return (string)base.PropertyStore.GetObject(0);
			}
			set
			{
				base.PropertyStore.SetObject(0, value);
			}
		}

		[ReportExpressionDefaultValue]
		public ReportExpression DocumentMapLabel
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression>(1);
			}
			set
			{
				base.PropertyStore.SetObject(1, value);
			}
		}

		[XmlElement(typeof(RdlCollection<ReportExpression>))]
		[XmlArrayItem("GroupExpression", typeof(ReportExpression))]
		public IList<ReportExpression> GroupExpressions
		{
			get
			{
				return (IList<ReportExpression>)base.PropertyStore.GetObject(3);
			}
			set
			{
				base.PropertyStore.SetObject(3, value);
			}
		}

		[XmlElement(typeof(RdlCollection<ReportExpression>))]
		[XmlArrayItem("GroupExpression", typeof(ReportExpression))]
		public IList<ReportExpression> ReGroupExpressions
		{
			get
			{
				return (IList<ReportExpression>)base.PropertyStore.GetObject(4);
			}
			set
			{
				base.PropertyStore.SetObject(4, value);
			}
		}

		public PageBreak PageBreak
		{
			get
			{
				return (PageBreak)base.PropertyStore.GetObject(5);
			}
			set
			{
				base.PropertyStore.SetObject(5, value);
			}
		}

		[ReportExpressionDefaultValue]
		public ReportExpression PageName
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression>(11);
			}
			set
			{
				base.PropertyStore.SetObject(11, value);
			}
		}

		[XmlElement(typeof(RdlCollection<Filter>))]
		public IList<Filter> Filters
		{
			get
			{
				return (IList<Filter>)base.PropertyStore.GetObject(6);
			}
			set
			{
				base.PropertyStore.SetObject(6, value);
			}
		}

		[ReportExpressionDefaultValue]
		public new ReportExpression Parent
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression>(7);
			}
			set
			{
				base.PropertyStore.SetObject(7, value);
			}
		}

		[DefaultValue("")]
		public string DataElementName
		{
			get
			{
				return (string)base.PropertyStore.GetObject(8);
			}
			set
			{
				base.PropertyStore.SetObject(8, value);
			}
		}

		[DefaultValue(DataElementOutputTypes.Output)]
		[ValidEnumValues("GroupDataElementOutputTypes")]
		public DataElementOutputTypes DataElementOutput
		{
			get
			{
				return (DataElementOutputTypes)base.PropertyStore.GetInteger(9);
			}
			set
			{
				((EnumProperty)DefinitionStore<Group, Definition.Properties>.GetProperty(9)).Validate(this, (int)value);
				base.PropertyStore.SetInteger(9, (int)value);
			}
		}

		[XmlElement(typeof(RdlCollection<Variable>))]
		public IList<Variable> Variables
		{
			get
			{
				return (IList<Variable>)base.PropertyStore.GetObject(10);
			}
			set
			{
				base.PropertyStore.SetObject(10, value);
			}
		}

		[DefaultValue("")]
		public string DomainScope
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

		public Group()
		{
		}

		internal Group(IPropertyStore propertyStore)
			: base(propertyStore)
		{
		}

		public override void Initialize()
		{
			base.Initialize();
			GroupExpressions = new RdlCollection<ReportExpression>();
			ReGroupExpressions = new RdlCollection<ReportExpression>();
			Filters = new RdlCollection<Filter>();
			DataElementOutput = DataElementOutputTypes.Output;
			Variables = new RdlCollection<Variable>();
		}
	}
}
