using Microsoft.ReportingServices.RdlObjectModel;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.RdlObjectModel2005
{
	internal class SubtotalStyle2005 : Style2005
	{
		private bool m_initialize;

		private Dictionary<string, bool> m_definedPropertiesOnInitialize = new Dictionary<string, bool>();

		public SubtotalStyle2005()
		{
		}

		public SubtotalStyle2005(IPropertyStore propertyStore)
			: base(propertyStore)
		{
		}

		public override void Initialize()
		{
			m_initialize = true;
			base.Initialize();
			m_initialize = false;
		}

		internal override void OnSetObject(int propertyIndex)
		{
			base.OnSetObject(propertyIndex);
			Style.Definition.Properties properties = (Style.Definition.Properties)propertyIndex;
			string key = properties.ToString();
			if (m_initialize)
			{
				m_definedPropertiesOnInitialize[key] = true;
			}
			else if (m_definedPropertiesOnInitialize.ContainsKey(key))
			{
				m_definedPropertiesOnInitialize.Remove(key);
			}
		}

		internal bool IsPropertyDefinedOnInitialize(string propertyName)
		{
			return m_definedPropertiesOnInitialize.ContainsKey(propertyName);
		}
	}
}
