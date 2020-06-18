using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Microsoft.Reporting.WinForms
{
	public sealed class ReportParameterInfo
	{
		private string m_name;

		private ParameterDataType m_dataType;

		private bool m_isNullable;

		private bool m_allowBlank;

		private bool m_isMultiValue;

		private bool m_isQueryParameter;

		private string m_prompt;

		private bool m_promptUser;

		private bool m_areDefaultValuesQueryBased;

		private bool m_areValidValuesQueryBased;

		private string m_errorMessage;

		private IList<ValidValue> m_validValues;

		private IList<string> m_currentValues;

		private ParameterState m_state;

		private ReportParameterInfoCollection m_dependencyCollection;

		private ReportParameterInfoCollection m_dependentsCollection;

		private string[] m_dependencies;

		private List<ReportParameterInfo> m_dependentsCollectionConstruction = new List<ReportParameterInfo>();

		private bool m_visible;

		internal bool HasUnsatisfiedDownstreamParametersWithDefaults
		{
			get
			{
				foreach (ReportParameterInfo dependent in Dependents)
				{
					if (dependent.AreDefaultValuesQueryBased && dependent.State != 0)
					{
						return true;
					}
				}
				return false;
			}
		}

		public string Name => m_name;

		public ParameterDataType DataType => m_dataType;

		public bool Nullable => m_isNullable;

		public bool AllowBlank => m_allowBlank;

		public bool MultiValue => m_isMultiValue;

		public bool IsQueryParameter => m_isQueryParameter;

		public string Prompt => m_prompt;

		public bool PromptUser => m_promptUser;

		public ReportParameterInfoCollection Dependencies => m_dependencyCollection;

		public ReportParameterInfoCollection Dependents
		{
			get
			{
				if (m_dependentsCollection == null)
				{
					m_dependentsCollection = new ReportParameterInfoCollection(m_dependentsCollectionConstruction);
				}
				return m_dependentsCollection;
			}
		}

		public bool AreValidValuesQueryBased => m_areValidValuesQueryBased;

		public IList<ValidValue> ValidValues => m_validValues;

		public bool AreDefaultValuesQueryBased => m_areDefaultValuesQueryBased;

		public IList<string> Values => m_currentValues;

		public ParameterState State => m_state;

		public string ErrorMessage => m_errorMessage;

		public bool Visible
		{
			get
			{
				return m_visible;
			}
			internal set
			{
				m_visible = value;
			}
		}

		internal ReportParameterInfo(string name, ParameterDataType dataType, bool isNullable, bool allowBlank, bool isMultiValue, bool isQueryParameter, string prompt, bool promptUser, bool areDefaultValuesQueryBased, bool areValidValuesQueryBased, string errorMessage, string[] currentValues, IList<ValidValue> validValues, string[] dependencies, ParameterState state)
		{
			m_name = name;
			m_dataType = dataType;
			m_isNullable = isNullable;
			m_allowBlank = allowBlank;
			m_isMultiValue = isMultiValue;
			m_isQueryParameter = isQueryParameter;
			m_prompt = prompt;
			m_promptUser = promptUser;
			m_areDefaultValuesQueryBased = areDefaultValuesQueryBased;
			m_areValidValuesQueryBased = areValidValuesQueryBased;
			m_errorMessage = errorMessage;
			m_currentValues = new ReadOnlyCollection<string>(currentValues ?? new string[0]);
			m_validValues = validValues;
			m_dependencies = dependencies;
			m_state = state;
			m_visible = true;
		}

		internal void SetDependencies(ReportParameterInfoCollection coll)
		{
			if (m_dependencyCollection != null)
			{
				return;
			}
			if (m_dependencies == null)
			{
				m_dependencyCollection = new ReportParameterInfoCollection();
				return;
			}
			List<ReportParameterInfo> list = new List<ReportParameterInfo>();
			string[] dependencies = m_dependencies;
			foreach (string name in dependencies)
			{
				ReportParameterInfo reportParameterInfo = coll[name];
				if (reportParameterInfo != null)
				{
					list.Add(reportParameterInfo);
					reportParameterInfo.m_dependentsCollectionConstruction.Add(this);
				}
			}
			m_dependencyCollection = new ReportParameterInfoCollection(list);
		}
	}
}
