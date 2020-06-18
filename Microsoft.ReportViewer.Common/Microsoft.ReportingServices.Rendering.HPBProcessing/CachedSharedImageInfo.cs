namespace Microsoft.ReportingServices.Rendering.HPBProcessing
{
	internal sealed class CachedSharedImageInfo
	{
		private string m_streamName;

		private ItemBoundaries m_itemBoundaries;

		internal string StreamName => m_streamName;

		internal ItemBoundaries ImageBounderies => m_itemBoundaries;

		internal CachedSharedImageInfo(string streamName, ItemBoundaries itemBoundaries)
		{
			m_streamName = streamName;
			m_itemBoundaries = itemBoundaries;
		}
	}
}
