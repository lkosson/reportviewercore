using System;
using System.Collections;
using System.Text.RegularExpressions;

namespace Microsoft.ReportingServices.ReportPublishing
{
	internal abstract class NameValidator
	{
		protected const int MAX_NAME_LENGTH = 256;

		protected const string MAX_NAME_LENGTH_STRING = "256";

		private const string m_identifierStart = "\\p{Lu}\\p{Ll}\\p{Lt}\\p{Lm}\\p{Lo}\\p{Nl}";

		private const string m_identifierExtend = "\\p{Mn}\\p{Mc}\\p{Nd}\\p{Pc}\\p{Cf}";

		private static Regex m_clsIdentifierRegex = new Regex("^[\\p{Lu}\\p{Ll}\\p{Lt}\\p{Lm}\\p{Lo}\\p{Nl}][\\p{Lu}\\p{Ll}\\p{Lt}\\p{Lm}\\p{Lo}\\p{Nl}\\p{Mn}\\p{Mc}\\p{Nd}\\p{Pc}\\p{Cf}]*$", RegexOptions.ExplicitCapture | RegexOptions.Compiled | RegexOptions.Singleline);

		protected Hashtable m_dictionary = new Hashtable();

		private Hashtable m_dictionaryCaseInsensitive;

		protected NameValidator(bool caseInsensitiveComparison)
		{
			if (caseInsensitiveComparison)
			{
				m_dictionaryCaseInsensitive = new Hashtable(StringComparer.OrdinalIgnoreCase);
			}
		}

		protected static bool IsCLSCompliant(string name)
		{
			return m_clsIdentifierRegex.Match(name).Success;
		}

		protected bool IsUnique(string name)
		{
			if (m_dictionary.ContainsKey(name))
			{
				return false;
			}
			m_dictionary.Add(name, null);
			return true;
		}

		protected bool IsCaseInsensitiveDuplicate(string name)
		{
			if (m_dictionaryCaseInsensitive == null)
			{
				return false;
			}
			if (m_dictionaryCaseInsensitive.ContainsKey(name))
			{
				return true;
			}
			m_dictionaryCaseInsensitive.Add(name, null);
			return false;
		}

		internal virtual bool Validate(string name)
		{
			if (IsCLSCompliant(name))
			{
				return IsUnique(name);
			}
			return false;
		}
	}
}
