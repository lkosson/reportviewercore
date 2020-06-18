using System.Collections.Generic;
using System.Xml.Serialization;

namespace Microsoft.ReportingServices.RdlObjectModel
{
	internal class ChartDerivedSeries : ReportObject
	{
		internal class Definition : DefinitionStore<ChartDerivedSeries, Definition.Properties>
		{
			internal enum Properties
			{
				ChartSeries,
				SourceChartSeriesName,
				DerivedSeriesFormula,
				ChartFormulaParameters,
				PropertyCount
			}

			private Definition()
			{
			}
		}

		public ChartSeries ChartSeries
		{
			get
			{
				return (ChartSeries)base.PropertyStore.GetObject(0);
			}
			set
			{
				base.PropertyStore.SetObject(0, value);
			}
		}

		public string SourceChartSeriesName
		{
			get
			{
				return (string)base.PropertyStore.GetObject(1);
			}
			set
			{
				base.PropertyStore.SetObject(1, value);
			}
		}

		public ChartFormulas DerivedSeriesFormula
		{
			get
			{
				return (ChartFormulas)base.PropertyStore.GetInteger(2);
			}
			set
			{
				base.PropertyStore.SetInteger(2, (int)value);
			}
		}

		[XmlElement(typeof(RdlCollection<ChartFormulaParameter>))]
		public IList<ChartFormulaParameter> ChartFormulaParameters
		{
			get
			{
				return (IList<ChartFormulaParameter>)base.PropertyStore.GetObject(3);
			}
			set
			{
				base.PropertyStore.SetObject(3, value);
			}
		}

		public ChartDerivedSeries()
		{
		}

		internal ChartDerivedSeries(IPropertyStore propertyStore)
			: base(propertyStore)
		{
		}

		public override void Initialize()
		{
			base.Initialize();
			ChartSeries = new ChartSeries();
			ChartFormulaParameters = new RdlCollection<ChartFormulaParameter>();
		}
	}
}
