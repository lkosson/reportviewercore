using Microsoft.ReportingServices.RdlObjectModel;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Microsoft.ReportingServices.RdlObjectModel2005
{
	internal class DataPoint2005 : ChartDataPoint
	{
		internal new class Definition : DefinitionStore<DataPoint2005, Definition.Properties>
		{
			public enum Properties
			{
				Action = 11,
				DataLabel,
				PropertyCount,
				Style,
				Marker,
				DataValues
			}

			private Definition()
			{
			}
		}

		[XmlElement(typeof(RdlCollection<DataValue2005>))]
		[XmlArrayItem("DataValue", typeof(DataValue2005))]
		public IList<DataValue2005> DataValues
		{
			get
			{
				return (IList<DataValue2005>)base.PropertyStore.GetObject(16);
			}
			set
			{
				base.PropertyStore.SetObject(16, value);
			}
		}

		public DataLabel2005 DataLabel
		{
			get
			{
				return (DataLabel2005)base.PropertyStore.GetObject(12);
			}
			set
			{
				base.PropertyStore.SetObject(12, value);
			}
		}

		public Action Action
		{
			get
			{
				return (Action)base.PropertyStore.GetObject(11);
			}
			set
			{
				base.PropertyStore.SetObject(11, value);
			}
		}

		public new EmptyColorStyle2005 Style
		{
			get
			{
				return (EmptyColorStyle2005)base.PropertyStore.GetObject(14);
			}
			set
			{
				base.PropertyStore.SetObject(14, value);
			}
		}

		public Marker2005 Marker
		{
			get
			{
				return (Marker2005)base.PropertyStore.GetObject(15);
			}
			set
			{
				base.PropertyStore.SetObject(15, value);
			}
		}

		public DataPoint2005()
		{
		}

		public DataPoint2005(IPropertyStore propertyStore)
			: base(propertyStore)
		{
		}

		public override void Initialize()
		{
			base.Initialize();
			base.DataElementOutput = DataElementOutputTypes.Output;
		}
	}
}
