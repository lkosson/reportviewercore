using System.ComponentModel;
using System.Xml.Serialization;

namespace Microsoft.ReportingServices.RdlObjectModel
{
	internal class Variable : ReportObject, INamedObject
	{
		internal class Definition : DefinitionStore<Variable, Definition.Properties>
		{
			internal enum Properties
			{
				Name,
				Value,
				Writable
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

		public ReportExpression Value
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

		[DefaultValue(false)]
		public bool Writable
		{
			get
			{
				return base.PropertyStore.GetBoolean(2);
			}
			set
			{
				base.PropertyStore.SetBoolean(2, value);
			}
		}

		public Variable()
		{
		}

		internal Variable(IPropertyStore propertyStore)
			: base(propertyStore)
		{
		}
	}
}
