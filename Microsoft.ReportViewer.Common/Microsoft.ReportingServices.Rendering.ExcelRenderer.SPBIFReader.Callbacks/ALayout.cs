using Microsoft.ReportingServices.Rendering.RPLProcessing;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.Rendering.ExcelRenderer.SPBIFReader.Callbacks
{
	internal abstract class ALayout
	{
		internal static readonly int TablixStructGenerationOffset = 100000;

		internal static readonly int TablixStructStart = int.MinValue;

		protected RPLReport m_report;

		internal abstract bool HeaderInBody
		{
			get;
		}

		internal abstract bool FooterInBody
		{
			get;
		}

		internal abstract bool? SummaryRowAfter
		{
			get;
			set;
		}

		internal abstract bool? SummaryColumnAfter
		{
			get;
			set;
		}

		internal RPLReport RPLReport => m_report;

		internal ALayout(RPLReport report)
		{
			m_report = report;
		}

		internal abstract void AddReportItem(object rplSource, int top, int left, int width, int height, int generationIndex, byte state, string subreportLanguage, Dictionary<string, ToggleParent> toggleParents);

		internal abstract void AddStructuralItem(int top, int left, int width, int height, bool isToggglable, int generationIndex, RPLTablixMemberCell member, TogglePosition togglePosition);

		internal abstract void AddStructuralItem(int top, int left, int width, int height, int generationIndex, int rowHeaderWidth, int columnHeaderHeight, bool rtl);

		internal abstract ALayout GetPageHeaderLayout(float width, float height);

		internal abstract ALayout GetPageFooterLayout(float width, float height);

		internal abstract void CompletePage();

		internal abstract void CompleteSection();

		internal abstract void SetIsLastSection(bool isLastSection);
	}
}
