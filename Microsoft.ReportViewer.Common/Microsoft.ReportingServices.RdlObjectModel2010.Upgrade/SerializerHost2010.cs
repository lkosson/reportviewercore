using Microsoft.ReportingServices.RdlObjectModel;
using Microsoft.ReportingServices.RdlObjectModel.RdlUpgrade;
using Microsoft.ReportingServices.RdlObjectModel.Serialization;
using System;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.RdlObjectModel2010.Upgrade
{
	internal class SerializerHost2010 : SerializerHostBase
	{
		private List<IUpgradeable2010> m_upgradeable;

		private static Type[,] m_substituteTypes = new Type[2, 2]
		{
			{
				typeof(Report),
				typeof(Report2010)
			},
			{
				typeof(StateIndicator),
				typeof(StateIndicator2010)
			}
		};

		public List<IUpgradeable2010> Upgradeable2010
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
			if (m_upgradeable != null && value is IUpgradeable2010)
			{
				m_upgradeable.Add((IUpgradeable2010)value);
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

		public SerializerHost2010(bool serializing)
			: base(serializing)
		{
		}
	}
}
