using System.Collections.Generic;
using System.Xml.Serialization;

namespace Microsoft.ReportingServices.RdlObjectModel
{
	internal class TablixBody : DataRegionBody
	{
		internal class Definition : DefinitionStore<TablixBody, Definition.Properties>
		{
			internal enum Properties
			{
				TablixColumns,
				TablixRows
			}

			private Definition()
			{
			}
		}

		[XmlElement(typeof(RdlCollection<TablixColumn>))]
		public IList<TablixColumn> TablixColumns
		{
			get
			{
				return (IList<TablixColumn>)base.PropertyStore.GetObject(0);
			}
			set
			{
				base.PropertyStore.SetObject(0, value);
			}
		}

		[XmlElement(typeof(RdlCollection<TablixRow>))]
		public IList<TablixRow> TablixRows
		{
			get
			{
				return (IList<TablixRow>)base.PropertyStore.GetObject(1);
			}
			set
			{
				base.PropertyStore.SetObject(1, value);
			}
		}

		public TablixBody()
		{
		}

		internal TablixBody(IPropertyStore propertyStore)
			: base(propertyStore)
		{
		}

		public override void Initialize()
		{
			base.Initialize();
			TablixColumns = new RdlCollection<TablixColumn>();
			TablixRows = new RdlCollection<TablixRow>();
		}
	}
}
