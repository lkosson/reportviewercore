namespace Microsoft.ReportingServices.Interfaces
{
	public abstract class Notification
	{
		public abstract Report Report
		{
			get;
		}

		public abstract string Owner
		{
			get;
		}

		public abstract Setting[] UserData
		{
			get;
		}

		public abstract int Attempt
		{
			get;
		}

		public abstract int MaxNumberOfRetries
		{
			get;
		}

		public abstract string Status
		{
			set;
		}

		public abstract bool Retry
		{
			get;
			set;
		}

		public abstract void Save();
	}
}
