using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Microsoft.ReportingServices.RdlObjectModel
{
	internal class ChartData : DataRegionBody
	{
		internal class Definition : DefinitionStore<ChartData, Definition.Properties>
		{
			internal enum Properties
			{
				ChartSeriesCollection,
				ChartDerivedSeriesCollection,
				PropertyCount
			}

			private Definition()
			{
			}
		}

		[XmlElement(typeof(RdlCollection<ChartSeries>))]
		public IList<ChartSeries> ChartSeriesCollection
		{
			get
			{
				return (IList<ChartSeries>)base.PropertyStore.GetObject(0);
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}
				base.PropertyStore.SetObject(0, value);
			}
		}

		[XmlElement(typeof(RdlCollection<ChartDerivedSeries>))]
		public IList<ChartDerivedSeries> ChartDerivedSeriesCollection
		{
			get
			{
				return (IList<ChartDerivedSeries>)base.PropertyStore.GetObject(1);
			}
			set
			{
				base.PropertyStore.SetObject(1, value);
			}
		}

		public ChartData()
		{
		}

		internal ChartData(IPropertyStore propertyStore)
			: base(propertyStore)
		{
		}

		public override void Initialize()
		{
			base.Initialize();
			ChartSeriesCollection = new RdlCollection<ChartSeries>();
			ChartDerivedSeriesCollection = new RdlCollection<ChartDerivedSeries>();
		}
	}
}
