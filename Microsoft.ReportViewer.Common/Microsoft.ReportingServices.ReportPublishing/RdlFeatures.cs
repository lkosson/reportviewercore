using System;

namespace Microsoft.ReportingServices.ReportPublishing
{
	[Flags]
	internal enum RdlFeatures
	{
		SharedDataSetReferences = 0x0,
		Image_Embedded = 0x1,
		Sort_Group_Applied = 0x2,
		Sort_DataRegion = 0x3,
		Filters = 0x4,
		Lookup = 0x5,
		RunningValue = 0x6,
		Previous = 0x7,
		RowNumber = 0x8,
		GroupParent = 0x9,
		Variables = 0xA,
		SubReports = 0xB,
		AutomaticSubtotals = 0xC,
		DomainScope = 0xD,
		InScope = 0xE,
		Level = 0xF,
		CreateDrillthroughContext = 0x10,
		UserSort = 0x11,
		AggregatesOfAggregates = 0x12,
		PageHeaderFooter = 0x13,
		SortGroupExpression_OnlySimpleField = 0x14,
		PeerGroups = 0x15,
		ImageTag = 0x16,
		ReportSectionName = 0x17,
		DeferredSort = 0x18,
		EmbeddingMode = 0x19,
		EmbeddingMode_Inline = 0x1A,
		ReportSection_LayoutDirection = 0x1B,
		ThemeFonts = 0x1C,
		TablixHierarchy_EnableDrilldown = 0x1D,
		ScopesCollection = 0x1E,
		ThemeColors = 0x1F,
		ChartHierarchy_EnableDrilldown = 0x20,
		Report_Code = 0x21,
		Report_Classes = 0x22,
		Report_CodeModules = 0x23,
		ComplexExpression = 0x24,
		BackgroundImageFitting = 0x25,
		BackgroundImageTransparency = 0x26,
		LabelData_KeyFields = 0x27,
		ImageTagsCollection = 0x28,
		CellLevelFormatting = 0x29,
		ParametersLayout = 0x2A,
		DefaultFontFamily = 0x2B
	}
}
