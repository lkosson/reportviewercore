using System.Collections.Generic;
using System.Xml.Serialization;

namespace Microsoft.ReportingServices.RdlObjectModel
{
	internal class GridLayoutDefinition : ReportObject
	{
		internal class Definition : DefinitionStore<GridLayoutDefinition, Definition.Properties>
		{
			internal enum Properties
			{
				NumberOfColumns,
				NumberOfRows,
				CellDefinitions
			}

			private Definition()
			{
			}
		}

		public const int MaxNumberOfRows = 10000;

		public const int MaxNumberOfColumns = 8;

		public const int MaxNumberOfConsecutiveEmptyRows = 20;

		public int NumberOfColumns
		{
			get
			{
				return base.PropertyStore.GetInteger(0);
			}
			set
			{
				base.PropertyStore.SetInteger(0, value);
			}
		}

		public int NumberOfRows
		{
			get
			{
				return base.PropertyStore.GetInteger(1);
			}
			set
			{
				base.PropertyStore.SetInteger(1, value);
			}
		}

		[XmlElement(typeof(RdlCollection<CellDefinition>))]
		public IList<CellDefinition> CellDefinitions
		{
			get
			{
				return base.PropertyStore.GetObject<RdlCollection<CellDefinition>>(2);
			}
			set
			{
				base.PropertyStore.SetObject(2, value);
			}
		}

		public GridLayoutDefinition()
		{
			CellDefinitions = new RdlCollection<CellDefinition>();
			NumberOfColumns = 4;
			NumberOfRows = 2;
		}

		internal GridLayoutDefinition(IPropertyStore propertyStore)
			: base(propertyStore)
		{
		}
	}
}
