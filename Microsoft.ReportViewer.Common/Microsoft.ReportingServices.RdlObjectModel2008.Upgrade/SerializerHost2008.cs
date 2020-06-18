using Microsoft.ReportingServices.RdlObjectModel;
using Microsoft.ReportingServices.RdlObjectModel.Serialization;
using Microsoft.ReportingServices.RdlObjectModel2010.Upgrade;
using System;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.RdlObjectModel2008.Upgrade
{
	internal class SerializerHost2008 : SerializerHost2010
	{
		private List<IUpgradeable2008> m_upgradeable;

		private static Type[,] m_substituteTypes = new Type[1, 2]
		{
			{
				typeof(Report),
				typeof(Report2008)
			}
		};

		public List<IUpgradeable2008> Upgradeable2008
		{
			get
			{
				return m_upgradeable;
			}
			set
			{
				m_upgradeable = value;
			}
		}

		public override Type GetSubstituteType(Type type)
		{
			if (!m_serializing)
			{
				for (int i = 0; i < m_substituteTypes.GetLength(0); i++)
				{
					if (type == m_substituteTypes[i, 0])
					{
						return m_substituteTypes[i, 1];
					}
				}
			}
			else
			{
				for (int j = 0; j < m_substituteTypes.GetLength(0); j++)
				{
					if (type == m_substituteTypes[j, 1])
					{
						return m_substituteTypes[j, 0];
					}
				}
			}
			return type;
		}

		public override void OnDeserialization(object value)
		{
			if (m_upgradeable != null && value is IUpgradeable2008)
			{
				m_upgradeable.Add((IUpgradeable2008)value);
			}
			base.OnDeserialization(value);
		}

		public override IEnumerable<ExtensionNamespace> GetExtensionNamespaces()
		{
			return new ExtensionNamespace[1]
			{
				new ExtensionNamespace("rd", "http://schemas.microsoft.com/SQLServer/reporting/reportdesigner")
			};
		}

		public SerializerHost2008(bool serializing)
			: base(serializing)
		{
		}
	}
}
