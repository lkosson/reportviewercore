using Microsoft.ReportingServices.RdlObjectModel;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Serialization;

namespace Microsoft.ReportingServices.RdlObjectModel2005
{
	internal class Subtotal2005 : ReportObject
	{
		internal class Definition : DefinitionStore<Subtotal2005, Definition.Properties>
		{
			public enum Properties
			{
				ReportItems,
				Style,
				Position,
				DataElementName,
				DataElementOutput
			}

			private Definition()
			{
			}
		}

		[XmlElement(typeof(RdlCollection<ReportItem>))]
		public IList<ReportItem> ReportItems
		{
			get
			{
				return (IList<ReportItem>)base.PropertyStore.GetObject(0);
			}
			set
			{
				base.PropertyStore.SetObject(0, value);
			}
		}

		public SubtotalStyle2005 Style
		{
			get
			{
				return (SubtotalStyle2005)base.PropertyStore.GetObject(1);
			}
			set
			{
				base.PropertyStore.SetObject(1, value);
			}
		}

		public SubtotalPositions Position
		{
			get
			{
				return (SubtotalPositions)base.PropertyStore.GetInteger(2);
			}
			set
			{
				base.PropertyStore.SetInteger(2, (int)value);
			}
		}

		[DefaultValue("")]
		public string DataElementName
		{
			get
			{
				return (string)base.PropertyStore.GetObject(3);
			}
			set
			{
				base.PropertyStore.SetObject(3, value);
			}
		}

		[DefaultValue(DataElementOutputTypes.NoOutput)]
		[ValidEnumValues(typeof(Constants2005), "Subtotal2005DataElementOutputTypes")]
		public DataElementOutputTypes DataElementOutput
		{
			get
			{
				return (DataElementOutputTypes)base.PropertyStore.GetInteger(4);
			}
			set
			{
				((EnumProperty)DefinitionStore<Subtotal2005, Definition.Properties>.GetProperty(4)).Validate(this, (int)value);
				base.PropertyStore.SetInteger(4, (int)value);
			}
		}

		public Subtotal2005()
		{
		}

		public Subtotal2005(IPropertyStore propertyStore)
			: base(propertyStore)
		{
		}

		public override void Initialize()
		{
			base.Initialize();
			ReportItems = new RdlCollection<ReportItem>();
			DataElementOutput = DataElementOutputTypes.NoOutput;
		}
	}
}
