namespace Microsoft.ReportingServices.Rendering.SPBProcessing
{
	internal enum PaginationInfoProperties : byte
	{
		Unknown = 0,
		ItemSizes = 1,
		PaddItemSizes = 2,
		ItemState = 3,
		PageItemsAbove = 4,
		PageItemsLeft = 5,
		ItemsCreated = 6,
		IndexesLeftToRight = 7,
		RepeatWithItems = 8,
		RightEdgeItem = 9,
		Children = 10,
		PrevPageEnd = 11,
		RelativeTop = 12,
		RelativeBottom = 13,
		RelativeTopToBottom = 14,
		DataRegionIndex = 0xF,
		LevelForRepeat = 0x10,
		TablixCreateState = 17,
		MembersInstanceIndex = 18,
		ChildPage = 19,
		IndexesTopToBottom = 20,
		DefLeftValue = 21,
		IgnoreTotalsOnLastLevel = 22,
		SectionIndex = 23,
		Delimiter = byte.MaxValue
	}
}
