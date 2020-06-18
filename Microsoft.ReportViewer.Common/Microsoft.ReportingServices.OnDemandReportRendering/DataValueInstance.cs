namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal abstract class DataValueInstance : BaseInstance
	{
		public abstract string Name
		{
			get;
		}

		public abstract object Value
		{
			get;
		}

		internal DataValueInstance(IReportScope repotScope)
			: base(repotScope)
		{
		}
	}
}
