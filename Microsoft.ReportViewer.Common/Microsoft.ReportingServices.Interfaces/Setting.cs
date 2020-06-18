using System.Collections;

namespace Microsoft.ReportingServices.Interfaces
{
	public class Setting
	{
		private string m_name;

		private string m_displayName;

		private string m_value;

		private bool m_required;

		private bool m_readOnly;

		private string m_field;

		private string m_error;

		private bool m_encrypted;

		private bool m_isPassword;

		private ArrayList m_validValues = new ArrayList();

		public string Name
		{
			get
			{
				return m_name;
			}
			set
			{
				m_name = value;
			}
		}

		public string DisplayName
		{
			get
			{
				return m_displayName;
			}
			set
			{
				m_displayName = value;
			}
		}

		public string Value
		{
			get
			{
				return m_value;
			}
			set
			{
				m_value = value;
			}
		}

		public bool Required
		{
			get
			{
				return m_required;
			}
			set
			{
				m_required = value;
			}
		}

		public bool ReadOnly
		{
			get
			{
				return m_readOnly;
			}
			set
			{
				m_readOnly = value;
			}
		}

		public string Field
		{
			get
			{
				return m_field;
			}
			set
			{
				m_field = value;
			}
		}

		public string Error
		{
			get
			{
				return m_error;
			}
			set
			{
				m_error = value;
			}
		}

		public ValidValue[] ValidValues
		{
			get
			{
				return m_validValues.ToArray(typeof(ValidValue)) as ValidValue[];
			}
			set
			{
				if (value == null)
				{
					m_validValues = new ArrayList();
				}
				else
				{
					m_validValues = new ArrayList(value);
				}
			}
		}

		public bool Encrypted
		{
			get
			{
				return m_encrypted;
			}
			set
			{
				m_encrypted = value;
			}
		}

		public bool IsPassword
		{
			get
			{
				return m_isPassword;
			}
			set
			{
				m_isPassword = value;
			}
		}

		public void AddValidValue(ValidValue val)
		{
			m_validValues.Add(val);
		}

		public void AddValidValue(string label, string val)
		{
			ValidValue validValue = new ValidValue();
			validValue.Value = val;
			validValue.Label = label;
			AddValidValue(validValue);
		}
	}
}
