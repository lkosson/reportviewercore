using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Microsoft.Reporting.WinForms
{
	internal sealed class ParameterControlCollection : Dictionary<string, ParameterControl>
	{
		private string m_hiddenParameterThatNeedsValue;

		private string m_anyParameterThatNeedsValue;

		private bool m_visibleParameterNeedsValue;

		public string HiddenParameterThatNeedsValue => m_hiddenParameterThatNeedsValue;

		public string AnyParameterThatNeedsValue => m_anyParameterThatNeedsValue;

		public bool VisibleParameterNeedsValue => m_visibleParameterNeedsValue;

		public bool HasLayout
		{
			get;
			private set;
		}

		public int NumberOfCols
		{
			get;
			private set;
		}

		public int NumberOfRows
		{
			get;
			private set;
		}

		public void Populate(ReportParameterInfoCollection reportParameters, bool isQueryExecutionAllowed, ToolTip tooltip, Font font, ParametersPaneLayout layout = null)
		{
			GridLayoutCellDefinitionCollection gridLayoutCellDefinitionCollection = null;
			Clear();
			m_hiddenParameterThatNeedsValue = null;
			m_anyParameterThatNeedsValue = null;
			m_visibleParameterNeedsValue = false;
			HasLayout = (layout != null);
			if (HasLayout)
			{
				NumberOfCols = layout.GridLayoutDefinition.NumberOfColumns;
				NumberOfRows = layout.GridLayoutDefinition.NumberOfRows;
				gridLayoutCellDefinitionCollection = layout.GridLayoutDefinition.CellDefinitions;
			}
			foreach (ReportParameterInfo reportParameter in reportParameters)
			{
				if (!ShouldDisplayParameter(reportParameter))
				{
					if (reportParameter.State != 0 && m_hiddenParameterThatNeedsValue == null)
					{
						m_hiddenParameterThatNeedsValue = reportParameter.Name;
					}
				}
				else
				{
					GridLayoutCellDefinition cellDefinition = null;
					if (HasLayout)
					{
						cellDefinition = gridLayoutCellDefinitionCollection.GetByName(reportParameter.Name);
					}
					ParameterControl value = ParameterControl.Create(reportParameter, tooltip, font, isQueryExecutionAllowed, cellDefinition);
					Add(reportParameter.Name, value);
					if (reportParameter.State != 0)
					{
						m_visibleParameterNeedsValue = true;
					}
				}
				if (reportParameter.State != 0 && m_anyParameterThatNeedsValue == null)
				{
					m_anyParameterThatNeedsValue = reportParameter.Name;
				}
			}
		}

		private static bool ShouldDisplayParameter(ReportParameterInfo p)
		{
			if (p.PromptUser && p.Prompt != null && p.Prompt.Length > 0)
			{
				return p.Visible;
			}
			return false;
		}
	}
}
