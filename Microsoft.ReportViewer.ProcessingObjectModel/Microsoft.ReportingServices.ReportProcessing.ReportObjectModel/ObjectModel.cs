using System;

namespace Microsoft.ReportingServices.ReportProcessing.ReportObjectModel
{
	internal abstract class ObjectModel : MarshalByRefObject
	{
		public abstract Fields Fields
		{
			get;
		}

		public abstract Parameters Parameters
		{
			get;
		}

		public abstract Globals Globals
		{
			get;
		}

		public abstract User User
		{
			get;
		}

		public abstract ReportItems ReportItems
		{
			get;
		}

		public abstract Aggregates Aggregates
		{
			get;
		}

		public abstract DataSets DataSets
		{
			get;
		}

		public abstract DataSources DataSources
		{
			get;
		}

		public abstract bool InScope(string scope);

		public abstract int RecursiveLevel(string scope);
	}
}
