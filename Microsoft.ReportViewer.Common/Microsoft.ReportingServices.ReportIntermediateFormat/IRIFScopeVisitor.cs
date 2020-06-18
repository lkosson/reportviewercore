namespace Microsoft.ReportingServices.ReportIntermediateFormat
{
	internal interface IRIFScopeVisitor
	{
		void PreVisit(DataRegion dataRegion);

		void PostVisit(DataRegion dataRegion);

		void PreVisit(ReportHierarchyNode member);

		void PostVisit(ReportHierarchyNode member);

		void PreVisit(Cell cell, int rowIndex, int colIndex);

		void PostVisit(Cell cell, int rowIndex, int colIndex);
	}
}
