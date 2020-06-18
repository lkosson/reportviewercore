using Microsoft.ReportingServices.Rendering.RichText;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.Rendering.HPBProcessing
{
	internal class SectionItemizedData
	{
		internal Dictionary<string, List<TextRunItemizedData>> HeaderFooter;

		private List<Dictionary<string, List<TextRunItemizedData>>> m_columns;

		internal List<Dictionary<string, List<TextRunItemizedData>>> Columns
		{
			get
			{
				if (m_columns == null)
				{
					m_columns = new List<Dictionary<string, List<TextRunItemizedData>>>();
				}
				return m_columns;
			}
		}
	}
}
