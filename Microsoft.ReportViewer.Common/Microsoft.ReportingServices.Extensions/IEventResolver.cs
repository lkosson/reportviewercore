namespace Microsoft.ReportingServices.Extensions
{
	internal interface IEventResolver
	{
		IEventHandler ResolveEvent(string eventType);

		void ItemPlacedInEventQueue();
	}
}
