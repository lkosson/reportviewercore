using Microsoft.Reporting.Chart.WebForms.Borders3D;
using Microsoft.Reporting.Chart.WebForms.ChartTypes;
using Microsoft.Reporting.Chart.WebForms.Data;
using Microsoft.Reporting.Chart.WebForms.Formulas;
using Microsoft.Reporting.Chart.WebForms.Utilities;
using System.ComponentModel.Design;
using System.Globalization;

namespace Microsoft.Reporting.Chart.WebForms
{
	internal class CommonElements
	{
		internal ChartAreaCollection chartAreaCollection;

		internal ChartGraphics graph;

		internal IServiceContainer container;

		internal bool processModePaint = true;

		internal bool processModeRegions;

		private int width;

		private int height;

		internal DataManager DataManager => (DataManager)container.GetService(typeof(DataManager));

		public bool ProcessModePaint => processModePaint;

		public bool ProcessModeRegions => processModeRegions;

		public HotRegionsList HotRegionsList
		{
			get
			{
				return ChartPicture.hotRegionsList;
			}
			set
			{
				ChartPicture.hotRegionsList = value;
			}
		}

		public DataManipulator DataManipulator => ChartPicture.DataManipulator;

		internal ImageLoader ImageLoader => (ImageLoader)container.GetService(typeof(ImageLoader));

		internal Chart Chart => (Chart)container.GetService(typeof(Chart));

		internal EventsManager EventsManager => (EventsManager)container.GetService(typeof(EventsManager));

		internal ChartTypeRegistry ChartTypeRegistry => (ChartTypeRegistry)container.GetService(typeof(ChartTypeRegistry));

		internal BorderTypeRegistry BorderTypeRegistry => (BorderTypeRegistry)container.GetService(typeof(BorderTypeRegistry));

		internal FormulaRegistry FormulaRegistry => (FormulaRegistry)container.GetService(typeof(FormulaRegistry));

		public ChartImage ChartPicture => (ChartImage)container.GetService(typeof(ChartImage));

		internal int Width
		{
			get
			{
				return width;
			}
			set
			{
				width = value;
			}
		}

		internal int Height
		{
			get
			{
				return height;
			}
			set
			{
				height = value;
			}
		}

		private CommonElements()
		{
		}

		public CommonElements(IServiceContainer container)
		{
			this.container = container;
		}

		public void TraceWrite(string category, string message)
		{
			if (container != null)
			{
				((TraceManager)container.GetService(typeof(TraceManager)))?.Write(category, message);
			}
		}

		internal static double ParseDouble(string stringToParse)
		{
			double num = 0.0;
			try
			{
				return double.Parse(stringToParse, CultureInfo.InvariantCulture);
			}
			catch
			{
				return double.Parse(stringToParse, CultureInfo.CurrentCulture);
			}
		}

		internal static float ParseFloat(string stringToParse)
		{
			float num = 0f;
			try
			{
				return float.Parse(stringToParse, CultureInfo.InvariantCulture);
			}
			catch
			{
				return float.Parse(stringToParse, CultureInfo.CurrentCulture);
			}
		}
	}
}
