namespace Microsoft.ReportingServices.Rendering.ImageRenderer
{
	internal sealed class CMapMappingRange
	{
		internal readonly CMapMapping Mapping;

		internal readonly ushort Length;

		internal CMapMappingRange(CMapMapping mapping, ushort length)
		{
			Mapping = mapping;
			Length = length;
		}
	}
}
