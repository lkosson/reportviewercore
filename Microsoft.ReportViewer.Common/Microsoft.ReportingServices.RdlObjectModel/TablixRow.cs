using System.Collections.Generic;
using System.Xml.Serialization;

namespace Microsoft.ReportingServices.RdlObjectModel
{
	internal class TablixRow : ReportObject
	{
		internal class Definition : DefinitionStore<TablixRow, Definition.Properties>
		{
			internal enum Properties
			{
				Height,
				TablixCells
			}

			private Definition()
			{
			}
		}

		public ReportSize Height
		{
			get
			{
				return base.PropertyStore.GetSize(0);
			}
			set
			{
				base.PropertyStore.SetSize(0, value);
			}
		}

		[XmlElement(typeof(RdlCollection<TablixCell>))]
		public IList<TablixCell> TablixCells
		{
			get
			{
				return (IList<TablixCell>)base.PropertyStore.GetObject(1);
			}
			set
			{
				base.PropertyStore.SetObject(1, value);
			}
		}

		public TablixRow()
		{
		}

		internal TablixRow(IPropertyStore propertyStore)
			: base(propertyStore)
		{
		}

		public override void Initialize()
		{
			base.Initialize();
			Height = Constants.DefaultZeroSize;
			TablixCells = new RdlCollection<TablixCell>();
		}
	}
}
