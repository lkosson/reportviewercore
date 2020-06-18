namespace Microsoft.Reporting.Chart.WebForms
{
	[SRDescription("DescriptionAttributeResizingMode")]
	internal enum ResizingMode
	{
		TopLeftHandle = 0,
		TopHandle = 1,
		TopRightHandle = 2,
		RightHandle = 3,
		BottomRightHandle = 4,
		BottomHandle = 5,
		BottomLeftHandle = 6,
		LeftHandle = 7,
		AnchorHandle = 8,
		Moving = 0x10,
		MovingPathPoints = 0x20,
		None = 0x40
	}
}
