using Microsoft.ReportingServices.Diagnostics;
using System;

namespace Microsoft.ReportingServices.Extensions
{
	internal abstract class Subscription
	{
		public abstract Guid ID
		{
			get;
		}

		public abstract UserContext Owner
		{
			get;
		}

		public abstract Guid ItemID
		{
			get;
		}

		public abstract string SubscriptionData
		{
			get;
		}

		public abstract string EventType
		{
			get;
		}

		public abstract string ReportName
		{
			get;
		}
	}
}
