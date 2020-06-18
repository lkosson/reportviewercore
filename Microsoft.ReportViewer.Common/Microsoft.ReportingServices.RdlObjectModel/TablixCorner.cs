using System.Collections.Generic;
using System.Xml.Serialization;

namespace Microsoft.ReportingServices.RdlObjectModel
{
	internal class TablixCorner : ReportObject
	{
		internal class Definition : DefinitionStore<TablixCorner, Definition.Properties>
		{
			internal enum Properties
			{
				TablixCornerRows
			}

			private Definition()
			{
			}
		}

		[XmlElement(typeof(RdlCollection<IList<TablixCornerCell>>))]
		[XmlArrayItem("TablixCornerRow", typeof(TablixCornerRow))]
		public IList<IList<TablixCornerCell>> TablixCornerRows
		{
			get
			{
				return (IList<IList<TablixCornerCell>>)base.PropertyStore.GetObject(0);
			}
			set
			{
				base.PropertyStore.SetObject(0, value);
			}
		}

		public TablixCorner()
		{
		}

		internal TablixCorner(IPropertyStore propertyStore)
			: base(propertyStore)
		{
		}

		public override void Initialize()
		{
			base.Initialize();
			TablixCornerRows = new RdlCollection<IList<TablixCornerCell>>();
		}
	}
}
