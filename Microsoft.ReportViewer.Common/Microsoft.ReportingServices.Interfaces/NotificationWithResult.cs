namespace Microsoft.ReportingServices.Interfaces
{
	public abstract class NotificationWithResult : Notification
	{
		public abstract string SubscriptionResult
		{
			get;
			set;
		}
	}
}
